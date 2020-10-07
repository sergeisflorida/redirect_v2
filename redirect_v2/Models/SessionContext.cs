using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;

public class User
{
    public string key { get; set; }
}

public class SessionContext
{
    public void SetAuthenticationToken(string name, bool isPersistant, User userData)
    {
        string data = new JavaScriptSerializer().Serialize(userData);

        FormsAuthenticationTicket ticket = 
            new FormsAuthenticationTicket(1, name, DateTime.Now,
            DateTime.Now.AddHours(1), isPersistant, data);

        string cookieData = FormsAuthentication.Encrypt(ticket);
        HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieData)
        {
            //HttpOnly = true,
            Expires = ticket.Expiration
        };
        HttpContext.Current.Response.Cookies.Add(cookie);
    }

    public User GetUserData()
    {
        User userData = null;
        try
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                userData = new JavaScriptSerializer().Deserialize(ticket.UserData, typeof(User)) as User;
            }
        }
        catch
        {
        }
        return userData;
    }
}