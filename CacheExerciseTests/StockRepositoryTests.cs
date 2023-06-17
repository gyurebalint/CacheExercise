using CacheExercise;
using Moq;

namespace CacheExerciseTests
{
    public class StockRepositoryTests
    {
        #region Testing StockRepository.GetStockPrice
        [Theory]
        [InlineData("TSL", 444)]
        [InlineData("AAPL", 444.5)]
        public void GivenValidTickerThatIsNotInCache_WhenCallingGetStockPrice_ReturnsStockPriceByCallingThirdPartyApi(string input, decimal expected)
        {
            //Arrange
            var mockCache = new Mock<ICache>(MockBehavior.Strict);
            mockCache.Setup(x => x.IsTickerInCache(input))
                .Returns(false);

            var mockStockApi = new Mock<IStockApi>(MockBehavior.Strict);
            mockStockApi.Setup(p => p.GetPriceForTicker(It.IsAny<string>()))
                .Returns(expected);

            mockCache.Setup(x => x.AddToCache(input, expected))
                .Verifiable();

            StockRepository stock = new StockRepository(mockStockApi.Object, mockCache.Object);

            //Act
            //cache empty
            var sut = stock.GetStockPrice(input);
            //cache filled


            //Assert
            Assert.Equal(expected, sut);
            mockCache.Verify();
        }

        [Theory]
        [InlineData("TSL", 444)]
        [InlineData("AAPL", 444.5)]
        public void GivenValidInputThatIsInCache_WhenCallingGetStockPrice_ReturnsStockPriceFromCache(string input, decimal expected)
        {
            //Arrange
            var mockCache = new Mock<ICache>(MockBehavior.Strict);
            mockCache.Setup(x => x.IsTickerInCache(input))
                .Returns(true);

            mockCache.Setup(x => x.GetValueByTicker(It.IsAny<string>()))
                .Returns(expected);

            var mockStockApi = new Mock<IStockApi>(MockBehavior.Strict);
            StockRepository stock = new StockRepository(mockStockApi.Object, mockCache.Object);

            //Act
            //cache empty
            var sut = stock.GetStockPrice(input);
            //cache filled

            //Assert
            Assert.Equal(expected, sut);
        }

        #endregion
    }
}