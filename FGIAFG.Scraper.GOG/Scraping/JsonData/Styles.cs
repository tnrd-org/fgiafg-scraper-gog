using Newtonsoft.Json;

namespace FGIAFG.Scraper.GOG.Scraping.JsonData
{
    internal class Styles
    {
        [JsonProperty("mobile")] public Desktop Mobile { get; set; }
        [JsonProperty("tablet")] public Desktop Tablet { get; set; }
        [JsonProperty("desktop")] public Desktop Desktop { get; set; }
    }
}
