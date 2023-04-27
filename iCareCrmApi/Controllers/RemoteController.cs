using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using iCareCrmApi.Ado;
using iCareCrmApi.Filters;
using iCareCrmApi.Handle;
using System.Web.Http.Cors;
using iCareCrmApi.Models;
using System.Data;
using System.Web;
using System.Security.Cryptography;

namespace iCareCrmApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [CorsOnActionHandle]
    [Access_Filter]
    [RoutePrefix("api/remote")]
    public class RemoteController : ApiController
    {
        #region 

        RemoteTypeAdo rType = new RemoteTypeAdo();
        RemoteInfoAdo rInfo = new RemoteInfoAdo();
        #endregion
        #region GET
        #region 連線工具列表
        [HttpGet]
        [Route("type/list")]
        public IHttpActionResult RemoteTypeList()
        {
            ResultModel result = new ResultModel();

            try
            {
                #region get value
                
                #endregion
                #region check value
                bool ColFlag = true;
                string ErrorMsg = string.Empty;
                #endregion
                #region main
                if (ColFlag)
                {
                    List<RemoteTypeModel> list = new List<RemoteTypeModel>();

                    DataTable dt = rType.QueryRemoteType();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            RemoteTypeModel RTM = new RemoteTypeModel();
                            RTM.id = dr["RTID"].ToString();
                            RTM.type = dr["RTName"].ToString();
                            list.Add(RTM);
                        }
                    }

                    result.status = true;
                    result.error = "";
                    result.code = 200;
                    result.data = list;
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
        #region 診所連線資訊列表
        [HttpGet]
        [Route("info/list")]
        public IHttpActionResult ClinicList()
        {
            ResultModel result = new ResultModel();

            try
            {
                #region get value
                string CLINIC = HttpContext.Current.Request.QueryString["clinic"] != null ? HttpContext.Current.Request.QueryString["clinic"].ToString() : "";
                #endregion
                #region check value
                bool ColFlag = true;
                string ErrorMsg = string.Empty;
                if (string.IsNullOrWhiteSpace(CLINIC)) { ColFlag = false; ErrorMsg = "請確認診所編號"; }
                #endregion
                #region main
                if (ColFlag)
                {
                    List<RemoteInfoModel> list = new List<RemoteInfoModel>();

                    string CId = CLINIC.Trim();

                    DataTable dt = rInfo.GetRemoteIInfo(CId);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            RemoteInfoModel RIM = new RemoteInfoModel();
                            RIM.id = dr["RID"].ToString();
                            RIM.device = dr["RDevice"].ToString();
                            RIM.type = dr["RType"].ToString();
                            RIM.number = dr["RNumber"].ToString();
                            RIM.pwd = dr["RPwd"].ToString();

                            list.Add(RIM);
                        }
                    }

                    result.status = true;
                    result.error = "";
                    result.code = 200;
                    result.data = list;
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

        #region POST
        #region 編輯診所連線資訊
        [HttpPost]
        [Route("info/action")]
        public IHttpActionResult ClinicInfoEdit()
        {
            ResultModel result = new ResultModel();

            try
            {
                #region get value
                string REMOTEID = HttpContext.Current.Request.Form["remote_id"] != null ? HttpContext.Current.Request.Form["remote_id"].ToString() : "";
                string CLINIC = HttpContext.Current.Request.Form["clinic_id"] != null ? HttpContext.Current.Request.Form["clinic_id"].ToString() : "";
                string ACTION = HttpContext.Current.Request.Form["action"] != null ? HttpContext.Current.Request.Form["action"].ToString() : "";
                string DEVICE = HttpContext.Current.Request.Form["device"] != null ? HttpContext.Current.Request.Form["device"].ToString() : "";
                string TYPE = HttpContext.Current.Request.Form["type"] != null ? HttpContext.Current.Request.Form["type"].ToString() : "";
                string NUMBER = HttpContext.Current.Request.Form["number"] != null ? HttpContext.Current.Request.Form["number"].ToString() : "";
                string PWD = HttpContext.Current.Request.Form["pwd"] != null ? HttpContext.Current.Request.Form["pwd"].ToString() : "";
                
                #endregion
                #region check value

                bool ColFlag = true;
                List<string> listError = new List<string>();

                int RemoteId;
                int.TryParse(REMOTEID, out RemoteId);

                switch (ACTION.Trim().ToLower())
                {
                    case "add":
                        if (string.IsNullOrWhiteSpace(CLINIC.Trim())) { ColFlag = false; listError.Add("請確認機構代碼"); }
                        break;
                    case "edit":
                        if(RemoteId < 1) { ColFlag = false; listError.Add("請確認連線資訊ID"); }
                        if (string.IsNullOrWhiteSpace(DEVICE.Trim())) { ColFlag = false; listError.Add("請確認設備名稱"); }
                        if (string.IsNullOrWhiteSpace(TYPE.Trim())) { ColFlag = false; listError.Add("請確認連線工具"); }
                        if (string.IsNullOrWhiteSpace(NUMBER.Trim())) { ColFlag = false; listError.Add("請確認連線編號"); }
                        break;
                    case "del":
                        if (RemoteId < 1) { ColFlag = false; listError.Add("請確認連線資訊ID"); }
                        break;
                    default:
                        ColFlag = false; listError.Add("請確認行動類別");
                        break;
                }
                
                #endregion
                #region main
                if (ColFlag)
                {
                    string CId = CLINIC.Trim();


                    switch (ACTION.Trim().ToLower())
                    {
                        case "add":
                            rInfo.AddRemoteIInfo(CId, DEVICE, TYPE, NUMBER, PWD);
                            break;
                        case "edit":
                            rInfo.UpdRemoteIInfo(RemoteId, DEVICE, TYPE, NUMBER, PWD);
                            break;
                        case "del":
                            rInfo.UpdRemoteIInfo(RemoteId, false);
                            break;
                    }

                    result.status = true;
                    result.error = "";
                    result.code = 200;
                    result.data = null;
                }
                else
                {
                    result.status = false;
                    result.error = string.Join(",",listError).Replace(",","</br>");
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
