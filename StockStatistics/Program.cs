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
            string url = "http://api.waditu.com";
            string requestParams = "{\"api_name\": \"stock_basic\", \"token\": \"e1bcce57a2b55596f167e114d298e8ebc6e95d2f5385937fd00f09d9\", \"params\": {\"list_stauts\":\"L\"}, \"fields\": \"ts_code,name,area,industry,list_date\"}";
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
            Console.WriteLine($"{result}");
            Console.ReadLine();
        }
    }
}
