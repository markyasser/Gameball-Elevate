using Gameball_Elevate.DbContexts;
using Gameball_Elevate.Models;
using Gameball_Elevate.Ops;
using Gameball_Elevate.Services;
using Gameball_Elevate.Services.SMTP;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using ZiggyCreatures.Caching.Fusion;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Database Connection
builder.Services.AddDbContext<GlobalDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories dependency injection
builder.Services.AddScoped<UserOps>();
builder.Services.AddScoped<TransactionOps>();

// Identity
builder.Services.AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<GlobalDbContext>()
        .AddDefaultTokenProviders();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Authorization 
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Customer", policy => policy.RequireRole("Customer"));
});

// For Sending Emails
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IEmailService, SmtpEmailService>();

// Hangfire configuration
builder.Services.AddHangfire(config => config
    .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

builder.Services.AddControllers();

// Add FusionCache
builder.Services.AddFusionCache(options =>
{
    options.DefaultEntryOptions = new FusionCacheEntryOptions
    {
        Duration = TimeSpan.FromMinutes(5)
    };
});

// Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter <SecurityRequirementsOperationFilter> ();
}
);
builder.Services.AddEndpointsApiExplorer();

// Add logging to the service collection
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHangfireDashboard();

app.MapControllers();

// Schedule the daily email job
RecurringJob.AddOrUpdate<DailyEmailService>(
    service => service.SendDailyEmails(),
    Cron.Daily(17, 0)); // 5:00 PM daily


// For testing only
// Schedule the job to run every 5 seconds
//RecurringJob.AddOrUpdate<DailyEmailService>(
//    service => service.SendDailyEmails(),
//    "*/5 * * * * *"); // Every 5 seconds
// Add logging to the service collection

app.Run();
