CREATE PROCEDURE [dbo].[spRoomTypes_GetById]
	@id int
AS
begin
	set nocount on;
	Select [Id], [Title], [Description], [Price] From dbo.RoomTypes
	where Id=@id;
end
