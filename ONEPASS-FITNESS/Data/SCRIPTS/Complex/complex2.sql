select Member,
       StartWeightKg,
       TargetWeightKg,
       CurrentKg,
       LastLogged,
       Done as ChangeKg,
       TargetWeightKg - CurrentKg as RemainingKg,
       case
           when TotalNeeded = 0 then 100.0
           when Done / TotalNeeded * 100 < 0 then 0.0
           when Done / TotalNeeded * 100 > 100 then 100.0
           else cast(Done / TotalNeeded * 100 as decimal(5,1))
       end as PercentToGoal,
       case
           when TargetDate is null then N'No target date'
           when TargetDate < cast(getdate() as date) then N'Target date passed'
           else cast(datediff(day, cast(getdate() as date), TargetDate) as nvarchar(10)) + N' days left'
       end as Deadline
from GoalProgress
order by PercentToGoal desc, Member;