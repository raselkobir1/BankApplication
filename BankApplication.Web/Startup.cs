using Bank.Application;
using Bank.Application.Repository.Implementation;
using Bank.Application.Repository.Interfaces;
using Bank.Entity.Core;
using Bank.Entity.MongoModels;
using Bank.Service;
using Bank.Service.Implementation;
using Bank.Service.Interface;
using Bank.Service.MongoServices;
using Bank.Utilities.EmailConfig;
using Bank.Utilities.EmailConfig.Models;
using Bank.Utilities.GlobalErrorHandler;
using Bank.Utilities.LoggerConfig;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using System.IO;
using System.Text;

namespace BankApplication.Web
{
    public class Startup
    {
        public string migrationAssemblyName;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("sqlConnection");

            services.AddDbContext<DatabaseContext>(optionsAction =>
            {
                optionsAction.UseSqlServer(connectionString, Options =>
                {
                    Options.MigrationsAssembly("BankApplication.Web");
                });
            });

            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            services.AddScoped<IRepositoryManager, RepositoryManager>();
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddScoped<IdentityServices>();
            services.AddSingleton<ILoggerManager, LoggerManager>();
            //-----mongoDb connection String settings
            services.Configure<SchoolDatabaseSettings>(Configuration.GetSection(nameof(SchoolDatabaseSettings)));
            services.AddSingleton<ISchoolDatabaseSettings, SchoolDatabaseSettings>(provider =>
            provider.GetRequiredService<IOptions<SchoolDatabaseSettings>>().Value);
            //----- mongo service registration
            services.AddScoped<StudentService>();
            services.AddScoped<CourseService>();

            // asp.net identity settings.
            services.AddIdentity<ApplicationUser, IdentityRole<long>>()
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders();

            //jwt settings
           services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidAudience = Configuration["JwtSettings:ValidAudience"],
                    ValidIssuer = Configuration["JwtSettings:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:Secret"]))
                };
            });

            services.AddHttpContextAccessor();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
                options.Password.RequiredUniqueChars = 1;
                // Default User settings.
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            });
            var emailConfig = Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
            services.AddScoped<IEmailSender, EmailSender>();

            services.AddControllers();
            services.AddControllersWithViews();

            //swager configuration for JWT Bearer Token
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BankApplication.Web", Version = "v1" , Description = "JWT authorization with swagger in asp.net core 5" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter Bearer [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\""
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DatabaseContext databaseContext)
        {
            bool AutoMigrationEnabled = Configuration.GetSection("MigrationSettings:AutoMigrate").Get<bool>();
            if (AutoMigrationEnabled)
            {
                databaseContext.Database.Migrate();
            }
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BankApplication.Web v1"));
            }

            app.UseMiddleware<ErrorHandlerMiddleware>();


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });
            //app.UseExceptionHandler();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(pattern: "confirm-email", name: "ConfirmEmail", defaults: new { controller = "Home", action = "ConfirmEmail" });
                endpoints.MapControllerRoute(pattern: "{*url}", name: "public", defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
