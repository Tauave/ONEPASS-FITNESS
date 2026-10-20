use [ONEPASS-FITNESS];
go

select Date, WeightKg, Note
from WeightEntries
where WeightKg < 100
order by Date;