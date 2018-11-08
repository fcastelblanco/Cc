using System;
using System.Net;
using System.Net.Mail;

namespace Isn.Common.EmailHelper
{
    public class EmailSender
    {
        public static void SendEmail(
            string mailTo, string body, string subject, string emailFrom, 
            string smtpServer, int port, bool enableSsl, string emailSender, 
            string passwordServer)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailFrom),
                    To =
                    {
                        mailTo
                    },
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                var smtpClient = new SmtpClient
                {
                    Host = smtpServer,
                    Port = port,
                    EnableSsl = enableSsl,
                    Credentials = new NetworkCredential
                    {
                        UserName = emailSender,
                        Password = passwordServer
                    }
                };

                smtpClient.Send(mailMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}