import argparse

class CmdLineArgs:
    def __init__(self, orig_city: str, dest_city: str):
        self.orig_city = orig_city
        self.dest_city = dest_city

    @staticmethod
    def parse_arguments_new() -> 'CmdLineArgs':
        parser = argparse.ArgumentParser(
            prog="RegioJet path finder",
            description="Finds all RJ routes between the speified cities.")
        parser.add_argument("origin",
                            help="Name of the originating city",
                            metavar="originCityName")
        parser.add_argument("destination",
                            help="Name of the destination city",
                            metavar="destCityName")
        parsed_args = vars(parser.parse_args())
        return CmdLineArgs(orig_city=parsed_args["origin"],
                           dest_city=parsed_args["destination"])
