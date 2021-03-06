﻿using System.Linq;
using BracketSystem.Core.Data;
using BracketSystem.Core.Data.Repositories;
using BracketSystem.Core.Helpers;
using BracketSystem.Core.Models.Entities;
using BracketSystem.Web.AspNetCore.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.SpaServices.AngularCli;

namespace BracketSystem.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            HostingEnvironment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            services.AddDbContext<BracketContext>(
                x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection"))
            );

            ConfigureServices(services);
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            services.AddDbContext<BracketContext>(x =>
                x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            ConfigureServices(services);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            IdentityBuilder builder = services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            });

            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<BracketContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();

            AddAuthenticationScheme(services);
            AddPolicies(services);

            services.AddControllers(
                    options =>
                    {
                        var policy = new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                            .Build();
                        options.Filters.Add(new AuthorizeFilter(policy));
                    })
                .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            var allowedOrigins = Configuration
                .GetSection("AllowedOrigins")
                .GetChildren()
                .Select(x => x.Value)
                .ToArray();
            
            services.AddCors(options =>
            {
                options.AddPolicy("Cors",
                    corsPolicyBuilder => corsPolicyBuilder
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .Build());
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // host and serve client application files
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "Client/Angular"; });

            if (HostingEnvironment.IsDevelopment())
            {
                // Register the Swagger generator, defining 1 or more Swagger documents
                _ = services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo {Title = "BracketSystem API", Version = "v1"});
                    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Description =
                            "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                        In = ParameterLocation.Header,
                        Name = "Authorization",
                        Scheme = "apiKey",
                    });
                    c.OperationFilter<SecurityRequirementsOperationFilter>();
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // enable swagger middleware
                app.UseSwagger();
                app.UseSwaggerUI(
                    setup =>
                    {
                        setup.SwaggerEndpoint("/swagger/v1/swagger.json", "BracketSystem API v1");
                        setup.RoutePrefix = string.Empty;
                    });
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();

                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(true);
                        }
                    });
                });
            }

            // app.UseHttpsRedirection();
// host and serve client application files
            app.UseSpaStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("Cors");
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                    .RequireCors("Cors");
            });

            // host and serve client application files
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "Client"; 
                /*spa.UseSpaPrerendering(options =>
                {
                    // This is the path where angular generates the server build result
                    // This path would be different when using React or Vue
                    options.BootModulePath = $"{spa.Options.SourcePath}/server/main.js";

                    options.ExcludeUrls = new[] { "/sockjs-node" };
                });*/
            });
        }

        private void AddAuthenticationScheme(IServiceCollection services)
        {
            var token = Configuration.GetValue<string>("AppSettings:Token");
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(token)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
        }

        private void AddPolicies(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequiredAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("Moderator", policy => policy.RequireRole("RootAdmin", "Admin", "Moderator"));
                options.AddPolicy("Root", policy => policy.RequireRole("RootAdmin"));
            });
        }
    }
}