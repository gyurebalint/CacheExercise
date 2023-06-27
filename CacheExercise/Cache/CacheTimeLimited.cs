namespace CacheExercise.Cache
{
    public class CacheTimeLimited : ACache
    {
        private TimeSpan? ExpirationDate { get; set; }
        private int Count;
        private int Capacity = 0;
        private List<string> AddedTickers = new List<string>();

        public CacheTimeLimited(IDateTimeService dateTimeService, TimeSpan? timeLimit) : base(dateTimeService)
        {
            ExpirationDate = timeLimit;
        }
        public override void Add(string ticker, StockValue stockValue)
        {
            var valueToAddToCache = new StockValue()
            {
                StockPrice = stockValue.StockPrice,
                TimeStamp = _dateTimeService.Now()
            };

            StockPriceCache.Add(ticker, valueToAddToCache);
            Count++;
        }

        public override bool Contains(string ticker)
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

        public override decimal GetValue(string ticker)
        {
            if (StockPriceCache.ContainsKey(ticker))
                return StockPriceCache[ticker].StockPrice;

            throw new KeyNotFoundException($"Ticker: {ticker} is not present in the cache as key. Use the IsTickerInCache(ticker) function to avoid this exception.");
        }
    }
}
