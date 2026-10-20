select me.Member,
       me.Date,
       me.WeightKg,
       me.WeightKg - lag(me.WeightKg) over (partition by me.UserId order by me.Date) as ChangeSinceLastKg,
       cast(avg7.AverageKg as decimal(6,2)) as SevenDayAverageKg,
       avg7.EntriesInWindow
from MemberEntries as me
cross apply (
    select avg(w2.WeightKg) as AverageKg,
           count(*) as EntriesInWindow
    from WeightEntries as w2
    where cast(w2.UserId as nvarchar(450)) = me.UserId
      and w2.Date between dateadd(day, -6, me.Date) and me.Date
) as avg7
order by me.Member, me.Date;