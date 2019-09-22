using System;
using System.Net;
using System.Net.Mail;

namespace RealProject.Classes
{
    public static class MailSender
    {
        private const string SendersAddress = "mahmut.softeal@gmail.com";
        private const string SendersPassword = "WulIgtM5zk";

        public static void SendMail(string recipent,string subject,string body)
        {
            try
            {
                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(SendersAddress, SendersPassword),
                    Timeout = 3000
                };

                MailMessage message = new MailMessage(SendersAddress, recipent, subject, body);

                smtp.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
