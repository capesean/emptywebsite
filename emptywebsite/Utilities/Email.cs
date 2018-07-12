using WEB.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System;

namespace WEB.Utilities
{
    public static class Email
    {
        public static void SendMail(MailMessage mailMessage, Settings settings)
        {
            if (mailMessage.From == null) mailMessage.From = new MailAddress(settings.EmailFromAddress, settings.EmailFromName);

            using (var smtp = new SmtpClient(settings.EmailSMTP, settings.EmailPort))
            {
                smtp.Credentials = new NetworkCredential(settings.EmailUserName, settings.EmailPassword);
                smtp.EnableSsl = settings.EmailSSL;

                if (!string.IsNullOrWhiteSpace(settings.SubstitutionAddress))
                {
                    // substitute all TO emails
                    var replacements = new List<MailAddress>();
                    foreach (var address in mailMessage.To) replacements.Add(new MailAddress(settings.SubstitutionAddress, address.DisplayName));
                    mailMessage.To.Clear();
                    foreach (var address in replacements) mailMessage.To.Add(address);
                    // substitute all CC emails
                    replacements = new List<MailAddress>();
                    foreach (var address in mailMessage.CC) replacements.Add(new MailAddress(settings.SubstitutionAddress, address.DisplayName));
                    mailMessage.CC.Clear();
                    foreach (var address in replacements) mailMessage.CC.Add(address);
                    // substitute all BCC emails
                    replacements = new List<MailAddress>();
                    foreach (var address in mailMessage.Bcc) replacements.Add(new MailAddress(settings.SubstitutionAddress, address.DisplayName));
                    mailMessage.Bcc.Clear();
                    foreach (var address in replacements) mailMessage.Bcc.Add(address);

                }

                var html = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "templates/email.html");
                html = html.Replace("{rootUrl}", settings.RootUrl);
                html = html.Replace("{title}", mailMessage.Subject);
                var body = mailMessage.Body;
                while (body.IndexOf(Environment.NewLine + Environment.NewLine) >= 0)
                    body = body.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
                var lines = "<p>" + string.Join("</p><p>", mailMessage.Body.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)) + "</p>";
                html = html.Replace("{body}", lines);
                mailMessage.Body = html;
                mailMessage.IsBodyHtml = true;

                // send if not local OR substitution address
                if (!System.Web.HttpContext.Current.Request.IsLocal || !string.IsNullOrWhiteSpace(settings.SubstitutionAddress))
                    smtp.Send(mailMessage);
            }
        }
    }
}