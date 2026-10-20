select ClassTypes.Name, ClassSessions.StartTime, ClassSessions.Capacity
from ClassSessions, ClassTypes
where ClassTypes.Id = ClassSessions.ClassTypeId
order by ClassSessions.StartTime;