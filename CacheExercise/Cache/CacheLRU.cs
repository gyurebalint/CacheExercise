namespace CacheExercise.Cache
{
    public class CacheLRU : ACache
    {
        private TimeSpan? ExpirationDate { get; set; }
        private int Count;
        private int Capacity;
        private List<string> AddedTickers;

        public CacheLRU(int memoryLimit, IDateTimeService dateTimeService) : base(dateTimeService)
        {
            ExpirationDate = null;
            Capacity = memoryLimit;
            AddedTickers = new List<string>();
        }

        public override void Add(string ticker, StockValue stockValue)
        {
            var valueToAddToCache = new StockValue()
            {
                StockPrice = stockValue.StockPrice,
                TimeStamp = _dateTimeService.Now()
            };

            AddToMemoryLimitedLRUCache(ticker, valueToAddToCache);
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
                var leastRecentlyUsedTicker = AddedTickers[0];
                AddedTickers.RemoveAt(0);
                StockPriceCache.Remove(leastRecentlyUsedTicker);

                StockPriceCache.Add(ticker, valueToAddToCache);
                AddedTickers.Add(ticker);
            }
            else
            {
                throw new ArgumentOutOfRangeException($"You've reached the maximum capacity of the cache, namely: {Capacity}");
            }
        }

        public override decimal GetValue(string ticker)
        {
            if (StockPriceCache.ContainsKey(ticker))
                return StockPriceCache[ticker].StockPrice;

            throw new KeyNotFoundException($"Ticker: {ticker} is not present in the cache as key. Use the IsTickerInCache(ticker) function to avoid this exception.");
        }

        public override bool Contains(string ticker)
        {
            if (StockPriceCache.ContainsKey(ticker))
            {
                var value = StockPriceCache[ticker];
                return true;
            }
            return false;
        }
    }
}
