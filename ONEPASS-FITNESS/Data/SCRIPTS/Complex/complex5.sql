select count(Bookings.Id) as TotalBookings, month(ClassSessions.StartTime) as "Month", year(ClassSessions.StartTime) as "Year"
from Bookings, ClassSessions
where ClassSessions.Id = Bookings.ClassSessionId
group by month(ClassSessions.StartTime), year(ClassSessions.StartTime)
order by "Year" asc, "Month" asc;