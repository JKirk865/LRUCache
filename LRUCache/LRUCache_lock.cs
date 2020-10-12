using System;
using System.Collections;
using System.Collections.Generic;

namespace LRUCache
{
    /// <typeparam name="N">The Object that will be cached that contained the Key and Value</typeparam>
    /// <typeparam name="K">Key</typeparam>
    /// <typeparam name="V">Value</typeparam>
    public class LRUCache_lock<N, K, V> : ILRUCache<N, K>
                                          where N : LRUCacheItem<K, V>
                                          where K : IComparable<K>
    {
        public int Capacity { get; private set; } // Capacity can not be changed once it is specified in the constructor
        private LinkedList<K> cache = new LinkedList<K>();  // Holds the Keys in order from (FRONT) least used to (Last) recently used/added.
        private Dictionary<K, N> items = new Dictionary<K, N>(); // Holds the Key/Value for O(1) lookup.
        private object cache_lock = new object(); // Used to ensure thread-safe operations

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
                    RemoveExpired();
                    return items.Count;
                }
            }
        }
        public N Get(K key)
        {
            if (key == null)
                throw new ArgumentNullException();

            lock (cache_lock)
            {
                N value;
                if (items.TryGetValue(key, out value) == true)
                {
                    if (value.IsExpired)
                    {
                        items.Remove(key);
                        throw new KeyNotFoundException(string.Format("Key expired: {0}", key.ToString()));
                    }
                    cache.Remove(key); // Remove it from the someplace in the list
                    cache.AddLast(key); // Add it to the END of the list
                    value.UpdateExpiration();
                    return value;
                }
            }

            throw new KeyNotFoundException(string.Format("Key Not Found: {0}", key.ToString()));
        }

        public void Put(N item)
        {
            if ((item == null) || (item.Key == null) || (item.Value == null))
                throw new ArgumentNullException();

            lock (cache_lock)
            {
                item.UpdateExpiration();

                // Does the Key already exist? If so just update the value and move it to end of the list.
                // Cache size is not changed.
                if (items.ContainsKey(item.Key) == true)
                {
                    items[item.Key].Value = item.Value; // Update the value 
                    cache.Remove(item.Key); // Remove it from someplace in the list
                    cache.AddLast(item.Key); // Add it to the END of the list
                    return;
                }

                // Add new Node
                items[item.Key] = item;
                cache.AddLast(item.Key); // Add it to the END of the list

                // Check to see if the cache has reached it's capacity
                if (cache.Count > Capacity)
                {
                    // Remove the first item from Cache and it's sibling Value in items because it's the oldest
                    items.Remove(cache.First.Value);
                    cache.RemoveFirst();
                }
            }
        }

        public bool Remove(K key)
        {
            if (key == null)
                throw new ArgumentNullException();

            lock (cache_lock)
            {
                if (items.Remove(key) == true)
                {
                    cache.Remove(key); // Remove it from the someplace in the list
                    return true;
                }
            }

            return false;
        }

        public List<N> ToList()
        {
            var list = new List<N>();
            lock (cache_lock)
            {
                // Loop from least used to Most Used
                foreach (K i in cache)
                {
                    N value;
                    if (items.TryGetValue(i, out value) == true)
                    {
                        list.Add(value);
                    }
                }
            }
            return list;
        }
        
        #region Extra Features that can't be added to lockfree version
        /// <summary>
        /// I can not yet think of a lockfree way to add these two additional calls.
        /// </summary>
        public void Clear()
        {
            lock (cache_lock)
            {
                cache.Clear();
                items.Clear();
            }
        }

        public int RemoveExpired()
        {
            var RemoveList = new List<N>();
            int ExpiredItems = 0;

            lock (cache_lock)
            {
                foreach (var item in items)
                {
                    if (item.Value.IsExpired)
                    {
                        cache.Remove(item.Key);
                        RemoveList.Add(item.Value);
                    }
                }
                ExpiredItems = RemoveList.Count;
                foreach (var N in RemoveList)
                    items.Remove(N.Key);
                return ExpiredItems;
            }
        }
        #endregion
    }
}
