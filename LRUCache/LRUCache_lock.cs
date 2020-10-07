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
using System.Collections.Generic;

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
    public class LRUCache_lock<K, V> : ILRUCache<K, V>
    {
        public int Capacity { get; private set; } // Capacity can not be changed once it is specified in the constructor
        private LinkedList<K> cache = new LinkedList<K>(); // Holds the Keys in order from (FRONT) least used to (Last) recently used/added.
        private Dictionary<K, V> items = new Dictionary<K, V>();        // Holds the Key/Value for O(1) lookup.
        private object cache_lock = new object();

        public LRUCache_lock(int capacity = 10)
        {
            Capacity = capacity;
        }

        public int Count
        {
            get
            {
                lock (cache_lock)
                {
                    return cache.Count;
                }
            }
        }
        /// <summary>
        /// Get a Value by the provide Key. This method does not change the size of the list so there is no need to 
        /// </summary>
        /// <param name="key">Required, may not be null</param>
        /// <returns></returns>
        public V Get(K key)
        {
            if (key == null)
                throw new ArgumentNullException();

            lock (cache_lock)
            {
                V value;
                if (items.TryGetValue(key, out value) == true)
                {

                    cache.Remove(key); // Remove it from the someplace in the list
                    cache.AddLast(key); // Add it to the END of the list
                    return value;
                }
            }

            throw new KeyNotFoundException(string.Format("Key Not Found: {0}", key.ToString()));
        }



        public void Put(K key, V Value)
        {
            if ((key == null) || (Value == null))
                throw new ArgumentNullException();

            lock (cache_lock)
            {
                // Does the Key already exist? If so just update the value and move it to end of the list.
                // This operation can not change the list of the list so we don't care about capacity.
                if (items.ContainsKey(key) == true)
                {
                    items[key] = Value; // Update the value 
                    cache.Remove(key); // Remove it from the someplace in the list
                    cache.AddLast(key); // Add it to the END of the list
                    return;
                }

                // Add new Node
                items[key] = Value;
                cache.AddLast(key); // Add it to the END of the list

                // Check to see if the cache has reached it's capacity
                if (cache.Count > Capacity)
                {
                    // Remove the first item from Cache and it's sibling Value in items becasue it's the oldest
                    items.Remove(cache.First.Value);
                    cache.RemoveFirst();
                }
            }
        }

        public List<KeyValuePair<K, V>> ToList()
        {
            var list = new List<KeyValuePair<K, V>>();
            lock (cache_lock)
            {
                // Loop from least used to Most Used
                foreach (var i in cache)
                {
                    var t = new KeyValuePair<K, V>(i, items[i]);
                    list.Add(t);
                }
            }
            return list;
        }
    }
}
