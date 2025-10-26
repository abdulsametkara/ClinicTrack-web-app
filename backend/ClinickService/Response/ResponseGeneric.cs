using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinickService.Response
{
    public class ResponseGeneric<T> : Response
    {
        public T Data { get; set; }
        public ResponseGeneric(T data, bool isSuccess, string message) : base(isSuccess, message)
        {
            Data = data;
        }

        public static ResponseGeneric<T> Success(T data, string message)
        {
            return new ResponseGeneric<T>(data, true, message);
        }

        public static ResponseGeneric<T> Error(string message)
        {
            return new ResponseGeneric<T>(default(T), false, message);
        }
    }
}
