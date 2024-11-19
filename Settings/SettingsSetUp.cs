using ClinicalTrialAPI.Custom;
using ClinicalTrialAPI.eRecruitment;
using ClinicalTrialAPI.Models;
using DBAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Xml.Linq;

namespace ClinicalTrialAPI.Settings
{
    public sealed class SettingsSetUp
    {
        ApiResponse _ApiResponse;
        private DAL _DAL;
        #region Singleton
        private SettingsSetUp() { }
        private static readonly Lazy<SettingsSetUp> lazy = new Lazy<SettingsSetUp>(() => new SettingsSetUp());
        public static SettingsSetUp Instance
        {
            get { return lazy.Value; }
        }
        #endregion

        #region set Visit Planner
        public ApiResponse StudyActivity(StudyActivity _studyActivity)
        {
            string XmlString = null;

            if (_studyActivity.UserList != null)
                XmlString = new XElement("ArrayOfUsers", from c in _studyActivity.UserList select new XElement("Users", new XElement("UserId", c.UserId))).ToString();//change by arfin ali 05/05/2021

            // XmlString = GenerateXML.Instance.ToXmlString(_studyActivity.UserList);
            string xmlLocationList = "";
            if (_studyActivity.StudyLocationList != null && _studyActivity.StudyLocationList.Any())
            {
                xmlLocationList = new XElement("SelectedStudyLocations", from c in _studyActivity.StudyLocationList
                                                                         select new XElement("LocationList", new XElement("StudyLocationId", c.StudyLocationId == null ? "" : c.StudyLocationId.ToUpper()),
                                                                         new XElement("LocationId", c.LocationIdRef == null ? "" : c.LocationIdRef.ToUpper())
                                                                         , new XElement("BuildingId", c.BuildingIdRef == null ? "" : c.BuildingIdRef == null ? "" : c.BuildingIdRef.ToUpper())
                                                                         , new XElement("FloorId", c.FloorIdRef == null ? "" : c.FloorIdRef.ToUpper())
                                                                         , new XElement("RoomId", c.RoomIdRef == null ? "" : c.RoomIdRef.ToUpper())
                                                                         )).ToString();
            }

            _ApiResponse = new ApiResponse();
            GetSqlParameterFromClassProperty ClsToPara = new GetSqlParameterFromClassProperty();
            //DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                //SqlParameter[] _SqlParameter = ClsToPara.GenerateParameterfromProperty(_studyActivity);
                SqlParameter[] _SqlParameter = new SqlParameter[20];

                _SqlParameter[0] = new SqlParameter("@Action", _studyActivity.Action);
                _SqlParameter[1] = new SqlParameter("@Study", _studyActivity.Study);
                _SqlParameter[2] = new SqlParameter("@StudyId", _studyActivity.StudyId);
                _SqlParameter[3] = new SqlParameter("@OrganizationId", _studyActivity.OrganizationId);
                _SqlParameter[4] = new SqlParameter("@CreatedBy", _studyActivity.CreatedBy);
                _SqlParameter[5] = new SqlParameter("@StudyNum", _studyActivity.StudyNum);
                _SqlParameter[6] = new SqlParameter("@PI", _studyActivity.PI);
                _SqlParameter[7] = new SqlParameter("@EudractNumber", _studyActivity.EudractNumber);
                _SqlParameter[8] = new SqlParameter("@RandNumber", _studyActivity.Randnumber);
                _SqlParameter[9] = new SqlParameter("@SiteID", _studyActivity.SiteID);
                _SqlParameter[10] = new SqlParameter("@Department", _studyActivity.Department);
                _SqlParameter[11] = new SqlParameter("@StudyColor", _studyActivity.StudyColor);
                _SqlParameter[12] = new SqlParameter("@Status", _studyActivity.Status);
                _SqlParameter[13] = new SqlParameter("@UserList", XmlString);
                _SqlParameter[14] = new SqlParameter("@StudyGroupId", _studyActivity.StudyGroupId);
                _SqlParameter[15] = new SqlParameter("@xmStudyLocations", xmlLocationList);
                _SqlParameter[16] = new SqlParameter("@VisitIdRef", _studyActivity.VisitIdRef);
                _SqlParameter[17] = new SqlParameter("@StudyLocationId", _studyActivity.StudyLocationId);
                _SqlParameter[18] = new SqlParameter("@LocationIdRef", _studyActivity.LocationIdRef);
                _SqlParameter[19] = new SqlParameter("@ProjectId", _studyActivity.ProjectId);

                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_StudyActivity]", CommandType.StoredProcedure, _SqlParameter);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].TableName = "SuccessStory";
                    if (_studyActivity.Action == "Update")
                    {
                        if (ds.Tables.Count > 1)
                        {
                            string msgOldUser = "You have been removed as a Principle investigator by " + Convert.ToString(ds.Tables[1].Rows[0]["RemovedOrAssignedBy"]) + "  for Study " + ds.Tables[1].Rows[0]["StudyName"] + ".<br /><br /> Thank you,<br /> AscensionQ Team" + " <br /> ";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[1].Rows[0]["OldPIEmail"]), "Removed PI", msgOldUser, "Yes");

                            string msgNewUser = "You have been assigned as a Principle investigator by " + Convert.ToString(ds.Tables[1].Rows[0]["RemovedOrAssignedBy"]) + "  for Study " + ds.Tables[1].Rows[0]["StudyName"] + ".<br /><br /> Thank you,<br /> AscensionQ Team" + " <br /> ";
                            SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[1].Rows[0]["NewPIEmail"]), "Assigned PI", msgNewUser, "Yes");
                        }
                    }
                    else if (_studyActivity.Action == "GetStudyInfo")
                    {
                        ds.Tables[1].TableName = "StudyInfo";
                        ds.Tables[2].TableName = "StudyUsers";
                        ds.Tables[3].TableName = "StudyLocations";
                    }
                    else if (_studyActivity.Action == "GetStudyUsers")
                    {
                        ds.Tables[0].TableName = "StudyUsers";

                    }

                    else if (_studyActivity.Action == "UpdateStdystatus")
                    {
                        ds.Tables[0].TableName = "StudyUsers";

                    }
                    else if (_studyActivity.Action == "GetGroups")
                    {
                        ds.Tables[0].TableName = "StudyGroups";
                    }
                    else if (_studyActivity.Action == "GetStudyLocationsByVisitId")
                    {
                        ds.Tables[1].TableName = "VisitLocations";
                    }
                    else if (_studyActivity.Action == "GetStudyLocationsInfoById")
                    {
                        ds.Tables[0].TableName = "StudyLocationsById";
                    }


                    if (Convert.ToString(ds.Tables[0].Rows[0]["Success"]) == "1")
                    {
                        _ApiResponse.success = 1;
                        _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                        _ApiResponse.data = ds;
                    }
                    else
                    {
                        _ApiResponse.success = 0;
                        _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                        _ApiResponse.data = ds;
                    }

                    if (ds.Tables.Count > 1 && (_studyActivity.Action == "Update" || _studyActivity.Action == "Insert"))
                    {
                        if (_studyActivity.UserList != null && _studyActivity.UserList.Count > 0)
                        {
                            for (int i = 0; i < _studyActivity.UserList.Count; i++)
                            {
                                if (_studyActivity.UserList[i].IsAssigned == null)
                                {
                                    string message = "Dear " + _studyActivity.UserList[i].UserName + ",<br /><br />";
                                    message += "" + _studyActivity.LoggedInUserName + " (" + Convert.ToString(ds.Tables[0].Rows[0]["orgName"]) + ") has assigned " + _studyActivity.Study + " to you.<br />Please connect to the site organisation in case of any queries.<br /><br />Thank you.";
                                    SendMail_SMS.Instance.sendExceptionMail(_studyActivity.UserList[i].EmailId, "Study delegation details", message, "Yes");
                                }
                            }
                        }
                        if (_studyActivity.RemovedUserList != null && _studyActivity.RemovedUserList.Count > 0)
                        {
                            for (int i = 0; i < _studyActivity.RemovedUserList.Count; i++)
                            {
                                if (_studyActivity.RemovedUserList[i].IsAssigned == "assigned")
                                {
                                    string message = "Dear " + _studyActivity.RemovedUserList[i].UserName + ",<br /><br />";
                                    message += "" + _studyActivity.LoggedInUserName + " (" + Convert.ToString(ds.Tables[0].Rows[0]["orgName"]) + ") has removed " + _studyActivity.Study + " from you.<br />Please connect to the site organisation in case of any queries.<br /><br />Thank you.";
                                    SendMail_SMS.Instance.sendExceptionMail(_studyActivity.RemovedUserList[i].EmailId, "Study delegation details", message, "Yes");
                                }
                            }
                        }
                    }
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";

                }

            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_studyActivity),
               "SettingsSetup", "StudyActivity", "[dbo].[StudyActivity]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        public ApiResponse SaveECommSetup(ECommunicationSetup _eCommunicationSetup)
        {
            string XmlString = null;
            if (_eCommunicationSetup.TemplateList != null)
                XmlString = GenerateXML.Instance.ToXmlString(_eCommunicationSetup.TemplateList);
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[8];
                _SqlParameter[0] = new SqlParameter("@StudyId", Convert.ToString(_eCommunicationSetup.StudyId));
                _SqlParameter[1] = new SqlParameter("@VisitId", Convert.ToString(_eCommunicationSetup.VisitId));
                _SqlParameter[2] = new SqlParameter("@OrganizationId", Convert.ToString(_eCommunicationSetup.OrganizationId));
                _SqlParameter[3] = new SqlParameter("@CreatedBy", Convert.ToString(_eCommunicationSetup.CreatedBy));
                _SqlParameter[4] = new SqlParameter("@ActionType", _eCommunicationSetup.ActionType);
                _SqlParameter[5] = new SqlParameter("@TemplateList", XmlString);
                _SqlParameter[6] = new SqlParameter("@Payment", _eCommunicationSetup.Payment);
                _SqlParameter[7] = new SqlParameter("@GroupId", _eCommunicationSetup.GroupId);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[EComm_Usp_ECommunicationSetup]", CommandType.StoredProcedure, _SqlParameter);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].TableName = "SuccessStory";
                    if (Convert.ToString(ds.Tables[0].Rows[0]["Success"]) == "1")
                    {
                        if (_eCommunicationSetup.ActionType == "GetTemplate" || _eCommunicationSetup.ActionType == "GetAdhocTemplate")
                        {
                            ds.Tables[1].TableName = "TemplateList";
                            if (ds.Tables.Count > 2)
                            {
                                ds.Tables[2].TableName = "PaymentDetails";
                                ds.Tables[3].TableName = "CurrencySymbol";
                                ds.Tables[4].TableName = "DepartmentRoleList";
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
                        _ApiResponse.data = ds;
                    }
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                }

                _ApiResponse.success = 0;
                _ApiResponse.message = "Try Again";

            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_eCommunicationSetup),
               "SettingsSetup", "SaveECommSetup", "[dbo].[EComm_Usp_ECommunicationSetup]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        //OrganizationSetUpActivity

        public ApiResponse SaveStudyStats(StudyActivity _Studystatusupdated)
        {


            _ApiResponse = new ApiResponse();
            GetSqlParameterFromClassProperty ClsToPara = new GetSqlParameterFromClassProperty();
            //DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                //SqlParameter[] _SqlParameter = ClsToPara.GenerateParameterfromProperty(_studyActivity);
                SqlParameter[] _SqlParameter = new SqlParameter[7];

                _SqlParameter[0] = new SqlParameter("@Action", _Studystatusupdated.Action);
                _SqlParameter[1] = new SqlParameter("@StudyId", _Studystatusupdated.StudyId);
                _SqlParameter[2] = new SqlParameter("@OrganizationId", _Studystatusupdated.OrganizationId);
                _SqlParameter[3] = new SqlParameter("@StudyStatus", _Studystatusupdated.Status);
                _SqlParameter[4] = new SqlParameter("@UndoArchiveOrInActiveBy", _Studystatusupdated.UndoArchiveOrInActiveBy);
                _SqlParameter[5] = new SqlParameter("@PI", _Studystatusupdated.PI);
                _SqlParameter[6] = new SqlParameter("@CreatedBy", _Studystatusupdated.CreatedBy);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_StudyActivity]", CommandType.StoredProcedure, _SqlParameter);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].TableName = "SuccessStory";
                    if (_Studystatusupdated.Action == "UpdateStdystatus")
                    {
                        if (ds.Tables.Count > 1)
                        {
                            string msgOldUser = "Status updated";


                        }
                    }


                    if (Convert.ToString(ds.Tables[0].Rows[0]["Success"]) == "1")
                    {
                        _ApiResponse.success = 1;
                        _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                        _ApiResponse.data = ds;
                    }
                    else
                    {
                        _ApiResponse.success = 0;
                        _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                        _ApiResponse.data = ds;
                    }
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                }

            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_Studystatusupdated),
                "SettingsSetup", "SaveStudyStats", "[dbo].[usp_StudyActivity]", "Clinical Trail");
            }
            return _ApiResponse;
        }


        #region Organization SetUp
        public ApiResponse OrganizationSetUpActivity(OrganizationSetup _organizationSetup)
        {
            _ApiResponse = new ApiResponse();

            string xmlLocationList = null;
            if (_organizationSetup.LocationList != null)
                xmlLocationList = new XElement("SelectedLocations", from c in _organizationSetup.LocationList select new XElement("LocationList", new XElement("LocationId", c.LocationId), new XElement("LocationName", c.LocationName))).ToString();

            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[13];
                _SqlParameter[0] = new SqlParameter("@AppointmentStartTime", _organizationSetup.AppointmentStartTime);
                _SqlParameter[1] = new SqlParameter("@AppointmentEndTime", _organizationSetup.AppointmentEndTime);
                _SqlParameter[2] = new SqlParameter("@AppointmentSlotDuration", _organizationSetup.AppointmentSlotDuration);
                _SqlParameter[3] = new SqlParameter("@LabelDefaultTime", _organizationSetup.LabelDefaultTime);
                _SqlParameter[4] = new SqlParameter("@NoofBookingPerSlot", _organizationSetup.NoofBookingPerSlot);
                _SqlParameter[5] = new SqlParameter("@IsDefaultSlot", _organizationSetup.IsDefaultSlot);
                _SqlParameter[6] = new SqlParameter("@CreatedBy", _organizationSetup.CreatedBy);
                _SqlParameter[7] = new SqlParameter("@OrganizationId", _organizationSetup.OrganizationId);
                _SqlParameter[8] = new SqlParameter("@CurrencySymbol", _organizationSetup.CurrencySymbol);
                _SqlParameter[9] = new SqlParameter("@ActionType", _organizationSetup.ActionType);
                //_SqlParameter[10] = new SqlParameter("@DefaultStudyLocation", _organizationSetup.DefaultStudyLocation);
                _SqlParameter[10] = new SqlParameter("@LocationList", xmlLocationList);
                _SqlParameter[11] = new SqlParameter("@OrgName", _organizationSetup.OrgName);
                _SqlParameter[12] = new SqlParameter("@OrgCountry", _organizationSetup.OrgCountry);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[Usp_OrganizationSetup]", CommandType.StoredProcedure, _SqlParameter);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].TableName = "SuccessStory";
                    if (_organizationSetup.ActionType == "GetSetup")
                    {
                        ds.Tables[1].TableName = "OrgLocations";
                    }
                    if (_organizationSetup.ActionType == "Getorganization")
                    {
                        ds.Tables[1].TableName = "Modules";
                    }
                    _ApiResponse.success = 1;
                    _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_organizationSetup),
                   "SettingsSetup", "OrganizationSetUpActivity", "[dbo].[Usp_OrganizationSetup]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region analaytics
        public ApiResponse AnalyticsActivity(Analytics _analytics)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                string StudyList = string.Empty;
                if (_analytics.StudyIds != null)
                    StudyList = new XElement("SelectedStudys", from c in _analytics.StudyIds
                                                               select new XElement("StudyList", new XElement("StudyId", c.ToUpper()))).ToString();

                string VisitList = string.Empty;
                if (_analytics.VisitIds != null)
                    VisitList = new XElement("SelectedVisits", from c in _analytics.VisitIds
                                                               select new XElement("VisitList", new XElement("VisitId", c.ToUpper()))).ToString();

                string FeasibilityList = string.Empty;
                if (_analytics.FeasibilityIds != null)
                    FeasibilityList = new XElement("SelectedParticipants", from c in _analytics.FeasibilityIds
                                                                           select new XElement("ParticipantList", new XElement("ParticipantId", c.ToUpper()))).ToString();

                string GroupList = string.Empty;
                if (_analytics.GroupIds != null)
                    GroupList = new XElement("SelectedGroups", from c in _analytics.GroupIds
                                                               select new XElement("GroupList", new XElement("GroupId", c.ToUpper()))).ToString();

                SqlParameter[] _sqlPara = new SqlParameter[12];
                _sqlPara[0] = new SqlParameter("@xmlStudyIds", StudyList);
                _sqlPara[1] = new SqlParameter("@AppointmentPeriod", _analytics.Period);
                _sqlPara[2] = new SqlParameter("@StartPeriod", _analytics.StartDate);
                _sqlPara[3] = new SqlParameter("@EndPeriod", _analytics.EndDate);
                _sqlPara[4] = new SqlParameter("@OrganizationId", _analytics.OrganizationId);
                _sqlPara[5] = new SqlParameter("@ActionType", _analytics.ActionType);
                _sqlPara[6] = new SqlParameter("@xmlVisitIds", VisitList);
                _sqlPara[7] = new SqlParameter("@xmlParticipantsIds", FeasibilityList);
                _sqlPara[8] = new SqlParameter("@AdhocVisit", _analytics.AdhocVisit);
                _sqlPara[9] = new SqlParameter("@VisitNo", _analytics.VisitNo);
                _sqlPara[10] = new SqlParameter("@XMLGroupIds", GroupList);
                _sqlPara[11] = new SqlParameter("@AppointmentStatus", _analytics.AppStatus);

                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[Usp_GetAnalytics]", CommandType.StoredProcedure, _sqlPara);

                if (ds.Tables.Count > 0)
                {
                    if (_analytics.ActionType == "AppointmentAnalyticsLabTechnicianStudyWise")
                    {
                        ds.Tables[1].TableName = "VisitWiseData";
                        ds.Tables[2].TableName = "Groups";
                        ds.Tables[3].TableName = "Visits";
                    }
                    ds.Tables[0].TableName = "SuccessStory";
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "";
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "";
                    _ApiResponse.data = null;
                }

            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_analytics),
                    "SettingsSetup", "AnalyticsActivity", "[dbo].[Usp_GetAnalytics]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion



        #region analaytics
        public ApiResponse SetupScheduler(ScheduledReminder _schRe)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);

                SqlParameter[] _SqlParameter = new SqlParameter[8];
                _SqlParameter[0] = new SqlParameter("@OrganizationId", _schRe.OrganizationId);
                _SqlParameter[1] = new SqlParameter("@ActionType", _schRe.ActionType);
                _SqlParameter[2] = new SqlParameter("@CreatedBy", _schRe.CreatedBy);
                _SqlParameter[3] = new SqlParameter("@Hour", _schRe.Hour);
                _SqlParameter[4] = new SqlParameter("@Min", _schRe.Min);
                _SqlParameter[5] = new SqlParameter("@SchedulerId", _schRe.Id);
                _SqlParameter[6] = new SqlParameter("@ReminderBeforeStartNumber", _schRe.ReminderBeforeStartNumber);
                _SqlParameter[7] = new SqlParameter("@ReminderBeforeStartSuffix", _schRe.ReminderBeforeStartSuffix);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_GetScheduledTime]", CommandType.StoredProcedure, _SqlParameter);

                var SchStatus = Reminder.Scheduler.Instance.ScheduleRunningStatus(_schRe.OrganizationId);
                string str = SchStatus.Result;

                DataTable dt = new DataTable();
                dt.Columns.Add("SchStatus");
                dt.Rows.Add(str);
                ds.Tables.Add(dt);
                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "SuccessStory";
                    ds.Tables[1].TableName = "ScheduleStatus";
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "";
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "";
                    _ApiResponse.data = ds.Tables[0];
                }

            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_schRe),
                    "SettingsSetup", "SetupScheduler", "[dbo].[usp_GetScheduledTime]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        public ApiResponse ReminderCheckList()
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);

                SqlParameter[] _SqlParameter = new SqlParameter[0];
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_ReminderCheck]", CommandType.StoredProcedure, _SqlParameter);
                DataTable dt = new DataTable();
                dt.Columns.Add("OrgName");
                dt.Columns.Add("OrganizationId");
                dt.Columns.Add("IsReminderActive");
                dt.Columns.Add("ReminderStatus");
                dt.Columns.Add("ScheduledTimeId");
                DataRow dr;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    dr = dt.NewRow();
                    var SchStatus = Reminder.Scheduler.Instance.ScheduleRunningStatus(Convert.ToString(ds.Tables[0].Rows[i]["OrganizationId"]));
                    dr[0] = Convert.ToString(ds.Tables[0].Rows[i]["OrgName"]);
                    dr[1] = Convert.ToString(ds.Tables[0].Rows[i]["OrganizationId"]);
                    dr[2] = Convert.ToString(ds.Tables[0].Rows[i]["IsReminderActive"]);

                    string str = SchStatus.Result;
                    dr[3] = Convert.ToString(str);
                    dr[4] = Convert.ToString(ds.Tables[0].Rows[i]["ScheduledTimeId"]);
                    dt.Rows.Add(dr);
                }

                dt.TableName = "ReminderStatusList";
                _ApiResponse.success = 1;
                _ApiResponse.message = "";
                _ApiResponse.data = dt;

            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: ",
                   "SettingsSetup", "ReminderCheckList", "[dbo].[usp_ReminderCheck]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        public ApiResponse StudyCROMappingActivity(StudyCROMapping _studyCROMapping)
        {
            string XmlString = null;
            string Xmlstudy = string.Empty;
            string XmlModule = string.Empty;

            if (_studyCROMapping.CROList != null)
                XmlString = new XElement("SelectedCROs", from c in _studyCROMapping.CROList
                                                         select new XElement("CROList", new XElement("CROId", c.CROId.ToUpper()))).ToString();

            if (_studyCROMapping.StudyList != null)
                Xmlstudy = new XElement("SelectedStudies", from c in _studyCROMapping.StudyList
                                                           select new XElement("StudyList", new XElement("StudyId", c.StudyId.ToUpper()),
                                                           new XElement("UnitId", c.UnitId.ToUpper()))).ToString();

            if (_studyCROMapping.ModuleList != null)
                XmlModule = new XElement("SelectedModules", from c in _studyCROMapping.ModuleList
                                                            select new XElement("ModuleList", new XElement("ModuleId", c.ModuleId.ToUpper()),
                                                            new XElement("ModuleName", c.ModuleName.ToUpper()))).ToString();

            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                if (_studyCROMapping.FromDate == "")
                    _studyCROMapping.FromDate = null;

                if (_studyCROMapping.ToDate == "")
                    _studyCROMapping.ToDate = null;

                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[11];
                _SqlParameter[0] = new SqlParameter("@StudyId", Convert.ToString(_studyCROMapping.StudyId));
                _SqlParameter[1] = new SqlParameter("@CROList", Convert.ToString(XmlString));
                _SqlParameter[2] = new SqlParameter("@OrganizationId", Convert.ToString(_studyCROMapping.OrganizationId));
                _SqlParameter[3] = new SqlParameter("@CreatedBy", Convert.ToString(_studyCROMapping.CreatedBy));
                _SqlParameter[4] = new SqlParameter("@ActionType", _studyCROMapping.ActionType);
                _SqlParameter[5] = new SqlParameter("@StudyList", Xmlstudy);
                _SqlParameter[6] = new SqlParameter("@CROId", Convert.ToString(_studyCROMapping.CROId));
                _SqlParameter[7] = new SqlParameter("@ModuleList", XmlModule);
                _SqlParameter[8] = new SqlParameter("@UnitId", _studyCROMapping.UnitId);
                _SqlParameter[9] = new SqlParameter("@FromDate", _studyCROMapping.FromDate);
                _SqlParameter[10] = new SqlParameter("@ToDate", _studyCROMapping.ToDate);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_AssignStudytoCRO]", CommandType.StoredProcedure, _SqlParameter);
                if (_studyCROMapping.ActionType == "CROANDStudyList" || _studyCROMapping.ActionType == "StaffANDStudyList")
                {
                    ds.Tables[0].TableName = "CROList";
                    ds.Tables[1].TableName = "StudyList";

                    if (ds.Tables.Count > 2)
                    {
                        ds.Tables[2].TableName = "StudyCRoRecordsList";
                        ds.Tables[3].TableName = "ModulesList";
                    }
                    _ApiResponse.success = 1;
                    _ApiResponse.data = ds;
                }
                else if (_studyCROMapping.ActionType == "ListOfCROPerStudy" || _studyCROMapping.ActionType == "ListOfStaffPerStudy")
                {
                    ds.Tables[0].TableName = "CROList";
                    _ApiResponse.success = 1;
                    _ApiResponse.data = ds;
                }

                else if (_studyCROMapping.ActionType == "ListOfStudyPerCRO" || _studyCROMapping.ActionType == "ListOfStudyPerStaff")
                {
                    ds.Tables[0].TableName = "StudyList";
                    _ApiResponse.success = 1;
                    _ApiResponse.data = ds;
                }

                else
                {
                    ds.Tables[0].TableName = "SuccessStory";
                    if (Convert.ToString(ds.Tables[0].Rows[0]["Success"]) == "1")
                    {
                        _ApiResponse.success = 1;
                        _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                        _ApiResponse.data = ds;

                        if (_studyCROMapping.ActionType == "AssignStudyToCRO")
                        {
                            //study 1 cro multiple
                            //in this case remove or assigned only single to a single staff/admin
                            if (_studyCROMapping.CROList != null)
                            {
                                for (int i = 0; i < _studyCROMapping.CROList.Count; i++)
                                {
                                    string message = "Dear " + _studyCROMapping.CROList[i].FirstName + ",<br /><br />";
                                    message += "" + _studyCROMapping.LoggedInuserName + " (" + Convert.ToString(ds.Tables[0].Rows[0]["orgName"]) + ") has assigned " + _studyCROMapping.StudyList[0].StudyName + " to your organisation.<br />Please connect to the site organisation in case of any queries.<br /><br />Thank you.";
                                    SendMail_SMS.Instance.sendExceptionMail(_studyCROMapping.CROList[i].EmailId, "Study delegation details", message);
                                }
                            }

                            if (_studyCROMapping.RemovedCRO != null)
                            {
                                for (int i = 0; i < _studyCROMapping.RemovedCRO.Count; i++)
                                {
                                    if (i % 100 == 0)
                                    {
                                        string message = "Dear " + _studyCROMapping.RemovedCRO[i + 1].ToString().Replace("\n", "").Trim() + ",<br /><br />";
                                        message += "" + _studyCROMapping.LoggedInuserName + " (" + Convert.ToString(ds.Tables[0].Rows[0]["orgName"]) + ") has removed " + _studyCROMapping.StudyList[0].StudyName + " from your organisation.<br />Please connect to the site organisation in case of any queries.<br /><br />Thank you.";
                                        SendMail_SMS.Instance.sendExceptionMail(_studyCROMapping.RemovedCRO[i], "Study delegation details", message);
                                    }
                                }
                            }

                        }

                        else if (_studyCROMapping.ActionType == "AssignCROToStudy")
                        {
                            //study multiple cro single
                            //in thi scase remove multiple study
                            string assignedstudy = "";
                            string removedstudy = "";
                            if (_studyCROMapping.StudyList != null)
                            {
                                for (int i = 0; i < _studyCROMapping.StudyList.Count; i++)
                                {
                                    assignedstudy += _studyCROMapping.StudyList[i].StudyName + ", ";
                                }
                            }
                            assignedstudy = assignedstudy.Trim().TrimEnd(',');
                            if (_studyCROMapping.RemovedStudy != null)
                                removedstudy = string.Join(",", _studyCROMapping.RemovedStudy);

                            string message = "Dear " + _studyCROMapping.CROList[0].FirstName.Replace("\n", "").Trim() + ",<br /><br />";

                            if (_studyCROMapping.StudyList != null && _studyCROMapping.RemovedStudy != null)
                            {
                                message += "" + _studyCROMapping.LoggedInuserName + " (" + Convert.ToString(ds.Tables[0].Rows[0]["orgName"]) + ") has assigned " + assignedstudy + " to your organisation and has removed " + removedstudy + " from your organisation.";
                            }

                            else if (_studyCROMapping.StudyList != null && _studyCROMapping.RemovedStudy == null)
                            {
                                message += "" + _studyCROMapping.LoggedInuserName + " (" + Convert.ToString(ds.Tables[0].Rows[0]["orgName"]) + ") has assigned " + assignedstudy + " to your organisation.";
                            }
                            else if (_studyCROMapping.StudyList == null && _studyCROMapping.RemovedStudy != null)
                            {
                                message += "" + _studyCROMapping.LoggedInuserName + " (" + Convert.ToString(ds.Tables[0].Rows[0]["orgName"]) + ") has removed " + removedstudy + " from your organisation.";
                            }

                            message += "<br /> Please connect to the site organisation in case of any queries.<br /><br /> Thank you.";
                            SendMail_SMS.Instance.sendExceptionMail(_studyCROMapping.CROList[0].EmailId, "Study delegation details", message);

                        }
                    }
                    else
                    {
                        _ApiResponse.success = 0;
                        _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                        _ApiResponse.data = ds;
                    }
                }

            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_studyCROMapping),
                   "SettingsSetup", "StudyCROMappingActivity", "[dbo].[usp_AssignStudytoCRO]", "Clinical Trail");
            }
            return _ApiResponse;
        }


        public ApiResponse ImportDataActivity(ImportData _importData)
        {
            string inputXML = string.Empty;
            DataTable dtError = new DataTable();
            DataTable dtInput = new DataTable();

            dtError = _importData.InputDataTable.Clone();

            DataRow[] rowsToCopy;
            rowsToCopy = _importData.InputDataTable.Select("ErrorMessage IS NOT NULL");
            foreach (DataRow temp in rowsToCopy)
            {
                dtError.ImportRow(temp);
            }

            dtInput = _importData.InputDataTable.Clone();
            rowsToCopy = _importData.InputDataTable.Select("ErrorMessage IS NULL");
            foreach (DataRow temp in rowsToCopy)
            {
                dtInput.ImportRow(temp);
            }

            if (_importData.ActionType == "VolunteerFile")
            {
                dtInput.TableName = "Volunteer";
            }
            else if (_importData.ActionType == "StudyFile")
            {
                dtInput.TableName = "Study";
            }
            else if (_importData.ActionType == "UserFile")
            {
                dtInput.TableName = "User";
            }
            StringWriter sw = new StringWriter();
            dtInput.WriteXml(sw, XmlWriteMode.IgnoreSchema);
            inputXML = sw.ToString();

            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                if (dtInput.Rows.Count > 0)
                {

                    _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                    SqlParameter[] _SqlParameter = new SqlParameter[4];
                    _SqlParameter[0] = new SqlParameter("@XMLContent", Convert.ToString(inputXML));
                    _SqlParameter[1] = new SqlParameter("@OrganizationId", Convert.ToString(_importData.OrganizationId));
                    _SqlParameter[2] = new SqlParameter("@CreatedBy", Convert.ToString(_importData.CreatedBy));
                    _SqlParameter[3] = new SqlParameter("@ActionType", _importData.ActionType);
                    ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_FileUpload]", CommandType.StoredProcedure, _SqlParameter);
                    if (ds.Tables[0].Rows.Count > 0)
                    {

                        ds.Tables[0].TableName = "SuccessStory";
                        if (Convert.ToString(ds.Tables[0].Rows[0]["Success"]) == "1")
                        {
                            _ApiResponse.success = 1;
                            _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);

                            rowsToCopy = ds.Tables[1].Select("ErrorMessage IS NOT NULL");
                            foreach (DataRow temp in rowsToCopy)
                            {
                                dtError.ImportRow(temp);
                            }

                            ds.Tables.Add(dtError);
                            ds.Tables[2].TableName = "ErrorList";

                            if (dtError.Rows.Count > 0)
                            {
                                ds.Tables[0].Rows[0]["Success"] = "0";
                                ds.Tables[0].Rows[0]["MSG"] = "Please check you downloaded excel how much record saved.";
                            }

                            if (_importData.ActionType == "UserFile")
                            {
                                DataTable dtSendMail = new DataTable();
                                dtSendMail = ds.Tables[1].Clone();

                                rowsToCopy = ds.Tables[1].Select("ErrorMessage IS NULL");
                                foreach (DataRow temp in rowsToCopy)
                                {
                                    dtSendMail.ImportRow(temp);
                                }

                                EmailTemplateSender.Instance.SendEmailToImportedUser(dtSendMail);
                            }
                            ds.Tables.RemoveAt(1);
                            _ApiResponse.data = ds;
                        }
                        else
                        {
                            _ApiResponse.success = 0;
                            _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                            _ApiResponse.data = ds;
                        }
                    }
                    else
                    {
                        _ApiResponse.success = 0;
                        _ApiResponse.message = "Try Again";
                    }
                }
                else
                {
                    DataTable dtResponse = new DataTable();
                    dtResponse.Columns.Add("MSG");
                    dtResponse.Columns.Add("Success");
                    dtResponse.Columns.Add("FilePath");

                    dtResponse.Rows.Add();
                    dtResponse.Rows[0]["MSG"] = "Please look at your uploaded excel sheet, it may be blank or wrong entries occurs.";
                    dtResponse.Rows[0]["Success"] = "0";

                    ds.Tables.Add(dtResponse);
                    ds.Tables.Add(dtError);

                    ds.Tables[0].TableName = "SuccessStory";
                    ds.Tables[1].TableName = "ErrorList";
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "Please look at your uploaded excel sheet, it may be blank or wrong entries occurs.";
                    _ApiResponse.data = ds;
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_importData),
                  "SettingsSetup", "ImportDataActivity", "", "Clinical Trail");
            }
            return _ApiResponse;
        }

        public ApiResponse AddCROActivity(ClinicalTrialAPI.Models.CRO _CRO)
        {

            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[14];
                _SqlParameter[0] = new SqlParameter("@CROId", _CRO.CROId);
                _SqlParameter[1] = new SqlParameter("@FirstName", _CRO.FirstName);
                _SqlParameter[2] = new SqlParameter("@LastName", _CRO.LastName);
                _SqlParameter[3] = new SqlParameter("@ProfilePictureUrl", _CRO.ProfilePictureUrl);
                _SqlParameter[4] = new SqlParameter("@Gender", _CRO.Gender);
                _SqlParameter[5] = new SqlParameter("@DOB", _CRO.DOB);
                _SqlParameter[6] = new SqlParameter("@EmailId", _CRO.EmailId);
                _SqlParameter[7] = new SqlParameter("@ContactNumber", _CRO.ContactNumber);
                _SqlParameter[8] = new SqlParameter("@RoleIdRef", _CRO.RoleIdRef);
                _SqlParameter[9] = new SqlParameter("@OrganizationId", _CRO.OrganizationId);
                _SqlParameter[10] = new SqlParameter("@CreatedBy", _CRO.CreatedBy);
                _SqlParameter[11] = new SqlParameter("@ActionType", _CRO.ActionType);
                _SqlParameter[12] = new SqlParameter("@OrgName", _CRO.OrgName);
                _SqlParameter[13] = new SqlParameter("@OrgCountry", _CRO.OrgCountry);

                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_CROReg]", CommandType.StoredProcedure, _SqlParameter);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (_CRO.ActionType == "Insert" && dt.Rows[0]["Success"].ToString() == "1")
                    {
                        string appurl = WebConfigurationManager.AppSettings["ApplicationURL"];
                        string msgPassword = " Dear " + Convert.ToString(dt.Rows[0]["UserName"]) + ",<br /><br /> Welcome to Ascension Q! Your Clinical Trial login  details are as follows: <br /><br /> URL : " + appurl + " <br /> Email Id : " + Convert.ToString(dt.Rows[0]["EmailId"]) + "<br />Password: " + Convert.ToString(dt.Rows[0]["Dos"]) + "<br /><br />Please note that your Access Code details will be provided in a subsequent email.<br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                        SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["EmailId"]), "Login Details: Password", msgPassword);

                        string msgAccessCode = " Dear " + Convert.ToString(dt.Rows[0]["UserName"]) + ",<br /><br />Welcome to Ascension Q! Your Clinical Trial Access code details are as follows: <br /><br /> Access Code :" + Convert.ToString(dt.Rows[0]["ACC"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                        SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(dt.Rows[0]["EmailId"]), "Login Details: Access Code", msgAccessCode);
                    }

                    if (_CRO.ActionType == "GetCROList")
                    {
                        ds.Tables[0].TableName = "CROList";
                        _ApiResponse.success = 1;
                        _ApiResponse.data = ds;
                    }
                    else if (_CRO.ActionType == "getCRObyId")
                    {
                        ds.Tables[0].TableName = "CROList";
                        _ApiResponse.success = 1;
                        _ApiResponse.data = ds;
                    }
                    else if (_CRO.ActionType == "GetModuleList")
                    {
                        ds.Tables[0].TableName = "ModuleList";
                        _ApiResponse.success = 1;
                        _ApiResponse.data = ds;
                    }
                    else
                    {
                        ds.Tables[0].TableName = "SuccessStory";
                        _ApiResponse.success = 1;
                        _ApiResponse.data = ds;
                    }
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_CRO),
                   "SettingsSetup", "AddCROActivity", "[dbo].[usp_CROReg]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        public ApiResponse LocationSetUpActivity(Location _location)
        {
            _ApiResponse = new ApiResponse();

            string xmlLocationList = null;


            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[5];
                _SqlParameter[0] = new SqlParameter("@LocationList", xmlLocationList);
                _SqlParameter[1] = new SqlParameter("@OrganizationId", _location.OrganizationId);
                _SqlParameter[2] = new SqlParameter("@CreatedBy", _location.CreatedBy);
                _SqlParameter[3] = new SqlParameter("@ActionType", _location.ActionType);
                _SqlParameter[4] = new SqlParameter("@LocationId", _location.LocationId);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_AssignLocationToOrganization]", CommandType.StoredProcedure, _SqlParameter);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].TableName = "SuccessStory";
                    _ApiResponse.success = 1;
                    _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_location),
                   "SettingsSetup", "LocationSetUpActivity", "[dbo].[usp_AssignLocationToOrganization]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        public ApiResponse CROOrganizationMappingActivity(CROOrganizationActivity _cROOrganizationActivity)
        {
            _ApiResponse = new ApiResponse();

            string xml = null;
            if (_cROOrganizationActivity.CROOrganizationXML != null)
                xml = new XElement("SelectedCROS", from c in _cROOrganizationActivity.CROOrganizationXML select new XElement("CROList", new XElement("CROId", c.CROId), new XElement("OrganizationId", c.OrganizationId))).ToString();

            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[4];
                _SqlParameter[0] = new SqlParameter("@CROOrganizationXML", xml);
                _SqlParameter[1] = new SqlParameter("@ActionType", _cROOrganizationActivity.ActionType);
                _SqlParameter[2] = new SqlParameter("@CreatedBy", _cROOrganizationActivity.CreatedBy);
                _SqlParameter[3] = new SqlParameter("@SelectedOrganizationId", _cROOrganizationActivity.SelectedOrganizationId);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_CROOrganizationActivity]", CommandType.StoredProcedure, _SqlParameter);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (_cROOrganizationActivity.ActionType == "GetOrganizationAndCRO")
                    {
                        ds.Tables[0].TableName = "OrganizationList";
                        ds.Tables[1].TableName = "CROList";
                    }
                    else if (_cROOrganizationActivity.ActionType == "GetAllOrganization")
                    {

                        ds.Tables[0].TableName = "SuccessStory";
                        ds.Tables[1].TableName = "ModuleList";
                        ds.Tables[2].TableName = "MSTModuleList";

                    }
                    else
                    {
                        ds.Tables[0].TableName = "SuccessStory";
                    }
                    _ApiResponse.success = 1;
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_cROOrganizationActivity),
                   "SettingsSetup", "CROOrganizationMappingActivity", "[dbo].[usp_CROOrganizationActivity]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        public ApiResponse CreateOrganizationActivity(OrgSetup _orgSetup)
        {
            _ApiResponse = new ApiResponse();

            DataSet ds = new DataSet();
            try
            {
                string modulelist = string.Empty;
                string pagelist = string.Empty;
                string pageactionlist = string.Empty;
                if (_orgSetup.OrgModules != null)
                    modulelist = new XElement("SelectedModules", from c in _orgSetup.OrgModules
                                                                 select new XElement("ModuleList", new XElement("ModuleId", c.ToUpper()))).ToString();
                if (_orgSetup.PageList != null)
                    pagelist = new XElement("SelectedPages", from c in _orgSetup.PageList
                                                             select new XElement("PageList", new XElement("PageId", c.PageId),
                                                             new XElement("ModuleId", c.ModuleId.ToUpper()))).ToString();

                if (_orgSetup.PageActionList != null)
                    pageactionlist = new XElement("SelectedPageActions", from c in _orgSetup.PageActionList
                                                                         select new XElement("PageActionList", new XElement("ActionId", c.ActionId),
                                                                         new XElement("ModuleId", c.ModuleId.ToUpper()),
                                                                         new XElement("PageId", c.PageId))).ToString();

                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[24];
                _SqlParameter[0] = new SqlParameter("@FirstName", _orgSetup.FirstName);
                _SqlParameter[1] = new SqlParameter("@LastName", _orgSetup.LastName);
                _SqlParameter[2] = new SqlParameter("@Gender", _orgSetup.Gender);
                _SqlParameter[3] = new SqlParameter("@DOB", _orgSetup.DOB);
                _SqlParameter[4] = new SqlParameter("@ContactNumber", _orgSetup.ContactNumber);
                _SqlParameter[5] = new SqlParameter("@EmailId", _orgSetup.EmailId);
                _SqlParameter[6] = new SqlParameter("@AccessDate", _orgSetup.AccessDate);
                _SqlParameter[7] = new SqlParameter("@ClinicStartTime", _orgSetup.ClinicStartTime);
                _SqlParameter[8] = new SqlParameter("@ClinicEndTime", _orgSetup.ClinicEndTime);
                _SqlParameter[9] = new SqlParameter("@SlotDuration", _orgSetup.SlotDuration);
                _SqlParameter[10] = new SqlParameter("@NoofBookingPerSlot", _orgSetup.NoofBookingPerSlot);
                _SqlParameter[11] = new SqlParameter("@CurrencySymbol", _orgSetup.CurrencySymbol);
                _SqlParameter[12] = new SqlParameter("@OrgName", _orgSetup.OrgName);
                _SqlParameter[13] = new SqlParameter("@OrgCountry", _orgSetup.OrgCountry);
                _SqlParameter[14] = new SqlParameter("@Hour", _orgSetup.Hour);
                _SqlParameter[15] = new SqlParameter("@Min", _orgSetup.Min);
                _SqlParameter[16] = new SqlParameter("@ReminderBeforeStartNumber", _orgSetup.ReminderBeforeStartNumber);
                _SqlParameter[17] = new SqlParameter("@ReminderBeforeStartSuffix", _orgSetup.ReminderBeforeStartSuffix);
                _SqlParameter[18] = new SqlParameter("@CreatedBy", _orgSetup.CreatedBy);
                _SqlParameter[19] = new SqlParameter("@OrganizationId", _orgSetup.OrganizationId);
                _SqlParameter[20] = new SqlParameter("@ActionType", _orgSetup.ActionType);
                _SqlParameter[21] = new SqlParameter("@ModuleXML", modulelist);
                _SqlParameter[22] = new SqlParameter("@PageXML", pagelist);
                _SqlParameter[23] = new SqlParameter("@PageActionXML", pageactionlist);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_CreateOrganization]", CommandType.StoredProcedure, _SqlParameter);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (_orgSetup.ActionType == "GetInternalOrgModules")
                    {
                        ds.Tables[0].TableName = "SuccessStory";
                        _ApiResponse.success = 1;
                        _ApiResponse.data = ds;
                    }
                    else
                    {
                        if (Convert.ToString(ds.Tables[0].Rows[0]["Success"]) == "1")
                        {
                            if (_orgSetup.ActionType == "Create")
                            {
                                string appurl = WebConfigurationManager.AppSettings["ApplicationURL"];
                                string msgPassword = " Dear " + Convert.ToString(ds.Tables[0].Rows[0]["UserName"]) + ",<br /><br /> Welcome to Ascension Q! Your Clinical Trial login  details are as follows: <br /><br /> URL : " + appurl + " <br /> Email Id : " + Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]) + "<br />Password: " + Convert.ToString(ds.Tables[0].Rows[0]["Dos"]) + "<br /><br />Please note that your Access Code details will be provided in a subsequent email.<br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";

                                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]), "Login Details: Password", msgPassword);

                                string msgAccessCode = " Dear " + Convert.ToString(ds.Tables[0].Rows[0]["UserName"]) + ",<br /><br />Welcome to Ascension Q! Your Clinical Trial Access code details are as follows: <br /><br /> Access Code :" + Convert.ToString(ds.Tables[0].Rows[0]["ACC"]) + "<br /><br /><br />" + "Thank you,<br /> AscensionQ Team" + "<br />";
                                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ds.Tables[0].Rows[0]["EmailId"]), "Login Details: Access Code", msgAccessCode);
                            }
                            ds.Tables[0].TableName = "SuccessStory";
                            _ApiResponse.success = 1;
                            _ApiResponse.data = ds;
                        }
                        else
                        {
                            ds.Tables[0].TableName = "SuccessStory";
                            _ApiResponse.success = 0;
                            _ApiResponse.data = ds;
                        }
                    }


                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_orgSetup),
                  "SettingsSetup", "CreateOrganizationActivity", "[dbo].[usp_CreateOrganization]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        public ApiResponse GetOrganizationsDetailsActivity()
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[0];

                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_GetOrgDetails]", CommandType.StoredProcedure, _SqlParameter);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].TableName = "SuccessStory";
                    _ApiResponse.success = 1;
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: ",
                 "SettingsSetup", "GetOrganizationsDetailsActivity", "[dbo].[usp_GetOrgDetails]", "Clinical Trail");
            }
            return _ApiResponse;
        }


        public ApiResponse DepartmentSetup(Department _department)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                string _departmentSiteXML = string.Empty;
                if (_department.DepartmentMappingList != null)
                    _departmentSiteXML = new XElement("SelectedDepartments", from c in _department.DepartmentMappingList
                                                                             select new XElement("DepartmentList", new XElement("DepartmentId", c.DepartmentId.ToUpper()),
                                                                             new XElement("OrganizationId", c.OrganizationId.ToUpper()))).ToString();

                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[7];
                _SqlParameter[0] = new SqlParameter("@DepartmentId", _department.DepartmentId);
                _SqlParameter[1] = new SqlParameter("@DepartmentName", _department.DepartmentName);
                _SqlParameter[2] = new SqlParameter("@CreatedBy", _department.CreatedBy);
                _SqlParameter[3] = new SqlParameter("@ActionType", _department.ActionType);
                _SqlParameter[4] = new SqlParameter("@IsArchived", _department.IsArchived);
                _SqlParameter[5] = new SqlParameter("@OrganizationId", _department.OrganizationId);
                _SqlParameter[6] = new SqlParameter("@DepartmentSiteXML", _departmentSiteXML);

                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_DepartmentActivity]", CommandType.StoredProcedure, _SqlParameter);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (_department.ActionType == "GetDepartmentandSite")
                    {
                        ds.Tables[0].TableName = "OrganizationList";
                        ds.Tables[1].TableName = "DepartmentList";
                    }
                    else if (_department.ActionType == "GetDepartmentofSite")
                    {
                        ds.Tables[0].TableName = "SuccessStory";
                        ds.Tables[1].TableName = "DepartmentList";
                    }
                    else if (_department.ActionType == "GetStaffRoleandSite")
                    {
                        ds.Tables[0].TableName = "OrganizationList";
                        ds.Tables[1].TableName = "StaffRolesList";
                    }
                    else
                    {
                        ds.Tables[0].TableName = "SuccessStory";
                    }
                    _ApiResponse.success = 1;
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                    _ApiResponse.data = ds;
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_department),
                  "SettingsSetup", "DepartmentSetup", "[dbo].[usp_DepartmentActivity]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        public ApiResponse DepartmentRoleAssignAcivity(DepartmentRole _departmentRole)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[10];
                _SqlParameter[0] = new SqlParameter("@VisitId", _departmentRole.VisitId);
                _SqlParameter[1] = new SqlParameter("@DepartmentId", _departmentRole.DepartmentId);
                _SqlParameter[2] = new SqlParameter("@StaffRoleId", _departmentRole.RoleId);
                _SqlParameter[3] = new SqlParameter("@ActionType", _departmentRole.ActionType);
                _SqlParameter[4] = new SqlParameter("@NoOfStaff", _departmentRole.StaffRequired);
                _SqlParameter[5] = new SqlParameter("@StudyIdRef", _departmentRole.StudyId);
                _SqlParameter[6] = new SqlParameter("@OrganizationId", _departmentRole.OrganizationId);
                _SqlParameter[7] = new SqlParameter("@CreatedBy", _departmentRole.CreatedBy);
                _SqlParameter[8] = new SqlParameter("@TimeReq", _departmentRole.TimeReq);
                _SqlParameter[9] = new SqlParameter("@GroupId", _departmentRole.GroupId);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_StaffDeptPerVisit]", CommandType.StoredProcedure, _SqlParameter);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].TableName = "SuccessStory";
                    _ApiResponse.success = 1;
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                    _ApiResponse.data = ds;
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_departmentRole),
                   "SettingsSetup", "DepartmentRoleAssignAcivity", "[dbo].[usp_StaffDeptPerVisit]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        public ApiResponse SuperAdminAnalyticsActivity(StaffRoleAnalytics _staffRoleAnalytics)
        {
            string VisitXML = string.Empty;
            string DepartmentXML = string.Empty;
            string StaffRolesXML = string.Empty;
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {


                if (_staffRoleAnalytics.VisitIds != null)
                    VisitXML = new XElement("VisitList", from c in _staffRoleAnalytics.VisitIds select new XElement("Visit", new XElement("VisitId", c.VisitId.ToUpper()))).ToString();

                if (_staffRoleAnalytics.DepartmentIds != null)
                    DepartmentXML = new XElement("DepartmentList", from c in _staffRoleAnalytics.DepartmentIds select new XElement("Department", new XElement("DepartmentId", c.DepartmentId.ToUpper()))).ToString();

                if (_staffRoleAnalytics.StaffRoleIds != null)
                    StaffRolesXML = new XElement("StaffRoleList", from c in _staffRoleAnalytics.StaffRoleIds select new XElement("StaffRole", new XElement("RoleId", c.RoleId.ToUpper()))).ToString();


                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[13];
                _SqlParameter[0] = new SqlParameter("@StudyId", _staffRoleAnalytics.StudyId);
                _SqlParameter[1] = new SqlParameter("@xmlVisitIds", VisitXML);
                _SqlParameter[2] = new SqlParameter("@xmlDepartmentIds", DepartmentXML);
                _SqlParameter[3] = new SqlParameter("@Period", _staffRoleAnalytics.Period);
                _SqlParameter[4] = new SqlParameter("@StartPeriod", _staffRoleAnalytics.StartPeriod);
                _SqlParameter[5] = new SqlParameter("@EndPeriod", _staffRoleAnalytics.EndPeriod);
                _SqlParameter[6] = new SqlParameter("@OrganizationId", _staffRoleAnalytics.OrganizationId);
                _SqlParameter[7] = new SqlParameter("@UnitId", _staffRoleAnalytics.UnitId);
                _SqlParameter[8] = new SqlParameter("@ActionType", _staffRoleAnalytics.ActionType);
                _SqlParameter[9] = new SqlParameter("@AdhocVisit", _staffRoleAnalytics.AdhocVisit);
                _SqlParameter[10] = new SqlParameter("@xmlStaffRoleIds", StaffRolesXML);
                _SqlParameter[11] = new SqlParameter("@GroupNameIdRef", _staffRoleAnalytics.GroupId);
                _SqlParameter[12] = new SqlParameter("@VisitType", _staffRoleAnalytics.VisitType);

                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_SuperAnalytics]", CommandType.StoredProcedure, _SqlParameter);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].TableName = "SuccessStory";

                    if (_staffRoleAnalytics.ActionType == "DepartmentWiseRole")
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("VisitName");
                        dt.Columns.Add("VisitId");

                        //dynamic addc columns respect to roles
                        var Y = (from r in ds.Tables["SuccessStory"].AsEnumerable()
                                 select r["Roles"]).Distinct().ToList();

                        for (int i = 0; i < Y.Count; i++)
                        {
                            dt.Columns.Add(Y[i].ToString());
                        }

                        //get distinct vistid
                        var x = (from r in ds.Tables["SuccessStory"].AsEnumerable()
                                 select r["VisitId"]).Distinct().ToList();

                        for (int i = 0; i < x.Count; i++)
                        {
                            DataRow dr = dt.NewRow();
                            dr["VisitId"] = x[i].ToString();
                            //loop for roles column
                            for (int j = 0; j < Y.Count; j++)
                            {
                                for (int k = 0; k < ds.Tables["SuccessStory"].Rows.Count; k++)
                                {
                                    if (ds.Tables["SuccessStory"].Rows[k]["VisitId"].ToString() == x[i].ToString())
                                    {
                                        dr["VisitName"] = ds.Tables["SuccessStory"].Rows[k]["VisitName"].ToString();
                                    }
                                    if (Y[j].ToString() == ds.Tables["SuccessStory"].Rows[k]["Roles"].ToString() && ds.Tables["SuccessStory"].Rows[k]["VisitId"].ToString() == x[i].ToString())
                                    {
                                        dr[Y[j].ToString()] = ds.Tables["SuccessStory"].Rows[k]["Utilizationhours"].ToString();

                                    }
                                }
                            }
                            dt.Rows.Add(dr);
                            dt.AcceptChanges();
                        }
                        ds.Tables.Remove("SuccessStory");
                        dt.TableName = "SuccessStory";
                        ds.Tables.Add(dt);
                    }
                    else if (_staffRoleAnalytics.ActionType == "StaffUtilizationByRole" || _staffRoleAnalytics.ActionType == "StaffUtilizationByDepartment" || _staffRoleAnalytics.ActionType == "DepartmentWiseRoleByVisitTime")
                    {
                        DataTable dt = new DataTable();
                        if (_staffRoleAnalytics.ActionType == "StaffUtilizationByDepartment")
                        {
                            dt.Columns.Add("DepartmentName");
                            dt.Columns.Add("DepartmentId");
                        }
                        else
                        {
                            dt.Columns.Add("RoleName");
                            dt.Columns.Add("RoleId");
                        }


                        //dynamic addc columns respect to visitname
                        var Y = (from r in ds.Tables["SuccessStory"].AsEnumerable()
                                 select r["VisitName"]).Distinct().ToList();

                        for (int i = 0; i < Y.Count; i++)
                        {
                            dt.Columns.Add(Y[i].ToString());
                        }

                        var x = new List<Object>();
                        //get distinct roleid
                        if (_staffRoleAnalytics.ActionType == "StaffUtilizationByDepartment")
                        {
                            x = (from r in ds.Tables["SuccessStory"].AsEnumerable()
                                 select r["DepartmentId"]).Distinct().ToList();
                        }
                        else
                        {
                            x = (from r in ds.Tables["SuccessStory"].AsEnumerable()
                                 select r["Roles"]).Distinct().ToList();
                        }


                        for (int i = 0; i < x.Count; i++)
                        {
                            DataRow dr = dt.NewRow();
                            if (_staffRoleAnalytics.ActionType == "StaffUtilizationByDepartment")
                                dr["DepartmentId"] = x[i].ToString();
                            else
                                dr["RoleName"] = x[i].ToString();

                            //loops for roles and hours column
                            for (int j = 0; j < Y.Count; j++)
                            {
                                for (int k = 0; k < ds.Tables["SuccessStory"].Rows.Count; k++)
                                {
                                    if (_staffRoleAnalytics.ActionType == "StaffUtilizationByDepartment")
                                    {
                                        if (ds.Tables["SuccessStory"].Rows[k]["DepartmentId"].ToString() == x[i].ToString())
                                        {
                                            dr["DepartmentName"] = ds.Tables["SuccessStory"].Rows[k]["DepartmentName"].ToString();
                                        }
                                    }
                                    else
                                    {
                                        if (ds.Tables["SuccessStory"].Rows[k]["Roles"].ToString() == x[i].ToString())
                                        {
                                            dr["RoleName"] = ds.Tables["SuccessStory"].Rows[k]["Roles"].ToString();
                                        }
                                    }
                                    if (_staffRoleAnalytics.ActionType == "StaffUtilizationByDepartment")
                                    {
                                        if (Y[j].ToString() == ds.Tables["SuccessStory"].Rows[k]["VisitName"].ToString() && ds.Tables["SuccessStory"].Rows[k]["DepartmentId"].ToString() == x[i].ToString())
                                        {
                                            dr[Y[j].ToString()] = ds.Tables["SuccessStory"].Rows[k]["Utilizationhours"].ToString();

                                        }
                                    }
                                    else
                                    {
                                        if (Y[j].ToString() == ds.Tables["SuccessStory"].Rows[k]["VisitName"].ToString() && ds.Tables["SuccessStory"].Rows[k]["Roles"].ToString() == x[i].ToString())
                                        {
                                            dr[Y[j].ToString()] = ds.Tables["SuccessStory"].Rows[k]["Utilizationhours"].ToString();

                                        }
                                    }
                                }
                            }
                            dt.Rows.Add(dr);
                            dt.AcceptChanges();
                        }
                        ds.Tables.Remove("SuccessStory");
                        dt.TableName = "SuccessStory";
                        ds.Tables.Add(dt);
                    }

                    _ApiResponse.success = 1;

                    if (ds.Tables.Count == 2)
                        ds.Tables[1].TableName = "StaffRolesList";

                    _ApiResponse.data = ds;
                }
                else if (_staffRoleAnalytics.ActionType == "DeptListPerStudyPerVisit")
                {
                    ds.Tables[0].TableName = "SuccessStory";
                    ds.Tables[1].TableName = "StaffRolesList";
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                    _ApiResponse.data = ds;
                }
                else
                {
                    ds.Tables[0].TableName = "SuccessStory";
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                    _ApiResponse.data = ds;
                }
            }
            catch (Exception ex)
            {
                //SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_staffRoleAnalytics),
                //"SettingsSetup", "SuperAdminAnalyticsActivity", "[dbo].[usp_SuperAnalytics]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        #region Report
        public ApiResponse StudyWiseAppointment(Analytics _analytics)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                string StudyList = string.Empty;
                string GroupList = string.Empty;
                if (_analytics.StudyIds != null)
                    StudyList = new XElement("SelectedStudys", from c in _analytics.StudyIds
                                                               select new XElement("StudyList", new XElement("StudyId", c.ToUpper()))).ToString();

                if (_analytics.GroupIds != null)
                    GroupList = new XElement("SelectedGroups", from c in _analytics.GroupIds
                                                               select new XElement("GroupList", new XElement("GroupId", c.ToUpper()))).ToString();

                SqlParameter[] _sqlPara = new SqlParameter[8];
                _sqlPara[0] = new SqlParameter("@xmlStudyIds", StudyList);
                _sqlPara[1] = new SqlParameter("@Period", _analytics.Period);
                _sqlPara[2] = new SqlParameter("@StartPeriod", _analytics.StartDate);
                _sqlPara[3] = new SqlParameter("@EndPeriod", _analytics.EndDate);
                _sqlPara[4] = new SqlParameter("@OrganizationId", _analytics.OrganizationId);
                _sqlPara[5] = new SqlParameter("@ActionType", _analytics.ActionType);
                _sqlPara[6] = new SqlParameter("@AppStatus", _analytics.AppStatus);
                _sqlPara[7] = new SqlParameter("@xmlGroupIds", GroupList);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[Usp_GetStudyWiseAppointment]", CommandType.StoredProcedure, _sqlPara);

                if (ds.Tables.Count > 0)
                {
                    if (_analytics.ActionType == "VolunteersInGroup")
                    {
                        ds.Tables[0].TableName = "SuccessStory";
                        ds.Tables[1].TableName = "Groups";
                    }
                    else
                    {
                        ds.Tables[0].TableName = "SuccessStory";
                    }
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "";
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "";
                    _ApiResponse.data = ds.Tables[0];
                }

            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_analytics),
                   "SettingsSetup", "StudyWiseAppointment", "[dbo].[Usp_GetStudyWiseAppointment]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        public ApiResponse StudyWiseVolunteers(VolunteerFilters _volunteerFilters)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                string VisitXML = string.Empty;
                string adhoc = string.Empty;

                if (_volunteerFilters.VisitIds != null)
                {
                    if (_volunteerFilters.VisitIds.IndexOf("") != -1)
                    {
                        adhoc = "ADHOCVISIT";
                        _volunteerFilters.VisitIds.RemoveAt(_volunteerFilters.VisitIds.IndexOf(""));
                    }

                    VisitXML = new XElement("VisitList", from c in _volunteerFilters.VisitIds select new XElement("Visit", new XElement("VisitId", c.ToUpper()))).ToString();
                }

                SqlParameter[] _sqlPara = new SqlParameter[11];
                _sqlPara[0] = new SqlParameter("@StudyId", _volunteerFilters.StudyId);
                _sqlPara[1] = new SqlParameter("@VisitXML", VisitXML);
                _sqlPara[2] = new SqlParameter("@AppointmentType", _volunteerFilters.AppointmentType == null ? null : _volunteerFilters.AppointmentType.TrimEnd(','));
                _sqlPara[3] = new SqlParameter("@AppointmentStatus", _volunteerFilters.AppointmentStatus == null ? null : _volunteerFilters.AppointmentStatus.TrimEnd(','));
                _sqlPara[4] = new SqlParameter("@VolunteerStatus", _volunteerFilters.VolunteerStatus == null ? null : _volunteerFilters.VolunteerStatus.TrimEnd(','));
                _sqlPara[5] = new SqlParameter("@OrganizationId", _volunteerFilters.OrganizationId);
                _sqlPara[6] = new SqlParameter("@CreatedBy", _volunteerFilters.CreatedBy);
                _sqlPara[7] = new SqlParameter("@ActionType", _volunteerFilters.ActionType);
                _sqlPara[8] = new SqlParameter("@AdhocVisit", adhoc);
                _sqlPara[9] = new SqlParameter("@StartPeriod", _volunteerFilters.StartPeriod);
                _sqlPara[10] = new SqlParameter("@EndPeriod", _volunteerFilters.EndPeriod);

                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[Usp_FilterVolunteers]", CommandType.StoredProcedure, _sqlPara);

                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "SuccessStory";
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "";
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "";
                    _ApiResponse.data = ds.Tables[0];
                }

            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_volunteerFilters),
                   "SettingsSetup", "StudyWiseVolunteers", "[dbo].[Usp_FilterVolunteers]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        public ApiResponse SendEmailSMS(EmailSMS _emailSMS)
        {
            _ApiResponse = new ApiResponse();

            var emailSMSLogModel = new EmailSMSLogModel
            {
                //ModuleName = item.ActionType,
                Subject = _emailSMS.EmailSubject,
                CreatedBy = _emailSMS.CreatedBy,
                OrganizationId = _emailSMS.OrganizationId,
            };

            if (_emailSMS.IsSMS == true)
            {
                if (_emailSMS.FiltersList.Count > 0)
                {
                    foreach (var item in _emailSMS.FiltersList)
                    {
                        if (_emailSMS.IsOverRideCommPre)
                        {
                            if (item.ContactNo != "")
                            {
                                string txt = _emailSMS.SMSBody.Replace("{VolunteerName}", item.VolunteerName);
                                SendMail_SMS.Instance.Sendmessage(item.ContactNo, txt, _emailSMS.OrganizationId, emailSMSLogModel);
                                emailSMSLogModel.MobileNo = item.ContactNo;
                                emailSMSLogModel.VolunteerId = item.VolunteerId;
                                emailSMSLogModel.EmailId = item.EmailId;
                                emailSMSLogModel.ModuleName = "Send bulk email/sms";
                                emailSMSLogModel.Body = txt;
                                emailSMSLogModel.Type = "SMS";
                                EmailSMSLog.Instance.SaveLog(new List<EmailSMSLogModel> { emailSMSLogModel });
                            }
                        }
                        else
                        {
                            if (item.ContactNo != "" && item.IsSMS)
                            {
                                string txt = _emailSMS.SMSBody.Replace("{VolunteerName}", item.VolunteerName);
                                SendMail_SMS.Instance.Sendmessage(item.ContactNo, txt, _emailSMS.OrganizationId, emailSMSLogModel);
                                emailSMSLogModel.MobileNo = item.ContactNo;
                                emailSMSLogModel.VolunteerId = item.VolunteerId;
                                emailSMSLogModel.EmailId = item.EmailId;
                                emailSMSLogModel.ModuleName = "Send bulk email/sms";
                                emailSMSLogModel.Body = txt;
                                emailSMSLogModel.Type = "SMS";
                                EmailSMSLog.Instance.SaveLog(new List<EmailSMSLogModel> { emailSMSLogModel });
                            }
                        }
                    }
                }
            }
            if (_emailSMS.IsEmail == true)
            {
                if (_emailSMS.FiltersList.Count > 0)
                {
                    List<EmailSMSLogModel> emailList = new List<EmailSMSLogModel>();
                    foreach (var item in _emailSMS.FiltersList)
                    {
                        if (_emailSMS.IsOverRideCommPre)
                        {
                            if (item.EmailId != "")
                            {
                                EmailSMSLogModel log = new EmailSMSLogModel();
                                string txt = _emailSMS.EmailBody.Replace("{VolunteerName}", item.VolunteerName);
                                string sub = _emailSMS.EmailSubject.Replace("{VolunteerName}", item.VolunteerName);
                                SendMail_SMS.Instance.sendFeasibilityMail(item.EmailId, sub, txt);
                                //SendMail_SMS.Instance.sendExceptionMail(item.Email, sub, txt);
                                log.MobileNo = "";
                                log.VolunteerId = item.VolunteerId;
                                log.EmailId = item.EmailId;
                                log.ModuleName = "Send bulk email/sms";
                                log.Body = SendMail_SMS.GetAttachmentfromString(txt);
                                log.Type = "Email";
                                log.Subject = sub;
                                log.OrganizationId = _emailSMS.OrganizationId;
                                log.CreatedBy = _emailSMS.CreatedBy;
                                emailList.Add(log);
                            }
                        }
                        else
                        {
                            if (item.EmailId != "" && item.IsEmail)
                            {
                                EmailSMSLogModel log = new EmailSMSLogModel();
                                string txt = _emailSMS.EmailBody.Replace("{VolunteerName}", item.VolunteerName);
                                string sub = _emailSMS.EmailSubject.Replace("{VolunteerName}", item.VolunteerName);
                                SendMail_SMS.Instance.sendFeasibilityMail(item.EmailId, sub, txt);
                                //SendMail_SMS.Instance.sendExceptionMail(item.Email, sub, txt);
                                log.MobileNo = "";
                                log.VolunteerId = item.VolunteerId;
                                log.EmailId = item.EmailId;
                                log.ModuleName = "Send bulk email/sms";
                                log.Body = SendMail_SMS.GetAttachmentfromString(txt);
                                log.Type = "Email";
                                log.Subject = sub;
                                log.OrganizationId = _emailSMS.OrganizationId;
                                log.CreatedBy = _emailSMS.CreatedBy;
                                emailList.Add(log);
                            }
                        }
                    }
                    EmailSMSLog.Instance.SaveLog(emailList);
                }
            }
            _ApiResponse.success = 1;
            if (_emailSMS.IsEmail == true && _emailSMS.IsSMS == true)
            {
                _ApiResponse.message = "Email and SMS has been sent successfully";
            }
            else if (_emailSMS.IsEmail == true && _emailSMS.IsSMS == false)
            {
                _ApiResponse.message = "Email has been sent successfully";
            }
            else if (_emailSMS.IsEmail == false && _emailSMS.IsSMS == true)
            {
                _ApiResponse.message = "SMS has been sent successfully";
            }
            VolunteerEmailSMSReq volunteerEmailSMSReq = new VolunteerEmailSMSReq();
            volunteerEmailSMSReq.EmailTemplateId = _emailSMS.EmailTemplateId;
            volunteerEmailSMSReq.SMSTemplateId = _emailSMS.SMSTemplateId;
            volunteerEmailSMSReq.OrganizationId = _emailSMS.OrganizationId;
            volunteerEmailSMSReq.CreatedBy = _emailSMS.CreatedBy;

            eRecruitmentSetUp.Instance.SaveECommFilters(volunteerEmailSMSReq, _emailSMS.FiltersList.Count);
            return _ApiResponse;
        }
        #endregion

        public ApiResponse GetUserDashboard(UserDashboard _userDashboard)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] _SqlParameter = new SqlParameter[3];
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                _SqlParameter = new SqlParameter[3];
                _SqlParameter[0] = new SqlParameter("@StaffId", _userDashboard.StaffId);
                _SqlParameter[1] = new SqlParameter("@OrganizationId", _userDashboard.OrganizationId);
                _SqlParameter[2] = new SqlParameter("@CreatedBy", _userDashboard.CreatedBy);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[Usp_GetUserDashboard]", CommandType.StoredProcedure, _SqlParameter);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].TableName = "BasicDetails";
                    ds.Tables[1].TableName = "CountData";
                    ds.Tables[2].TableName = "StudyList";
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "";
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                }

            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_userDashboard),
                    "SettingsSetup", "GetUserDashboard", "[dbo].[Usp_GetUserDashboard]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        #region Change organization
        public ApiResponse ChangeOrganizationFun(ChangeOrganizationReq _chorg)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _sqlPara = new SqlParameter[6];
                _sqlPara[0] = new SqlParameter("@StaffId", _chorg.StaffId);
                _sqlPara[1] = new SqlParameter("@OrganizaionId", _chorg.OrganizaionId);
                _sqlPara[2] = new SqlParameter("@EmailId", _chorg.EmailId);
                _sqlPara[3] = new SqlParameter("@OrgType", _chorg.OrgType);
                _sqlPara[4] = new SqlParameter("@SessionKeyOld", _chorg.SessionKey);
                _sqlPara[5] = new SqlParameter("@IP", _chorg.IP);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_ChangeOrganizaion]", CommandType.StoredProcedure, _sqlPara);

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
                    _ApiResponse.message = "Invalid Request";
                }

            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_chorg),
                    "SettingsSetup", "ChangeOrganizationFun", "[dbo].[usp_ChangeOrganizaion]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        public ApiResponse GetMyOrganizationFun(string EmailId)
        {
            _ApiResponse = new ApiResponse();
            DataTable dt = new DataTable();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);

                SqlParameter[] _sqlPara = new SqlParameter[1];
                _sqlPara[0] = new SqlParameter("@EmailId", EmailId);
                dt = _DAL.ExecuteCommand("[dbo].[usp_GetMyOrganization]", CommandType.StoredProcedure, _sqlPara);

                _ApiResponse.success = 1;
                _ApiResponse.message = "My Org List";
                _ApiResponse.data = dt;
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(EmailId),
                    "SettingsSetup", "ChangeOrganizationFun", "[dbo].[usp_GetMyOrganization]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region Study Task Activity

        public ApiResponse StudyTaskActivity(StudyTaskActivity _studyTaskActivity)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] _SqlParameter = new SqlParameter[3];
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                _SqlParameter = new SqlParameter[5];
                _SqlParameter[0] = new SqlParameter("@StudyTaskId", _studyTaskActivity.StudyTaskId);
                _SqlParameter[1] = new SqlParameter("@TaskName", _studyTaskActivity.TaskName);
                _SqlParameter[2] = new SqlParameter("@ActionType", _studyTaskActivity.ActionType);
                _SqlParameter[3] = new SqlParameter("@OrganizationId", _studyTaskActivity.OrganizationId);
                _SqlParameter[4] = new SqlParameter("@CreatedBy", _studyTaskActivity.CreatedBy);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[Usp_Esett_StudyTaskActivities]", CommandType.StoredProcedure, _SqlParameter);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (_studyTaskActivity.ActionType == "GetStudyTaskList")
                    {
                        ds.Tables[0].TableName = "StudyTaskList";
                    }
                    else
                    {
                        ds.Tables[0].TableName = "SuccessStory";
                    }
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "";
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                }

            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_studyTaskActivity),
                    "SettingsSetup", "StudyTaskActivity", "[dbo].[Usp_Esett_StudyTaskActivities]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region Study Task Activity

        public ApiResponse PaymentTemplateActivity(PaymentEmailTemplate _paymentEmailTemplate)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] _SqlParameter = new SqlParameter[3];
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                _SqlParameter = new SqlParameter[7];
                _SqlParameter[0] = new SqlParameter("@TemplateId", _paymentEmailTemplate.TemplateId);
                _SqlParameter[1] = new SqlParameter("@TemplateText", _paymentEmailTemplate.EmailText);
                _SqlParameter[2] = new SqlParameter("@TemplateSubject", _paymentEmailTemplate.EmailSubject);
                _SqlParameter[3] = new SqlParameter("@IsAllowEmail", _paymentEmailTemplate.IsAllowEmail);
                _SqlParameter[4] = new SqlParameter("@CreatedBy", _paymentEmailTemplate.CreatedBy);
                _SqlParameter[5] = new SqlParameter("@OrganizationId", _paymentEmailTemplate.OrganizationId);
                _SqlParameter[6] = new SqlParameter("@ActionType", _paymentEmailTemplate.ActionType);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[Usp_PaymentTemplateActivity]", CommandType.StoredProcedure, _SqlParameter);
                ds.Tables[0].TableName = "SuccessStory";
                _ApiResponse.success = 1;
                _ApiResponse.message = "";
                _ApiResponse.data = ds;
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_paymentEmailTemplate),
                    "SettingsSetup", "PaymentTemplateActivity", "[dbo].[Usp_PaymentTemplateActivity]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region SMS Count
        public ApiResponse SMSCountFun(GetSMSCountReq getSMSCountReq)
        {
            _ApiResponse = new ApiResponse();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[4];
                _SqlParameter[0] = new SqlParameter("@Period", getSMSCountReq.Period);
                _SqlParameter[1] = new SqlParameter("@StartPeriod", getSMSCountReq.StartPeriod);
                _SqlParameter[2] = new SqlParameter("@EndPeriod", getSMSCountReq.EndPeriod);
                _SqlParameter[3] = new SqlParameter("@ActionType", getSMSCountReq.ActionType);
                DataTable dt = _DAL.ExecuteCommand("[dbo].[usp_GetSMSCount]", CommandType.StoredProcedure, _SqlParameter);
                _ApiResponse.success = 1;
                _ApiResponse.message = "SMS Count";
                _ApiResponse.data = dt;
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(getSMSCountReq),
                    "SettingsSetup", "SMSCountFun", "[dbo].[usp_GetSMSCount]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region SMS Count
        public ApiResponse UsersVolunteersMatriciesActivity(UsersVolunteersMatricies _usersVolunteersMatricies)
        {
            string XmlString = string.Empty;
            _ApiResponse = new ApiResponse();
            try
            {
                if (_usersVolunteersMatricies.OrganizationId != null)
                    XmlString = new XElement("ArrayOfString", from c in _usersVolunteersMatricies.OrganizationId select new XElement("string", c)).ToString();//change by arfin ali 05/05/2021
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[6];
                _SqlParameter[0] = new SqlParameter("@OrganizationIdXML", XmlString);
                _SqlParameter[1] = new SqlParameter("@Period", _usersVolunteersMatricies.Period);
                _SqlParameter[2] = new SqlParameter("@StartPeriod", _usersVolunteersMatricies.StartPeriod);
                _SqlParameter[3] = new SqlParameter("@EndPeriod", _usersVolunteersMatricies.EndPeriod);
                _SqlParameter[4] = new SqlParameter("@ActionType", _usersVolunteersMatricies.ActionType);
                _SqlParameter[5] = new SqlParameter("@GroupBy", _usersVolunteersMatricies.GroupBy);
                DataSet ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_GetSiteMetricsData]", CommandType.StoredProcedure, _SqlParameter);
                ds.Tables[0].TableName = "SuccessStory";

                _ApiResponse.success = 1;
                _ApiResponse.message = "SMS Count";
                _ApiResponse.data = ds;
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_usersVolunteersMatricies),
                    "SettingsSetup", "UsersVolunteersMatriciesActivity", "[dbo].[usp_GetSiteMetricsData]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        #region OW analytics
        public ApiResponse GetOWAppointmnetAnalytics(OWAnalytics _OWAnalytics)
        {
            _ApiResponse = new ApiResponse();
            GetSqlParameterFromClassProperty ClsToPara = new GetSqlParameterFromClassProperty();
            DataSet ds = new DataSet();
            try
            {
                string xmlStudy = string.Empty;

                if (_OWAnalytics.StudyIds != null)
                    xmlStudy = new XElement("SelectedStudys", from c in _OWAnalytics.StudyIds
                                                              select new XElement("StudyList", new XElement("StudyId", c.ToUpper()))).ToString();

                string GroupList = string.Empty;
                if (_OWAnalytics.GroupIds != null)
                    GroupList = new XElement("SelectedGroups", from c in _OWAnalytics.GroupIds
                                                               select new XElement("GroupList", new XElement("GroupId", c.ToUpper()))).ToString();

                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[10];
                _SqlParameter[0] = new SqlParameter("@AppointmentPeriod", _OWAnalytics.AppointmentPeriod);
                _SqlParameter[1] = new SqlParameter("@StartPeriod", _OWAnalytics.StartPeriod);
                _SqlParameter[2] = new SqlParameter("@EndPeriod", _OWAnalytics.EndPeriod);
                _SqlParameter[3] = new SqlParameter("@OrganizationId", _OWAnalytics.OrganizationId);
                _SqlParameter[4] = new SqlParameter("@CreatedBy", _OWAnalytics.CreatedBy);
                _SqlParameter[5] = new SqlParameter("@ActionType", _OWAnalytics.ActionType);
                _SqlParameter[6] = new SqlParameter("@StudyIds", xmlStudy);
                _SqlParameter[7] = new SqlParameter("@ViewBy", _OWAnalytics.Viewby);
                _SqlParameter[8] = new SqlParameter("@StudyId", _OWAnalytics.StudyId);
                _SqlParameter[9] = new SqlParameter("@XMLGroupIds", GroupList);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_GetOutWindowAppointmentData]", CommandType.StoredProcedure, _SqlParameter);

                if (ds.Tables.Count > 0)
                {
                    if (_OWAnalytics.ActionType == "GetCountDataforStudy")
                        ds.Tables[0].TableName = "SuccessStory";

                    if (_OWAnalytics.ActionType == "GetDetailsDataforStudy" || _OWAnalytics.ActionType == "GetDetailsDataforStudyOutWindow")
                    {
                        ds.Tables[0].TableName = "SuccessStory";
                        ds.Tables[1].TableName = "DetailsList";
                    }
                    if (_OWAnalytics.ActionType == "GetExcel")
                    {
                        ds.Tables[0].TableName = "SuccessStory";
                    }
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "GetRecords";
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                    _ApiResponse.data = ds;
                }

            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_OWAnalytics), "ePlannerSetUp", "GetAppointment", "[dbo].[usp_GetOutWindowAppointmentData]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion


        #region set Visit Planner
        public ApiResponse StudyLocationVisits(StudyLocationVisits _studyLocationVisits)
        {
            string xmlLocationList = "";
            if (_studyLocationVisits.StudyLocationList != null && _studyLocationVisits.StudyLocationList.Any())
            {
                xmlLocationList = new XElement("SelectedStudyLocations", from c in _studyLocationVisits.StudyLocationList
                                                                         select new XElement("LocationList", new XElement("StudyLocationId", c.StudyLocationId == null ? "" : c.StudyLocationId.ToUpper()),
                                                                         new XElement("LocationId", c.LocationIdRef == null ? "" : c.LocationIdRef.ToUpper())
                                                                         , new XElement("BuildingId", c.BuildingIdRef == null ? "" : c.BuildingIdRef.ToUpper())
                                                                         , new XElement("FloorId", c.FloorIdRef == null ? "" : c.FloorIdRef.ToUpper())
                                                                         , new XElement("RoomId", c.RoomIdRef == null ? "" : c.RoomIdRef.ToUpper())
                                                                         )).ToString();
            }

            _ApiResponse = new ApiResponse();
            GetSqlParameterFromClassProperty ClsToPara = new GetSqlParameterFromClassProperty();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[6];


                _SqlParameter[0] = new SqlParameter("@StudyId", _studyLocationVisits.StudyId);
                _SqlParameter[1] = new SqlParameter("@OrganizationId", _studyLocationVisits.OrganizationId);
                _SqlParameter[2] = new SqlParameter("@CreatedBy", _studyLocationVisits.CreatedBy);
                _SqlParameter[3] = new SqlParameter("@xmStudyLocations", xmlLocationList);
                _SqlParameter[4] = new SqlParameter("@VisitIdRef", _studyLocationVisits.VisitId);
                _SqlParameter[5] = new SqlParameter("@GroupIdRef", _studyLocationVisits.GroupIdRef);

                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_StudyLocationVisits]", CommandType.StoredProcedure, _SqlParameter);

                if (ds.Tables[0].Rows.Count > 0)
                {

                    ds.Tables[0].TableName = "SuccessStory";
                    if (ds.Tables.Count > 1)
                        ds.Tables[1].TableName = "StudyLocationVisits";

                    if (Convert.ToString(ds.Tables[0].Rows[0]["Success"]) == "1")
                    {
                        _ApiResponse.success = 1;
                        _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                        _ApiResponse.data = ds;
                    }
                    else
                    {
                        _ApiResponse.success = 0;
                        _ApiResponse.message = "Try Again";

                    }
                }
            }
            catch (Exception ex)
            {
                return null;
                // SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_studyLocationVisits),
                //"SettingsSetup", "StudyLocationVisits", "[dbo].[usp_StudyLocationVisits]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        public ApiResponse LocationOccupancyAnalyticsActivity(LocationOccupancyAnalytics _locationOccupancyAnalytics)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                string StudyList = string.Empty;
                string GroupList = string.Empty;
                string TypeList = string.Empty;
                string DayList = string.Empty;
                if (_locationOccupancyAnalytics.StudyIdList != null)
                    StudyList = new XElement("SelectedStudys", from c in _locationOccupancyAnalytics.StudyIdList
                                                               select new XElement("StudyList", new XElement("StudyId", c.ToUpper()))).ToString();

                if (_locationOccupancyAnalytics.Type != null)
                    TypeList = new XElement("SelectedTypes", from c in _locationOccupancyAnalytics.Type
                                                             select new XElement("TypeList", new XElement("Type", c.ToUpper()))).ToString();
                if (_locationOccupancyAnalytics.Days != null)
                    DayList = new XElement("SelectedDays", from c in _locationOccupancyAnalytics.Days
                                                           select new XElement("DayList", new XElement("Day", c.ToUpper()))).ToString();

                //if (_analytics.GroupIds != null)
                //    GroupList = new XElement("SelectedGroups", from c in _analytics.GroupIds
                //                                               select new XElement("GroupList", new XElement("GroupId", c.ToUpper()))).ToString();

                SqlParameter[] _sqlPara = new SqlParameter[15];
                _sqlPara[0] = new SqlParameter("@LocationId", _locationOccupancyAnalytics.LocationId);
                _sqlPara[1] = new SqlParameter("@BuildingId", _locationOccupancyAnalytics.BuildingId);
                _sqlPara[2] = new SqlParameter("@FloorId", _locationOccupancyAnalytics.FloorId);
                _sqlPara[3] = new SqlParameter("@RoomId", _locationOccupancyAnalytics.RoomId);
                _sqlPara[4] = new SqlParameter("@Type", TypeList);
                _sqlPara[5] = new SqlParameter("@Period", _locationOccupancyAnalytics.Period);
                _sqlPara[6] = new SqlParameter("@StartPeriod", _locationOccupancyAnalytics.StartPeriod);
                _sqlPara[7] = new SqlParameter("@EndPeriod", _locationOccupancyAnalytics.EndPeriod);
                _sqlPara[8] = new SqlParameter("@StartTime", _locationOccupancyAnalytics.StartTime);
                _sqlPara[9] = new SqlParameter("@EndTime", _locationOccupancyAnalytics.EndTime);
                _sqlPara[10] = new SqlParameter("@Days", DayList);
                _sqlPara[11] = new SqlParameter("@StudyIdList", StudyList);
                _sqlPara[12] = new SqlParameter("@OrganizationId", _locationOccupancyAnalytics.OrganizationId);
                _sqlPara[13] = new SqlParameter("@CreatedBy", _locationOccupancyAnalytics.CreatedBy);
                _sqlPara[14] = new SqlParameter("@IsDNA", _locationOccupancyAnalytics.IsDNA);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[Usp_GetLocationOccupancyAnalytics]", CommandType.StoredProcedure, _sqlPara);

                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "SuccessStory";
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "";
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "";
                    _ApiResponse.data = ds.Tables[0];
                }

            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_locationOccupancyAnalytics),
                   "SettingsSetup", "LocationOccupancyAnalyticsActivity", "[dbo].[Usp_GetLocationOccupancyAnalytics]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        #region Reminder Setup
        public ApiResponse SetReminder(AppointmentReminder _req)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();

            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                string studyDetailsXML = string.Empty;

                if (_req.StudyDetails != null && _req.StudyDetails.Count > 0)
                    studyDetailsXML = GenerateXML.Instance.ToXmlString(_req.StudyDetails);

                if (string.IsNullOrEmpty(_req.CreatedBy))
                    _req.CreatedBy = _req.OrganizationId;

                SqlParameter[] _SqlParameter = new SqlParameter[27];
                _SqlParameter[0] = new SqlParameter("@studyDetailsXML", studyDetailsXML);
                _SqlParameter[1] = new SqlParameter("@ActionType", _req.ActionType);
                _SqlParameter[2] = new SqlParameter("@ReminderName", _req.ReminderName);
                _SqlParameter[3] = new SqlParameter("@ReminderTime", _req.ReminderTime);
                _SqlParameter[4] = new SqlParameter("@ReminderHour", _req.ReminderHour);
                _SqlParameter[5] = new SqlParameter("@ReminderMin", _req.ReminderMin);
                _SqlParameter[6] = new SqlParameter("@ReminderBeforeStartNumber", _req.ReminderBeforeStartNumber);
                _SqlParameter[7] = new SqlParameter("@ReminderBeforeStartSuffix", _req.ReminderBeforeStartSuffix);
                _SqlParameter[8] = new SqlParameter("@ReminderType", _req.ReminderType);
                _SqlParameter[9] = new SqlParameter("@OrganizationId", _req.OrganizationId);
                _SqlParameter[10] = new SqlParameter("@CreatedBy", _req.CreatedBy);
                _SqlParameter[11] = new SqlParameter("@IsActive", _req.IsActive);
                _SqlParameter[12] = new SqlParameter("@IsDeleted", _req.IsDeleted);
                _SqlParameter[13] = new SqlParameter("@ScheduleType", _req.ScheduleType);
                _SqlParameter[14] = new SqlParameter("@StartDate", _req.StartDate);
                _SqlParameter[15] = new SqlParameter("@EndDate", _req.EndDate);
                _SqlParameter[16] = new SqlParameter("@EmailId", _req.EmailId);
                _SqlParameter[17] = new SqlParameter("@Password", _req.Password);
                _SqlParameter[18] = new SqlParameter("@AccessCodeIndex0", _req.AccessCodeIndex0);
                _SqlParameter[19] = new SqlParameter("@AccessCodeIndex1", _req.AccessCodeIndex1);
                _SqlParameter[20] = new SqlParameter("@AccessCodeIndex2", _req.AccessCodeIndex2);
                _SqlParameter[21] = new SqlParameter("@AccessCodeValue0", _req.AccessCodeValue0);
                _SqlParameter[22] = new SqlParameter("@AccessCodeValue1", _req.AccessCodeValue1);
                _SqlParameter[23] = new SqlParameter("@AccessCodeValue2", _req.AccessCodeValue2);
                _SqlParameter[24] = new SqlParameter("@ReminderId", _req.ReminderId);
                _SqlParameter[25] = new SqlParameter("@DeleteReason", _req.Reason);
                _SqlParameter[26] = new SqlParameter("@StudyId", _req.StudyId);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_ESch_SetUpReminder]", CommandType.StoredProcedure, _SqlParameter);

                if (ds != null && (_req.ActionType == "GetReminderList" || _req.ActionType == "DeleteReminder") && ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "SuccessStory";
                    _ApiResponse.success = 1;
                    _ApiResponse.data = ds;
                }
                else if (ds != null && _req.ActionType == "GetReminderStudiesData" && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ds.Tables[0].TableName = "SuccessStory";
                    }
                    ds.Tables[0].TableName = "SuccessStory";
                    _ApiResponse.success = 1;
                    _ApiResponse.data = ds;
                }
                else if (ds != null && _req.ActionType == "UpdateReminder" && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ds.Tables[0].TableName = "SuccessStory";
                    }
                    _ApiResponse.success = 1;
                    _ApiResponse.data = ds;
                }
                else if (ds != null && _req.ActionType == "SaveReminder" && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ds.Tables[0].TableName = "SuccessStory";
                    }
                    _ApiResponse.message = Convert.ToString(ds.Tables[0].Rows[0]["MSG"]);
                    _ApiResponse.success = Convert.ToInt32(ds.Tables[0].Rows[0]["Success"]);
                    _ApiResponse.data = ds;
                }
                else if (ds != null && _req.ActionType == "UpdateStudyMapping" && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ds.Tables[0].TableName = "SuccessStory";
                    }
                    _ApiResponse.success = 1;
                    _ApiResponse.data = ds;
                }
                else if (ds != null && (_req.ActionType == "StartReminder" || _req.ActionType == "StartAllReminder") && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ds.Tables[0].TableName = "SuccessStory";
                    }
                    _ApiResponse.success = 1;
                    _ApiResponse.data = ds;
                }
                else if (ds != null && (_req.ActionType == "StopReminder" || _req.ActionType == "StopAllReminder") && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ds.Tables[0].TableName = "SuccessStory";
                    }
                    _ApiResponse.success = 1;
                    _ApiResponse.data = ds;
                }
                else if (ds != null && _req.ActionType == "GetLog" && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ds.Tables[0].TableName = "TransactionLog";
                        ds.Tables[1].TableName = "SetupLog";
                    }
                    _ApiResponse.success = 1;
                    _ApiResponse.data = ds;
                }
                else if (ds != null && ds.Tables.Count > 0 && _req.ActionType == "GetRemindersByStudyId")
                {
                    ds.Tables[0].TableName = "Reminders";
                    ds.Tables[1].TableName = "GroupVisitList";
                    _ApiResponse.success = 1;
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_req), "ePlannerSetUp", "SetReminder", "[dbo].[usp_ESch_SetUpReminder]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion


        #region CheckReminderNameExists
        public ApiResponse CheckReminderNameExists(AppointmentReminder data)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();

            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);


                SqlParameter[] _SqlParameter = new SqlParameter[3];
                _SqlParameter[0] = new SqlParameter("@ReminderName", data.ReminderName);
                _SqlParameter[1] = new SqlParameter("@ActionType", "CheckReminderNameExists");
                _SqlParameter[2] = new SqlParameter("@OrganizationId", data.OrganizationId);

                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_ESch_CheckReminderNameExists]", CommandType.StoredProcedure, _SqlParameter);

                if (ds != null && ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "SuccessStory";
                    _ApiResponse.success = 1;
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(data), "ePlannerSetUp", "CheckReminderNameExists", "[dbo].[usp_ESch_CheckReminderNameExists]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        //rihan ahmad
        #region StaffDocument
        public ApiResponse SaveStaffDocumentFun(SaveStaffDocuments _saveStaffDocuments)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();

            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[14];
                _SqlParameter[0] = new SqlParameter("@DocumentId", _saveStaffDocuments.DocumentId);
                _SqlParameter[1] = new SqlParameter("@DocumentName", _saveStaffDocuments.DocumentName);
                _SqlParameter[2] = new SqlParameter("@DocumentURL", _saveStaffDocuments.DocumentURL);
                _SqlParameter[3] = new SqlParameter("@ExpiryNumber", _saveStaffDocuments.ExpiryNumber);
                _SqlParameter[4] = new SqlParameter("@DateCertificate", _saveStaffDocuments.DateCertification);
                _SqlParameter[5] = new SqlParameter("@CertificateType", _saveStaffDocuments.CertificateType);
                _SqlParameter[6] = new SqlParameter("@ExpirySuffix", _saveStaffDocuments.ExpirySuffix);
                _SqlParameter[7] = new SqlParameter("@DocumentSize", _saveStaffDocuments.DocumentSize);
                _SqlParameter[8] = new SqlParameter("@StaffId", _saveStaffDocuments.StaffId);
                _SqlParameter[9] = new SqlParameter("@CreatedBy", _saveStaffDocuments.CreatedBy);
                _SqlParameter[10] = new SqlParameter("@OrganizationId", _saveStaffDocuments.OrganizationId);
                _SqlParameter[11] = new SqlParameter("@ActionType", _saveStaffDocuments.ActionType);
                _SqlParameter[12] = new SqlParameter("@StudyId", _saveStaffDocuments.StudyId);
                _SqlParameter[13] = new SqlParameter("@IsActive", _saveStaffDocuments.IsActive);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_StaffDocument]", CommandType.StoredProcedure, _SqlParameter);

                if (ds != null && ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "SuccessStory";
                    _ApiResponse.success = 1;
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_saveStaffDocuments), "SettingsSetup", "SaveStaffDocumentFun", "[dbo].[usp_StaffDocument] ", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        //Arfin Ali 09-May-2023
        public ApiResponse GetMainDashboard(DBBasicActivity _dbba)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[2];
                _SqlParameter[0] = new SqlParameter("@OrganizationId", _dbba.OrganizationId);
                _SqlParameter[1] = new SqlParameter("@CreatedBy", _dbba.CreatedBy);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[Usp_GetMainDashboard]", CommandType.StoredProcedure, _SqlParameter);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].TableName = "SuccessStory";
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "";
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_dbba),
                    "SettingsSetup", "Usp_GetMainDashboard", "[dbo].[Usp_GetMainDashboard]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        //Arfin Ali 09-May-2023
        public ApiResponse StaffRolesSetup(StaffRolesModal _dbba)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[5];
                _SqlParameter[0] = new SqlParameter("@OrganizationId", _dbba.OrganizationId);
                _SqlParameter[1] = new SqlParameter("@CreatedBy", _dbba.CreatedBy);
                _SqlParameter[2] = new SqlParameter("@ActionType", _dbba.ActionType);
                _SqlParameter[3] = new SqlParameter("@Roles", _dbba.Roles);
                _SqlParameter[4] = new SqlParameter("@RoleId", _dbba.RoleId);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[Usp_StaffRolesActivity]", CommandType.StoredProcedure, _SqlParameter);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].TableName = "SuccessStory";
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "";
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_dbba),
                    "SettingsSetup", "StaffRolesSetup", "[dbo].[Usp_StaffRolesActivity]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        //08-Dec-2023
        public ApiResponse UserAccessSetup(UserAccess _userAccess)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[6];
                _SqlParameter[0] = new SqlParameter("@UserId", _userAccess.UserId);
                _SqlParameter[1] = new SqlParameter("@PageId", _userAccess.PageId);
                _SqlParameter[2] = new SqlParameter("@PageActionId", _userAccess.PageActionId);
                _SqlParameter[3] = new SqlParameter("@OrganizationId", _userAccess.OrganizationId);
                _SqlParameter[4] = new SqlParameter("@CreatedBy", _userAccess.CreatedBy);
                _SqlParameter[5] = new SqlParameter("@ActionType", _userAccess.ActionType);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[Usp_UserAccessActivity]", CommandType.StoredProcedure, _SqlParameter);

                if (ds.Tables.Count > 0)
                {
                    if (_userAccess.ActionType == "GetUserAccess" || _userAccess.ActionType == "GetUserMenuAndActions")
                    {
                        ds.Tables[0].TableName = "PageAccess";
                        ds.Tables[1].TableName = "PageActionAccess";
                        _ApiResponse.success = 1;
                        _ApiResponse.message = "";
                        _ApiResponse.data = ds;
                    }
                    else
                    {
                        ds.Tables[0].TableName = "SuccessStory";
                        _ApiResponse.success = 1;
                        _ApiResponse.message = "";
                        _ApiResponse.data = ds;
                    }

                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_userAccess),
                    "SettingsSetup", "StaffRolesSetup", "[dbo].[Usp_StaffRolesActivity]", "Clinical Trail");
            }
            return _ApiResponse;
        }


        public ApiResponse StudyWorkSheetSetup(StudyWorkSheetModel _StudyWorkSheetModel)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[11];
                _SqlParameter[0] = new SqlParameter("@StudyWorkSheetId", _StudyWorkSheetModel.StudyWorkSheetId);
                _SqlParameter[1] = new SqlParameter("@StudyIdRef", _StudyWorkSheetModel.StudyIdRef);
                _SqlParameter[2] = new SqlParameter("@GroupIdRef", _StudyWorkSheetModel.GroupIdRef);
                _SqlParameter[3] = new SqlParameter("@VisitIdRef", _StudyWorkSheetModel.VisitIdRef);
                _SqlParameter[4] = new SqlParameter("@WorkSheetName", _StudyWorkSheetModel.WorkSheetName);
                _SqlParameter[5] = new SqlParameter("@WorkSheetURL", _StudyWorkSheetModel.WorkSheetURL);
                _SqlParameter[6] = new SqlParameter("@OrganizationId", _StudyWorkSheetModel.OrganizationId);
                _SqlParameter[7] = new SqlParameter("@CreatedBy", _StudyWorkSheetModel.CreatedBy);
                _SqlParameter[8] = new SqlParameter("@ActionType", _StudyWorkSheetModel.ActionType);
                _SqlParameter[9] = new SqlParameter("@Reason", _StudyWorkSheetModel.Reason);
                _SqlParameter[10] = new SqlParameter("@IsSameForAllGroup", _StudyWorkSheetModel.IsSameForAllGroup);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[Usp_StudyWorkSheet]", CommandType.StoredProcedure, _SqlParameter);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].TableName = "SuccessStory";
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "";
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "Try Again";
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_StudyWorkSheetModel),
                    "SettingsSetup", "StudyWorkSheetSetup", "[dbo].[Usp_StudyWorkSheet]", "Clinical Trail");
            }
            return _ApiResponse;
        }

        #region analaytics dashboard
        public ApiResponse AnalyticsDashboardActivity(Analytics _analytics)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                string StudyList = string.Empty;


                string GroupList = string.Empty;
                if (_analytics.GroupIds != null)
                    GroupList = new XElement("SelectedGroups", from c in _analytics.GroupIds
                                                               select new XElement("GroupList", new XElement("GroupId", c.ToUpper()))).ToString();

                SqlParameter[] _sqlPara = new SqlParameter[7];
                _sqlPara[0] = new SqlParameter("@StudyId", _analytics.StudyId);
                _sqlPara[1] = new SqlParameter("@AppointmentPeriod", _analytics.Period);
                _sqlPara[2] = new SqlParameter("@StartPeriod", _analytics.StartDate);
                _sqlPara[3] = new SqlParameter("@EndPeriod", _analytics.EndDate);
                _sqlPara[4] = new SqlParameter("@OrganizationId", _analytics.OrganizationId);
                _sqlPara[5] = new SqlParameter("@ActionType", _analytics.ActionType);
                _sqlPara[6] = new SqlParameter("@XMLGroupIds", GroupList);

                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[Usp_GetAnalyticsDashboard]", CommandType.StoredProcedure, _sqlPara);

                if (ds.Tables.Count > 0)
                {

                    ds.Tables[0].TableName = "CohortAppointment";
                    ds.Tables[1].TableName = "AppointmentStatus";
                    ds.Tables[2].TableName = "AppointmentStatusByVisit";
                    ds.Tables[3].TableName = "DeviationReport";
                    ds.Tables[4].TableName = "ParticipantsCompletedVisit";
                    ds.Tables[5].TableName = "ParticipantsWithdrawnScreeningFailed";
                    ds.Tables[6].TableName = "ApprochedParticipants";
                    ds.Tables[7].TableName = "ScreeningFailedParticipants";
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "";
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 0;
                    _ApiResponse.message = "";
                    _ApiResponse.data = null;
                }

            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_analytics),
                    "SettingsSetup", "AnalyticsActivity", "[dbo].[Usp_GetAnalyticsDashboard]", "Clinical Trail");
            }
            return _ApiResponse;
        }
        #endregion

        public ApiResponse UserNotificationSetup(UserNotification _userNotification)
        {
            _ApiResponse = new ApiResponse();
            DataSet ds = new DataSet();
            try
            {
                _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
                SqlParameter[] _SqlParameter = new SqlParameter[4];
                _SqlParameter[0] = new SqlParameter("@NotificationId", _userNotification.NotificationId);
                _SqlParameter[1] = new SqlParameter("@CreatedBy", _userNotification.CreatedBy);
                _SqlParameter[2] = new SqlParameter("@OrganizationId", _userNotification.OrganizationId);
                _SqlParameter[3] = new SqlParameter("@ActionType", _userNotification.ActionType);
                ds = _DAL.ExecuteCommandGetDataSet("[dbo].[usp_NotificationActivity]", CommandType.StoredProcedure, _SqlParameter);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].TableName = "SuccessStory";
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "";
                    _ApiResponse.data = ds;
                }
                else
                {
                    _ApiResponse.success = 1;
                    _ApiResponse.message = "Try Again";
                    ds.Tables[0].TableName = "SuccessStory";
                    _ApiResponse.data = ds;
                }
            }
            catch (Exception ex)
            {
                SendMail_SMS.Instance.sendExceptionMail(Convert.ToString(ex.Message) + "<br />" + ex.InnerException + "<br /> RequestObject: " + JsonConvert.SerializeObject(_userNotification),
                    "SettingsSetup", "UserNotificationSetup", "[dbo].[usp_NotificationActivity]", "Clinical Trail");
            }
            return _ApiResponse;
        }
    }
}