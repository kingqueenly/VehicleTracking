using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using VT.Model;
using VT.WebService.Requests;
using System.Web.Script.Services;

namespace VT.WebService
{
    [WebService(Namespace = "VehicleTracking.WebService.VehicleManager")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    //[System.Web.Script.Services.ScriptService]
    public class VehicleManager : System.Web.Services.WebService
    {
        public CredentialSoapHeader credentialSoapHeader = new CredentialSoapHeader();

        [SoapHeader("credentialSoapHeader")]
        [WebMethod(Description = "", EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public UserInformation GetLocatins(GetLocationsRequest request)
        {
            UserInformation userInfo = new UserInformation();
            bool check = credentialSoapHeader.CheckUsernameAndPassword();
            userInfo.Name = "李奎";
            userInfo.Username = "kui.li";
            userInfo.Password = "123456";
            return userInfo;
        }

        [SoapHeader("credentialSoapHeader")]
        [WebMethod(Description = "", EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string Test()
        {
            return "hello";
        }
    }
}
