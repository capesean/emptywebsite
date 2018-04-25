using System;
using WEB.Models;
using System.Data.Entity;
using System.Net.Mail;
using System.IO;
using System.Data.Entity.Validation;

namespace WEB.Utilities
{
    public static class ErrorLogger
    {
        public static Guid ProcessExceptions(ApplicationDbContext db, Error error, Exception exc)
        {
            var InnerExceptionId = (Guid?)null;

            if (exc.InnerException != null)
                InnerExceptionId = ProcessExceptions(db, error, exc.InnerException);

            var Exception = new ErrorException
            {
                Id = Guid.NewGuid(),
                Message = exc.Message,
                StackTrace = exc.StackTrace,
                InnerExceptionId = InnerExceptionId
            };

            db.Entry(Exception).State = EntityState.Added;
            db.SaveChanges();

            return Exception.Id;
        }

        public static void Log(Exception exc, System.Web.HttpRequest request, string url, string userName)
        {
            if (exc.Message == "A task was canceled.") return;

            string form = string.Empty;
            foreach (var key in request.Form.AllKeys)
                form += key + ":" + request.Form[key] + Environment.NewLine;

            if (request.RequestType == "POST" && string.IsNullOrWhiteSpace(form))
            {
                using (StreamReader sr = new StreamReader(request.InputStream))
                {
                    if (request.InputStream.CanSeek) request.InputStream.Seek(0, SeekOrigin.Begin);
                    if (request.InputStream.CanRead) form = sr.ReadToEnd();
                }
            }

            if (!string.IsNullOrWhiteSpace(form) && form.IndexOf("password", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                form = "<REMOVED DUE TO PASSWORD SENSITIVITY>";
            }

            var entityValidationError = (string)null;
            try
            {
                if (exc is DbEntityValidationException)
                {
                    foreach (var eve in ((DbEntityValidationException)exc).EntityValidationErrors)
                    {
                        entityValidationError += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State) + Environment.NewLine;

                        foreach (var ve in eve.ValidationErrors)
                        {
                            entityValidationError += ve.PropertyName + " : " + ve.ErrorMessage + Environment.NewLine;
                        }
                    }
                }
            }
            catch { }

            using (var db = new ApplicationDbContext())
            {
                var error = new Error
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.UtcNow,
                    Message = exc.Message,
                    EntityValidationErrors = entityValidationError,
                    Url = url,
                    UserName = userName,
                    Form = form
                };

                error.ExceptionId = ProcessExceptions(db, error, exc);

                try
                {
                    db.Entry(error).State = EntityState.Added;
                    db.SaveChanges();
                }
                catch { }

                var settings = new Settings(db);
                if (!string.IsNullOrWhiteSpace(settings.EmailToErrors))
                {
                    var body = string.Empty;
                    body += "URL: " + error.Url + Environment.NewLine;
                    body += "DATE: " + error.Date.ToString("dd MMMM yyyy, HH:mm:ss") + Environment.NewLine;
                    body += "USER: " + error.UserName + Environment.NewLine;
                    body += "MESSAGE: " + error.Message + Environment.NewLine + Environment.NewLine + entityValidationError;
                    body += Environment.NewLine;
                    body += settings.RootUrl + "api/errors/" + error.Id + Environment.NewLine;

                    using (var mail = new MailMessage())
                    {
                        mail.To.Add(new MailAddress(settings.EmailToErrors));
                        mail.Subject = settings.SiteName + " Error";
                        mail.Body = body;
                        try
                        {
                            Email.SendMail(mail, settings);
                        }
                        catch { }
                    }
                }
            }
        }
    }
}
