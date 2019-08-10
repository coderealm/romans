using Newtonsoft.Json;

namespace RomansBuyerScraper
{
    public class HouseImage
    {
        [JsonProperty(PropertyName = "imageurl")]
        public string ImageUrl { get; set; }
    }
}
