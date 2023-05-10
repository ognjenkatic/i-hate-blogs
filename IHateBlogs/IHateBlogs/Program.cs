using IHateBlogs.Infrastructure;
using IHateBlogs.Application;
using IHateBlogs;
using IHateBlogs.Infrastructure.Persistence;
using IHateBlogs.Application.Common.Util;
using IHateBlogs.Hubs;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddInfrastructureServices(configuration);
builder.Services.AddApplicationServices();
builder.Services.AddApiServices(configuration);

var app = builder.Build();

MigrationUtil.MigrateDatabase(app.Services);
await SeedUtil.Seed(app.Services);
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
app.MapHub<PostCompletionHub>("/postCompletionHub");
app.Run();
