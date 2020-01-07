using System;
using System.Net;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Utility;

namespace StockStatistics
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://api.waditu.com";
            WebRequest request = WebRequest.Create(url);

            #region 设置Request的请求体

            string requestParams = "{\"api_name\": \"stock_basic\", \"token\": \"e1bcce57a2b55596f167e114d298e8ebc6e95d2f5385937fd00f09d9\", \"params\": {\"list_stauts\":\"L\"}, \"fields\": \"ts_code,name,area,industry,list_date\"}";
            byte[] requestParamsBytes = Encoding.UTF8.GetBytes(requestParams);
            request.Method = "post";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = requestParamsBytes.Length;
            Stream requestParamsStream = request.GetRequestStream();
            requestParamsStream.Write(requestParamsBytes, 0, requestParamsBytes.Length);
            requestParamsStream.Close();

            #endregion


            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.Default);
            string responseFromServer = reader.ReadToEnd();
            string result = Decode.Unicode2String(responseFromServer);
            Console.WriteLine(responseFromServer);
        }


    }
}
