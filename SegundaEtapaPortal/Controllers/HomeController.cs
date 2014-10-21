using Microsoft.Win32;
/*
************************************************************
* Copyright © Ultimus.                                     *
* Diseño y desarrollo: Rubén Enrique Spagnuolo (respag)    *
* Panamá - 2014                                            *
************************************************************
*/
using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Ultimus.WFServer;

namespace ULAPW.Controllers
{ 
    public class HomeController : Controller
    {
      
        public string GetValueKeyRegedit(string k ,string KeyStringValue)
        {
            string retorno = string.Empty;
            try
            {
                RegistryKey localMachineRegistry = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                                       Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
                RegistryKey rk = localMachineRegistry.OpenSubKey(string.Format(@"SOFTWARE\\{0}", k, false));
                if (rk != null)
                {
                    retorno = (string)rk.GetValue(KeyStringValue);
                    rk.Close();
                }
            }
            catch (Exception ex)
            {
                retorno = string.Empty;

            }
            return retorno;

        }

        // Verica si usar loguin de ultimus o particular
        bool flag = Convert.ToBoolean(ConfigurationManager.AppSettings["FlagUltimusLogin"]);

        public ActionResult Index(string BLUP, string Aplicacion)
        {
            string dominio = GetValueKeyRegedit("ParametrosProcesoUltimus", "DominioCustomOc");
            ViewBag.Dominio = dominio;

            // En caso que el flag sea  falso , como en el caso de SERCOP...
            if (!flag)
            {
                // Si la URL contiene los parámetros correctos
                if (BLUP != null && Aplicacion == "MA")
                {
                    var ip = Request.UserHostAddress;

                    // Si el token es válido se crea la cookie usr_logued con el valor true, 
                    // una expiración de 300 segundos - 5 minutos - y se navega a la vista initiate
                    if (IsTokenValido(BLUP, ip))
                    {
                        //Crea la cookie usr_logued y le pone el valor true
                        var cookie = new HttpCookie("usr_logued", "true");
                        var tiempoExperacion = ConfigurationManager.AppSettings["ExpiracionEnSegundos"];
                        cookie.Expires = DateTime.Now.AddSeconds(Convert.ToDouble(tiempoExperacion));
                        Response.AppendCookie(cookie);

                        //Obtengo el ruc del resultado de ValidaToken, luego el tipo de empresa
                        // lo guardo en una cookie y redirijo a lo que corresponda
                        var ruc = ViewBag.Resultado.ToString().Split('|')[3];
                        InformationService.ServiceReference.InformationServiceClient client =
                                 new InformationService.ServiceReference.InformationServiceClient();
                        var resultado = client.GetDepartmentByRuc(ruc);
                        var TipoEmpresa = resultado.DepartamentType;
                        var cookie3 = new HttpCookie("tipo", TipoEmpresa);
                        var tiempoExpiracion = ConfigurationManager.AppSettings["ExpiracionEnSegundos"];
                        cookie3.Expires = DateTime.Now.AddSeconds(Convert.ToDouble(tiempoExpiracion));
                        Response.AppendCookie(cookie3);
                        if (TipoEmpresa == "ENT")
                            return View("Initiate");
                        else
                            return View("InboxProveedores");
                    }
                    // Si el token no es válido, ya expiró o hay algun problema
                    else
                    {
                        if (!flag)
                            return Redirect(ConfigurationManager.AppSettings["EasyLoginUrl"]);
                        else
                            return RedirectToAction("Login");
                    }
                }
                // Si la llamamos in darle los parametros 
                else
                {
                    if (!flag)
                        return Redirect(ConfigurationManager.AppSettings["EasyLoginUrl"]);
                    else
                        return RedirectToAction("Login");
                }
            }
            // Si la aplicación usa Autenticación de Ultimus
            else
                return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View("Login");
        }

        public ActionResult Initiate()
        {
            string dominio = GetValueKeyRegedit("ParametrosProcesoUltimus", "DominioCustomOc");
            ViewBag.Dominio = dominio;

            // Verifica a traves de la cookie si el usuario esta logueado
             var cookie = Request.Cookies["usr_logued"];
             if (cookie != null && cookie.Value.ToString() == "true")
             {
                 //Verifica la otra cookie con la respuesta del ValidaToken
                 var cookie2 = Request.Cookies["respuesta"];
                 if (cookie2 != null)
                     ViewBag.Resultado = cookie2.Value.ToString();
                 else
                     ViewBag.Resultado = "";
                // Redirige a la vista Initiate
                return View();
             }
             // Si no exisrte la cookie
             else
             {
                 if (!flag)
                     return Redirect(ConfigurationManager.AppSettings["EasyLoginUrl"]);
                 else
                     return RedirectToAction("Login");
             }
        }

        private bool IsTokenValido(string token, string ip)
        {
            // Llama al método VerificaToken del Sevicio WSEasyLogin
            var svc = new ULAPW.ServiceEasyLogin.WSEasyLoginSoapClient();
            var resp = svc.VerificaToken(token, "MA", ip);
 
            // Si devuelve alguno de lso mensajes de error, el método devolvera false ...
            if (resp == "ERROR" || resp == "TOKEN_NO_VALIDO" || resp == "TOKEN_EXPIRADO" || resp == "IP_INCORRECTA")
                return false;
            // caso contrario creo la segunda cookie con la respuesta y el metodo devuelve true
            else
            {   
                ViewBag.Resultado = resp;
                var cookie2 = new HttpCookie("respuesta", resp);
                var tiempoExperacion = ConfigurationManager.AppSettings["ExpiracionEnSegundos"];
                cookie2.Expires = DateTime.Now.AddSeconds(Convert.ToDouble(tiempoExperacion));
                Response.AppendCookie(cookie2);
                ViewBag.Ip = ip;
                return true;
            }
        }

        public ActionResult Inbox()
        {
            string dominio = GetValueKeyRegedit("ParametrosProcesoUltimus", "DominioCustomOc");
            ViewBag.Dominio = dominio;

            // Si existe la cookie de autenticacion
            var cookie = Request.Cookies["usr_logued"];
            if (cookie != null && cookie.Value.ToString() == "true")
            {
                //Verifica la segunda cookie para tener dispomnible la respuesta del validaToken
                var cookie2 = Request.Cookies["respuesta"];
                if (cookie2 != null)
                    ViewBag.Resultado = cookie2.Value.ToString();
                else
                    ViewBag.Resultado = "";
                // Verifica la tercera cookie para saber el tipo de empresa y redirige
                // a la vista correspondiente
                var cookie3 = Request.Cookies["tipo"];
                if (cookie3 != null)
                {
                    if (cookie3.Value.ToString() == "ENT")
                        return View();
                    else
                        return View("InboxProveedores");
                }
                else
                {
                    if (!flag)
                        return Redirect(ConfigurationManager.AppSettings["EasyLoginUrl"]);
                    else
                        return RedirectToAction("Login");
                }
            }
            //Si la cookie de autenticacion no existe 
            else
            {
                if (!flag)
                    return Redirect(ConfigurationManager.AppSettings["EasyLoginUrl"]);
                else
                    return RedirectToAction("Login");
            }
        }
   

        public ActionResult InboxProveedores()
        {
            var cookie = Request.Cookies["usr_logued"];
            if (cookie != null && cookie.Value.ToString() == "true")
            {
                var cookie2 = Request.Cookies["respuesta"];
                if (cookie2 != null)
                    ViewBag.Resultado = cookie2.Value.ToString();
                else
                    ViewBag.Resultado = "";
                return View();
            }
            else
            {
                if (!flag)
                    return Redirect(ConfigurationManager.AppSettings["EasyLoginUrl"]);
                else
                    return RedirectToAction("Login");
            }
        }

        public ActionResult Completed()
        {
            string dominio = GetValueKeyRegedit("ParametrosProcesoUltimus", "DominioCustomOc");
            ViewBag.Dominio = dominio;

            var cookie = Request.Cookies["usr_logued"];
            if (cookie != null && cookie.Value.ToString() == "true")
            {
                var cookie2 = Request.Cookies["respuesta"];
                if (cookie2 != null)
                    ViewBag.Resultado = cookie2.Value.ToString();
                else
                    ViewBag.Resultado = "";

                var cookie3 = Request.Cookies["tipo"];
                if (cookie3 != null)
                {
                    if (cookie3.Value.ToString() == "ENT")
                        return View();
                    else
                        return View("CompletedProveedores");
                }
                else
                {
                    if (!flag)
                        return Redirect(ConfigurationManager.AppSettings["EasyLoginUrl"]);
                    else
                        return RedirectToAction("Login");
                }
            }
            else
            {
                if (!flag)
                    return Redirect(ConfigurationManager.AppSettings["EasyLoginUrl"]);
                else
                    return RedirectToAction("Login");
            }
        }

        public ActionResult CompletedProveedores()
        {
            var cookie = Request.Cookies["usr_logued"];
            if (cookie != null && cookie.Value.ToString() == "true")
            {
                var cookie2 = Request.Cookies["respuesta"];
                if (cookie2 != null)
                    ViewBag.Resultado = cookie2.Value.ToString();
                else
                    ViewBag.Resultado = "";
                return View();
            }
            else
            {
                if (!flag)
                    return Redirect(ConfigurationManager.AppSettings["EasyLoginUrl"]);
                else
                    return RedirectToAction("Login");
            }
        }

        [OutputCache(Duration = 300)]
        public ActionResult MuestraImagen(string processName, int incidente, int version)
        {
            GetImageBytes(processName, incidente, version);
            ViewBag.processName = processName;
            ViewBag.incidente = incidente;
            ViewBag.version = version;
            var cookie = Request.Cookies["usr_logued"];
            if (cookie != null && cookie.Value.ToString() == "true")
            {
                var cookie2 = Request.Cookies["respuesta"];
                if (cookie2 != null)
                    ViewBag.Resultado = cookie2.Value.ToString();
                else
                    ViewBag.Resultado = "";
                var cookie3 = Request.Cookies["tipo"];
                ViewBag.Oculta = "no";
                if (cookie3 != null)
                {
                    if (cookie3.Value.ToString() == "PRO")
                        ViewBag.Oculta = "si";
                }
                return View();
            }
            else
            {
                if (!flag)
                    return Redirect(ConfigurationManager.AppSettings["EasyLoginUrl"]);
                else
                    return RedirectToAction("Login");
            }
        }

        public void GetImageBytes(string processname, int incident, int version)
        {
            byte[] ret;
            Incident.Status status = new Incident.Status();
            status.GetGraphicalStatus(processname, incident, version, out ret);
            string FileName = null;
            FileName = ConfigurationManager.AppSettings["PathImages"] + processname + incident + version + ".gif";
            var fs = new FileStream(FileName, FileMode.Create, FileAccess.Write);
            fs.Write(ret, 0, ret.Length);
            fs.Close();
        }
    }

}