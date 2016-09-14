using HomeWork2.Services.Interface;
using HomeWork2.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HomeWork2.Controllers
{
    public class HomeController : Controller
    {
        const string groupId = "gameofthroneshomework";
        private IFacesIdentifyService service = new FacesIdentifyService();

        public async Task<ActionResult> Index()
        {

            var files = ImagesHelper.GetImagesPath();

            var faces = await service.IdentifyPersons(files.FirstOrDefault(w => w.Contains("out6")), groupId);

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}