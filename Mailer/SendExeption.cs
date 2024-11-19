using System;
using System.Net.Mail;
using System.Threading;
using System.Web.Configuration;

namespace ClinicalTrialAPI.Mailer
{
    public sealed class SendExeption
    {
        #region Singleton
        private SendExeption() { }
        private static readonly Lazy<SendExeption> lazy = new Lazy<SendExeption>(() => new SendExeption());
        public static SendExeption Instance
        {
            get { return lazy.Value; }
        }
        #endregion

        static void MailSendingStart(object args)
        {
            ExceptionEntity _eE = (ExceptionEntity)args;
            SensMailToTeam(_eE);
        }

        public void sendExceptionMail(string FullException, string ClassName, string FunctionName, string ProcedureName, string ProjectName)
        {
            Thread thrd = new Thread(new ParameterizedThreadStart(MailSendingStart));
            ExceptionEntity _eE = new ExceptionEntity();
            _eE.ClassName = ClassName;
            _eE.Exception = FullException;
            _eE.FunctionName = FunctionName;
            _eE.ProcedureName = ProcedureName;
            _eE.ProjectName = ProjectName;
            thrd.Start(_eE);
        }

        private static void SensMailToTeam(ExceptionEntity _eE)
        {

            try
            {
                MailMessage mailMsg = new MailMessage();
                mailMsg.From = new MailAddress(WebConfigurationManager.AppSettings["AdminMail"]);
                mailMsg.To.Add(WebConfigurationManager.AppSettings["ExceptionEmail"]);
                mailMsg.IsBodyHtml = true;
                mailMsg.Subject = "New Exception in (" + _eE.ProjectName + ") Date: " + DateTime.Now.ToString("dd-MM-yyyy");
                mailMsg.Body = "Hi, <br /> <br />New Exception found <br /> <br /> Exception: " + _eE.Exception + "<br /> <br /> Class Name: " + _eE.ClassName + "<br /><br /> Function Name: " + _eE.FunctionName + "<br /><br /> Procedure Name" + _eE.ProcedureName;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 25;
                smtp.Credentials = new System.Net.NetworkCredential(WebConfigurationManager.AppSettings["AdminMail"], WebConfigurationManager.AppSettings["AdminEmailPassword"]);
                smtp.EnableSsl = true;
                mailMsg.IsBodyHtml = true;
                smtp.Send(mailMsg);

            }
            catch (Exception ex)
            {

            }

        }
    }
    public class ExceptionEntity
    {
        public string Exception { get; set; }
        public string ClassName { get; set; }
        public string FunctionName { get; set; }
        public string ProcedureName { get; set; }
        public string ProjectName { get; set; }

    }
}