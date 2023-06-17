namespace CacheExercise
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
        void AddToCache(string ticker, decimal stockPrice);
        decimal GetValueByTicker(string ticker);
        bool IsTickerInCache(string ticker);
    }
    public class Cache : ICache
    {
        public class StockValue
        {
            public decimal StockPrice { get; set; }
            public DateTime TimeStamp { get; set; }
        }

        private readonly IDateTimeService _dateTimeService;
        private Dictionary<string, StockValue> StockPriceCache;
        private TimeSpan? ExpirationDate { get; set; }
        private CacheConfigurationSetting CacheConfigurationSetting { get; set; }

        private int Count;
        private int Capacity;
        private List<string> AddedTickers;

        public Cache(CacheConfigurationSetting cacheConfigurationSetting, int memoryLimit, IDateTimeService dateTimeService)
        {
            _dateTimeService = dateTimeService;
            StockPriceCache = new Dictionary<string, StockValue>();
            CacheConfigurationSetting = cacheConfigurationSetting;

            ExpirationDate = null;
            Capacity = memoryLimit;
            AddedTickers = new List<string>();
        }

        public Cache(CacheConfigurationSetting cacheConfigurationSetting, TimeSpan? timeLimit, IDateTimeService dateTimeService)
        {
            _dateTimeService = dateTimeService;
            StockPriceCache = new Dictionary<string, StockValue>();
            ExpirationDate = timeLimit;
            CacheConfigurationSetting = cacheConfigurationSetting;

            Capacity = 0;
            AddedTickers = new List<string>();
        }

        public decimal GetValueByTicker(string ticker)
        {
            if (StockPriceCache.ContainsKey(ticker))
                return StockPriceCache[ticker].StockPrice;

            throw new KeyNotFoundException($"Ticker: {ticker} is not present in the cache as key. Use the IsTickerInCache(ticker) function to avoid this exception.");
        }

        public bool IsTickerInCache(string ticker)
        {
            if (StockPriceCache.ContainsKey(ticker))
            {
                var value = StockPriceCache[ticker];
                if (CacheConfigurationSetting == CacheConfigurationSetting.TimeLimited)
                {
                    return IsDataUpToDate(value);
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsDataUpToDate(StockValue value)
        {
            if (_dateTimeService.Now() - value.TimeStamp > ExpirationDate)
                return false;
            return true;
        }

        public void AddToCache(string ticker, decimal stockPrice)
        {
            var valueToAddToCache = new StockValue()
            {
                StockPrice = stockPrice,
                TimeStamp = _dateTimeService.Now()
            };

            if (CacheConfigurationSetting == CacheConfigurationSetting.MemoryLimited)
            {
                AddToMemoryLimitedLRUCache(ticker, valueToAddToCache);
            }
            else
            {
                AddToNotMemoryLimitedCache(ticker, valueToAddToCache);
            }
        }

        private void AddToMemoryLimitedLRUCache(string ticker, StockValue valueToAddToCache)
        {
            if (Count < Capacity)
            {
                StockPriceCache.Add(ticker, valueToAddToCache);
                AddedTickers.Add(ticker);
                Count++;
            }
            else if (Count == Capacity)
            {
                var lastRecentlyUsedTicker = AddedTickers[0];
                AddedTickers.RemoveAt(0);
                StockPriceCache.Remove(lastRecentlyUsedTicker);

                StockPriceCache.Add(ticker, valueToAddToCache);
                AddedTickers.Add(ticker);
            }
            else
            {
                throw new ArgumentOutOfRangeException($"You've reached the maximum capacity of the cache, namely: {Capacity}");
            }
        }
        private void AddToNotMemoryLimitedCache(string ticker, StockValue valueToAddToCache)
        {
            StockPriceCache.Add(ticker, valueToAddToCache);
            Count++;
        }
    }
}
