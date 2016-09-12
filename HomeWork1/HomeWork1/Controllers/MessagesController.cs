using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.ProjectOxford.Linguistics.Contract;
using Microsoft.ProjectOxford.Linguistics;
using SynonymsService.Interface;
using SynonymsService.Implementation;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using SynonymsService;

namespace HomeWork1
{

    [BotAuthentication]
    public class MessagesController : ApiController
    {

        #region fields

        private readonly bool needRemoveNouns = true;
        private readonly bool needRemoveAdjectives = true;
        private readonly bool needRemoveVerbs = true;
        private readonly ISynonymService _synonymService = new BigHugeLabsSynonymService();

        #endregion


        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                var Client = new LinguisticsClient("159e8805f1644d02a880e9e7b45ec136");

                var Analyzers = await Client.ListAnalyzersAsync();
                var Req = new AnalyzeTextRequest();
                Req.Language = "en";
                Req.Text = activity.Text;
                Req.AnalyzerIds = new Guid[] { Analyzers[1].Id };
                var Res = await Client.AnalyzeTextAsync(Req);
                var resultLinguisticsTree = Res[0].Result.ToString();

                string result = Req.Text;
                if (needRemoveNouns)
                    result = await ReplaceText(result, SynonymType.noun, GetPartsOfSpeach(resultLinguisticsTree, SynonymType.noun));
                if (needRemoveAdjectives)
                    result = await ReplaceText(result, SynonymType.adjective, GetPartsOfSpeach(resultLinguisticsTree, SynonymType.adjective));
                if (needRemoveVerbs)
                    result = await ReplaceText(result, SynonymType.verb, GetPartsOfSpeach(resultLinguisticsTree, SynonymType.verb));

                Activity reply = activity.CreateReply(result);

                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task<string> ReplaceText(string text, SynonymType type, List<string> words)
        {
            foreach (var word in words)
            {
                var synonym = await _synonymService.GetRandomSynonymAsync(type, word);
                if (!string.IsNullOrEmpty(synonym))
                    text = text.Replace(word, synonym);
            }
            return text;
        }

        private List<string> GetPartsOfSpeach(string text, SynonymType type)
        {
            var result = new List<string>();
            string regex = string.Empty;

            switch ((int)type)
            {

                case 1:
                    regex = "([(]VB(D|G|N|P|Z)?\\s*(?<word>\\w+)[)])";
                    break;
                case 2:
                    regex = "([(]NNP?S?\\s*(?<word>\\w+)[)])";
                    break;
                case 3:
                    regex = "([(]JJ(S|R)?\\s*(?<word>\\w+)[)])";
                    break;

            }
            RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
            Regex reg = new Regex(regex, options);

            var matches = reg.Matches(text);

            foreach (Match ItemMatch in matches)
            {
                result.Add(ItemMatch.Groups["word"].ToString());
            }

            return result;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}