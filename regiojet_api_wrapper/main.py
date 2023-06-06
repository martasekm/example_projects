import datetime
from typing import Any

from fastapi import FastAPI, HTTPException
from fastapi.responses import JSONResponse

import regiojet_scraper.rjScraper
from args import CmdLineArgs

app = FastAPI()


@app.get("/search")
def search(origin, destination):
    """
    Searches the route between the origin and destination city
    :param origin: Origin city name
    :param destination: Destination city name
    :return: JSON object - API response with routes
    """
    scraper = regiojet_scraper.rjScraper.RjScraper()
    failed = False
    origin_fetched = scraper.fetch_cities_by_name(origin)
    target_fetched = scraper.fetch_cities_by_name(destination)
    if len(origin_fetched) != 1:
        print("Multiple possible origin cities found")
        failed = True
    if len(target_fetched) != 1:
        print("Multiple possible target cities found")
        failed = True
    if failed:
        raise HTTPException(status_code=400, detail="Multiple possible cities / none cities found")
    res = scraper.get_routes_between_cities(origin_fetched[0]['id'], target_fetched[0]['id'])
    return JSONResponse(res)


def print_routes(result: Any, scraper) -> None:
    """
    Pretty prints the found routes from API response
    :param result: API response with routes
    :param scraper: Scraper obj. instance (used for translating station IDs to names)
    :return:
    """
    print("- ROUTES")
    for route in result["routes"]:
        print("--- ROUTE")
        print(f"----- Departure station: {scraper.fetch_station_by_id(str(route['departureStationId']))['fullname']}")
        print(f"----- Arrival station: {scraper.fetch_station_by_id(str(route['arrivalStationId']))['fullname']}")
        print(f"----- Departure time: {datetime.datetime.fromisoformat(route['departureTime'])}")
        print(f"----- Arrival time: {datetime.datetime.fromisoformat(route['arrivalTime'])}")
        print(f"----- Free seats: {route['freeSeatsCount']}")


def main():
    """
    Loads the user arguments (origin and destination city), queries the
    API and prints the resulting routes.
    :return:
    """
    args = CmdLineArgs.parse_arguments_new()
    origin = args.orig_city
    target = args.dest_city
    result = search(origin, target)
    print(result.body)


if __name__ == '__main__':
    main()
