using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Shopify.Helper
{
    public class EmailHelper
    {
        private readonly EmailConfiuration _emailConfiuration;


            public EmailHelper(IOptions<EmailConfiuration> emailConfiuration)
        {
            _emailConfiuration = emailConfiuration.Value;
        }
         public async Task<bool> SendEmailAsync(string userEmail , string token)
        {


            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_emailConfiuration.Email),
                Subject = "Reset Password"
            };

            email.To.Add(MailboxAddress.Parse(userEmail));

            var builder = new BodyBuilder();

            builder.HtmlBody = EmailBody(userEmail, token);
            email.Body = builder.ToMessageBody();
            email.From.Add(new MailboxAddress(_emailConfiuration.DisplayName, _emailConfiuration.Email));

            using var smtp = new SmtpClient();
            bool result = false;
            try
            {
                smtp.Connect(_emailConfiuration.Host, _emailConfiuration.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_emailConfiuration.Email, _emailConfiuration.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                result = true;
            }
            catch (Exception e) { }
            return result;
           
        }

        private static string EmailBody(string userEmail,string token)
        {
            //return "<div><h4>Shopify</h4>" +
            //    " <h3> Reset Password please click on this link</h3>" +
            //    "<a href='http://www.google.com?email="+userEmail+"&token="+token+"'> click here </a>" +
            //    "</div>";

            //string body = String.Empty;
            //using (StreamReader reader=new StreamReader( )
            //{

            //}

            var templetePath = $"{Directory.GetCurrentDirectory()}\\Templates\\ForgetPasswordPage.html";
            var str = new StreamReader(templetePath);
            var mailText = str.ReadToEnd();
            str.Close();

            mailText = mailText.Replace("[email]", userEmail).Replace("[token]", token);

            return mailText;
        }
    }
}
