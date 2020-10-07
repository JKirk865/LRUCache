using LRUCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
    
    [TestClass]
    public class SimpleLRUCacheTests_lockfree
    {
        static public void AddRainbowItems(ILRUCache2<SimpleLRUCacheItem, int> c)
        {
            c.Put(new SimpleLRUCacheItem(0, "Red"));
            c.Put(new SimpleLRUCacheItem(1, "Orange"));
            c.Put(new SimpleLRUCacheItem(2, "Yellow"));
            c.Put(new SimpleLRUCacheItem(3, "Green"));
            c.Put(new SimpleLRUCacheItem(4, "Blue"));
            c.Put(new SimpleLRUCacheItem(5, "Indigo"));
            c.Put(new SimpleLRUCacheItem(6, "Violet"));
        }

        static public void DumpCache(ILRUCache2<SimpleLRUCacheItem,int> Cache, string Title = null)
        {
            if (Title != null)
            {
                Console.WriteLine(Title);
                Console.WriteLine("++++++++++++++++++++++");
            }

            var list = Cache?.ToList();
            foreach (SimpleLRUCacheItem i in list)
            {
                Console.WriteLine("Key:{0}  Value:{1}", i.Key, i.Value);
            }
        }
        public ILRUCache2<SimpleLRUCacheItem,int> MakeRainbowCache_lockfree(int Capacity)
        {
            var c = new LRUCache_lockfree<SimpleLRUCacheItem,int, string>(Capacity);
            SimpleLRUCacheTests_lockfree.AddRainbowItems(c);
            return c;
        }

        [TestMethod]
        public void CreateLRUCache_lockfree()
        {
            var c = new LRUCache_lockfree<SimpleLRUCacheItem, int, string>();
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
            Assert.AreEqual(7, c.Count, 0, "Cache size is not 7");
            Assert.AreEqual("Red", c.Get(0).Value, "Cache did not contain key 0");
            Assert.AreEqual("Violet", c.Get(6).Value, "Cache did not contain key 6");
            SimpleLRUCacheTests_lockfree.DumpCache(c, "Most Used/ Recently added to Least used/Oldest ");
            Console.WriteLine("Test Complete.");
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
    }
}
