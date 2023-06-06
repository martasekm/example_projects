from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy import Column, Integer, String, Numeric
from sqlalchemy.dialects.postgresql import TIMESTAMP

Base = declarative_base()


class Journey(Base):
    __tablename__ = "journeys_xmackov"
    id = Column(Integer, primary_key=True)
    source_id = Column(String(50))
    dest_id = Column(String(50))
    departure_datetime = Column(TIMESTAMP)
    arrival_datetime = Column(TIMESTAMP)
    carrier = Column(String(50))
    price = Column(Numeric)
    currency = Column(String(3))
