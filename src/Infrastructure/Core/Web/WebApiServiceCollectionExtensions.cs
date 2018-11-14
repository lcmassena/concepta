using DryIoc;
using Massena.Infrastructure.Core.Infrastructure.Validation.Implementation;
using Massena.Infrastructure.Core.IoC;
using Massena.Infrastructure.Core.Web.Validation;
using Massena.Infrastructure.Validation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Text;

namespace Massena.Infrastructure.Core.Web
{
    public class IoCInfrastructureRegistrator
    {
        public IoCInfrastructureRegistrator(IRegistrator registrator)
        {
            registrator.RegisterDelegate<ModelStateDictionary>(r => new ModelStateDictionary(), reuse: Reuse.InWebRequest);
            registrator.RegisterDelegate<IValidationDictionary>(r => new ModelStateWrapper(new ModelStateDictionary()), reuse: Reuse.InWebRequest);
        }
    }

    public static class WebApiServiceCollectionExtensions
    {
        public static MvcBuilder AddWebApi(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var builder = services.AddMvcCore();
            builder.AddJsonFormatters();
            builder.AddApiExplorer();
            builder.AddCors();

            builder.AddJsonFormatters(options => options.ContractResolver = new CamelCasePropertyNamesContractResolver());
            builder.AddAuthorization();

            return new MvcBuilder(builder.Services, builder.PartManager);
        }

        /// <summary>
        /// AddWebApi é um método de extensão definido no Core, que define rotas no mesmo padrão, 
        /// configurações de CORS e Swagger
        /// </summary>
        /// <param name="services">Coleção de serviços do ASP.NET Core</param>
        /// <param name="setupAction">Método para configurações adicionais</param>
        /// <param name="swaggerInfo">Informações do Swagger</param>
        /// <returns>Retorna a configuração de serviços devidamente configurada</returns>
        public static IServiceProvider AddWebApi<ioCRegistratorClass>(this IServiceCollection services, Type mediatorClassType, Action<MvcOptions> setupAction, Info swaggerInfo)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

            var builder = services.AddWebApi();

            builder.Services
                .Configure((MvcOptions options) =>
                {
                    options.OutputFormatters.Remove(new XmlDataContractSerializerOutputFormatter());
                    //options.UseCentralRoutePrefix(new RouteAttribute("v{version}"));
                })
                .Configure(setupAction)
                .AddSwaggerGen(s => s.SwaggerDoc(swaggerInfo.Version, swaggerInfo));

            // Adding MediatR for Domain Events and Notifications
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;

                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = "me",
                        ValidAudience = "you",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("rlyaKithdrYVl6Z80ODU350md")) //Secret
                    };
                });

            services.AddMvcCore();

                services.AddDryIoc<IoCInfrastructureRegistrator>(); ;

            services.AddMediatR(mediatorClassType);

            return services.AddDryIoc<ioCRegistratorClass>();
        }

        public static void UseCentralRoutePrefix(this MvcOptions opts, IRouteTemplateProvider routeAttribute)
        {
            opts.Conventions.Insert(0, new RouteConvention(routeAttribute));
        }


        public static IApplicationBuilder UseWebApi(this IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IHttpContextAccessor accessor)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app
                .UseAuthentication()
                .UseCors(c =>
                {
                    c.AllowAnyHeader();
                    c.AllowAnyMethod();
                    c.AllowAnyOrigin();
                })
                .UseSwagger()
                .UseSwaggerUI(s => s.SwaggerEndpoint("/swagger/v1/swagger.json", "Concepta API v1.0"))
                .UseDefaultFiles()
                .UseStaticFiles()
                .UseMvc();

            return app;
        }
    }
}