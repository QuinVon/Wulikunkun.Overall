using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class ResponseModel
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public object Data { get; set; }

        public ResponseModel(string message, int statusCode, object data)
        {
            this.Message = message;
            this.StatusCode = statusCode;
            this.Data = data;
        }

    }
}
