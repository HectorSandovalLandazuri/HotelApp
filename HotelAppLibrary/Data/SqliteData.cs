using HotelAppLibrary.Databases;
using HotelAppLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelAppLibrary.Data
{
	public class SqliteData : IDatabaseData
	{
		private readonly ISqliteDataAccess _db;
		private const string connectionStringName = "SqliteDb";
		public SqliteData(ISqliteDataAccess db)
		{
			_db = db;
		}


		public void BookGuest(string firstName,
							  string lastName,
							  DateTime startDate,
							  DateTime endDate,
							  int roomTypeId)
		{
			string sql = @"select 1 from Guests where FirstName = @firstName And LastName = @lastName";
			int results= _db.LoadData<dynamic, dynamic>(sql,
													new { firstName, lastName },
													connectionStringName).Count();
			if (results ==0)
			{
				sql = @"insert into Guests(FirstName, LastName) Values(@firstName, @lastName);";
				_db.SaveData(sql,
					new { firstName,lastName },
					connectionStringName);
			}
			sql = @"select [Id], [FirstName], [LastName]
					from Guests where FirstName = @firstName And LastName = @lastName Limit 1;";

			GuestModel guest = _db.LoadData<GuestModel, dynamic>(sql,
																 new { firstName, lastName },
																 connectionStringName).First();

			RoomTypeModel roomType = _db.LoadData<RoomTypeModel, dynamic>("Select * From RoomTypes where Id = @roomTypeId",
																		  new { roomTypeId },
																		  connectionStringName).First();
			TimeSpan timeStaying = endDate.Date.Subtract(startDate.Date);


			sql = @"Select r.*
				from Rooms r
				inner join RoomTypes t on r.RoomTypeId = t.Id
				where roomTypeId=@roomTypeId and r.Id not in 
				(Select RoomId from Bookings b
				where (b.StartDate>@startDate and b.EndDate<@endDate)
				or (b.StartDate<=@startDate and b.EndDate>@startDate)
				or (b.StartDate<@endDate and b.EndDate>@endDate));";
			List<RoomModel> availableRooms = _db.LoadData<RoomModel, dynamic>(sql,
																			 new { startDate, endDate, roomTypeId },
																			 connectionStringName);
			sql = @"insert into Bookings (RoomId,GuestId,StartDate,EndDate,TotalCost)
				values (@roomId,@guestId,@startDate,@endDate,@totalCost);";
			_db.SaveData(sql,
						 new { roomId = availableRooms.First().Id, guestId = guest.Id, startDate, endDate, totalCost = timeStaying.Days * roomType.Price },
						 connectionStringName);

		}

		public void CheckInGuest(int Id)
		{
			string sql = @"update Bookings set CheckedIn=1 where Id=@Id;";
			_db.SaveData(sql, new { Id }, connectionStringName);
		}

		public List<RoomTypeModel> GetAvailableRoomTypes(DateTime startDate, DateTime endDate)
		{

			string sql = @"	Select t.Id, t.Title, t.Description,t.Price
				from Rooms r
				inner join RoomTypes t on r.RoomTypeId = t.Id
				where r.Id not in 
				(Select RoomId from Bookings b
				where (b.StartDate>@startDate and b.EndDate<@endDate)
				or (b.StartDate<=@startDate and b.EndDate>@startDate)
				or (b.StartDate<@endDate and b.EndDate>@endDate))
				group by t.Id, t.Title, t.Description,t.Price;";

			var output = _db.LoadData<RoomTypeModel, dynamic>(sql,
				new { startDate, endDate }, connectionStringName);
			output.ForEach(x => x.Price = x.Price / 100);
			return output;

		}

		public RoomTypeModel GetRoomTypeById(int id)
		{
			string sql= @"Select [Id], [Title], [Description], [Price] From RoomTypes
				where Id=@id;";
			return _db.LoadData<RoomTypeModel, dynamic>(sql,
													   new { id },
													   connectionStringName).FirstOrDefault();
		}

		public List<BookingFullModel> searchBookings(string lastName)
		{
			string sql = @"Select [b].[Id], [b].[RoomId], [b].[GuestId], [b].[StartDate], [b].[EndDate], [b].[CheckedIn], [b].[TotalCost], [g].[FirstName], [g].[LastName], [r].[RoomNumber], [r].[RoomTypeId],  [rt].[Title], [rt].[Description], [rt].[Price] from Bookings b 
				inner Join Guests g on b.GuestId = g.id
				inner join Rooms r on b.RoomId=r.Id
				inner join RoomTypes rt on r.RoomTypeId = rt.Id
				where b.StartDate=@startDate and g.LastName=@lastName;";
			var output= _db.LoadData<BookingFullModel, dynamic>(sql,
												   new { lastName, startDate = DateTime.Now.Date },
												   connectionStringName);
			output.ForEach(x=> {
				x.Price = x.Price / 100;
				x.TotalCost = x.TotalCost / 100;
			});
			return output;
		}
	}
}
