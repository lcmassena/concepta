using Concepta.Application.Interfaces.Services.TravelLogix;
using Concepta.Services.TravelLogix;
using DryIoc;
using Massena.Infrastructure.Core.Resilience.Http;
using Massena.Infrastructure.Core.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Net.Http;

namespace Concepta.API
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            Configuration = builder.AddEnvironmentVariables().Build();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services
                ///// AddWebApi é um método de extensão definido no Core
                ///// Ele já define rotas no mesmo padrão, configurações de CORS
                ///// E inicializa o Swagger
                .AddWebApi<IoCRegistrator>(typeof(Startup), options =>
                {
                }, new Info
                {
                    Version = "v1",
                    Title = "Concepta API",
                    Description = "API documentation for Concepta",
                    Contact = new Contact { Name = "Lucas Massena", Email = "lucas@massena.com.br", Url = "http://architecture.massena.com.br" },
                });
        }

        public void Configure(IApplicationBuilder app,
                          IHostingEnvironment env,
                          ILoggerFactory loggerFactory,
                          IHttpContextAccessor accessor)
        {
            // Generic starter created in infrastructure to define the same
            // behavior in every application
            app.UseWebApi(env, loggerFactory, accessor);
        }
    }

    public class IoCRegistrator
    {
        int _retryCount = 3;
        int _exceptionsAllowedBeforeBreaking = 3;

        public IoCRegistrator(IRegistrator registrator)
        {
            registrator.RegisterDelegate<IHttpClient>(sp => sp.Resolve<IResilientHttpClientFactory>().CreateResilientHttpClient());
            registrator.Register<ITravelLogixApiService, TravelLogixApiService>(reuse: Reuse.InWebRequest);
        }

        private Policy[] GeneratePolicies(string s)
        {
            return new Polly.Policy[] {
                    Polly.Policy.Handle<HttpRequestException>()
                    .WaitAndRetryAsync(
                        // number of retries
                        _retryCount,
                        // exponential backofff
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        // on retry
                        (exception, timeSpan, retryCount, context) =>
                        {
                            var msg = $"Retry {retryCount} implemented with Polly's RetryPolicy " +
                                $"of {context.PolicyKey} " +
                                $"at {context.OperationKey}, " +
                                $"due to: {exception}.";
                            //_logger.LogWarning(msg);
                            //_logger.LogDebug(msg);
                        }),
                    Polly.Policy.Handle<HttpRequestException>()
                    .CircuitBreakerAsync( 
                       // number of exceptions before breaking circuit
                       _exceptionsAllowedBeforeBreaking,
                       // time circuit opened before retry
                       TimeSpan.FromMinutes(1),
                       (exception, duration) => {
                            // on circuit opened
                            //_logger.LogTrace("Circuit breaker opened");
                       },
                       () => {
                            // on circuit closed
                            //_logger.LogTrace("Circuit breaker reset");
                       })
                };
        }
    }
}
