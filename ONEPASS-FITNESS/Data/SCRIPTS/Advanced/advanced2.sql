select ClassTypes.Name as ClassName, ClassSessions.StartTime, ClassSessions.Capacity, count(Bookings.Id) as Booked, ClassSessions.Capacity - count(Bookings.Id) as SpotsLeft
from ClassSessions, ClassTypes, Bookings
where ClassTypes.Id = ClassSessions.ClassTypeId and ClassSessions.Id = Bookings.ClassSessionId
group by ClassSessions.Id, ClassTypes.Name, ClassSessions.StartTime, ClassSessions.Capacity
order by ClassSessions.StartTime;