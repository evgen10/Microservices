using Mongo.Services.OrderAPI.Messaging;

namespace Mongo.Services.OrderAPI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IAzureServiceBusConsumer AzureServiceBusConsumer { get; set; }
        
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            AzureServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostAppLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostAppLife.ApplicationStarted.Register(OnStart);
            hostAppLife.ApplicationStopped.Register(OnStop);

            return app;
        }

        private static void OnStart()
        {
            AzureServiceBusConsumer.Start();
        }

        private static void OnStop()
        {
            AzureServiceBusConsumer.Stop();
        }
    }
}
