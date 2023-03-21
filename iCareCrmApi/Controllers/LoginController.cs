using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Http;

using iCareCrmApi.Models;
using iCareCrmApi.Ado;
using iCareCrmApi.Class;
using iCareCrmApi.Handle;
using System.Web.Http.Cors;

namespace iCareCrmApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [CorsOnActionHandle]
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        #region 
        LoginClass loginC = new LoginClass();

        UsersInfoAdo uInfo = new UsersInfoAdo();
        #endregion
        #region POST
        #region CRM登入
        [HttpPost]
        public IHttpActionResult ULogin()
        {
            ResultModel result = new ResultModel();

            try
            {
                #region get value
                string Id = HttpContext.Current.Request.Form["userID"] != null ? HttpContext.Current.Request.Form["userID"].ToString() : "";
                string Pwd = HttpContext.Current.Request.Form["password"] != null ? HttpContext.Current.Request.Form["password"].ToString() : "";
                #endregion
                #region check value
                bool ColFlag = true;
                string ErrorMsg = string.Empty;
                if (string.IsNullOrWhiteSpace(Id)) { ColFlag = false; ErrorMsg = "請確認帳戶"; }
                if (string.IsNullOrWhiteSpace(Pwd)) { ColFlag = false; ErrorMsg = "請確認密碼"; }
                #endregion
                #region main
                if (ColFlag)
                {
                    DataTable dt = uInfo.GetUsersInfo(Id.Trim());
                    if(dt != null && dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0]["UPassword"].ToString() == Pwd.Trim())
                        {
                            LoginModel LM = new LoginModel();
                            LM.token = loginC.ConvertToken(dt.Rows[0]["UID"].ToString());

                            result.status = true;
                            result.error = "";
                            result.code = 200;
                            result.data = LM;
                        }
                        else
                        {
                            result.status = false;
                            result.error = "請確認密碼是否正確";
                            result.code = 403;
                            result.data = null;
                        }
                    }
                    else
                    {
                        result.status = false;
                        result.error = "請確認帳戶是否正確";
                        result.code = 403;
                        result.data = null;
                    }
                }
                else
                {
                    result.status = false;
                    result.error = ErrorMsg;
                    result.code = 403;
                    result.data = null;
                }
                #endregion
            }
            catch (Exception ex)
            {
                result.status = false;
                result.error = ex.Message;
                result.code = 403;
                result.data = null;
            }

            return Ok(result);
        }
        #endregion
        #endregion

    }
}
