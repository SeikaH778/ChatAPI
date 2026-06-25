
using ChatApplication.Interfaces;
using ChatApplication.UseCases;
using ChatPersistance;
using Microsoft.EntityFrameworkCore;
using TestChatAPI.Hubs;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ChatDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSignalR();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddCors(options =>{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://0.0.0.0:5126")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});
var app = builder.Build();
app.MapHub<ChatHub>("/chat");
app.MapHub<DbHub>("/db");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run("http://0.0.0.0:5126");
