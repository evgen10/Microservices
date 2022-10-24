using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public static class ServicesConfiguration
    {
        public static void AddAzureMessageBus(this IServiceCollection services)
        {
            services.AddSingleton<IMessageBus, AzureMessageBus>();
        }
    }
}
