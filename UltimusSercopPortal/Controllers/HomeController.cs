using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ultimus.WFServer;


namespace ULAPW.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string BLUP, string Aplicacion)
        {             
            if (BLUP != null && Aplicacion =="MA")
            {
                var flag = Convert.ToBoolean(ConfigurationManager.AppSettings["FlagUltimusLogin"]);
                var ip = Request.UserHostAddress;
                
                if (IsTokenValido(BLUP, ip) )
                    return View("Initiate");
                else
                {
                    if (!flag)
                        return Redirect("http://192.168.9.212/easywebacceso/EasyLogin/wfLogin.aspx");
                    else
                        return RedirectToAction("Login");
                }
            }
            else
            {
                return View("Login");
            }
        }

        public ActionResult Login()
        {
            return View("Login");
        }

        public ActionResult Initiate(bool userLoged = false)
        {
            if (userLoged)
                return View();
             else
                return RedirectToAction("Login");
        }

        private bool IsTokenValido(string token, string ip)
        {
            var svc = new ULAPW.ServiceEasyLogin.WSEasyLoginSoapClient();
            //var ip = Request.UserHostAddress; 
            var resp = svc.VerificaToken(token, "MA", ip);
            if (resp == "ERROR" || resp == "TOKEN_NO_VALIDO" || resp == "TOKEN_EXPIRADO" || resp == "IP_INCORRECTA")
                return false;
            else
            {
                //userLoged = true;
                ViewBag.Resultado = resp;
                ViewBag.Ip = ip;
                return true;
            }
            //return true;
        }

        //public ActionResult Redirige()
        //{
        //    return View();
        //}

        public ActionResult Inbox(string BLUP, string Aplicacion)
        {
            if (BLUP != null && Aplicacion == "MA")
            {
                var flag = Convert.ToBoolean(ConfigurationManager.AppSettings["FlagUltimusLogin"]);
                var ip = Request.UserHostAddress;

                if (IsTokenValido(BLUP, ip))
                    return View("Inbox");
                else
                {
                    if (!flag)
                        return Redirect("http://192.168.9.212/easywebacceso/EasyLogin/wfLogin.aspx");
                    else
                        return RedirectToAction("Login");
                }
            }
            else
            {
                return View("Login");
            }
        }

        public ActionResult Completed(string BLUP, string Aplicacion)
        {
            if (BLUP != null && Aplicacion == "MA")
            {
                var flag = Convert.ToBoolean(ConfigurationManager.AppSettings["FlagUltimusLogin"]);
                var ip = Request.UserHostAddress;

                if (IsTokenValido(BLUP, ip))
                    return View("Completed");
                else
                {
                    if (!flag)
                        return Redirect("http://192.168.9.212/easywebacceso/EasyLogin/wfLogin.aspx");
                    else
                        return RedirectToAction("Login");
                }
            }
            else
            {
                return View("Login");
            }
        }

        [OutputCache(Duration=0)]
        public ActionResult MuestraImagen(string processName, int incidente, int version)
        {
            byte[] bytesGif  = GetImageBytes(processName, incidente, version);
            if (bytesGif != null)
            {
                string imageBase64Data = Convert.ToBase64String(bytesGif);
                string imageDataURL = string.Format("data:image/png;charset=utf-8;base64,{0}", Convert.ToBase64String(bytesGif));
                ViewBag.ImageData = imageDataURL;
            }
            return View();
        }


        public byte[] GetImageBytes(string processname, int incident, int version)
        {
            byte[] ret;
            Incident.Status status = new Incident.Status();
            status.GetGraphicalStatus(processname, incident, version, out ret);

            return ret;
        }


    }
}