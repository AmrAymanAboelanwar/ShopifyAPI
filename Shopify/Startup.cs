using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shopify.Helper;
using Shopify.Models;
using Shopify.Repository;
using Shopify.Repository.Interfaces;
using Shopify.Services;
using Shopify.Services.Interfaces;
using Shopify.ViewModels.Facebook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Shopify
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        string MyAllowSpecificOrigins = "m";
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });
            });

          



            services.AddDbContext<ShopifyContext>(op =>
            {
                op.UseSqlServer(Configuration.GetConnectionString("myconnection"));
               // op.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("myconnection"));

            });


            services.AddHttpContextAccessor();
            services.AddSingleton<IUriService>(o =>
            {
                var accessor = o.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
                return new UriService(uri);
            });

            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ShopifyContext>().AddDefaultTokenProviders();

           
            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling =
                                                               Newtonsoft.Json.ReferenceLoopHandling.Ignore);


            services.Configure<JwtHelper>(Configuration.GetSection("JWT"));
            services.Configure<EmailConfiuration>(Configuration.GetSection("MailSettings"));
            services.Configure<FacebookSettings>(Configuration.GetSection("facebookConfig"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
               .AddJwtBearer(o =>
               {
                   o.RequireHttpsMetadata = false;
                   o.SaveToken = false;
                   o.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidIssuer = Configuration["JWT:Issuer"],
                       ValidAudience = Configuration["JWT:Audience"],
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]))
                   };
               });




            services.AddScoped<IAuthentication , AuthenticationRepo>();
            services.AddScoped<CustomerServices>();
            services.AddScoped<EmployeeService>();
            services.AddScoped<SellerService>();
            services.AddScoped<ManageRoles>();
            services.AddScoped<CategoryService>();
            services.AddScoped<SubCategoryService>();
            services.AddScoped<BrandService>();
            services.AddScoped<InventoryServices>();
            services.AddScoped<PromotionService>();
            services.AddScoped<ProductService>();
            services.AddScoped<RecentlyViewService>();
            services.AddScoped<EmailHelper>();
            services.AddScoped<StatusService>();
            services.AddScoped<GovernorateService>();
            services.AddScoped<CartService>();
            services.AddScoped<CartItemService>();
            services.AddScoped<FacebookService>();
            services.AddScoped<FacebookSettings>();
            services.AddScoped<ReviewService>();
            services.AddHttpClient();
            




            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shopify", Version = "v1" });
            });
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shopify v1"));
            }


            app.UseStaticFiles();// For the wwwroot folder

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                            Path.Combine(Directory.GetCurrentDirectory(), "Files")),
                RequestPath = "/Files"


            });


            //Enable directory browsing
            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(
                            Path.Combine(Directory.GetCurrentDirectory(), "Files")),
                RequestPath = "/Files"
            });

           

            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
