using System;

namespace LRUCache
{
    public interface ILRUCacheItem<K, V>
                    where K : IComparable<K>
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
}
