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
    public interface ICache<TKey, TValue>
    {
        void Add(TKey key, TValue value);
        decimal GetValue(TKey value);
        bool Contains(TKey value);
    }
    public class StockValue
    {
        public decimal StockPrice { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public abstract class ACache : ICache<string, StockValue>
    {
        protected Dictionary<string, StockValue> StockPriceCache { get; set; } = new Dictionary<string, StockValue>();
        protected IDateTimeService _dateTimeService { get; set; }
        public ACache(IDateTimeService dateTimeService)
        {
            _dateTimeService = dateTimeService;
        }
        public abstract void Add(string key, StockValue value);

        public abstract bool Contains(string value);

        public abstract decimal GetValue(string value);
        
    }
    public class CacheSimple : ACache
    {
        public CacheSimple(IDateTimeService dateTimeService):base(dateTimeService){}

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
                return true;
            }
            return false;
        }

        public override void Add(string ticker, StockValue stockValue)
        {
            var valueToAddToCache = new StockValue()
            {
                StockPrice = stockValue.StockPrice,
                TimeStamp = _dateTimeService.Now()
            };
            StockPriceCache.Add(ticker, valueToAddToCache);
        }
    }
}
