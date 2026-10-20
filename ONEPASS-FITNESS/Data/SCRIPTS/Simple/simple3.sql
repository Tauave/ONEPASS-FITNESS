use [ONEPASS-FITNESS];
go

select ClassTypeId, StartTime, Capacity
from ClassSessions
where StartTime > getutcdate()
order by StartTime;