using Newtonsoft.Json;

namespace Lykke.AzureRepositories.CandleHistory
{
    public class CandleItem
    {
        [JsonProperty("O")]
        public double Open { get; internal set; }

        [JsonProperty("C")]
        public double Close { get; internal set; }

        [JsonProperty("H")]
        public double High { get; internal set; }

        [JsonProperty("L")]
        public double Low { get; internal set; }

        [JsonProperty("T")]
        public int Tick { get; set; }
    }
}