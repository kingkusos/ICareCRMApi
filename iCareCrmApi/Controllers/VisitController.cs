using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using iCareCrmApi.Models;
using iCareCrmApi.Ado;
using iCareCrmApi.Class;
using iCareCrmApi.Handle;
using System.Web.Http.Cors;
using iCareCrmApi.Filters;

namespace iCareCrmApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [CorsOnActionHandle]
    [Access_Filter]
    [RoutePrefix("api/visit")]
    
    public class VisitController : ApiController
    {
        #region 
        LoginClass loginC = new LoginClass();

        VisitLogAdo vLog = new VisitLogAdo();
        VisitEditLogAdo veLog = new VisitEditLogAdo();
        ClinicInfoAdo cInfo = new ClinicInfoAdo();
        #endregion

        #region GET
        #region 診所拜訪紀錄
        [HttpGet]
        [Route("log/list/{page}")]
        
        public IHttpActionResult VisitLogList(int page)
        {
            ResultModel result = new ResultModel();

            try
            {
                #region get value
                string CLINIC = HttpContext.Current.Request.QueryString["clinic"] != null ? HttpContext.Current.Request.QueryString["clinic"].ToString() : "";
                string FILTERCONTENT = HttpContext.Current.Request.QueryString["filter_content"] != null ? HttpContext.Current.Request.QueryString["filter_content"].ToString() : "";
                #endregion
                #region check value
                bool ColFlag = true;
                string ErrorMsg = string.Empty;
                if (string.IsNullOrWhiteSpace(CLINIC)) { ColFlag = false; ErrorMsg = "請確認診所編號"; }

                #endregion
                #region main
                if (ColFlag)
                {
                    VisitLogPageModel VLP = new VisitLogPageModel();
                    
                    string CId = CLINIC.Trim();
                    DataTable dt = vLog.GetVisitLogByCId(page, 5, CId, FILTERCONTENT);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        VLP.total = int.Parse(dt.Rows[0]["TotalPage"].ToString());
                        VLP.totalpage = int.Parse(dt.Rows[0]["TotalCnt"].ToString());
                        foreach (DataRow dr in dt.Rows)
                        {
                            VisitLogModel VLM = new VisitLogModel();
                            VLM.id = dr["LogID"].ToString();
                            VLM.visitor_id = dr["UserID"].ToString();
                            VLM.visitor_name = dr["UNickName"].ToString();
                            VLM.content = dr["VisitContent"].ToString();
                            VLM.visit_datetime = DateTime.Parse(dr["VisitTime"].ToString()).ToString("yyyy/MM/dd HH:mm");
                            VLM.now_datetime = DateTime.Parse(dr["CreateTime"].ToString()).ToString("yyyy/MM/dd HH:mm");
                            VLM.isApproval = bool.Parse(dr["IsApproved"].ToString());
                            VLM.visit_category = dr["VisitType"].ToString();
                            VLM.clinic_status = dr["CType"].ToString();
                            VLP.list.Add(VLM);
                        }
                    }
                    result.status = true;
                    result.error = "";
                    result.code = 200;
                    result.data = VLP;
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
        #region 新增編輯拜訪紀錄
        [HttpPost]
        [Route("log/action")]
        public IHttpActionResult VisitLogAction()
        {
            ResultModel result = new ResultModel();

            try
            {
                #region get value
                string CATEGORY = HttpContext.Current.Request.Form["visit_category"] != null ? HttpContext.Current.Request.Form["visit_category"].ToString() : "";
                string LOGID = HttpContext.Current.Request.Form["log_id"] != null ? HttpContext.Current.Request.Form["log_id"].ToString() : "";
                string CLINICID = HttpContext.Current.Request.Form["clinic_id"] != null ? HttpContext.Current.Request.Form["clinic_id"].ToString() : "";
                string VISITTIME = HttpContext.Current.Request.Form["visit_datetime"] != null ? HttpContext.Current.Request.Form["visit_datetime"].ToString() : "";
                string DESC = HttpContext.Current.Request.Form["description"] != null ? HttpContext.Current.Request.Form["description"].ToString() : "";
                string ACTION = HttpContext.Current.Request.Form["action"] != null ? HttpContext.Current.Request.Form["action"].ToString() : "";
                string CLINICSTATUS = HttpContext.Current.Request.Form["clinic_status"] != null ? HttpContext.Current.Request.Form["clinic_status"].ToString() : "";
                #endregion
                #region check value
                LoginTokenModel LTM = loginC.DesConvertToken();
                bool ColFlag = true;
                string ErrorMsg = string.Empty;
                if (string.IsNullOrWhiteSpace(CATEGORY)) { ColFlag = false; ErrorMsg = "請確認狀態類別"; }


                if (string.IsNullOrWhiteSpace(VISITTIME)) { ColFlag = false; ErrorMsg = "請確認拜訪時間"; }
                else
                {
                    DateTime VisitTime;
                    if (!DateTime.TryParse(VISITTIME, out VisitTime)) { ColFlag = false; ErrorMsg = "請確認拜訪時間"; }
                }

                if (string.IsNullOrWhiteSpace(ACTION)) { ColFlag = false; ErrorMsg = "請確認行動類別"; }
                else
                {
                    switch (ACTION.Trim().ToLower()) {
                        case "add":
                            if (string.IsNullOrWhiteSpace(CLINICID)) { ColFlag = false; ErrorMsg = "請確認診所編號"; }
                            break;
                        case "edit":
                            if (string.IsNullOrWhiteSpace(LOGID)) { ColFlag = false; ErrorMsg = "請確認拜訪記錄編號"; }
                            int vId;
                            int.TryParse(LOGID, out vId);
                            if (vId < 1) { ColFlag = false; ErrorMsg = "請確認拜訪記錄編號"; }
                            else
                            {
                                DataTable dt = vLog.GetVisitLogById(vId);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    if (dt.Rows[0]["UserID"].ToString().ToUpper() != LTM.id) { ColFlag = false; ErrorMsg = "無可編輯的權限"; }
                                }
                                else
                                {
                                    ColFlag = false; ErrorMsg = "查無可編輯拜訪記錄";
                                }
                            }
                            break;
                        default:
                            ColFlag = false; ErrorMsg = "請確認行動類別";
                            break;
                    }
                }

                if (string.IsNullOrWhiteSpace(CLINICSTATUS)) { ColFlag = false; ErrorMsg = "請確認診所狀態"; }
                #endregion
                #region main
                if (ColFlag)
                {
                    string CId = CLINICID.Trim();
                    DateTime VisitTime = DateTime.Parse(VISITTIME);
                    
                    switch (ACTION.Trim().ToLower())
                    {
                        case "add":
                            
                            vLog.AddVisitLog(CId, LTM.id, VisitTime, CATEGORY, DESC);
                            
                            cInfo.UpdStatusByCId(CId, CLINICSTATUS);
                            break;
                        case "edit":
                            int logId = int.Parse(LOGID);

                            veLog.AddVisitEditLog(logId);

                            vLog.UpdVisitLog(logId, VisitTime, CATEGORY, DESC);
                            
                            cInfo.UpdStatusByCId(CId, CLINICSTATUS);
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
