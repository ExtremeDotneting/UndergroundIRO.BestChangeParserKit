namespace BestChangeParserKit.Models
{
    public struct BestChangePairInfo
    {
        public string ExchangerTitle { get; set; }

        public string ExchangerUrl { get; set; }

        public string CoinNameFrom { get; set; }

        public string CoinNameTo { get; set; }

        /// <summary>
        /// CoinFrom * Rate = CoinTo.
        /// </summary>
        public decimal? Rate { get; set; }

        public decimal? Min_FromCoin { get; set; }

        public decimal? Fund { get; set; }

        public bool? IsFloatingRate { get; set; }

        public bool? IsManual { get; set; }

        public bool? NeedVerification { get; set; }
    }
}
