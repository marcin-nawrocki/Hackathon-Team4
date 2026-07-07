using System.Linq;
using System.Web.Mvc;

namespace Supercontrol.Web.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var db = new Models.Supercontrol2Context();
            var v = db.Database.SqlQuery<int>("SELECT 1").FirstOrDefault();
            return View();
        }
    }
}
