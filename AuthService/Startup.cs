using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace AuthService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // The public key to the corresponding private key of the authentication service
        // It is used to validate the JWT
        private const string PUBLIC_KEY = @"MIICCgKCAgEAtCR2Pii+q9C76P2E9ydHYxnBPjJFGT7MvHuQPKpcS9RImfrkobt0
        LPS/406eWm/tRBvnYD9nDpHJNKN3TjEenFQuDGR4RHcGK/e43SAhTAi7+s0tfAQd
        6BK4gznIwvs5cWyilh1B7c9sCnxhJ/EYLIe1N2yiD8mhvfojIF4vMYxONIMTGYXy
        87lnO9zRAdXAZ39YbtmFmQwK8gfXX5d/XVlKy0tc2y5bRY5iXn9kwqwvFlzL6O4v
        pjhqA5kwsJV7efhL9nU0ACR4dG3zwFR3SAOOSETXjnfmjH2ocga+oa65ToypUz2L
        1DwnNHt+M5CtDJ9um4dbYaqfBWkjWe3FuGB0GNPS8pbX2nVt76OfHA/QKmxTWvFd
        POZnjpg2QhDujyXgoIY731zx5bAklKVoKFma/qfWfCyCSTUzhgu1KQm9swipMsQy
        NYr9CjbnIlPn4EvrBIbGcIiaRNCLCIlcAuxE/GiH1zBUfeJxfJQmurejp6mBAtAS
        FY08DmUebBz8mlUbB+LXMYKHZ4GK6TecPy0WJU2qRMQ//PKfOa+wkesp4M53SQdp
        ItDp5akTzYUo4rXwk3HPCtemKaSNhyG+EYtZ1CAmPN5sEjU0/x0Dq7SU5o8KhogB
        m/5HRJ3M9dMRcwD3OcsMl0kW1PPUt04itboS3SlFav90V9uc2YNGpPsCAwEAAQ==";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularDevClient",
                  builder =>
                  {
                      builder
                      .WithOrigins("http://localhost:4200/","http://localhost")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
                  });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    RSA rsa = RSA.Create();
                    rsa.ImportRSAPublicKey(Convert.FromBase64String(PUBLIC_KEY), out _);

                    options.IncludeErrorDetails = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new RsaSecurityKey(rsa),
                        ValidateIssuer = true,
                        ValidIssuer = "AuthService",
                        ValidateAudience = true,
                        ValidAudience = "AuthService",
                        CryptoProviderFactory = new CryptoProviderFactory()
                        {
                            CacheSignatureProviders = false
                        }
                    };
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
                app.UseCors(builder => builder
            .WithOrigins("http://localhost:4200","http://localhost:4200/","http://localhost")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
             );
           
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}