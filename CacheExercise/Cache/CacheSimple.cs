namespace CacheExercise.Cache
{
    public interface IDateTimeService
    {
        DateTime Now();
    }
    public enum CacheConfigurationSetting
    {
        TimeLimited,
        MemoryLimited
    }
    public interface ICache
    {
        void Add(string ticker, decimal stockPrice);
        decimal GetValue(string ticker);
        bool Contains(string ticker);
    }
    public class StockValue
    {
        public decimal StockPrice { get; set; }
        public DateTime TimeStamp { get; set; }
    }
    public class CacheSimple : ICache
    {
        private readonly IDateTimeService _dateTimeService;
        private Dictionary<string, StockValue> StockPriceCache;
        private CacheConfigurationSetting CacheConfigurationSetting { get; set; }

        public CacheSimple( IDateTimeService dateTimeService)
        {
            _dateTimeService = dateTimeService;
            StockPriceCache = new Dictionary<string, StockValue>();
        }

        public decimal GetValue(string ticker)
        {
            if (StockPriceCache.ContainsKey(ticker))
                return StockPriceCache[ticker].StockPrice;

            throw new KeyNotFoundException($"Ticker: {ticker} is not present in the cache as key. Use the IsTickerInCache(ticker) function to avoid this exception.");
        }

        public bool Contains(string ticker)
        {
            if (StockPriceCache.ContainsKey(ticker))
            {
                return true;
            }
            return false;
        }

        public void Add(string ticker, decimal stockPrice)
        {
            var valueToAddToCache = new StockValue()
            {
                StockPrice = stockPrice,
                TimeStamp = _dateTimeService.Now()
            };
            StockPriceCache.Add(ticker, valueToAddToCache);
        }
    }
}
