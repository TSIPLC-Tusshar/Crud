using Polly;

using Practice.Services;
using Practice.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();

builder.Services.AddHttpClient("httpClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:44349/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(3)))
.AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.CircuitBreakerAsync(5, TimeSpan.FromMinutes(1)));

builder.Services.AddSingleton<ApiService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
