from sqlalchemy import create_engine, cast, Date
from sqlalchemy.orm.session import Session
from sqlalchemy.pool import NullPool
from datetime import datetime, date
from typing import List
import regiojet_scraper.db_model as db_model

DB_HOST = "sql.pythonweekend.skypicker.com"
DB_DBNAME = "pythonweekend"
DB_USER = "pythonweekend"
DB_PWD = "2oggOq1e2buHENZa"
DB_URL = f"postgresql://pythonweekend:2oggOq1e2buHENZa@sql.pythonweekend.skypicker.com/pythonweekend"


class DatabaseAccesser:
    def __init__(self):
        self.engine = create_engine(DB_URL, echo=True, poolclass=NullPool)

    def write_journey(self, journey: db_model.Journey) -> None:
        with Session(self.engine) as session:
            session.add(journey)
            session.commit()

    def get_all_journeys(self):
        with Session(self.engine) as session:
            results = session.query(db_model.Journey.source, db_model.Journey.destination).all()
            return results

    def get_journeys_by_origin_destination(self, source_id: str, target_id: str,
                                           departure_date: datetime) -> List[db_model.Journey]:
        with Session(self.engine) as session:
            results = session.query(db_model.Journey).filter(
                db_model.Journey.source == str(source_id),
                db_model.Journey.destination == str(target_id),
                cast(db_model.Journey.departure_datetime, Date) == date(day=departure_date.day,
                                                                        month=departure_date.month,
                                                                        year=departure_date.year)
            ).all()
            for result in results:
                print(result.departure_datetime)
