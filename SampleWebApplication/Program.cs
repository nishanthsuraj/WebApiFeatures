using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SampleWebApplication.Areas.Identity.Data;
using SampleWebApplication.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SampleWebApplicationContextConnection") ?? throw new InvalidOperationException("Connection string 'SampleWebApplicationContextConnection' not found.");

builder.Services.AddDbContext<SampleWebApplicationContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<SampleWebApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<SampleWebApplicationContext>();

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
