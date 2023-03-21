using iCareCrmApi.Ado;
using iCareCrmApi.Models;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Web;
using System.Web.Http.Results;


namespace iCareCrmApi.Class
{
    public class LoginClass
    {

        #region 
        

        BaseUtilty BU = new BaseUtilty();
        CryptoKeyClass CKC = new CryptoKeyClass();
        UsersInfoAdo uInfo = new UsersInfoAdo();
        #endregion
        public string ConvertToken(string UID)
        {
            string result= string.Empty;

            #region 
            string UId = UID.Trim().ToUpper();
            //產生不重覆字串
            string ACCESSTOKEN = Guid.NewGuid().ToString();
            //到期日
            string UTCEXPRIED = Convert.ToInt32((DateTime.UtcNow.AddDays(30) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds).ToString();

            #region update usersinfo
            uInfo.UpdLastLogin(UId, ACCESSTOKEN, UTCEXPRIED);
            #endregion

            #region token Encrypt
            LoginTokenModel LTM = new LoginTokenModel();
            LTM.token = ACCESSTOKEN;
            LTM.expried = UTCEXPRIED;
            LTM.id = UId;

            string jsonToken = JsonConvert.SerializeObject(LTM);
            string EnCodeJson = BU.aesEncryptBase64(jsonToken, CKC.CRMCryptoKey);
            string UrlEnStr = Uri.EscapeDataString(EnCodeJson);
            #endregion

            #endregion

            result = UrlEnStr;

            return result; 
        }
        public LoginTokenModel DesConvertToken()
        {
            LoginTokenModel LTM = new LoginTokenModel();

            #region get
            string authHeader = HttpContext.Current.Request.Headers["Authorization"];

            #endregion
            #region check
            if (authHeader != null && authHeader.StartsWith("Bearer"))
            {
                string token = authHeader.Substring("Bearer".Length).Trim().Replace("{", "").Replace("}", "");

                string CKValue = Uri.UnescapeDataString(token);
                string DeCKValue = BU.aesDecryptBase64(CKValue, CKC.CRMCryptoKey);
                if (!string.IsNullOrWhiteSpace(DeCKValue))
                {
                    LTM = JsonConvert.DeserializeObject<LoginTokenModel>(DeCKValue);
                }
            }
            #endregion

            return LTM;
        }
    }
}