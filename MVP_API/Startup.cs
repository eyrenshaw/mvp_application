using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace MVP_API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var validIssuer = Configuration.GetSection("authenticationAuthorization:issuer").Value;
            var tokenSecureAuth = new TokenSecureAuthentication(validIssuer);
            var signingKey = tokenSecureAuth.GetOpenIdConnectConfigSigningKey();
            var validAudience = Configuration.GetSection("authenticationAuthorization:audience").Value;

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMemoryCache();
            services.Configure<DbSetting>(options =>
            {
                options.ConnectionString = Configuration.GetSection("database:connectionString").Value;
                options.DatabaseName = Configuration.GetSection("database:databaseName").Value;
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = validIssuer,
                    ValidAudience = validAudience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    IssuerValidator = tokenSecureAuth.CustomIssuerValidator
                };
            });

            services.AddSingleton<IAuthorizationHandler, SubjectNameAuthorizationHandler>();

            var validUsers = Configuration.GetSection("authenticationAuthorization:authorizedUsers").Value;
            services.AddAuthorization(options =>
                {
                    options.AddPolicy("SubjectNames",
                        policy => policy.AddRequirements(new SubjectNameRequirement(validUsers)));
                });

            services.AddSingleton<IRepository, Repository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
