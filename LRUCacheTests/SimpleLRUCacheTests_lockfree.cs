using LRUCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LRUCacheTests
{
   
    [TestClass]
    public class SimpleLRUCacheTests_lockfree : SimpleLRUCacheTests
    {
        public ILRUCache<SimpleLRUCacheItem,int> MakeRainbowCache_lockfree(int Capacity)
        {
            var c = new LRUCache_lockfree<SimpleLRUCacheItem,int, string>(Capacity);
            SimpleLRUCacheTests_lockfree.AddRainbowItems(c);
            return c;
        }

        [TestMethod]
        public void CreateLRUCache_lockfree()
        {
            ILRUCache<SimpleLRUCacheItem, int> c = new LRUCache_lockfree<SimpleLRUCacheItem, int, string>();
            Console.WriteLine("Created Empty Cache.");
            Assert.AreEqual(0, c.Count, 0, "Cache size is not zero");
            c.Put(new SimpleLRUCacheItem(1, "Red"));
            Assert.AreEqual(1, c.Count, 0, "Cache size is not one");
            c.Put(new SimpleLRUCacheItem(2, "Blue"));
            Assert.AreEqual(2, c.Count, 0, "Cache size is not two");
            SimpleLRUCacheTests_lockfree.DumpCache(c, "Most Used/ Recently added to Least used/Oldest ");
            Console.WriteLine("lockfree Test Complete.");
        }
        [TestMethod]
        public void TestFind()
        {
            var c = MakeRainbowCache_lockfree(10);
            this.TestFind(c);
        }

        [TestMethod]
        public void TestMaxSize_Cleanup()
        {
            var c = MakeRainbowCache_lockfree(4);
            SimpleLRUCacheTests_lockfree.DumpCache(c, "After Creation");
            Assert.AreEqual(4, c.Count, 0, "Cache size is not 4");
            Console.WriteLine("Test Complete.");
        }
        [TestMethod]
        public void Put_and_Replace()
        {
            var c = MakeRainbowCache_lockfree(10);
            this.Put_and_Replace(c);
        }

        [TestMethod]
        public void TestMaxSize_Find()
        {
            var c = MakeRainbowCache_lockfree(4);
            SimpleLRUCacheTests_lockfree.DumpCache(c, "After Creation");
            try
            {
                var val = c.Get(0);
                //Assert.AreEqual(4, c.Count, 0, "Cache size is not 4");
            }
            catch
            {
                // Ignore
            }
            finally
            {
                SimpleLRUCacheTests_lockfree.DumpCache(c, "\nAfter Find");
                Console.WriteLine("Test Complete.");
            }
        }
        [TestMethod]
        public void RemoveTest()
        {
            var c = new LRUCache_lockfree<SimpleLRUCacheItem, int, string>(10);
            RemoveTest(c);
        }
        [TestMethod]
        public void ManyPuts10k()
        {
            var c = new LRUCache_lockfree<SimpleLRUCacheItem, int, string>(1000);
            ManyPuts(c, 10000);
        }
        [TestMethod]
        public void ManyGets10k()
        {
            var c = new LRUCache_lockfree<SimpleLRUCacheItem, int, string>(1000);
            ManyGets(c, 10000);
        }

        [TestMethod]
        public void ParallelOperation1()
        {
            var c = new LRUCache_lockfree<SimpleLRUCacheItem, int, string>(1000);
            ParallelOperations(c);
        }
    }
}
