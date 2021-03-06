# LRUCache
A C# .Net Core implementation of a LRUCache with optional expiration.

## What is an LRU Cache?
A Least Recently Used (LRU) Cache organizes items in order of use, allowing you to quickly identify which
item hasn't been used for the longest amount of time. It is a common cache to use when the backing store is slow and
the application frequently retrieves the same information (by a unique key) frequently.

## Why did I develop this code?
I was asked to develop an LRU Cache in an technical interview but at that time I was not familiar with the concept. I was
looking for a suitable project to investigate new-to-me C# concepts and selected this.

## History of the project

**Version 1**
A singly linked list that was not thread safe and had poor O(N) performance issues. This version is buried deep
in the repo history and has no business being reviewed.

**LRUCache_lock**
A fast O(1) implemented as a generic allowing the user to define their own key/value structure. While this version
is thread-safe it does so with a single lock shared for all the operations.  But, it is still faster than the
"LRUCache_nolock" version discussed below until the user applies MANY asynchronous operations. When my first linked list
implementation had such poor performance I investigated other architectures and found this inspiration from a developer *"yozaam"* 
(https://www.youtube.com/watch?v=zDknUrGFoxI&t) 

**LRUCache_nolock**
A fast O(1) implemented as a generic allowing the user to define their on Key/Value structure. This implementation is
"lock free" and more ideal for highly multi-threaded users. It architecture is the same as "LRUCache_lock" but makes
use of a lock free doubly linked list class that I did not write. The underlying data structure is borrowed from
*https://github.com/c7hm4r/LockFreeDoublyLinkedList* which is C# .Net Core 2.0 implementation of the paper
*Lock-free deques and doubly linked lists� by H�kan Sundell and Philippas Tsigas (2008)*.

## Performance
Surprisingly the "LRUCache_lock" implementation has better single and multi-threaded performance than I expected. It is simpler
and if you are looking to use an LRUCache I would start with this one.

The "LRUCache_nolock" performance is better for lots(50+) of threads. I plan to investigate this further and see
if I can identify any additional bottlenecks. I initially found that the *Count* operation was very expensive and
now use an *Interlocked.Increment* to keep track of the size.

## Architecture
Both implementations use a similar architecture. A dictionary is used to hold each Key/Value for fast O(1) lookup, and a second linked
list that just carries the Key is used to maintain order, from oldest (Left) to newest(Right). This means that the key/value may be
stored twice but the performance is much better than a single linked list could ever achieve.

**Expiration**
The Expiration support varies between the two implementations. The "LRUCache_lock" contains a method to remove the expires nodes
named *RemoveExpired()* which is automatically called in the Count property. Due to the nature of "LRUCache_nolock" this is not
possible. The "LRUCache_nolock" implantation will not return a expired node, but it will keep them and consider them part of the
Count until they are ejected due to capacity or an attempt to Get them.

## Usage
**First**
Define the object to be cached and inherit from the LRUCacheItem class. The key must be comparable(i.e. int, Guid)
```
    public class SimpleLRUCacheItem : LRUCacheItem<int, string>
    {
        public SimpleLRUCacheItem(int key, string value, TimeSpan? lifetime = null)
            : base(key, value, lifetime)
        {
            // Nothing to do here
        }
    }
```
**Second**
Instantiate the class but be sure to use the ILRUCache so you can change which implementation you are using.
```
       ILRUCache<SimpleLRUCacheItem, int> c = new LRUCache_lockfree<SimpleLRUCacheItem, int, string>(int Capacity = 10);
       
       The generic takes three arguments:
         N => The object to be cached
         K => The type of the Key that will be used
         V => The type of the value object that will be stored
```
**Examples**
```
    c.Put(new SimpleLRUCacheItem(1, "Red")); // Becomes the Left most item with no expiration date
    c.Put(new SimpleLRUCacheItem(2, "Blue", new TimeSpan(0,0,5))); // Becomes the Left most item with a 5 second lifetime
    var numItems = c.Count;
    SimpleLRUCacheItem n = c.Get(1); // After found, becomes the Left most item
    List<SimpleLRUCacheItem> itemList = c.ToList(); // Note, the first item in the list is the oldest
```
## Unit Tests
For now the unit tests are just ad hoc test ideas I had during development to target specific areas. Additional unit tests
could be added in the future to ensure 100% code coverage.

## Future Features


## Class Diagram
![Class Diagram](/ClassDiagram.png)