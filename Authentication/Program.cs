using Authentication.Data.Entities;
using Authentication.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Authentication.Services.Interfaces;
using Authentication.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
});


builder.Services.AddIdentity<AspNetUser,IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Lockout = new LockoutOptions()
    {
        AllowedForNewUsers = true,
        DefaultLockoutTimeSpan = TimeSpan.FromSeconds(30),
        MaxFailedAccessAttempts = 5
    };
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
}).AddEntityFrameworkStores<AuthDbContext>().AddDefaultTokenProviders();

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
        ValidIssuer = "PracticeIssuer",
        ValidAudience = "PracticeIssuer",
        RequireExpirationTime = true,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin()
        .Build();
    });
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.IOTimeout = TimeSpan.FromSeconds(60);
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.UseSwaggerUI(swagger =>
    {
        swagger.SwaggerEndpoint("/OpenAPI/v1.json", "OpenApi");
    });
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllers();

app.Run();