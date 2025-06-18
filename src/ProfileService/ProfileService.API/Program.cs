using Microsoft.EntityFrameworkCore;
using ProfileService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ProfilesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(""Default"")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
