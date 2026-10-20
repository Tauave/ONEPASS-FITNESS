select format((s.StartTime at time zone 'UTC' at time zone 'New Zealand Standard Time'), 'ddd d MMM yyyy h:mm tt') as ClassTime,
       ct.Name as ClassName,
       row_number() over (partition by s.Id order by b.Id) as SpotNumber,
       coalesce(u.Name + N' ' + u.Lastname, N'(account not found)') as Member,
       u.Email,
       u.PhoneNumber,
       count(*) over (partition by s.Id) as TotalBooked,
       s.Capacity,
       s.Capacity - count(*) over (partition by s.Id) as SpotsLeft
from ClassSessions as s
inner join ClassTypes as ct on ct.Id = s.ClassTypeId
inner join Bookings as b on b.ClassSessionId = s.Id
left join AspNetUsers as u on u.Id = b.UserId
where s.StartTime > sysutcdatetime()
order by s.StartTime, SpotNumber;