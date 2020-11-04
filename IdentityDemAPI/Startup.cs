using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityDemo.API.BaseRepository;
using IdentityDemo.API.Entities;
using IdentityDemo.API.Services.Handle;
using IdentityDemo.API.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Shared.Users;

namespace IdentityDemo.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {       //SQL DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DbConnection")));
            // Register identity service
            services.AddIdentity<AppUser, AppRole>(options =>
             {
                 //Password.RequiredDigit is default true
                 //Password.RequiredLowerCase is default true
                 options.Password.RequiredLength = 5;
                 // Cấu hình Lockout - khóa user
                 options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
                 options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lần thì khóa
                 options.Lockout.AllowedForNewUsers = true;
                 options.User.RequireUniqueEmail = true;
                 // Cấu hình đăng nhập.
               /*  options.SignIn.RequireConfirmedEmail = true; */           // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                /* options.SignIn.RequireConfirmedPhoneNumber = false;*/     // Xác thực số điện thoại
             }).AddEntityFrameworkStores<ApplicationDbContext>() // lưu trữ thông tin identity trên EF( dbcontext->MySQL)
                 .AddDefaultTokenProviders();// register tokenprovider : phát sinh token (resetpassword, email...)
            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = Configuration["AuthSettings:Audience"],
                    ValidIssuer = Configuration["AuthSettings:Issuer"],
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AuthSettings:Key"])),
                };

            });

            //DI IProductService
            services.AddTransient<IProductService, ProductService>();
            //DI UserManager,SignInManager & RoleManager
            services.AddTransient<UserManager<AppUser>,UserManager<AppUser>>();
            services.AddTransient<SignInManager<AppUser>,SignInManager<AppUser>>();
            services.AddTransient<RoleManager<AppRole>, RoleManager<AppRole>>();
            //DI IUserService
            services.AddTransient<IUserService, UserService>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
