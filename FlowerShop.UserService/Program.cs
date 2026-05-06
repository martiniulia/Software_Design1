using FlowerShop.Core.CQRS;
using FlowerShop.Data;
using FlowerShop.Models;
using FlowerShop.UserService.CQRS.Commands;
using FlowerShop.UserService.CQRS.Queries;
using FlowerShop.UserService.CQRS.Handlers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=localhost;Database=flowershop;User=root;Password=root;";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();

builder.Services.AddScoped<ICommandHandler<CreateUserCommand, int>, UserCommandHandlers>();
builder.Services.AddScoped<IQueryHandler<GetUserByIdQuery, User?>, UserQueryHandlers>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
