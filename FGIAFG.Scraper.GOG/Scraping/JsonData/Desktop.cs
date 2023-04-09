using Newtonsoft.Json;

namespace FGIAFG.Scraper.GOG.Scraping.JsonData
{
    internal class Desktop
    {
        [JsonProperty("top")] public string Top { get; set; }
        [JsonProperty("left")] public string Left { get; set; }
        [JsonProperty("width")] public string Width { get; set; }
    }
}
