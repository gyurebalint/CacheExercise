namespace CacheExercise.Cache
{
    public static class CacheFactory
    {
        public static ICache CreateCache(CacheConfigurationSetting cacheConfigurationSetting, IDateTimeService dateTimeService, int memoryLimit = 0, TimeSpan? timeLimit = null)
        {
            switch (cacheConfigurationSetting)
            {
                case CacheConfigurationSetting.MemoryLimited:
                    return CreateLRUCache(memoryLimit, dateTimeService);
                case CacheConfigurationSetting.TimeLimited:
                    return new CacheTimeLimited(dateTimeService, timeLimit);
                default:
                    return new CacheSimple(dateTimeService);
            }
        }

        private static ICache CreateLRUCache(int memoryLimit, IDateTimeService dateTimeService)
        {
            if (memoryLimit <= 0) throw new ArgumentOutOfRangeException("MemoryLimit cannot be <= 0, if cache is memory limited, the memory must be larger than 0");
            return new CacheLRU(memoryLimit, dateTimeService);
        }

        private static ICache CreateTimeLimitedCache(IDateTimeService dateTimeService, TimeSpan? timeLimit)
        {
            if (timeLimit == null) throw new ArgumentOutOfRangeException("TimeLimit must not be null");
            return new CacheTimeLimited(dateTimeService, timeLimit);
        }
    }
}
