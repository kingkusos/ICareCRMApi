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
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Xml.Linq;
using iCareCrmApi.Handle;
using iCareCrmApi.Filters;
using System.Web.Http.Cors;

namespace iCareCrmApi.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    //[CorsOnActionHandle]
    //[Access_Filter]
    [RoutePrefix("api/clinic")]
    
    public class ClinicController : ApiController
    {
        #region 

        ClinicInfoAdo cInfo = new ClinicInfoAdo();
        #endregion
        #region GET
        #region 診所列表
        
        [HttpGet]
        [Route("list/{page}")]
        public IHttpActionResult ClinicList(int page)
        {
            ResultModel result = new ResultModel();

            try
            {
                #region get value
                string CITY = HttpContext.Current.Request.QueryString["filter_city"] != null ? HttpContext.Current.Request.QueryString["filter_city"].ToString() : "";
                string AREA = HttpContext.Current.Request.QueryString["filter_district"] != null ? HttpContext.Current.Request.QueryString["filter_district"].ToString() : "";
                string ROAD = HttpContext.Current.Request.QueryString["filter_name"] != null ? HttpContext.Current.Request.QueryString["filter_name"].ToString() : "";
                string SORT = HttpContext.Current.Request.QueryString["Permutations"] != null ? HttpContext.Current.Request.QueryString["Permutations"].ToString() : "";
                string STATUS = HttpContext.Current.Request.QueryString["filter_clinic_status"] != null ? HttpContext.Current.Request.QueryString["filter_clinic_status"].ToString() : "";
                string DEPT = HttpContext.Current.Request.QueryString["Department"] != null ? HttpContext.Current.Request.QueryString["Department"].ToString() : "";
                #endregion
                #region check value
                bool ColFlag = true;
                string ErrorMsg = string.Empty;
                if (string.IsNullOrWhiteSpace(SORT)) { ColFlag = false; ErrorMsg = "請確認排序方式"; }

                #endregion
                #region main
                if (ColFlag)
                {
                    ClinicPageModel CPM = new ClinicPageModel();

                    DataTable dt = cInfo.QueryPagingClinicInfoBySearch(page, 10, CITY.Trim(), AREA.Trim(), ROAD.Trim(), STATUS.Trim(), SORT.Trim(), DEPT.Trim());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        CPM.total = int.Parse(dt.Rows[0]["TotalCnt"].ToString());
                        CPM.totalpage = int.Parse(dt.Rows[0]["TotalPage"].ToString());

                        foreach (DataRow dr in dt.Rows)
                        {
                            string vTimeString = string.Empty;
                            DateTime vTime;
                            if (!string.IsNullOrWhiteSpace(dr["VisitTime"].ToString()))
                            {
                                if (DateTime.TryParse(dr["VisitTime"].ToString(), out vTime))
                                {
                                    vTimeString = vTime.ToString("yyyy/MM/dd HH:mm");
                                }
                            }
                            
                            ClinicListModel CLM = new ClinicListModel();
                            CLM.id = dr["CID"].ToString();
                            CLM.name = dr["CName"].ToString();
                            CLM.phone = dr["CPhone"].ToString();
                            CLM.city = dr["CCity"].ToString();
                            CLM.district = dr["CArea"].ToString();
                            CLM.road = dr["CAddr"].ToString();
                            CLM.visitor_id = dr["UID"].ToString();
                            CLM.visitor_name = dr["UNickName"].ToString();
                            CLM.visit_datetime = vTimeString;
                            CLM.clinic_status = dr["CType"].ToString();
                            CPM.list.Add(CLM);
                        }
                    }
                    
                    result.status = true;
                    result.error = "";
                    result.code = 200;
                    result.data = CPM;
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
        #region 診所資訊
        [HttpGet]
        [Route("info")]
        public IHttpActionResult ClinicInfo()
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
                    DataTable dt = cInfo.GetClinicInfo(CLINIC.Trim());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        ClinicModel clinicMD = new ClinicModel();
                        foreach (DataRow dr in dt.Rows)
                        {
                            clinicMD.id = dr["CID"].ToString();
                            clinicMD.name = dr["CName"].ToString();
                            clinicMD.phone = dr["CPhone"].ToString();
                            clinicMD.city = dr["CCity"].ToString();
                            clinicMD.district = dr["CArea"].ToString();
                            clinicMD.road = dr["CAddr"].ToString();
                            clinicMD.his = dr["HisSys"].ToString();
                            clinicMD.isUse_video = bool.Parse(dr["IsVideo"].ToString());
                            clinicMD.isDecided = bool.Parse(dr["IsMaster"].ToString());
                            clinicMD.people = int.Parse(dr["NumberDr"].ToString());
                            clinicMD.call_number_way = dr["CallWay"].ToString();
                            clinicMD.isVisit_datetime = dr["OrderInfo"].ToString();
                            clinicMD.care_group = dr["MedicalGroup"].ToString();
                            clinicMD.experience = dr["EvenPracticing"].ToString();
                            clinicMD.care_network = dr["SharedCareContent"].ToString();
                            clinicMD.clinic_status = dr["CType"].ToString();
                        }

                        result.status = true;
                        result.error = "";
                        result.code = 200;
                        result.data = clinicMD;
                    }
                    else
                    {
                        result.status = false;
                        result.error = "查無此診所資料";
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
        #region POST
        #region 編輯基本資
        [HttpPost]
        [Route("info/edit")]
        public IHttpActionResult ClinicInfoEdit()
        {
            ResultModel result = new ResultModel();

            try
            {
                #region get value
                string ID = HttpContext.Current.Request.Form["id"] != null ? HttpContext.Current.Request.Form["id"].ToString() : "";
                string NAME = HttpContext.Current.Request.Form["name"] != null ? HttpContext.Current.Request.Form["name"].ToString() : "";
                string PHONE = HttpContext.Current.Request.Form["phone"] != null ? HttpContext.Current.Request.Form["phone"].ToString() : "";
                string ADDR = HttpContext.Current.Request.Form["addr"] != null ? HttpContext.Current.Request.Form["addr"].ToString() : "";
                string HIS = HttpContext.Current.Request.Form["his"] != null ? HttpContext.Current.Request.Form["his"].ToString() : "";
                string ISVIDEO = HttpContext.Current.Request.Form["isUse_video"] != null ? HttpContext.Current.Request.Form["isUse_video"].ToString() : "";
                string ISDECIDED = HttpContext.Current.Request.Form["isDecided"] != null ? HttpContext.Current.Request.Form["isDecided"].ToString() : "";
                string PEOPLE = HttpContext.Current.Request.Form["people"] != null ? HttpContext.Current.Request.Form["people"].ToString() : "";
                string CALLWAY = HttpContext.Current.Request.Form["call_number_way"] != null ? HttpContext.Current.Request.Form["call_number_way"].ToString() : "";
                string VISITTIME = HttpContext.Current.Request.Form["isVisit_datetime"] != null ? HttpContext.Current.Request.Form["isVisit_datetime"].ToString() : "";
                string MEDGROUP = HttpContext.Current.Request.Form["care_group"] != null ? HttpContext.Current.Request.Form["care_group"].ToString() : "";
                string EXPERIENCE = HttpContext.Current.Request.Form["experience"] != null ? HttpContext.Current.Request.Form["experience"].ToString() : "";
                string CARENETWORK = HttpContext.Current.Request.Form["care_network"] != null ? HttpContext.Current.Request.Form["care_network"].ToString() : "";
                #endregion
                #region check value
                
                bool ColFlag = true;
                string ErrorMsg = string.Empty;
                if (string.IsNullOrWhiteSpace(ID.Trim())) { ColFlag = false; ErrorMsg = "請確認機構代碼"; }

                if (string.IsNullOrWhiteSpace(NAME.Trim())) { ColFlag = false; ErrorMsg = "請確認診所名稱"; }

                if (string.IsNullOrWhiteSpace(ADDR)) { ColFlag = false; ErrorMsg = "請確認地址"; }
                else
                {
                    string[] ZoneAry = ADDR.Split(',');
                    if (string.IsNullOrWhiteSpace(ADDR[0].ToString())) { ColFlag = false; ErrorMsg = "請確認城市"; }
                    if (string.IsNullOrWhiteSpace(ADDR[1].ToString())) { ColFlag = false; ErrorMsg = "請確認地區"; }
                    if (string.IsNullOrWhiteSpace(ADDR[2].ToString())) { ColFlag = false; ErrorMsg = "請確認路段"; }
                }

                //if (string.IsNullOrWhiteSpace(HIS)) { ColFlag = false; ErrorMsg = "請確認HIS系統"; }

                bool IsVideo;
                if(!bool.TryParse(ISVIDEO, out IsVideo)) { ColFlag = false; ErrorMsg = "請確認是否視訊"; }

                bool IsDecided;
                if (!bool.TryParse(ISDECIDED, out IsDecided)) { ColFlag = false; ErrorMsg = "請確認醫師能否做主"; }

                int People;
                if(!int.TryParse(PEOPLE, out People)) { ColFlag = false; ErrorMsg = "請確認醫師人數"; }

                #endregion
                #region main
                if (ColFlag)
                {
                    string CId = ID.Trim();

                    string[] ZoneAry = ADDR.Split(',');
                    string City = ZoneAry[0];
                    string Area = ZoneAry[1];
                    string Addr = ZoneAry[2];

                    bool IsOrder = false;
                    if (!string.IsNullOrWhiteSpace(VISITTIME))
                    {
                        IsOrder = true;
                    }

                    bool IsSharedCare = false;
                    if (!string.IsNullOrWhiteSpace(CARENETWORK))
                    {
                        IsSharedCare = true;
                    }


                    cInfo.UpdInfoByCId(
                        CId, NAME, PHONE, City, Area, Addr,
                        HIS, IsVideo, IsDecided, People, CALLWAY,
                        IsOrder, VISITTIME,
                        MEDGROUP, EXPERIENCE, 
                        IsSharedCare, CARENETWORK
                    );

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
