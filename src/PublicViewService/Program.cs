// src\PublicViewService\Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseDeveloperExceptionPage(); app.UseSwagger(); app.UseSwaggerUI(); }
app.MapGet("/publicview/api/info", () => Results.Ok(new { message = "Public view ok" }));
app.Run();
