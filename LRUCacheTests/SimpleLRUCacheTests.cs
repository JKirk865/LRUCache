using LRUCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace LRUCacheTests
{
    public class SimpleLRUCacheItem : LRUCacheItem<int, string>
    {
        public SimpleLRUCacheItem(int key, string value)
            : base(key, value)
        {
            // Nothing to do here
        }
    }

 
    public abstract class SimpleLRUCacheTests
    {
        Random random = new Random(Guid.NewGuid().GetHashCode());

        public SimpleLRUCacheTests()
        {
            // TBD
        }
        static public void AddRainbowItems(ILRUCache<SimpleLRUCacheItem, int> c)
        {
            c.Put(new SimpleLRUCacheItem(0, "Red"));
            c.Put(new SimpleLRUCacheItem(1, "Orange"));
            c.Put(new SimpleLRUCacheItem(2, "Yellow"));
            c.Put(new SimpleLRUCacheItem(3, "Green"));
            c.Put(new SimpleLRUCacheItem(4, "Blue"));
            c.Put(new SimpleLRUCacheItem(5, "Indigo"));
            c.Put(new SimpleLRUCacheItem(6, "Violet"));
        }

        static public void DumpCache(ILRUCache<SimpleLRUCacheItem, int> Cache, string Title = null)
        {
            if (Title != null)
            {
                Console.WriteLine(Title);
                Console.WriteLine("++++++++++++++++++++++");
            }

            var list = Cache?.ToList();
            foreach (var i in list)
            {
                Console.WriteLine("Key:{0}  Value:{1}", i.Key, i.Value);
            }
        }

        public void TestFind(ILRUCache<SimpleLRUCacheItem, int> c)
        {            
            Assert.AreEqual(7, c.Count, 0, "Cache size is not 7");
            Assert.AreEqual("Red", c.Get(0).Value, "Cache did not contain key 0");
            Assert.AreEqual("Violet", c.Get(6).Value, "Cache did not contain key 6");
            SimpleLRUCacheTests_lockfree.DumpCache(c, "Most Used/ Recently added to Least used/Oldest ");
            Console.WriteLine("Test Complete.");
        }

        public void Put_and_Replace(ILRUCache<SimpleLRUCacheItem, int> c)
        {
            c.Put(new SimpleLRUCacheItem(10, "Dog"));
            var numItems = c.Count;
            Assert.AreEqual("Dog", c.Get(10).Value, "Cache did not contain key 10");
            c.Put(new SimpleLRUCacheItem(10, "Cat")); // Dog should be replaced with "Cat"
            Assert.AreEqual("Cat", c.Get(10).Value, "Cache did not contain key 10");
            Assert.AreEqual(numItems, c.Count, 0, "Cache size changed");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c">Expected to be empty, capacity xconstraint may impact performance</param>
        /// <param name="NumPuts"></param>
        public void ManyPuts(ILRUCache<SimpleLRUCacheItem, int> c, int NumPuts=1000)
        {
            for(int i=0; i < NumPuts; i++)
            {
                c.Put(new SimpleLRUCacheItem(i, i.ToString()));
            }
        }

        public void ManyGets(ILRUCache<SimpleLRUCacheItem, int> c, int NumGets = 1000, int MaxKey = 1000)
        {
            if (c.Capacity < MaxKey)
                throw new ArgumentOutOfRangeException();

            ManyPuts(c, MaxKey);
            for (int i = 0; i < NumGets; i++)
            {
                var k = this.random.Next(0, MaxKey);
                var n = c.Get(k);
                Assert.AreEqual(k.ToString(),n.Value, "Cache did not contain key.");
            }
        }

        public void ParallelOperations(ILRUCache<SimpleLRUCacheItem, int> c, int NumThreads = 10)
        {
            int MaxKey = NumThreads * 100;
            int NumGets = 1000;

            if (c.Capacity < MaxKey)
                throw new ArgumentOutOfRangeException();

            Parallel.For(0, NumThreads, t =>
            {
                var maxIndex = (t * 100) + 100;
                for (int i = (t * 100); i < maxIndex; i++)
                {
                    c.Put(new SimpleLRUCacheItem(i, i.ToString()));
                }
            });

            Assert.AreEqual(MaxKey, c.Count, 0, "Cache size incorrect");

            Parallel.For(0, NumThreads*10, t =>
            {
                for (int i = 0; i < NumGets; i++)
                {
                    var k = this.random.Next(0, MaxKey);
                    var n = c.Get(k);
                    Assert.AreEqual(k.ToString(), n.Value, "Cache did not contain key.");
                }
            });

        }
    }
}
