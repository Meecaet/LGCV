﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebCV_Fiches.Data;
using WebCV_Fiches.Models.Admin;
using WebCV_Fiches.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Caching.Memory;
using DAL_CV_Fiches.Repositories.Graph;
using DAL_CV_Fiches.Models.Graph;
using DAL_CV_Fiches;
using WebCV_Fiches.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebCV_Fiches.Models.AccountViewModels;
using WebCV_Fiches.Filters;
using WebCV_Fiches.Helpers;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WebCV_Fiches
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private string endpoint, primaryKey, database, collection;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            endpoint = Configuration.GetSection("GraphConnectionEndPoint").Value;
            primaryKey = Configuration.GetSection("GraphConnectionPrimaryKey").Value;
            database = Configuration.GetSection("GraphConnectionDatabase").Value;
            collection = Configuration.GetSection("GraphConnectionCollection").Value;

            GraphConfig.SetGraphDataBaseConnection(endpoint, primaryKey, database, collection);

            services = TokenConfigurations(services);

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddCors();

            services.AddMvc();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddSingleton(Configuration);
            services.AddSingleton<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
            });


        }
        private IServiceCollection TokenConfigurations(IServiceCollection services)
        {
            services.AddTransient<LoginService>();

            var signingConfigurations = new SigningConfigurationsExtensions();
            services.AddSingleton(signingConfigurations);

            var tokenConfigurations = new TokenConfigurationExtentions();
            new ConfigureFromConfigurationOptions<TokenConfigurationExtentions>(
                Configuration.GetSection("TokenConfigurations"))
                    .Configure(tokenConfigurations);
            services.AddSingleton(tokenConfigurations);


            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearerOptions =>
            {
                var paramsValidation = bearerOptions.TokenValidationParameters;
                paramsValidation.IssuerSigningKey = signingConfigurations.Key;
                paramsValidation.ValidAudience = tokenConfigurations.Audience;
                paramsValidation.ValidIssuer = tokenConfigurations.Issuer;

                paramsValidation.ValidateIssuerSigningKey = true;

                paramsValidation.ValidateLifetime = true;

                paramsValidation.ClockSkew = TimeSpan.Zero;
            });

            // Ativa o uso do token como forma de autorizar o acesso
            // a recursos deste projeto
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });


            services.AddCors(option => option.AddPolicy("Bearer", build =>
            {
                build
                .AllowAnyOrigin()
                .AllowCredentials()
                .AllowAnyMethod()
                .AllowAnyHeader();
            }));

            return services;
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMemoryCache cache, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/AppCV-Fiches-{Date}.log");
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; ;
                        context.Response.AddErrorHeaders();

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            var logger = loggerFactory.CreateLogger("Global Exception Filter");
                            logger.LogError(error.Error.Message + error.Error.StackTrace);
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message + error.Error.StackTrace);

                        }
                    });
                });
            }


            app.UseCors("Bearer");
            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());


            app.UseAuthentication();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseSession();
            //app.UseCors(p => p.WithOrigins("http://localhost:4200"));
            app.UseMvc(routes =>
            {
                routes.MapSpaFallbackRoute(
                   name: "spa-fallback", defaults: new { controller = "Fallback", action = "Index" }
                    );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "deleteUserFromRole",
                    template: "{controller=Admin}/{action}/{roleId}/User/{userId}");
            });

            LoadDataInCache(cache);


        }

        private void LoadDataInCache(IMemoryCache cache)
        {
            TechnologieGraphRepository technologieGraphRepository = new TechnologieGraphRepository();
            LangueGraphRepository langueGraphRepository = new LangueGraphRepository();
            FonctionGraphRepository fonctionGraphRepository = new FonctionGraphRepository();

            cache.Set("Technologies", technologieGraphRepository.GetAll());
            cache.Set("Langues", langueGraphRepository.GetAll());
            cache.Set("Fonctions", fonctionGraphRepository.GetAll());
        }
    }
}
