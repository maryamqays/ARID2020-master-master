using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ARID.Data;
using ARID.Models;
using ARID.Services;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Hangfire;
using System;
using Newtonsoft.Json.Serialization;
//using DinkToPdf.Contracts;
//using DinkToPdf;
using ARID.Extensions;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Http;

namespace ARID
{
    public class Startup
    {
        private string DCI = "ar-LY";
        public Startup(IConfiguration configuration
            // for database transfere
            //, IHostingEnvironment env, IServiceProvider serviceProvider 
            )
        {
            Configuration = configuration;

            // for database transfere
            // var builder = new ConfigurationBuilder()
            //         .SetBasePath(env.ContentRootPath)
            //         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //         .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            // var mvcBuilder = serviceProvider.GetService<IMvcBuilder>();
            //// new MvcConfiguration().ConfigureMvc(mvcBuilder);

            // Configuration = builder.Build();
            // for database transfere
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // To allow downloading files
            //services.AddSingleton<IFileProvider>(
            //    new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            //https://stackoverflow.com/questions/44027058/how-to-configure-confirmation-email-token-lifespan-in-asp-net-core-mvc
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(30); // .FromDays(1) ...
            });

           // services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));


            // Add framework services.
            services.AddDbContext<ARIDDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        

            // services.AddDbContext<OLDARIDDB.Models.aridProductionDBContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("OldARIDConnection")));

            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = "256525281605634";
                facebookOptions.AppSecret = "1022b7ac56de1fd8488df11de0ec0f0d";


            });
            // For hangfire
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));
            services.AddCloudscribePagination();
            //https://stackoverflow.com/questions/50006589/how-to-properly-set-session-timeout-using-netcore-2-0-identity-and-cookie-authe
            services.Configure<SecurityStampValidatorOptions>(options =>
            options.ValidationInterval = TimeSpan.FromDays(150));

            services.AddIdentity<ApplicationUser, IdentityRole>(o =>
            {   // configure identity options
                o.User.RequireUniqueEmail = true;
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 3;
                o.SignIn.RequireConfirmedEmail = false; // to enable requiring email confirmation                
            })
                .AddEntityFrameworkStores<ARIDDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<ISmsSender, SmsSender>();
            services.AddTransient<IEmailSender, EmailSender>();

            //  services.AddTransient<IBulkDBUpdater, BulkDBUpdater>();

            // If you want to tweak Identity cookies, they're no longer part of IdentityOptions.
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/" + DCI + "/Account/LogIn";
                options.AccessDeniedPath = "/" + DCI + "/Account/AccessDenied/";
                options.LogoutPath = "/" + DCI + "/Account/Logout";
            });

            //services.AddAuthentication()
            //        .AddFacebook(options => {
            //            options.AppId = Configuration["auth:facebook:appid"];
            //            options.AppSecret = Configuration["auth:facebook:appsecret"];
            //        });
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromDays(30);//You can set Time
            });

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();
            //below line must be added for Telerik Autocomplete to works properly, otherwise it will not work. Above ; must be removed
            //.AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            //services.AddKendo();
            // Add localization services (StringLocalizer, HtmlLocalizer, etc.)
            // This will put our translations in a folder called Resources
            services.AddLocalization(options => { options.ResourcesPath = "Resources"; });

            // To Configure startup to use AuthMessageSenderOptions 
            services.Configure<AuthMessageSenderOptions>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseDeveloperExceptionPage();
            app.UseDatabaseErrorPage();
            app.UseBrowserLink();


            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseDatabaseErrorPage();
            //    app.UseBrowserLink();
            //}
            //else
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseDatabaseErrorPage();
            //    app.UseBrowserLink();
            //    app.UseExceptionHandler("/" + DCI + "/Home/Error");
            //}
            app.UseHangfireServer();
            // app.UseHangfireDashboard();
            app.UseHangfireDashboard("/hangfire");
            app.UseRewriter(new RewriteOptions().AddRedirectToHttps(StatusCodes.Status301MovedPermanently, 443));
          

            app.UseStaticFiles();
            app.UseSession();

            //app.UseKendo(env);

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            // UseAuthentication must be set before localization
            app.UseAuthentication();

         //   var context = new CustomAssemblyLoadContext();
         //   context.LoadUnmanagedLibrary(Path.Combine(env.WebRootPath, "DinkToPdf", "libwkhtmltox.dll"));

            // to add the request localization middleware
            IList<CultureInfo> supportedCultures = new List<CultureInfo>
            {                
              //  new CultureInfo("ar"),              
                new CultureInfo("ar-MA"),
                new CultureInfo("ar-DZ"),
                new CultureInfo("ar-TN"),
                new CultureInfo("ar-LY"),
                new CultureInfo("ar-EG"),
                new CultureInfo("ar-LB"),
                new CultureInfo("ar-JO"),
                new CultureInfo("ar-SY"),
                new CultureInfo("ar-IQ"),
                new CultureInfo("ar-KW"),
                new CultureInfo("ar-YE"),
                new CultureInfo("ar-OM"),
                new CultureInfo("ar-QA"),
                new CultureInfo("ar-AE"),
                new CultureInfo("ar-BH"),
              //  new CultureInfo("ar-SA"),
                new CultureInfo("ar-MR"),
                new CultureInfo("ar-SD"),
                new CultureInfo("ar-SO"),
                new CultureInfo("ar-DJ"),
                new CultureInfo("ar-KM"),
                new CultureInfo("en")
            };

            //var localizationOptions = new RequestLocalizationOptions
            //{
            //    DefaultRequestCulture = new RequestCulture(DCI),
            //    SupportedCultures = supportedCultures,
            //    SupportedUICultures = supportedCultures
            //};

            //var requestProvider = new RouteDataRequestCultureProvider
            //{
            //    Options = localizationOptions
            //};

            //localizationOptions.RequestCultureProviders.Insert(0, requestProvider);

            //app.UseRequestLocalization(localizationOptions);

            //app.UseRouter(routes =>
            //{
            //    routes.MapMiddlewareRoute("{culture=" + DCI + "}/{*mvcRoute}", subApp =>
            //    {
            //        subApp.UseRequestLocalization(localizationOptions);

            //        subApp.UseMvc(mvcRoutes =>
            //        {
            //            mvcRoutes.MapRoute(
            //                name: "default",
            //                template: "{culture=" + DCI + "}/{controller=Home}/{action=Index}/{id?}");
            //        });
            //    });
            //});

            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(DCI),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            var requestProvider = new RouteDataRequestCultureProvider
            {
                Options = localizationOptions
            };

            localizationOptions.RequestCultureProviders.Insert(0, requestProvider);

            app.UseRequestLocalization(localizationOptions);

            app.UseRouter(routes =>
            {
                routes.MapMiddlewareRoute("{culture=" + DCI + "}/{*mvcRoute}", subApp =>
                {
                    subApp.UseRequestLocalization(localizationOptions);



                    subApp.UseMvc(mvcRoutes =>
                    {
  //                      mvcRoutes.MapRoute(
  //name: "PostById",
  //template: "{culture=" + DCI + "}/Post/{id:Guid}/{t}",
  //defaults: new { controller = "Posts", action = "Details", t = String.Empty });

                        mvcRoutes.MapAreaRoute(
             name: "messages",
             areaName: "messages",
             template: "{culture=" + DCI + "}/messages/{controller}/{action}/{id?}");



                        mvcRoutes.MapRoute(
  name: "Blogs",
  template: "{culture=" + DCI + "}/Blog/{id:int}/{keyword}",
  defaults: new { controller = "Blogs", action = "Index", keyword = String.Empty });


                        mvcRoutes.MapRoute(
                            name: "default",
                            template: "{culture=" + DCI + "}/{controller=Home}/{action=Index}/{id?}");
                    });
                });
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "MyArea",
                  template: "{area:exists}/{controller}/{action}/{id?}");

                routes.MapRoute(
                   name: "default",
                   template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
