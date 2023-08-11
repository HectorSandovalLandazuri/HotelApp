/*
Plantilla de script posterior a la implementación							
--------------------------------------------------------------------------------------
 Este archivo contiene instrucciones de SQL que se anexarán al script de compilación.		
 Use la sintaxis de SQLCMD para incluir un archivo en el script posterior a la implementación.			
 Ejemplo:      :r .\miArchivo.sql								
 Use la sintaxis de SQLCMD para hacer referencia a una variable en el script posterior a la implementación.		
 Ejemplo:      :setvar TableName miTabla							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

if not exists (Select 1 from dbo.RoomTypes)
begin
    insert into dbo.RoomTypes (Title,Description,Price) 
    Values ('King Size Bed','A Room with a king-side bed and a window.',100),
           ('Two Queen Size Beds','A Room with two queen-side bed and a window.',115),
           ('Executive Suite','A Room with two queen-side bed and a window.',205);
end
if not exists (Select 1 from dbo.Rooms)
begin
    declare @roomId1 int;
    declare @roomId2 int;
    declare @roomId3 int;
    select @roomId1= Id from dbo.RoomTypes where Title='King Size Bed';
    select @roomId2= Id from dbo.RoomTypes where Title='Two Queen Size Beds';
    select @roomId3= Id from dbo.RoomTypes where Title='Executive Suite';
    
    insert into dbo.Rooms (RoomNumber,RoomTypeId) 
    Values ('101',@roomId1),
           ('102',@roomId1),
           ('103',@roomId1),
           ('201',@roomId2),
           ('202',@roomId2),
           ('301',@roomId3);
end

