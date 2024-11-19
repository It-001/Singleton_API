using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicalTrialAPI.Models
{
    public class StaffRolesModal : DBBasicActivity
    {
        public string RoleId { get; set; }
        public string Roles { get; set; }
    }

    public class SettingsModel
    {

    }

    public class Users
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string IsAssigned { get; set; }
    }
    public class Location
    {
        public string LocationId { get; set; }
        public string LocationName { get; set; }
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
        public string ActionType { get; set; }
    }

    public class StudyActivity
    {
        public string Action { get; set; }
        public string Study { get; set; }
        public string StudyId { get; set; }
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
        public string StudyNum { get; set; }
        public string PI { get; set; }
        public string EudractNumber { get; set; }
        public string Randnumber { get; set; }
        public string SiteID { get; set; }
        public string Department { get; set; }
        public string StudyColor { get; set; }
        public string Status { get; set; }
        public string UndoArchiveOrInActiveBy { get; set; }

        public List<Users> UserList { get; set; }
        public List<Users> RemovedUserList { get; set; }
        public string LoggedInUserName { get; set; }
        public string StudyGroupId { get; set; }

        public string StudyLocationId { get; set; }
        public string LocationIdRef { get; set; }
        public string BuildingIdRef { get; set; }
        public string FloorIdRef { get; set; }
        public string RoomIdRef { get; set; }
        public string VisitIdRef { get; set; }

        public List<StudyLocationList> StudyLocationList { get; set; }
        public string ProjectId { get; set; }
    }

    public class TemplateList
    {
        public string SMSText { get; set; }
        public string EmailSubject { get; set; }
        [AllowHtml]
        public string EmailText { get; set; }
        public string TemplateType { get; set; }
        public bool IsSMS { get; set; }
        public bool IsEmail { get; set; }
        public bool IsAdhoc { get; set; }
    }

    public class ECommunicationSetup
    {
        public string SetUpId { get; set; }
        public string StudyId { get; set; }
        public string VisitId { get; set; }
        public string CreatedBy { get; set; }
        public string OrganizationId { get; set; }
        public string ActionType { get; set; }
        public List<TemplateList> TemplateList { get; set; }
        public decimal Payment { get; set; }
        public string GroupId { get; set; }
    }

    public class OrganizationSetup
    {
        public string OrgName { get; set; }
        public string OrgCountry { get; set; }
        public string AppointmentStartTime { get; set; }
        public string AppointmentEndTime { get; set; }
        public int AppointmentSlotDuration { get; set; }
        public string LabelDefaultTime { get; set; }
        public int NoofBookingPerSlot { get; set; }
        public bool IsDefaultSlot { get; set; }
        public string CreatedBy { get; set; }
        public string OrganizationId { get; set; }
        public string ActionType { get; set; }
        public string CurrencySymbol { get; set; }
        public string DefaultStudyLocation { get; set; }
        public List<Location> LocationList { get; set; }
    }

    public class Analytics
    {
        public List<string> StudyIds { get; set; }
        public List<string> VisitIds { get; set; }
        public List<string> GroupIds { get; set; }
        public List<string> FeasibilityIds { get; set; }
        public string Period { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
        public string ActionType { get; set; }
        public string AdhocVisit { get; set; }
        public int VisitNo { get; set; }
        public string AppStatus { get; set; }
        public string StudyId { get; set; }
    }


    public class CRO
    {
        public string CROId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string EmailId { get; set; }
        public string ContactNumber { get; set; }
        public string RoleIdRef { get; set; }
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
        public string ActionType { get; set; }
        public string OrgName { get; set; }
        public string OrgCountry { get; set; }
    }




    public class StudyCROMapping
    {
        public string CROId { get; set; }
        public List<CRO> CROList { get; set; }
        public List<Study> StudyList { get; set; }
        public string StudyId { get; set; }
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
        public string ActionType { get; set; }
        public List<string> RemovedStudy { get; set; }
        public List<string> RemovedCRO { get; set; }
        public string LoggedInuserName { get; set; }
        public List<Module> ModuleList { get; set; }
        public List<Module> RemovedModuleList { get; set; }
        public string UnitId { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }

    public class Module
    {
        public string ModuleId { get; set; }
        public string ModuleName { get; set; }
    }

    public class ImportData
    {
        public DataTable InputDataTable { get; set; }
        public string XML { get; set; }
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
        public string ActionType { get; set; }
    }

    public class CROOrganizationXML
    {
        public string OrganizationId { get; set; }
        public string CROId { get; set; }
    }

    public class CROOrganizationActivity
    {
        public List<CROOrganizationXML> CROOrganizationXML { get; set; }
        public string CreatedBy { get; set; }
        public string ActionType { get; set; }
        public string SelectedOrganizationId { get; set; }
        public string CROId { get; set; }
    }
    //use for superadmin
    public class OrgSetup
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string ContactNumber { get; set; }
        public string EmailId { get; set; }
        public string AccessDate { get; set; }
        public string ClinicStartTime { get; set; }
        public string ClinicEndTime { get; set; }
        public string SlotDuration { get; set; }
        public string NoofBookingPerSlot { get; set; }
        public string CurrencySymbol { get; set; }
        public string OrgName { get; set; }
        public string OrgCountry { get; set; }
        public int Hour { get; set; }
        public int Min { get; set; }
        public string ReminderBeforeStartNumber { get; set; }
        public string ReminderBeforeStartSuffix { get; set; }
        public string CreatedBy { get; set; }
        public string OrganizationId { get; set; }
        public string ActionType { get; set; }
        public List<string> OrgModules { get; set; }
        public List<Pages> PageList { get; set; }
        public List<PageActions> PageActionList { get; set; }
    }


    public class DepartmentList
    {
        public string DepartmentId { get; set; }
        public string OrganizationId { get; set; }
    }

    public class Department
    {
        public List<DepartmentList> DepartmentMappingList { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string CreatedBy { get; set; }
        public string ActionType { get; set; }
        public bool IsArchived { get; set; }
        public string OrganizationId { get; set; }
    }

    public class DepartmentRole
    {
        public string DepartmentId { get; set; }
        public string StudyId { get; set; }
        public string VisitId { get; set; }
        public string RoleId { get; set; }
        public int StaffRequired { get; set; }
        public string ActionType { get; set; }
        public string CreatedBy { get; set; }
        public string OrganizationId { get; set; }
        public int TimeReq { get; set; }
        public string GroupId { get; set; }
    }

    public class StaffRoleAnalytics
    {
        public string StudyId { get; set; }
        public List<ECommunicationSetup> VisitIds { get; set; }
        public List<DepartmentList> DepartmentIds { get; set; }
        public string Period { get; set; }
        public string StartPeriod { get; set; }
        public string EndPeriod { get; set; }
        public string OrganizationId { get; set; }
        public string UnitId { get; set; }
        public string ActionType { get; set; }
        public string AdhocVisit { get; set; }
        public List<StaffRoleList> StaffRoleIds { get; set; }
        public string GroupId { get; set; }
        public string VisitType { get; set; }


    }

    public class StaffRoleList
    {
        public string RoleId { get; set; }
    }

    public class VolunteerFilters
    {
        public string StudyId { get; set; }
        public List<string> VisitIds { get; set; }
        public string AppointmentStatus { get; set; }
        public string VolunteerStatus { get; set; }
        public string AppointmentType { get; set; }
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
        public string ActionType { get; set; }
        public string StartPeriod { get; set; }
        public string EndPeriod { get; set; }
    }

    public class FiltersData
    {
        public string EmailId { get; set; }
        public string ContactNo { get; set; }
        public string VolunteerName { get; set; }
        public string VolunteerId { get; set; }
        public string StudyName { get; set; }
        public string VisitName { get; set; }
        public string AppointmentDateTime { get; set; }
        public string VolunteerStatus { get; set; }
        public string AppointmentType { get; set; }
        public bool IsEmail { get; set; }
        public bool IsSMS { get; set; }
    }

    public class EmailSMS
    {
        public List<FiltersData> FiltersList { get; set; }
        [AllowHtml]
        public string EmailBody { get; set; }
        [AllowHtml]
        public string SMSBody { get; set; }
        public bool IsEmail { get; set; }
        public bool IsSMS { get; set; }
        [AllowHtml]
        public string EmailSubject { get; set; }
        public string OrganizationId { get; set; }
        public string SMSTemplateId { get; set; }
        public string EmailTemplateId { get; set; }
        public string CreatedBy { get; set; }
        public bool IsOverRideCommPre { get; set; }
    }

    public class UserDashboard
    {
        public string StaffId { get; set; }
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
    }

    public class ChangeOrganizationReq
    {
        public string StaffId { get; set; }
        public string OrganizaionId { get; set; }
        public string EmailId { get; set; }
        public string OrgType { get; set; }
        public string SessionKey { get; set; }
        public string IP { get; set; }
    }
    public class StudyTaskActivity
    {
        public string StudyTaskId { get; set; }
        public string TaskName { get; set; }
        public string ActionType { get; set; }
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
    }

    public class PaymentEmailTemplate
    {
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
        [AllowHtml]
        public string EmailText { get; set; }
        public string EmailSubject { get; set; }
        public bool IsAllowEmail { get; set; }
        public string TemplateId { get; set; }
        public string ActionType { get; set; }
    }
    public class GetSMSCountReq
    {
        public string Period { get; set; }
        public string StartPeriod { get; set; }
        public string EndPeriod { get; set; }
        public string ActionType { get; set; }
    }
    public class UsersVolunteersMatricies
    {
        public List<string> OrganizationId { get; set; }
        public string Period { get; set; }
        public string StartPeriod { get; set; }
        public string EndPeriod { get; set; }
        public string ActionType { get; set; }
        public string GroupBy { get; set; }
    }

    public class OWAnalytics
    {
        public string AppointmentPeriod { get; set; }
        public string StartPeriod { get; set; }
        public string EndPeriod { get; set; }
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
        public string ActionType { get; set; }
        public List<string> StudyIds { get; set; }
        public List<string> GroupIds { get; set; }
        public string Viewby { get; set; }
        public string StudyId { get; set; }
    }
    public class StudyLocationList
    {
        public string StudyLocationId { get; set; }
        public string LocationIdRef { get; set; }
        public string BuildingIdRef { get; set; }
        public string FloorIdRef { get; set; }
        public string RoomIdRef { get; set; }
    }
    public class StudyLocationVisits
    {
        public string StudyId { get; set; }
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
        public string VisitId { get; set; }
        public string GroupIdRef { get; set; }
        public List<StudyLocationList> StudyLocationList { get; set; }

    }

    public class LocationOccupancyAnalytics
    {
        public string LocationId { get; set; }
        public string BuildingId { get; set; }
        public string FloorId { get; set; }
        public string RoomId { get; set; }
        public List<string> Type { get; set; }
        public string Period { get; set; }
        public string StartPeriod { get; set; }
        public string EndPeriod { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public List<string> Days { get; set; }
        public List<string> StudyIdList { get; set; }
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
        public string IsDNA { get; set; }//did not attend status if it is one means comes data with did not attend else without did not attend
    }

    public class ReminderStudyDetails
    {
        public string StudyIdRef { get; set; }
        public string GroupIdRef { get; set; }
        public string VisitIdRef { get; set; }
    }

    public class AppointmentReminder
    {

        public List<ReminderStudyDetails> StudyDetails { get; set; }
        public string ActionType { get; set; }
        public string ReminderName { get; set; }
        public string ReminderTime { get; set; }
        public int ReminderHour { get; set; }
        public int ReminderMin { get; set; }
        public int ReminderBeforeStartNumber { get; set; }
        public string ReminderBeforeStartSuffix { get; set; }
        public string ScheduleType { get; set; }
        public string ReminderType { get; set; }
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime SystemDateTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "EmailId is a required field")]
        [EmailAddress(ErrorMessage = "Please enter valid Email Id")]
        public string EmailId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is a required field")]
        public string Password { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "AccessCodeIndex0 is a required field")]
        [Range(0, 9, ErrorMessage = "AccessCodeIndex0 must be between 1-9")]
        public int AccessCodeIndex0 { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "AccessCodeIndex1 is a required field")]
        [Range(0, 9, ErrorMessage = "AccessCodeIndex1 must be between 1-9")]
        public int AccessCodeIndex1 { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "AccessCodeIndex2 is a required field")]
        [Range(0, 9, ErrorMessage = "AccessCodeIndex2 must be between 1-9")]
        public int AccessCodeIndex2 { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "AccessCodeValue0 is a required field")]
        [Range(0, 9, ErrorMessage = "AccessCodeValue0 must be between 1-9")]
        public int AccessCodeValue0 { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "AccessCodeValue1 is a required field")]
        [Range(0, 9, ErrorMessage = "AccessCodeValue1 must be between 1-9")]
        public int AccessCodeValue1 { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "AccessCodeValue2 is a required field")]
        [Range(0, 9, ErrorMessage = "AccessCodeValue2 must be between 1-9")]
        public int AccessCodeValue2 { get; set; }

        public string ReminderId { get; set; }
        public string Reason { get; set; }
        public string StudyId { get; set; }
    }


    public class SaveStaffDocuments
    {
        public string DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentURL { get; set; }
        public string ExpiryNumber { get; set; }
        public string CertificateType { get; set; }
        public string DateCertification { get; set; }
        public string ExpirySuffix { get; set; }
        public string FileName { get; set; }
        public string DocumentSize { get; set; }
        public string StaffId { get; set; }
        public string CreatedBy { get; set; }
        public string OrganizationId { get; set; }
        public string ActionType { get; set; }
        public string FileExtension { get; set; }
        public string StudyId { get; set; }
        public bool IsActive { get; set; }
    }

    public class Pages
    {
        public string ModuleId { get; set; }
        public string PageId { get; set; }
        public string PageName { get; set; }
    }

    public class PageActions
    {
        public string ModuleId { get; set; }
        public string PageId { get; set; }
        public string ActionId { get; set; }
        public string ActionName { get; set; }
    }

    public class UserAccess
    {
        public string UserId { get; set; }
        public string PageId { get; set; }
        public string PageActionId { get; set; }
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
        public string ActionType { get; set; }
    }

    public class StudyWorkSheetModel : DBBasicActivity
    {
        public string StudyWorkSheetId { get; set; }
        public string StudyIdRef { get; set; }
        public string GroupIdRef { get; set; }
        public string VisitIdRef { get; set; }
        public string WorkSheetName { get; set; }
        public string WorkSheetURL { get; set; }
        public string Reason { get; set; }
        public bool IsSameForAllGroup { get; set; }
    }

    public class UserNotification : DBBasicActivity
    {
        public string NotificationId { get; set; }
    }
}