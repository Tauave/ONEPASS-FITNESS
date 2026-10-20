select ct.Name as ClassName,
       ct.IsActive,
       COUNT(DISTINCT s.Id) as Sessions,
       COUNT(b.Id) as TotalBookings,
       CAST(1.0 * COUNT(b.Id) / NULLIF(COUNT(DISTINCT s.Id), 0) as decimal(5,1)) as AvgPeoplePerSession
from ClassTypes as ct
LEFT JOIN ClassSessions as s on s.ClassTypeId = ct.Id
LEFT JOIN Bookings as b on b.ClassSessionId = s.Id
group by ct.Id, ct.Name, ct.IsActive
order by TotalBookings desc, ct.Name;
