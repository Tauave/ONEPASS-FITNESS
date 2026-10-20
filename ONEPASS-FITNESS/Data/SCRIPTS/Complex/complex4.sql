select ClassTypes.Name as ClassName, count(distinct ClassSessions.Id) as SessionsBooked, count(Bookings.Id) as TotalBookings
from Bookings, ClassSessions, ClassTypes
where Bookings.ClassSessionId = ClassSessions.Id
and ClassSessions.ClassTypeId = ClassTypes.Id
group by ClassTypes.Name
order by TotalBookings desc;