select u.Name + N' ' + u.Lastname as Member,
       u.Email,
       COUNT(*) as Entries,
       MIN(we.Date) as FirstLogged,
       MAX(we.Date) as LastLogged,
       MIN(we.WeightKg) as LowestKg,
       MAX(we.WeightKg) as HighestKg,
       CAST(AVG(we.WeightKg) as decimal(6,2)) as AverageKg
from WeightEntries as we
INNER JOIN AspNetUsers as u on u.Id = we.UserId
group by u.Id, u.Name, u.Lastname, u.Email
having COUNT(*) >= 2
order by Entries desc;
