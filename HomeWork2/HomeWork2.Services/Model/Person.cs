using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Face.Contract;
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

        public Person(Guid peronId, string personGroupId, string name, Emotion emotion, string imageFileName)
        {
            PeronId = peronId;

            PersonGroupId = personGroupId;

            Name = name;

            Emotion = emotion;

            ImageFileName = imageFileName;
        }

        public Guid PeronId { get; set; }

        public string PersonGroupId { get; set; }

        public string Name { get; set; }

        public Emotion Emotion { get; set; }

        public string ImageFileName { get; set; }

    }
}
