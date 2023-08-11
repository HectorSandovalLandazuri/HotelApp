using HotelAppLibrary.Data;
using HotelAppLibrary.Databases;
using System.Diagnostics.Eventing.Reader;

var builder = WebApplication.CreateBuilder(args);


string dbChoice = builder.Configuration.GetSection("DatabaseChoice").Get<string>();	;
if (dbChoice=="SQL") {
	builder.Services.AddTransient<IDatabaseData, SqlData>();
	builder.Services.AddTransient<ISqlDataAccess, SqlDataAccess>();

}
else if (dbChoice == "SQLite")
{
	builder.Services.AddTransient<IDatabaseData, SqliteData>();
	builder.Services.AddTransient<ISqliteDataAccess, SqliteDataAccess>();
}
else
{
	//Default
	builder.Services.AddTransient<IDatabaseData, SqlData>();
	builder.Services.AddTransient<ISqlDataAccess, SqlDataAccess>();

}


// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
