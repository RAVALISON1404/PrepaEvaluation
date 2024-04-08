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

// TODO Generation de PDF
app.UseRotativa();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();