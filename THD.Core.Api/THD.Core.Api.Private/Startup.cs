
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.Globalization;
using System.Text;

using THD.Core.Api.Repository.DataContext;
using THD.Core.Api.Repository.Interface;
using THD.Core.Api.Repository.DataHandler;
using THD.Core.Api.Business.Interface;
using THD.Core.Api.Business;
using Microsoft.Net.Http.Headers;
using THD.Core.Api.Models.Config;
using THD.Core.Api.Helpers;

namespace THD.Core.Api.Private
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
            services.Configure<EnvironmentConfig>(Configuration.GetSection(nameof(EnvironmentConfig)));
            services.AddSingleton<IEnvironmentConfig>(sp => sp.GetRequiredService<IOptions<EnvironmentConfig>>().Value);

            services.Configure<EmailConfig>(Configuration.GetSection(nameof(EmailConfig)));
            services.AddSingleton<IEmailConfig>(sp => sp.GetRequiredService<IOptions<EmailConfig>>().Value);

            services.AddDbContext<DataContextProvider>(options =>
            {
                options.UseSqlServer(Encoding.UTF8.GetString(Convert.FromBase64String(Configuration.GetConnectionString("SqlConnection"))));
            });

            services.AddHttpClient();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TH Developer Service", Version = "v1" });
                var securityScheme = new OpenApiSecurityScheme()
                {
                    Description = "Please enter into field the word 'Bearer' followed by a space and the JWT value",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                };
                c.AddSecurityDefinition("Bearer", securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[] { } } });
            });

            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            //----------------------------------------------------------------------------------------------
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //Add Service
            services.AddScoped(typeof(IEmailHelper), typeof(EmailHelper));
            services.AddScoped(typeof(ISystemService), typeof(SystemService));

            //Add Repository
            services.AddScoped(typeof(IDocMenuHomeRepository), typeof(DocMenuHomeRepository));

            services.AddScoped(typeof(ISystemRepository), typeof(SystemRepository));
            services.AddScoped(typeof(IRegisterUserRepository), typeof(RegisterUserRepository));
            services.AddScoped(typeof(IDropdownListRepository), typeof(DropdownListRepository));
            services.AddScoped(typeof(IDocMeetingRoundRepository), typeof(DocMeetingRoundRepository));
            services.AddScoped(typeof(IMailTemplateRepository), typeof(MailTemplateRepository));

            services.AddScoped(typeof(IDocMenuA1Repository), typeof(DocMenuA1Repository));
            services.AddScoped(typeof(IDocMenuA2Repository), typeof(DocMenuA2Repository));
            services.AddScoped(typeof(IDocMenuA3Repository), typeof(DocMenuA3Repository));
            services.AddScoped(typeof(IDocMenuA4Repository), typeof(DocMenuA4Repository));
            services.AddScoped(typeof(IDocMenuA5Repository), typeof(DocMenuA5Repository));
            services.AddScoped(typeof(IDocMenuA6Repository), typeof(DocMenuA6Repository));
            services.AddScoped(typeof(IDocMenuA7Repository), typeof(DocMenuA7Repository));

            services.AddScoped(typeof(IDocMenuB1Repository), typeof(DocMenuB1Repository));
            services.AddScoped(typeof(IDocMenuB2Repository), typeof(DocMenuB2Repository));

            services.AddScoped(typeof(IDocMenuC1Repository), typeof(DocMenuC1Repository));
            services.AddScoped(typeof(IDocMenuC2Repository), typeof(DocMenuC2Repository));
            services.AddScoped(typeof(IDocMenuC3Repository), typeof(DocMenuC3Repository));

            services.AddScoped(typeof(IDocMenuC34HistoryRepository), typeof(DocMenuC34HistoryRepository));

            services.AddScoped(typeof(IDocMenuD1Repository), typeof(DocMenuD1Repository));
            services.AddScoped(typeof(IDocMenuD2Repository), typeof(DocMenuD2Repository));

            services.AddScoped(typeof(IDocMenuE1Repository), typeof(DocMenuE1Repository));

            services.AddScoped(typeof(IDocMenuF1Repository), typeof(DocMenuF1Repository));
            services.AddScoped(typeof(IDocMenuF2Repository), typeof(DocMenuF2Repository));

            services.AddScoped(typeof(IDocMenuR1Repository), typeof(DocMenuR1Repository));

            // Report Service
            services.AddScoped(typeof(IDocMenuReportService), typeof(DocMenuReportService));
            services.AddScoped(typeof(IDocMenuReportRepository), typeof(DocMenuReportRepository));

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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TH Developer Service V1");
            });

            app.UseMiddleware(typeof(AuthenticationMiddleware));
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
