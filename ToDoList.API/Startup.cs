using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.API.Authentication;
using ToDoList.API.Authentication.Interfaces;
using ToDoList.API.Authentication.SecurityKeys;
using ToDoList.API.Helpers;
using ToDoList.API.Helpers.Interfaces;
using ToDoList.API.Middlewares.Extensions;
using ToDoList.API.Services;
using ToDoList.API.Services.Interfaces;
using ToDoList.DAL.Contexts;
using ToDoList.DAL.Repository;
using ToDoList.DAL.Repository.Interfaces;

namespace ToDoList.API
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        private IWebHostEnvironment Environment { get; }

        readonly private string _corsDevPolicy;

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
            _corsDevPolicy = "DevCorsPolicy";
        }


        public void ConfigureServices(IServiceCollection services)
        {
            var connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<Context>(options => options.UseMySql(connection, m => m.MigrationsAssembly("ToDoList.DAL")));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));// Problems with singleton parents
            
            // There is only one helper so we use so, in either case we have an option to use Autofac
            services.AddSingleton<IAccountHelper, AccountHelper>();

            services.AddSingleton<IWebsocketHandler, WebsocketHandler>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()
                                       .Where(a => a?.FullName != null &&
                                                   a.FullName.StartsWith("ToDoList.API")));

            services.AddRouting();

            services.AddCors(options =>
            {
                options.AddPolicy(_corsDevPolicy, builder =>
                                      builder.WithOrigins("https://localhost:44336", "https://localhost:44336")
                                             .AllowAnyHeader()
                                             .AllowCredentials()
                                             .WithExposedHeaders("Token-Expired", "WWW-Authenticate", "Authorization")
                                             .AllowAnyMethod());
            });

            services.AddControllers()
                    .AddControllersAsServices()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                    .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    });

            // Jwt with signing key and encoding, it's have 5(there is no second because of symmetric) segments in itself(strongly private, ps i think so)
            const string signingSecurityKey = JwtSettings.SigningKey;
            var signingKey = new SigningSymmetricKey(signingSecurityKey);
            services.AddSingleton<IJwtSigningEncodingKey>(signingKey);

            const string encodingSecurityKey = JwtSettings.EncodingKey;
            var encryptionEncodingKey = new EncryptingSymmetricKey(encodingSecurityKey);
            services.AddSingleton<IJwtEncryptingEncodingKey>(encryptionEncodingKey);

            var signingDecodingKey = (IJwtSigningDecodingKey)signingKey;
            var encryptingDecodingKey = (IJwtEncryptingDecodingKey)encryptionEncodingKey;
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = true;
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = JwtSettings.ValidateIssuer,
                            ValidIssuer = JwtSettings.ValidIssuer,
                            ValidateAudience = JwtSettings.ValidateAudience,
                            ValidAudience = JwtSettings.ValidAudience,
                            ValidateLifetime = JwtSettings.ValidateLifetime,
                            IssuerSigningKey = signingDecodingKey.GetKey(),
                            TokenDecryptionKey = encryptingDecodingKey.GetKey(),
                            ValidateIssuerSigningKey = JwtSettings.ValidateIssuerSigningKey,
                            RequireExpirationTime = JwtSettings.RequireExpirationTime,
                            ClockSkew = JwtSettings.ClockSew
                        };
                        options.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = context =>
                            {
                                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                                {
                                    context.Response.Headers.Add("Token-Expired", "true");
                                }

                                return Task.CompletedTask;
                            }
                        };
                    });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Context context)
        {
            context.Database.Migrate();

            app.UseExceptionMiddleware();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(_corsDevPolicy);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseWebSockets();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                         .RequireAuthorization();
            });
        }
    }
}
