global using RBXOSeed.Extensions;
using Microsoft.AspNetCore.Routing.Matching;
using RBXOSeed.Data;
using RBXOSeed.Services;
using RBXOSeed.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Configure this in the appsettings.json
//app.Urls.Add("http://localhost:80");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RBXOne Seed Service API v1");
        c.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

DbContext.Initialize();
await DbContext.CheckPoint();

//Processes arguments
await ArgumentParser.ProcessArgs(args);

//Runs services based on arguments
await StartupServices.RunArgs();
await StartupServices.SetGlobals(app);

await NodeProcessor.PopulateNodeBag();

_ = NodeProcessor.StartNodeQueue();
_ = NodeProcessor.StartNodePortChecks();

//This needs to be at bottom.
app.Run();


