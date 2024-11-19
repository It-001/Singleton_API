using ClinicalTrialAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ClinicalTrialAPI.Custom
{
    public class ValidateModel
    {
        #region Singleton Object
        private ValidateModel() { }
        private static readonly Lazy<ValidateModel> lazy = new Lazy<ValidateModel>(() => new ValidateModel());
        public static ValidateModel Instance { get { return lazy.Value; } }
        #endregion
        public List<string> Validate(StaffRegistration PR)
        {
            List<string> vm = new List<string>();
            if (PR.Action == "Insert" && PR.Action == "Update")
            {
                if (string.IsNullOrEmpty(PR.FirstName))
                {
                    vm.Add("First Name is a required field");
                }
                if (string.IsNullOrEmpty(PR.LastName))
                {
                    vm.Add("Surname is a required field.");
                }
                if (string.IsNullOrEmpty(PR.EmployeeNum))
                {
                    vm.Add("Employee No is a required field.");
                }
                if (string.IsNullOrEmpty(PR.RoleIdRef))
                {
                    vm.Add("Staff Role is a required field.");
                }
                if (string.IsNullOrEmpty(PR.StartDate))
                {
                    vm.Add("Start Date is a required field.");
                }
                if (string.IsNullOrEmpty(PR.AccessDate))
                {
                    vm.Add("Access Date is a required field.");
                }
                if (string.IsNullOrEmpty(PR.Gender))
                {
                    vm.Add("Gender is a required field.");
                }
                if (string.IsNullOrEmpty(PR.EmailId))
                {
                    vm.Add("EmailId is a required field.");
                }
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(PR.EmailId);
                if (!match.Success)
                {
                    vm.Add(PR.EmailId + " is Invalid Email Address");
                }
                if (string.IsNullOrEmpty(PR.DOB))
                {
                    vm.Add("Date of birth is a required field.");
                }
                if (string.IsNullOrEmpty(PR.LineManagerId))
                {
                    vm.Add("Line Manageris a required field.");
                }
            }
            else if (PR.Action == "GetPendingStudyRole")
            {
                if (string.IsNullOrEmpty(PR.EmailId))
                {
                    vm.Add("EmailId is a required field.");
                }
            }
            else if (PR.Action == "Archive" || PR.Action == "UserArchive" || PR.Action == "Approve" || PR.Action == "Unarchive")
            {
                if (string.IsNullOrEmpty(PR.CreatedById))
                {
                    vm.Add("CreatedById is a required field.");
                }
                if (string.IsNullOrEmpty(PR.StaffId))
                {
                    vm.Add("StaffId is a required field.");
                }
            }
            else if (string.IsNullOrEmpty(PR.Action))
            {
                vm.Add("Action Type is a required field");
            }
            return vm;
        }

        public List<string> Validate(StudyDetails _studyDetails)
        {
            List<string> vm = new List<string>();
            if (string.IsNullOrEmpty(_studyDetails.OrganizationId))
            {
                vm.Add("OrganizationId is a required field");
            }
            if (_studyDetails.Action == "Insert" || _studyDetails.Action == "Update")
            {
                if (string.IsNullOrEmpty(_studyDetails.Study))
                {
                    vm.Add("Study Title is a required field");
                }
                if (string.IsNullOrEmpty(_studyDetails.StudyNum))
                {
                    vm.Add("Study Number is a required field");
                }
                if (string.IsNullOrEmpty(_studyDetails.PI))
                {
                    vm.Add("Principle Investigator is a required field");
                }
                if (_studyDetails.StaffIdList==null)
                {
                    vm.Add("Select Staff is a required field");
                }
                if (_studyDetails.StudyTaskList == null)
                {
                    vm.Add("Study Task is a required field");
                }
                
            }
            else if(_studyDetails.Action == "Archive" || _studyDetails.Action == "Unarchive" || _studyDetails.Action == "InActiveStudy" || _studyDetails.Action == "RejectArchiveStudyReq")
            {
                if (string.IsNullOrEmpty(_studyDetails.StudyId))
                {
                    vm.Add("StudyId is a required field");
                }
                if (string.IsNullOrEmpty(_studyDetails.CreatedBy))
                {
                    vm.Add("CreatedBy is a required field");
                }
            }
            else if (string.IsNullOrEmpty(_studyDetails.Action))
            {
                vm.Add("Action Type is a required field");
            }
            return vm;
        }
    }
   
}