using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomansBuyerScraper
{
    public class House
    {

        [JsonProperty(PropertyName = "price")]
        public decimal Price { get; set; }

        [JsonProperty(PropertyName = "town")]
        public string Town { get; set; }

        [JsonProperty(PropertyName = "county")]
        public string County { get; set; }

        [JsonProperty(PropertyName = "postcode")]
        public string Postcode { get; set; }

        [JsonProperty(PropertyName = "street")]
        public string Street { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "listedon")]
        public string ListedOn { get; set; }

        [JsonProperty(PropertyName = "estateagentId")]
        public string EstateAgentId { get; set; }

        [JsonProperty(PropertyName = "estageagent")]
        public string EstateAgent { get; set; }

        [JsonProperty(PropertyName = "created")]
        public string Created { get; set; }

        [JsonProperty(PropertyName = "domain")]
        public string Domain { get; set; }

        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        [JsonProperty(PropertyName = "purchase")]
        public string Purchase { get; set; }

        [JsonProperty(PropertyName = "houseurl")]
        public string HouseUrl { get; set; }

        public virtual ICollection<HouseImage> HouseImages { get; set; }
    }
}
