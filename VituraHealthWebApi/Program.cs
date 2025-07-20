using VituraHealthWebApi;
using VituraHealthWebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container
builder.Services.AddControllers();

// Add API Explorer services for Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the data access service using configuration
builder.Services.AddServices(configuration);

var app = builder.Build();

// Add error handling middleware (should be one of the first middlewares)
app.UseErrorHandling();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
