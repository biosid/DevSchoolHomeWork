using HomeWork2.Services.Model;
using HomeWork2.Services.Utils;
using Microsoft.ProjectOxford.Common;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWork2.Services.Interface
{
    public class FacesIdentifyService : IFacesIdentifyService
    {

        #region fields

        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient(ApiKeys.Face);

        private readonly EmotionServiceClient emotionServiceClient = new EmotionServiceClient(ApiKeys.Emotion);

        #endregion

        #region utils

        private async Task NeedWait(int seconds = 10)
        {

            //Ограничение API на 20 попыток в минуту 
            await Task.Delay(seconds * 1000);
        }

        private async Task DefinePerson(CreatePersonResult person, string personGroupId, List<string> imagesDir)
        {
            foreach (string imagePath in imagesDir)
            {
                using (Stream s = File.OpenRead(imagePath))
                {
                    try
                    {
                        // Detect faces in the image and add to Anna
                        await faceServiceClient.AddPersonFaceAsync(personGroupId, person.PersonId, s);
                    }
                    catch (FaceAPIException ex)
                    {
                        //ignore
                    }

                }
            }
            await NeedWait();

        }

        private async Task<TrainingStatus> CheckGroupTrainingStatus(string personGroupId)
        {
            try
            {

                var trainingStatus = await faceServiceClient.GetPersonGroupTrainingStatusAsync(personGroupId);
                return trainingStatus;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        private async Task<List<Model.Person>> CreateAndTrainGroup()
        {
            try
            {
                string daenerysImages = "Content/learning/daenerys/";
                string missandeiImages = "Content/learning/missandei/";
                string mormontImages = "Content/learning/mormont/";

                var result = new List<Model.Person>();
                // Create an empty person group
                string personGroupId = "gameofthroneshomework";

                var group = await faceServiceClient.GetPersonGroupAsync(personGroupId);

                if (group == null)
                    await faceServiceClient.CreatePersonGroupAsync(personGroupId, "Game of Thrones");

                // Define Daenerys
                CreatePersonResult daenerys = await faceServiceClient.CreatePersonAsync(personGroupId, "Daenerys");

                await DefinePerson(daenerys, personGroupId, ImagesHelper.GetImagesPath(daenerysImages));

                result.Add(new Model.Person(daenerys.PersonId, personGroupId));

                // Define Missandei
                CreatePersonResult missandei = await faceServiceClient.CreatePersonAsync(personGroupId, "Missandei");

                await DefinePerson(missandei, personGroupId, ImagesHelper.GetImagesPath(missandeiImages));

                result.Add(new Model.Person(missandei.PersonId, personGroupId));

                // Define Mormont
                CreatePersonResult mormont = await faceServiceClient.CreatePersonAsync(personGroupId, "Mormont");

                await DefinePerson(mormont, personGroupId, ImagesHelper.GetImagesPath(mormontImages));

                result.Add(new Model.Person(mormont.PersonId, personGroupId));


                TrainingStatus trainingStatus = await CheckGroupTrainingStatus(personGroupId);

                if (trainingStatus == null || trainingStatus.Status != Status.Failed)
                    await faceServiceClient.TrainPersonGroupAsync(personGroupId);

                while (true)
                {
                    trainingStatus = await faceServiceClient.GetPersonGroupTrainingStatusAsync(personGroupId);

                    if (trainingStatus.Status != Status.Running)
                    {
                        break;
                    }

                    await NeedWait();
                }
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private bool CompareFaceRectangles(Rectangle s, FaceRectangle t)
        {
            return (s.Height == t.Height) && (s.Left == t.Left) && (s.Top == t.Top) && (s.Width == t.Width);
        }

        private async Task<IdentifyResult[]> GetidentifyResults(string group, Guid[] faces)
        {
            try
            {
                // Detect faces in the image and add to Anna
                var identifyResults = await faceServiceClient.IdentifyAsync(group, faces);
                await NeedWait(2);
                return identifyResults;
            }
            catch (FaceAPIException ex)
            {
                return null;
            }

        }
        #endregion

        public async Task<List<Model.Person>> CreateAndLearnGroup()
        {
            return await CreateAndTrainGroup();
        }

        public async Task<List<Model.Person>> IdentifyPersons(string imagePath, string personGroupId)
        {
            var fileName = Path.GetFileName(imagePath);

            var list = new List<Model.Person>();

            Face[] faces = null;

            Emotion[] emotions = null;

            //на случай RateException
            try
            {

                using (Stream s = File.OpenRead(imagePath))
                {
                    try
                    {
                        faces = await faceServiceClient.DetectAsync(s, true, true,
                        new List<FaceAttributeType> {
                        FaceAttributeType.Age,
                        FaceAttributeType.FacialHair,
                        FaceAttributeType.Gender,
                        FaceAttributeType.Glasses,
                        FaceAttributeType.HeadPose,
                        FaceAttributeType.Smile });
                        await NeedWait(2);
                    }
                    catch (FaceAPIException ex)
                    {
                        return list;
                    }
                }

                using (Stream s = File.OpenRead(imagePath))
                {
                    emotions = await emotionServiceClient.RecognizeAsync(s);
                    await NeedWait(2);
                }
                if (faces == null || faces.Count() == 0)
                    return list;

                var faceIds = faces.Select(face => face.FaceId).ToArray();

                var identifyResults = await GetidentifyResults(personGroupId, faceIds);

                if (identifyResults != null)
                    foreach (var identifyResult in identifyResults)
                    {
                        if (identifyResult.Candidates.Length != 0)
                        {
                            var currentFace = faces.FirstOrDefault(w => w.FaceId == identifyResult.FaceId);

                            var currentEmotion = emotions.FirstOrDefault(w => CompareFaceRectangles(w.FaceRectangle, currentFace.FaceRectangle));

                            var candidateId = identifyResult.Candidates[0].PersonId;

                            var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);

                            list.Add(new Model.Person(candidateId, personGroupId, person.Name, currentEmotion, fileName));

                            await NeedWait(2);
                        }
                    }
                await NeedWait(5);
            }
            catch (FaceAPIException ex)
            {
                await NeedWait(10);
            }
            return list;

        }

        public async Task<bool> ChekGroupHasLearn(string personGroupId)
        {
            var status = await CheckGroupTrainingStatus(personGroupId);
            return (status != null) && status.Status != Status.Failed;
        }
    }
}
