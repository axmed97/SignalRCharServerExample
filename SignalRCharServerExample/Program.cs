using SignalRCharServerExample.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
{
    policy.AllowCredentials();
    policy.AllowAnyHeader();
    policy.AllowAnyMethod();
    policy.SetIsOriginAllowed(x => true);
}));
builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors();
app.UseRouting();

app.MapHub<ChatHub>("/chatHub");

app.Run();
