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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LockFreeDoublyLinkedLists;

namespace LRUCache
{
    /// <summary>
    /// LRUCache Description
    ///   A Least Recently Used (LRU) Cache organizes items in order of use, allowing you to quickly identify which item hasn't been used for the longest amount of time.
    ///   To find the least-recently used item, look at the beginning of the linked list. The Key

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
    public class LRUCache_lockfree : ILRUCache<int, string>
    {
        public int Capacity { get; private set; } // Capacity can not be changed once it is specified in the constructor
        
        // This is the doubly linked list that tracks the ORDER of the items. Left is more recent, Right is oldest.
        private ILockFreeDoublyLinkedList<CacheItem> cache = LockFreeDoublyLinkedList.Create<CacheItem>();
        // Holds the Key/Value for O(1) lookup.
        private ConcurrentDictionary<int, ILockFreeDoublyLinkedListNode<CacheItem>> items = 
            new ConcurrentDictionary<int, ILockFreeDoublyLinkedListNode<CacheItem>>(); 

        public LRUCache_lockfree(int capacity = 10)
        {
            Capacity = capacity;
        }
        public int Count
        {
            get => cache.Count();
        }
        
        private void RemoveFromCache(int key)
        {
        }

        /// <summary>
        /// Get a Value by the provide Key. This method does not change the size of the list so there is no need to 
        /// </summary>
        /// <param name="key">Required, may not be null</param>
        /// <returns></returns>
        public string Get(int key)
        {
            if (key == null)
                throw new ArgumentNullException();

            //lock (cache_lock)
            {
                ILockFreeDoublyLinkedListNode<CacheItem> value;
                if (items.TryGetValue(key, out value) == true)
                {

                    value.Remove(); // Remove it from the someplace in the list
                    cache.PushLeft(value.Value); // Add it to the left side
                    return value.Value.Value;
                }
            }

            throw new KeyNotFoundException(string.Format("Key Not Found: {0}", key.ToString()));
        }

        public void Put(int key, string value)
        {
            if ((key == null) || (value == null))
                throw new ArgumentNullException();

            // Does the Key already exist? If so just update the value and move it to end of the list.
            // This operation can not change the list of the list so we don't care about capacity.
            ILockFreeDoublyLinkedListNode<CacheItem> valueNode;
            if (items.TryGetValue(key, out valueNode) == true)
            {
                valueNode.Value.Value = value; // Update the value 
                valueNode.Remove(); // Remove it from the someplace in the list
                cache.PushLeft(valueNode.Value); // Add it to the left side
                return;
            }

            // Add new Node
            valueNode = cache.PushLeft(new CacheItem(key, value)); // Add it to the Left side of the list
            items[key] = valueNode;
                
            // Check to see if the cache has reached it's capacity
            if (cache.Count() > Capacity)
            {
                // Remove the last item from Cache and it's sibling Value in items becasue it's the oldest
                valueNode = cache.Tail;
                while (valueNode.IsDummyNode)
                    valueNode = valueNode.Prev;

                var success = items.TryRemove(valueNode.Value.Key, out valueNode);
                if (success)
                {
                    cache.PopRightNode();
                }
            }
        }

        public List<KeyValuePair<int, string>> ToList()
        {
            var list = new List<KeyValuePair<int, string>>();
            
            // Loop from least used to Most Used
            foreach (CacheItem i in cache)
            {
                var t = new KeyValuePair<int, string>(i.Key, i.Value);
                list.Add(t);
            }
            return list;
        }

        internal class CacheItem
        {
            public int Key { get; private set; }
            public string Value { get; set; }

            public CacheItem(int key, string value)
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

                CacheItem objAsLid = obj as CacheItem;

                return Key == objAsLid.Key
                    && Value == objAsLid.Value;
            }
        }
    }
}
