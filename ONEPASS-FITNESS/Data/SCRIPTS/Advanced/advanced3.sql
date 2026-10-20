select Name, concat(DurationMinutes, ' mins') as Duration, Description
from ClassTypes
where IsActive = 1
order by Name;