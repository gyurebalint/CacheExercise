namespace CacheExercise
{
    public interface IStockApi
    {
        decimal GetPriceForTicker(string ticker);
    }

    public class StockRepository
    {
        private readonly IStockApi _stockApi;
        private readonly ICache _cache;

        public StockRepository(IStockApi stockApi, ICache cache)
        {
            _cache = cache;
            _stockApi = stockApi;
        }

        public decimal GetStockPrice(string ticker)
        {
            decimal stockPrice;
            if (_cache.IsTickerInCache(ticker))
            {
                stockPrice = _cache.GetValueByTicker(ticker);
            }
            else
            {
                stockPrice = _stockApi.GetPriceForTicker(ticker);
                _cache.AddToCache(ticker, stockPrice);
            }

            return stockPrice;
        }

    }
}