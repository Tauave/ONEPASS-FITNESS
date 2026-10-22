select concat(AspNetUsers.Name, ' ', AspNetUsers.Lastname) as Member, avg(WeightEntries.WeightKg) as AverageWeight
from WeightEntries, AspNetUsers
where AspNetUsers.Id = WeightEntries.UserId
group by AspNetUsers.Id, AspNetUsers.Name, AspNetUsers.Lastname
having count(WeightEntries.Id) >= 2
order by Member;
