using HomeWork2.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWork2.Services.Interface
{
    public interface IFacesIdentifyService
    {

        Task<List<Person>> CreateAndLearnGroup();

        Task<List<Person>> IdentifyPersons(string imagePath, string personGroupId);

    }
}
