using Microsoft.EntityFrameworkCore;

namespace MonthlyClaimsSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Basically it sets up MVC and the database context for SQL Server, very needed.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ClaimDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddSession();

            // Build the app.
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            // Which is basically how requests are handled, with error handling, HTTPS redirection, static files, routing, sessions, and authorization.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Middleware setup
            // Middleware is like a pipeline that processes requests and responses.
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // Enables session management for storing user data across requests.
            // We need this for login sessions and stuff.
            app.UseSession();
            app.UseAuthorization();

            // Sets up the default route for MVC controllers.
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Run the app.
            app.Run();
        }
    }
}