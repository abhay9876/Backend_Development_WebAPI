using FundooNotes.Business.Interface;
using FundooNotes.Business.Services;
using FundooNotes.Repository.Context;
using FundooNotes.Repository.Interfaces;
using FundooNotes.Repository.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

namespace FundooNotes
{
    public class Program
    {
        public static void Main(string[] args)
        {


            Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console()
                        .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                        .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);  // Create webapplication builder object, it is like a container where we can add logginf, DI , Configuration etc.
            builder.Host.UseSerilog();

            try
            {
                Log.Information("Starting FundooNotes API...");
                builder.Services.AddControllers();   //  builder,Services == ASP.NET Core ka built-in Dependency Injection container hai.


                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("Development", policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
                });
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();

                // Swagger Config
                builder.Services.AddSwaggerGen(options =>
                {
                    // JWT Authorization setup
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Enter JWT Token like: Bearer {your_token}"
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
                });

                // EF Register
                builder.Services.AddDbContext<AppDbContext>(options =>

                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            // Redis Register
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration["RedisCacheOptions:Configuration"];
                options.InstanceName = builder.Configuration["RedisCacheOptions:InstanceName"];
            });

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserBL, UserBL>();
            builder.Services.AddScoped<INoteRepository, NoteRepository>();
            builder.Services.AddScoped<INoteBL, NoteBL>();


            // JWT
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
                app.UseCors("Development");
                app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
