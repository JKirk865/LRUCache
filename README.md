# LRUCache
A C# .Net Core implementation of a LRUCache with UnitTests.

## What is an LRU Cache?
A Least Recently Used (LRU) Cache organizes items in order of use, allowing you to quickly identify which
item hasn't been used for the longest amount of time. It is a common cache to use when the backing store is slow and
the application frequently retrives the same information by unique key frequently.

## Why did I develop this code?
I was asking to develop an LRU Cache in an technical interview but at that time I was not familiar with the concept,
so it does not go well. I was lookinug for a suitable project to investigate new-to-me C# concepts and selected this.

## History of the project

**Version 1**
A singley linked list that was not thread safe and had poor O(N) performance issues. This version is buried deep
in the repo history and has business bring reviewed.

**LRUCache_lock**
A fast O(1) implemented as a generic allowing the user to define their on Key/Value structure. While this version
is thread-safe it does so with a single lock shared for all thge operations.  But, it is till overall faster than the
"LRUCache_nolock" version discused below until the user applies MANY asynchronous operations.

**LRUCache_nolock**
A fast O(1) implemented as a generic allowing the user to define their on Key/Value structure. This implementation is
"locl free" and more ideal for highly multiu-threaed use. It archtecture is the same the  "LRUCache_lock" but makes
us of a lock free doubly linked list class that I did not write.

## Performance
Surprisingly the "LRUCache_lock" has better single and multi-threaded performance than I expected. It is simpler
and if you are looking to use an LRUCache I would start with this one.

The "LRUCache_nolock" performance is better for lots(50+) of threads. I plamn to investigate this further and see
if I can idenentify any additional bottlenecks. I initially found that the Count() operation was very expensive and
now use an Interlocked.Increment to keep track of the size.

##Usage Examples

