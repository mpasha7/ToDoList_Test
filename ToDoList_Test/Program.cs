using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.OpenApi.Models;
using ToDoList_Test.Models;

namespace ToDoList_Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            // Добавление контекста базы данных SQL Server
            builder.Services.AddDbContext<ToDoContext>(opts =>
            {
                opts.UseSqlServer(builder.Configuration.GetConnectionString("ToDoConnection"));
            });
            // Добавление КЭШа и сессий
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(opts =>
            {
                opts.IdleTimeout = TimeSpan.FromMinutes(30);
                opts.Cookie.IsEssential = true;
            });
            // Добавление хранилища
            builder.Services.AddScoped<IRepository, ToDoRepository>();
            // Поддержка документирования Web API
            builder.Services.AddSwaggerGen(opts =>
            {
                opts.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WebApp",
                    Version = "v1"
                });
            });
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
            app.UseSession();
            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Login}/{id?}");

            app.MapControllerRoute(
                name: "api",
                pattern: "api/{controller}/{id?}");

            app.UseSwagger();
            app.UseSwaggerUI(opts =>
            {
                opts.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApp");
            });

            app.Run();
        }
    }
}
