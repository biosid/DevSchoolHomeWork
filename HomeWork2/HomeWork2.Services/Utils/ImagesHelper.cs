using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HomeWork2.Services.Utils
{
    public class ImagesHelper
    {
        public static List<string> GetImagesPath()
        {
            var path = HttpContext.Current.Server.MapPath("/Content/images/");

            var files = Directory.GetFiles(path);

            return files.ToList();
        }
        public static List<string> GetImagesPath(string pathDdir)
        {
            var path = HttpContext.Current.Server.MapPath(pathDdir);

            var files = Directory.GetFiles(path);

            return files.ToList();
        }

    }
}
