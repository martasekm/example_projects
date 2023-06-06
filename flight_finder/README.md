# Example project - Flight finder

10th April, 2022

### Author
Martin Mackovík | tel.: +420 774 009 081 | mackovikmartin@gmail.com

### Functionality

This Python package allows the user to load a list of flights from a CSV file, finds all possible flights between destinations and returns a JSON with found journeys.

### How it works

Arguments are parsed using the argparse package and the input CSV file is loaded using the csv Python package. The input is then serialized into a list of Flight objects and then transformed into a special type, resembling a representation of oriented graph with all possible flight paths (this representation is easier to use in graph discovery funtions).

Then, all possible connections from origin to destination are found using a recursive, depth-first search, starting from the originating airport. Each DFS branch is terminated when there is no possible connecting flight from the current airport or when the destination airport is reached, in which case the current found path is saved into a list of found connections.

Lastly, the result is sorted by price from the lowest, converted to JSON using the json Python package and printed to stdout.

### Usage

(this can be also viewed by using --help flag in command line)

usage: Flights finder [-h] [--bags BAGS] [--return] [--beautify]
                      [--return-turnover RETURN_TURNOVER]
                      filePath originAirport destinationAirport

Parses a CSV of flights and returns a JSON representing all possible paths
between the originating and target airport

positional arguments:
  filePath              path to the input CSV file with flight data
  originAirport         code of the originating airport
  destinationAirport    code of the destination airport

optional arguments:
  -h, --help            show this help message and exit
  --bags BAGS           set the minimum amount of bags to be allowed by all
                        flights from the route.
  --return              search also for the return flights.
  --beautify            pretty-print the output JSON
  --return-turnover RETURN_TURNOVER
                        Minimum turnover hours to stay in the
                        targetdestination before leaving on returnflight.
                        (float number)