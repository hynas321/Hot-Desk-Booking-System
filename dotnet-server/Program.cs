using Dotnet.Server.Managers;
using Dotnet.Server.Database;
using Microsoft.EntityFrameworkCore;
using Dotnet.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options => 
{
    options.JsonSerializerOptions.PropertyNamingPolicy = new PascalCaseNamingPolicy();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => 
{
    options.AddPolicy("AllowReactApp",
        builder => builder.WithOrigins("*")
            .AllowAnyHeader()
            .AllowAnyMethod()   
    );
});

builder.Services.AddScoped<SessionTokenManager>();
builder.Services.AddScoped<HashManager>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<LocationRepository>();
builder.Services.AddScoped<DeskRepository>();
builder.Services.AddScoped<BookingRepository>();
builder.Services.AddHostedService<DailyTaskService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
