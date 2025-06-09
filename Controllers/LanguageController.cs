using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Nhom19_WebBanHoa.Controllers
{
    public class LanguageController : Controller
    {
        [HttpGet]
        public IActionResult Set(string culture, string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(culture))
            {
                HttpContext.Session.SetString("Lang", culture);
            }
            // Redirect to previous page or home
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }
    }
}