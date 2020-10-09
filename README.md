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
A fast O(1) implemented as a generic allowing the user to define their on Key/Value structure.
