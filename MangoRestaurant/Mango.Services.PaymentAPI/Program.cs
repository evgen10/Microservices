using Mango.MessageBus;
using Mango.Services.PaymentAPI.Configurations;
using Mongo.Services.PaymentAPI.Extensions;
using Mongo.Services.PaymentAPI.Messaging;
using PaymentProcessor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



builder.Services.AddControllers();
builder.Services.AddSingleton<IProcessPayment, ProcessPayment>();
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
builder.Services.AddAzureMessageBus();
builder.Services.Configure<AzureConfig>(builder.Configuration.GetSection("AzureSettings"));

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
app.UseAzureServiceBusConsumer();

app.MapControllers();


app.Run();
