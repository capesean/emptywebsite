using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEB.Models
{
    public class ReportException : Exception
    {
        public List<string> Errors { get; set; }

        public ReportException()
        {
            Errors = new List<string>();
        }

        public ReportException(List<string> errors)
        {
            Errors = errors;
        }

        public ReportException(string error)
        {
            Errors = new List<string> { error };
        }
    }
}