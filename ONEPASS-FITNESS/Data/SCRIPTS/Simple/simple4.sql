select UserId, Date, WeightKg, Note
from WeightEntries
where Date >= DateAdd(day, -30, Cast(GetDate() as date))
order by Date desc;