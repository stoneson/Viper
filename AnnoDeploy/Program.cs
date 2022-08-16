using System;
using System.Linq;
using Anno.Loader;
using Anno.Log;
using Autofac;

namespace AnnoDeploy
{
    using Anno.Const;
    using Anno.EngineData;
    using Anno.Rpc.Server;
    static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Contains("start"))
            {
                ServiceControl.Start(args);
                return;
            }
            if (args.Contains("stop"))
            {
                ServiceControl.Stop(args);
                return;
            }
            if (args.Contains("status"))
            {
                ServiceControl.Status(args);
                return;
            }
            if (args.Contains("-help"))
            {
                Log.WriteLineNoDate("\r\n启动参数：\r\n                -p   6659                    设置启动端口\r\n                -xt  200                     设置服务最大线程数\r\n                -t   20000                   设置超时时间（单位毫秒）\r\n                -w   1                       设置权重\r\n                -h   192.168.0.2             设置服务在注册中心的地址\r\n                -tr  false                   设置调用链追踪是否启用\r\n");
                return;
            }
            Bootstrap.StartUp(args, delegate
            {
                SettingService.TraceOnOff = false;
                IocLoader.GetAutoFacContainerBuilder().RegisterType(typeof(RpcConnectorImpl)).As(new Type[]
                {
                    typeof(IRpcConnector)
                }).SingleInstance();
            }, delegate
            {
                Bootstrap.ApiDoc();
            }, IocType.Autofac);
        }
        //        static void Main(string[] args)
        //        {
        //            if (args.Contains("-help"))
        //            {
        //                Log.WriteLineAlignNoDate(@"
        //启动参数：
        //                -p     6659                    设置启动端口
        //                -xt    200                     设置服务最大线程数
        //                -t     20000                   设置超时时间（单位毫秒）
        //                -w     1                       设置权重
        //                -h     192.168.0.2             设置服务在注册中心的地址
        //                -tr    false                   设置调用链追踪是否启用
        //                --reg  false                   重新注册服务
        //");
        //                return;
        //            }
        //            /**
        //             * 启动默认DI库为 Autofac 可以切换为微软自带的DI库 DependencyInjection
        //             */
        //            Bootstrap.StartUp(args, () =>//服务配置文件读取完成后回调(服务未启动)
        //            {
        //                //Anno.Const.SettingService.TraceOnOff = true;

        //                /*
        //                 * 功能插件选择是Thrift还是 Grpc
        //                 * Install-Package Anno.Rpc.Client -Version 1.0.2.6 Thrift
        //                 * Install-Package Anno.Rpc.ServerGrpc -Version 1.0.1.5 Grpc
        //                 * 此处为 Thrift
        //                 */
        //                var autofac = IocLoader.GetAutoFacContainerBuilder();
        //                #region 自带依赖注入过滤器 true  注入 false 不注入
        //                //IocLoader.AddFilter((type) =>
        //                //       {
        //                //           return true;
        //                //       });
        //                #endregion
        //                /**
        //                 * IRpcConnector 是Anno.EngineData 内置的服务调用接口
        //                 * 例如：this.InvokeProcessor("Anno.Plugs.SoEasy", "AnnoSoEasy", "SayHi", input)
        //                 * IRpcConnector 接口用户可以自己实现也可以使用 Thrift或者Grpc Anno内置的实现
        //                 * 此处使用的是Thrift的实现
        //                 */
        //                autofac.RegisterType(typeof(RpcConnectorImpl)).As(typeof(IRpcConnector)).SingleInstance();
        //            }
        //            , () =>//服务启动后的回调方法
        //            {
        //                /**
        //                 * 服务Api文档写入注册中心
        //                 */
        //                Bootstrap.ApiDoc();
        //            });
        //        }
    }
}