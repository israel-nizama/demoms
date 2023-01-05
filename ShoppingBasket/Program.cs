using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Polly.Extensions.Http;
using Polly;
using ShoppingBasket.DbContexts;
using ShoppingBasket.Repositories;
using ShoppingBasket.Services;
using MessagingBus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<ShoppingBasketDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddSingleton<IMessageBus, AzServiceBusMessageBus>();
builder.Services.AddHttpClient<IDiscountService, DiscountService>(c => c.BaseAddress = new Uri(builder.Configuration["ApiConfigs:Discount:Uri"])).AddPolicyHandler(GetRetryPolicy()).AddPolicyHandler(GetCircuitBreakerPolicy());
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

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions.HandleTransientHttpError()
        .WaitAndRetryAsync(5,
            retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(1.5, retryAttempt) * 1000),
            (_, waitingTime) =>
            {
                Console.WriteLine("Retrying due to Polly retry policy");
            });
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(3, TimeSpan.FromSeconds(15));
}