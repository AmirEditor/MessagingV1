using AmirMahdiMousazadeh;
using AmirMahdiMousazadeh.Extentions;
using AmirMahdiMousazadeh.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(options=>
{
    options.EnableDetailedErrors = true;
});

var app = builder.Build();
app.MapGrpcService<MessengerService>();
MEFManager.Initialize();

app.Run();
