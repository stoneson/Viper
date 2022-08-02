using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Anno.Const.Enum;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Viper.GetWay
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var urls = Anno.ArgsValue.GetValueByName("-h", args);
                    webBuilder.UseStartup<Startup>();
                    if (!string.IsNullOrWhiteSpace(urls))
                    {
                        webBuilder.UseUrls(urls);
                    }
                    /**
                     * active 为prod 的时候关闭日志 None，默认日志级别 Information
                     */
                    var active = Anno.ArgsValue.GetValueByName("-active", args);
                    LogLevel logLevel = LogLevel.Debug;
                    if (!string.IsNullOrWhiteSpace(active)&&active.Equals("prod"))
                    {
                        logLevel = LogLevel.None;
                    }
                    webBuilder
                        .UseAnnoSvc((setupFilter,endpoints) =>
                        {
                            endpoints.Map("SysMg/Api", context =>
                            {
                                return setupFilter.ApiInvoke(context, (input) =>
                                  {
                                      return Anno.Rpc.Client.Connector.BrokerDns(input);
                                  });
                            });
                            endpoints.Map("AnnoApi/ServiceInstance", ServiceInstanceApi);
                            endpoints.Map("AnnoApi/DeployService", DeployServiceApi);
                            //endpoints.Map(Eng.BasePath + "/{channel}/{router}/{method}/{param1?}/{param2?}/{param3?}/{param4?}/{param5?}", AnnoApi);
                        })
                        .ConfigureLogging(log => log.SetMinimumLevel(logLevel))
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseKestrel(option =>
                        {
                            option.Limits.MaxRequestBodySize = long.MaxValue;// 200 * 1024 * 1000;

                            //if (Anno.Const.AppSettings.GetAppSettings<bool>("UseHttps", "false"))
                            //{
                            //    option.ListenLocalhost(Anno.Const.AppSettings.GetAppSettings<int>("ListenPort", "5001"), opts => opts.UseHttps());
                            //}
                            //else
                            //{
                            //    option.ListenAnyIP(Anno.Const.AppSettings.GetAppSettings<int>("ListenPort", "5000"));
                            //}
                        }).ConfigureServices(service =>
                        {
                            service.Configure<FormOptions>(options =>
                            {
                                options.MultipartBodyLengthLimit = long.MaxValue;
                            });
                        });
                });

        private static async Task ServiceInstanceApi(HttpContext context)
        {
            context.Response.ContentType = "application/javascript; charset=utf-8";
            var instances = UtilService.GetServiceInstances();
            context.Response.StatusCode = 200;
            Dictionary<string, object> rlt = new Dictionary<string, object>();
            rlt.Add("output", null);
            rlt.Add("outputData", instances);
            rlt.Add("status", true);
            rlt.Add("msg", null);
            var rltExec = System.Text.Encoding.UTF8.GetString(rlt.ExecuteResult());
            await context.Response.WriteAsync(rltExec);
        }
        private static async Task DeployServiceApi(HttpContext context)
        {
            context.Response.ContentType = "application/javascript; charset=utf-8";
            var instances = UtilService.GetDeployServices();
            context.Response.StatusCode = 200;
            Dictionary<string, object> rlt = new Dictionary<string, object>();
            rlt.Add("output", null);
            rlt.Add("outputData", instances);
            rlt.Add("status", true);
            rlt.Add("msg", null);
            var rltExec = System.Text.Encoding.UTF8.GetString(rlt.ExecuteResult());
            await context.Response.WriteAsync(rltExec);
        }
    }
}
