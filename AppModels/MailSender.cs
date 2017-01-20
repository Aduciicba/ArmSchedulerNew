﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.IO;

namespace KodeksArmScheduler
{
    public class MailSender
    {
        private string fromAlias = "АРМ ОТ";
        private string testEmail = "aduciicba@gmail.com";

        MailMessage message;
        SmtpClient smtp;

        public MailSender()
        {
            message = new MailMessage();
            smtp = new SmtpClient();

            try
            {
                message.From = new MailAddress(KodeksArmScheduler.Properties.Settings.Default.Email, fromAlias);
                smtp.Port = Int32.Parse(KodeksArmScheduler.Properties.Settings.Default.SmtpPort);
                smtp.Host = KodeksArmScheduler.Properties.Settings.Default.SmtpServer;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(KodeksArmScheduler.Properties.Settings.Default.EmailLogin, KodeksArmScheduler.Properties.Settings.Default.EmailPassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            }
            catch(Exception ex)
            {

            }
        }

        public MailSender(
                           string emailFrom
                         , int    smtpPort
                         , string smtpServer
                         , string emailLogin
                         , string emailPassword
                         )
        {
            message = new MailMessage();
            smtp = new SmtpClient();

            message.From = new MailAddress(emailFrom, fromAlias);
            smtp.Port = smtpPort;
            smtp.Host = smtpServer;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(emailLogin, emailPassword);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        }


        public string SendTestMessage()
        {
            try
            {
                message.To.Add(new MailAddress(testEmail));
                message.Subject = "Test";
                message.Body = "Content";
                smtp.Send(message);
                return "";
            }
            catch (Exception ex) 
            {
                return ex.Message;
            }
        }

        public bool SendMessage(
                                 string emailTo
                               , string subject
                               , string content)
        {
            try
            {
                message.To.Clear();
                message.To.Add(new MailAddress(emailTo));
                message.Subject = subject;
                message.Body = content;
                message.IsBodyHtml = true;
                smtp.Send(message);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string getTemplate(string templateName)
        {
            StreamReader reader = new StreamReader(templateName);
            string readFile = reader.ReadToEnd();
            return readFile;
        }
    }
}
