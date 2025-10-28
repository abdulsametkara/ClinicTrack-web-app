using ClinickService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinickService.Response
{
    public class Responses : IResponses
    {
        public bool IsSuccess { get ; set ; }
        public string Message { get ; set ; }

        public Responses(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public static Responses Success(string message)
        {
            return new Responses(true, message);
        }

        public static Responses Error(string message)
        {
            return new Responses(false, message);
        }
    }
}
