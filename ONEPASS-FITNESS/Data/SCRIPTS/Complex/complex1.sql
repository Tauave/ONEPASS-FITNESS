select AspNetUsers.Name, AspNetUsers.Lastname, year(ClassSessions.StartTime) as ClassYear, month(ClassSessions.StartTime) as ClassMonth, count(Bookings.Id) as ClassesBooked
from Bookings, AspNetUsers, ClassSessions
where Bookings.UserId = AspNetUsers.Id and Bookings.ClassSessionId = ClassSessions.Id
group by AspNetUsers.Id, AspNetUsers.Name, AspNetUsers.Lastname, year(ClassSessions.StartTime), month(ClassSessions.StartTime)
order by ClassYear desc, ClassMonth desc, ClassesBooked desc;