select s.Id as SessionId,
       ct.Name as ClassName,
       FORMAT((s.StartTime AT TIME ZONE 'UTC' AT TIME ZONE 'New Zealand Standard Time'), 'ddd d MMM h:mm tt') as StartsAt,
       s.Capacity,
       COUNT(b.Id) as Booked,
       s.Capacity - COUNT(b.Id) as SpotsLeft
from ClassSessions as s
INNER JOIN ClassTypes as ct on ct.Id = s.ClassTypeId
LEFT JOIN Bookings as b on b.ClassSessionId = s.Id
where s.StartTime > SYSUTCDATETIME()
group by s.Id, ct.Name, s.StartTime, s.Capacity
order by s.StartTime;
