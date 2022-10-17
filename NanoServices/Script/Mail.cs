using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace NanoServices.Script
{
    public class Mail
    {
        private SmtpClient _smtpClient;
        private MailMessage _mailMessage;

        private string _domain;
        private string _hostSmtp;
        private int _port;
        private bool _enableSsl;
        private string _username;
        private string _login;
        private string _password;

        public Mail(MailParams mailParams)
        {
            _domain = mailParams.Domain;
            _hostSmtp = mailParams.HostSmtp;
            _port = mailParams.Port;
            _enableSsl = mailParams.EnableSsl;
            _username = mailParams.Username;
            _login = mailParams.Login;
            _password = mailParams.Password;
        }

        public async Task Send(string subject, string body, string to)
        {
            _mailMessage = new MailMessage();
            _mailMessage.From = new MailAddress(_login, _username);
            _mailMessage.To.Add(new MailAddress(to));
            _mailMessage.Subject = subject;
            _mailMessage.Body = body;
            _mailMessage.BodyEncoding = Encoding.UTF8;
            _mailMessage.SubjectEncoding = Encoding.UTF8;
            _mailMessage.IsBodyHtml = true;

            _smtpClient = new SmtpClient
            {
                Host = _hostSmtp,
                Port = _port,
                EnableSsl = _enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_username, _password, _domain)
            };

            _smtpClient.SendCompleted += OnSendCompleted;

            await _smtpClient.SendMailAsync(_mailMessage);
        }

        private void OnSendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            _smtpClient.Dispose();
            _mailMessage.Dispose();
        }
    }

    public class MailParams
    {
        public string Domain { get; set; }
        public string HostSmtp { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string Username { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
