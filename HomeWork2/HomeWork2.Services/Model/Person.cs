using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWork2.Services.Model
{
    public class Person
    {
        public Person()
        {

        }
        public Person(Guid peronId, string personGroupId)
        {
            PeronId = peronId;

            PersonGroupId = personGroupId;
        }
        public Guid PeronId { get; set; }

        public string PersonGroupId { get; set; }

        public string Name { get; set; }

        public int Rate { get; set; }

    }
}
