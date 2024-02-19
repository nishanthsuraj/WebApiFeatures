using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using WebApiFeatures.Db;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
{
    options.Authority = "https://localhost:5001";
    options.Audience = "scope1";

    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateAudience = false
    };
});

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;

    // Implements Header Versioning
    //options.ApiVersionReader = new HeaderApiVersionReader("X-API-Version"); 

    // Implements Query String Versioning
    //options.ApiVersionReader = new QueryStringApiVersionReader("version"); 
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ShopContext>(options => options.UseInMemoryDatabase("Shop"));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        // This is for any Origins, any Method or any Header
        //builder.AllowAnyOrigin()
        //.AllowAnyMethod()
        //.AllowAnyHeader(); 

        builder.WithOrigins("https://localhost:7270") // Allow only the web application we have created.
        .WithHeaders("X-API-Version"); // Allow this header as Web API may use this in "Implements Header Versioning"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.Run();
