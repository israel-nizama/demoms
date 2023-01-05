using MessagingBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Orders.DbContexts;
using Orders.Extensions;
using Orders.Messaging;
using Orders.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<OrderDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
//Specific DbContext for use from singleton AzServiceBusConsumer
var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>();
optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddSingleton(sp => new OrderRepository(optionsBuilder.Options));
builder.Services.AddSingleton<IMessageBus, AzServiceBusMessageBus>();
builder.Services.AddSingleton<IAzServiceBusConsumer, AzServiceBusConsumer>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseAzServiceBusConsumer();

app.Run();
