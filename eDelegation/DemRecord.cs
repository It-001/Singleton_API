using ClinicalTrialAPI.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using DBAccess;
using System.Web.Configuration;
using System.Xml.Linq;
using System.Linq;
using ClinicalTrialAPI.Custom;

namespace ClinicalTrialAPI.eDelegation
{
    public class DemRecord
    {
        ApiResponse _ApiResponse;
        private DAL _DAL;
        #region Singleton
        private DemRecord() { }
        private static readonly Lazy<DemRecord> lazy = new Lazy<DemRecord>(() => new DemRecord());
        public static DemRecord Instance {
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
                SqlParameter[] _SqlParameter = new SqlParameter[8];
                _SqlParameter[0] = new SqlParameter("@EmailId", Convert.ToString(_StaffLogin.EmailId));
                _SqlParameter[1] = new SqlParameter("@Password", Convert.ToString(_StaffLogin.Password));
                _SqlParameter[2] = new SqlParameter("@AccessCodeIndex0", _StaffLogin.AccessCodeIndex0);
                _SqlParameter[3] = new SqlParameter("@AccessCodeIndex1", _StaffLogin.AccessCodeIndex1);
                _SqlParameter[4] = new SqlParameter("@AccessCodeIndex2", _StaffLogin.AccessCodeIndex2);
                _SqlParameter[5] = new SqlParameter("@AccessCodeValue0", _StaffLogin.AccessCodeValue0);
                _SqlParameter[6] = new SqlParameter("@AccessCodeValue1", _StaffLogin.AccessCodeValue1);
                _SqlParameter[7] = new SqlParameter("@AccessCodeValue2", _StaffLogin.AccessCodeValue2);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_UserLogin]", CommandType.StoredProcedure, _SqlParameter);
                if (ds.Tables[0].Rows.Count > 0) {
                    if (Convert.ToString(ds.Tables[0].Rows[0]["Success"]) == "1")
                    {
                        StaffLoginDetails _obj = new StaffLoginDetails();
                        _obj.UserName = Convert.ToString(ds.Tables[0].Rows[0]["UserName"]);
                        _obj.Role = Convert.ToString(ds.Tables[0].Rows[0]["Roles"]);
                        _obj.StaffId = Convert.ToString(ds.Tables[0].Rows[0]["StaffId"]);
                        _obj.OrganizationId = Convert.ToString(ds.Tables[0].Rows[0]["Organizationid"]);
                        _obj.EmailId = Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]);
                        _obj.AccountStatus = Convert.ToString(ds.Tables[0].Rows[0]["AccountStatus"]);
                        _obj.HaveProfileViewRights = Convert.ToString(ds.Tables[0].Rows[0]["HaveProfileViewRights"]);
                        _obj.HaveDelegationLogViewRights = Convert.ToString(ds.Tables[0].Rows[0]["HaveDelegationLogViewRights"]);
                        _obj.HaveProfileEditRights = Convert.ToString(ds.Tables[0].Rows[0]["HaveProfileEditRights"]);
                        _obj.HaveDelegationLogEditRights = Convert.ToString(ds.Tables[0].Rows[0]["HaveDelegationLogEditRights"]);
                        _obj.HavingLineManagerRights = Convert.ToString(ds.Tables[0].Rows[0]["HavingLineManagerRights"]);
                        _obj.PrincipalInvestigator = Convert.ToString(ds.Tables[0].Rows[0]["PrincipalInvestigator"]);
                        _obj.IsAuthorized = Convert.ToString(ds.Tables[0].Rows[0]["IsAuthorized"]);
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
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "StaffLoginFun", "[dbo].[usp_UserLogin]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

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
                SqlParameter[] _SqlParameter = new SqlParameter[37];
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
                _SqlParameter[27] = new SqlParameter("@HaveLineManagerRights", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.HaveLineManagerRights))? "0":Convert.ToString(_StaffRegistration.HaveLineManagerRights));
                _SqlParameter[28] = new SqlParameter("@xmlStudyTaskList", StudyTaskList);
                _SqlParameter[29] = new SqlParameter("@xmlStudies", StudyList);
                _SqlParameter[30] = new SqlParameter("@xmlUnitRoleList", UnitRoles);
                _SqlParameter[31] = new SqlParameter("@Action", _StaffRegistration.Action);
                _SqlParameter[32] = new SqlParameter("@LineManagerId", _StaffRegistration.LineManagerId);
                _SqlParameter[33] = new SqlParameter("@HaveProfileViewRights", _StaffRegistration.HaveProfileViewRights);
                _SqlParameter[34] = new SqlParameter("@HaveDelegationLogViewRights", _StaffRegistration.HaveDelegationLogViewRights);
                _SqlParameter[35] = new SqlParameter("@ReasonArchive", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.ReasonArchive)) ? string.Empty : Convert.ToString(_StaffRegistration.ReasonArchive));
                _SqlParameter[36] = new SqlParameter("@ActionDoneBy", string.IsNullOrEmpty(Convert.ToString(_StaffRegistration.ActionDoneBy)) ? string.Empty : Convert.ToString(_StaffRegistration.ActionDoneBy));

                dt = _DAL.ExecuteCommand("[dbo].[usp_StaffRegistration]", CommandType.StoredProcedure, _SqlParameter);
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToString(dt.Rows[0]["Success"]) == "1")
                    {
                        if (Convert.ToString(dt.Rows[0]["ActionType"]) == "Approve")
                        {
                            string msg = " Dear " + Convert.ToString(dt.Rows[0]["UserName"]) + ",<br /><br /> Welcome to Ascension Q! Your Clinical Trial login  details: <br /><br /> Email Id : " + Convert.ToString(dt.Rows[0]["EmailId"]) + "<br />Password: " + Convert.ToString(dt.Rows[0]["Dos"]) + "<br />" + "Access Code :" + Convert.ToString(dt.Rows[0]["ACC"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["EmailId"]), "Login Details", msg);
                        }
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
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "StaffRegistrationCTFun", "[dbo].[usp_StaffRegistration]", "Clinical Trail");
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
                SqlParameter[] _sqlPara = new SqlParameter[6];
                _sqlPara[0] = new SqlParameter("@StaffId", _unitRolePerUser.StaffId);
                _sqlPara[1] = new SqlParameter("@Action", _unitRolePerUser.Action);
                _sqlPara[2] = new SqlParameter("@OrganizationId", _unitRolePerUser.OrganizationId);
                _sqlPara[3] = new SqlParameter("@CreatedBy", _unitRolePerUser.CreatedBy);
                _sqlPara[4] = new SqlParameter("@UnitRoleId",_unitRolePerUser.UnitRoleId);
                _sqlPara[5] = new SqlParameter("@ReasonArchive", string.IsNullOrEmpty(Convert.ToString(_unitRolePerUser.ReasonArchive)) ? string.Empty : Convert.ToString(_unitRolePerUser.ReasonArchive));
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_GetIndividualDetails]", CommandType.StoredProcedure, _sqlPara);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (Convert.ToString(ds.Tables[0].Rows[0]["Success"]) == "1")
                    {
                        if(Convert.ToString(ds.Tables[0].Rows[0]["ActionType"])=="Get")
                        {
                            ds.Tables[0].TableName = "StaffRecord";
                            ds.Tables[1].TableName = "AssignedStudyList";
                            ds.Tables[2].TableName = "AssignedStudyTaskListPerStudy";
                            ds.Tables[3].TableName = "AssignedUnitRole";
                            ds.Tables[4].TableName = "RemoveUnitRole";
                            _ApiResponse.success = 1;
                            _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                            _ApiResponse.data = ds;
                        }
                        else
                        {
                         switch (Convert.ToString(ds.Tables[0].Rows[0]["ActionType"]))
                            {
                                case "AuthorizeAllRemovedUnitRole":
                                    msg = " Hi, <br /> " + Convert.ToString(ds.Tables[0].Rows[0]["UserName"]) + "<br /><br /> Welcome to Ascension Q! Your Unit Roles are<br/>" + Convert.ToString(ds.Tables[0].Rows[0]["UnitRoles"]) + " <br /><br />Removed by " + Convert.ToString(ds.Tables[0].Rows[0]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                    SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]), "In Active Unit Roles", msg);
                                    break;
                                case "AuthorizeRemovedUnitRole":
                                    msg = " Hi, <br /> " + Convert.ToString(ds.Tables[0].Rows[0]["UserName"]) + "<br /><br /> Welcome to Ascension Q! Your Unit Role<br/>" + Convert.ToString(ds.Tables[0].Rows[0]["UnitRoles"]) + " <br /><br />Removed by " + Convert.ToString(ds.Tables[0].Rows[0]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                    SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]), "In Active Unit Roles", msg);
                                    break;
                                case "AuthorizeAllNewUnitRole":
                                    msg = " Hi, <br /> " + Convert.ToString(ds.Tables[0].Rows[0]["UserName"]) + "<br /><br /> Welcome to Ascension Q! New Unit Role Assigned,these are given below <br/>" + Convert.ToString(ds.Tables[0].Rows[0]["UnitRoles"]) + " <br /><br />Authorised by " + Convert.ToString(ds.Tables[0].Rows[0]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                    SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]), "New Unit Role Authorised", msg);
                                    break;
                                case "AuthorizeNewUnitRole":
                                    msg = " Hi, <br /> " + Convert.ToString(ds.Tables[0].Rows[0]["UserName"]) + "<br /><br /> Welcome to Ascension Q! New Unit Role Assigned is given below<br/>" + Convert.ToString(ds.Tables[0].Rows[0]["UnitRoles"]) + " <br /><br />Authorised by " + Convert.ToString(ds.Tables[0].Rows[0]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                    SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]), "New Unit Role Authorised", msg);
                                    break;
                                case "AuthArchivedUser":
                                    msg = " Hi, <br /> " + Convert.ToString(ds.Tables[0].Rows[0]["UserName"]) + "<br /><br /> Welcome to Ascension Q! You are now Inactive by " + Convert.ToString(ds.Tables[0].Rows[0]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                    SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]), "User InActive", msg);
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
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "GetIndividuaRecord", "[dbo].[usp_GetIndividualDetails]", "Clinical Trail");
                _ApiResponse.success = 0;
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
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "SaveVolunteerDetailsFun", "[dbo].[usp_SaveVolunteerDetails]", "Clinical Trail");
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
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "StaffRoleCurdFunction", "[dbo].[usp_StaffRole]", "Clinical Trail");
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
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "UpdateStaffVolunteer", "[dbo].[usp_UpdateDemRecord]", "Clinical Trail");
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
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "StaffSpecificationFun", "[dbo].[usp_StaffSpecification]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region Get Theme
        public ApiResponse GetThemeOfTask(ThemesOfTask _themesOfTask)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[2];
                _SqlParameter[0] = new SqlParameter("@Action", _themesOfTask.Action);
                _SqlParameter[1] = new SqlParameter("@OrganizationId", _themesOfTask.OrganizationId);
                dt = _DAL.ExecuteCommand("[dbo].[usp_GetTheme]", CommandType.StoredProcedure, _SqlParameter);
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
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "GetThemeOfStudy", "[dbo].[usp_GetTheme]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion
        #region Get Task List
        public ApiResponse GetTaskList(string OrgId)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[1];
                _SqlParameter[0] = new SqlParameter("@OrganizationId", OrgId);
                dt = _DAL.ExecuteCommand("[dbo].[usp_GetStudyList]", CommandType.StoredProcedure, _SqlParameter);
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
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "GEtStudyList", "[dbo].[usp_GetStudyList]", "Clinical Trail");
            }
            return _ApiResponse;
        }
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
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "GetTaskListOfTheme", "[dbo].[usp_StudyASPerTheme]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region AssignTaskRole
        public ApiResponse AssignTaskRole(AssignTaskRole _assignTaskRole)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[5];
                _SqlParameter[0] = new SqlParameter("@Action", _assignTaskRole.Action);
                _SqlParameter[1] = new SqlParameter("@ThemeId", _assignTaskRole.ThemeId);
                _SqlParameter[2] = new SqlParameter("@StudyId", _assignTaskRole.TaskId);
                _SqlParameter[3] = new SqlParameter("@OrganizationId", _assignTaskRole.OrganizationId);
                _SqlParameter[4] = new SqlParameter("@RoleIdRef", _assignTaskRole.RoleId);
                dt = _DAL.ExecuteCommand("[dbo].[usp_AssignTaskAsPerRole]", CommandType.StoredProcedure, _SqlParameter);
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
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "AssignTaskRole", "[dbo].[usp_AssignTaskAsPerRole]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region StudyDetailsCrud
        public ApiResponse StudyDetailsCrud(StudyDetails _studyDetails)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                string StaffList = string.Empty;
                string StudyTaskListId = string.Empty;
                if (_studyDetails.StaffIdList != null)
                    StaffList = new XElement("StaffList", from c in _studyDetails.StaffIdList select new XElement("Staff", new XElement("StaffId", c.ToUpper()))).ToString();

                if (_studyDetails.StudyTaskList != null)
                    StudyTaskListId = new XElement("StudyTaskList", from c in _studyDetails.StudyTaskList select new XElement("StudyTasks", new XElement("StaffId", c.StaffId.ToUpper()), new XElement("StudyTaskId", c.StudyTaskId.ToUpper()))).ToString();


                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[12];
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
                dt = _DAL.ExecuteCommand("[dbo].[usp_StudyDetails]", CommandType.StoredProcedure, _SqlParameter);
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
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "StudyDetailsCrud", "[dbo].[usp_StudyDetails]", "Clinical Trail");
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
                        else if(ds.Tables.Count > 1)
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
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "GetIndividualStudySetails", "[dbo].[usp_GetIndividualStudyDetails]", "Clinical Trail");
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
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "StudyTaskDetailsCrud", "[dbo].[usp_StudyTaskCrud]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region StudyTaskPerUser
        public ApiResponse StudyTaskPerUserAction(StudyTaskPerUser _studytaskperUser)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            string msg = string.Empty;
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlpara = new SqlParameter[8];
                _sqlpara[0] = new SqlParameter("@StaffId", _studytaskperUser.StaffId);
                _sqlpara[1] = new SqlParameter("@StudyId", _studytaskperUser.StudyId);
                _sqlpara[2] = new SqlParameter("@Action", _studytaskperUser.Action);
                _sqlpara[3] = new SqlParameter("@OrganizationId", _studytaskperUser.OrganizationId);
                _sqlpara[4] = new SqlParameter("@CreatedBy", _studytaskperUser.CreatedBy);
                _sqlpara[5] = new SqlParameter("@ActionPerformedBy", _studytaskperUser.ActionPerformedBy);
                _sqlpara[6] = new SqlParameter("@StudyTaskId", _studytaskperUser.StudyTaskId);
                _sqlpara[7] = new SqlParameter("@ReasonArchive", string.IsNullOrEmpty(Convert.ToString(_studytaskperUser.ReasonArchive)) ? string.Empty : Convert.ToString(_studytaskperUser.ReasonArchive));
                dt = _DAL.ExecuteCommand("[dbo].[usp_StudyTaskPerUser]", CommandType.StoredProcedure, _sqlpara);
                if(dt.Rows.Count>0)
                {
                    switch (Convert.ToString(dt.Rows[0]["ActionType"]))
                    {
                        case "ApproveStaffForStudy":
                            msg = " Hi, <br /> " + Convert.ToString(dt.Rows[0]["UserName"]) + "<br /><br /> Welcome to Ascension Q! Your Study tasks are <br/>" + Convert.ToString(dt.Rows[0]["StudyRoles"]) + " <br /><br />For Study :- " + Convert.ToString(dt.Rows[0]["StudyName"]) + "<br/>Authorised by :- " + Convert.ToString(dt.Rows[0]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["EmailId"]), "New Study task Authorised", msg);
                            break;
                        case "AuthorizeNewSingleStudyTask":
                            msg = " Hi, <br /> " + Convert.ToString(dt.Rows[0]["UserName"]) + "<br /><br /> Welcome to Ascension Q! Your Study task <br/>" + Convert.ToString(dt.Rows[0]["StudyRoles"]) + " <br /><br />For Study :- " + Convert.ToString(dt.Rows[0]["StudyName"]) + "<br/>Authorised by :- " + Convert.ToString(dt.Rows[0]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["EmailId"]), "New Study task Authorised", msg);
                            break;
                        case "ApproveRejectedStudyOrStudyTask":
                            msg = " Hi, <br /> " + Convert.ToString(dt.Rows[0]["UserName"]) + "<br /><br /> Welcome to Ascension Q! Your Study tasks are <br/>" + Convert.ToString(dt.Rows[0]["StudyRoles"]) + " <br /><br />For Study :- " + Convert.ToString(dt.Rows[0]["StudyName"]) + "<br/>Removed by :- " + Convert.ToString(dt.Rows[0]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["EmailId"]), "In Active Study tasks", msg);
                            break;
                        case "ApproveRejectedSingleStudyTask":
                            msg = " Hi, <br /> " + Convert.ToString(dt.Rows[0]["UserName"]) + "<br /><br /> Welcome to Ascension Q! Your Study task <br/>" + Convert.ToString(dt.Rows[0]["StudyRoles"]) + " <br /><br />For Study :- " + Convert.ToString(dt.Rows[0]["StudyName"]) + "<br/>Removed by :- " + Convert.ToString(dt.Rows[0]["AuthorizedBy"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["EmailId"]), "In Active Study tasks", msg);
                            break;
                    }
                    
                    _ApiResponse.success = 1;
                    _ApiResponse.data = dt;
                    _ApiResponse.message = Convert.ToString(dt.Rows[0]["MSG"]);
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message= "No List Found";
                }
            }
            catch(Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "StudyTaskPerUser", "[dbo].[usp_StudyTaskPerUser]", "Clinical Trail");
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
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "UnitRoleDetailsCrud", "[dbo].[usp_UnitRoleCrud]", "Clinical Trail");
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
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "GetUniqueCode", "[dbo].[usp_UnitRoleCrud]", "Clinical Trail");
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
                _sqlPara[0] = new SqlParameter("@Action",_stafReg.Action);
                _sqlPara[1] = new SqlParameter("@OrganizationId",_stafReg.OrganizationId);
                dt = _DAL.ExecuteCommand("[dbo].[usp_GetModifiedUserList]", CommandType.StoredProcedure, _sqlPara);
                if(dt.Rows.Count>0)
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
            catch(Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "GetUpdatedUserList", "[dbo].[usp_GetModifiedUserList]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region GetRequestedUsersDetails
        public ApiResponse GetRequestedUsersDetails(StaffRegistration _stafReg)
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
                dt = _DAL.ExecuteCommand("[dbo].[usp_RequiredUserDetails]", CommandType.StoredProcedure, _sqlPara);
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
            catch(Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "GetRequestedUsersDetails", "[dbo].[usp_RequiredUserDetails]", "Clinical Trail");
            }
            return _ApiResponse;
        }
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
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "GetRequestedUserDetails", "[dbo].[usp_RequiredStudyDetails]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region Update Study Task List
        public ApiResponse UpdateStudyTaskList(StaffRegistration _staffDetails)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                string StudyTaskList = string.Empty;
                string StudyList = string.Empty;
                string UnitRoles = string.Empty;


                if (_staffDetails.StudyRoleList != null)
                    StudyTaskList = new XElement("SelectedStudyTasks", from c in _staffDetails.StudyRoleList
                                                                       select new XElement("StudyTaskList", new XElement("Study", c.Study.ToUpper()), new XElement("StudyTask", c.StudyTask.ToUpper()))).ToString();

                if (_staffDetails.Studies != null)
                    StudyList = new XElement("SelectedStudies", from c in _staffDetails.Studies
                                                                select new XElement("StudiesList", new XElement("Studies", c.ToUpper()))).ToString();

                if (_staffDetails.UnitRoleList != null)
                    UnitRoles = new XElement("SelectedUnitRole", from c in _staffDetails.UnitRoleList
                                                                 select new XElement("UnitRoleList", new XElement("UnitRole", c.ToUpper()))).ToString();

                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[8];
                _sqlPara[0] = new SqlParameter("@StaffId", _staffDetails.StaffId);
                _sqlPara[1] = new SqlParameter("@Action", _staffDetails.Action);
                _sqlPara[2] = new SqlParameter("@OrganizationId", _staffDetails.OrganizationId);
                _sqlPara[3] = new SqlParameter("@CreatedBy", _staffDetails.CreatedById);
                _sqlPara[4] = new SqlParameter("@xmlStudyTaskList", StudyTaskList);
                _sqlPara[5] = new SqlParameter("@xmlStudies", StudyList);
                _sqlPara[6] = new SqlParameter("@xmlUnitRoleList", UnitRoles);
                _sqlPara[7] = new SqlParameter("@ActionDoneBy", _staffDetails.ActionDoneBy);
                dt = _DAL.ExecuteCommand("[dbo].[usp_StudyOrUnitRoleModification]", CommandType.StoredProcedure, _sqlPara);
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
            catch(Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "UpdateStudyTaskList", "[dbo].[usp_StudyOrUnitRoleModification]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region StudyTraining
        #region get study training
        internal ApiResponse GetStudyTraining(StaffInfoRef _StaffInfoRef)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[2];
                _sqlPara[0] = new SqlParameter("@StaffIdRef", _StaffInfoRef.StaffId);
                _sqlPara[1] = new SqlParameter("@OrganizationId", _StaffInfoRef.OrganizationId);
                dt = _DAL.ExecuteCommand("[dbo].[usp_GetStudyTraining]", CommandType.StoredProcedure, _sqlPara);
                _ApiResponse.success = 1;
                _ApiResponse.message = "";
                _ApiResponse.data = dt;
            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "GetStudyTraining", "[dbo].[usp_GetStudyTraining]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region get study training
        internal ApiResponse UpdateStudyTraining(StaffStudyTrainingReq _StaffInfoRef)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[4];
                _sqlPara[0] = new SqlParameter("@StudyIdRef", _StaffInfoRef.StudyId);
                _sqlPara[1] = new SqlParameter("@StaffIdRef", _StaffInfoRef.StaffId);
                _sqlPara[2] = new SqlParameter("@OrganizationIdRef", _StaffInfoRef.OrganizationId);
                _sqlPara[3] = new SqlParameter("@StudyStatus", _StaffInfoRef.StudyStatus);
                //_sqlPara[4] = new SqlParameter("@AssignStudyToUserId", _StaffInfoRef.AssignStudyToUserId);
                dt = _DAL.ExecuteCommand("[dbo].[usp_EditAsinineStudyTraining]", CommandType.StoredProcedure, _sqlPara);
                if (dt.Rows.Count > 0)
                {
                    _ApiResponse.success = 1;
                    _ApiResponse.message = Convert.ToString(dt.Rows[0]["Response"]);
                    _ApiResponse.data = dt;
                }
            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "SaveStudyTraining", "[dbo].[usp_AsinineStudyTraining]", "Clinical Trail");
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
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "GetRejection", "[dbo].[usp_GetRejection]", "Clinical Trail");
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
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "LM Or Study Name";
                    _ApiResponse.data = ds;
                }

            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "CheckLMOrPIAuthFun", "[dbo].[usp_CheckLMOrPIAuth]", "Clinical Trail");
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
                SqlParameter[] _sqlPara = new SqlParameter[5];
                string xmlStaff = string.Empty;
                if (obj.Action == "Update")
                    xmlStaff = new XElement("ParentNode", from c in obj.LinkedEmployeeId
                                                          select new XElement("StaffList", new XElement("StaffId", c.ToUpper()))).ToString();

                _sqlPara[0] = new SqlParameter("@LineManagerId", obj.LineManagerId);
                _sqlPara[1] = new SqlParameter("@OrganizationId", obj.OrganizationId);
                _sqlPara[2] = new SqlParameter("@Action", obj.Action);
                _sqlPara[3] = new SqlParameter("@NewLineManagerId", obj.NewLineManagerId);
                _sqlPara[4] = new SqlParameter("@EmpList", xmlStaff);

                dt = _DAL.ExecuteCommand("[dbo].[usp_ChangeEmpWithLineManager]", CommandType.StoredProcedure, _sqlPara);
                if (dt.Rows.Count > 0)
                {

                    _ApiResponse.success = 1;
                    if (obj.Action == "Get")
                        _ApiResponse.message = "Linked Employee List";
                    else if (obj.Action == "Update")
                        _ApiResponse.message = "Record has been updated successfully";
                    _ApiResponse.data = dt;
                }

            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "ChangeLineManagerFun", "[dbo].[usp_ChangeEmpWithLineManager]", "Clinical Trail");
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
                SqlParameter[] _sqlPara = new SqlParameter[5];
                string xmlStudies = string.Empty;
                if (obj.Action == "Update")
                    xmlStudies = new XElement("ParentNode", from c in obj.LinkedStudyId
                                                            select new XElement("StudyList", new XElement("StudyId", c.ToUpper()))).ToString();

                _sqlPara[0] = new SqlParameter("@StaffIdPI", obj.PiId);
                _sqlPara[1] = new SqlParameter("@OrganizationId", obj.OrganizationId);
                _sqlPara[2] = new SqlParameter("@Action", obj.Action);
                _sqlPara[3] = new SqlParameter("@NewPiId", obj.NewPiId);
                _sqlPara[4] = new SqlParameter("@StudyList", xmlStudies);

                dt = _DAL.ExecuteCommand("[dbo].[usp_ChangePIOfStudies]", CommandType.StoredProcedure, _sqlPara);
                if (dt.Rows.Count > 0)
                {

                    _ApiResponse.success = 1;
                    if (obj.Action == "Get")
                        _ApiResponse.message = "Linked Studies List";
                    else if (obj.Action == "Update")
                        _ApiResponse.message = "Record has been updated successfully";
                    _ApiResponse.data = dt;
                }

            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "PrincipalInvestigatorFun", "[dbo].[usp_ChangePIOfStudies]", "Clinical Trail");
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
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[7];
                _sqlPara[0] = new SqlParameter("@StaffId", obj.StaffId);
                _sqlPara[1] = new SqlParameter("@Action", obj.Action);
                _sqlPara[2] = new SqlParameter("@Approval", obj.ApprovalStatus);
                _sqlPara[3] = new SqlParameter("@OrganizationId", obj.OrganizationId);
                _sqlPara[4] = new SqlParameter("@AssignUnitRolePerUserId", obj.AssignUnitRolePerUserId);
                _sqlPara[5] = new SqlParameter("@ActionStatus", obj.ActionStatus);
                _sqlPara[6] = new SqlParameter("@ReasonArchive", obj.ReasonArchive);

                dt = _DAL.ExecuteCommand("[dbo].[usp_UnitRoleApproval]", CommandType.StoredProcedure, _sqlPara);
                if (dt.Rows.Count > 0)
                {

                    _ApiResponse.success = 1;
                    if (obj.Action == "Get")
                        _ApiResponse.message = "Pending Aprovel List";
                    else if (obj.Action == "Update")
                        _ApiResponse.message = "Record has been updated successfully";
                    _ApiResponse.data = dt;
                }

            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "StaffRoleApproval", "[dbo].[usp_UnitRoleApproval]", "Clinical Trail");
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
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[7];
                _sqlPara[0] = new SqlParameter("@StaffId", obj.StaffId);
                _sqlPara[1] = new SqlParameter("@Action", obj.Action);
                _sqlPara[2] = new SqlParameter("@Approval", obj.ApprovalStatus);
                _sqlPara[3] = new SqlParameter("@OrganizationId", obj.OrganizationId);
                _sqlPara[4] = new SqlParameter("@AssignUnitRolePerUserId", obj.AssignStudyRoleId);
                _sqlPara[5] = new SqlParameter("@ActionStatus", obj.ActionStatus);
                _sqlPara[6] = new SqlParameter("@ReasonArchive", obj.ReasonArchive);

                dt = _DAL.ExecuteCommand("[dbo].[usp_StaffStudyRoleApproval]", CommandType.StoredProcedure, _sqlPara);
                if (dt.Rows.Count > 0)
                {

                    _ApiResponse.success = 1;
                    if (obj.Action == "Get")
                        _ApiResponse.message = "Pending Aprovel List";
                    else if (obj.Action == "Update")
                        _ApiResponse.message = "Record has been updated successfully";
                    _ApiResponse.data = dt;
                }

            }
            catch (Exception ex)
            {
                _ApiResponse.success = 0;
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.InnerException), "DemRecord", "StaffRoleApproval", "[dbo].[usp_StaffStudyRoleApproval]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

    }

}