use [ONEPASS-FITNESS];
go

select Name, Description, DurationMinutes
from ClassTypes
where IsActive = 1	
order by DurationMinutes desc;