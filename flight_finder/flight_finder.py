# ---------------------------------------------------------------
# Author:
# Martin Mackovík
# +420 774 009 081
# mackovikmartin@gmail.com
# ---------------------------------------------------------------
import sys

import src.custom_exceptions
from src.arguments import CmdLineArgs
from src.flight import Flight, FlightFinder


def main() -> None:
    args = CmdLineArgs.parse_arguments_new()
    flights = Flight.load_flights_from_csv(args.input_file_path)
    flight_finder = FlightFinder(flights)
    result_journeys = flight_finder \
        .find_flights(orig=args.origin_airport,
                      dest=args.dest_airport,
                      bags=args.bags_count,
                      is_return_trip=args.return_flight,
                      return_turnover=args.return_turnover)
    print(FlightFinder.generate_output_json(result_journeys, args.beautify))


if __name__ == '__main__':
    try:
        main()
    except src.custom_exceptions.FlightFinderError as err:
        print("ERROR: " + str(err))
        sys.exit(1)
