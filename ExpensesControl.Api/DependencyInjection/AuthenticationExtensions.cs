using ExpensesControl.Application.Common.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ExpensesControl.Api.DependencyInjection
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var keyBytes = Encoding.UTF8.GetBytes(JwtOptions.Key);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = JwtOptions.Issuer,
                    ValidAudience = JwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ClockSkew = TimeSpan.Zero
                };

                // Recomendación: no loguear tokens en consola. Si necesitas debug, usa ILogger en Development.
            });

            return services;
        }
    }
}
