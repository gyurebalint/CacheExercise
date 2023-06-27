namespace CacheExercise.Cache
{
    public class CacheTimeLimited : ICache
    {
        private readonly IDateTimeService _dateTimeService;
        private Dictionary<string, StockValue> StockPriceCache;
        private TimeSpan? ExpirationDate { get; set; }

        private int Count;
        private int Capacity;
        private List<string> AddedTickers;

        public CacheTimeLimited(IDateTimeService dateTimeService, TimeSpan? timeLimit)
        {
            _dateTimeService = dateTimeService;
            StockPriceCache = new Dictionary<string, StockValue>();
            ExpirationDate = timeLimit;

            Capacity = 0;
            AddedTickers = new List<string>();
        }
        public void Add(string ticker, decimal stockPrice)
        {
            var valueToAddToCache = new StockValue()
            {
                StockPrice = stockPrice,
                TimeStamp = _dateTimeService.Now()
            };

            StockPriceCache.Add(ticker, valueToAddToCache);
            Count++;
        }

        public bool Contains(string ticker)
        {
            if (StockPriceCache.ContainsKey(ticker))
            {
                var value = StockPriceCache[ticker];
                return IsDataUpToDate(value);
            }
            return false;
        }
        private bool IsDataUpToDate(StockValue value)
        {
            if (_dateTimeService.Now() - value.TimeStamp > ExpirationDate)
                return false;
            return true;
        }

        public decimal GetValue(string ticker)
        {
            if (StockPriceCache.ContainsKey(ticker))
                return StockPriceCache[ticker].StockPrice;

            throw new KeyNotFoundException($"Ticker: {ticker} is not present in the cache as key. Use the IsTickerInCache(ticker) function to avoid this exception.");
        }
    }
}
