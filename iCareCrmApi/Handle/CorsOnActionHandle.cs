using iCareCrmApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace iCareCrmApi.Handle
{
    public class CorsOnActionHandle : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            ResultModel result = new ResultModel();
            try
            {
                //設定允許的網域清單
                List<string> strAllowDomain = new List<string>()
                {
                    "https://mis.sprinf.com",
                    "http://mis.sprinf.com",
                    "https://localhost:44326",
                    "http://localhost:44326",
                    "http://192.168.1.110:3000",
                    "https://192.168.1.110:3000",
                    "http://localhost:3000",
                    "https://localhost:3000"
                };

                    // 取出來自呼叫端的網域
                string strOrigin = actionContext.Request.Headers.GetValues("Origin").FirstOrDefault();
                // 確認呼叫端的網域是否存在於允許的清單中
                bool blCheckDomain = strAllowDomain.Contains(strOrigin);
                // 如果不存在允許的網域清單，就回傳自訂的錯誤訊息
                if (!blCheckDomain)
                {
                    result.status = false;
                    result.code = 401;
                    result.error = "domain is not allow";
                    result.data = null;
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, result);
                }
            }
            catch(Exception ex)
            {
                result.status = false;
                result.code = 401;
                result.error = ex.Message;
                result.data = null;
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, result);
            }
        }
    }
}