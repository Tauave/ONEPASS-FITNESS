with SessionStats as (
    select s.Id,
           s.ClassTypeId,
           s.Capacity,
           cast((s.StartTime at time zone 'UTC' at time zone 'New Zealand Standard Time') as datetime2) as LocalStart,
           (select count(*) from Bookings as b where b.ClassSessionId = s.Id) as BookingCount
    from ClassSessions as s
),
MonthlyTotals as (
    select datefromparts(year(ss.LocalStart), month(ss.LocalStart), 1) as MonthStart,
           ct.Name as ClassName,
           count(*) as Sessions,
           sum(ss.BookingCount) as Bookings,
           sum(ss.Capacity) as TotalSpots
    from SessionStats as ss
    inner join ClassTypes as ct on ct.Id = ss.ClassTypeId
    group by datefromparts(year(ss.LocalStart), month(ss.LocalStart), 1), ct.Name
),
Ranked as (
    select MonthStart,
           ClassName,
           Sessions,
           Bookings,
           TotalSpots,
           cast(100.0 * Bookings / nullif(TotalSpots, 0) as decimal(5,1)) as PercentFull,
           rank() over (partition by MonthStart order by Bookings desc) as PopularityRank
    from MonthlyTotals
)
select format(MonthStart, 'MMMM yyyy') as Month,
       PopularityRank,
       ClassName,
       Sessions,
       Bookings,
       TotalSpots,
       PercentFull
from Ranked
order by MonthStart, PopularityRank, ClassName;