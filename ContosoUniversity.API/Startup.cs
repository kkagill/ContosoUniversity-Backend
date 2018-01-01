using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ContosoUniversity.Data;
using Scheduler.Data;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AspNet.Security.OpenIdConnect.Primitives;
using AutoMapper;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using ContosoUniversity.API.Provider;
using ContosoUniversity.Model.ViewModels.Mapping;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using ContosoUniversity.Data.Abstract;
using Scheduler.Data.Repositories;

namespace ContosoUniversity.API
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
            // Add database configurations  
            services.AddDbContext<ContosoContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("ContosoUniversity"),
                b => b.MigrationsAssembly("ContosoUniversity.API"));
                options.UseOpenIddict();
            });

            // Add initial data in database
            services.AddTransient<DbInitializer>();

            // Add membership
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.User.AllowedUserNameCharacters = null;

                // Confirmation email required for new account
                options.SignIn.RequireConfirmedEmail = true;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
                .AddEntityFrameworkStores<ContosoContext>()
                .AddDefaultTokenProviders();

            // Register the OAuth2 validation handler.
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                   .AddJwtBearer(options =>
                   {
                       options.Audience = "resource-server";
                       options.Authority = "http://localhost:51089/";
                       options.RequireHttpsMetadata = false;
                       options.IncludeErrorDetails = true;
                       options.TokenValidationParameters = new TokenValidationParameters
                       {
                           NameClaimType = OpenIdConnectConstants.Claims.Subject,
                           RoleClaimType = OpenIdConnectConstants.Claims.Role
                       };
                   });

            // Configure Identity to use the same JWT claims as OpenIddict instead
            // of the legacy WS-Federation claims it uses by default (ClaimTypes),
            // which saves you from doing the mapping in your authorization controller.
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });

            // Register the OpenIddict services.
            services.AddOpenIddict(options =>
            {
                // Register the Entity Framework stores.
                options.AddEntityFrameworkCoreStores<ContosoContext>();
                // Register the ASP.NET Core MVC binder used by OpenIddict.
                // Note: if you don't call this method, you won't be able to
                // bind OpenIdConnectRequest or OpenIdConnectResponse parameters.
                options.AddMvcBinders();
                // Enable the token endpoint.
                options.EnableTokenEndpoint("/connect/token");
                // Enable the password flow.
                options.AllowPasswordFlow();
                // During development, you can disable the HTTPS requirement.
                options.DisableHttpsRequirement();

                options.UseJsonWebTokens();
                options.AddEphemeralSigningKey();
            });

            // Automapper   
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            services.AddCors();
            services.AddMvc()
                .AddJsonOptions(opts =>
                {
                    // Force Camel Case to JSON
                    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            // Repositories
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IStudentsRepository, StudentsRepository>();
            services.AddScoped<ICoursesRepository, CoursesRepository>();
            services.AddScoped<IInstructorsRepository, InstructorsRepository>();
            services.AddScoped<IDepartmentsRepository, DepartmentsRepository>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            //services.AddScoped<IUserRepository, UserRepository>();

            // Without this controller actions are not forbidden if other roles are trying to access
            services.AddSingleton<IAuthenticationSchemeProvider, CustomAuthenticationSchemeProvider>();
            services.AddSingleton(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DbInitializer seeder)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            app.UseStaticFiles();

            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:4200");
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
            });

            app.UseExceptionHandler(
              builder =>
              {
                  builder.Run(
                    async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            //context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                        }
                    });
              });

            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
            app.UseWelcomePage();

            //seeder.InitializeData().Wait();
        }
    }
}
