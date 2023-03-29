using System;
using System.Data;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using Newtonsoft.Json;

using iCareCrmApi.Class;
using iCareCrmApi.Ado;
using iCareCrmApi.Models;

namespace iCareCrmApi.Filters
{
    public class Access_Filter : AuthorizationFilterAttribute
    {
        #region 
        BaseUtilty BU = new BaseUtilty();
        CryptoKeyClass CKC = new CryptoKeyClass();
        UsersInfoAdo uInfo = new UsersInfoAdo();
        #endregion
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            ResultModel result = new ResultModel();
            bool ColFlag = true;
            string ErrorMsg = string.Empty;
            try
            {
                
                #region get
                string authHeader = HttpContext.Current.Request.Headers["Authorization"];

                #endregion
                #region check
                if (authHeader != null && authHeader.StartsWith("Bearer"))
                {
                    string token = authHeader.Substring("Bearer".Length).Trim().Replace("{", "").Replace("}", "");

                    string CKValue = Uri.UnescapeDataString(token);
                    string DeCKValue = BU.aesDecryptBase64(CKValue, CKC.CRMCryptoKey);
                    if (string.IsNullOrWhiteSpace(DeCKValue))
                    {
                        result.status = false;
                        result.error = "授權金鑰驗證失敗";
                        result.code = 601;
                        result.data = null;

                        ColFlag = false;
                        
                    }
                    else
                    {
                        LoginTokenModel LTM = JsonConvert.DeserializeObject<LoginTokenModel>(DeCKValue);
                        #region 
                        int Expired = Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
                        DataTable dt= uInfo.GetUsersInfoByToken(LTM.token);
                        if (dt != null && dt.Rows.Count > 0)
                        {

                        }
                        else
                        {
                            result.status = false;
                            result.error = "授權金鑰驗證失敗";
                            result.code = 601;
                            result.data = null;

                            ColFlag = false;
                        }
                        #endregion
                    }
                }
                else
                {
                    result.status = false;
                    result.error = "授權金鑰驗證失敗";
                    result.code = 601;
                    result.data = null;

                    ColFlag = false;
                }
                #endregion
                
            }
            catch (Exception ex)
            {
                result.status = false;
                result.error = "授權金鑰驗證失敗";
                result.code = 601;
                result.data = null;

                ColFlag = false;
            }

            if (!ColFlag)
            {
                actionContext.Response = actionContext.Request.CreateResponse(result);
            }
            
        }
    }
}