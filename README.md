# Cache excercise

For project structure I left it as it was, I didn't want to break it up too much, it makes it easier for you to grasp what's going on.

## Problem
1. Your code must have a single entry point, allowing us to fetch the price of a stock. For example: getStockPrice('TSLA') // 442.25
2. Users must not be able to decide whether or not they receive a cached value.
3. We only want to call the third party once for each stock (except in other cache modes)
4. The cache should also be configurable into different modes:
    - Limited capacity - the cache should hold a limited number of items, and evict entries when it is full.
    - Limited time - the cache should store entries for a limited amount of time. If the entry has expired, it should retrieve a fresh value.

## Solution
1. done
2. done - workings of cache is hidden from user
3. done
4. There are two cache modes, `MemoryLimited` and `TimeLimited`. You can set the mode when instantiating the `Cache` class.
    - MemoryLimited: Least Recently Used
    - TimeLimited: Data in cache can expire

## In the future (for fun) <br>
Could be other modes e.g.: Least frequently used.

If I created another mode, I think i'd just create seperate cache classes that all inherits from the same interface `ICache` or inherit from some abstract class. I would not want to make `Cache` class too dense.

If that so i'd probably use a `CacheFactory` to actually create the different instances of the necessary cache based on the configuration:
- MemoryLimited
- Least Frequently Used
- TimeLimited