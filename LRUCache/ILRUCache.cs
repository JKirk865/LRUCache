using System.Collections.Generic;

namespace LRUCache
{
    /// <typeparam name="N">A "node" class inheriting from LRUCacheItem<TKey, TValue></typeparam>
    /// <typeparam name="TKey">The type of the key field used in the LRUCacheItem</typeparam>
    public interface ILRUCache<N, TKey>
    {
        int Capacity { get; }
        int Count { get; }
        N Get(TKey key);
        void Put(N item);
        public List<N> ToList(); // I chose not to use IEnumerable for threading reasons
    }
}