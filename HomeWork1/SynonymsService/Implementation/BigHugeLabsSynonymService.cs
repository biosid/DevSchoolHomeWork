using Flurl.Http;
using Newtonsoft.Json;
using SynonymsService.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SynonymsService.Implementation
{
    public class BigHugeLabsSynonymService : ISynonymService
    {

        #region fields

        private readonly string _serviceApiUrl = "http://words.bighugelabs.com/api/2";

        private readonly string _apiKey = "91b5ea01f5fc75a9e039ebbf040e3cb9";

        #endregion

        #region utils

        private string prepareQueryString(string word)
        {
            return $"{_serviceApiUrl}/{_apiKey}/{word}/json";
        }

        #endregion

        #region Implementetion of ISynonymService

        public async Task<List<string>> GetSynonymsAsync(SynonymType type, string word)
        {
            try
            {
                var client = new FlurlClient(prepareQueryString(word), true);

                var jsonResult = await client.GetJsonAsync();

                var serializer = new JavaScriptSerializer();
                var jsonString = serializer.Serialize(jsonResult);

                SynonymResult result = JsonConvert.DeserializeObject<SynonymResult>(jsonString);

                switch ((int)type)
                {
                    case 1:
                        return result.FirstOrDefault(w => w.Key == "verb").Value.FirstOrDefault(w => w.Key == "syn").Value;
                    case 2:
                        return result.FirstOrDefault(w => w.Key == "noun").Value.FirstOrDefault(w => w.Key == "syn").Value;
                    case 3:
                        return result.FirstOrDefault(w => w.Key == "adjective").Value.FirstOrDefault(w => w.Key == "syn").Value;
                    default:
                        return new List<string>();
                }

            }
            catch (Exception)
            {
                return new List<string>();
            }
        }


        public async Task<string> GetRandomSynonymAsync(SynonymType type, string word)
        {
            var random = new Random();

            var synonyms = await GetSynonymsAsync(type, word);
            if (synonyms.Count == 0)
                return string.Empty;

            var synonym = synonyms[random.Next(0, synonyms.Count - 1)];

            return synonym;

        }


        #endregion
    }
}
