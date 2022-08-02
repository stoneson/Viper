using Anno.Rpc.Center;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Flurl;
using Flurl.Http;
using Flurl.Util;

namespace ViperCenter
{
    public static class DingTalkNotify
    {
        public static string access_token = string.Empty;
        public static string access_keyWord = string.Empty;
        public static string notifyUrl = "https://oapi.dingtalk.com/robot/send?access_token=";

        public static void Notice(ServiceInfo service, NoticeType noticeType)
        {
            Init();

            var requestUrl = $"{notifyUrl}{access_token}";

            //long timestamp = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);
            //string sign = addSign(timestamp);
            //requestUrl = requestUrl + "&timestamp=" + timestamp + "&sign=" + sign;

            var rlt = requestUrl
                   .WithHeader("Content-Type", "application/json;charset=utf-8")
                   .PostJsonAsync(
                   new
                   {
                       msgtype = "markdown",
                       markdown = new
                       {
                           title = $"{service.NickName}-->{GetNoticeType(noticeType)}",
                           text = $"#### **{service.NickName} {GetNoticeType(noticeType)}** \n{BuildMsg(service)}"
                       }
                   }
                   ).ReceiveJson().Result;
        }

        public static void ChangeNotice(ServiceInfo newService, ServiceInfo oldService)
        {
            Init();

            var requestUrl = $"{notifyUrl}{access_token}";

            //long timestamp = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);
            //string sign = addSign(timestamp);
            //requestUrl = requestUrl + "&timestamp=" + timestamp + "&sign=" + sign;

            var rlt = requestUrl
                .WithHeader("Content-Type", "application/json;charset=utf-8")
                .PostJsonAsync(
                new
                {
                    msgtype = "markdown",
                    markdown = new
                    {
                        title = $"{oldService.NickName} --> {newService.NickName}",
                        text = $"#### 变更前:**{oldService.NickName}**\n{BuildMsg(oldService)} \n#### 变更后:**{newService.NickName}**\n{BuildMsg(newService)}"
                    }
                }
                ).ReceiveJson().Result;
        }
        public static string GetNoticeType(NoticeType noticeType) {
            switch (noticeType)
            {
                case NoticeType.OnLine:
                    return "服务上线";
                    case NoticeType.OffLine:
                    return "服务下线";
                case NoticeType.NotHealth:
                    return "服务不可用";
                case NoticeType.RecoverHealth:
                    return "服务恢复正常";
                default:
                    return "未知状态";
            }
        }

        public static string BuildMsg(ServiceInfo service)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"> #### 消息来源：**{access_keyWord}**");
            sb.AppendLine($"> #### 服务地址：**{service.Ip}:{service.Port}**");
            sb.AppendLine($"> #### 超时时间：**{service.Timeout}**(毫秒)");
            sb.AppendLine($"> #### 服务权重：**{service.Weight}**");
            sb.AppendLine($"> #### 服务标签:");
            string[] tags = service.Name.Split(',');
            foreach (string tag in tags)
            {
                sb.AppendLine($">  - {tag}");
            }
            sb.AppendLine($"\n#### 预警时间:**{DateTime.Now:yyyy:MM:dd HH:mm:ss}**");
            return sb.ToString();
        }
        public static void Init()
        {
            if (string.IsNullOrWhiteSpace(access_token))
            {
                CustomConfiguration.Settings.TryGetValue("DingTalkNotifyToken", out access_token);
            }

            if (string.IsNullOrWhiteSpace(access_keyWord))
            {
                CustomConfiguration.Settings.TryGetValue("DingTalkNotifyKeyWord", out access_keyWord);
            }
            if (string.IsNullOrWhiteSpace(access_keyWord))
            {
                access_keyWord = "注册中心";
            }
        }
        private static string addSign(long timestamp)
        {
            string secret1 = access_token;
            string stringToSign = timestamp + "\n" + secret1;
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret1);
            byte[] messageBytes = encoding.GetBytes(stringToSign);
            using (var hmacsha256 = new System.Security.Cryptography.HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return System.Web.HttpUtility.UrlEncode(Convert.ToBase64String(hashmessage), Encoding.UTF8);
            }
        }
        /// <summary>
        /// 自定义参数
        /// </summary>
        public static class CustomConfiguration
        {
            private static readonly Dictionary<string, string> settings = new Dictionary<string, string>();
            /// <summary>
            ///  自定义参数
            /// //appSettings
            /// </summary>
            public static Dictionary<string, string> Settings => settings;

            public static void InitConst(string docName = "Anno.config")
            {
                string xmlPath = Path.Combine(Directory.GetCurrentDirectory(), docName);
                XmlDocument xmlDoc = new XmlDocument();
                if (File.Exists(xmlPath))
                {
                    xmlDoc.Load(xmlPath);
                }
                else
                {
                    return;
                }
                XmlNode appSettings = xmlDoc.SelectSingleNode("//appSettings");
                if (appSettings == null || appSettings.ChildNodes.Count <= 0)
                {
                    return;
                }

                foreach (XmlNode node in appSettings.ChildNodes)
                {
                    try
                    {
                        if (node.Attributes != null)
                        {
                            string key = node.Attributes["key"].Value;
                            if (!string.IsNullOrWhiteSpace(key) && settings.ContainsKey(key) == false)
                            {
                                string value = node.Attributes["value"].Value;
                                settings.Add(key, value);
                            }
                        }
                    }
                    catch
                    {
                        //
                    }
                }
            }
        }
    }
}
