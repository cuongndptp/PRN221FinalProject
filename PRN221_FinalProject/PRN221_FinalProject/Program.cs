using Microsoft.EntityFrameworkCore;
using PRN221_FinalProject.Logics;
using PRN221_FinalProject.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<PRN221FinalProjectContext>(
    o => o.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn"))
    );
builder.Services.AddScoped<ScheduleServices>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
