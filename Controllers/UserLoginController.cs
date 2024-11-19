using ClinicalTrialAPI.Custom;
using ClinicalTrialAPI.eDelegation;
using ClinicalTrialAPI.ePlanner;
using ClinicalTrialAPI.Models;
using ClinicalTrialAPI.Reminder;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;

namespace ClinicalTrialAPI.Controllers
{
    [ValidationActionFilter]
    public class UserLoginController : ApiController
    {
        LoginAuditRecord _loginAuditInstance = Custom.LoginAuditRecord.LoginAuditInstance;
        SendNotificationToApp _appInstance = SendNotificationToApp._AppNotiObj;
        #region staff Login
        [HttpPost]
        [ActionName("User_Login")]
        public HttpResponseMessage User_Login(StaffLogin _StaffLogin)
        {

            return Request.CreateResponse(HttpStatusCode.OK, eDelegationSetUp.Instance.StaffLoginFun(_StaffLogin), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("GetChagedSessionValues")]
        public HttpResponseMessage GetChagedSessionValues(StaffInfoRef _staffId)
        {

            return Request.CreateResponse(HttpStatusCode.OK, eDelegationSetUp.Instance.GetChangedSessionValuesFun(_staffId), Configuration.Formatters.JsonFormatter);
        }
        #endregion

        #region Login/Logout audit
        [HttpPost]
        [ActionName("LoginAuditRecord")]
        public HttpResponseMessage LoginAuditRecord(StaffLoginDetails _StaffLoginDetails)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _loginAuditInstance.LogSessionInOut(_StaffLoginDetails), Configuration.Formatters.JsonFormatter);
        }

        #endregion

        #region TermAndCondition Save
        [HttpPost]
        [ActionName("SaveTermsAndConditionsFlag")]
        public HttpResponseMessage SaveTermsAndConditionsFlag(StaffLoginDetails _staffLogin)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _loginAuditInstance.SaveTermsAndConditionsFlag(_staffLogin), Configuration.Formatters.JsonFormatter);
        }
        #endregion

        #region change password
        [HttpPost]
        [ActionName("ChangePwd")]
        public HttpResponseMessage ChangePwd(ChangePassword _cp)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _loginAuditInstance.ChangePwdFun(_cp), Configuration.Formatters.JsonFormatter);
        }
        #endregion

        #region change AccessCode
        [HttpPost]
        [ActionName("ChangeAccessCode")]
        public HttpResponseMessage ChangeAccessCode(ChangeAccessCode _ca)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _loginAuditInstance.ChangeAccessCodeFun(_ca), Configuration.Formatters.JsonFormatter);
        }
        #endregion

        #region change AccessCode
        [HttpGet]
        [ActionName("CheckEmailId")]
        public HttpResponseMessage CheckEmailId(string Email)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _loginAuditInstance.CheckEmailIdFun(Email), Configuration.Formatters.JsonFormatter);
        }
        #endregion

        #region change AccessCode
        [HttpPost]
        [ActionName("SetNewPassword")]
        public HttpResponseMessage SetNewPasswordFun(ResetNewPassword _rpd)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _loginAuditInstance.SetNewPasswordFun(_rpd), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("ValidateOTP")]
        public HttpResponseMessage ValidateOTP(ValidateOTP _rpd)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _loginAuditInstance.ValidateOTP(_rpd), Configuration.Formatters.JsonFormatter);
        }

        #endregion

        #region change AccessCode
        [HttpPost]
        [ActionName("SwitchUser")]
        public HttpResponseMessage SwitchUser(StaffLogin _StaffLogin)
        {
            return Request.CreateResponse(HttpStatusCode.OK, eDelegationSetUp.Instance.SwitchUser(_StaffLogin), Configuration.Formatters.JsonFormatter);
        }
        #endregion

        #region Save/update/get/deactive Contact Info
        [HttpPost]
        [ActionName("ContactInfo")]
        public HttpResponseMessage ContactInfo(ContactInfo _ContactInfo)
        {
            return Request.CreateResponse(HttpStatusCode.OK, eDelegationSetUp.Instance.ContactInfo(_ContactInfo), Configuration.Formatters.JsonFormatter);
        }
        #endregion

        #region
        [HttpPost]
        [ActionName("VedioConfOTP")]
        public HttpResponseMessage VedioConfOTP(ConferenceRoom _conferenceRoom)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _loginAuditInstance.VedioConfOTPFun(_conferenceRoom), Configuration.Formatters.JsonFormatter);
        }
        #endregion

        #region Use here for APiURL validation remove
        [HttpPost]
        [ActionName("VirtualConsultation")]
        public HttpResponseMessage VirtualConsultation(VirtualConsultation _virtualConsultation)
        {
            return Request.CreateResponse(HttpStatusCode.OK, ePlannerSetUp.Instance.VirtualConsultation(_virtualConsultation), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("SaveFeedback")]
        public HttpResponseMessage SaveFeedback(Feedback _feedback)
        {
            return Request.CreateResponse(HttpStatusCode.OK, ePlannerSetUp.Instance.SaveFeedback(_feedback), Configuration.Formatters.JsonFormatter);
        }
        #endregion

        [HttpPost]
        [ActionName("AppFAQ")]
        public HttpResponseMessage AppFAQ(FAQ _fAQ)
        {
            return Request.CreateResponse(HttpStatusCode.OK, ePlannerSetUp.Instance.GetAppFaq(_fAQ), Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
        public string sendSMS()
        {
            ReminderBLayer bsLayer = ReminderBLayer.Instance;
            string OrgId = "37E4A74B-CBC8-4531-9D81-910BF6686F7E";
            System.Collections.Generic.List<VolDetails> vdetail = bsLayer.VolunteerListForSMS(OrgId);
            string response = string.Empty;
            string responseString = string.Empty;

            foreach (var list in vdetail)
            {

                if (list.MobileNumber != null && list.MobileNumber != "0")
                {
                //    HttpWebRequest SmsRequest;

                //    SmsRequest = (HttpWebRequest)WebRequest.Create("https://faretext-api.co.uk:9443/api?action=sendmessage&username=Sidqam2&password=S1dkan6&recipient=" + list.MobileNumber + "&messagedata=" + list.msgBody + "&Originator=" + list.OrganizationId);


                //    HttpWebResponse SmsResponse = (HttpWebResponse)SmsRequest.GetResponse();
                //    StreamReader SmsStreamReader = new StreamReader(SmsResponse.GetResponseStream());
                //    responseString = SmsStreamReader.ReadToEnd();
                //    SmsStreamReader.Close();
                //    SmsResponse.Close();
                //    response = responseString;
                //    bsLayer.SaveSMSLog(list.MobileNumber, list.msgBody, list.OrganizationId);
                }
            }
            return "Done";
        }


        [HttpGet]
        public string WeeklySMS()
        {

            string OrgId = "051AAA42-4CAD-45AB-B5CC-327E942136EB";
            ReminderBLayer bsLayer = ReminderBLayer.Instance;

            DBAccess.DAL _DAL = new DBAccess.DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
            System.Data.SqlClient.SqlParameter[] _SqlParameter = new System.Data.SqlClient.SqlParameter[0];
            DataTable dt = _DAL.ExecuteCommand("[dbo].[usp_WeeklySMS]", System.Data.CommandType.StoredProcedure, _SqlParameter);

            foreach (DataRow dr in dt.Rows)
            {
                string MobileNumber = Convert.ToString(dr["ContactNumber"]);

                string msgBody = "Dear " + Convert.ToString(dr["ForeName"]) + ", " + "\n" + "Thank you for your time and participation on the Sanofi Pasteur COVID-19 booster vaccine trial. Are you currently experiencing any symptoms/conditions of a COVID-19-like illness, have you recently tested positive for COVID-19, or have you received any other vaccine since we last saw you? If so, please contact your study site staff as you may need to organise a safety visit. Telephone: 01928 753 303 or e-mail: whh.halton.cru@nhs.net " + "\n" +
                              "To receive study payment/compensation for this contact please acknowledge receipt by emailing whh.halton.cru@nhs.net, noting the approximate date and time you read the text. " + "\n" +
                              "Yours Sincerely, " + "\n" + "Dr Toong Chin " + "\n" + "Halton Clinical Research Unit";


                HttpWebRequest SmsRequest;
                SmsRequest = (HttpWebRequest)WebRequest.Create("https://faretext-api.co.uk:9443/api?action=sendmessage&username=Sidqam2&password=S1dkan6&recipient=" + MobileNumber + "&messagedata=" + msgBody + "&Originator=" + OrgId);
                HttpWebResponse SmsResponse = (HttpWebResponse)SmsRequest.GetResponse();
                StreamReader SmsStreamReader = new StreamReader(SmsResponse.GetResponseStream());
                string responseString = SmsStreamReader.ReadToEnd();
                SmsStreamReader.Close();
                SmsResponse.Close();
                string response = responseString;
               // bsLayer.SaveSMSLog(MobileNumber, msgBody, OrgId);
            }


            return "Done";
        }
        //[HttpGet]
        //[ActionName("SendNotification")]
        //public HttpResponseMessage SendNotification()
        //{
        //    _appInstance.SendSingleAndroidNotification("Testing", "Testing msg", "eGH47x2qSx6wq7HDk8-Fmq:APA91bGy4JO1ksq-5OW7YctQNYcsu_AECjLPZmTy9vEmeRxO5QcxB4f3DBa0JuYDxx1TTdTpaTxq2-2IBfMLlMmDVsQe5ApvlexD5rcTp2W2glgWeIb8ppLUS0V-fxvjpCFIeCrT5rtW", "Android");
        //    return Request.CreateResponse(HttpStatusCode.OK, "", Configuration.Formatters.JsonFormatter);
        //}
    }
}
