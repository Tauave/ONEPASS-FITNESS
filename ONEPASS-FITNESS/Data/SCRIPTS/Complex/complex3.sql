select concat(AspNetUsers.Name, ' ', AspNetUsers.Lastname) as Member, WeightGoals.StartWeightKg, WeightGoals.TargetWeightKg, min(WeightEntries.WeightKg) as LowestKg, max(WeightEntries.WeightKg) as HighestKg, count(WeightEntries.Id) as EntriesLogged
from WeightGoals, AspNetUsers, WeightEntries
where WeightGoals.UserId = AspNetUsers.Id
and WeightEntries.UserId = AspNetUsers.Id
and WeightEntries.Date >= dateadd(month, -3, getdate())
group by AspNetUsers.Id, AspNetUsers.Name, AspNetUsers.Lastname, WeightGoals.StartWeightKg, WeightGoals.TargetWeightKg
order by Member;