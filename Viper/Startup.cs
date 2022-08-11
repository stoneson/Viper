using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anno;
using Anno.Api.GateWay;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Viper.GetWay
{
    public class Startup
    {
        //object startupFilter;
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //���cors ���� ���ÿ�����   
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder =>
                {
                    builder.AllowAnyHeader().AllowAnyMethod()//.WithMethods("GET", "POST", "HEAD", "PUT", "DELETE", "OPTIONS")
                    //.AllowCredentials()//ָ������cookie
                .AllowAnyOrigin(); //�����κ���Դ����������
                });
            });
            services.AddSingleton<Hubs.TaskManager, Hubs.TaskManager>();
            services.AddMvc();
            services.AddSignalR();
            services.AddResponseCompression(options =>
            {
                //options.Providers.Add<BrotliCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
            });
            services.AddAnnoSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error/Index/500");
                app.UseStatusCodePagesWithReExecute("/error/Index/404");
            }
            app.UseCors("any");
           
            app.UseResponseCompression();
            //���MIME
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".vue"] = "text/plain";
            //ʹ�þ�̬�ļ�Ĭ�ϵ��ļ���Ϊwwwroot
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<Hubs.SystemHub>("/SystemHub");
                endpoints.MapHub<Hubs.MonitorHub>("/MonitorHub");
                endpoints.MapControllerRoute("default", "{Controller=Home}/{action=Index}/{id?}");
            });
            //app.Use(next => new RequestDelegate(
            //     async context =>
            //     {
            //         context.Request.EnableBuffering();
            //         await next(context);
            //     }
            //   ));
            app.UseAnnoSwagger(env);
        }
    }
}
