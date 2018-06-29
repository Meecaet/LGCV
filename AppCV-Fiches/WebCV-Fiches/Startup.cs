using System;
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

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMemoryCache cache)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "deleteUserFromRole",
                    template: "{controller=Admin}/{action}/{roleId}/User/{userId}");
            });

            LoadDataInCache(cache);
        }

        private  void LoadDataInCache(IMemoryCache cache)
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
