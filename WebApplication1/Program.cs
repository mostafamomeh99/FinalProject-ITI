
using DatabaseConnection;
using Hangfire;
using IRepositoryService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Models;
using RepositoryFactory;
using Stripe;
using System.Security.Claims;
using System.Text;
using WebApplication1.BackGroundJobs;
using WebApplication1.Middlewares.ExecptionFeatures;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(op =>
            {
                op.AddPolicy("AllowAnyUser", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
                

            });
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddTransient<IOtpService, OtpService>();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddHttpClient<UnitOfWork>();


            builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHangfireServer();
            builder.Services.AddTransient<DailyJob>();

            //RecurringJob.AddOrUpdate
            //    ("daily-trip-check", (DailyJob job) => job.Run(), "0 0 * * *", TimeZoneInfo.Utc);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
 .AddJwtBearer(options =>
 {
     var secretKeys = builder.Configuration.GetSection("Jwt:SecretKeys").Get<List<string>>();

     options.TokenValidationParameters = new TokenValidationParameters
     {         ValidateIssuer = true,
         ValidateAudience = false,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = "https://localhost:7028",
         RoleClaimType = ClaimTypes.Role,
         IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
         {
             // check key in token with list of secret keys
             return secretKeys.Select(secret =>
                 new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)));
         }
     };
 });
            //strip configuration

            StripeConfiguration.ApiKey = builder.Configuration["StripeSettings:SecretKey"];
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<HandleExecption>();

            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images")),
                RequestPath = "/images"
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("AllowAnyUser");
            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

                recurringJobManager.AddOrUpdate<DailyJob>(
                    "daily-trip-check",
                    job => job.Run(),
                    "0 0 * * *",
                    TimeZoneInfo.Utc
                );
            }

            app.Run();
        }
    }
}
