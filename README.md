# LRUCache
A C# .Net Core implementation of a LRUCache with UnitTests.

## What is an LRU Cache?
A Least Recently Used (LRU) Cache organizes items in order of use, allowing you to quickly identify which
item hasn't been used for the longest amount of time. It is a common cache to use when the backing store is slow and
the application frequently retrieves the same information by unique key frequently.

## Why did I develop this code?
I was asking to develop an LRU Cache in an technical interview but at that time I was not familiar with the concept,
so it does not go well. I was looking for a suitable project to investigate new-to-me C# concepts and selected this.

## History of the project

**Version 1**
A singly linked list that was not thread safe and had poor O(N) performance issues. This version is buried deep
in the repo history and has business bring reviewed.

**LRUCache_lock**
A fast O(1) implemented as a generic allowing the user to define their on Key/Value structure. While this version
is thread-safe it does so with a single lock shared for all the operations.  But, it is till overall faster than the
"LRUCache_nolock" version discussed below until the user applies MANY asynchronous operations. When my first linked list
implementation had such poor performance I investigated other architecture and got inspiration from a developer *"yozaam"* 
(https://www.youtube.com/watch?v=zDknUrGFoxI&t) 

**LRUCache_nolock**
A fast O(1) implemented as a generic allowing the user to define their on Key/Value structure. This implementation is
"lock free" and more ideal for highly multi-threaded users. It architecture is the same the  "LRUCache_lock" but makes
us of a lock free doubly linked list class that I did not write. The underlying data structure is borrowed from
*https://github.com/c7hm4r/LockFreeDoublyLinkedList* which is C# .Net Core 2.0 implementation of the paper
*Lock-free deques and doubly linked lists” by Håkan Sundell and Philippas Tsigas (2008)*.

## Performance
Surprisingly the "LRUCache_lock" implementation has better single and multi-threaded performance than I expected. It is simpler
and if you are looking to use an LRUCache I would start with this one.

The "LRUCache_nolock" performance is better for lots(50+) of threads. I plan to investigate this further and see
if I can identify any additional bottlenecks. I initially found that the *Count* operation was very expensive and
now use an *Interlocked.Increment* to keep track of the size.

## Usage Examples

**First define the object to be cached and inherit from the LRUCacheItem. The key must be comparable(int, Guid, etc.)
    public class SimpleLRUCacheItem : LRUCacheItem<int, string>
    {
        public SimpleLRUCacheItem(int key, string value)
            : base(key, value)
        {
            // Nothing to do here
        }
    }

**Second Instantiate the class but be sure to use the ILRUCache so you can change which implementation you are using.
       ILRUCache<SimpleLRUCacheItem, int> c = new LRUCache_lockfree<SimpleLRUCacheItem, int, string>(int Capacity = 10);
       The generic takes three arguments:
         N => The object to be cached
         K => The type of the Key that will be used
         V => The type of the value object that will be stored

** Usage
  c.Put(new SimpleLRUCacheItem(1, "Red"));
  var numItems = c.Count;
  SimpleLRUCacheItem n = c.Get(1);
  List<SimpleLRUCacheItem> itemList = c.ToList(); // Note, the first item in the list is the oldest

## Unit Tests
For now the unit tests are just the ad hoc test ideas I had during development to target specific areas. For complete unit tests
could probably be done in the future to ensure 100% code coverage.

## Architecture
Both implementations use a similar architecture. A dictionary is used to hold each Key/Value for fast O(1) lookup, and a second linked
list that just carries the Key is used to maintain order, from oldest (Left) to newest(Right). This means that the key/value may be
stored twice but the performance is much better than a single linked list could ever achieve.

## Future Features
1. Add expiration date to each node, handy to ensure cache'd item is not too old
2. Remove node, handy if the node needs to be removed because the underlying data item was changed
3. Clear all nodes


