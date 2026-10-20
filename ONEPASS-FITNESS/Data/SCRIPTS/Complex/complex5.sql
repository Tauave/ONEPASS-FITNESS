with Problems as (
    select N'1. Session over capacity' as Problem,
           cast(s.Id as nvarchar(450)) as RecordId,
           concat(count(b.Id), N' booked, capacity is ', s.Capacity) as Detail
    from ClassSessions as s
    inner join Bookings as b on b.ClassSessionId = s.Id
    group by s.Id, s.Capacity
    having count(b.Id) > s.Capacity

    union all

    select N'2. Two weight entries on the same date',
           cast(we.UserId as nvarchar(450)),
           concat(count(*), N' entries on ', convert(nvarchar(10), we.Date, 23))
    from WeightEntries as we
    group by cast(we.UserId as nvarchar(450)), we.Date
    having count(*) > 1

    union all

    select N'3. Booking for a missing account',
           cast(b.Id as nvarchar(450)),
           concat(N'UserId ', b.UserId, N' in session ', b.ClassSessionId)
    from Bookings as b
    where not exists (select 1 from AspNetUsers as u where u.Id = b.UserId)

    union all

    select N'4. Weight entry for a missing account',
           cast(we.Id as nvarchar(450)),
           concat(N'UserId ', cast(we.UserId as nvarchar(450)))
    from WeightEntries as we
    where not exists (select 1 from AspNetUsers as u where u.Id = we.UserId)

    union all

    select N'5. Weight outside 20 to 500 kg',
           cast(we.Id as nvarchar(450)),
           concat(we.WeightKg, N' kg on ', convert(nvarchar(10), we.Date, 23))
    from WeightEntries as we
    where we.WeightKg < 20 or we.WeightKg > 500

    union all

    select N'6. Weight entry dated in the future',
           cast(we.Id as nvarchar(450)),
           concat(N'Dated ', convert(nvarchar(10), we.Date, 23))
    from WeightEntries as we
    where we.Date > cast(getdate() as date)
)
select Problem, RecordId, Detail
from Problems

union all

select N'No problems found', null, null
where not exists (select 1 from Problems)

order by Problem;