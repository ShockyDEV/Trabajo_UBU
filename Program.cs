var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register ICapaDatos with its implementation
builder.Services.AddSingleton<ICapaDatos, DBGenTree>();

// Configure cookie policy for development environments
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None; // Allow cookies with SameSite=None for local testing
    options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.None; // No HTTPS restriction for localhost
});

builder.Services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", options =>
{
    options.LoginPath = "/Index";
    options.Cookie.SameSite = SameSiteMode.None; // Allow SameSite=None for local testing
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // No HTTPS restriction for localhost
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
});


var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy(); // Apply cookie policy configurations
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
