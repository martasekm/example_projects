# ---------------------------------------------------------------
# Author:
# Martin Mackovík
# +420 774 009 081
# mackovikmartin@gmail.com
# ---------------------------------------------------------------
import csv
import json
from collections import deque
from datetime import datetime, timedelta
from typing import Dict, List, Tuple, Set, Deque, Any, Optional

from . import custom_exceptions

DEFAULT_MIN_TURNOVER = timedelta(hours=1)
DEFAULT_MAX_TURNOVER = timedelta(hours=6)

FLIGHTS_GRAPH = Dict[str, List[Tuple[str, 'Flight']]]
"""
FLIGHTS_GRAPH is basically a graph representation of all the possible flights,
used by some functions (find_flights) because it makes performing depth-first
search easier. Keys are the source nodes (= origin airports) and values
are lists of outer edges going from that node to other nodes (= to destination
airports).

Let's say there are following flights:
    A -> B,
    A -> C,
    B -> C,
    B -> A,
    C -> B

Then the corresponding FLIGHTS_GRAPH will look like this:
    {
        A: [(B, flight_A_to_B), (C, flight_A_to_C)],
        B: [(C, flight_B_to_C), (A, flight_B_to_A)],
        C: [(B, flight_C_to_B)]
    }
"""

TRIPS = List[Tuple[float, str, str, List['Flight']]]
"""
TRIPS represents the resulting type of all resulting flight
connections.
"""


class Flight:
    """
    Class representing a single direct flight.
    """

    def __init__(self, number: str, origin: str, dest: str,
                 depart_time: datetime, arrival_time: datetime, price: float,
                 bag_price: int, bags_allowed: int):
        self.number = number
        self.orig = origin
        self.dest = dest
        self.depart_time = depart_time
        self.arrive_time = arrival_time
        self.price = price
        self.bag_price = bag_price
        self.bags_allowed = bags_allowed

    @staticmethod
    def _parse_csv_line(csv_line: Dict[str, str]) -> 'Flight':
        """
        Parses a dictionary representing a single CSV input line into
        the Flight object.
        @param csv_line: Dictionary representing a CSV line from the input file
        @return: Flight object
        """
        if len(csv_line) != 8:
            raise custom_exceptions \
                .InvalidInputFileError("Invalid input file format.")
        number, orig, dest = csv_line['flight_no'], csv_line['origin'], \
                             csv_line['destination']
        depart_time = datetime.fromisoformat(csv_line['departure'])
        arrive_time = datetime.fromisoformat(csv_line['arrival'])
        price = float(csv_line['base_price'])
        bag_price = int(csv_line['bag_price'])
        bags_allowed = int(csv_line['bags_allowed'])
        return Flight(number, orig, dest, depart_time, arrive_time, price,
                      bag_price, bags_allowed)

    @staticmethod
    def load_flights_from_csv(file_path: str) -> List['Flight']:
        """
        Loads all flights from the supplied CSV file.
        @param file_path: Path to the CSV file with flights
        @return: List of all flights from the input CSV file
        """
        result: List['Flight'] = []
        try:
            with open(file_path, mode='r') as csv_file:
                csv_reader = csv.DictReader(csv_file)
                for line in csv_reader:
                    result.append(Flight._parse_csv_line(line))
            return result
        except OSError:
            raise custom_exceptions.FileOpenError("Could not open the "
                                                  "provided file.")
        except custom_exceptions.InvalidInputFileError:
            raise
        except Exception:
            raise custom_exceptions.InvalidInputFileError("Invalid input "
                                                          "file format.")

    def __str__(self) -> str:
        return "Flight {number}: {orig}-{dest}, departure {depart}, " \
               "arrival {arrive}".format(number=self.number,
                                         orig=self.orig,
                                         dest=self.dest,
                                         depart=self.depart_time,
                                         arrive=self.arrive_time)


class FlightJourney:
    """
    Class representing a single journey between airports, can constitute
    of multiple flights.
    """

    def __init__(self, flights: List['Flight'], origin: str, destination: str, bags_count: int = 1):
        assert flights
        self.flights = flights
        self.origin = origin
        self.dest = destination
        self.bags_count = bags_count

    @property
    def journey_price(self) -> float:
        return sum(flight.price for flight in self.flights)

    @property
    def max_bags_count(self) -> int:
        return min(flight.bags_allowed for flight in self.flights)

    @property
    def total_travel_time(self) -> timedelta:
        assert self.flights
        return self.flights[-1].arrive_time - self.flights[0].depart_time


class FlightFinder:
    """
    Class used for finding trips between airports. It is initialized with
    a list of all flights and values of minimum and maximum turnover. Trips
    themselves are then found using the find_flights function.
    """

    def __init__(self, flights: List['Flight'],
                 min_turnover: timedelta = DEFAULT_MIN_TURNOVER,
                 max_turnover: timedelta = DEFAULT_MAX_TURNOVER):
        """
        Initializes a flight finder object.
        :param flights: List of all possible flights
        :param min_turnover: Minimum turnover at transfer airports
        :param max_turnover: Maximum turnover at transfer airports
        """
        self._flights = flights
        self._airports = self._get_flights_graph(flights)
        self.min_turnover = min_turnover
        self.max_turnover = max_turnover

    @staticmethod
    def _get_flights_graph(flights: List['Flight']) -> FLIGHTS_GRAPH:
        """
        Transforms a list of flights to a graph representation.
        (see documentation of the FLIGHTS_GRAPH type)

        @param flights: List of flights
        @return: Graph representation of all possible flight paths.
        """
        result: FLIGHTS_GRAPH = {}
        for flight in flights:
            result[flight.orig] = result.get(flight.orig, []) + \
                                  [(flight.dest, flight)]
        return result

    def find_flights(self, orig: str, dest: str, bags: int = 0,
                     is_return_trip: bool = False, return_turnover: int = 12) \
            -> List['FlightJourney']:
        """
        Finds all possible connections between the specified airports
        and sorts them by their total price from the cheapest.
        @param orig: Originating airport code
        @param dest: Destination airport code
        @param bags: Minimum allowed number of bags for every flight
        @param return_turnover: Minimum count of hours to stay
        at the destination before leaving of the return trip flight
        @param is_return_trip: Whether to search for the return trip or not
        @return: List of all possible flight journeys between the specified
        airports, sorted by the total price of the trip from the cheapest
        to the most expensive.
        """
        # first, let's found all outbound trips from origin to destination
        outbound_trips: TRIPS = []
        self._find_flights_subroutine(orig, dest, set(), deque(),
                                      outbound_trips, bags, None)
        full_trips: TRIPS = []
        if is_return_trip:
            return_trips: TRIPS = []
            # find trips in reverse direction (return trips)
            self._find_flights_subroutine(dest, orig, set(), deque(),
                                          return_trips, bags, None)
            # lastly, pair each outbound trip with all possible return trips
            for out_price, out_orig, out_dest, out_flights in outbound_trips:
                for ret_price, _, _, ret_flights in return_trips:
                    dest_turnover = ret_flights[0].depart_time - \
                                    out_flights[-1].arrive_time
                    if dest_turnover <= timedelta(0) or \
                            dest_turnover < timedelta(hours=return_turnover):
                        # Return trip doesn't connect to outbound trip
                        continue
                    full_trips.append(
                        (out_price + ret_price, out_orig, out_dest,
                         out_flights + ret_flights))
        else:
            full_trips = outbound_trips
        # sort, finalize and return
        full_trips.sort(key=(lambda x: x[0]))
        result_journeys = [FlightJourney(flights, orig, dest, bags)
                           for _, orig, dest, flights in full_trips]
        return result_journeys

    def _find_flights_subroutine(self, orig: str, dest: str,
                                 visited_airports: Set[str],
                                 current_flights: Deque['Flight'],
                                 result_flights: TRIPS,
                                 minimum_bags: int,
                                 after: Optional[datetime]) -> None:
        # Uses recursive depth-first search to find all possible routes between
        # the originating and the destination airport.
        #
        # This function modifies its arguments (visited_airports,
        # current_flights, result_flights). As such, the function
        # isn't clean, and it's not a good idea to call it directly.
        if orig == dest:
            # recursion base case = a single complete connection has been found
            price_sum = FlightFinder.get_flight_price_sum(
                list(current_flights))
            result_flights.append((price_sum, current_flights[0].orig,
                                   dest, list(current_flights)))
            return
        visited_airports.add(orig)
        out_flight: 'Flight'
        for next_stop, out_flight in self._airports.get(orig, []):
            # iterate through all outgoing flights from the current airport
            if after is not None and out_flight.depart_time <= after:
                continue
            if next_stop in visited_airports:
                continue
            if current_flights \
                    and not self._enough_turnover_time(current_flights[-1],
                                                       out_flight):
                continue
            if out_flight.bags_allowed < minimum_bags:
                continue
            current_flights.append(out_flight)
            self._find_flights_subroutine(next_stop, dest, visited_airports,
                                          current_flights, result_flights,
                                          minimum_bags, after)
            current_flights.pop()
        visited_airports.remove(orig)

    def _enough_turnover_time(self, arriving_flight: 'Flight',
                              connection_flight: 'Flight') -> bool:
        """
        Checks if there is enough turnover time between the arriving flight
        and connection flight at the transfer airport.
        :param arriving_flight: Arriving flight
        :param connection_flight: Connection flight
        :return: True if there is enough time, False otherwise
        """
        arrival_time = arriving_flight.arrive_time
        departure_time = connection_flight.depart_time
        turnover = departure_time - arrival_time
        return self.min_turnover <= turnover <= self.max_turnover

    @staticmethod
    def get_flight_price_sum(flights: List['Flight']) -> float:
        """
        Calculates the total price sum of a list of flights.
        :param flights: List of flights to calculate the sum of
        :return: Sum of prices of all the flights
        """
        result: float = 0
        for flight in flights:
            result += flight.price
        return result

    @staticmethod
    def generate_output_json(connections: List['FlightJourney'],
                             beautify: bool = False) -> str:
        """
        Generates output json from the found connections.
        :param connections: Connections to generate a json from
        :param beautify: True for beautified output, False otherwise
        :return: Formatted JSON
        """
        if not connections:
            return "[]"
        indent = 4 if beautify else None
        return json.dumps(connections, cls=FlightsJsonEncoder, indent=indent)


class FlightsJsonEncoder(json.JSONEncoder):
    """
    Helper class used by the JSONEncoder.
    """

    def default(self, o: Any) -> Any:
        if isinstance(o, Flight):
            return {
                'flight_no': o.number,
                'origin': o.orig,
                'destination': o.dest,
                'departure': o.depart_time,
                'arrival': o.arrive_time,
                'base_price': o.price,
                'bag_price': o.bag_price,
                'bags_allowed': o.bags_allowed
            }
        if isinstance(o, FlightJourney):
            return {
                'flights': o.flights,
                'bags_allowed': o.max_bags_count,
                'bags_count': o.bags_count,
                'destination': o.dest,
                'origin': o.origin,
                'total_price': o.journey_price,
                'travel_time': o.total_travel_time
            }
        if isinstance(o, timedelta):
            # the following code is used to prevent time-delta's default
            # string formatting, which returns:
            #       n days, HH:MM:SS
            # when timedelta >= 24 hours. The following formats it always as:
            #       HH:MM:SS
            total_secs = o.total_seconds()
            res = divmod(total_secs, 3600)
            hours, remaining = int(res[0]), int(res[1])
            res = divmod(remaining, 60)
            minutes, seconds = int(res[0]), int(res[1])
            return f"{hours:02d}:{minutes:02d}:{seconds:02d}"
        if isinstance(o, datetime):
            return o.isoformat()
        return json.JSONEncoder.default(self, o)
