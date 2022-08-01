using Anno.Plugs.AnalyseService.Dto;
using Anno.Repository;
using System;
using Anno.Domain.Dto;
using System.Collections.Generic;
using System.Text;
using SqlSugar;

namespace Anno.Plugs.AnalyseService.Analyse
{
    public class AnalyseQuery : IAnalyseQuery
    {
        private readonly SqlSugarClient _db;
        public AnalyseQuery(IBaseQueryRepository baseRepository)
        {
            _db = baseRepository.Context; 
        }
         

        public List<NameValueOutPutDto> GetServiceData(DateTime startDate, DateTime endDate, string type,bool? isError=false)
        {
            var rlt = _db.Queryable<Model.sys_trace>()
                .WhereIF(isError.HasValue&& isError.Value, it =>  it.Rlt  && it.Timespan >= startDate && it.Timespan < endDate)
                .WhereIF(isError.HasValue && !isError.Value, it => !it.Rlt && it.Timespan >= startDate && it.Timespan < endDate)
                .WhereIF(!isError.HasValue  , it => it.Timespan >= startDate && it.Timespan < endDate)
                .Select<NameValueOutPutDto>( type +" as name,count("+type+") as value")
                .GroupBy(type)
                .OrderBy("count(" + type + ")").Take(10).ToList();
            return rlt;  
        }

        public List<string> GetUserAnalyse(DateTime startDate, DateTime endDate)
        {
            var q1 = _db.Queryable<Model.sys_trace>()
              .Where(it => it.Timespan >= startDate && it.Timespan < endDate)
              .Select(it => new NameValueOutPutDto {
                  name = it.Ip,
                  value = SqlFunc.CharIndex( ":",it.Ip)-1 
              });

            var q2 = _db.Queryable(q1)
                .Select(it => SqlFunc.Substring(it.name, 0,it.value )).ToList();
            return q2; 
        }
       

        public List<NameValueOutPutDto> GetUserVisitDistribute(DateTime startDate, DateTime endDate)
        {

            var q1 = _db.Queryable<Model.sys_trace>()
              .Where(it => it.Timespan >= startDate && it.Timespan < endDate)
              .Select(it => new NameValueOutPutDto
              {
                  name = it.Timespan.Value.ToString("HH:00"),
                  value = 1
              });

            var rlt = _db.Queryable(q1)
                .Select(it => new NameValueOutPutDto { name = it.name, value = SqlFunc.AggregateCount(it.name) })
                .GroupBy(it => it.name).OrderBy(it => it.name).ToList();

            return rlt;   
        }

        public List<NameValueOutPutDto> GetUserVisitTrend(DateTime startDate, DateTime endDate)
        {
            var q1 = _db.Queryable<Model.sys_trace>()
               .Where(it => it.Timespan >= startDate && it.Timespan < endDate)
               .Select(it => new NameValueOutPutDto
               {
                   name = it.Timespan.Value.ToString("yyyy-MM-dd HH:00"),
                   value = 1
               });

            var rlt = _db.Queryable(q1)
                .Select(it => new NameValueOutPutDto { name=it.name,value= SqlFunc.AggregateCount(it.name) })
                .GroupBy(it=>it.name).OrderBy(it => it.name).ToList();

            return rlt; 
        }

  
    }
}
