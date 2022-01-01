using BlazorServerApp.DocxReader;
using BlazorServerApp.Models;
using BlazorServerApp.proccessService;
using BlazorServerApp.STMPMailer;
using BlazorServerApp.TextProcessor;
using BlazorServerApp.WordsAPI;
using DataLibrary;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;

namespace BlazorServerApp
{
    public class Startup
    {
        private readonly string _modelPath;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _modelPath = GetAbsolutePath("MLModel.zip");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // BLAZOR COOKIE Auth Code (begin)
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
            // BLAZOR COOKIE Auth Code (end)
            // ******

            services.AddConfiguration<EmailSettings>(Configuration, "EmailSettings");
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<IDataAccess, MySqlDataAccess>(); //Inject the mysql data access- could be changed to any data layer implementing IDataAcess. This launches it in memory- the overhead is worth it as data layer is vital.
            services.AddSingleton<IDocxReader, docxReader>();

            services.AddSingleton<IRecipeDataLoader, RecipeDataLoader>();
            services.AddSingleton<IFileManger, FileManager>();
            services.AddSingleton<IRecipeProcessorService, RecipeProcessorService>();
            services.AddSingleton<IWordsAPIService, WordsAPIService>();
            services.AddTransient<INounExtractor, BasicNounExtractor>();
            services.AddTransient<ITextProcessor, TextProcessor.TextProcessor>();
            services.AddTransient<IEmailSender, EmailSender>();
            //    services.AddPredictionEnginePool<ModelInput, ModelOutput>()
            //.FromFile(_modelPath);
            services.AddHttpClient();
            // ******
            // BLAZOR COOKIE Auth Code (begin)
            // From: https://github.com/aspnet/Blazor/issues/1554
            // HttpContextAccessor
            services.AddHttpContextAccessor();
            services.AddScoped<HttpContextAccessor>();
            services.AddHttpClient();
            services.AddScoped<HttpClient>();
            // BLAZOR COOKIE Auth Code (end)
            // ******
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // BLAZOR COOKIE Auth Code (begin)
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            // BLAZOR COOKIE Auth Code (end)
            // ******

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);
            return fullPath;
        }
    }

    public static class ConfigurationExtension
    {
        public static void AddConfiguration<T>(
            this IServiceCollection services,
            IConfiguration configuration,
            string configurationTag = null)
            where T : class
        {
            if (string.IsNullOrEmpty(configurationTag))
            {
                configurationTag = typeof(T).Name;
            }

            var instance = Activator.CreateInstance<T>();
            new ConfigureFromConfigurationOptions<T>(configuration.GetSection(configurationTag)).Configure(instance);
            services.AddSingleton(instance);
        }
    }
}