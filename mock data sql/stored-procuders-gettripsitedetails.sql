
CREATE PROCEDURE GetTripSiteDetails @tripid nvarchar(450)
as
begin
select t.TripId as TripId, t.Name as TripName ,t.Description as TripDescription ,t.IsDeleted as TripIsDeleted ,
t.AvailablePeople as AvaiblePeople , t.Duration as TripTotalDuration , t.EndDate as TripendDate,
t.StartDate as TripStartDate , t.MaxPeople as TripMaxPeople , t.Money as TripMoney , t.OutOfDate as TripisOutOfDate
,tps.Id as TransportId ,tps.Name as TransportName,tix.Id as IncludedId,tix.Item as IncludedItem , tex.Id as ExcludedId ,
tex.Item as ExcludedItem,
s.SiteId as SiteId, s.SiteName as SiteName ,s.SiteDescription as SiteDescription, si.Id as ImagId,
si.ImageName as SiteImageName, tp.PlanInSite as PlanOfTripInSite  , g.GovernmentId as GovernmentId , g.Name as GovernmentName,
g.Image as GovernmentImage ,
tp.NumberDays as NumberOfDaysInSite, d.Id as DriverId, d.Name as DriverName
from TripSites tp
join Trips t 
on tp.TripId=t.TripId
join TripExcludeds tex
on tex.TripId=t.TripId
join TripIncludeds tix
on tix.TripId=t.TripId
join Sites s
on s.SiteId=tp.SiteId
join Governments g
on g.GovernmentId=s.GovernmentId
join SiteImages si
on si.SiteId=s.SiteId
join Transportations tps
on tps.Id=t.TransportationId
join Drivers d
on d.TransportationId=tps.Id
where t.TripId=@tripid
end

