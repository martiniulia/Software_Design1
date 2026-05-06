using FlowerShop.Core.CQRS;
using FlowerShop.Data;
using FlowerShop.Models;
using FlowerShop.CatalogService.CQRS.Commands;
using FlowerShop.CatalogService.CQRS.Queries;
using FlowerShop.CatalogService.CQRS.Handlers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=localhost;Database=flowershop;User=root;Password=root;";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();

builder.Services.AddScoped<ICommandHandler<CreateFlowerCommand, int>, CreateFlowerCommandHandler>();
builder.Services.AddScoped<IQueryHandler<GetAllFlowersQuery, List<Flower>>, FlowerQueryHandlers>();
builder.Services.AddScoped<IQueryHandler<GetFlowerByIdQuery, Flower?>, FlowerQueryHandlers>();

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
