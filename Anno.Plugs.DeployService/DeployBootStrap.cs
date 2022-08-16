using Anno.EngineData;
using Anno.Loader;
using Anno.Rpc.Client.DynamicProxy;
using Anno.ViperLog;
using Autofac;
#if NETSTANDARD
using Microsoft.Extensions.DependencyInjection;
#endif
using System;

namespace Anno.Plugs.DeployService
{
    /// <summary>
    /// 插件启动引导器
    /// DependsOn 依赖的类型程序集自动注入DI容器
    /// </summary>
    [DependsOn(
        typeof(ViperLog.ViperLogBootstrap)
       //, typeof(QueryServices.Bootstrap)
       //, typeof(Repository.Bootstrap)
       //, typeof(Command.Handler.Bootstrap
        )]
    public class DeployBootStrap : IPlugsConfigurationBootstrap
    {
        /// <summary>
        /// Service 依赖注入构建之后调用
        /// </summary>
        public void ConfigurationBootstrap()
        {
            ServiceMonitorTask.Start();
        }
        /// <summary>
        /// Service 依赖注入构建之前调用
        /// </summary>
        public void PreConfigurationBootstrap()
        {
            try
            {
                IocLoader.GetAutoFacContainerBuilder().RegisterInstance(AnnoProxyBuilder.GetService<ILogService>());
            }
            catch
            {
#if NETSTANDARD
                IocLoader.GetServiceDescriptors().AddSingleton(AnnoProxyBuilder.GetService<ILogService>());
#endif
            }
        }
    }
}
