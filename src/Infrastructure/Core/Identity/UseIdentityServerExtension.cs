using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Massena.Infrastructure.Core.Identity
{
    /// <summary>
    /// Configura a Autenticação do sistema
    /// </summary>
    public class IdentityServerSettings
    {
        /// <summary>
        /// Permisson endpoint to validate authorization
        /// </summary>
        public string PermissionAuthority { get; set; }

        /// <summary>
        /// Authority
        /// </summary>
        public string Authority { get; set; }
        /// <summary>
        /// Audience
        /// </summary>
        public string Audience { get; set; }

    }

    /// <summary>
    /// Configura a Autenticação do sistema
    /// </summary>
    public static class IdentityServerExtension
    {

        /// <summary>
        /// Faz a aplicação autenticar no endpoint do IdentityServer
        /// </summary>
        /// <param name="services">Coleção de serviços</param>
        /// <param name="identityServerConfiguration">Configuration do identity server</param>
        public static IServiceCollection UseIdentityServerAuthenticationService(this IServiceCollection services, IdentityServerSettings authSettings)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var tvps = new TokenValidationParameters
            {
                ValidAudience = authSettings.Audience,
                ValidateIssuer = false
            };

            services.AddAuthentication("Bearer")
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = authSettings.Authority;
                options.RequireHttpsMetadata = false;
            });

            return services;
        }
    }
}
