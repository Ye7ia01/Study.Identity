using Identity.DAL;
using Identity.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.View
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("con")));


            // Add services to the container.
            builder.Services.AddControllersWithViews();


            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Set Password Complexity of Passwwords
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                //options.User.RequireUniqueEmail = true;

            })
            .AddEntityFrameworkStores<AppDbContext>()
            // The repository Layer (UserStore / RoleStore) of the Identity API to handle dealing with DB, thus taking the Models & DBContext as Paramters
            .AddUserStore<UserStore<ApplicationUser, ApplicationRole, AppDbContext, Guid>>()
            .AddDefaultTokenProviders()


            .AddRoleStore<RoleStore<ApplicationRole, AppDbContext, Guid>>();


            builder.Services.AddAuthorization(options => {


                options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            });

            builder.Services.ConfigureApplicationCookie(options => options.LoginPath = "/Account/Login");
        

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

            app.Run();
        }
    }
}
