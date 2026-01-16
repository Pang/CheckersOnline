using CheckersOnline.Server.Hubs;
using CheckersOnline.Server.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.Converters.Add(
        new JsonStringEnumConverter()
    );
}); 
;
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddSingleton<GameEngine>();
builder.Services.AddSingleton<CheckersRules>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.UseWebSockets();
app.UseCors("DevCors");

app.MapControllers();
app.MapHub<CheckersHub>("/checkersHub");
app.Run();
