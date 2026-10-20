use [ONEPASS-FITNESS];
go

select AspNetUsers.Name, AspNetUsers.Lastname, year(ClassSessions.StartTime at time zone 'UTC' at time zone 'New Zealand Standard Time') as ClassYear, month(ClassSessions.StartTime at time zone 'UTC' at time zone 'New Zealand Standard Time') as ClassMonth, count(Bookings.Id) as ClassesBooked
from Bookings, AspNetUsers, ClassSessions
where Bookings.UserId = AspNetUsers.Id and Bookings.ClassSessionId = ClassSessions.Id
group by AspNetUsers.Id, AspNetUsers.Name, AspNetUsers.Lastname, year(ClassSessions.StartTime at time zone 'UTC' at time zone 'New Zealand Standard Time'), month(ClassSessions.StartTime at time zone 'UTC' at time zone 'New Zealand Standard Time')
order by ClassYear desc, ClassMonth desc, ClassesBooked desc;