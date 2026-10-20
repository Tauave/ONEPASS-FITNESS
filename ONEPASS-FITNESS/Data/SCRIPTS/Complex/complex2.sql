select AspNetUsers.Name, AspNetUsers.Lastname, count(distinct ClassSessions.ClassTypeId) as NumberOfClassTypesBooked
from AspNetUsers, Bookings, ClassSessions
where AspNetUsers.Id = Bookings.UserId and Bookings.ClassSessionId = ClassSessions.Id
group by AspNetUsers.Id, AspNetUsers.Name, AspNetUsers.Lastname
having count(distinct ClassSessions.ClassTypeId) > 1
order by NumberOfClassTypesBooked desc;