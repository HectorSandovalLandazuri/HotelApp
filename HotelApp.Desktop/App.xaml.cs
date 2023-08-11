using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using HotelAppLibrary.Data;
using HotelAppLibrary.Databases;

namespace HotelApp.Desktop
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static ServiceProvider serviceProvider;
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			var services=new ServiceCollection();
			services.AddTransient<MainWindow>();
			services.AddTransient<CheckInForm>();







			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json");
			IConfiguration config = builder.Build();
			services.AddSingleton(config);

			string dbChoice = config.GetValue<string>("DatabaseChoice");
			if (dbChoice == "SQL")
			{
				services.AddTransient<IDatabaseData, SqlData>();
				services.AddTransient<ISqlDataAccess, SqlDataAccess>();

			}
			else if (dbChoice == "SQLite")
			{
				services.AddTransient<IDatabaseData, SqliteData>();
				services.AddTransient<ISqliteDataAccess, SqliteDataAccess>();
			}
			else
			{
				//Default
				services.AddTransient<IDatabaseData, SqlData>();
				services.AddTransient<ISqlDataAccess, SqlDataAccess>();
			}


			serviceProvider = services.BuildServiceProvider();






			var mainWindow=serviceProvider.GetService<MainWindow>();
			mainWindow.Show();

		}
	}
}
