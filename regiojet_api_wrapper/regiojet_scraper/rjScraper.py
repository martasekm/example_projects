import json.decoder
import sys
from datetime import timedelta
from typing import Any, List

import requests
from redis import Redis
from slugify import slugify

LOCATIONS_RJ_ENDPOINT = 'https://brn-ybus-pubapi.sa.cz/restapi/consts/locations'
ROUTES_RJ_ENDPOINT = 'https://brn-ybus-pubapi.sa.cz/restapi/routes/search/simple'


class RjScraper:
    def __init__(self):
        """
        Initializes the scraper object and preloads some data from API
        (e.g. CityID to CityName dictionaries and vice versa)
        """
        self.redis_obj = Redis(host="PLACEHOLDER",
                               password="PLACEHOLDER",
                               charset="utf-8",
                               decode_responses=True)
        self.fetched_locations = self.fetch_all_locations()  # List of all bus and train stops
        self.cities_by_name = self.fetch_all_cities_by_name()  # Dictionary CityName -> CityID
        self.cities_by_ids = self.fetch_all_cities_by_ids()  # Dictionary CityID -> CityName
        self.stations_by_id = self.fetch_all_stations_by_id()  # Dictionary StationID -> StationName

    def fetch_all_locations(self) -> dict:
        """
        Fetches all locations from either Redis or RJ API and returns a dict.
        :return: API response
        """
        redis_key = "xmackov:rj:locations"
        if self.is_redis_valid(redis_key) and self.redis_obj.exists(redis_key):
            print("LOADING LOCATIONS FROM REDIS", file=sys.stderr)
            redis_str = self.redis_obj.get("xmackov:rj:locations", )
            return json.decoder.JSONDecoder().decode(redis_str)
        res = requests.get(LOCATIONS_RJ_ENDPOINT)

        if self.is_redis_valid(redis_key):
            self.redis_obj.setex(name=redis_key,
                                 time=timedelta(minutes=1),
                                 value=res.content)
        print("LOADING LOCATIONS FROM SERVER (NOT REDIS)", file=sys.stderr)
        return res.json()

    @staticmethod
    def is_redis_valid(ksey: str) -> bool:
        """
        Checks if the Redis cache is running and containing the expected master key
        :param key: Key to check for
        :return: Whether Redis cache server is running
        """
        return False  # PLACEHOLDER: Redis caching database currently not running
        # try:
        #     redis_res = self.redis_obj.get(key)
        #     if redis_res is None:
        #         return False
        #     json.decoder.JSONDecoder().decode(redis_res)
        # except json.JSONDecodeError:
        #     return False
        # return True

    def fetch_all_cities_by_name(self) -> dict:
        """
        Fetches all cities from either Redis or RJ API and returns a dict.
        :return: Dictionary; keys=[city name / alias], value=[city]
        """
        redis_key = "xmackov:rj:all_cities_by_names"
        if self.is_redis_valid(redis_key) and \
                self.redis_obj.exists(redis_key):
            redis_res = self.redis_obj.get(redis_key)
            return json.decoder.JSONDecoder().decode(redis_res)
        result = {}
        for country in self.fetched_locations:
            for city in country["cities"]:
                result[slugify(city["name"])] = city
                for alias in city["aliases"]:
                    result[slugify(alias)] = city
        if self.is_redis_valid(redis_key):
            self.redis_obj.setex(name=redis_key,
                                 value=json.encoder.JSONEncoder().encode(result),
                                 time=timedelta(minutes=1))
        return result

    def fetch_all_cities_by_ids(self) -> dict:
        """
        Fetches all cities from either Redis or RJ API and returns a dict.
        :return: Dictionary; keys=[city id], value=[city]
        """
        redis_key = "xmackov:rj:all_cities_by_ids"
        if self.is_redis_valid(redis_key) and self.redis_obj.exists(redis_key):
            redis_res = self.redis_obj.get(redis_key)
            return json.decoder.JSONDecoder().decode(redis_res)
        result = {}
        for country in self.fetched_locations:
            for city in country["cities"]:
                result[city["id"]] = city
        if self.is_redis_valid(redis_key):
            self.redis_obj.setex(name=redis_key,
                                 value=json.encoder.JSONEncoder().encode(result),
                                 time=timedelta(minutes=1))
        return result

    def fetch_all_stations_by_id(self) -> dict:
        """
        Fetches all stations from either Redis or RJ API and returns a dict.
        :return: Dictionary; keys=[station id], value=[station]
        """
        redis_key = "xmackov:rj:all_stations_by_id"
        if self.is_redis_valid(redis_key) and self.redis_obj.exists(redis_key):
            redis_res = self.redis_obj.get(redis_key)
            return json.decoder.JSONDecoder().decode(redis_res)
        result = {}
        for country in self.fetched_locations:
            for city in country["cities"]:
                for station in city["stations"]:
                    result[station["id"]] = station
        if self.is_redis_valid(redis_key):
            self.redis_obj.setex(name=redis_key,
                                 value=json.encoder.JSONEncoder().encode(result),
                                 time=timedelta(minutes=1))
        return result

    def fetch_cities_by_name(self, city_name: str) -> List[Any]:
        """
        Returns the IDs of the cities that have the specified name
        :param city_name: Name of the city to search
        :return: API response of the cities
        """
        all_names = [dict_key for dict_key in self.cities_by_name.keys()
                     if slugify(city_name) in slugify(dict_key)]
        result = []
        for name in all_names:
            result.append(self.cities_by_name[name])
        return result

    def fetch_station_by_id(self, station_id: str) -> Any:
        """
        Returns the name of the station specified by ID
        :param station_id: ID of the station
        :return: Name of the station
        """
        return self.stations_by_id[station_id]

    def get_routes_between_cities(self, origin_id: str, target_id: str) -> Any:
        """
        Finds routes between the specified cities
        :param origin_id: ID of the origin city
        :param target_id: ID of the target city
        :return: API response of routes between cities
        """
        redis_key = f"xmackov:rj:route:{origin_id}:{target_id}"
        if self.is_redis_valid(redis_key) and self.redis_obj.exists(redis_key):
            print("LOADING CONNECTIONS FROM REDIS", file=sys.stderr)
            redis_res = self.redis_obj \
                .get(redis_key)
            return json.decoder.JSONDecoder().decode(redis_res)
        routes = requests.get(ROUTES_RJ_ENDPOINT,
                              {
                                  "tariffs": "REGULAR",
                                  "fromLocationType": "CITY",
                                  "fromLocationId": int(origin_id),
                                  "toLocationType": "CITY",
                                  "toLocationId": int(target_id),
                                  "departureDate": "2023-06-06"
                              })
        if self.is_redis_valid(redis_key):
            self.redis_obj.setex(name=redis_key,
                                 value=routes.content,
                                 time=timedelta(minutes=1))
        print("LOADING CONNECTIONS FROM SERVER (NOT REDIS)", file=sys.stderr)
        return routes.json()
