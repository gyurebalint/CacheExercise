using CacheExercise;
using Moq;

namespace CacheExerciseTests
{
    public class CacheTests
    {
        #region Testing ICache.IsTickerInCache function
        [Fact]
        public void GivenEmptyCache_WhenIsTickerInCacheIsCalled_ReturnsFalseForBothCacheModes()
        {
            //Arrange
            var inputTestTicker = "AAPL";
            var mockCache = new Mock<ICache>(MockBehavior.Strict);
            var memoryLimit = 10;
            var timeLimit = TimeSpan.FromSeconds(5);
            var mockDateTimeService = new Mock<IDateTimeService>(MockBehavior.Strict);
            mockDateTimeService.Setup(x => x.Now()).Returns(DateTime.Now);

            Cache cacheTimeLimited = new Cache(CacheConfigurationSetting.TimeLimited, timeLimit, mockDateTimeService.Object);
            Cache cacheMemoryLimited = new Cache(CacheConfigurationSetting.MemoryLimited, memoryLimit, mockDateTimeService.Object);

            //Act
            var resultTimeLimited = cacheTimeLimited.IsTickerInCache(inputTestTicker);
            var resultMemoryLimited = cacheMemoryLimited.IsTickerInCache(inputTestTicker);

            //Assert
            Assert.False(resultTimeLimited);
            Assert.False(resultMemoryLimited);
        }

        [Fact]
        public void GivenNonEmptyCacheWithMatchingKeys_WhenIsTickerInCacheIsCalled_ReturnsTrueForMemoryLimitedMode()
        {
            //Arrange
            var inputTestTicker = "AAPL";
            var valueOfTestTicker = 400;
            var memoryCapacityOfCache = 10;
            var mockDateTimeService = new Mock<IDateTimeService>(MockBehavior.Strict);
            mockDateTimeService.Setup(x => x.Now()).Returns(DateTime.Now);

            var mockCache = new Mock<ICache>(MockBehavior.Strict);

            Cache cacheMemoryLimited = new Cache(CacheConfigurationSetting.MemoryLimited, memoryCapacityOfCache, mockDateTimeService.Object);
            cacheMemoryLimited.AddToCache(inputTestTicker, valueOfTestTicker);

            //Act
            var resultMemoryLimited = cacheMemoryLimited.IsTickerInCache(inputTestTicker);

            //Assert
            Assert.True(resultMemoryLimited);
        }

        [Fact]
        public void GivenNonEmptyCacheWithMatchingKeysAndNotExpiredCacheData_WhenIsTickerInCacheIsCalled_ReturnsTrueForTimeLimitedCache()
        {
            //Arrange
            var inputTestTicker = "AAPL";
            var valueOfTestTicker = 400;
            var timeLimit = TimeSpan.FromSeconds(3);
            var mockCache = new Mock<ICache>(MockBehavior.Strict);

            var mockDateTimeService = new Mock<IDateTimeService>(MockBehavior.Strict);
            mockDateTimeService.Setup(x => x.Now()).Returns(new DateTime(1990, 12, 15, 8, 0, 0));

            Cache cacheMemoryLimited = new Cache(CacheConfigurationSetting.TimeLimited, timeLimit, mockDateTimeService.Object);
            cacheMemoryLimited.AddToCache(inputTestTicker, valueOfTestTicker);
            mockDateTimeService.Setup(x => x.Now()).Returns(new DateTime(1990, 12, 15, 8, 0, 2));

            //Act
            var resultMemoryLimited = cacheMemoryLimited.IsTickerInCache(inputTestTicker);

            //Assert
            Assert.True(resultMemoryLimited);
        }

        [Fact]
        public void GivenNonEmptyCacheWithMatchingKeysAndExpiredCacheData_WhenIsTickerInCacheIsCalled_ReturnsFalseForTimeLimitedCache()
        {
            //Arrange
            var inputTestTicker = "AAPL";
            var valueOfTestTicker = 400;
            var timeLimit = TimeSpan.FromSeconds(3);
            var mockCache = new Mock<ICache>(MockBehavior.Strict);

            var mockDateTimeService = new Mock<IDateTimeService>(MockBehavior.Strict);
            mockDateTimeService.Setup(x => x.Now()).Returns(new DateTime(1990, 12, 15, 8, 0, 0));

            Cache cacheMemoryLimited = new Cache(CacheConfigurationSetting.TimeLimited, timeLimit, mockDateTimeService.Object);
            cacheMemoryLimited.AddToCache(inputTestTicker, valueOfTestTicker);
            mockDateTimeService.Setup(x => x.Now()).Returns(new DateTime(1990, 12, 15, 8, 0, 4));

            //Act
            var resultMemoryLimited = cacheMemoryLimited.IsTickerInCache(inputTestTicker);

            //Assert
            Assert.False(resultMemoryLimited);
        }
        #endregion

        #region Testing ICache.AddToCacheIsCalled
        [Fact]
        public void GivenNotFullCapacityMemoryLimitedCache_WhenAddToCacheIsCalled_SavesTickersAndValuesIntoCache()
        {
            //Arrange
            var inputTestTicker = "AAPL";
            var valueOfTestTicker = 400;

            var memoryCapacityOfCache = 10;
            var mockDateTimeService = new Mock<IDateTimeService>(MockBehavior.Strict);
            mockDateTimeService.Setup(x => x.Now()).Returns(DateTime.Now);

            var mockCache = new Mock<ICache>(MockBehavior.Strict);

            Cache cacheMemoryLimited = new Cache(CacheConfigurationSetting.MemoryLimited, memoryCapacityOfCache, mockDateTimeService.Object);

            //Act
            for (int i = 0; i < 5; i++)
            {
                var tickerName = inputTestTicker + "_" + i;
                cacheMemoryLimited.AddToCache(tickerName, valueOfTestTicker + i);
            }


            //Assert
            for (int i = 0; i < 5; i++)
            {
                var tickerName = inputTestTicker + "_" + i;
                var resultMemoryLimited = cacheMemoryLimited.IsTickerInCache(tickerName);
                Assert.True(resultMemoryLimited);
            }
        }

        [Fact]
        public void GivenFullCapacityMemoryLimitedCache_WhenAddToCacheIsCalled_LeastRecentDataIsDeletedAndNewTickersAndValuesAddedIntoCache()
        {
            //Arrange
            var inputTestTicker = "AAPL";
            var valueOfTestTicker = 400;

            var memoryCapacityOfCache = 5;
            var mockDateTimeService = new Mock<IDateTimeService>(MockBehavior.Strict);
            mockDateTimeService.Setup(x => x.Now()).Returns(DateTime.Now);

            var mockCache = new Mock<ICache>(MockBehavior.Strict);

            Cache cacheMemoryLimited = new Cache(CacheConfigurationSetting.MemoryLimited, memoryCapacityOfCache, mockDateTimeService.Object);

            //Act
            for (int i = 0; i <= memoryCapacityOfCache; i++)
            {
                var tickerName = inputTestTicker + "_" + i;
                cacheMemoryLimited.AddToCache(tickerName, valueOfTestTicker + i);
            }


            //Assert
            for (int i = 1; i <= memoryCapacityOfCache; i++)
            {
                var tickerName = inputTestTicker + "_" + i;
                var resultMemoryLimited = cacheMemoryLimited.IsTickerInCache(tickerName);
                Assert.True(resultMemoryLimited);
            }

            var leastRecentCachedData = cacheMemoryLimited.IsTickerInCache(inputTestTicker + "_" + 0);
            Assert.False(leastRecentCachedData);
        }
        #endregion

        #region Testing ICache.GetValueByTicker
        [Fact]
        public void GivenEmptyCache_WhenGetValueByTickerIsCalled_ThrowKeyNotFoundException()
        {
            var inputTestTicker = "AAPL";
            var mockCache = new Mock<ICache>(MockBehavior.Strict);
            var memoryLimit = 10;
            var timeLimit = TimeSpan.FromSeconds(5);
            var mockDateTimeService = new Mock<IDateTimeService>(MockBehavior.Strict);
            mockDateTimeService.Setup(x => x.Now()).Returns(DateTime.Now);

            Cache cacheTimeLimited = new Cache(CacheConfigurationSetting.TimeLimited, timeLimit, mockDateTimeService.Object);
            Cache cacheMemoryLimited = new Cache(CacheConfigurationSetting.MemoryLimited, memoryLimit, mockDateTimeService.Object);

            //Act
            Action exceptionTimeLimited = () => cacheTimeLimited.GetValueByTicker(inputTestTicker);
            Action exceptionMemoryLimited = () => cacheMemoryLimited.GetValueByTicker(inputTestTicker);

            //Assert
            KeyNotFoundException keyNotFoundException1 = Assert.Throws<KeyNotFoundException>(exceptionTimeLimited);
            KeyNotFoundException keyNotFoundException2 = Assert.Throws<KeyNotFoundException>(exceptionMemoryLimited);
        }

        [Fact]
        public void GivenNonEmptyCacheWithMatchingKeys_WhenGetValueByTickerIsCalled_ReturnsTheValueOfAskedKeys()
        {
            var inputTestTicker = "AAPL";
            decimal inputTestValue = 404.4m;
            var mockCache = new Mock<ICache>(MockBehavior.Strict);
            var memoryLimit = 10;
            var timeLimit = TimeSpan.FromSeconds(5);
            var mockDateTimeService = new Mock<IDateTimeService>(MockBehavior.Strict);
            mockDateTimeService.Setup(x => x.Now()).Returns(DateTime.Now);

            Cache cacheTimeLimited = new Cache(CacheConfigurationSetting.TimeLimited, timeLimit, mockDateTimeService.Object);
            Cache cacheMemoryLimited = new Cache(CacheConfigurationSetting.MemoryLimited, memoryLimit, mockDateTimeService.Object);
            cacheTimeLimited.AddToCache(inputTestTicker, inputTestValue);
            cacheMemoryLimited.AddToCache(inputTestTicker, inputTestValue);


            //Act
            var resultTimeLimited = cacheTimeLimited.GetValueByTicker(inputTestTicker);
            var resultMemoryLimited = cacheMemoryLimited.GetValueByTicker(inputTestTicker);

            //Assert
            Assert.Equal(inputTestValue, resultTimeLimited);
            Assert.Equal(inputTestValue, resultMemoryLimited);
        }

        #endregion
    }
}