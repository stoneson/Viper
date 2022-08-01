using Anno.Plugs.AnalyseService.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Plugs.AnalyseService.Analyse
{
    public interface IAnalyseQuery
    {
        //List<NameValueOutPutDto> GetVisitTop(DateTime startDate, DateTime endDate);
        //List<NameValueOutPutDto> GetErrorTop(DateTime startDate, DateTime endDate);   
        List<NameValueOutPutDto> GetServiceData(DateTime startDate, DateTime endDate, string type, bool? isError = false);
        List<string> GetUserAnalyse(DateTime startDate, DateTime endDate );
        List<NameValueOutPutDto> GetUserVisitTrend(DateTime startDate, DateTime endDate );
        List<NameValueOutPutDto> GetUserVisitDistribute(DateTime startDate, DateTime endDate );
    }
}
