using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Wulikunkun.Utility;
using Newtonsoft.Json;
using Wulikunkun.StockStatistic.Models;

namespace Wulikunkun.StockStatistic
{
    public class RequestResult
    {
        public string Request_Id { get; set; }
        public int Code { get; set; }
        public string Msg { get; set; }
        public DataResult Data { get; set; }
    }

    public class DataResult
    {
        public List<string> Fields { get; set; }
        public List<List<string>> Items { get; set; }
        public bool Has_More { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            SyncStockList();
        }

        public static RequestResult RequestData(string requestParams)
        {
            string url = "http://api.waditu.com";
            byte[] requestParamsBytes = Encoding.UTF8.GetBytes(requestParams);
            WebRequest request = WebRequest.Create(url);
            request.Method = "post";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = requestParamsBytes.Length;
            Stream requestParamsStream = request.GetRequestStream();
            requestParamsStream.Write(requestParamsBytes, 0, requestParamsBytes.Length);
            requestParamsStream.Close();
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.Default);
            string responseFromServer = reader.ReadToEnd();
            string result = Decode.Unicode2String(responseFromServer);
            RequestResult requestResult = JsonConvert.DeserializeObject<RequestResult>(result);
            return requestResult;
        }

        public static void SyncNews()
        {
            string requestParams =
                "{\"api_name\": \"major_news\", \"token\": \"e1bcce57a2b55596f167e114d298e8ebc6e95d2f5385937fd00f09d9\", \"params\": {\"start_date\":\"2020-01-17 00:00:00\",\"end_date\":\"2020-01-18 00:00:00\"},\"fields\": \"title,content,pub_time,src\"}";
            RequestResult requestResult = RequestData(requestParams);
        }

        public static void SyncDailyPrice(string ts_code)
        {
            string requestParams =
                "{\"api_name\": \"daily\", \"token\": \"e1bcce57a2b55596f167e114d298e8ebc6e95d2f5385937fd00f09d9\", \"params\": {\"ts_code\":\"" +
                ts_code + "\",\"start_date\":\"20200101\",\"end_date\":\"20200117\"}}";
            RequestResult requestResult = RequestData(requestParams);
            using (StockContext context = new StockContext())
            {
                IList<ExchangeCalendar> exchangeCalendars = new List<ExchangeCalendar>();
                foreach (List<string> list in requestResult.Data.Items)
                {
                    ExchangeCalendar exchangeCalendar = new ExchangeCalendar()
                    {
                        Exchange = list[0],
                        Cal_Date = list[1],
                        Is_Open = list[2].Equals("1") ? true : false
                    };
                    exchangeCalendars.Add(exchangeCalendar);
                }
                context.ExchangeCalendars.AddRange(exchangeCalendars);
                context.SaveChanges();
            }
        }

        public static void SyncTradeCalendar()
        {
            string requestParams =
                "{\"api_name\": \"trade_cal\", \"token\": \"e1bcce57a2b55596f167e114d298e8ebc6e95d2f5385937fd00f09d9\", \"params\": {\"exchange\":\"\",\"start_date\":\"20200101\",\"end_date\":\"20201231\"}}";
            RequestResult requestResult = RequestData(requestParams);
            using (StockContext context = new StockContext())
            {
                IList<ExchangeCalendar> exchangeCalendars = new List<ExchangeCalendar>();
                foreach (List<string> list in requestResult.Data.Items)
                {
                    ExchangeCalendar exchangeCalendar = new ExchangeCalendar()
                    {
                        Exchange = list[0],
                        Cal_Date = list[1],
                        Is_Open = list[2].Equals("1") ? true : false
                    };
                    exchangeCalendars.Add(exchangeCalendar);
                }

                context.ExchangeCalendars.AddRange(exchangeCalendars);
                context.SaveChanges();
            }
        }

        public static void SyncStockList()
        {
            string requestParams =
                "{\"api_name\": \"stock_basic\", \"token\": \"e1bcce57a2b55596f167e114d298e8ebc6e95d2f5385937fd00f09d9\", \"params\": {\"list_stauts\":\"L\"}, \"fields\": \"ts_code,name,area,industry,list_date\"}";
            RequestResult requestResult = RequestData(requestParams);
            using (StockContext context = new StockContext())
            {
                IList<StockBasicInfo> stockBasicInfos = new List<StockBasicInfo>();
                foreach (List<string> list in requestResult.Data.Items)
                {
                    StockBasicInfo stockBasicInfo = new StockBasicInfo()
                    {
                        Ts_Code = list[0],
                        Name = list[1],
                        Area = list[2],
                        Industry = list[3],
                        List_Date = list[4]
                    };
                    stockBasicInfos.Add(stockBasicInfo);
                }

                context.StockBasicInfos.AddRange(stockBasicInfos);
                context.SaveChanges();
            }
        }
    }
}