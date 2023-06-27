using CacheExercise.Cache;

namespace CacheExercise
{
    public interface IStockApi
    {
        decimal GetPriceForTicker(string ticker);
    }

    public class StockRepository
    {
        private readonly IStockApi _stockApi;
        private readonly ICache<string, StockValue> _cache;

        public StockRepository(IStockApi stockApi, ICache<string, StockValue> cache)
        {
            _cache = cache;
            _stockApi = stockApi;
        }

        public decimal GetStockPrice(string ticker)
        {
            decimal stockPrice;
            if (_cache.Contains(ticker))
            {
                stockPrice = _cache.GetValue(ticker);
            }
            else
            {
                stockPrice = _stockApi.GetPriceForTicker(ticker);
                _cache.Add(ticker, new StockValue()
                    { 
                        StockPrice = stockPrice, TimeStamp = DateTime.Now 
                    });
            }

            return stockPrice;
        }

    }
}