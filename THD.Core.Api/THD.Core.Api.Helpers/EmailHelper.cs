using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using THD.Core.Api.Models.Config;

namespace THD.Core.Api.Helpers
{
    public interface IEmailHelper
    {
        Task<bool> SentGmail(string to_email, string subject, string content, string base64Attachment);
    }
    public class EmailHelper : IEmailHelper
    {
        private readonly IEmailConfig _EmailConfig;
        public EmailHelper(IEmailConfig EmailConfig)
        {
            _EmailConfig = EmailConfig;
        }


        public async Task<bool> SentGmail(string to_email, string subject, string content, string base64Attachment)
        {
            MailMessage mail = new MailMessage();
            bool resp = false;

            SmtpClient client = new SmtpClient(_EmailConfig.Host);

            client.Port = Convert.ToInt32(_EmailConfig.Port);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(_EmailConfig.User, _EmailConfig.Pass);
            client.EnableSsl = true;
            client.Credentials = credentials;

            try
            {
                mail.From = new MailAddress(_EmailConfig.User);
                mail.To.Add(to_email);
                mail.Subject = subject;
                mail.Body = content;
                mail.IsBodyHtml = true;

                if (!string.IsNullOrEmpty(base64Attachment))
                {
                    string file_name = "nuibc_report.pdf";
                    string remove_content = "data:application/pdf;base64,";
                    string fileBase64 = base64Attachment.Replace(remove_content, "");

                    var bytes = Convert.FromBase64String(fileBase64);
                    MemoryStream strm = new MemoryStream(bytes);
                    Attachment data = new Attachment(strm, file_name);
                    ContentDisposition disposition = data.ContentDisposition;
                    data.ContentId = file_name;
                    data.ContentDisposition.Inline = true;
                    mail.Attachments.Add(data);
                }

                client.Send(mail);

                resp = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
            mail.Dispose();
            return resp;


        }


    }
}
