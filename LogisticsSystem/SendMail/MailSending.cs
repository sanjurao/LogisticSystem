using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;

namespace LogisticSystem.SendMail
{
    public class MailSending
    {
        public void sendMail(string To,string Subject,string Body)
        {
            try
            {
                #region formatter
                string text = string.Format("Message From mrclogisticssystem@gmail.com ", Subject, Body);
                string html = "You Have notification Message from mrclogisticssystem@gmail.com " + Body;             
                #endregion

                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("mrclogisticssystem@gmail.com");
                msg.IsBodyHtml = true;
                msg.To.Add(new MailAddress(To));
                msg.Subject = Subject;
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));

                var credentials = new NetworkCredential(
                           ConfigurationManager.AppSettings["mailAccount"],
                           ConfigurationManager.AppSettings["mailPassword"]
                           );
                //smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = credentials;
                smtpClient.EnableSsl = true;
                smtpClient.Send(msg);
            }
            catch { }
        }
    }
}