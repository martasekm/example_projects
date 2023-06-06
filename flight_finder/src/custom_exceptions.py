# ---------------------------------------------------------------
# Author:
# Martin Mackovík
# +420 774 009 081
# mackovikmartin@gmail.com
# ---------------------------------------------------------------
class FlightFinderError(Exception):
    pass


class CmdLineArgumentError(FlightFinderError):
    pass


class FileOpenError(FlightFinderError):
    pass


class InvalidInputFileError(FlightFinderError):
    pass
