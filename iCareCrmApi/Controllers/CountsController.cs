using iCareCrmApi.Ado.iCare;
using iCareCrmApi.Handle;
using iCareCrmApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace iCareCrmApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [CorsOnActionHandle]
    [RoutePrefix("api/counts")]
    public class CountsController : ApiController
    {
        #region 
        ClinicsAdo clinicADO = new ClinicsAdo();
        PatientsAdo patientADO = new PatientsAdo();
        #endregion
        #region GET
        [HttpGet]
        [Route("clinic")]
        public IHttpActionResult ClinicList()
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
                    CountsModel COM = new CountsModel();
                    COM.clinic = clinicADO.GetCountClinics();
                    COM.patient = patientADO.GetCountPatient();

                    result.status = true;
                    result.error = "";
                    result.code = 200;
                    result.data = COM;
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
    }
}
