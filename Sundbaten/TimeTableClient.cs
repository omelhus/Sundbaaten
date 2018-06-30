using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sundbaten
{
    public static class TimeTableClient
    {
        static readonly HttpClient HttpClient = new HttpClient();

        public static async Task<TimeTableResponse> GetTask()
        {
            var rsp = await HttpClient.GetAsync("https://rutetider.on-it.xyz/api/rutetider");
            rsp.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<TimeTableResponse>(await rsp.Content.ReadAsStringAsync());
        }

        public static async Task<List<Ad>> GetAdsTask()
        {
            var rsp = await HttpClient.GetAsync("https://rutetider.on-it.xyz/api/reklame");
            rsp.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<List<Ad>>(await rsp.Content.ReadAsStringAsync());
        }
    }

    public class TimeTableResponse
    {
        [JsonProperty("hverdag")]
        public TimeTableResponseEntry hverdag { get; set; }

        [JsonProperty("lordag")]
        public TimeTableResponseEntry lordag { get; set; }

        [JsonProperty("sondag")]
        public TimeTableResponseEntry sondag { get; set; }

        public DateTime LastUpdate { get; set; }
    }

    public class TimeTableResponseEntry {
        [JsonProperty("kirklandet")]
        public List<string> kirklandet { get; set; }

        [JsonProperty("innlandet")]
        public List<string> innlandet { get; set; }

        [JsonProperty("goma")]
        public List<string> goma { get; set; }

        [JsonProperty("nordlandet")]
        public List<string> nordlandet { get; set; }
    }
}
