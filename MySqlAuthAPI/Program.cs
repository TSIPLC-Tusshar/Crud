using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using MySqlAuthAPI.Data;
using MySqlAuthAPI.Data.Entities;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDbContext<MySqlDbContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("Database"),
        serverVersion: ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Database")));
}, contextLifetime: ServiceLifetime.Singleton, optionsLifetime: ServiceLifetime.Scoped);

builder.Services.AddIdentity<Aspnetuser, IdentityRole>(options =>
{
    options.Lockout = new LockoutOptions()
    {
        AllowedForNewUsers = true,
        DefaultLockoutTimeSpan = TimeSpan.FromSeconds(30),
        MaxFailedAccessAttempts = 5
    };

    options.Password = new PasswordOptions()
    {
        RequireDigit = true,
        RequiredLength = 6,
        RequiredUniqueChars = 1,
        RequireLowercase = true,
        RequireNonAlphanumeric = true,
        RequireUppercase = true
    };
}).AddEntityFrameworkStores<MySqlDbContext>().AddDefaultTokenProviders();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().Build();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("lnO0xg5lemIXjvxSgMqkffuJv0eGrKHB")),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "MySqlIssuer",
        ValidAudience = "MySqlIssuer",
        RequireExpirationTime = true,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.IOTimeout = TimeSpan.FromSeconds(60);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllers();

app.Run();
