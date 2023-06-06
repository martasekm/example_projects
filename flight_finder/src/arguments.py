# ---------------------------------------------------------------
# Author:
# Martin Mackovík
# +420 774 009 081
# mackovikmartin@gmail.com
# ---------------------------------------------------------------

import argparse
import pathlib

from . import custom_exceptions


class CmdLineArgs:
    def __init__(self, input_file_path: str, origin_airport: str,
                 dest_airport: str, bags_count: int = 0,
                 return_flight: bool = False, beautify: bool = False,
                 return_turnover: int = 0):
        self.input_file_path = input_file_path
        self.origin_airport = origin_airport
        self.dest_airport = dest_airport
        self.bags_count = bags_count
        self.return_flight = return_flight
        self.beautify = beautify
        self.return_turnover = return_turnover

    @staticmethod
    def parse_arguments_new() -> 'CmdLineArgs':
        parser = argparse.ArgumentParser(
            prog="Flights finder",
            description="Parses a CSV of flights and returns a JSON "
                        "representing all possible paths between "
                        "the originating and target airport")
        parser.add_argument("file_path",
                            help="path to the input CSV file with flight data",
                            metavar="filePath", type=pathlib.Path)
        parser.add_argument("origin",
                            help="code of the originating airport",
                            metavar="originAirport")
        parser.add_argument("destination",
                            help="code of the destination airport",
                            metavar="destinationAirport")
        parser.add_argument("--bags", required=False, default=0,
                            help="set the minimum amount of bags to be "
                                 "allowed by all flights from the route.",
                            dest='bags',
                            type=int)
        parser.add_argument("--return", action='store_true',
                            required=False, dest='return',
                            help="search also for the return flights.")
        parser.add_argument("--beautify", action='store_true',
                            required=False, dest='beautify', default=False,
                            help="pretty-print the output JSON")
        parser.add_argument("--return-turnover", required=False, type=float,
                            default='12', dest='return_turnover',
                            help="Minimum turnover hours to stay in the target"
                                 "destination before leaving on return"
                                 "flight. (float number)")
        parsed_args = vars(parser.parse_args())
        if parsed_args['bags'] < 0:
            raise custom_exceptions.CmdLineArgumentError(
                "Bags count can't be a negative number")
        if parsed_args['origin'] == parsed_args['destination']:
            raise custom_exceptions.CmdLineArgumentError(
                "Origin airport cannot be the same as the destination.")
        if parsed_args['return_turnover'] < 0:
            raise custom_exceptions.CmdLineArgumentError(
                "Return trip turnover can't be a negative number."
            )
        return CmdLineArgs(input_file_path=parsed_args['file_path'],
                           origin_airport=parsed_args['origin'],
                           dest_airport=parsed_args['destination'],
                           bags_count=parsed_args['bags'],
                           return_flight=parsed_args['return'],
                           beautify=parsed_args['beautify'],
                           return_turnover=parsed_args['return_turnover'])
