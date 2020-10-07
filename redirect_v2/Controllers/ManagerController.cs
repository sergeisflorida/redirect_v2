using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace redirect_v2.Controllers
{
    public class rController : Controller
    {
        public ActionResult Index()
        {
            var vid = Url.RequestContext.RouteData.Values["id"];
            string id;

            if (vid == null)
            {
                return View();
            }
            id = "custom_" + vid.ToString();

            Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Request.ApplicationPath);
            if (config.AppSettings.Settings[id] == null) return View();

            return Redirect(config.AppSettings.Settings[id].Value);
        }
    }

    public class ManagerController : Controller
    {
        SessionContext context = new SessionContext();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string key)
        {
           if (GetSecurityKey() == key)
            {
                User u = new User { key = key };
                context.SetAuthenticationToken("token", false, u);
                return RedirectToAction("config", "manager");
            }
            return View();
        }

        private string GetSecurityKey()
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Request.ApplicationPath);
            return config.AppSettings.Settings["key"].Value;
        }

        private string GetAllKeys()
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Request.ApplicationPath);
            string conf = "";
            string keyS = "";

            foreach (var key in config.AppSettings.Settings.AllKeys)
            {
                if (key.StartsWith("custom_"))
                {
                    keyS = key.Replace("custom_", "");
                    conf += keyS + "=" + config.AppSettings.Settings[key].Value + "\n";
                }
            }
            return conf;
        }

        private void CleanCustomKeys()
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Request.ApplicationPath);
            foreach (var key in config.AppSettings.Settings.AllKeys)
            {
                if (key.StartsWith("custom_"))
                {
                    config.AppSettings.Settings.Remove(key);
                }
            }
            config.Save();
        }

        private void SaveCustomKeys(string conf)
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Request.ApplicationPath);
            string[] lines = conf.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                string[] parts = line.Split('=');
                if (parts.Length == 2)
                {
                    string key = parts[0];
                    string value = parts[1];
                    if (key.Length > 0 && value.Length > 0)
                    {
                        key = "custom_" + key;
                        config.AppSettings.Settings.Add(key, value);
                    }
                }
            }
            config.Save();
        }

        [HttpGet]
        public ActionResult Config()
        {
            User u = context.GetUserData();
            if (u == null || GetSecurityKey() != u.key) return RedirectToAction("index", "manager");

            ViewBag.keys = GetAllKeys();
            return View();
        }

        [HttpPost]
        public ActionResult Config(string keys)
        {
            User u = context.GetUserData();
            if (u == null || GetSecurityKey() != u.key) return RedirectToAction("index", "manager");

            CleanCustomKeys();
            SaveCustomKeys(keys);

            ViewBag.keys = GetAllKeys();
            return View();
        }


    }
}