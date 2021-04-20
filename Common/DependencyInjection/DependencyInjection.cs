using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Common.DependencyInjection
{
    public class Utilities
    {
        public class DependencyInjection
        {
            public static ServiceProvider Provider;
            public static void AddServices(IServiceCollection services)
            {
                Provider = services.BuildServiceProvider();
            }
            public static void ClearServices()
            {

            }
            public static T GetService<T>()
            {
                var serviceScopeFactory = Provider.GetRequiredService<IServiceScopeFactory>();
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    return scope.ServiceProvider.GetService<T>();
                }
            }
        }
    }
}
