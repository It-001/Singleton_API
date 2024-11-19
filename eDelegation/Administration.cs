using ClinicalTrialAPI.Custom;
using ClinicalTrialAPI.Models;
using DBAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace ClinicalTrialAPI.eDelegation
{
    public class Administration
    {
        #region Singleton Object
        private Administration() { }
        private static readonly Lazy<Administration> lazy = new Lazy<Administration>(() => new Administration());
        public static Administration AdminInstance { get { return lazy.Value; } }
        #endregion

        ApiResponse _ApiResponse;
        private DAL _DAL;

        public ApiResponse PasswordCheck(StaffLogin _StaffLogin)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[8];
                _SqlParameter[0] = new SqlParameter("@EmailId", _StaffLogin.EmailId.ToString());
                _SqlParameter[1] = new SqlParameter("@Pwd", _StaffLogin.Password.ToString());
                _SqlParameter[2] = new SqlParameter("@AccessCodeIndex0", _StaffLogin.AccessCodeIndex0);
                _SqlParameter[3] = new SqlParameter("@AccessCodeIndex1", _StaffLogin.AccessCodeIndex1);
                _SqlParameter[4] = new SqlParameter("@AccessCodeIndex2", _StaffLogin.AccessCodeIndex2);
                _SqlParameter[5] = new SqlParameter("@AccessCodeValue0", _StaffLogin.AccessCodeValue0);
                _SqlParameter[6] = new SqlParameter("@AccessCodeValue1", _StaffLogin.AccessCodeValue1);
                _SqlParameter[7] = new SqlParameter("@AccessCodeValue2", _StaffLogin.AccessCodeValue2);
                dt = _DAL.ExecuteCommand("[dbo].[usp_PasswordAuth1]", CommandType.StoredProcedure, _SqlParameter);
                if (dt.Rows[0]["Success"].ToString() == "1")
                {
                    _ApiResponse.success = 1;
                    _ApiResponse.data = dt;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Invalid Request";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_StaffLogin),
                       "Administration", "PasswordCheck", "[dbo].[usp_PasswordAuth1]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        #region unit summary
        public ApiResponse GetUnitSummary(OtherItemDD _OtherItemDD)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[3];
                _SqlParameter[0] = new SqlParameter("@OrganizationId", _OtherItemDD.OrganizationId.ToString());
                _SqlParameter[1] = new SqlParameter("@StudyIdRef", _OtherItemDD.StudyIdRef.ToString());
                _SqlParameter[2] = new SqlParameter("@ActionType", _OtherItemDD.ActionType.ToString());
                dt = _DAL.ExecuteCommand("[dbo].[Usp_UnitSummaryDetails]", CommandType.StoredProcedure, _SqlParameter);
                if (_OtherItemDD.ActionType.ToString() == "StudyProgress")
                {
                    _ApiResponse.success = 1;
                    _ApiResponse.data = dt;
                }
                else
                {
                    if (dt.Rows[0]["Success"].ToString() == "1")
                    {
                        _ApiResponse.success = 1;
                        _ApiResponse.data = dt;
                    }
                    else
                    {
                        _ApiResponse.success = 0;
                        _ApiResponse.message = "Invalid Request";
                    }
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_OtherItemDD),
                         "Administration", "GetUnitSummary", "[dbo].[Usp_UnitSummaryDetails]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion
    }
}