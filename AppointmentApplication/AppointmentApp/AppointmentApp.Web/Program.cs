using AppointmentApp.Domain.Identity;
using AppointmentApp.Repository;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using AppointmentApp.Service.Interface;
using AppointmentApp.Service.Implementation;
using AppointmentApp.Domain.Models;
using AppointmentApp.Repository.Interface;
using AppointmentApp.Repository.Implementation;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<AppointmentAppUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;

    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;

    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.MaxFailedAccessAttempts = 5;

    options.User.RequireUniqueEmail = true;
})
    .AddRoles<IdentityRole>()  
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

        googleOptions.Scope.Add("profile");
        googleOptions.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
        googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");

        googleOptions.Events.OnCreatingTicket = async context =>
        {
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<AppointmentAppUser>>();
            var email = context.Principal?.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email)) return;

            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                bool isUpdated = false;

                var givenName = context.Principal?.FindFirstValue(ClaimTypes.GivenName);
                var surname = context.Principal?.FindFirstValue(ClaimTypes.Surname);

                if (string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(givenName))
                {
                    user.FirstName = givenName;
                    isUpdated = true;
                }
                if (string.IsNullOrEmpty(user.LastName) && !string.IsNullOrEmpty(surname))
                {
                    user.LastName = surname;
                    isUpdated = true;
                }

                
                var roles = await userManager.GetRolesAsync(user);
                if (roles.Count == 0)
                {
                    await userManager.AddToRoleAsync(user, "Client");
                }

                if (!user.EmailConfirmed)
                {
                    user.EmailConfirmed = true;
                    isUpdated = true;
                }

                if (isUpdated)
                {
                    await userManager.UpdateAsync(user);
                }
            }
        };
    });


builder.Services.AddControllersWithViews();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddTransient<IOfferingService, OfferingService>();
builder.Services.AddTransient<IReservationService, ReservationService>();
builder.Services.AddTransient<IDataFetchService, DataFetchService>();

builder.Services.AddHttpClient();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); 

app.Run();
