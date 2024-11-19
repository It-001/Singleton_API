using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ClinicalTrialAPI.Models
{
    public class eDelegationModel { }

    #region API response
    public class ApiResponse
    {
        public int success { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
    #endregion

    #region user Login
    public class StaffLogin
    {
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

    }
    #endregion

    #region Staff Details
    public class StaffRegistration
    {
        public string StaffId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string EmployeeNum { get; set; }
        public string EmployeeUniqueId { get; set; }
        public string AscNum { get; set; }
        public string AscNumId { get; set; }
        public string RoleIdRef { get; set; }
        public string Gender { get; set; }
        public string StartDate { get; set; }
        public string AccessDate { get; set; }
        public string DOB { get; set; }
        public string EmailId { get; set; }
        public string ContactNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "OrganizationId is a required field")]
        public string OrganizationId { get; set; }
        public string CreatedById { get; set; }
        public string PIAuthRight { get; set; }
        public string HaveEditRights { get; set; }
        public string HaveStudyTaskArchiveRights { get; set; }
        public string HaveStudydelegationLogRights { get; set; }
        public string HaveLineManagerRights { get; set; }
        //public List<string> StudyRoleList { get; set; }
        public List<StudyTaskList> StudyRoleList { get; set; }
        public List<string> Studies { get; set; }
        public List<string> UnitRoleList { get; set; }
        public string Action { get; set; }
        public string LineManagerId { get; set; }
        public string HaveProfileViewRights { get; set; }
        public string HaveDelegationLogViewRights { get; set; }
        public string ReasonArchive { get; set; }
        public string ActionDoneBy { get; set; }
        public string ImageData { get; set; }
        public string extension { get; set; }
        public string SiteId { get; set; }
        public string DepartmentId { get; set; }
        public List<StudyUserUpdate> StudyList { get; set; }
        public List<StudyUserUpdate> RemovedStudyList { get; set; }
        public string LoggedInUserName { get; set; }
        public string OrganizationName { get; set; }

    }

    public class StudyUserUpdate
    {
        public string StudyId { get; set; }
        public string StudyName { get; set; }
        public string IsAssigned { get; set; }
    }

    public class StaffList
    {
        public List<string> StaffIdList { get; set; }
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
        public string ActionType { get; set; }
        public string ReasonArchive { get; set; }
    }

    public class StudyList
    {
        public List<string> StudyIdList { get; set; }
        public List<StudyListMultiActivity> StudyListMulti { get; set; }
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
        public string ActionType { get; set; }
        public string ReasonArchive { get; set; }
    }

    public class StudyListMultiActivity
    {
        public string StudyId { get; set; }
        public string ActionPerformedBy { get; set; }
        public string StaffId { get; set; }
    }

    public class StudyTaskList
    {
        public string Study { get; set; }
        public string StudyTask { get; set; }
    }
    #endregion
    //#region get List of Staff Enrolled
    //public class GetStaffList
    //{
    //    public string StaffId { get; set; }
    //    public string FName { get; set; }
    //    public string LName { get; set; }
    //    public string EmployeeNum { get; set; }
    //    public string AscNum { get; set; }
    //    public string StaffRoleId { get; set; }
    //    public string StaffRole { get; set; }
    //}
    //#endregion

    #region Staff Login Details
    public class StaffLoginDetails
    {
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string StaffId { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string OrganizationId { get; set; }
        public string Role { get; set; }
        public string Specification { get; set; }
        public string AccountStatus { get; set; }
        public string HaveProfileViewRights { get; set; }
        public string HaveDelegationLogViewRights { get; set; }
        public string HaveProfileEditRights { get; set; }
        public string HaveDelegationLogEditRights { get; set; }
        public string HavingLineManagerRights { get; set; }
        public string HaveStudyTaskArchiveRights { get; set; }
        public string IsAuthorized { get; set; }
        public string PrincipalInvestigator { get; set; }
        public string SchApprovalCount { get; set; }
        public string ReviewersCount { get; set; }
        public string Sessionkey { get; set; }
        public string Status { get; set; }
        public string IPAddress { get; set; }
        public bool IsTrainingCompleted { get; set; }
        public bool ISUnitSOPCompleted { get; set; }
        public string APIPWD { get; set; }
        //for new features cro login
        public int IsCRO { get; set; }
        public DataTable OtherOrgLink { get; set; } //Add on for Other Link
        public string OrganizationName { get; set; } //Added by arfin ali 01-3-2022
        public DataTable Modules { get; set; } //Add on for Other Link

    }
    #endregion

    #region Volunteer Details
    public class VolunteerDetails
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "TrialIdRef is a required field")]
        public string TrialIdRef { get; set; }
        public string DisorderIdRef { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "FirstName is a required field")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Gender is a required field")]
        public string Gender { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "DOB is a required field")]
        public string DOB { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "ContactNumber is a required field")]
        public string ContactNumber { get; set; }
        public string EmailId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "OrganizationId is a required field")]
        public string OrganizationId { get; set; }
    }

    #endregion

    #region get update staff role
    public class StaffRoleCurd
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Action is a required field")]
        public string Action { get; set; }
        public string RoleId { get; set; }
        public string Role { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "OrganizationId is a required field")]
        public string OrganizationId { get; set; }
    }
    #endregion

    #region get update staff Specification
    public class StaffSpecification
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Action is a required field")]
        public string Action { get; set; }
        public string Specification { get; set; }
        public string SpecificationId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "OrganizationId is a required field")]
        public string OrganizationId { get; set; }
        public string RoleIdRef { get; set; }
    }
    #endregion


    #region update staff Volunteer dem record
    public class UpdateDemRecord
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Value is a required field")]
        public string Value { get; set; }
        public string OldValue { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Element is a required field")]
        public string Element { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "CommitterRef is a required field")]
        public string CommitterRef { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Staff_VolunteerId is a required field")]
        public string Staff_VolunteerId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "UserType is a required field")]
        public string UserType { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "OrganizationId is a required field")]
        public string OrganizationId { get; set; }

    }

    #endregion

    #region get Themes
    public class ThemesOfTask
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Action is a required field")]
        public string Action { get; set; }
        public string ThemeId { get; set; }
        public string Theme { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Organization is a required field")]
        public string OrganizationId { get; set; }
    }
    #endregion

    #region Task details
    public class TaskAsPerTheme
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Action is a required field")]
        public string Action { get; set; }
        public string ThemeIdRef { get; set; }
        public string TaskId { get; set; }
        public string Task { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Organization is a required field")]
        public string OrganizationId { get; set; }
    }
    #endregion


    #region Assign Task to the Role
    public class AssignTaskRole
    {
        public string Action { get; set; }
        public string ThemeId { get; set; }
        public string TaskId { get; set; }
        public string RoleId { get; set; }
        public string OrganizationId { get; set; }
        public string CreatedById { get; set; }
    }
    #endregion

    #region Study Details
    public class StudyDetails
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Action is a required field")]
        public string Action { get; set; }
        public string Study { get; set; }
        public string StudyNum { get; set; }
        public string StudyId { get; set; }
        public string PI { get; set; }
        public string HavePI { get; set; }
        public string StudyUniqueId { get; set; }
        public List<string> StaffIdList { get; set; }
        public List<SelectedStudytaskList> StudyTaskList { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Organization is a required field")]
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
        public string ReasonArchive { get; set; }
        public string EudractNumber { get; set; }
        public string RandDnumber { get; set; }
        public string SiteID { get; set; }
        public string Department { get; set; }
    }

    public class SelectedStudytaskList
    {
        public string StaffId { get; set; }
        public string StudyTaskId { get; set; }
    }
    #endregion

    #region StudyTaskPerUser
    public class StudyTaskPerUser
    {
        public string StaffId { get; set; }
        public string StudyId { get; set; }
        public string StudyTaskId { get; set; }
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
        public string ActionPerformedBy { get; set; }
        public string Action { get; set; }
        public string ReasonArchive { get; set; }
        public string EventRejectionId { get; set; }
        public string StaffName { get; set; }
    }
    #endregion

    #region Unit Role per Staff
    public class UnitRolePerUser
    {
        public string StaffId { get; set; }
        public string UnitRoleId { get; set; }
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
        public string Action { get; set; }
        public string ReasonArchive { get; set; }
        public string AssignedBy { get; set; }
        public string EventRejectionId { get; set; }
        public string StaffName { get; set; }
    }
    #endregion

    #region Staff Info Ref
    public class StaffInfoRef
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "StaffId is a required field")]
        public string StaffId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "OrganizationId is a required field")]
        public string OrganizationId { get; set; }

    }
    #endregion

    #region Study training request
    public class StaffStudyTrainingReq
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "StaffId is a required field")]
        public string StaffId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "StudyId is a required field")]
        public string StudyId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "OrganizationId is a required field")]
        public string OrganizationId { get; set; }
        //public string AssignStudyToUserId { get; set; }
        public Boolean StudyStatus { get; set; }
        public string TimeGapTillTraning { get; set; }
    }
    #endregion

    #region Change LineM GRef
    public class ChangeLineMGRef
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "StaffId is a required field")]
        public string LineManagerId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "OrganizationId is a required field")]
        public string OrganizationId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Action is a required field")]
        public string Action { get; set; }
        public string NewLineManagerId { get; set; }
        public List<string> LinkedEmployeeId { get; set; }
        public string CreatedBy { get; set; }

    }
    #endregion

    #region Change PI Ref
    public class ChangePIRef
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "PiId is a required field")]
        public string PiId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "OrganizationId is a required field")]
        public string OrganizationId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Action is a required field")]
        public string Action { get; set; }
        public string NewPiId { get; set; }
        public List<string> LinkedStudyId { get; set; }
        public string CreatedBy { get; set; }

    }
    #endregion

    #region upadate staff Role Approval Ref
    public class RoleApprovalRef
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "StaffId is a required field")]
        public string StaffId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "OrganizationId is a required field")]
        public string OrganizationId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Action is a required field")]
        public string Action { get; set; }
        public int ApprovalStatus { get; set; }
        public string AssignUnitRolePerUserId { get; set; }
        public string ReasonArchive { get; set; }
        public string ActionStatus { get; set; }
        public List<string> UnitRoleList { get; set; }
    }
    #endregion

    #region upadate staff Study task Approval Ref
    public class StudyRoleApprovalRef
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "StaffId is a required field")]
        public string StaffId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "OrganizationId is a required field")]
        public string OrganizationId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Action is a required field")]
        public string Action { get; set; }
        public int ApprovalStatus { get; set; }
        public string AssignStudyRoleId { get; set; }
        public string ReasonArchive { get; set; }
        public string ActionStatus { get; set; }
        public string StudyId { get; set; }
        public List<string> StudyIds { get; set; }
    }
    #endregion

    public class Document
    {
        public string ImageData { get; set; }
        public string DocumentName { get; set; }
        public string DocumentUrl { get; set; }
        public string Extension { get; set; }
    }

    public class UserTrainingDocument
    {
        public List<Document> DocumentList { get; set; }
        public bool IsAQSyatemTrainingComplete { get; set; }
        public bool IsUnitSOPComplete { get; set; }
        public string Action { get; set; }
        public string StaffIdref { get; set; }
        public string OrganizationId { get; set; }
    }



    #region New E-delegation Models
    public class UsersDetailsProp
    {
        public string OrganizationId { get; set; }
        public string ActionType { get; set; }
        public string CreatedBy { get; set; }
        public string FilterId { get; set; }
        public string FilterTitle { get; set; }
        public List<FilterData> FilterProp { get; set; }
    }

    public class StudyManagementReq
    {
        public string OrganizationId { get; set; }
        public string CreatedBy { get; set; }
        public string StaffId { get; set; }
        public string ActionType { get; set; }
        public string FilterId { get; set; }
        public string FilterTitle { get; set; }
        public List<FilterData> FilterProp { get; set; }
    }

    public class UsersActivity
    {
        public string OrganizationId { get; set; }
        public string StaffId { get; set; }
        public string Action { get; set; }
        public string AccessCode { get; set; }
        public string Password { get; set; }
        public int HaveProfileViewRights { get; set; }
        public int HaveProfileEditRights { get; set; }
        public int HaveDelegationLogViewRights { get; set; }
        public int HaveDelegationLogEditRights { get; set; }
        public int HavingLineManagerRights { get; set; }
        public int HaveStudyTaskArchiveRights { get; set; }
        public string CreatedBy { get; set; }
    }

    public class CheckValueExistReq
    {
        public string OrganizationId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
    #endregion


    #region Conference Room
    public class ConferenceRoom
    {
        public string ConferenceId { get; set; }
        public string EmailId { get; set; }
        public string OTP { get; set; }
        public string CreatedBy { get; set; }
        public string OrganizationId { get; set; }
    }
    #endregion

    public class PrintPreview
    {
        public string VersionId { get; set; }
        public string StudyId { get; set; }
        public string VersionNumber { get; set; }
        public string StudyName { get; set; }
        public string FilePath { get; set; }
        public string OrganizationId { get; set; }
        public string StaffId { get; set; }
        public string Action { get; set; }
    }

    public class SiteDetails
    {
        public string SiteId { get; set; }
        public int SiteNumber { get; set; }
        public string SiteName { get; set; }
        public string LogPic { get; set; }
        public string CompressedPic { get; set; }
        public string Location { get; set; }
        public string CreatedBy { get; set; }
        public string OrganizationId { get; set; }
        public string ActionName { get; set; }
        public string SiteUniqueId { get; set; }
    }
}