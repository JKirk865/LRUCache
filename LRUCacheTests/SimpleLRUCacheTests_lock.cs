using LRUCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LRUCacheTests
{
    [TestClass]
    public class SimpleLRUCacheTests_lock : SimpleLRUCacheTests
    {
        public ILRUCache<SimpleLRUCacheItem, int> MakeRainbowCache_lock(int Capacity)
        {
            var c = new LRUCache_lock<SimpleLRUCacheItem, int, string>(Capacity);
            SimpleLRUCacheTests.AddRainbowItems(c);
            return c;
        }

        [TestMethod]
        public void CreateLRUCache_lock()
        {
            var c = new LRUCache_lock<SimpleLRUCacheItem, int, string>();
            Console.WriteLine("Created Empty Cache.");
            Assert.AreEqual(0, c.Count, 0, "Cache size is not zero");
            c.Put(new SimpleLRUCacheItem(1, "Red"));
            Assert.AreEqual(1, c.Count, 0, "Cache size is not one");
            c.Put(new SimpleLRUCacheItem(2, "Blue"));
            Assert.AreEqual(2, c.Count, 0, "Cache size is not two");
            Console.WriteLine("lock Test Complete.");
        }

        [TestMethod]
        public void TestFind()
        {
            var c = MakeRainbowCache_lock(10);
            this.TestFind(c);
        }

        [TestMethod]
        public void Put_and_Replace()
        {
            var c = MakeRainbowCache_lock(10);
            this.Put_and_Replace(c);
        }

        [TestMethod]
        public void TestMaxSize_Cleanup()
        {
            var c = MakeRainbowCache_lock(4);
            SimpleLRUCacheTests.DumpCache(c, "After Creation");
            Assert.AreEqual(4, c.Count, 0, "Cache size is not 4");
            Console.WriteLine("Test Complete.");
        }
        [TestMethod]
        public void TestMaxSize_Find()
        {
            var c = MakeRainbowCache_lock(4);
            SimpleLRUCacheTests.DumpCache(c, "After Creation");
            try
            {
                var val = c.Get(0);
                //Assert.AreEqual(4, c.Count, 0, "Cache size is not 4");
            } catch
            {
                // Ignore
            }
            finally {
                SimpleLRUCacheTests.DumpCache(c, "\nAfter Find");
                Console.WriteLine("Test Complete.");
            }
        }
        [TestMethod]
        public void ManyPuts10k()
        {
            var c = new LRUCache_lock<SimpleLRUCacheItem, int, string>(1000);
            ManyPuts(c, 10000);
        }
        [TestMethod]
        public void ManyGets10k()
        {
            var c = new LRUCache_lock<SimpleLRUCacheItem, int, string>(1000);
            ManyGets(c, 10000);
        }
        [TestMethod]
        public void ParallelOperation1()
        {
            var c = new LRUCache_lock<SimpleLRUCacheItem, int, string>(1000);
            ParallelOperations(c);
        }
    }
}
