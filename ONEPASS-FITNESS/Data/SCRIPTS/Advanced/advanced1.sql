select ct.Name as ClassName,
       FORMAT((s.StartTime AT TIME ZONE 'UTC' AT TIME ZONE 'New Zealand Standard Time'), 'dddd d MMM yyyy') as ClassDate,
       FORMAT((s.StartTime AT TIME ZONE 'UTC' AT TIME ZONE 'New Zealand Standard Time'), 'h:mm tt') as StartsAt,
       FORMAT((DATEADD(MINUTE, ct.DurationMinutes, s.StartTime) AT TIME ZONE 'UTC' AT TIME ZONE 'New Zealand Standard Time'), 'h:mm tt') as EndsAt,
       ct.DurationMinutes,
       s.Capacity
from ClassSessions as s
INNER JOIN ClassTypes as ct on ct.Id = s.ClassTypeId
where s.StartTime > SYSUTCDATETIME()
  AND ct.IsActive = 1
order by s.StartTime;
