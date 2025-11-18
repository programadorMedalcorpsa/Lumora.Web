using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Text;
using SecurityHelper;
using Lumora.Web.Models; // Referencia a tu DLL de encriptación

var builder = WebApplication.CreateBuilder(args);

// Configuración DB (SQL Server por defecto, cambia a MySQL si necesitas)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)); // O UseMySql para MySQL

// Configuración JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.Configure<AppSettingsConfig>(builder.Configuration.GetSection("AppSettings"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Middleware para detección de primera ejecución (flag híbrido)
app.Use(async (context, next) =>
{
    var appConfig = context.RequestServices.GetRequiredService<IOptions<AppSettingsConfig>>().Value;
    var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();

    bool dbConnected = false;
    bool setupInDb = false;

    if (!string.IsNullOrEmpty(connectionString))
    {
        try
        {
            dbConnected = await dbContext.Database.CanConnectAsync();
            if (dbConnected)
            {
                var flag = await dbContext.SystemConfigs.FirstOrDefaultAsync(c => c.Key == "SetupCompleted");
                setupInDb = flag?.Value == "true";
            }
        }
        catch { }
    }

    bool isFullySetup = appConfig.SetupCompleted && dbConnected && setupInDb;

    if (!isFullySetup)
    {
        var path = context.Request.Path.Value ?? "";
        if (!path.StartsWith("/Setup") && !path.StartsWith("/Account/Login"))
        {
            context.Response.Redirect("/Account/Login");
            return;
        }
    }

    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();