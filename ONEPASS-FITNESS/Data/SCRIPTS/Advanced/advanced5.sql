select count(Bookings.Id) as TotalYogaBookings
from Bookings, ClassSessions, ClassTypes
where Bookings.ClassSessionId = ClassSessions.Id and ClassSessions.ClassTypeId = ClassTypes.Id
and ClassTypes.Name = 'Yoga';