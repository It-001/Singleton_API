using ClinicalTrialAPI.Custom;
using ClinicalTrialAPI.Settings;
using ClinicalTrialAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ClinicalTrialAPI.Reminder;
using ClinicalTrialAPI.eDelegation;

namespace ClinicalTrialAPI.Controllers
{
    [CustomAuthorize]
    public class SettingsController : ApiController
    {

        #region Study Activity
        [HttpPost]
        [ActionName("StudyActivity")]
        public HttpResponseMessage StudyActivity(StudyActivity _studyActivity)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.StudyActivity(_studyActivity), Configuration.Formatters.JsonFormatter);
        }
        #endregion

        [ActionName("SaveECommSetup")]
        public HttpResponseMessage SaveECommSetup(ECommunicationSetup _eCommunicationSetup)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.SaveECommSetup(_eCommunicationSetup), Configuration.Formatters.JsonFormatter);
        }

        [ActionName("OrganizationSetUpActivity")]
        public HttpResponseMessage OrganizationSetUp(OrganizationSetup _organizationSetup)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.OrganizationSetUpActivity(_organizationSetup), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("GetAnalytics")]
        public HttpResponseMessage GetAnalytics(Analytics _analytics)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.AnalyticsActivity(_analytics), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("StartSchedulerAction")]
        public HttpResponseMessage StartSchedulerAction(ScheduledReminder _schRe)
        {
            return Request.CreateResponse(HttpStatusCode.OK, Scheduler.Instance.StartScheduler(_schRe), Configuration.Formatters.JsonFormatter);
        }


        [HttpPost]
        [ActionName("StopSchedulerAction")]
        public HttpResponseMessage StopSchedulerAction(ScheduledReminder _schRe)
        {
            return Request.CreateResponse(HttpStatusCode.OK, Scheduler.Instance.DeleteScheduler(_schRe), Configuration.Formatters.JsonFormatter);
        }


        [HttpPost]
        [ActionName("SetupSchedulerAction")]
        public HttpResponseMessage SetupSchedulerAction(ScheduledReminder _schRe)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.SetupScheduler(_schRe), Configuration.Formatters.JsonFormatter);
        }

        #region Check for Reminder status
        [HttpGet]
        [ActionName("ReminderCheckLists")]
        public HttpResponseMessage ReminderCheckLists()
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.ReminderCheckList(), Configuration.Formatters.JsonFormatter);
        }
        #endregion


        [HttpPost]
        [ActionName("UpdateStdystatus")]
        public HttpResponseMessage UpdateStdystatus(StudyActivity _Studystatusupdated)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.SaveStudyStats(_Studystatusupdated), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("StudyCROMappingActivity")]
        public HttpResponseMessage StudyCROMappingActivity(StudyCROMapping _studyCROMapping)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.StudyCROMappingActivity(_studyCROMapping), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("ImportData")]
        public HttpResponseMessage ImportData(ImportData _importData)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.ImportDataActivity(_importData), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("AddCRO")]
        public HttpResponseMessage AddCRO(ClinicalTrialAPI.Models.CRO _CRO)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.AddCROActivity(_CRO), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("LocationSetUpActivity")]
        public HttpResponseMessage LocationSetUpActivity(Location _location)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.LocationSetUpActivity(_location), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("CROOrganizationMappingActivity")]
        public HttpResponseMessage CROOrganizationMappingActivity(CROOrganizationActivity _CROOrganizationActivity)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.CROOrganizationMappingActivity(_CROOrganizationActivity), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("CreateOrganization")]
        public HttpResponseMessage CreateOrganization(OrgSetup _orgSetup)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.CreateOrganizationActivity(_orgSetup), Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
        [ActionName("GetOrganizationsDetails")]
        public HttpResponseMessage GetOrganizationsDetails()
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.GetOrganizationsDetailsActivity(), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("DepartmentActivity")]
        public HttpResponseMessage DepartmentActivity(Department _department)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.DepartmentSetup(_department), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("DepartmentRoleAssignAcivity")]
        public HttpResponseMessage DepartmentRoleAssignAcivity(DepartmentRole _departmentRole)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.DepartmentRoleAssignAcivity(_departmentRole), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("GetSuperAdminAnalytics")]
        public HttpResponseMessage GetSuperAdminAnalytics(StaffRoleAnalytics _staffRoleAnalytics)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.SuperAdminAnalyticsActivity(_staffRoleAnalytics), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("StudyWiseAppointment")]
        public HttpResponseMessage StudyWiseAppointment(Analytics _analytics)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.StudyWiseAppointment(_analytics), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("StudyWiseVolunteers")]
        public HttpResponseMessage StudyWiseVolunteers(VolunteerFilters _volunteerFilters)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.StudyWiseVolunteers(_volunteerFilters), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("SendEmailSMS")]
        public HttpResponseMessage SendEmailSMS(EmailSMS _emailSMS)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.SendEmailSMS(_emailSMS), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("GetUserDashboard")]
        public HttpResponseMessage GetUserDashboard(UserDashboard _userDashboard)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.GetUserDashboard(_userDashboard), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("ChangeOrganization")]
        public HttpResponseMessage ChangeOrganization(ChangeOrganizationReq _chorg)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.ChangeOrganizationFun(_chorg), Configuration.Formatters.JsonFormatter);
        }
        [HttpGet]
        [ActionName("GetMyOrganization")]
        public HttpResponseMessage GetMyOrganization(string EmailId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.GetMyOrganizationFun(EmailId), Configuration.Formatters.JsonFormatter);
        }


        [HttpPost]
        [ActionName("StudyTaskActivity")]
        public HttpResponseMessage StudyTaskActivity(StudyTaskActivity _studyTaskActivity)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.StudyTaskActivity(_studyTaskActivity), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("PaymentTemplateActivity")]
        public HttpResponseMessage PaymentTemplateActivity(PaymentEmailTemplate _paymentEmailTemplate)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.PaymentTemplateActivity(_paymentEmailTemplate), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("SMSCount")]
        public HttpResponseMessage SMSCount(GetSMSCountReq getSMSCountReq)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.SMSCountFun(getSMSCountReq), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("UsersVolunteersMatrices")]
        public HttpResponseMessage UsersVolunteersMatrices(UsersVolunteersMatricies _UsersVolunteersMatricies)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.UsersVolunteersMatriciesActivity(_UsersVolunteersMatricies), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("GetOWAppAnalytics")]
        public HttpResponseMessage GetOWAppointment(OWAnalytics _OWAnalytics)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.GetOWAppointmnetAnalytics(_OWAnalytics), Configuration.Formatters.JsonFormatter);
        }

        #region StudyLocationVisits
        [HttpPost]
        [ActionName("StudyLocationVisits")]
        public HttpResponseMessage StudyLocationVisits(StudyLocationVisits _studyLocationVisits)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.StudyLocationVisits(_studyLocationVisits), Configuration.Formatters.JsonFormatter);
        }
        #endregion

        [HttpPost]
        [ActionName("LocationOccupancyAnalytics")]
        public HttpResponseMessage LocationOccupancyAnalytics(LocationOccupancyAnalytics _locationOccupancyAnalytics)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.LocationOccupancyAnalyticsActivity(_locationOccupancyAnalytics), Configuration.Formatters.JsonFormatter);
        }

        #region ReminderSetUp
        [HttpPost]
        [ActionName("ReminderSetUp")]
        public HttpResponseMessage ReminderSetUp(AppointmentReminder _req)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.SetReminder(_req), Configuration.Formatters.JsonFormatter);
        }
        #endregion

        #region ReminderSetUp
        [HttpPost]
        [ActionName("CheckReminderNameExists")]
        public HttpResponseMessage CheckReminderNameExists(AppointmentReminder _req)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.CheckReminderNameExists(_req), Configuration.Formatters.JsonFormatter);
        }
        #endregion

        //rihan ahmad
        #region StaffDocument
        [HttpPost]
        [ActionName("SaveStaffDocument")]
        public HttpResponseMessage SaveStaffDocument(SaveStaffDocuments _saveStaffDocuments)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.SaveStaffDocumentFun(_saveStaffDocuments), Configuration.Formatters.JsonFormatter);
        }
        #endregion

        [HttpPost]
        [ActionName("GetMainDashboard")]
        public HttpResponseMessage GetMainDashboard(DBBasicActivity _dbba)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.GetMainDashboard(_dbba), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("StaffRolesActivity")]
        public HttpResponseMessage StaffRolesActivity(StaffRolesModal _dbba)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.StaffRolesSetup(_dbba), Configuration.Formatters.JsonFormatter);
        }


        [HttpPost]
        [ActionName("UserAccessActivity")]
        public HttpResponseMessage UserAccessActivity(UserAccess _userAccess)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.UserAccessSetup(_userAccess), Configuration.Formatters.JsonFormatter);
        }
        [HttpPost]
        [ActionName("StudyWorkSheetActivity")]
        public HttpResponseMessage StudyWorkSheetActivity(StudyWorkSheetModel _StudyWorkSheetModel)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.StudyWorkSheetSetup(_StudyWorkSheetModel), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("GetAnalyticsDashboard")]
        public HttpResponseMessage GetAnalyticsDashboard(Analytics _analytics)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.AnalyticsDashboardActivity(_analytics), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [ActionName("UserNotificationActitvity")]
        public HttpResponseMessage UserNotificationActitvity(UserNotification _userNotification)
        {
            return Request.CreateResponse(HttpStatusCode.OK, SettingsSetUp.Instance.UserNotificationSetup(_userNotification), Configuration.Formatters.JsonFormatter);
        }
    }
}
