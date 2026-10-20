select Id, ClassTypeId, StartTime as StartTimeUtc, Capacity
from ClassSessions
where StartTime > SYSDATETIMEOFFSET()
Order by StartTime;