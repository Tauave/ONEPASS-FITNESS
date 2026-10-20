select Name, Decription, DurationMinutes
from ClassTypes
where IsActive = 1	
Order by DurationMinutes desc;