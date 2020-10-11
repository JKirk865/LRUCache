using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using LockFreeDoublyLinkedLists;

namespace LRUCache
{
    /// <typeparam name="N">The Object that will be cached that contained the Key and Value</typeparam>
    /// <typeparam name="K">Key</typeparam>
    /// <typeparam name="V">Value</typeparam>
    public class LRUCache_lockfree<N, K, V> : ILRUCache<N, K> 
                                            where N : LRUCacheItem<K, V>
                                            where K : IComparable<K>
    {
        public int Capacity { get; private set; } // Capacity can not be changed once it is specified in the constructor
        
        // This is the doubly linked list that tracks the ORDER of the items. Left is more recent, Right is oldest.
        private ILockFreeDoublyLinkedList<N> cache = LockFreeDoublyLinkedList.Create<N>();
        // Holds the Key/Value for O(1) lookup, holds the CacheItem for O(1) Removal
        private ConcurrentDictionary<K, ILockFreeDoublyLinkedListNode<N>> items = 
            new ConcurrentDictionary<K, ILockFreeDoublyLinkedListNode<N>>();
        private int NumRecords = 0; // The performance for items.Count() is AWFUL, using my own.

        public LRUCache_lockfree(int capacity = 10)
        {
            Capacity = capacity;
        }
        public int Count
        {            
            get => NumRecords;
        }

        public N Get(K key)
        {
            if (key == null)
                throw new ArgumentNullException();

            ILockFreeDoublyLinkedListNode<N> value;
            if (items.TryGetValue(key, out value) == true)
            {
                value.Remove(); // Remove it from the someplace in the "cache" list
                if (value.Value.IsExpired)
                {
                    items.TryRemove(key, out value);
                    throw new KeyNotFoundException(string.Format("Key expired: {0}", key.ToString()));
                }
                value = cache.PushLeft(value.Value); // Add it to the left side
                return value.Value;
            }

            throw new KeyNotFoundException(string.Format("Key Not Found: {0}", key.ToString()));
        }

        public void Put(N item)
        {
            if ((item == null) || (item.Key == null) || (item.Value == null))
                throw new ArgumentNullException();

            item.UpdateExpiration();

            // Does the Key already exist? If so just update the value and move it to end of the list.
            // The cache size does not change.
            ILockFreeDoublyLinkedListNode<N> valueNode;
            if (items.TryGetValue(item.Key, out valueNode) == true)
            {
                valueNode.Value.Value = item.Value; // Update the value 
                valueNode.Remove(); // Remove it from the someplace in the list
                cache.PushLeft(valueNode.Value); // Add it to the left side
                return;
            }

            // Add new Node to the two data structures
            items[item.Key] = cache.PushLeft(item); // Add it to the Left side of the list
            Interlocked.Increment(ref NumRecords);

            // Check to see if the cache has reached it's capacity
            if (Count > Capacity)
            {
                // Remove the last item from Cache and it's sibling Value in items because it's the oldest
                valueNode = cache.Tail;
                while (valueNode.IsDummyNode)
                    valueNode = valueNode.Prev;

                var success = items.TryRemove(valueNode.Value.Key, out valueNode);
                if (success)
                {
                    cache.PopRightNode();
                    Interlocked.Decrement(ref NumRecords);
                }
            }
        }
        public bool Remove(K key)
        {
            if (key == null)
                throw new ArgumentNullException();

            ILockFreeDoublyLinkedListNode<N> valueNode;
            if (items.TryRemove(key, out valueNode) == true)
            {
                valueNode.Remove(); // Remove it from the someplace in the list
                return true;
            }

            return false;
        }
        public List<N> ToList()
        {
            var list = new List<N>();
            
            // Loop from Least used to Most Used
            foreach (N i in cache)
            {
                list.Add(i);
            }
            return list;
        }
    }
}
