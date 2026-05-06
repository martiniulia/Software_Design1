using FlowerShop.Core.CQRS;
using FlowerShop.Data;
using FlowerShop.Models;
using FlowerShop.OrderService.CQRS.Commands;
using FlowerShop.OrderService.CQRS.Queries;
using FlowerShop.OrderService.CQRS.Handlers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=localhost;Database=flowershop;User=root;Password=root;";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();

builder.Services.AddScoped<ICommandHandler<CreateOrderCommand, int>, OrderCommandHandlers>();
builder.Services.AddScoped<IQueryHandler<GetOrderByIdQuery, Order?>, OrderQueryHandlers>();
builder.Services.AddScoped<IQueryHandler<GetOrdersByUserIdQuery, List<Order>>, OrderQueryHandlers>();

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
