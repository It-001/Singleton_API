using ClinicalTrialAPI.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using DBAccess;
using System.Web.Configuration;
using System.Xml.Linq;
using System.Linq;
using ClinicalTrialAPI.Custom;
using System.Globalization;
using Newtonsoft.Json;

namespace ClinicalTrialAPI.eDelegation
{
    public class eDelegationSetUp
    {
        ApiResponse _ApiResponse;
        private DAL _DAL;
        #region Singleton
        private eDelegationSetUp() { }
        private static readonly Lazy<eDelegationSetUp> lazy = new Lazy<eDelegationSetUp>(() => new eDelegationSetUp());
        public static eDelegationSetUp Instance
        {
            get { return lazy.Value; }
        }
        #endregion

        #region staff login
        public ApiResponse StaffLoginFun(StaffLogin _StaffLogin)
        {
            _ApiResponse = new ApiResponse();
            //DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[10];
                _SqlParameter[0] = new SqlParameter("@EmailId", Convert.ToString(_StaffLogin.EmailId));
                _SqlParameter[1] = new SqlParameter("@Password", Convert.ToString(_StaffLogin.Password));
                _SqlParameter[2] = new SqlParameter("@AccessCodeIndex0", _StaffLogin.AccessCodeIndex0);
                _SqlParameter[3] = new SqlParameter("@AccessCodeIndex1", _StaffLogin.AccessCodeIndex1);
                _SqlParameter[4] = new SqlParameter("@AccessCodeIndex2", _StaffLogin.AccessCodeIndex2);
                _SqlParameter[5] = new SqlParameter("@AccessCodeValue0", _StaffLogin.AccessCodeValue0);
                _SqlParameter[6] = new SqlParameter("@AccessCodeValue1", _StaffLogin.AccessCodeValue1);
                _SqlParameter[7] = new SqlParameter("@AccessCodeValue2", _StaffLogin.AccessCodeValue2);
                _SqlParameter[8] = new SqlParameter("@Action", "");
                _SqlParameter[9] = new SqlParameter("@LoggedInUser", "");
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_UserLogin]", CommandType.StoredProcedure, _SqlParameter);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (Convert.ToString(ds.Tables[0].Rows[0]["Success"]) == "1")
                    {
                        DataTable dtOtherOrgLink = ds.Tables[ds.Tables.Count - 2];
                        DataTable dtmodules = ds.Tables[ds.Tables.Count - 1];
                        if (Convert.ToString(ds.Tables[0].Rows[0]["IsCRO"]) == "0")
                        {
                            StaffLoginDetails _obj = new StaffLoginDetails();
                            _obj.UserName = Convert.ToString(ds.Tables[0].Rows[0]["UserName"]);
                            _obj.Role = Convert.ToString(ds.Tables[0].Rows[0]["Roles"]);
                            _obj.StaffId = Convert.ToString(ds.Tables[0].Rows[0]["StaffId"]);
                            _obj.ProfilePictureUrl = Convert.ToString(ds.Tables[0].Rows[0]["ProfilePictureUrl"]);
                            _obj.OrganizationId = Convert.ToString(ds.Tables[0].Rows[0]["Organizationid"]);
                            _obj.EmailId = Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]);
                            _obj.AccountStatus = Convert.ToString(ds.Tables[0].Rows[0]["AccountStatus"]);
                            _obj.HaveProfileViewRights = Convert.ToString(ds.Tables[0].Rows[0]["HaveProfileViewRights"]);
                            _obj.HaveDelegationLogViewRights = Convert.ToString(ds.Tables[0].Rows[0]["HaveDelegationLogViewRights"]);
                            _obj.HaveProfileEditRights = Convert.ToString(ds.Tables[0].Rows[0]["HaveProfileEditRights"]);
                            _obj.HaveDelegationLogEditRights = Convert.ToString(ds.Tables[0].Rows[0]["HaveDelegationLogEditRights"]);
                            _obj.HavingLineManagerRights = Convert.ToString(ds.Tables[0].Rows[0]["HavingLineManagerRights"]);
                            _obj.HaveStudyTaskArchiveRights = Convert.ToString(ds.Tables[0].Rows[0]["HaveStudyTaskArchiveRights"]);
                            _obj.PrincipalInvestigator = Convert.ToString(ds.Tables[0].Rows[0]["PrincipalInvestigator"]);
                            _obj.IsAuthorized = Convert.ToString(ds.Tables[0].Rows[0]["IsAuthorized"]);
                            _obj.IsTrainingCompleted = Convert.ToBoolean(ds.Tables[0].Rows[0]["IsTrainingCompleted"]);
                            _obj.ISUnitSOPCompleted = Convert.ToBoolean(ds.Tables[0].Rows[0]["ISUnitSOPCompleted"]);
                            _obj.SchApprovalCount = Convert.ToString(ds.Tables[1].Rows[0]["SchApprovalCount"]);
                            _obj.ReviewersCount = Convert.ToString(ds.Tables[2].Rows[0]["ReviewersCount"]);
                            _obj.APIPWD = Convert.ToString(ds.Tables[0].Rows[0]["APIPWD"]);
                            _obj.IsCRO = Convert.ToInt32(ds.Tables[0].Rows[0]["IsCRO"]);
                            _obj.OtherOrgLink = dtOtherOrgLink; //Add on for Other Link
                            _obj.OrganizationName = Convert.ToString(ds.Tables[0].Rows[0]["OrgName"]); //Add on for Other Link
                            _obj.Modules = dtmodules; //Add on for Other Link
                            _ApiResponse.success = 1;
                            _ApiResponse.message = "Login success";
                            _ApiResponse.data = _obj;
                        }
                        else if (Convert.ToString(ds.Tables[0].Rows[0]["IsCRO"]) == "1" || Convert.ToString(ds.Tables[0].Rows[0]["IsCRO"]) == "2")
                        {
                            StaffLoginDetails _obj = new StaffLoginDetails();
                            _obj.IsCRO = Convert.ToInt32(ds.Tables[0].Rows[0]["IsCRO"]);
                            _obj.APIPWD = Convert.ToString(ds.Tables[0].Rows[0]["APIPWD"]);
                            _obj.UserName = Convert.ToString(ds.Tables[0].Rows[0]["UserName"]);
                            _obj.ProfilePictureUrl = Convert.ToString(ds.Tables[0].Rows[0]["ProfilePictureUrl"]);
                            _obj.StaffId = Convert.ToString(ds.Tables[0].Rows[0]["StaffId"]);
                            _obj.OrganizationId = Convert.ToString(ds.Tables[0].Rows[0]["Organizationid"]);
                            _obj.EmailId = Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]);
                            _obj.AccountStatus = Convert.ToString(ds.Tables[0].Rows[0]["AccountStatus"]);
                            _obj.OtherOrgLink = dtOtherOrgLink; //Add on for Other Link
                            _ApiResponse.success = 1;
                            _ApiResponse.message = "Login success";
                            _ApiResponse.data = _obj;
                        }
                    }
                    else
                    {
                        _ApiResponse.success = 0;
                        _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["Message"]);
                    }
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Invalid login details";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_StaffLogin),
                          "eDelegationSetUp", "StaffLoginFun", "[dbo].[usp_UserLogin]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion
        public ApiResponse GetChangedSessionValuesFun(StaffInfoRef _staffId)
        {
            _ApiResponse = new ApiResponse();
            //DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[10];
                _SqlParameter[0] = new SqlParameter("@EmailId", "");
                _SqlParameter[1] = new SqlParameter("@Password", "");
                _SqlParameter[2] = new SqlParameter("@AccessCodeIndex0", "");
                _SqlParameter[3] = new SqlParameter("@AccessCodeIndex1", "");
                _SqlParameter[4] = new SqlParameter("@AccessCodeIndex2", "");
                _SqlParameter[5] = new SqlParameter("@AccessCodeValue0", "");
                _SqlParameter[6] = new SqlParameter("@AccessCodeValue1", "");
                _SqlParameter[7] = new SqlParameter("@AccessCodeValue2", "");
                _SqlParameter[8] = new SqlParameter("@Action", "GetChangesSession");
                _SqlParameter[9] = new SqlParameter("@LoggedInUser", _staffId.StaffId);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_UserLogin]", CommandType.StoredProcedure, _SqlParameter);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (Convert.ToString(ds.Tables[0].Rows[0]["Success"]) == "1")
                    {
                        StaffLoginDetails _obj = new StaffLoginDetails();
                        _obj.UserName = Convert.ToString(ds.Tables[0].Rows[0]["UserName"]);
                        _obj.Role = Convert.ToString(ds.Tables[0].Rows[0]["Roles"]);
                        _obj.StaffId = Convert.ToString(ds.Tables[0].Rows[0]["StaffId"]);
                        _obj.ProfilePictureUrl = Convert.ToString(ds.Tables[0].Rows[0]["ProfilePictureUrl"]);
                        _obj.OrganizationId = Convert.ToString(ds.Tables[0].Rows[0]["Organizationid"]);
                        _obj.EmailId = Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]);
                        _obj.AccountStatus = Convert.ToString(ds.Tables[0].Rows[0]["AccountStatus"]);
                        _obj.HaveProfileViewRights = Convert.ToString(ds.Tables[0].Rows[0]["HaveProfileViewRights"]);
                        _obj.HaveDelegationLogViewRights = Convert.ToString(ds.Tables[0].Rows[0]["HaveDelegationLogViewRights"]);
                        _obj.HaveProfileEditRights = Convert.ToString(ds.Tables[0].Rows[0]["HaveProfileEditRights"]);
                        _obj.HaveDelegationLogEditRights = Convert.ToString(ds.Tables[0].Rows[0]["HaveDelegationLogEditRights"]);
                        _obj.HavingLineManagerRights = Convert.ToString(ds.Tables[0].Rows[0]["HavingLineManagerRights"]);
                        _obj.HaveStudyTaskArchiveRights = Convert.ToString(ds.Tables[0].Rows[0]["HaveStudyTaskArchiveRights"]);
                        _obj.PrincipalInvestigator = Convert.ToString(ds.Tables[0].Rows[0]["PrincipalInvestigator"]);
                        _obj.IsAuthorized = Convert.ToString(ds.Tables[0].Rows[0]["IsAuthorized"]);
                        _obj.IsTrainingCompleted = Convert.ToBoolean(ds.Tables[0].Rows[0]["IsTrainingCompleted"]);
                        _obj.ISUnitSOPCompleted = Convert.ToBoolean(ds.Tables[0].Rows[0]["ISUnitSOPCompleted"]);

                        _ApiResponse.success = 1;
                        _ApiResponse.message = "Login success";
                        _ApiResponse.data = _obj;
                    }
                    else
                    {
                        _ApiResponse.success = 0;
                        _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["Message"]);
                    }
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Invalid login details";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_staffId),
                          "eDelegationSetUp", "GetChangedSessionValuesFun", "[dbo].[usp_UserLogin]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        #region staff registration
        public ApiResponse StaffRegistrationCTFun(StaffRegistration _StaffRegistration)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                string StudyTaskList = string.Empty;
                string StudyList = string.Empty;
                string UnitRoles = string.Empty;

                //if (_StaffRegistration.StudyRoleList != null)
                //    StudyTaskList = new XElement("SelectedStudyTasks", from c in _StaffRegistration.StudyRoleList
                //                                                       select new XElement("StudyTaskList", new XElement("StudyTask", c.ToUpper()))).ToString();

                if (_StaffRegistration.StudyRoleList != null)
                    StudyTaskList = new XElement("SelectedStudyTasks", from c in _StaffRegistration.StudyRoleList
                                                                       select new XElement("StudyTaskList", new XElement("Study", c.Study.ToUpper()), new XElement("StudyTask", c.StudyTask.ToUpper()))).ToString();

                if (_StaffRegistration.Studies != null)
                    StudyList = new XElement("SelectedStudies", from c in _StaffRegistration.Studies
                                                                select new XElement("StudiesList", new XElement("Studies", c.ToUpper()))).ToString();

                if (_StaffRegistration.UnitRoleList != null)
                    UnitRoles = new XElement("SelectedUnitRole", from c in _StaffRegistration.UnitRoleList
                                                                 select new XElement("UnitRoleList", new XElement("UnitRole", c.ToUpper()))).ToString();


                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[38];
                _SqlParameter[0] = new SqlParameter("@StaffId", _StaffRegistration.StaffId);
                _SqlParameter[1] = new SqlParameter("@FirstName", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.FirstName)) ? string.Empty : Convert.ToString(_StaffRegistration.FirstName));
                _SqlParameter[2] = new SqlParameter("@LastName", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.LastName)) ? string.Empty : Convert.ToString(_StaffRegistration.LastName));
                _SqlParameter[3] = new SqlParameter("@ProfilePictureUrl", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.ProfilePictureUrl)) ? string.Empty : Convert.ToString(_StaffRegistration.ProfilePictureUrl));
                _SqlParameter[4] = new SqlParameter("@EmployeeNum", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.EmployeeNum)) ? string.Empty : Convert.ToString(_StaffRegistration.EmployeeNum));
                _SqlParameter[5] = new SqlParameter("@EmployeeUniqueId", _StaffRegistration.EmployeeUniqueId);
                _SqlParameter[6] = new SqlParameter("@AscensionNum", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.EmployeeNum)) ? string.Empty : Convert.ToString(_StaffRegistration.AscNum));
                _SqlParameter[7] = new SqlParameter("@AscensionNumId", _StaffRegistration.AscNumId);
                _SqlParameter[8] = new SqlParameter("@Gender", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.Gender)) ? string.Empty : Convert.ToString(_StaffRegistration.Gender));
                _SqlParameter[9] = new SqlParameter("@StartDate", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.StartDate)) ? string.Empty : Convert.ToString(_StaffRegistration.StartDate));
                _SqlParameter[10] = new SqlParameter("@AccessDate", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.AccessDate)) ? string.Empty : Convert.ToString(_StaffRegistration.AccessDate));
                _SqlParameter[11] = new SqlParameter("@DOB", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.DOB)) ? string.Empty : Convert.ToString(_StaffRegistration.DOB));
                _SqlParameter[12] = new SqlParameter("@EmailId", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.EmailId)) ? string.Empty : Convert.ToString(_StaffRegistration.EmailId));
                _SqlParameter[13] = new SqlParameter("@ContactNumber", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.ContactNumber)) ? string.Empty : Convert.ToString(_StaffRegistration.ContactNumber));
                _SqlParameter[14] = new SqlParameter("@AddressLine1", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.AddressLine1)) ? string.Empty : Convert.ToString(_StaffRegistration.AddressLine1));
                _SqlParameter[15] = new SqlParameter("@AddressLine2", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.AddressLine2)) ? string.Empty : Convert.ToString(_StaffRegistration.AddressLine2));
                _SqlParameter[16] = new SqlParameter("@City", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.City)) ? string.Empty : Convert.ToString(_StaffRegistration.City));
                _SqlParameter[17] = new SqlParameter("@PostCode", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.PostCode)) ? string.Empty : Convert.ToString(_StaffRegistration.PostCode));
                _SqlParameter[18] = new SqlParameter("@County", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.County)) ? string.Empty : Convert.ToString(_StaffRegistration.County));
                _SqlParameter[19] = new SqlParameter("@Country", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.Country)) ? string.Empty : Convert.ToString(_StaffRegistration.Country));
                _SqlParameter[20] = new SqlParameter("@RoleIdRef", _StaffRegistration.RoleIdRef);
                _SqlParameter[21] = new SqlParameter("@OrganizationId", _StaffRegistration.OrganizationId);
                _SqlParameter[22] = new SqlParameter("@CreatedBy", _StaffRegistration.CreatedById);
                _SqlParameter[23] = new SqlParameter("@PIAuthRight", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.PIAuthRight)) ? string.Empty : Convert.ToString(_StaffRegistration.PIAuthRight));
                _SqlParameter[24] = new SqlParameter("@HaveEditRights", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.HaveEditRights)) ? string.Empty : Convert.ToString(_StaffRegistration.HaveEditRights));
                _SqlParameter[25] = new SqlParameter("@HaveStudyTaskArchiveRights", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.HaveStudyTaskArchiveRights)) ? string.Empty : Convert.ToString(_StaffRegistration.HaveStudyTaskArchiveRights));
                _SqlParameter[26] = new SqlParameter("@HaveStudydelegationLogRights", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.HaveStudydelegationLogRights)) ? string.Empty : Convert.ToString(_StaffRegistration.HaveStudydelegationLogRights));
                _SqlParameter[27] = new SqlParameter("@HaveLineManagerRights", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.HaveLineManagerRights)) ? "0" : Convert.ToString(_StaffRegistration.HaveLineManagerRights));
                _SqlParameter[28] = new SqlParameter("@xmlStudyTaskList", StudyTaskList);
                _SqlParameter[29] = new SqlParameter("@xmlStudies", StudyList);
                _SqlParameter[30] = new SqlParameter("@xmlUnitRoleList", UnitRoles);
                _SqlParameter[31] = new SqlParameter("@Action", _StaffRegistration.Action);
                _SqlParameter[32] = new SqlParameter("@LineManagerId", _StaffRegistration.LineManagerId);
                _SqlParameter[33] = new SqlParameter("@HaveProfileViewRights", _StaffRegistration.HaveProfileViewRights);
                _SqlParameter[34] = new SqlParameter("@HaveDelegationLogViewRights", _StaffRegistration.HaveDelegationLogViewRights);
                _SqlParameter[35] = new SqlParameter("@ReasonArchive", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.ReasonArchive)) ? string.Empty : Convert.ToString(_StaffRegistration.ReasonArchive));
                _SqlParameter[36] = new SqlParameter("@ActionDoneBy", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.ActionDoneBy)) ? string.Empty : Convert.ToString(_StaffRegistration.ActionDoneBy));
                _SqlParameter[37] = new SqlParameter("@SiteId", _StaffRegistration.SiteId);

                dt = _DAL.ExecuteCommand("[dbo].[usp_StaffRegistration]", CommandType.StoredProcedure, _SqlParameter);
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToString(dt.Rows[0]["Success"]) == "1")
                    {
                        if (Convert.ToString(dt.Rows[0]["ActionType"]) == "Approve")
                        {
                            string msgPassword = " Dear " + Convert.ToString(dt.Rows[0]["UserName"]) + ",<br /><br /> Welcome to Ascension Q! Your Clinical Trial login  details are as follows: <br /><br /> Email Id : " + Convert.ToString(dt.Rows[0]["EmailId"]) + "<br />Password: " + Convert.ToString(dt.Rows[0]["Dos"]) + "<br /><br />Please note that your Access Code details will be provided in a subsequent email.<br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["EmailId"]), "Login Details: Password", msgPassword);

                            string msgAccessCode = " Dear " + Convert.ToString(dt.Rows[0]["UserName"]) + ",<br /><br />Welcome to Ascension Q! Your Clinical Trial Access code details are as follows: <br /><br /> Access Code :" + Convert.ToString(dt.Rows[0]["ACC"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["EmailId"]), "Login Details: Access Code", msgAccessCode);
                        }
                        else if (Convert.ToString(dt.Rows[0]["ActionType"]) == "Insert")
                        {
                            string msgNew = " Dear " + Convert.ToString(dt.Rows[0]["UserName"]) + ", <br /><br /> Welcome to Ascension Q! <br /> Your access credentials to the system will be send to you by the respective official <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["UserEmailId"]), "Welcome to Ascension Q!", msgNew);

                            string msgNewUser = " Hello, <br /><br /> You are assigned as a line manager for the new user: " + Convert.ToString(dt.Rows[0]["UserName"]) + "<br /> Please acknowledge and approve " + Convert.ToString(dt.Rows[0]["UserName"]) + " for system training and system access <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["EmailId"]), "New User for Approval", msgNewUser);
                        }
                        else if (_StaffRegistration.Action == "Archive")
                        {
                            string msgNew = " Dear " + Convert.ToString(dt.Rows[0]["UserName"]) + ",<br /><br />You are now archive by " + Convert.ToString(dt.Rows[0]["ArchivedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["UserEmail"]), "User Archive", msgNew);

                            msgNew = " Hello, <br /><br />" + Convert.ToString(dt.Rows[0]["UserName"]) + " has now been archive by " + Convert.ToString(dt.Rows[0]["ArchivedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["LMEmail"]), "User Archive", msgNew);
                        }
                        else if (_StaffRegistration.Action == "Unarchive")
                        {
                            string msgNew = " Dear " + Convert.ToString(dt.Rows[0]["UserName"]) + ",<br /><br />You are now unarchived by " + Convert.ToString(dt.Rows[0]["UnarchivedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["UserEmail"]), "User unarchived", msgNew);

                            msgNew = " Hello, <br />" + Convert.ToString(dt.Rows[0]["UserName"]) + ",<br /><br /> has now been unarchived by " + Convert.ToString(dt.Rows[0]["UnarchivedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["LMEmail"]), "User unarchived", msgNew);
                        }

                        else if (_StaffRegistration.Action == "Update")
                        {
                            if (Convert.ToString(dt.Rows[0]["UserRights"]) != "")
                            {
                                string msgNew = " Dear " + Convert.ToString(dt.Rows[0]["UserName"]) + ",<br /><br /> The following authorisation rights has/have been:" + Convert.ToString(dt.Rows[0]["UserRights"]) + "<br /><br />by " + Convert.ToString(dt.Rows[0]["LoginUserName"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["EmailId"]), "User Authorisation", msgNew);
                            }
                        }
                        _ApiResponse.success = 1;
                        _ApiResponse.message = Convert.ToString(dt.Rows[0]["MSG"]);
                        _ApiResponse.data = dt;
                    }
                    else
                    {
                        _ApiResponse.success = 0;
                        _ApiResponse.message = Convert.ToString(dt.Rows[0]["MSG"]);
                        _ApiResponse.data = dt;
                    }
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Invalid request";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_StaffRegistration),
                          "eDelegationSetUp", "StaffRegistrationCTFun", "[dbo].[usp_StaffRegistration]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region staff registration
        public ApiResponse StaffRegistrationFun(StaffRegistration _StaffRegistration)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                string XmlString = null;
                if (_StaffRegistration.StudyList != null)
                {
                    XmlString = new XElement("ArrayofStudy", from c in _StaffRegistration.StudyList
                                                             select new XElement("Studies", new XElement("StudyId", c.StudyId.ToUpper()))).ToString();
                }
                string StudyTaskList = string.Empty;
                string StudyList = string.Empty;
                string UnitRoles = string.Empty;

                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[29];

                _SqlParameter[0] = new SqlParameter("@StaffId", _StaffRegistration.StaffId);
                _SqlParameter[1] = new SqlParameter("@FirstName", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.FirstName)) ? string.Empty : Convert.ToString(_StaffRegistration.FirstName));
                _SqlParameter[2] = new SqlParameter("@LastName", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.LastName)) ? string.Empty : Convert.ToString(_StaffRegistration.LastName));
                _SqlParameter[3] = new SqlParameter("@ProfilePictureUrl", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.ProfilePictureUrl)) ? string.Empty : Convert.ToString(_StaffRegistration.ProfilePictureUrl));
                _SqlParameter[4] = new SqlParameter("@EmployeeNum", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.EmployeeNum)) ? string.Empty : Convert.ToString(_StaffRegistration.EmployeeNum));
                _SqlParameter[5] = new SqlParameter("@EmployeeUniqueId", _StaffRegistration.EmployeeUniqueId);
                _SqlParameter[6] = new SqlParameter("@AscensionNum", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.EmployeeNum)) ? string.Empty : Convert.ToString(_StaffRegistration.AscNum));
                _SqlParameter[7] = new SqlParameter("@AscensionNumId", _StaffRegistration.AscNumId);
                _SqlParameter[8] = new SqlParameter("@Gender", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.Gender)) ? string.Empty : Convert.ToString(_StaffRegistration.Gender));
                _SqlParameter[9] = new SqlParameter("@StartDate", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.StartDate)) ? string.Empty : Convert.ToString(_StaffRegistration.StartDate));
                _SqlParameter[10] = new SqlParameter("@AccessDate", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.AccessDate)) ? string.Empty : Convert.ToString(_StaffRegistration.AccessDate));
                _SqlParameter[11] = new SqlParameter("@DOB", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.DOB)) ? string.Empty : Convert.ToString(_StaffRegistration.DOB));
                _SqlParameter[12] = new SqlParameter("@EmailId", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.EmailId)) ? string.Empty : Convert.ToString(_StaffRegistration.EmailId));
                _SqlParameter[13] = new SqlParameter("@ContactNumber", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.ContactNumber)) ? string.Empty : Convert.ToString(_StaffRegistration.ContactNumber));
                _SqlParameter[14] = new SqlParameter("@AddressLine1", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.AddressLine1)) ? string.Empty : Convert.ToString(_StaffRegistration.AddressLine1));
                _SqlParameter[15] = new SqlParameter("@City", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.City)) ? string.Empty : Convert.ToString(_StaffRegistration.City));
                _SqlParameter[16] = new SqlParameter("@PostCode", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.PostCode)) ? string.Empty : Convert.ToString(_StaffRegistration.PostCode));
                _SqlParameter[17] = new SqlParameter("@County", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.County)) ? string.Empty : Convert.ToString(_StaffRegistration.County));
                _SqlParameter[18] = new SqlParameter("@Country", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.Country)) ? string.Empty : Convert.ToString(_StaffRegistration.Country));
                _SqlParameter[19] = new SqlParameter("@RoleIdRef", _StaffRegistration.RoleIdRef);
                _SqlParameter[20] = new SqlParameter("@OrganizationId", _StaffRegistration.OrganizationId);
                _SqlParameter[21] = new SqlParameter("@CreatedBy", _StaffRegistration.CreatedById);
                _SqlParameter[22] = new SqlParameter("@LineManagerId", (_StaffRegistration.LineManagerId) == "" ? null : _StaffRegistration.LineManagerId);
                _SqlParameter[23] = new SqlParameter("@HaveLineManagerRights", _StaffRegistration.HaveLineManagerRights);
                _SqlParameter[24] = new SqlParameter("@ReasonArchive", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.ReasonArchive)) ? string.Empty : Convert.ToString(_StaffRegistration.ReasonArchive));
                _SqlParameter[25] = new SqlParameter("@SiteId", _StaffRegistration.SiteId);
                _SqlParameter[26] = new SqlParameter("@Action", _StaffRegistration.Action);
                if (_StaffRegistration.DepartmentId == "")
                {
                    _SqlParameter[27] = new SqlParameter("@DepartmentId", DBNull.Value);
                }
                else
                {
                    _SqlParameter[27] = new SqlParameter("@DepartmentId", _StaffRegistration.DepartmentId);
                }
                _SqlParameter[28] = new SqlParameter("@StudyList", XmlString);
                dt = _DAL.ExecuteCommand("[dbo].[usp_StaffReg]", CommandType.StoredProcedure, _SqlParameter);
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToString(dt.Rows[0]["Success"]) == "1")
                    {
                        if (Convert.ToString(dt.Rows[0]["ActionType"]) == "Insert")
                        {
                            if (Convert.ToBoolean(dt.Rows[0]["IsAlreadyHaveAccount"]) == true)
                            {
                                string body3 = "Dear " + _StaffRegistration.FirstName + ", <br /><br /> You have been provided access to " + Convert.ToString(dt.Rows[0]["OrgName"]) + ". Please use the same login details to access the Organisation.<br /><br />Kind regards, <br />" + "AscensionQ Team";
                                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["EmailId"]), "Organisation Access", body3,"Yes");
                            }
                            else
                            {
                                string appurl = WebConfigurationManager.AppSettings["ApplicationURL"];
                                string msgPassword = " Dear " + Convert.ToString(dt.Rows[0]["UserName"]) + ",<br /><br /> Welcome to Ascension Q! Your Clinical Trial login  details are as follows: <br /><br /> URL : " + appurl + " <br /> Email Id : " + Convert.ToString(dt.Rows[0]["EmailId"]) + "<br />Password: " + Convert.ToString(dt.Rows[0]["Dos"]) + "<br /><br />Please note that your Access Code details will be provided in a subsequent email.<br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["EmailId"]), "Login Details: Password", msgPassword, "Yes");

                                string msgAccessCode = " Dear " + Convert.ToString(dt.Rows[0]["UserName"]) + ",<br /><br />Welcome to Ascension Q! Your Clinical Trial Access code details are as follows: <br /><br /> Access Code :" + Convert.ToString(dt.Rows[0]["ACC"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["EmailId"]), "Login Details: Access Code", msgAccessCode, "Yes");
                            }

                        }

                        string studies = "";
                        string message = "";
                        if (_StaffRegistration.StudyList != null && _StaffRegistration.StudyList.Count > 0)
                        {
                            for (int i = 0; i < _StaffRegistration.StudyList.Count; i++)
                            {

                                if (_StaffRegistration.StudyList[i].IsAssigned == null)
                                {
                                    studies = studies + "" + _StaffRegistration.StudyList[i].StudyName + ", ";

                                }
                            }
                            if (studies != "")
                            {
                                studies = studies.Substring(0, studies.Length - 1);
                                message = "Dear " + _StaffRegistration.FirstName + " " + _StaffRegistration.LastName + ",<br /><br />";
                                message += "" + _StaffRegistration.LoggedInUserName + " (" + Convert.ToString(_StaffRegistration.OrganizationName) + ") has assigned " + studies + " to you.<br />Please connect to the site organisation in case of any queries.<br /><br />Thank you.";

                                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["EmailId"]), "Study delegation details", message, "Yes");
                            }
                        }

                        studies = "";
                        message = "";
                        if (_StaffRegistration.RemovedStudyList != null && _StaffRegistration.RemovedStudyList.Count > 0)
                        {
                            for (int i = 0; i < _StaffRegistration.RemovedStudyList.Count; i++)
                            {
                                if (_StaffRegistration.RemovedStudyList[i].IsAssigned == "assigned")
                                {
                                    studies = studies + "" + _StaffRegistration.RemovedStudyList[i].StudyName + ", ";                                    
                                }
                            }
                            if (studies != "")
                            {
                                studies = studies.Substring(0, studies.Length - 1);
                                message = "Dear " + _StaffRegistration.FirstName + " " + _StaffRegistration.LastName + ",<br /><br />";
                                message += "" + _StaffRegistration.LoggedInUserName + " (" + _StaffRegistration.OrganizationName + ") has removed " + studies + " from you.<br />Please connect to the site organisation in case of any queries.<br /><br />Thank you.";
                                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["EmailId"]), "Study delegation details", message, "Yes");
                            }
                        }


                        _ApiResponse.success = 1;
                        _ApiResponse.message = Convert.ToString(dt.Rows[0]["MSG"]);
                        _ApiResponse.data = dt;
                    }
                    else
                    {
                        _ApiResponse.success = 0;
                        _ApiResponse.message = Convert.ToString(dt.Rows[0]["MSG"]);
                        _ApiResponse.data = dt;
                    }
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Invalid request";
                }
            }
            catch (Exception ex)
            {

                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_StaffRegistration),
                          "eDelegationSetUp", "StaffRegistrationFun", "[dbo].[usp_StaffReg]", "Clinical Trail");

            }
            return _ApiResponse;
        }
        #endregion

        #region GetIndividuaRecord
        public ApiResponse GetIndividuaRecord(UnitRolePerUser _unitRolePerUser)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            string msg = string.Empty;
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[9];
                _sqlPara[0] = new SqlParameter("@StaffId", _unitRolePerUser.StaffId);
                _sqlPara[1] = new SqlParameter("@Action", _unitRolePerUser.Action);
                _sqlPara[2] = new SqlParameter("@OrganizationId", _unitRolePerUser.OrganizationId);
                _sqlPara[3] = new SqlParameter("@CreatedBy", _unitRolePerUser.CreatedBy);
                _sqlPara[4] = new SqlParameter("@UnitRoleId", _unitRolePerUser.UnitRoleId);
                _sqlPara[5] = new SqlParameter("@ReasonArchive", string.IsNullOrEmpty(Convert.ToString(_unitRolePerUser.ReasonArchive)) ? string.Empty : Convert.ToString(_unitRolePerUser.ReasonArchive));
                _sqlPara[6] = new SqlParameter("@AssignedBy", _unitRolePerUser.AssignedBy);
                _sqlPara[7] = new SqlParameter("@EventRejectionId", _unitRolePerUser.EventRejectionId);
                _sqlPara[8] = new SqlParameter("@RejectedTaskStaffName", _unitRolePerUser.StaffName);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_GetIndividualDetails]", CommandType.StoredProcedure, _sqlPara);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (Convert.ToString(ds.Tables[0].Rows[0]["Success"]) == "1")
                    {
                        if (Convert.ToString(ds.Tables[0].Rows[0]["ActionType"]) == "Get")
                        {
                            ds.Tables[0].TableName = "StaffRecord";
                            ds.Tables[1].TableName = "AssignedStudyList";
                            ds.Tables[2].TableName = "AssignedStudyTaskListPerStudy";
                            ds.Tables[3].TableName = "AssignedUnitRole";
                            ds.Tables[4].TableName = "RemoveUnitRole";
                            ds.Tables[5].TableName = "UserTrainingDocument";
                            ds.Tables[6].TableName = "UnitRoleApproval";
                            ds.Tables[7].TableName = "StudyTaskApproval";
                            ds.Tables[8].TableName = "UnitRoleList";
                            ds.Tables[9].TableName = "StudyTraining";
                            ds.Tables[10].TableName = "NewUnitRoleForStaff";
                            _ApiResponse.success = 1;
                            _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                            _ApiResponse.data = ds;
                        }
                        else if (Convert.ToString(ds.Tables[0].Rows[0]["ActionType"]) == "GetProfileDetails")
                        {
                            ds.Tables[0].TableName = "StaffRecord";
                            ds.Tables[1].TableName = "StudyList";
                            ds.Tables[2].TableName = "PIStudyList";
                            _ApiResponse.success = 1;
                            _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                            _ApiResponse.data = ds;
                        }
                        else
                        {
                            switch (Convert.ToString(ds.Tables[0].Rows[0]["ActionType"]))
                            {
                                case "AuthorizeAllRemovedUnitRole":
                                    msg = " Dear " + Convert.ToString(ds.Tables[0].Rows[0]["UserName"]) + ",<br /><br /> Your Unit Roles are as follows:<br/>" + Convert.ToString(ds.Tables[0].Rows[0]["UnitRoles"]) + " <br /><br />Removed by " + Convert.ToString(ds.Tables[0].Rows[0]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                    SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]), "In Active Unit Roles", msg);
                                    break;
                                case "AuthorizeRemovedUnitRole":
                                    msg = " Dear " + Convert.ToString(ds.Tables[0].Rows[0]["UserName"]) + ",<br /><br />  Your Unit Role<br/>" + Convert.ToString(ds.Tables[0].Rows[0]["UnitRoles"]) + " <br /><br />Removed by " + Convert.ToString(ds.Tables[0].Rows[0]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                    SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]), "In Active Unit Roles", msg);
                                    break;
                                case "AuthorizeAllNewUnitRole":
                                    msg = " Dear " + Convert.ToString(ds.Tables[0].Rows[0]["UserName"]) + ",<br /><br />  New Unit Role has been assigned to you. They are as follows: <br/>" + Convert.ToString(ds.Tables[0].Rows[0]["UnitRoles"]) + " <br /><br />Authorised by " + Convert.ToString(ds.Tables[0].Rows[0]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                    SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]), "New Unit Role Authorised", msg);
                                    break;
                                case "AuthorizeNewUnitRole":
                                    msg = " Dear " + Convert.ToString(ds.Tables[0].Rows[0]["UserName"]) + ",<br /><br /> A New Unit Role has been assigned to you. It is as follows:<br/>" + Convert.ToString(ds.Tables[0].Rows[0]["UnitRoles"]) + " <br /><br />Authorised by " + Convert.ToString(ds.Tables[0].Rows[0]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                    SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]), "New Unit Role Authorised", msg);
                                    break;
                                case "AuthArchivedUser":
                                    msg = " Dear " + Convert.ToString(ds.Tables[0].Rows[0]["UserName"]) + ",<br /><br />Your access to the system has been blocked by " + Convert.ToString(ds.Tables[0].Rows[0]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                    SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]), "User Inactive", msg);
                                    break;
                            }
                            _ApiResponse.success = 1;
                            _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                            _ApiResponse.data = ds.Tables[0];
                        }


                    }
                    else
                    {
                        _ApiResponse.success = 0;
                        _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                    }
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                }
            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;

                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_unitRolePerUser),
                          "eDelegationSetUp", "GetIndividuaRecord", "[dbo].[usp_GetIndividualDetails]", "Clinical Trail");

                _ApiResponse.message = "No list found";
            }
            return _ApiResponse;
        }
        #endregion

        #region Save Volunteer Details
        public ApiResponse SaveVolunteerDetailsFun(VolunteerDetails _VolunteerDetails)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[15];
                _SqlParameter[0] = new SqlParameter("@TrialIdRef", Convert.ToString(_VolunteerDetails.TrialIdRef));
                _SqlParameter[1] = new SqlParameter("@DisorderIdRef", Convert.ToString(_VolunteerDetails.DisorderIdRef));
                _SqlParameter[2] = new SqlParameter("@FirstName", _VolunteerDetails.FirstName);
                _SqlParameter[3] = new SqlParameter("@LastName", _VolunteerDetails.LastName);
                _SqlParameter[4] = new SqlParameter("@DOB", _VolunteerDetails.DOB);
                _SqlParameter[5] = new SqlParameter("@Gender", _VolunteerDetails.DOB);
                _SqlParameter[6] = new SqlParameter("@ContactNumber", _VolunteerDetails.ContactNumber);
                _SqlParameter[7] = new SqlParameter("@EmailId", _VolunteerDetails.EmailId);
                _SqlParameter[8] = new SqlParameter("@AddressLine1", _VolunteerDetails.AddressLine1);
                _SqlParameter[9] = new SqlParameter("@AddressLine2", _VolunteerDetails.AddressLine2);
                _SqlParameter[10] = new SqlParameter("@City", _VolunteerDetails.City);
                _SqlParameter[11] = new SqlParameter("@PostCode", _VolunteerDetails.PostCode);
                _SqlParameter[12] = new SqlParameter("@County", _VolunteerDetails.County);
                _SqlParameter[13] = new SqlParameter("@Country", _VolunteerDetails.OrganizationId);
                _SqlParameter[14] = new SqlParameter("@OrganizationId", _VolunteerDetails.OrganizationId);
                dt = _DAL.ExecuteCommand("[dbo].[usp_SaveVolunteerDetails]", CommandType.StoredProcedure, _SqlParameter);


                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToString(dt.Rows[0]["Success"]) == "1")
                    {
                        _ApiResponse.success = 1;
                        _ApiResponse.message = Convert.ToString(dt.Rows[0]["MSG"]);
                        _ApiResponse.data = dt;
                    }
                    else
                    {
                        _ApiResponse.success = 0;
                        _ApiResponse.message = Convert.ToString(dt.Rows[0]["MSG"]);
                    }
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Invalid request";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_VolunteerDetails),
                           "eDelegationSetUp", "SaveVolunteerDetailsFun", "[dbo].[usp_SaveVolunteerDetails]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region get/update/insert staff role
        public ApiResponse StaffRoleCurdFunction(StaffRoleCurd _StaffRoleCurd)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[4];
                _SqlParameter[0] = new SqlParameter("@Action", _StaffRoleCurd.Action);
                _SqlParameter[1] = new SqlParameter("@RoleId", _StaffRoleCurd.RoleId);
                _SqlParameter[2] = new SqlParameter("@Role", _StaffRoleCurd.Role);
                _SqlParameter[3] = new SqlParameter("@OrganizationId", _StaffRoleCurd.OrganizationId);
                dt = _DAL.ExecuteCommand("[dbo].[usp_StaffRole]", CommandType.StoredProcedure, _SqlParameter);
                if (dt.Rows.Count > 0)
                {
                    _ApiResponse.success = 1;
                    _ApiResponse.data = dt;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Invalid request";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_StaffRoleCurd),
                            "eDelegationSetUp", "StaffRoleCurdFunction", "[dbo].[usp_StaffRole]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region  update staff/Volunteer dem record
        public ApiResponse UpdateStaffVolunteerFun(UpdateDemRecord _udr)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[7];
                _SqlParameter[0] = new SqlParameter("@OrganizationId", _udr.OrganizationId);
                _SqlParameter[1] = new SqlParameter("@Value", _udr.Value);
                _SqlParameter[2] = new SqlParameter("@OldValue", _udr.OldValue);
                _SqlParameter[3] = new SqlParameter("@Element", _udr.Element);
                _SqlParameter[4] = new SqlParameter("@CommitterRef", _udr.CommitterRef);
                _SqlParameter[5] = new SqlParameter("@Staff_VolunteerId", _udr.Staff_VolunteerId);
                _SqlParameter[6] = new SqlParameter("@UserType", _udr.UserType);
                dt = _DAL.ExecuteCommand("[dbo].[usp_UpdateDemRecord]", CommandType.StoredProcedure, _SqlParameter);
                if (dt.Rows.Count > 0)
                {
                    _ApiResponse.success = 1;
                    _ApiResponse.message = Convert.ToString(dt.Rows[0]["MSG"]);
                    _ApiResponse.data = dt;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Invalid request";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_udr),
                             "eDelegationSetUp", "UpdateStaffVolunteerFun", "[dbo].[usp_UpdateDemRecord]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region get/update/insert Staff Specification
        public ApiResponse StaffSpecificationFun(StaffSpecification _StaffSpecification)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[5];
                _SqlParameter[0] = new SqlParameter("@Action", _StaffSpecification.Action);
                _SqlParameter[1] = new SqlParameter("@Specification", _StaffSpecification.Specification);
                _SqlParameter[2] = new SqlParameter("@SpecificationId", _StaffSpecification.SpecificationId);
                _SqlParameter[3] = new SqlParameter("@OrganizationId", _StaffSpecification.OrganizationId);
                _SqlParameter[4] = new SqlParameter("@RoleIdRef", _StaffSpecification.RoleIdRef);
                dt = _DAL.ExecuteCommand("[dbo].[usp_StaffSpecification]", CommandType.StoredProcedure, _SqlParameter);
                if (dt.Rows.Count > 0)
                {
                    _ApiResponse.success = 1;
                    _ApiResponse.data = dt;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Invalid request";
                }
            }
            catch (Exception ex)
            {

                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_StaffSpecification),
                             "eDelegationSetUp", "StaffSpecificationFun", "[dbo].[usp_StaffSpecification]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        //#region Get Theme
        //public ApiResponse GetThemeOfTask(ThemesOfTask _themesOfTask)
        //{
        //    _ApiResponse = new ApiResponse();
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
        //        SqlParameter[] _SqlParameter = new SqlParameter[2];
        //        _SqlParameter[0] = new SqlParameter("@Action", _themesOfTask.Action);
        //        _SqlParameter[1] = new SqlParameter("@OrganizationId", _themesOfTask.OrganizationId);
        //        dt = _DAL.ExecuteCommand("[dbo].[usp_GetTheme]", CommandType.StoredProcedure, _SqlParameter);
        //        if (dt.Rows.Count > 0)
        //        {
        //            _ApiResponse.success = 1;
        //            _ApiResponse.data = dt;
        //        }
        //        else
        //        {
        //            _ApiResponse.success = 0;
        //            _ApiResponse.message = "Invalid request";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "GetThemeOfStudy", "[dbo].[usp_GetTheme]", "Clinical Trail");
        //    }
        //    return _ApiResponse;
        //}
        //#endregion
        #region Get Task List
        //public ApiResponse GetTaskList(string OrgId)
        //{
        //    _ApiResponse = new ApiResponse();
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
        //        SqlParameter[] _SqlParameter = new SqlParameter[1];
        //        _SqlParameter[0] = new SqlParameter("@OrganizationId", OrgId);
        //        dt = _DAL.ExecuteCommand("[dbo].[usp_GetStudyList]", CommandType.StoredProcedure, _SqlParameter);
        //        if (dt.Rows.Count > 0)
        //        {
        //            _ApiResponse.success = 1;
        //            _ApiResponse.data = dt;
        //        }
        //        else
        //        {
        //            _ApiResponse.success = 0;
        //            _ApiResponse.message = "Invalid request";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "GEtStudyList", "[dbo].[usp_GetStudyList]", "Clinical Trail");
        //    }
        //    return _ApiResponse;
        //}
        #endregion

        #region Get Task List based on theme
        public ApiResponse GetTaskListOfTheme(TaskAsPerTheme _taskAsPerTheme)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[5];
                _SqlParameter[0] = new SqlParameter("@Action", _taskAsPerTheme.Action);
                _SqlParameter[1] = new SqlParameter("@ThemeIdRef", _taskAsPerTheme.ThemeIdRef);
                _SqlParameter[2] = new SqlParameter("@Study", _taskAsPerTheme.Task);
                _SqlParameter[3] = new SqlParameter("@StudyId", _taskAsPerTheme.TaskId);
                _SqlParameter[4] = new SqlParameter("@OrganizationId", _taskAsPerTheme.OrganizationId);
                dt = _DAL.ExecuteCommand("[dbo].[usp_StudyASPerTheme]", CommandType.StoredProcedure, _SqlParameter);
                if (dt.Rows.Count > 0)
                {
                    _ApiResponse.success = 1;
                    _ApiResponse.data = dt;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Invalid request";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_taskAsPerTheme),
                             "eDelegationSetUp", "GetTaskListOfTheme", "[dbo].[usp_StudyASPerTheme]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region AssignTaskRole
        //public ApiResponse AssignTaskRole(AssignTaskRole _assignTaskRole)
        //{
        //    _ApiResponse = new ApiResponse();
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
        //        SqlParameter[] _SqlParameter = new SqlParameter[5];
        //        _SqlParameter[0] = new SqlParameter("@Action", _assignTaskRole.Action);
        //        _SqlParameter[1] = new SqlParameter("@ThemeId", _assignTaskRole.ThemeId);
        //        _SqlParameter[2] = new SqlParameter("@StudyId", _assignTaskRole.TaskId);
        //        _SqlParameter[3] = new SqlParameter("@OrganizationId", _assignTaskRole.OrganizationId);
        //        _SqlParameter[4] = new SqlParameter("@RoleIdRef", _assignTaskRole.RoleId);
        //        dt = _DAL.ExecuteCommand("[dbo].[usp_AssignTaskAsPerRole]", CommandType.StoredProcedure, _SqlParameter);
        //        if (dt.Rows.Count > 0)
        //        {
        //            _ApiResponse.success = 1;
        //            _ApiResponse.data = dt;
        //        }
        //        else
        //        {
        //            _ApiResponse.success = 0;
        //            _ApiResponse.message = "Invalid request";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "AssignTaskRole", "[dbo].[usp_AssignTaskAsPerRole]", "Clinical Trail");
        //    }
        //    return _ApiResponse;
        //}
        #endregion

        #region StudyDetailsCrud
        public ApiResponse StudyDetailsCrud(StudyDetails _studyDetails)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                string StaffList = string.Empty;
                string StudyTaskListId = string.Empty;
                if (_studyDetails.StaffIdList != null)
                    StaffList = new XElement("StaffList", from c in _studyDetails.StaffIdList select new XElement("Staff", new XElement("StaffId", c.ToUpper()))).ToString();

                if (_studyDetails.StudyTaskList != null)
                    StudyTaskListId = new XElement("StudyTaskList", from c in _studyDetails.StudyTaskList select new XElement("StudyTasks", new XElement("StaffId", c.StaffId.ToUpper()), new XElement("StudyTaskId", c.StudyTaskId.ToUpper()))).ToString();


                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[16];
                _SqlParameter[0] = new SqlParameter("@Action", _studyDetails.Action);
                _SqlParameter[1] = new SqlParameter("@Study", string.IsNullOrEmpty(Convert.ToString(_studyDetails.Study)) ? string.Empty : Convert.ToString(_studyDetails.Study));
                _SqlParameter[2] = new SqlParameter("@StudyId", _studyDetails.StudyId);
                _SqlParameter[3] = new SqlParameter("@StudyNum", _studyDetails.StudyNum);
                _SqlParameter[4] = new SqlParameter("@PI", _studyDetails.PI);
                _SqlParameter[5] = new SqlParameter("@HavePI", _studyDetails.HavePI);
                _SqlParameter[6] = new SqlParameter("@Staffxml", StaffList);
                _SqlParameter[7] = new SqlParameter("@xmlStudyTaskAssigned", StudyTaskListId);
                _SqlParameter[8] = new SqlParameter("@OrganizationId", _studyDetails.OrganizationId);
                _SqlParameter[9] = new SqlParameter("@CreatedBy", _studyDetails.CreatedBy);
                _SqlParameter[10] = new SqlParameter("@StudyUniqueId", _studyDetails.StudyUniqueId);
                _SqlParameter[11] = new SqlParameter("@ReasonArchive", string.IsNullOrEmpty(Convert.ToString(_studyDetails.ReasonArchive)) ? string.Empty : Convert.ToString(_studyDetails.ReasonArchive));
                _SqlParameter[12] = new SqlParameter("@EudractNumber", _studyDetails.EudractNumber);
                _SqlParameter[13] = new SqlParameter("@Randnumber", _studyDetails.RandDnumber);
                _SqlParameter[14] = new SqlParameter("@SiteID", _studyDetails.SiteID);
                _SqlParameter[15] = new SqlParameter("@Department", _studyDetails.Department);
                if (_studyDetails.Action == "Insert")
                {
                    ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_StudyDetails]", CommandType.StoredProcedure, _SqlParameter);
                    dt = ds.Tables[0];
                    if (Convert.ToString(dt.Rows[0]["Success"]) == "1")
                    {
                        for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                        {
                            string task = "";
                            string StaffId = Convert.ToString(ds.Tables[1].Rows[i]["StaffId"]);
                            DataRow[] drs = ds.Tables[2].Select("StaffId = '" + StaffId.ToUpper() + "'");
                            foreach (var itm in drs)
                            {
                                task = task + "<li>" + Convert.ToString(itm["StudyTask"]) + "</li>";
                            }
                            task = "<ol>" + task + "</ol>";
                            string msgNewUser = "Dear " + ds.Tables[1].Rows[i]["UserName"] + ", <br /><br /> You have been delegated the following tasks in the  " + Convert.ToString(_studyDetails.Study) + ": <br /> " + task + " <br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[1].Rows[i]["EmailId"]), "New Study task assigned", msgNewUser);
                        }
                    }
                }
                else if (_studyDetails.Action == "Update")
                {
                    ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_StudyDetails]", CommandType.StoredProcedure, _SqlParameter);
                    dt = ds.Tables[0];
                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {
                        string taskAssigned = "";
                        string taskRejected = "";
                        string StaffMailId = Convert.ToString(ds.Tables[1].Rows[i]["UserMailId"]);
                        string msgNewUser = "Dear " + ds.Tables[1].Rows[i]["UserName"] + ",<br /><br />";
                        DataRow[] drs1 = ds.Tables[2].Select("UserMailId = '" + StaffMailId.ToUpper() + "' AND Activitydone='Assigned'");
                        foreach (var itm in drs1)
                        {
                            taskAssigned = taskAssigned + "<li>" + Convert.ToString(itm["StudyTask"]) + "</li>";
                        }
                        taskAssigned = "<ol>" + taskAssigned + "</ol>";
                        if (taskAssigned != "" && taskAssigned != "<ol></ol>")
                        {
                            msgNewUser += "You have been delegated the following tasks by " + Convert.ToString(ds.Tables[1].Rows[i]["PerfomedBy"]) + " in the " + Convert.ToString(_studyDetails.Study) + ": <br /> " + taskAssigned + "<br />";
                        }


                        DataRow[] drs2 = ds.Tables[2].Select("UserMailId = '" + StaffMailId.ToUpper() + "' AND Activitydone='Rejected'");
                        foreach (var itm in drs2)
                        {
                            taskRejected = taskRejected + "<li>" + Convert.ToString(itm["StudyTask"]) + "</li>";
                        }
                        taskRejected = "<ol>" + taskRejected + "</ol>";
                        if (taskRejected != "" && taskRejected != "<ol></ol>")
                        {
                            msgNewUser += "The following delegated tasks has been rejected by " + Convert.ToString(ds.Tables[1].Rows[i]["PerfomedBy"]) + " in the " + Convert.ToString(_studyDetails.Study) + ": <br /> " + taskRejected + "<br />";
                        }
                        msgNewUser += "Thank you,<br /> AscensionQ Team" + "<br />";

                        SendMail_SMS.Instance.sendExceptionMail(StaffMailId, "Study task(s) delegation", msgNewUser);
                    }
                    if (ds.Tables.Count > 3)
                    {
                        //for (int i = 0; i < ds.Tables[3].Rows.Count; i++)
                        //{
                        string msgOldUser = "You have been removed as a Principle investigator by " + Convert.ToString(ds.Tables[3].Rows[0]["RemovedOrAssignedBy"]) + "  for Study " + ds.Tables[3].Rows[0]["StudyName"] + ".<br /><br /> Thank you,<br /> AscensionQ Team" + " <br /> ";
                        SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[3].Rows[0]["OldPIEmail"]), "Removed PI", msgOldUser);

                        string msgNewUser = "You have been assigned as a Principle investigator by " + Convert.ToString(ds.Tables[3].Rows[0]["RemovedOrAssignedBy"]) + "  for Study " + ds.Tables[3].Rows[0]["StudyName"] + ".<br /><br /> Thank you,<br /> AscensionQ Team" + " <br /> ";
                        SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[3].Rows[0]["NewPIEmail"]), "Assigned PI", msgNewUser);
                        //}
                    }

                }
                else
                {
                    dt = _DAL.ExecuteCommand("[dbo].[usp_StudyDetails]", CommandType.StoredProcedure, _SqlParameter);

                }
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToString(dt.Rows[0]["Success"]) == "1")
                    {
                        _ApiResponse.success = 1;
                        _ApiResponse.message = Convert.ToString(dt.Rows[0]["MSG"]);
                        _ApiResponse.data = dt;

                        if (_studyDetails.Action == "Archive")
                        {
                            string msgNewUser = " Hello, <br /><br /> Request to archive study : " + Convert.ToString(dt.Rows[0]["StudyName"]) + " <br /> has been sent by : " + Convert.ToString(dt.Rows[0]["ArchivedBy"]) + " <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["PIEmail"]), "Study Archive", msgNewUser);
                        }
                        else if (_studyDetails.Action == "InActiveStudy")
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string msgNewUser = "" + Convert.ToString(dt.Rows[i]["StudyName"]) + " has been archived By " + Convert.ToString(dt.Rows[i]["ArchivedBy"]) + " <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[i]["StaffEmail"]), "Study Archived", msgNewUser);
                            }
                        }
                        else if (_studyDetails.Action == "Unarchive")
                        {
                            string msgNewUser = "Hello, <br /><br /> Study: " + Convert.ToString(dt.Rows[0]["StudyName"]) + " <br /> has been unarchived by: " + Convert.ToString(dt.Rows[0]["UnrchivedBy"]) + " <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["PIEmail"]), "Study unarchived", msgNewUser);

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                if (Convert.ToString(dt.Rows[i]["StaffEmail"]) != "")
                                {
                                    msgNewUser = "" + Convert.ToString(dt.Rows[i]["StudyName"]) + " has been archived By " + Convert.ToString(dt.Rows[i]["UnrchivedBy"]) + " <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                    SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[i]["StaffEmail"]), "Study Archived", msgNewUser);
                                }
                            }
                        }
                    }
                    else
                    {
                        _ApiResponse.success = 0;
                        _ApiResponse.message = Convert.ToString(dt.Rows[0]["MSG"]);
                    }
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Invalid request";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_studyDetails),
                             "eDelegationSetUp", "StudyDetailsCrud", "[dbo].[usp_StudyDetails]", "Clinical Trail");

            }
            return _ApiResponse;
        }
        #endregion

        #region GetIndividualStudySetails
        public ApiResponse GetIndividualStudySetails(StudyTaskPerUser _studyTaskPerUser)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[5];
                _sqlPara[0] = new SqlParameter("@StudyId", _studyTaskPerUser.StudyId);
                _sqlPara[1] = new SqlParameter("@Action", _studyTaskPerUser.Action);
                _sqlPara[2] = new SqlParameter("@OrganizationId", _studyTaskPerUser.OrganizationId);
                _sqlPara[3] = new SqlParameter("@CreatedBy", _studyTaskPerUser.CreatedBy);
                _sqlPara[4] = new SqlParameter("@StaffId", _studyTaskPerUser.StaffId);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_GetIndividualStudyDetails]", CommandType.StoredProcedure, _sqlPara);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (Convert.ToString(ds.Tables[0].Rows[0]["Success"]) == "1")
                    {
                        ds.Tables[0].TableName = "StudyRecord";
                        if (ds.Tables.Count > 2)
                        {
                            ds.Tables[1].TableName = "AssignedStudyTasktoStaff";
                            ds.Tables[2].TableName = "PendingStudyTasktoStaff";
                        }
                        else if (ds.Tables.Count > 1)
                        {
                            ds.Tables[1].TableName = "AssignedStudyTasktoStaff";
                        }

                        _ApiResponse.success = 1;
                        _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                        _ApiResponse.data = ds;
                    }
                    else
                    {
                        _ApiResponse.success = 0;
                        _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                    }
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_studyTaskPerUser),
                             "eDelegationSetUp", "GetIndividualStudySetails", "[dbo].[usp_GetIndividualStudyDetails]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region StudyTaskCrud
        public ApiResponse StudyTaskDetailsCrud(StudyTaskCrud _studytaskcrud)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[6];
                _SqlParameter[0] = new SqlParameter("@Action", _studytaskcrud.Action);
                _SqlParameter[1] = new SqlParameter("@StudyTaskId", _studytaskcrud.StudyTaskId);
                _SqlParameter[2] = new SqlParameter("@StudyTask", _studytaskcrud.StudyTaskName);
                _SqlParameter[3] = new SqlParameter("@StudyTaskCode", _studytaskcrud.StudyTaskCode);
                _SqlParameter[4] = new SqlParameter("@OrganizationId", _studytaskcrud.OrganizationId);
                _SqlParameter[5] = new SqlParameter("@CreatedBy", _studytaskcrud.CreatedBy);
                dt = _DAL.ExecuteCommand("[dbo].[usp_StudyTaskCrud]", CommandType.StoredProcedure, _SqlParameter);
                if (dt.Rows.Count > 0)
                {
                    _ApiResponse.success = 1;
                    _ApiResponse.data = dt;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Invalid request";
                }
            }
            catch (Exception ex)
            {

                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_studytaskcrud),
                              "eDelegationSetUp", "StudyTaskDetailsCrud", "[dbo].[usp_StudyTaskCrud]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region StudyTaskPerUser
        public ApiResponse StudyTaskPerUserAction(StudyTaskPerUser _studytaskperUser)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            string msg = string.Empty;
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlpara = new SqlParameter[10];
                _sqlpara[0] = new SqlParameter("@StaffId", _studytaskperUser.StaffId);
                _sqlpara[1] = new SqlParameter("@StudyId", _studytaskperUser.StudyId);
                _sqlpara[2] = new SqlParameter("@Action", _studytaskperUser.Action);
                _sqlpara[3] = new SqlParameter("@OrganizationId", _studytaskperUser.OrganizationId);
                _sqlpara[4] = new SqlParameter("@CreatedBy", _studytaskperUser.CreatedBy);
                _sqlpara[5] = new SqlParameter("@ActionPerformedBy", _studytaskperUser.ActionPerformedBy);
                _sqlpara[6] = new SqlParameter("@StudyTaskId", _studytaskperUser.StudyTaskId);
                _sqlpara[7] = new SqlParameter("@ReasonArchive", string.IsNullOrEmpty(Convert.ToString(_studytaskperUser.ReasonArchive)) ? string.Empty : Convert.ToString(_studytaskperUser.ReasonArchive));
                _sqlpara[8] = new SqlParameter("@EventRejectionId", _studytaskperUser.EventRejectionId);
                _sqlpara[9] = new SqlParameter("@RejectedTaskStaffName", _studytaskperUser.StaffName);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_StudyTaskPerUser]", CommandType.StoredProcedure, _sqlpara);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    switch (Convert.ToString(ds.Tables[0].Rows[0]["ActionType"]))
                    {
                        case "ApproveStaffForStudy":
                            msg = " Dear " + Convert.ToString(ds.Tables[0].Rows[0]["UserName"]) + ",<br /><br />  Your Study Tasks are as follows:<br/>" + Convert.ToString(ds.Tables[0].Rows[0]["StudyRoles"]) + " <br /><br />For Study: " + Convert.ToString(ds.Tables[0].Rows[0]["StudyName"]) + "<br/>Authorised by: " + Convert.ToString(ds.Tables[0].Rows[0]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]), "New Study tasks: Authorised", msg);
                            break;
                        case "AuthorizeNewSingleStudyTask":
                            msg = " Dear " + Convert.ToString(ds.Tables[0].Rows[0]["UserName"]) + ",<br /><br /> Your Study task is as follows:<br/>" + Convert.ToString(ds.Tables[0].Rows[0]["StudyRoles"]) + " <br /><br />For Study: " + Convert.ToString(ds.Tables[0].Rows[0]["StudyName"]) + "<br/>Authorised by: " + Convert.ToString(ds.Tables[0].Rows[0]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]), "New Study task: Authorised", msg);
                            break;
                        case "ApproveRejectedStudyOrStudyTask":
                            msg = " Dear " + Convert.ToString(ds.Tables[0].Rows[0]["UserName"]) + ",<br /><br /> Your Study Tasks are as follows:<br/>" + Convert.ToString(ds.Tables[0].Rows[0]["StudyRoles"]) + " <br /><br />For Study: " + Convert.ToString(ds.Tables[0].Rows[0]["StudyName"]) + "<br/>Rejected by: " + Convert.ToString(ds.Tables[0].Rows[0]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]), "Rejected Study Tasks", msg);
                            break;
                        case "ApproveRejectedSingleStudyTask":
                            msg = " Dear " + Convert.ToString(ds.Tables[0].Rows[0]["UserName"]) + "<br /><br /> Your Study task is as follows:<br/>" + Convert.ToString(ds.Tables[0].Rows[0]["StudyRoles"]) + " <br /><br />For Study: " + Convert.ToString(ds.Tables[0].Rows[0]["StudyName"]) + "<br/>Rejected by: " + Convert.ToString(ds.Tables[0].Rows[0]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]), "Rejected Study Task", msg);
                            break;
                    }

                    if (Convert.ToString(ds.Tables[0].Rows[0]["ActionType"]) == "GetPendingStudyRole")
                    {
                        ds.Tables[1].TableName = "PendingStudyTask";
                        ds.Tables[2].TableName = "AuthorizedStudyTask";
                        _ApiResponse.success = 1;
                        _ApiResponse.data = ds;
                        _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                    }
                    else
                    {
                        _ApiResponse.success = 1;
                        _ApiResponse.data = ds.Tables[0];
                        _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                    }

                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "No List Found";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_studytaskperUser),
                              "eDelegationSetUp", "StudyTaskPerUserAction", "[dbo].[usp_StudyTaskPerUser]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region Unit Role Details Crud
        public ApiResponse UnitRoleDetailsCrud(UnitRoleCrud _unitRolecrud)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[6];
                _SqlParameter[0] = new SqlParameter("@Action", _unitRolecrud.Action);
                _SqlParameter[1] = new SqlParameter("@UnitRoleId", _unitRolecrud.UnitRoleId);
                _SqlParameter[2] = new SqlParameter("@UnitRoleName", _unitRolecrud.UnitRoleName);
                _SqlParameter[3] = new SqlParameter("@UnitRoleCode", _unitRolecrud.UnitRoleCode);
                _SqlParameter[4] = new SqlParameter("@OrganizationId", _unitRolecrud.OrganizationId);
                _SqlParameter[5] = new SqlParameter("@CreatedBy", _unitRolecrud.CreatedBy);
                dt = _DAL.ExecuteCommand("[dbo].[usp_UnitRoleCrud]", CommandType.StoredProcedure, _SqlParameter);
                if (dt.Rows.Count > 0)
                {
                    _ApiResponse.success = 1;
                    _ApiResponse.data = dt;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Invalid request";
                }
            }
            catch (Exception ex)
            {

                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_unitRolecrud),
                              "eDelegationSetUp", "UnitRoleDetailsCrud", "[dbo].[usp_UnitRoleCrud]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region GetSingleuniqueCode
        public ApiResponse GetUniqueCode(UniquecodeCrud _uniqueSingleCode)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[8];
                _SqlParameter[0] = new SqlParameter("@Action", _uniqueSingleCode.Action);
                _SqlParameter[1] = new SqlParameter("@Perpose", _uniqueSingleCode.Perpose);
                _SqlParameter[2] = new SqlParameter("@Prefix", _uniqueSingleCode.Prefix);
                _SqlParameter[3] = new SqlParameter("@Suffix", _uniqueSingleCode.Suffix);
                _SqlParameter[4] = new SqlParameter("@NumberFormate", _uniqueSingleCode.NumberFormate);
                _SqlParameter[5] = new SqlParameter("@UniqueCodeId", _uniqueSingleCode.UniqueCodeId);
                _SqlParameter[6] = new SqlParameter("@OrganizationId", _uniqueSingleCode.OrganizationId);
                _SqlParameter[7] = new SqlParameter("@CreatedBy", _uniqueSingleCode.CreatedBy);
                dt = _DAL.ExecuteCommand("[dbo].[usp_UniqueCode]", CommandType.StoredProcedure, _SqlParameter);
                if (dt.Rows.Count > 0)
                {
                    _ApiResponse.success = 1;
                    _ApiResponse.data = dt;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Invalid request";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_uniqueSingleCode),
                               "eDelegationSetUp", "GetUniqueCode", "[dbo].[usp_UniqueCode]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region GetUpdatedUserList
        public ApiResponse GetUpdatedUserList(StaffRegistration _stafReg)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[2];
                _sqlPara[0] = new SqlParameter("@Action", _stafReg.Action);
                _sqlPara[1] = new SqlParameter("@OrganizationId", _stafReg.OrganizationId);
                dt = _DAL.ExecuteCommand("[dbo].[usp_GetModifiedUserList]", CommandType.StoredProcedure, _sqlPara);
                if (dt.Rows.Count > 0)
                {
                    _ApiResponse.success = 1;
                    _ApiResponse.data = dt;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Invalid request";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_stafReg),
                                "eDelegationSetUp", "GetUpdatedUserList", "[dbo].[usp_GetModifiedUserList]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region GetRequestedUsersDetails
        //public ApiResponse GetRequestedUsersDetails(StaffRegistration _stafReg)
        //{
        //    _ApiResponse = new ApiResponse();
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
        //        SqlParameter[] _sqlPara = new SqlParameter[4];
        //        _sqlPara[0] = new SqlParameter("@Action", _stafReg.Action);
        //        _sqlPara[1] = new SqlParameter("@StaffId", _stafReg.StaffId);
        //        _sqlPara[2] = new SqlParameter("@CreatedBy", _stafReg.CreatedById);
        //        _sqlPara[3] = new SqlParameter("@OrganizationId", _stafReg.OrganizationId);
        //        dt = _DAL.ExecuteCommand("[dbo].[usp_RequiredUserDetails]", CommandType.StoredProcedure, _sqlPara);
        //        if (dt.Rows.Count > 0)
        //        {
        //            _ApiResponse.success = 1;
        //            _ApiResponse.data = dt;
        //        }
        //        else
        //        {
        //            _ApiResponse.success = 0;
        //            _ApiResponse.message = "Invalid request";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "GetRequestedUsersDetails", "[dbo].[usp_RequiredUserDetails]", "Clinical Trail");
        //    }
        //    return _ApiResponse;
        //}
        #endregion

        #region Get Requested User Details
        public ApiResponse GetRequestedUserDetails(StaffRegistration _stafReg)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[4];
                _sqlPara[0] = new SqlParameter("@Action", _stafReg.Action);
                _sqlPara[1] = new SqlParameter("@StaffId", _stafReg.StaffId);
                _sqlPara[2] = new SqlParameter("@CreatedBy", _stafReg.CreatedById);
                _sqlPara[3] = new SqlParameter("@OrganizationId", _stafReg.OrganizationId);
                dt = _DAL.ExecuteCommand("[dbo].[usp_RequiredStudyDetails]", CommandType.StoredProcedure, _sqlPara);
                if (dt.Rows.Count > 0)
                {
                    _ApiResponse.success = 1;
                    _ApiResponse.data = dt;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Invalid request";
                }
            }
            catch (Exception ex)
            {

                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_stafReg),
                                 "eDelegationSetUp", "GetRequestedUserDetails", "[dbo].[usp_RequiredStudyDetails]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region Update Study Task List
        public ApiResponse UpdateStudyTaskList(StaffRegistration _staffDetails)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                string StudyTaskList = string.Empty;
                string StudyList = string.Empty;
                string UnitRoles = string.Empty;


                if (_staffDetails.StudyRoleList != null)
                    StudyTaskList = new XElement("SelectedStudyTasks", from c in _staffDetails.StudyRoleList
                                                                       select new XElement("StudyTaskList", new XElement("Study", c.Study), new XElement("StudyTask", c.StudyTask))).ToString();

                if (_staffDetails.Studies != null)
                    StudyList = new XElement("SelectedStudies", from c in _staffDetails.Studies
                                                                select new XElement("StudiesList", new XElement("Studies", c.ToUpper()))).ToString();

                if (_staffDetails.UnitRoleList != null)
                    UnitRoles = new XElement("SelectedUnitRole", from c in _staffDetails.UnitRoleList
                                                                 select new XElement("UnitRoleList", new XElement("UnitRole", c.ToUpper()))).ToString();

                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[9];
                _sqlPara[0] = new SqlParameter("@StaffId", _staffDetails.StaffId);
                _sqlPara[1] = new SqlParameter("@Action", _staffDetails.Action);
                _sqlPara[2] = new SqlParameter("@OrganizationId", _staffDetails.OrganizationId);
                _sqlPara[3] = new SqlParameter("@CreatedBy", _staffDetails.CreatedById);
                _sqlPara[4] = new SqlParameter("@xmlStudyTaskList", StudyTaskList);
                _sqlPara[5] = new SqlParameter("@xmlStudies", StudyList);
                _sqlPara[6] = new SqlParameter("@xmlUnitRoleList", UnitRoles);
                _sqlPara[7] = new SqlParameter("@ActionDoneBy", _staffDetails.ActionDoneBy);
                _sqlPara[8] = new SqlParameter("@Reason", _staffDetails.ReasonArchive);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_StudyOrUnitRoleModification]", CommandType.StoredProcedure, _sqlPara);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (Convert.ToString(ds.Tables[0].Rows[0]["Success"]) == "1")
                    {
                        if (Convert.ToString(ds.Tables[0].Rows[0]["ActionType"]) == "UpdateUnitRole")
                        {
                            if (ds.Tables[1].Rows.Count > 0)
                            {
                                string msg = "";
                                for (int i = 0; ds.Tables[1].Rows.Count > i; i++)
                                {
                                    if (i == ds.Tables[1].Rows.Count - 1)
                                    {
                                        msg += Convert.ToString(ds.Tables[1].Rows[i]["StatusValue"]) + " : " + Convert.ToString(ds.Tables[1].Rows[i]["TaskName"]) + " <br />by " + Convert.ToString(ds.Tables[1].Rows[i]["PerformedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                    }
                                    else if (i == 0)
                                    {
                                        msg = " Dear " + Convert.ToString(ds.Tables[1].Rows[i]["UserName"]) + ",<br /><br /> " + Convert.ToString(ds.Tables[1].Rows[i]["StatusValue"]) + " : " + Convert.ToString(ds.Tables[1].Rows[i]["TaskName"]) + "<br />";
                                    }
                                    else
                                    {
                                        msg += Convert.ToString(ds.Tables[1].Rows[i]["StatusValue"]) + " : " + Convert.ToString(ds.Tables[1].Rows[i]["TaskName"]) + "<br />";
                                    }
                                }

                                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[1].Rows[0]["EmailId"]), "Unit roles", msg);
                            }

                        }



                        _ApiResponse.success = 1;
                        _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                        _ApiResponse.data = ds;
                    }
                    else
                    {
                        _ApiResponse.success = 0;
                        _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                    }
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Invalid request";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_staffDetails),
                                  "eDelegationSetUp", "UpdateStudyTaskList", "[dbo].[usp_StudyOrUnitRoleModification]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region StudyTraining
        #region get study training

        //Need to check it if requiredin any case
        //internal ApiResponse GetStudyTraining(StaffInfoRef _StaffInfoRef)
        //{
        //    _ApiResponse = new ApiResponse();
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
        //        SqlParameter[] _sqlPara = new SqlParameter[2];
        //        _sqlPara[0] = new SqlParameter("@StaffIdRef", _StaffInfoRef.StaffId);
        //        _sqlPara[1] = new SqlParameter("@OrganizationId", _StaffInfoRef.OrganizationId);
        //        dt = _DAL.ExecuteCommand("[dbo].[usp_GetStudyTraining]", CommandType.StoredProcedure, _sqlPara);
        //        _ApiResponse.success = 1;
        //        _ApiResponse.message = "";
        //        _ApiResponse.data = dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        _ApiResponse.success = 0;
        //        SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "GetStudyTraining", "[dbo].[usp_GetStudyTraining]", "Clinical Trail");
        //    }
        //    return _ApiResponse;
        //}
        #endregion

        #region get study training
        internal ApiResponse UpdateStudyTraining(StaffStudyTrainingReq _StaffInfoRef)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[5];
                _sqlPara[0] = new SqlParameter("@StudyIdRef", _StaffInfoRef.StudyId);
                _sqlPara[1] = new SqlParameter("@StaffIdRef", _StaffInfoRef.StaffId);
                _sqlPara[2] = new SqlParameter("@OrganizationIdRef", _StaffInfoRef.OrganizationId);
                _sqlPara[3] = new SqlParameter("@StudyStatus", _StaffInfoRef.StudyStatus);
                _sqlPara[4] = new SqlParameter("@TimeGapTillTraning", _StaffInfoRef.TimeGapTillTraning);
                //_sqlPara[4] = new SqlParameter("@AssignStudyToUserId", _StaffInfoRef.AssignStudyToUserId);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_EditAsinineStudyTraining]", CommandType.StoredProcedure, _sqlPara);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].TableName = "ResponseTable";
                    ds.Tables[1].TableName = "TrainingStatus";
                    _ApiResponse.success = 1;
                    _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["Response"]);
                    _ApiResponse.data = ds;
                }
            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;

                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_StaffInfoRef),
                   "eDelegationSetUp", "UpdateStudyTraining", "[dbo].[usp_EditAsinineStudyTraining]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion
        #endregion

        #region Get Rejection Fun
        internal ApiResponse GetRejectionFun(StaffInfoRef obj)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[2];

                _sqlPara[0] = new SqlParameter("@RequestCreatedBy", obj.StaffId);
                _sqlPara[1] = new SqlParameter("@OrganizationIdRef", obj.OrganizationId);
                dt = _DAL.ExecuteCommand("[dbo].[usp_GetRejection]", CommandType.StoredProcedure, _sqlPara);
                if (dt.Rows.Count > 0)
                {
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "Rejection List";
                    _ApiResponse.data = dt;
                }
            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(obj),
                   "eDelegationSetUp", "GetRejectionFun", "[dbo].[usp_GetRejection]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region Check LM Or PI Auth
        internal ApiResponse CheckLMOrPIAuthFun(StaffInfoRef obj)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[2];

                _sqlPara[0] = new SqlParameter("@StaffId", obj.StaffId);
                _sqlPara[1] = new SqlParameter("@OrganizationId", obj.OrganizationId);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_CheckLMOrPIAuth]", CommandType.StoredProcedure, _sqlPara);
                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "StaffName";
                    ds.Tables[1].TableName = "StudyName";
                    ds.Tables[2].TableName = "AdminCheck";
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "LM Or Study Name";
                    _ApiResponse.data = ds;
                }

            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(obj),
                   "eDelegationSetUp", "CheckLMOrPIAuthFun", "[dbo].[usp_CheckLMOrPIAuth]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region Change line manager
        internal ApiResponse ChangeLineManagerFun(ChangeLineMGRef obj)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[6];
                string xmlStaff = string.Empty;
                if (obj.Action == "Update")
                    xmlStaff = new XElement("ParentNode", from c in obj.LinkedEmployeeId
                                                          select new XElement("StaffList", new XElement("StaffId", c.ToUpper()))).ToString();

                _sqlPara[0] = new SqlParameter("@LineManagerId", obj.LineManagerId);
                _sqlPara[1] = new SqlParameter("@OrganizationId", obj.OrganizationId);
                _sqlPara[2] = new SqlParameter("@Action", obj.Action);
                _sqlPara[3] = new SqlParameter("@NewLineManagerId", obj.NewLineManagerId);
                _sqlPara[4] = new SqlParameter("@CreatedBy", obj.CreatedBy);
                _sqlPara[5] = new SqlParameter("@EmpList", xmlStaff);

                dt = _DAL.ExecuteCommand("[dbo].[usp_ChangeEmpWithLineManager]", CommandType.StoredProcedure, _sqlPara);
                if (dt.Rows.Count > 0)
                {

                    _ApiResponse.success = 1;
                    if (obj.Action == "Get")
                        _ApiResponse.message = "Linked Employee List";
                    else if (obj.Action == "Update")
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string msgOldUser = "Hello, <br /><br />You have been removed as a Line manager by " + Convert.ToString(dt.Rows[i]["RemovedOrAssignedBy"]) + "  for " + dt.Rows[i]["UserName"] + ".<br /><br /> Thank you,<br /> AscensionQ Team" + " <br /> ";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["OldLMEmail"]), "Removed Line Manager", msgOldUser);

                            string msgNewUser = "Hello, <br /><br />You have been assigned as a Line manager by " + Convert.ToString(dt.Rows[i]["RemovedOrAssignedBy"]) + "  for " + dt.Rows[i]["UserName"] + ".<br /><br /> Thank you,<br /> AscensionQ Team" + " <br /> ";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["NewLMEmail"]), "Assigned Line Manager", msgNewUser);
                        }
                        _ApiResponse.message = "Record has been updated successfully";
                    }
                    _ApiResponse.data = dt;
                }

            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(obj),
                    "eDelegationSetUp", "ChangeLineManagerFun", "[dbo].[usp_ChangeEmpWithLineManager]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region Change PI
        internal ApiResponse ChangePrincipalInvestigatorFun(ChangePIRef obj)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[6];
                string xmlStudies = string.Empty;
                if (obj.Action == "Update")
                    xmlStudies = new XElement("ParentNode", from c in obj.LinkedStudyId
                                                            select new XElement("StudyList", new XElement("StudyId", c.ToUpper()))).ToString();


                _sqlPara[0] = new SqlParameter("@StaffIdPI", obj.PiId);
                _sqlPara[1] = new SqlParameter("@OrganizationId", obj.OrganizationId);
                _sqlPara[2] = new SqlParameter("@Action", obj.Action);
                _sqlPara[3] = new SqlParameter("@NewPiId", obj.NewPiId);
                _sqlPara[4] = new SqlParameter("@StudyList", xmlStudies);
                _sqlPara[5] = new SqlParameter("@CreatedBy", obj.CreatedBy);

                dt = _DAL.ExecuteCommand("[dbo].[usp_ChangePIOfStudies]", CommandType.StoredProcedure, _sqlPara);
                if (dt.Rows.Count > 0)
                {

                    _ApiResponse.success = 1;
                    if (obj.Action == "Get")
                        _ApiResponse.message = "Linked Studies List";
                    else if (obj.Action == "Update")
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string msgOldUser = "You have been removed as a Principle investigator by " + Convert.ToString(dt.Rows[i]["RemovedOrAssignedBy"]) + "  for Study " + dt.Rows[i]["StudyName"] + ".<br /><br /> Thank you,<br /> AscensionQ Team" + " <br /> ";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["OldPIEmail"]), "Removed PI", msgOldUser);

                            string msgNewUser = "You have been assigned as a Principle investigator by " + Convert.ToString(dt.Rows[i]["RemovedOrAssignedBy"]) + "  for Study " + dt.Rows[i]["StudyName"] + ".<br /><br /> Thank you,<br /> AscensionQ Team" + " <br /> ";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["NewPIEmail"]), "Assigned PI", msgNewUser);
                        }
                        _ApiResponse.message = "Record has been updated successfully";
                    }
                    _ApiResponse.data = dt;
                }

            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(obj),
                    "eDelegationSetUp", "ChangePrincipalInvestigatorFun", "[dbo].[usp_ChangePIOfStudies]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region Staff Role ApprovalFun
        internal ApiResponse StaffRoleApprovalFun(RoleApprovalRef obj)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                string xmlunitRoleList = string.Empty;
                if (obj.Action == "ApproveRejectAll")
                {
                    xmlunitRoleList = new XElement("ParentNode", from c in obj.UnitRoleList
                                                                 select new XElement("UnitRoleList", new XElement("UnitRoleId", c.ToUpper()))).ToString();
                }

                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[8];
                _sqlPara[0] = new SqlParameter("@StaffId", obj.StaffId);
                _sqlPara[1] = new SqlParameter("@Action", obj.Action);
                _sqlPara[2] = new SqlParameter("@Approval", obj.ApprovalStatus);
                _sqlPara[3] = new SqlParameter("@OrganizationId", obj.OrganizationId);
                _sqlPara[4] = new SqlParameter("@AssignUnitRolePerUserId", obj.AssignUnitRolePerUserId);
                _sqlPara[5] = new SqlParameter("@ActionStatus", obj.ActionStatus);
                _sqlPara[6] = new SqlParameter("@ReasonArchive", obj.ReasonArchive);
                _sqlPara[7] = new SqlParameter("@UnitRoleList", xmlunitRoleList);

                dt = _DAL.ExecuteCommand("[dbo].[usp_UnitRoleApproval]", CommandType.StoredProcedure, _sqlPara);
                if (dt.Rows.Count > 0)
                {

                    _ApiResponse.success = 1;
                    if (obj.Action == "Get")
                        _ApiResponse.message = "Pending Aprovel List";
                    else if (obj.Action == "Update")
                    {
                        _ApiResponse.message = "Record has been updated successfully";
                        if (Convert.ToString(dt.Rows[0]["ApproveLevel"]) == "2")
                        {
                            string msgNewUser = " Hello, <br /><br /> Unit Role: " + Convert.ToString(dt.Rows[0]["UnitRole"]) + " <br /> has/have been rejected by " + Convert.ToString(dt.Rows[0]["StaffName"]) + " <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["LMEMailId"]), "Unit Role Rejected", msgNewUser);
                        }
                        else
                        {
                            string msgNewUser = " Hello, <br /><br /> Unit Role: " + Convert.ToString(dt.Rows[0]["UnitRole"]) + " <br /> has/have been accepted by " + Convert.ToString(dt.Rows[0]["StaffName"]) + " <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["LMEMailId"]), "Unit Role Accepted", msgNewUser);
                        }

                    }
                    else if (obj.Action == "ApproveRejectAll")
                    {
                        _ApiResponse.message = "Record has been updated successfully";
                        if (Convert.ToString(dt.Rows[0]["ApproveLevel"]) == "2")
                        {
                            string msgNewUser = " Hello, <br /><br /> Unit Role(s): " + Convert.ToString(dt.Rows[0]["UnitRole"]) + " <br /> has/have been rejected by " + Convert.ToString(dt.Rows[0]["StaffName"]) + " <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["LMEMailId"]), "Unit Role Rejected", msgNewUser);
                        }
                        else
                        {
                            string msgNewUser = " Hello, <br /><br /> Unit Role(s): " + Convert.ToString(dt.Rows[0]["UnitRole"]) + " <br /> has/have been accepted by " + Convert.ToString(dt.Rows[0]["StaffName"]) + " <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["LMEMailId"]), "Unit Role Accepted", msgNewUser);
                        }

                    }

                    _ApiResponse.data = dt;
                }

            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(obj),
                    "eDelegationSetUp", "StaffRoleApprovalFun", "[dbo].[usp_UnitRoleApproval]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region Staff Study task ApprovalFun
        internal ApiResponse StaffStudyRoleApprovalFun(StudyRoleApprovalRef obj)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                string xmlstudyList = string.Empty;
                if (obj.Action == "MultipleStudyTaskApproveAndReject")
                {
                    xmlstudyList = new XElement("ParentNode", from c in obj.StudyIds
                                                              select new XElement("StudyIdList", new XElement("StudyId", c.ToUpper()))).ToString();
                }
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[9];
                _sqlPara[0] = new SqlParameter("@StaffId", obj.StaffId);
                _sqlPara[1] = new SqlParameter("@Action", obj.Action);
                _sqlPara[2] = new SqlParameter("@Approval", obj.ApprovalStatus);
                _sqlPara[3] = new SqlParameter("@OrganizationId", obj.OrganizationId);
                _sqlPara[4] = new SqlParameter("@AssignUnitRolePerUserId", obj.AssignStudyRoleId);
                _sqlPara[5] = new SqlParameter("@ActionStatus", obj.ActionStatus);
                _sqlPara[6] = new SqlParameter("@ReasonArchive", obj.ReasonArchive);
                _sqlPara[7] = new SqlParameter("@StudyId", obj.StudyId);
                _sqlPara[8] = new SqlParameter("@StudyIds", xmlstudyList);

                dt = _DAL.ExecuteCommand("[dbo].[usp_StaffStudyRoleApproval]", CommandType.StoredProcedure, _sqlPara);
                if (dt.Rows.Count > 0)
                {
                    _ApiResponse.success = 1;
                    if (obj.Action == "Get")
                        _ApiResponse.message = "Pending Aprovel List";
                    else if (obj.Action == "Update")
                    {
                        _ApiResponse.message = "Record has been updated successfully";
                        if (obj.ApprovalStatus == 2)
                        {
                            string msgNewUser = " Hello, <br /><br /> Study: " + Convert.ToString(dt.Rows[0]["StudyName"]) + "  <br /><br />Study task: " + Convert.ToString(dt.Rows[0]["StudyTaskName"]) + " <br /> has been rejected by " + Convert.ToString(dt.Rows[0]["StaffName"]) + " <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["LMEMailId"]), "Study Task Rejected", msgNewUser);
                        }
                        else
                        {
                            string msgNewUser = " Hello, <br /><br /> Study: " + Convert.ToString(dt.Rows[0]["StudyName"]) + "  <br /><br /> Study task: " + Convert.ToString(dt.Rows[0]["StudyTaskName"]) + " <br /> has been accepted by " + Convert.ToString(dt.Rows[0]["StaffName"]) + " <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["LMEMailId"]), "Study Task Accepted", msgNewUser);
                        }
                    }
                    else if (obj.Action == "AllStudyTaskApproveAndReject")
                    {
                        _ApiResponse.message = Convert.ToString(dt.Rows[0]["Response"]);
                        if (obj.ApprovalStatus == 2)
                        {
                            string msgNewUser = string.Empty;
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                if (i == 0)
                                {
                                    msgNewUser = " Hello, <br /><br /> Study: " + Convert.ToString(dt.Rows[0]["StudyName"]) + "  <br /><br /> Study task: " + Convert.ToString(dt.Rows[i]["StudyTaskName"]) + "<br />";
                                }
                                else
                                {
                                    msgNewUser += "Study task: " + Convert.ToString(dt.Rows[i]["StudyTaskName"]) + "<br />";
                                }
                            }
                            msgNewUser += "<br /> have been rejected by " + Convert.ToString(dt.Rows[0]["StaffName"]) + " <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["LMEMailId"]), "Study Task Rejected", msgNewUser);
                        }
                        else
                        {
                            string msgNewUser = string.Empty;
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                if (i == 0)
                                {
                                    msgNewUser = " Hello, <br /><br /> Study: " + Convert.ToString(dt.Rows[0]["StudyName"]) + "  <br /><br /> Study task: " + Convert.ToString(dt.Rows[i]["StudyTaskName"]) + " <br />";
                                }
                                else
                                {
                                    msgNewUser += "Study task: " + Convert.ToString(dt.Rows[i]["StudyTaskName"]) + " <br />";
                                }
                            }
                            msgNewUser += "<br /> have been accepted by " + Convert.ToString(dt.Rows[0]["StaffName"]) + " <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["LMEMailId"]), "Study Task Accepted", msgNewUser);
                        }
                    }
                    else if (obj.Action == "MultipleStudyTaskApproveAndReject")
                    {
                        _ApiResponse.message = Convert.ToString(dt.Rows[0]["Response"]);
                        if (obj.ApprovalStatus == 2)
                        {
                            string msgNewUser = string.Empty;
                            string StudyTask = "";
                            int Rank = 0;

                            if (dt.Rows.Count > 0)
                            {
                                Rank = Convert.ToInt32(dt.Rows[0]["Rank"]);

                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    if (Rank == Convert.ToInt32(dt.Rows[i]["Rank"]) && i != dt.Rows.Count)
                                    {
                                        StudyTask += "Study task: " + Convert.ToString(dt.Rows[i]["StudyTaskName"]) + " <br />";
                                    }
                                    else
                                    {
                                        Rank = Convert.ToInt32(dt.Rows[i]["Rank"]);
                                        msgNewUser = " Hello, <br /><br /> Study: " + Convert.ToString(dt.Rows[i - 1]["StudyName"]) + "  <br /><br />";
                                        msgNewUser += StudyTask;
                                        StudyTask = "";
                                        msgNewUser += "<br /> have been rejected by " + Convert.ToString(dt.Rows[0]["StaffName"]) + " <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                        SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[i - 1]["LMEMailId"]), "Study Task Rejected", msgNewUser);
                                        msgNewUser = "";
                                    }

                                    if (i == (dt.Rows.Count - 1))
                                    {
                                        msgNewUser = " Hello, <br /><br /> Study: " + Convert.ToString(dt.Rows[i]["StudyName"]) + "  <br /><br />";
                                        msgNewUser += StudyTask;
                                        msgNewUser += "<br /> have been rejected by " + Convert.ToString(dt.Rows[0]["StaffName"]) + " <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                        SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[i]["LMEMailId"]), "Study Task Rejected", msgNewUser);
                                    }
                                }


                            }
                        }
                        else
                        {
                            string msgNewUser = string.Empty;
                            string StudyTask = "";
                            int Rank = 0;

                            if (dt.Rows.Count > 0)
                            {
                                Rank = Convert.ToInt32(dt.Rows[0]["Rank"]);

                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    if (Rank == Convert.ToInt32(dt.Rows[i]["Rank"]) && i != dt.Rows.Count)
                                    {
                                        StudyTask += "Study task: " + Convert.ToString(dt.Rows[i]["StudyTaskName"]) + " <br />";
                                    }
                                    else
                                    {
                                        Rank = Convert.ToInt32(dt.Rows[i]["Rank"]);
                                        msgNewUser = " Hello, <br /><br /> Study: " + Convert.ToString(dt.Rows[i - 1]["StudyName"]) + "  <br /><br />";
                                        msgNewUser += StudyTask;
                                        StudyTask = "";
                                        msgNewUser += "<br /> have been accepted by " + Convert.ToString(dt.Rows[0]["StaffName"]) + " <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                        SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[i - 1]["LMEMailId"]), "Study Task Accepted", msgNewUser);
                                        msgNewUser = "";
                                    }

                                    if (i == (dt.Rows.Count - 1))
                                    {
                                        msgNewUser = " Hello, <br /><br /> Study: " + Convert.ToString(dt.Rows[i]["StudyName"]) + "  <br /><br />";
                                        msgNewUser += StudyTask;
                                        msgNewUser += "<br /> have been accepted by " + Convert.ToString(dt.Rows[0]["StaffName"]) + " <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                        SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[i]["LMEMailId"]), "Study Task Accepted", msgNewUser);
                                    }
                                }


                            }
                        }


                    }
                    _ApiResponse.data = dt;
                }
            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(obj),
                    "eDelegationSetUp", "StaffStudyRoleApprovalFun", "[dbo].[usp_StaffStudyRoleApproval]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region E-delegation StudySummary
        internal ApiResponse StudySummary(OtherItemDD _otherItemDD)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[4];
                _sqlPara[0] = new SqlParameter("@OrganizationId", _otherItemDD.OrganizationId);
                _sqlPara[1] = new SqlParameter("@StudyId", _otherItemDD.StudyIdRef);
                _sqlPara[2] = new SqlParameter("@ActionType", _otherItemDD.ActionType);
                _sqlPara[3] = new SqlParameter("@staffId", _otherItemDD.StaffIDRef);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_StudySummaryDetails]", CommandType.StoredProcedure, _sqlPara);
                if (ds.Tables.Count > 0)
                {


                    _ApiResponse.success = 1;
                    _ApiResponse.message = "";
                    _ApiResponse.data = ds;
                }
            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_otherItemDD),
                    "eDelegationSetUp", "StudySummary", "[dbo].[usp_StudySummaryDetails]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        internal ApiResponse GetEdelegationStudies(OtherItemDD _otherItemDD)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[3];
                _sqlPara[0] = new SqlParameter("@OrganizationId", _otherItemDD.OrganizationId);
                _sqlPara[1] = new SqlParameter("@StaffId", _otherItemDD.StaffIDRef);
                _sqlPara[2] = new SqlParameter("@Action", _otherItemDD.ActionType);
                dt = _DAL.ExecuteCommand("[dbo].[Usp_GetEdelegationStudy]", CommandType.StoredProcedure, _sqlPara);
                if (dt.Rows.Count > 0)
                {
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "";
                    _ApiResponse.data = dt;
                }
            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "Study List", "edelegation Studies", "[dbo].[Usp_GetEdelegationStudy]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region GetIndividuaRecord
        public ApiResponse GetPageRecordsByPageName(OtherItemDD _otherItemDD)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            string msg = string.Empty;
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[4];
                _sqlPara[0] = new SqlParameter("@OrganizationId", _otherItemDD.OrganizationId);
                _sqlPara[1] = new SqlParameter("@StaffId", _otherItemDD.StaffIDRef);
                _sqlPara[2] = new SqlParameter("@PageName", _otherItemDD.PageName);
                _sqlPara[3] = new SqlParameter("@CreatedBy", _otherItemDD.CreatedBy);

                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[Usp_GetPageRecordsByPageName]", CommandType.StoredProcedure, _sqlPara);
                if (ds.Tables.Count > 0)
                {
                    switch (_otherItemDD.PageName)
                    {
                        case "UserProfile":
                            ds.Tables[0].TableName = "ApproveList";
                            ds.Tables[1].TableName = "Archive";
                            ds.Tables[2].TableName = "ApprovedLineManager";
                            ds.Tables[3].TableName = "NewList";
                            ds.Tables[4].TableName = "StaffRoleList";
                            ds.Tables[5].TableName = "StudyTaskList";
                            ds.Tables[6].TableName = "UnitRoleList";
                            ds.Tables[7].TableName = "EmpNum";
                            ds.Tables[8].TableName = "SiteList";
                            ds.Tables[9].TableName = "DepartmentList";
                            ds.Tables[10].TableName = "StudyList";
                            ds.Tables[12].TableName = "DocumentList";
                            break;
                        case "StudyDelegationLog":
                            ds.Tables[0].TableName = "StudytaskList";
                            ds.Tables[1].TableName = "ApprovedList";
                            ds.Tables[2].TableName = "EmpNum";
                            ds.Tables[3].TableName = "UnitRoleBasedList";
                            ds.Tables[4].TableName = "ArchivedStudy";
                            ds.Tables[5].TableName = "NewStudy";
                            break;
                        case "AuthorisationRequest":
                            ds.Tables[0].TableName = "UnitRoleList";
                            ds.Tables[1].TableName = "View";
                            ds.Tables[2].TableName = "Removed";
                            ds.Tables[3].TableName = "New";
                            ds.Tables[4].TableName = "Archive";
                            ds.Tables[5].TableName = "Approve";
                            ds.Tables[6].TableName = "PendingUnitRole";
                            break;
                        case "StudyAuthorisation":
                            ds.Tables[0].TableName = "StudyTaskList";
                            ds.Tables[1].TableName = "NewStudy";
                            ds.Tables[2].TableName = "AuthorisedStudy";
                            ds.Tables[3].TableName = "RemovedStudy";
                            ds.Tables[4].TableName = "RejectedStudy";
                            ds.Tables[5].TableName = "ArchivedStudy";
                            ds.Tables[6].TableName = "StudyPendingTask";
                            break;
                        case "GetSiteDetails":
                            ds.Tables[0].TableName = "SiteNum";
                            ds.Tables[1].TableName = "SiteList";
                            break;
                        case "AddStudy":
                            ds.Tables[0].TableName = "DepartmentList";
                            ds.Tables[1].TableName = "ProjectList";//added on 13-06-2023 Due to CRF new modules come in
                            ds.Tables[2].TableName = "Pages";//added on 26-12-2023 Due to dynamic page accessibility
                            ds.Tables[3].TableName = "PageActions";//added on 26-12-2023 Due to dynamic page accessibility
                            break;
                    }

                    _ApiResponse.success = 1;
                    _ApiResponse.message = "success";
                    _ApiResponse.data = ds;
                }

            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_otherItemDD), "eDelegationSetUp", "GetPageRecordsByPageName", "[dbo].[Usp_GetPageRecordsByPageName]", "Clinical Trail");
                _ApiResponse.success = 0;
                _ApiResponse.message = "No list found";
            }
            return _ApiResponse;
        }
        #endregion

        #region user training document
        internal ApiResponse SaveUserTrainingDocument(UserTrainingDocument _UserTrainingDocument)
        {
            string DocumentList = "";
            if (_UserTrainingDocument.DocumentList != null)
                DocumentList = GenerateXML.Instance.ToXmlString(_UserTrainingDocument.DocumentList);

            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                if (_UserTrainingDocument.Action == "Add")
                {
                    SqlParameter[] _sqlPara = new SqlParameter[5];
                    _sqlPara[0] = new SqlParameter("@StaffIdRef", _UserTrainingDocument.StaffIdref);
                    _sqlPara[1] = new SqlParameter("@OrganizationId", _UserTrainingDocument.OrganizationId);
                    _sqlPara[2] = new SqlParameter("@DocumentlistXml", DocumentList);
                    _sqlPara[3] = new SqlParameter("@IsActive", 1);
                    _sqlPara[4] = new SqlParameter("@Action", _UserTrainingDocument.Action);
                    ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_UserTrainingDocument]", CommandType.StoredProcedure, _sqlPara);
                }
                if (ds.Tables.Count > 0)
                {
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "";
                    _ApiResponse.data = ds;
                }
            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_UserTrainingDocument), "eDelegationSetUp", "SaveUserTrainingDocument", "[dbo].[usp_UserTrainingDocument]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region switch user
        public ApiResponse SwitchUser(StaffLogin _StaffLogin)
        {
            _ApiResponse = new ApiResponse();
            //DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[8];
                _SqlParameter[0] = new SqlParameter("@EmailId", Convert.ToString(_StaffLogin.EmailId));
                _SqlParameter[1] = new SqlParameter("@Password", Convert.ToString(_StaffLogin.Password));
                _SqlParameter[2] = new SqlParameter("@AccessCodeIndex0", _StaffLogin.AccessCodeIndex0);
                _SqlParameter[3] = new SqlParameter("@AccessCodeIndex1", _StaffLogin.AccessCodeIndex1);
                _SqlParameter[4] = new SqlParameter("@AccessCodeIndex2", _StaffLogin.AccessCodeIndex2);
                _SqlParameter[5] = new SqlParameter("@AccessCodeValue0", _StaffLogin.AccessCodeValue0);
                _SqlParameter[6] = new SqlParameter("@AccessCodeValue1", _StaffLogin.AccessCodeValue1);
                _SqlParameter[7] = new SqlParameter("@AccessCodeValue2", _StaffLogin.AccessCodeValue2);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_SwitchUser]", CommandType.StoredProcedure, _SqlParameter);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (Convert.ToString(ds.Tables[0].Rows[0]["Success"]) == "1")
                    {
                        StaffLoginDetails _obj = new StaffLoginDetails();
                        _obj.UserName = Convert.ToString(ds.Tables[0].Rows[0]["UserName"]);
                        _obj.Role = Convert.ToString(ds.Tables[0].Rows[0]["Roles"]);
                        _obj.StaffId = Convert.ToString(ds.Tables[0].Rows[0]["StaffId"]);
                        _obj.ProfilePictureUrl = Convert.ToString(ds.Tables[0].Rows[0]["ProfilePictureUrl"]);
                        _obj.OrganizationId = Convert.ToString(ds.Tables[0].Rows[0]["Organizationid"]);
                        _obj.EmailId = Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]);
                        _obj.AccountStatus = Convert.ToString(ds.Tables[0].Rows[0]["AccountStatus"]);
                        _obj.HaveProfileViewRights = Convert.ToString(ds.Tables[0].Rows[0]["HaveProfileViewRights"]);
                        _obj.HaveDelegationLogViewRights = Convert.ToString(ds.Tables[0].Rows[0]["HaveDelegationLogViewRights"]);
                        _obj.HaveProfileEditRights = Convert.ToString(ds.Tables[0].Rows[0]["HaveProfileEditRights"]);
                        _obj.HaveDelegationLogEditRights = Convert.ToString(ds.Tables[0].Rows[0]["HaveDelegationLogEditRights"]);
                        _obj.HavingLineManagerRights = Convert.ToString(ds.Tables[0].Rows[0]["HavingLineManagerRights"]);
                        _obj.HaveStudyTaskArchiveRights = Convert.ToString(ds.Tables[0].Rows[0]["HaveStudyTaskArchiveRights"]);
                        _obj.PrincipalInvestigator = Convert.ToString(ds.Tables[0].Rows[0]["PrincipalInvestigator"]);
                        _obj.IsAuthorized = Convert.ToString(ds.Tables[0].Rows[0]["IsAuthorized"]);
                        _obj.IsTrainingCompleted = Convert.ToBoolean(ds.Tables[0].Rows[0]["IsTrainingCompleted"]);
                        _obj.ISUnitSOPCompleted = Convert.ToBoolean(ds.Tables[0].Rows[0]["ISUnitSOPCompleted"]);
                        _obj.SchApprovalCount = Convert.ToString(ds.Tables[1].Rows[0]["SchApprovalCount"]);
                        _obj.ReviewersCount = Convert.ToString(ds.Tables[2].Rows[0]["ReviewersCount"]);
                        _ApiResponse.success = 1;
                        _ApiResponse.message = "Login success";
                        _ApiResponse.data = _obj;
                    }
                    else
                    {
                        _ApiResponse.success = 0;
                        _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["Message"]);
                    }
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Invalid login details";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_StaffLogin), "eDelegationSetUp", "SwitchUser", "[dbo].[usp_SwitchUser]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        internal ApiResponse ChekLMUser(OtherItemDD _OtherItemDD)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[3];
                _sqlPara[0] = new SqlParameter("@StaffId", _OtherItemDD.StaffIDRef);
                _sqlPara[1] = new SqlParameter("@OrganizationId", _OtherItemDD.OrganizationId);
                _sqlPara[2] = new SqlParameter("@Action", "CheckLMUsers");
                dt = _DAL.ExecuteCommand("[dbo].[usp_UnitRoleApproval]", CommandType.StoredProcedure, _sqlPara);
                _ApiResponse.success = 1;
                _ApiResponse.message = "";
                _ApiResponse.data = dt;
            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_OtherItemDD), "eDelegationSetUp", "ChekLMUser", "[dbo].[usp_UnitRoleApproval]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        internal ApiResponse NewsAndNotification(NewsAndNotification _NewsAndNotification)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[8];
                _sqlPara[0] = new SqlParameter("@NewsAndNotificationId", _NewsAndNotification.NewsAndNotificationId);
                _sqlPara[1] = new SqlParameter("@Image", _NewsAndNotification.Image);
                _sqlPara[2] = new SqlParameter("@Titile", _NewsAndNotification.Title);
                _sqlPara[3] = new SqlParameter("@About", _NewsAndNotification.About);
                _sqlPara[4] = new SqlParameter("@IsActive", _NewsAndNotification.IsActive);
                _sqlPara[5] = new SqlParameter("@OrganizationId", _NewsAndNotification.OrganizationId);
                _sqlPara[6] = new SqlParameter("@CreatedBy", _NewsAndNotification.StaffId);
                _sqlPara[7] = new SqlParameter("@ActionType", _NewsAndNotification.ActionType);
                dt = _DAL.ExecuteCommand("[dbo].[usp_NewsAndNotification]", CommandType.StoredProcedure, _sqlPara);
                _ApiResponse.success = 1;
                _ApiResponse.message = "";
                _ApiResponse.data = dt;
            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_NewsAndNotification), "eDelegationSetUp", "NewsAndNotification", "[dbo].[usp_NewsAndNotification]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        internal ApiResponse ContactInfo(ContactInfo _ContactInfo)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[12];
                _sqlPara[0] = new SqlParameter("@ContactInfoId", _ContactInfo.ContactInfoId);
                _sqlPara[1] = new SqlParameter("@FirstName", _ContactInfo.FirstName);
                _sqlPara[2] = new SqlParameter("@LastName", _ContactInfo.LastName);
                _sqlPara[3] = new SqlParameter("@Subject", _ContactInfo.Subject);
                _sqlPara[4] = new SqlParameter("@Email", _ContactInfo.Email);
                _sqlPara[5] = new SqlParameter("@Phone", _ContactInfo.Phone);
                _sqlPara[6] = new SqlParameter("@Country", _ContactInfo.Country);
                _sqlPara[7] = new SqlParameter("@Reason", _ContactInfo.Reason);
                _sqlPara[8] = new SqlParameter("@Query", _ContactInfo.Query);
                _sqlPara[9] = new SqlParameter("@IsActive", _ContactInfo.IsActive);
                _sqlPara[10] = new SqlParameter("@OrganizationId", _ContactInfo.OrganizationId);
                _sqlPara[11] = new SqlParameter("@ActionType", _ContactInfo.ActionType);
                dt = _DAL.ExecuteCommand("[dbo].[usp_ContactInfo]", CommandType.StoredProcedure, _sqlPara);
                _ApiResponse.success = 1;
                _ApiResponse.message = "";
                _ApiResponse.data = dt;
            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_ContactInfo), "eDelegationSetUp", "ContactInfo", "[dbo].[usp_ContactInfo]", "Clinical Trail");
            }
            return _ApiResponse;
        }


        #region GetIndividuaRecord
        public ApiResponse GetPendingUnitRoleandStudyTask(OtherItemDD _otherItemDD)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            string msg = string.Empty;
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[4];
                _sqlPara[0] = new SqlParameter("@OrganizationId", _otherItemDD.OrganizationId);
                _sqlPara[1] = new SqlParameter("@StaffId", _otherItemDD.StaffIDRef);
                _sqlPara[2] = new SqlParameter("@ActionType", _otherItemDD.ActionType);
                _sqlPara[3] = new SqlParameter("@StudyIdRef", _otherItemDD.StudyIdRef);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[USP_Get_PendingUnitRole_StudyTask]", CommandType.StoredProcedure, _sqlPara);
                if (ds.Tables.Count > 0)
                {
                    if (_otherItemDD.ActionType == "PendingUnitRoleFromUser" || _otherItemDD.ActionType == "PendingUnitRoleFromLM")
                    {
                        ds.Tables[0].TableName = "PendingUnitRole";
                    }
                    else
                    {
                        ds.Tables[0].TableName = "PendingStudyTask";
                    }
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "success";
                    _ApiResponse.data = ds;
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_otherItemDD), "eDelegationSetUp", "GetPendingUnitRoleandStudyTask", "[dbo].[USP_Get_PendingUnitRole_StudyTask]", "Clinical Trail");
                _ApiResponse.success = 0;
                _ApiResponse.message = "No list found";
            }
            return _ApiResponse;
        }
        #endregion

        internal ApiResponse SystemAlert(SystemAlert _SystemAlert)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[10];
                _sqlPara[0] = new SqlParameter("@SystemAlertId", _SystemAlert.SystemAlertId);
                _sqlPara[1] = new SqlParameter("@Message", _SystemAlert.Message);
                //_sqlPara[2] = new SqlParameter("@FromDate", _SystemAlert.FromDate == null ? _SystemAlert.FromDate : Convert.ToDateTime(_SystemAlert.FromDate).ToString());
                //_sqlPara[3] = new SqlParameter("@ToDate", _SystemAlert.ToDate == null ? _SystemAlert.ToDate : Convert.ToDateTime(_SystemAlert.ToDate).ToString());
                _sqlPara[2] = new SqlParameter("@FromDate", _SystemAlert.FromDate == null ? DateTime.Now : DateTime.ParseExact(_SystemAlert.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                _sqlPara[3] = new SqlParameter("@ToDate", _SystemAlert.ToDate == null ? DateTime.Now : DateTime.ParseExact(_SystemAlert.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                _sqlPara[4] = new SqlParameter("@IsRead", _SystemAlert.IsRead);
                _sqlPara[5] = new SqlParameter("@Priority", _SystemAlert.Priority);
                _sqlPara[6] = new SqlParameter("@IsActive", _SystemAlert.IsActive);
                _sqlPara[7] = new SqlParameter("@OrganizationId", _SystemAlert.OrganizationId);
                _sqlPara[8] = new SqlParameter("@StaffId", _SystemAlert.StaffId);
                _sqlPara[9] = new SqlParameter("@ActionType", _SystemAlert.ActionType);
                dt = _DAL.ExecuteCommand("[dbo].[Usp_SystemAlert]", CommandType.StoredProcedure, _sqlPara);
                _ApiResponse.success = 1;
                _ApiResponse.message = "";
                _ApiResponse.data = dt;
            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_SystemAlert), "eDelegationSetUp", "SystemAlert", "[dbo].[Usp_SystemAlert]", "Clinical Trail");
            }
            return _ApiResponse;
        }


        #region New E-delegation Section
        internal ApiResponse GetUsersLits(UsersDetailsProp _UsersDetailsProp)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                string FiltersProp = "";
                if (_UsersDetailsProp.FilterProp != null)
                    FiltersProp = GenerateXML.Instance.ToXmlString(_UsersDetailsProp.FilterProp);

                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlParameter = new SqlParameter[6];
                _sqlParameter[0] = new SqlParameter("@OrganizationId", _UsersDetailsProp.OrganizationId);
                _sqlParameter[1] = new SqlParameter("@CreatedBy", _UsersDetailsProp.CreatedBy);
                _sqlParameter[2] = new SqlParameter("@ActionType", _UsersDetailsProp.ActionType);
                _sqlParameter[3] = new SqlParameter("@Filters", FiltersProp);
                _sqlParameter[4] = new SqlParameter("@FilterId", _UsersDetailsProp.FilterId);
                _sqlParameter[5] = new SqlParameter("@FilterTitle", _UsersDetailsProp.FilterTitle);
                if (_UsersDetailsProp.ActionType == "GetAllUsersList" || _UsersDetailsProp.ActionType == "FilterSearch" || _UsersDetailsProp.ActionType == "GetAllUsersListForLM")
                {
                    ds = _DAL.ExecuteCommandGetDataSet("[dbo].[Usp_EDel_GetUsersLists]", CommandType.StoredProcedure, _sqlParameter);

                    ds.Tables[0].TableName = "UserList";
                    ds.Tables[1].TableName = "Filters";
                    ds.Tables[2].TableName = "FiltersOptions";
                    if (_UsersDetailsProp.ActionType == "FilterSearch" && _UsersDetailsProp.FilterId != "All")
                    {
                        ds.Tables[3].TableName = "SelectedFilter";
                    }
                    else if (_UsersDetailsProp.ActionType == "GetAllUsersList")
                    {
                        ds.Tables[3].TableName = "UnitRoleList";
                        ds.Tables[4].TableName = "LineManagerList";
                    }
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "Users list";
                    _ApiResponse.data = ds;
                }
                else if (_UsersDetailsProp.ActionType == "GetFilteredUserList")
                {
                    ds = _DAL.ExecuteCommandGetDataSet("[dbo].[Usp_EDel_GetUsersLists]", CommandType.StoredProcedure, _sqlParameter);
                    ds.Tables[0].TableName = "UserList";
                    ds.Tables[1].TableName = "Filters";

                    _ApiResponse.success = 1;
                    _ApiResponse.message = "Users list";
                    _ApiResponse.data = ds;
                }
                else if (_UsersDetailsProp.ActionType == "SaveFilters")
                {
                    ds = _DAL.ExecuteCommandGetDataSet("[dbo].[Usp_EDel_GetUsersLists]", CommandType.StoredProcedure, _sqlParameter);

                    ds.Tables[0].TableName = "FilterSuccessStory";
                    ds.Tables[1].TableName = "FilterList";

                    _ApiResponse.success = 1;
                    _ApiResponse.message = "Users list";
                    _ApiResponse.data = ds;
                }
                else if (_UsersDetailsProp.ActionType == "DeleteUserFilter")
                {
                    ds = _DAL.ExecuteCommandGetDataSet("[dbo].[Usp_EDel_GetUsersLists]", CommandType.StoredProcedure, _sqlParameter);
                    ds.Tables[0].TableName = "FilterSuccessStory";

                    _ApiResponse.success = 1;
                    _ApiResponse.message = "Users list";
                    _ApiResponse.data = ds;
                }

            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_UsersDetailsProp), "eDelegationSetUp", "GetUsersLits", "[dbo].[Usp_EDel_GetUsersLists]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        internal ApiResponse GetStudyManagementDataFun(StudyManagementReq _studyManagementReq)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                string FiltersProp = "";
                if (_studyManagementReq.FilterProp != null)
                    FiltersProp = GenerateXML.Instance.ToXmlString(_studyManagementReq.FilterProp);

                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlParameter = new SqlParameter[6];
                _sqlParameter[0] = new SqlParameter("@OrganizationId", _studyManagementReq.OrganizationId);
                _sqlParameter[1] = new SqlParameter("@StaffId", _studyManagementReq.StaffId);
                _sqlParameter[2] = new SqlParameter("@ActionType", _studyManagementReq.ActionType);
                _sqlParameter[3] = new SqlParameter("@Filters", FiltersProp);
                _sqlParameter[4] = new SqlParameter("@FilterId", _studyManagementReq.FilterId);
                _sqlParameter[5] = new SqlParameter("@FilterTitle", _studyManagementReq.FilterTitle);
                if (_studyManagementReq.ActionType == "GetAllStudyList" || _studyManagementReq.ActionType == "FilterSearch")
                {
                    ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_getStudyManagementData]", CommandType.StoredProcedure, _sqlParameter);

                    ds.Tables[0].TableName = "StudyManagementData";
                    ds.Tables[1].TableName = "Filters";
                    ds.Tables[2].TableName = "FiltersOptions";

                    if (_studyManagementReq.ActionType == "FilterSearch" && _studyManagementReq.FilterId != "All")
                    {
                        ds.Tables[3].TableName = "SelectedFilter";
                    }
                    else if (_studyManagementReq.ActionType == "GetAllStudyList")
                    {
                        ds.Tables[3].TableName = "StudyTaskList";
                        ds.Tables[4].TableName = "PIlist";
                    }
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "Study list";
                    _ApiResponse.data = ds;
                }
                else if (_studyManagementReq.ActionType == "GetFilteredStudyList")
                {
                    ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_getStudyManagementData]", CommandType.StoredProcedure, _sqlParameter);
                    ds.Tables[0].TableName = "StudyManagementData";
                    ds.Tables[1].TableName = "Filters";

                    _ApiResponse.success = 1;
                    _ApiResponse.message = "Study list";
                    _ApiResponse.data = ds;
                }
                else if (_studyManagementReq.ActionType == "SaveFilters")
                {
                    ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_getStudyManagementData]", CommandType.StoredProcedure, _sqlParameter);
                    ds.Tables[0].TableName = "FilterSuccessStory";
                    ds.Tables[1].TableName = "FilterList";

                    _ApiResponse.success = 1;
                    _ApiResponse.message = "Study list";
                    _ApiResponse.data = ds;
                }
                else if (_studyManagementReq.ActionType == "DeleteStudyFilter")
                {
                    ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_getStudyManagementData]", CommandType.StoredProcedure, _sqlParameter);
                    ds.Tables[0].TableName = "FilterSuccessStory";

                    _ApiResponse.success = 1;
                    _ApiResponse.message = "Study list";
                    _ApiResponse.data = ds;
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_studyManagementReq), "eDelegationSetUp", "GetStudyManagementDataFun", "[dbo].[usp_getStudyManagementData]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        internal ApiResponse UsersActivity(UsersActivity _usersActivity)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlParameter = new SqlParameter[12];
                _sqlParameter[0] = new SqlParameter("@OrganizationId", _usersActivity.OrganizationId);
                _sqlParameter[1] = new SqlParameter("@Action", _usersActivity.Action);
                _sqlParameter[2] = new SqlParameter("@AccessCode", _usersActivity.AccessCode);
                _sqlParameter[3] = new SqlParameter("@Password", _usersActivity.Password);
                _sqlParameter[4] = new SqlParameter("@StaffId", _usersActivity.StaffId);
                _sqlParameter[5] = new SqlParameter("@HaveProfileViewRights", _usersActivity.HaveProfileViewRights);
                _sqlParameter[6] = new SqlParameter("@HaveProfileEditRights", _usersActivity.HaveProfileEditRights);
                _sqlParameter[7] = new SqlParameter("@HaveDelegationLogViewRights", _usersActivity.HaveDelegationLogViewRights);
                _sqlParameter[8] = new SqlParameter("@HaveDelegationLogEditRights", _usersActivity.HaveDelegationLogEditRights);
                _sqlParameter[9] = new SqlParameter("@HavingLineManagerRights", _usersActivity.HavingLineManagerRights);
                _sqlParameter[10] = new SqlParameter("@HaveStudyTaskArchiveRights", _usersActivity.HaveStudyTaskArchiveRights);
                _sqlParameter[11] = new SqlParameter("@CreatedBy", _usersActivity.CreatedBy);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[Usp_UsersActivity]", CommandType.StoredProcedure, _sqlParameter);
                if (_usersActivity.Action == "Get")
                {
                    ds.Tables[0].TableName = "UsersActivity";
                }
                else if(_usersActivity.Action == "GetMyVaultStaff")
                {
                    ds.Tables[0].TableName = "UserList";
                }

                _ApiResponse.success = 1;
                _ApiResponse.message = "Users Activity";
                _ApiResponse.data = ds;

            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_usersActivity), "eDelegationSetUp", "UsersActivity", "[dbo].[Usp_UsersActivity]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        internal ApiResponse CheckValueExist(CheckValueExistReq _CheckValueExistReq)
        {

            _ApiResponse = new ApiResponse();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlParameter = new SqlParameter[3];
                _sqlParameter[0] = new SqlParameter("@Key", _CheckValueExistReq.Key);
                _sqlParameter[1] = new SqlParameter("@Value", _CheckValueExistReq.Value);
                _sqlParameter[2] = new SqlParameter("@OrganizationId", _CheckValueExistReq.OrganizationId);
                DataTable dt = _DAL.ExecuteCommand("[dbo].[usp_CheckValueExist]", CommandType.StoredProcedure, _sqlParameter);
                _ApiResponse.success = Int16.Parse(Convert.ToString(dt.Rows[0]["success"]));
                _ApiResponse.message = Convert.ToString(dt.Rows[0]["message"]);
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_CheckValueExistReq), "eDelegationSetUp", "CheckValueExist", "[dbo].[usp_CheckValueExist]", "Clinical Trail");
            }

            return _ApiResponse;
        }

        internal ApiResponse GetPDFData(PrintPreview _PrintPreview)
        {

            _ApiResponse = new ApiResponse();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlParameter = new SqlParameter[3];
                _sqlParameter[0] = new SqlParameter("@StudyId", _PrintPreview.StudyId);
                _sqlParameter[1] = new SqlParameter("@OrganizationId", _PrintPreview.OrganizationId);
                _sqlParameter[2] = new SqlParameter("@ActionType", _PrintPreview.Action);
                DataSet ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_GetStudyPDFDetails]", CommandType.StoredProcedure, _sqlParameter);
                _ApiResponse.success = 1;
                _ApiResponse.message = Convert.ToString("success");
                _ApiResponse.data = ds;
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_PrintPreview), "eDelegationSetUp", "GetPDFData", "[dbo].[usp_GetStudyPDFDetails]", "Clinical Trail");
            }

            return _ApiResponse;
        }

        internal ApiResponse SaveVersion(PrintPreview _PrintPreview)
        {
            _ApiResponse = new ApiResponse();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlParameter = new SqlParameter[6];
                _sqlParameter[0] = new SqlParameter("@VersionId", _PrintPreview.VersionId);
                _sqlParameter[1] = new SqlParameter("@StudyId", _PrintPreview.StudyId);
                _sqlParameter[2] = new SqlParameter("@PDFUrl", _PrintPreview.FilePath);
                _sqlParameter[3] = new SqlParameter("@CreatedBy", _PrintPreview.StaffId);
                _sqlParameter[4] = new SqlParameter("@OrganizationId", _PrintPreview.OrganizationId);
                _sqlParameter[5] = new SqlParameter("@ActionName", _PrintPreview.Action);
                DataSet ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_StudyPDFVersion]", CommandType.StoredProcedure, _sqlParameter);
                _ApiResponse.success = 1;
                _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                _ApiResponse.data = ds;
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_PrintPreview), "eDelegationSetUp", "SaveVersion", "[dbo].[usp_StudyPDFVersion]", "Clinical Trail");
            }

            return _ApiResponse;
        }

        internal ApiResponse UserVariousActivity(StaffList _staffList)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                string StaffList = string.Empty;
                if (_staffList.StaffIdList != null)
                    StaffList = new XElement("StaffList", from c in _staffList.StaffIdList select new XElement("Staff", new XElement("StaffId", c.ToUpper()))).ToString();


                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[5];
                _SqlParameter[0] = new SqlParameter("@OrganizationId", _staffList.OrganizationId);
                _SqlParameter[1] = new SqlParameter("@CreatedBy", _staffList.CreatedBy);
                _SqlParameter[2] = new SqlParameter("@ActionType", _staffList.ActionType);
                _SqlParameter[3] = new SqlParameter("@StaffList", StaffList);
                _SqlParameter[4] = new SqlParameter("ReasonArchive", _staffList.ReasonArchive);

                dt = _DAL.ExecuteCommand("[dbo].[usp_UserManagementMultiAction]", CommandType.StoredProcedure, _SqlParameter);
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToString(dt.Rows[0]["Success"]) == "1")
                    {

                        switch (Convert.ToString(dt.Rows[0]["ActionType"]))
                        {
                            case "Approve":
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    string msgPassword = " Dear " + Convert.ToString(dt.Rows[i]["UserName"]) + ",<br /><br /> Welcome to Ascension Q! Your Clinical Trial login  details are as follows: <br /><br /> Email Id : " + Convert.ToString(dt.Rows[i]["EmailId"]) + "<br />Password: " + Convert.ToString(dt.Rows[i]["Dos"]) + "<br /><br />Please note that your Access Code details will be provided in a subsequent email.<br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                    SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[i]["EmailId"]), "Login Details: Password", msgPassword);

                                    string msgAccessCode = " Dear " + Convert.ToString(dt.Rows[i]["UserName"]) + ",<br /><br />Welcome to Ascension Q! Your Clinical Trial Access code details are as follows: <br /><br /> Access Code :" + Convert.ToString(dt.Rows[i]["ACC"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                    SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[i]["EmailId"]), "Login Details: Access Code", msgAccessCode);
                                }
                                break;
                            case "AuthArchivedUser":
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    string msg = " Dear " + Convert.ToString(dt.Rows[i]["UserName"]) + ",<br /><br />Your access to the system has been blocked by " + Convert.ToString(dt.Rows[i]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                    SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[i]["EmailId"]), "User Inactive", msg);
                                }
                                break;
                            case "AuthorizeAllNewUnitRole":
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    string msg = " Dear " + Convert.ToString(dt.Rows[i]["UserName"]) + ",<br /><br />  New Unit Role has been assigned to you. They are as follows: <br/>" + Convert.ToString(dt.Rows[i]["UnitRoles"]) + " <br /><br />Authorised by " + Convert.ToString(dt.Rows[i]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                    SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[i]["EmailId"]), "New Unit Role Authorised", msg);
                                }
                                break;
                            case "Unarchive":
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    string msgNew = " Dear " + Convert.ToString(dt.Rows[i]["UserName"]) + ",<br /><br />You are now unarchived by " + Convert.ToString(dt.Rows[i]["UnarchivedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                    SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[i]["UserEmail"]), "User unarchived", msgNew);

                                    msgNew = " Hello, <br />" + Convert.ToString(dt.Rows[i]["UserName"]) + ",<br /><br /> has now been unarchived by " + Convert.ToString(dt.Rows[i]["UnarchivedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                    SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[i]["LMEmail"]), "User unarchived", msgNew);
                                }
                                break;
                            case "Archive":
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    string msgNew = " Dear " + Convert.ToString(dt.Rows[i]["UserName"]) + ",<br /><br />You are now archive by " + Convert.ToString(dt.Rows[i]["ArchivedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                    SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[i]["UserEmail"]), "User Archive", msgNew);

                                    msgNew = " Hello, <br /><br />" + Convert.ToString(dt.Rows[i]["UserName"]) + " has now been archive by " + Convert.ToString(dt.Rows[i]["ArchivedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                    SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[i]["LMEmail"]), "User Archive", msgNew);
                                }
                                break;
                        }

                        _ApiResponse.success = 1;
                        _ApiResponse.message = Convert.ToString(dt.Rows[0]["MSG"]);
                        _ApiResponse.data = dt;
                    }
                    else
                    {
                        _ApiResponse.success = 0;
                        _ApiResponse.message = Convert.ToString(dt.Rows[0]["MSG"]);
                        _ApiResponse.data = dt;
                    }
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Invalid request";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_staffList), "eDelegationSetUp", "UserVariousActivity", "[dbo].[usp_UserManagementMultiAction]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        internal ApiResponse StudyVariousActivity(StudyList _studyList)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                string Studylist = string.Empty;
                if (_studyList.StudyIdList != null)
                    Studylist = new XElement("StudyList", from c in _studyList.StudyIdList select new XElement("Study", new XElement("StudyId", c.ToUpper()))).ToString();

                string StudyMultilist = string.Empty;
                if (_studyList.StudyListMulti != null)
                    StudyMultilist = new XElement("StudyMultiList", from c in _studyList.StudyListMulti select new XElement("StudyMulti", new XElement("StudyId", c.StudyId.ToUpper()), new XElement("ActionPerformBy", c.ActionPerformedBy.ToUpper()), new XElement("StaffId", c.StaffId.ToUpper()))).ToString();


                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[6];
                _SqlParameter[0] = new SqlParameter("@OrganizationId", _studyList.OrganizationId);
                _SqlParameter[1] = new SqlParameter("@CreatedBy", _studyList.CreatedBy);
                _SqlParameter[2] = new SqlParameter("@ActionType", _studyList.ActionType);
                _SqlParameter[3] = new SqlParameter("@StudyList", Studylist);
                _SqlParameter[4] = new SqlParameter("@ReasonArchive", _studyList.ReasonArchive);
                _SqlParameter[5] = new SqlParameter("@MultiStudy", StudyMultilist);

                dt = _DAL.ExecuteCommand("[dbo].[usp_StudyManagementMultiAction]", CommandType.StoredProcedure, _SqlParameter);
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToString(dt.Rows[0]["Success"]) == "1")
                    {
                        if (Convert.ToString(dt.Rows[0]["ActionType"]) == "Archive")
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string msgNewUser = " Hello, <br /><br /> Request to archive study : " + Convert.ToString(dt.Rows[i]["StudyName"]) + " <br /> has been sent by : " + Convert.ToString(dt.Rows[i]["ArchivedBy"]) + " <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[i]["PIEmail"]), "Study Archive", msgNewUser);
                            }
                        }
                        else if (Convert.ToString(dt.Rows[0]["ActionType"]) == "Study unarchived")
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string msgNewUser = "Hello, <br /><br /> Study: " + Convert.ToString(dt.Rows[i]["StudyName"]) + " <br /> has been unarchived by: " + Convert.ToString(dt.Rows[i]["UnrchivedBy"]) + " <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[i]["PIEmail"]), "Study unarchived", msgNewUser);
                            }
                        }
                        else if ((Convert.ToString(dt.Rows[0]["ActionType"])) == "ApproveStaffForStudy")
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string msg = " Dear " + Convert.ToString(dt.Rows[i]["UserName"]) + ",<br /><br /> Your Study Tasks are as follows:<br/>" + Convert.ToString(dt.Rows[i]["StudyRoles"]) + " <br /><br />For Study: " + Convert.ToString(dt.Rows[i]["StudyName"]) + "<br/>Authorised by: " + Convert.ToString(dt.Rows[i]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[i]["EmailId"]), "New Study tasks: Authorised", msg);
                            }
                        }
                        //else if ((Convert.ToString(dt.Rows[0]["ActionType"])) == "RejectedRequest")
                        //{

                        //}
                        else if ((Convert.ToString(dt.Rows[0]["ActionType"])) == "InActiveStudy")
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                string msgNewUser = "" + Convert.ToString(dt.Rows[i]["StudyName"]) + " has been archived By " + Convert.ToString(dt.Rows[i]["ArchivedBy"]) + " <br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[i]["StaffEmail"]), "Study Archived", msgNewUser);
                            }
                        }
                        _ApiResponse.success = 1;
                        _ApiResponse.message = Convert.ToString(dt.Rows[0]["MSG"]);
                        _ApiResponse.data = dt;
                    }
                    else
                    {
                        _ApiResponse.success = 0;
                        _ApiResponse.message = Convert.ToString(dt.Rows[0]["MSG"]);
                        _ApiResponse.data = dt;
                    }
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Invalid request";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_studyList), "eDelegationSetUp", "StudyVariousActivity", "[dbo].[usp_StudyManagementMultiAction]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region Site Related
        internal ApiResponse SiteModification(SiteDetails _siteDetails)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[10];
                _sqlPara[0] = new SqlParameter("@SiteId", _siteDetails.SiteId);
                _sqlPara[1] = new SqlParameter("@SiteNumber", _siteDetails.SiteNumber);
                _sqlPara[2] = new SqlParameter("@SiteName", _siteDetails.SiteName);
                _sqlPara[3] = new SqlParameter("@LogPic", _siteDetails.LogPic);
                _sqlPara[4] = new SqlParameter("@Location", _siteDetails.Location);
                _sqlPara[5] = new SqlParameter("@CreatedBy", _siteDetails.CreatedBy);
                _sqlPara[6] = new SqlParameter("@OrganizationId", _siteDetails.OrganizationId);
                _sqlPara[7] = new SqlParameter("@ActionName", _siteDetails.ActionName);
                _sqlPara[8] = new SqlParameter("@SiteUniqueId", _siteDetails.SiteUniqueId);
                _sqlPara[9] = new SqlParameter("@CompressedPic", _siteDetails.CompressedPic);
                dt = _DAL.ExecuteCommand("[dbo].[usp_EDel_SiteDetails]", CommandType.StoredProcedure, _sqlPara);
                _ApiResponse.success = 1;
                _ApiResponse.message = "";
                _ApiResponse.data = dt;
            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_siteDetails), "eDelegationSetUp", "SiteModification", "[dbo].[usp_EDel_SiteDetails]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

    }

}