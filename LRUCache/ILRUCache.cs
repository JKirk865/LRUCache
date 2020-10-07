using System;
using System.Collections.Generic;

namespace LRUCache
{
    /// <summary>
    /// The purpose of this interface class was to assist with testing different implementations of an LRUCache.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface ILRUCache<TKey, TValue>
    {
        int Capacity { get; }
        int Count { get; }
        TValue Get(TKey key);
        void Put(TKey key, TValue value);
        public List<KeyValuePair<TKey, TValue>> ToList(); // I chose not to use IEnumerable for threading reasons
    }
}