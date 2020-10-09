#region license
/*
 * MIT License
 * 
 * Copyright (c) 2020 Jerry L. Kirk
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
#endregion

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LockFreeDoublyLinkedLists;

namespace LRUCache
{
    public interface ILRUCacheItem<K, V>
                        where K : IComparable<K>
                        //where V : IEqualityComparer
    {
        K Key { get; }
        V Value { get; set; }
    }

    public class LRUCacheItem<K, V> : ILRUCacheItem<K, V>
                                where K : IComparable<K>
    {
        public K Key { get; private set; }
        public V Value { get; set; }

        public LRUCacheItem(K key, V value)
        {
            Key = key;
            Value = value;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            LRUCacheItem<K, V> objAsLid = obj as LRUCacheItem<K, V>;

            return Key.Equals(objAsLid.Key) && Value.Equals(objAsLid.Value);
        }
    }

    /// <summary>
    /// LRUCache Description
    ///   A Least Recently Used (LRU) Cache organizes items in order of use, allowing you to quickly identify which item hasn't been used for the longest amount of time.
    ///   To find the least-recently used item, look at the beginning of the linked list.

    /// Features
    ///   1. Implemented as a Generic, the user may specify Key and Value types in construction.
    ///   2. TBD -> Add automatic expiration?
    ///   3. TBD -> Add better multi-threaded support. Current implentation is not very good
    /// Goals
    ///   1. All operations should be O(1)
    ///   2. Thread safe for simultaneous users using a single lock (this is not optimal)   
    /// </summary>
    /// 
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
        private int NumRecords = 0;

        public LRUCache_lockfree(int capacity = 10)
        {
            Capacity = capacity;
        }
        public int Count
        {
            // The performance for Count() for items and cache is AWFUL, using my own.
            get => NumRecords;
        }

        /// <summary>
        /// Get a Value by the provide Key. This method does not change the size of the list so there is no need to 
        /// </summary>
        /// <param name="key">Required, may not be null</param>
        /// <returns></returns>
        public N Get(K key)
        {
            if (key == null)
                throw new ArgumentNullException();

            ILockFreeDoublyLinkedListNode<N> value;
            if (items.TryGetValue(key, out value) == true)
            {
                value.Remove(); // Remove it from the someplace in the list
                value = cache.PushLeft(value.Value); // Add it to the left side
                return value.Value;
            }

            throw new KeyNotFoundException(string.Format("Key Not Found: {0}", key.ToString()));
        }

        public void Put(N item)
        {
            if ((item == null) || (item.Key == null) || (item.Value == null))
                throw new ArgumentNullException();

            // Does the Key already exist? If so just update the value and move it to end of the list.
            // This operation can not change the list of the list so we don't care about capacity.
            ILockFreeDoublyLinkedListNode<N> valueNode;
            if (items.TryGetValue(item.Key, out valueNode) == true)
            {
                valueNode.Value.Value = item.Value; // Update the value 
                valueNode.Remove(); // Remove it from the someplace in the list
                cache.PushLeft(valueNode.Value); // Add it to the left side
                return;
            }

            // Add new Node to the two data structures
            valueNode = cache.PushLeft(item); // Add it to the Left side of the list
            items[item.Key] = valueNode;
            Interlocked.Increment(ref NumRecords);

            // Check to see if the cache has reached it's capacity
            if (Count > Capacity)
            {
                // Remove the last item from Cache and it's sibling Value in items becasue it's the oldest
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

        public List<N> ToList()
        {
            var list = new List<N>();
            
            // Loop from least used to Most Used
            foreach (N i in cache)
            {
                list.Add(i);
            }
            return list;
        }
    }
}
