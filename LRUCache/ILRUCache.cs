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

    //public interface ILRUCache2<N, TKey, TValue> where N : ILRUCacheItem<TKey, TValue>
    public interface ILRUCache2<N, TKey>
    {
        int Capacity { get; }
        int Count { get; }
        N Get(TKey key);
        void Put(N item);
        public List<N> ToList(); // I chose not to use IEnumerable for threading reasons
    }

}