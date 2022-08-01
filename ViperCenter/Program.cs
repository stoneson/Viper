using Anno.Log;
using Anno.Rpc.Center;
using System;

namespace ViperCenter
{
    class Program
    {
        /// <summary>
        /// 注册中心只用增加一个 Anno.config配置文件，然后直接 Bootstrap.StartUp(args);启动即可
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.Title = "ViperCenter";
            DingTalkNotify.CustomConfiguration.InitConst();
            Bootstrap.StartUp(args
                , (service, noticeType) =>//上线下线
                {
                    DingTalkNotify.Notice(service,noticeType);
                    
                    Log.WriteLine(noticeType.ToString() + ":\t\n" + Newtonsoft.Json.JsonConvert.SerializeObject(service));
                }, (newService, oldService) =>//服务配置更改
                {
                    DingTalkNotify.ChangeNotice(newService, oldService); 
                    
                    Log.WriteLine("NewConfig:\t\n" + Newtonsoft.Json.JsonConvert.SerializeObject(newService));
                    Log.WriteLine("OldConfig:\t\n" + Newtonsoft.Json.JsonConvert.SerializeObject(oldService));
                });
        }
    }
}
