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
            string url = "http://192.168.107.252:8056/monitor/cross/version";
            WebRequest request = WebRequest.Create(url);
            //get请求不能设置请求体，但是可以设置请求头
            request.Method = "get";
            request.Headers.Add("token", "token");
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.Default);
            string responseFromServer = reader.ReadToEnd();
            string result = Decode.Unicode2String(responseFromServer);
            Console.WriteLine(result);
            Console.Read();
        }


    }
}
