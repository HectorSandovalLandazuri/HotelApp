CREATE PROCEDURE [dbo].[spRoomTypes_GetAvailableTypes]
	@startDate date,
	@endDate date
AS
begin
	set nocount on;
	Select t.Id, t.Title, t.Description,t.Price
	from dbo.Rooms r
	inner join dbo.RoomTypes t on r.RoomTypeId = t.Id
	where r.Id not in 
	(Select RoomId from dbo.Bookings b
	where (b.StartDate>@startDate and b.EndDate<@endDate)
	or (b.StartDate<=@startDate and b.EndDate>@startDate)
	or (b.StartDate<@endDate and b.EndDate>@endDate))
	group by t.Id, t.Title, t.Description,t.Price;
end
