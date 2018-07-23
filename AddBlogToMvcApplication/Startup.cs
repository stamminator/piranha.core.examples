﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.AspNetCore;
using Piranha.ImageSharp;
using Piranha.Local;

namespace AddBlogToMvcApplication
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config => 
            {
                config.ModelBinderProviders.Insert(0, new Piranha.Manager.Binders.AbstractModelBinderProvider());
            });
            services.AddPiranhaApplication();
            services.AddPiranhaFileStorage();
            services.AddPiranhaImageSharp();
            //
            // TODO: Change this to point to your current database
            //
            services.AddPiranhaEF(options => 
                options.UseSqlite("Filename=./piranha.blog.db"));
            services.AddPiranhaSimpleSecurity(new SimpleUser(Piranha.Manager.Permission.All()) 
            {
                UserName = "admin", Password = "password"
            });
            services.AddPiranhaManager();

            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Initialize Piranha
            var api = services.GetService<IApi>();
            App.Init(api);

            // Build content types
            var pageTypeBuilder = new Piranha.AttributeBuilder.PageTypeBuilder(api)
                .AddType(typeof(Models.BlogArchive));
            pageTypeBuilder.Build()
                .DeleteOrphans();
            var postTypeBuilder = new Piranha.AttributeBuilder.PostTypeBuilder(api)
                .AddType(typeof(Models.BlogPost));
            postTypeBuilder.Build()
                .DeleteOrphans();

            // Register middleware
            app.UseStaticFiles();

            // This security provider is only for development
            // purposes
            app.UsePiranhaSimpleSecurity();

            // Add the middleware needed for a blog
            app.UsePiranhaApplication();
            app.UsePiranhaAliases();
            app.UsePiranhaPosts();
            app.UsePiranhaArchives();
            app.UsePiranhaManager();

            app.UseMvc(routes => 
            {
                routes.MapRoute(name: "areaRoute",
                    template: "{area:exists}/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=home}/{action=index}/{id?}");
            });
        }
    }
}
