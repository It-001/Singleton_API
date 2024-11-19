using ClinicalTrialAPI.Models;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace ClinicalTrialAPI.eDelegation
{
    /*
    public class CustomAuthorizeAttribute: AuthorizeAttribute
    {
        private const string BasicAuthResponseHeader = "WWW-Authenticate";
        private const string BasicAuthResponseHeaderValue = "Basic";


        public string UsersConfigKey { get; set; }
        public string RolesConfigKey { get; set; }

        protected CustomPrincipal CurrentUser
        {
            get { return Thread.CurrentPrincipal as CustomPrincipal; }
            set { Thread.CurrentPrincipal = value as CustomPrincipal; }
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            DataContext Context = new DataContext();
            List<User> objuser = new List<User>();
            #region
            objuser.Add(new User()
            {
                Username = "noreply@sidqam.com",
                Password = "45Sidqam67"
                // Roles="Admin"
            });
            #endregion
            Context.Users = objuser;
            try
            {
                AuthenticationHeaderValue authValue = actionContext.Request.Headers.Authorization;

                if (authValue != null && !String.IsNullOrWhiteSpace(authValue.Parameter) && authValue.Scheme == BasicAuthResponseHeaderValue)
                {
                    Credentials parsedCredentials = ParseAuthorizationHeader(authValue.Parameter);

                    if (parsedCredentials != null)
                    {
                        var user = Context.Users.Where(u => u.Username == parsedCredentials.Username && u.Password == parsedCredentials.Password).FirstOrDefault();
                        if (user != null)
                        {

                        }
                        else
                        {
                            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                            actionContext.Response.Headers.Add(BasicAuthResponseHeader, BasicAuthResponseHeaderValue);
                            return;
                        }
                    }
                    else
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                        actionContext.Response.Headers.Add(BasicAuthResponseHeader, BasicAuthResponseHeaderValue);
                        return;
                    }
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                    actionContext.Response.Headers.Add(BasicAuthResponseHeader, BasicAuthResponseHeaderValue);
                    return;
                }

            }
            catch (Exception)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                actionContext.Response.Headers.Add(BasicAuthResponseHeader, BasicAuthResponseHeaderValue);
                return;

            }
        }

        private Credentials ParseAuthorizationHeader(string authHeader)
        {
            string[] credentials = Encoding.ASCII.GetString(Convert.FromBase64String(authHeader)).Split(new[] { ':' });

            if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[1]))
                return null;

            return new Credentials() { Username = credentials[0], Password = credentials[1], };
        }
    }
    public class Credentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class DataContext
    {

        public List<User> Users { get; set; }
    }
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        // public string Roles { get; set; }
    }
    #region CustomPrincipal
    public class CustomPrincipal : IPrincipal
    {
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role)
        {
            if (roles.Any(r => role.Contains(r)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public CustomPrincipal(string Username)
        {
            this.Identity = new GenericIdentity(Username);
        }

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string[] roles { get; set; }
    }

    #endregion

    */
   
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private DAL _DAL;
        private const string BasicAuthResponseHeader = "WWW-Authenticate";
        private const string BasicAuthResponseHeaderValue = "Basic";
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Method.ToString() == "OPTIONS")
            {
                // do nothing let IIS deal with reply!
                actionContext.Response = null;
            }
            string sty = actionContext.Request.Method.ToString();
            try
            {
                AuthenticationHeaderValue authValue = actionContext.Request.Headers.Authorization;
                if (authValue != null && !String.IsNullOrWhiteSpace(authValue.Parameter) && authValue.Scheme == BasicAuthResponseHeaderValue)
                {
                    Credentials parsedCredentials = ParseAuthorizationHeader(authValue.Parameter);
                    if (parsedCredentials != null)
                    {
                        if (actionContext.Request.Method.ToString() == "OPTIONS")
                        {
                            // do nothing let IIS deal with reply!
                            actionContext.Response = null;
                        }
                        else
                        {
                            CheckLoginRes obj = CheckSessionKey(parsedCredentials.Username, parsedCredentials.Password, actionContext);
                            if (obj.Status)
                            {
                                return;
                            }
                            else
                            {
                                ApiResponse _ApiResponse = new ApiResponse();
                                _ApiResponse.success = -2;
                                _ApiResponse.message = obj.MSG;

                                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, _ApiResponse, System.Net.Http.Formatting.JsonMediaTypeFormatter.DefaultMediaType);
                                actionContext.Response.Headers.Add(BasicAuthResponseHeader, BasicAuthResponseHeaderValue);
                                return;
                            }
                        }
                    }
                    else
                    {
                        ApiResponse _ApiResponse = new ApiResponse();
                        _ApiResponse.success = -2;
                        _ApiResponse.message = "Unauthorized Action, Session expired";

                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, _ApiResponse, System.Net.Http.Formatting.JsonMediaTypeFormatter.DefaultMediaType);
                        actionContext.Response.Headers.Add(BasicAuthResponseHeader, BasicAuthResponseHeaderValue);
                        return;
                    }
                }
                else
                {
                    ApiResponse _ApiResponse = new ApiResponse();
                    _ApiResponse.success = -2;
                    _ApiResponse.message = "Unauthorized Action, Session expired";

                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, _ApiResponse, System.Net.Http.Formatting.JsonMediaTypeFormatter.DefaultMediaType);
                    actionContext.Response.Headers.Add(BasicAuthResponseHeader, BasicAuthResponseHeaderValue);
                    return;
                }

            }
            catch (Exception ex)
            {
                ApiResponse _ApiResponse = new ApiResponse();
                _ApiResponse.success = -2;
                _ApiResponse.message = "Unauthorized Action";

                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, _ApiResponse, System.Net.Http.Formatting.JsonMediaTypeFormatter.DefaultMediaType);
                actionContext.Response.Headers.Add(BasicAuthResponseHeader, BasicAuthResponseHeaderValue);
                return;

            }
        }

        private Credentials ParseAuthorizationHeader(string authHeader)
        {
            string[] credentials = Encoding.ASCII.GetString(Convert.FromBase64String(authHeader)).Split(new[] { ':' });

            if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[1]))
                return null;

            return new Credentials() { Username = credentials[0], Password = credentials[1], };
        }
        private CheckLoginRes CheckSessionKey(string EmailId, string SessionKey, HttpActionContext actionContext)
        {
            CheckLoginRes obj = new CheckLoginRes();

            string rawRequest = "", RequestIp = "";

            using (var stream = new MemoryStream())
            {
                var context = (HttpContextBase)actionContext.Request.Properties["MS_HttpContext"];
                context.Request.InputStream.Seek(0, SeekOrigin.Begin);
                context.Request.InputStream.CopyTo(stream);
                rawRequest = Encoding.UTF8.GetString(stream.ToArray());
                RequestIp = context.Request.UserHostAddress;
            }

            string Method = actionContext.Request.Method.ToString();
            string URL = actionContext.Request.RequestUri.ToString();
            _DAL = new DAL(WebConfigurationManager.AppSettings["ClinicalTrailDB"]);
            //DBAccess _DBAccess = new DBAccess(WebConfigurationManager.AppSettings["CurrentDBString"]);
            SqlParameter[] _SqlParameter = new SqlParameter[6];
            _SqlParameter[0] = new SqlParameter("@EmailId", EmailId);
            _SqlParameter[1] = new SqlParameter("@SessionKey", SessionKey);
            _SqlParameter[2] = new SqlParameter("@Method", Method);
            _SqlParameter[3] = new SqlParameter("@URL", URL);
            _SqlParameter[4] = new SqlParameter("@Data", rawRequest);
            _SqlParameter[5] = new SqlParameter("@RequestIp", RequestIp);
            DataTable dt = _DAL.ExecuteCommand("[dbo].[usp_getSessionKey]", CommandType.StoredProcedure, _SqlParameter);
            if (dt.Rows.Count > 0)
            {
                if (Convert.ToString(dt.Rows[0]["Response"]) == "true")
                {
                    obj.Status = true;
                    obj.MSG = Convert.ToString(dt.Rows[0]["MSG"]);
                }
                else
                {
                    obj.Status = false;
                    obj.MSG = Convert.ToString(dt.Rows[0]["MSG"]);
                }

            }
            else
            {
                obj.Status = false;
                obj.MSG = "Unauthorized Request";
            }

            return obj;
        }
    }
    public class Credentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class CheckLoginRes
    {
        public bool Status { get; set; }
        public string MSG { get; set; }
    }
}