use [ONEPASS-FITNESS];
go

select concat(AspNetUsers.Name, ' ', AspNetUsers.Lastname) as FullName, AspNetUsers.Email, AspNetRoles.Name as RoleName
from AspNetUsers, AspNetUserRoles, AspNetRoles
where AspNetUsers.Id = AspNetUserRoles.UserId and AspNetRoles.Id = AspNetUserRoles.RoleId and AspNetRoles.Name = 'Admin'
order by FullName;