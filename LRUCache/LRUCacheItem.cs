using System;

namespace LRUCache
{
    public interface ILRUCacheItem<K, V>
                    where K : IComparable<K>
    {
        K Key { get; }
        V Value { get; set; }
        DateTime? Expiration { get; set; }
        TimeSpan? Lifetime { get; }
        bool IsExpired { get; }
        void UpdateExpiration();
    }
    
    public class LRUCacheItem<K, V> : ILRUCacheItem<K, V>
                                where K : IComparable<K>
    {
        public K Key { get; private set; }
        public V Value { get; set; }
        public DateTime? Expiration { get; set; } = null;
        public TimeSpan? Lifetime { get; protected set; } = null;

        public LRUCacheItem(K key, V value, TimeSpan? lifetime = null)
        {
            Key = key;
            Value = value;
            Lifetime = lifetime;
            UpdateExpiration();
        }
        public void UpdateExpiration()
        {
            if (Lifetime.HasValue)
                Expiration = DateTime.UtcNow + Lifetime;
        }
        public bool IsExpired
        {
            get
            {
                if ((Lifetime.HasValue) && (DateTime.UtcNow > Expiration))
                    return true;
                return false;
            }
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            LRUCacheItem<K, V> objAsItem = obj as LRUCacheItem<K, V>;
            return Key.Equals(objAsItem.Key) && Value.Equals(objAsItem.Value);
        }

        /// <summary>
        /// This is not an ideal Hash code function, consider replacing it with something better,
        /// this was here mostly to fix the warning.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ Value.GetHashCode();
        }
    }
}
