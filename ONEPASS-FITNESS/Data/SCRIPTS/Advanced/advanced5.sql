select u.Name + N' ' + u.Lastname as FullName,
       u.Email,
       u.EmailConfirmed,
       r.Name as RoleName
from AspNetUsers as u
inner join AspNetUserRoles as ur on ur.UserId = u.Id
inner join AspNetRoles as r on r.Id = ur.RoleId
where r.Name = N'Admin'
order by u.Email;