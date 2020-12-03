namespace OrderProcessor
{
    public class Settings
    {
        public string OrderDirectory { get; set; }

        public string DestinationDirectory { get; set; }

        public string ApiEndpoint { get; set; }

        public double TaxRatePercent { get; set; }
    }
}