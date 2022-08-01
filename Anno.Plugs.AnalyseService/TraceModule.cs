using System;
using System.Collections.Generic;
using System.Text;
using Anno.Infrastructure;
using Anno.Plugs.AnalyseService.Dto;
using Anno.EngineData;
using Anno.Plugs.AnalyseService.Analyse;

namespace Anno.Plugs.AnalyseService
{
    using Anno.Const.Attribute;
    using System.Linq;

    public class TraceModule : LogBaseModule
    {
        private readonly IAnalyseQuery _analyseQuery;
        private static IP2Region.DbSearcher _search = new IP2Region.DbSearcher(System.IO.Path.Combine(Environment.CurrentDirectory, "DB", "ip2region.db"));

        public TraceModule()
        {
            _analyseQuery = this.Resolve<IAnalyseQuery>(); 
        }

        [AnnoInfo(Desc = "服务访问量统计 Top 10")]
        public dynamic ServiceAnalyse([AnnoInfo(Desc = "开始时间（2021-03-30）")] DateTime startDate, [AnnoInfo(Desc = "结束时间（2021-03-31）")] DateTime endDate)
        {
            var rlt = _analyseQuery.GetServiceData(startDate, endDate, "AppNameTarget",null);
            return rlt;  
        }


        [AnnoInfo(Desc = "服务访问错误量统计 Top 10")]
        public dynamic ServiceErrorAnalyse([AnnoInfo(Desc = "开始时间（2021-03-30）")] DateTime startDate, [AnnoInfo(Desc = "结束时间（2021-03-31）")] DateTime endDate)
        {
            var rlt = _analyseQuery.GetServiceData(startDate, endDate , "AppNameTarget",true);
            return rlt;
           
        }

        [AnnoInfo(Desc = "服务管道访问统计 Top 10")]
        public dynamic ServiceModuleAnalyse([AnnoInfo(Desc = "开始时间（2021-03-30）")] DateTime startDate, [AnnoInfo(Desc = "结束时间（2021-03-31）")] DateTime endDate)
        {
            var rlt = _analyseQuery.GetServiceData(startDate, endDate, "Askchannel");
            return rlt; 
        }

        [AnnoInfo(Desc = "服务模块访问统计 Top 10")]
        public dynamic ServiceRouterAnalyse([AnnoInfo(Desc = "开始时间（2021-03-30）")] DateTime startDate, [AnnoInfo(Desc = "结束时间（2021-03-31）")] DateTime endDate)
        {
            var rlt = _analyseQuery.GetServiceData(startDate, endDate, "Askrouter");
            return rlt; 
        }

        [AnnoInfo(Desc = "服务方法访问统计 Top 10")]
        public dynamic ServiceMethodAnalyse([AnnoInfo(Desc = "开始时间（2021-03-30）")] DateTime startDate, [AnnoInfo(Desc = "结束时间（2021-03-31）")] DateTime endDate)
        {
            var rlt = _analyseQuery.GetServiceData(startDate, endDate, "Askmethod");
            return rlt;
           
        }

        [AnnoInfo(Desc = "服务方法Error访问统计 Top 10")]
        public dynamic ServiceMethodErrorAnalyse([AnnoInfo(Desc = "开始时间（2021-03-30）")] DateTime startDate, [AnnoInfo(Desc = "结束时间（2021-03-31）")] DateTime endDate)
        {
            var rlt = _analyseQuery.GetServiceData(startDate, endDate, "Askmethod",true);
            return rlt;
            

         
        }

        

        #region 用户访问全国分布
        [AnnoInfo(Desc = "用户访问全国分布 IP2Region 未知数据归北京 默认最近6个月数据 数据缓存20分钟")]
        [CacheLRU(100, 1200, false)]
        public dynamic UserAnalyse([AnnoInfo(Desc = "开始时间（2021-03-30）")] DateTime startDate, [AnnoInfo(Desc = "结束时间（2021-03-31）")] DateTime endDate)
        {
           var nameValues= InitData();

            if (startDate == null||startDate.Equals(DateTime.MinValue))
            {
                startDate = DateTime.Now.AddMonths(-6).AddDays(1);
            }
            if (endDate == null || endDate.Equals(DateTime.MinValue))
            {
                endDate = DateTime.Now.AddDays(1);
            }
            //string sql = @"SELECT SUBSTRING_INDEX(Ip,':',1) Ip FROM  sys_trace WHERE   Timespan>=?startDate AND Timespan<?endDate;  ";
            //var ips = DbInstance.Db.Ado.SqlQuery<string>(sql, new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") });
            var ips= _analyseQuery.GetUserAnalyse(startDate, endDate );
            foreach (var item in ips)
            { 
                if (!string.IsNullOrWhiteSpace(item))
                { 
                    var data = _search.MemorySearch(item).Region;
                    var datas = data.Split('|');
                    if (datas.Length >= 3)
                    {
                        NameValueOutPutDto nv;
                        if (datas[0] == "中国")
                        {
                            nv = nameValues.Find(n => n.name.Equals(datas[2].Trim(new char[] { '省' })));
                        }
                        else if (datas[0] == "0" && datas[3] == "内网IP")
                        {
                            nv = nameValues.Find(n => n.name.Equals("内网IP"));
                        }
                        else
                        {
                            nv = nameValues.Find(n => n.name.Equals("其它"));
                        } 

                        nv.value = nv.value + 1; 
                    }
                    else {
                        nameValues.Find(n => n.name.Equals("其它")).value++;
                    }
                }
                else {
                    nameValues.Find(n => n.name.Equals("其它")).value++;
                }
            }
            nameValues = nameValues.OrderByDescending(it => it.value).ToList();
            return nameValues;
        }

        private List<NameValueOutPutDto> InitData()
        {
            List<NameValueOutPutDto> nameValues = new List<NameValueOutPutDto>() {
                new NameValueOutPutDto() { name= "内网IP", value= 0 },
                new NameValueOutPutDto() { name= "其它", value= 0 },
                new NameValueOutPutDto() { name= "西藏", value= 0 },
                new NameValueOutPutDto() { name= "青海", value= 0 },
                new NameValueOutPutDto() { name= "宁夏", value= 0 },
                new NameValueOutPutDto() { name= "海南", value= 0 },
                new NameValueOutPutDto() { name= "甘肃", value= 0 },
                new NameValueOutPutDto() { name= "贵州", value= 0 },
                new NameValueOutPutDto() { name= "新疆", value= 0 },
                new NameValueOutPutDto() { name= "云南", value= 0 },
                new NameValueOutPutDto() { name= "重庆", value= 0 },
                new NameValueOutPutDto() { name= "吉林", value= 0 },
                new NameValueOutPutDto() { name= "山西", value= 0 },
                new NameValueOutPutDto() { name= "天津", value= 0 },
                new NameValueOutPutDto() { name= "江西", value= 0 },
                new NameValueOutPutDto() { name= "广西", value= 0 },
                new NameValueOutPutDto() { name= "陕西", value= 0 },
                new NameValueOutPutDto() { name= "黑龙江", value= 0 },
                new NameValueOutPutDto() { name= "内蒙古", value= 0 },
                new NameValueOutPutDto() { name= "安徽", value= 0 },
                new NameValueOutPutDto() { name= "北京", value= 0 },
                new NameValueOutPutDto() { name= "福建", value= 0 },
                new NameValueOutPutDto() { name= "上海", value= 0 },
                new NameValueOutPutDto() { name= "湖北", value= 0 },
                new NameValueOutPutDto() { name= "湖南", value= 0 },
                new NameValueOutPutDto() { name= "四川", value= 0 },
                new NameValueOutPutDto() { name= "辽宁", value= 0 },
                new NameValueOutPutDto() { name= "河北", value= 0 },
                new NameValueOutPutDto() { name= "河南", value= 0 },
                new NameValueOutPutDto() { name= "浙江", value= 0 },
                new NameValueOutPutDto() { name= "山东", value= 0 },
                new NameValueOutPutDto() { name= "江苏", value= 0 },
                new NameValueOutPutDto() { name= "广东", value= 0 },
                new NameValueOutPutDto() { name= "台湾", value= 0 },
                new NameValueOutPutDto() { name= "香港", value= 0 },
                new NameValueOutPutDto() { name= "澳门", value= 0 },
                new NameValueOutPutDto() { name= "南海诸岛", value= 0 }
            };

            return nameValues;
        }
        #endregion

        #region 用户访问走势
        [AnnoInfo(Desc = "用户访问走势")]
        public dynamic UserVisitTrend([AnnoInfo(Desc = "开始时间（2021-03-30）")] DateTime startDate, [AnnoInfo(Desc = "结束时间（2021-03-31）")] DateTime endDate) 
        {
            var rlt = _analyseQuery.GetUserVisitTrend(startDate, endDate);
            return new { xAxis = rlt.Select(it => it.value).ToList(), series = rlt.Select(it => it.name).ToList() };
        }

        [AnnoInfo(Desc = "用户访问一天之中时间分布")]
        public dynamic UserVisitDistribute([AnnoInfo(Desc = "开始时间（2021-03-30）")] DateTime startDate, [AnnoInfo(Desc = "结束时间（2021-03-31）")] DateTime endDate)
        {
            var rlt = _analyseQuery.GetUserVisitDistribute(startDate, endDate); 
            return new { xAxis = rlt.Select(it => it.value).ToList(), series = rlt.Select(it => it.name).ToList() };
        }
        #endregion
    }
}
