using Newtonsoft.Json;

namespace FGIAFG.Scraper.GOG.Scraping.JsonData
{
    internal class Logo
    {
        [JsonProperty("image")] public string Image { get; set; }
        [JsonProperty("styles")] public Styles Styles { get; set; }
    }
}
