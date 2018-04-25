using System;

namespace WEB.Models
{
    public class ErrorDTO
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string Message { get; set; }

        public string Url { get; set; }

        public string Form { get; set; }

        public string UserName { get; set; }

        public Guid ExceptionId { get; set; }
        public virtual ErrorExceptionDTO Exception { get; set; }
    }

    public class ErrorExceptionDTO
    {
        public Guid Id { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public Guid? InnerExceptionId { get; set; }
        public virtual ErrorExceptionDTO InnerException { get; set; }
    }

    public partial class ModelFactory
    {
        public ErrorDTO Create(Error error)
        {
            if (error == null) return null;

            return new ErrorDTO
            {
                Id = error.Id,
                Date = error.Date,
                Message = error.Message,
                Url = error.Url,
                Form = error.Form,
                UserName = error.UserName,
                ExceptionId = error.ExceptionId,
                Exception = Create(error.Exception)
            };
        }

        public ErrorExceptionDTO Create(ErrorException exception)
        {
            if (exception == null) return null;

            return new ErrorExceptionDTO
            {
                Id = exception.Id,
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                InnerExceptionId = exception.InnerExceptionId,
                InnerException = Create(exception.InnerException)
            };
        }
    }
}