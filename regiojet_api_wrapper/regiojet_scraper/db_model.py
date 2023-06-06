from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy import Sequence, Column, Integer, String, TEXT, NUMERIC
from sqlalchemy.dialects.postgresql import TIMESTAMP

Base = declarative_base()


class Journey(Base):
    # name of the table
    __tablename__ = "journeys_xmackov"
    id = Column(Integer, primary_key=True)
    source = Column(String(255))
    destination = Column(String(255))
    departure_datetime = Column(TIMESTAMP)
    arrival_datetime = Column(TIMESTAMP)
    carrier = Column(String(255))
    vehicle_type = Column(String(255))
    price = Column(NUMERIC)
    currency = Column(String(3))
