CREATE PROCEDURE [dbo].[spRooms_GetAvailableRooms]
	@startDate date,
	@endDate date,
	@roomTypeId int
AS
begin
	set nocount on;
	Select r.*
	from dbo.Rooms r
	inner join dbo.RoomTypes t on r.RoomTypeId = t.Id
	where roomTypeId=@roomTypeId and r.Id not in 
	(Select RoomId from dbo.Bookings b
	where (b.StartDate>@startDate and b.EndDate<@endDate)
	or (b.StartDate<=@startDate and b.EndDate>@startDate)
	or (b.StartDate<@endDate and b.EndDate>@endDate));

end