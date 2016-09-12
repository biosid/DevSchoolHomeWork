using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynonymsService.Implementation
{
    [JsonArray]
    public class SynonymResult : Dictionary<string, SynonymResultItem>
    {
    }
    [JsonArray]
    public class SynonymResultItem : Dictionary<string, List<string>>
    {
    }
}
