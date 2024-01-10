global using RBXOSeed.Extensions;
using Microsoft.AspNetCore.Routing.Matching;
using RBXOSeed;
using RBXOSeed.Data;
using RBXOSeed.Services;
using RBXOSeed.Utility;
using System.Globalization;

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
var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (app.Environment.IsDevelopment() || environmentName == "Devprod")
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RBXOne Seed Service API v1");
        c.DisplayRequestDuration();
    });
}

//Commented these out to avoid too many redirects error from Cloudflare
//app.UseHttpsRedirection();
//app.UseAuthorization();

//Also used for Cloudflare
// The default HSTS value is 30 days
app.UseHsts();

app.MapControllers();

var culture = CultureInfo.GetCultureInfo("en-US");
if (Thread.CurrentThread.CurrentCulture.Name != "en-US")
{
    CultureInfo.DefaultThreadCurrentCulture = culture;
    CultureInfo.DefaultThreadCurrentUICulture = culture;

    Thread.CurrentThread.CurrentCulture = culture;
    Thread.CurrentThread.CurrentUICulture = culture;
}

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

Globals.CLIVersion = $"{Globals.MajorVer}.{Globals.MinorVer}.{Globals.RevisionVer}";
//This needs to be at bottom.
app.Run();


