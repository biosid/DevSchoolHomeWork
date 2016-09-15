using HomeWork2.Services.Interface;
using HomeWork2.Services.Model;
using HomeWork2.Services.Utils;
using System;
using System.Collections.Concurrent;
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

        /// <summary>
        /// Для веб приложения так делать не нужно, сделано в учебных целях на локальной машине, чтоб не транжирить вызовы API
        /// </summary>
        private static ConcurrentBag<Person> PersonsList = new ConcurrentBag<Person>();

        public async Task<ActionResult> Index()
        {
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
        [HttpPost]
        public async Task<JsonResult> LearnGroup()
        {
            var faces = await service.CreateAndLearnGroup();
            return Json(faces);
        }

        [HttpPost]
        public async Task<JsonResult> CheckGroupHasLearn()
        {
            var status = await service.ChekGroupHasLearn(groupId);
            return Json(status);
        }

        [HttpPost]
        public async Task<JsonResult> RefresStatistic()
        {
            if (PersonsList.Count == 0)
            {
                var files = ImagesHelper.GetImagesPath();
                foreach (var file in files)
                {
                    var persons = await service.IdentifyPersons(file, groupId);
                    persons.ForEach(person => PersonsList.Add(person));
                }
            }

            return Json(new
            {
                daenerys = PersonsList.Where(w => w.Name == "Daenerys").OrderBy(w => w.ImageFileName).ToList(),
                missandei = PersonsList.Where(w => w.Name == "Missandei").OrderBy(w => w.ImageFileName).ToList(),
                mormont = PersonsList.Where(w => w.Name == "Mormont").OrderBy(w => w.ImageFileName).ToList()
            });
        }
    }
}