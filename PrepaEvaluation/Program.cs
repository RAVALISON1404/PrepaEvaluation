using Microsoft.EntityFrameworkCore;
using PrepaEvaluation.Utils;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// TODO Add dbContext
builder.Services.AddDbContext<Connection>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("connection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    // TODO Generation de PDF
    RotativaConfiguration.Setup("D:\\GitHub\\PrepaEvaluation\\PrepaEvaluation\\wwwroot\\Rotativa");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// TODO Génération de PDF
app.UseRotativa();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// TODO création des tables temporaires utilisées dans les imports de fichiers
app.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStarted.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var serviceProvider = scope.ServiceProvider;
    var connection = serviceProvider.GetRequiredService<Connection>();
    connection.Database.OpenConnection();
    using var command = connection.Database.GetDbConnection().CreateCommand();
    command.CommandText = "CREATE TABLE sceancetemp (id SERIAL PRIMARY KEY, film VARCHAR(255) NOT NULL, categorie VARCHAR(255) NOT NULL, salle VARCHAR(255) NOT NULL, date DATE NOT NULL, heure TIME NOT NULL);";
    command.ExecuteNonQuery();
});

// TODO suppression des tables temporaires utilisées dans les imports de fichiers
app.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopped.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var serviceProvider = scope.ServiceProvider;
    var connection = serviceProvider.GetRequiredService<Connection>();
    connection.Database.OpenConnection();
    using var command = connection.Database.GetDbConnection().CreateCommand();
    command.CommandText = "DROP TABLE sceancetemp";
    command.ExecuteNonQuery();
});

app.Run();