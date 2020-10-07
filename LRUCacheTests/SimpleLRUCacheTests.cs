using LRUCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace LRUCacheTests
{
    public class LRUCacheTestHelpers
    {

    }
    [TestClass]
    public class SimpleLRUCacheTests
    {
        static public void AddRainbowItems(ILRUCache<int, string> c)
        {
            c.Put(0, "Red");
            c.Put(1, "Orange");
            c.Put(2, "Yellow");
            c.Put(3, "Green");
            c.Put(4, "Blue");
            c.Put(5, "Indigo");
            c.Put(6, "Violet");
        }

        static public void DumpCache(ILRUCache<int, string> Cache, string Title = null)
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
        
        public ILRUCache<int, string> MakeRainbowCache_lock(int Capacity)
        {
            var c = new LRUCache_lock<int, string>(Capacity);
            SimpleLRUCacheTests.AddRainbowItems(c);
            return c;
        }

        [TestMethod]
        public void CreateLRUCache_lock()
        {
            var c = new LRUCache_lock<int, string>();
            Console.WriteLine("Created Empty Cache.");
            Assert.AreEqual(0, c.Count, 0, "Cache size is not zero");
            c.Put(1, "Red");
            Assert.AreEqual(1, c.Count, 0, "Cache size is not one");
            c.Put(2, "Blue");
            Assert.AreEqual(2, c.Count, 0, "Cache size is not two");
            Console.WriteLine("lock Test Complete.");
        }

        [TestMethod]
        public void TestFind()
        {
            var c = MakeRainbowCache_lock(10);
            Assert.AreEqual(7, c.Count, 0, "Cache size is not 7");
            Assert.AreEqual("Red", c.Get(0), "Cache did not contain key 0");
            Assert.AreEqual("Violet", c.Get(6), "Cache did not contain key 6");
            SimpleLRUCacheTests.DumpCache(c, "Least Used to Most Used");
            Console.WriteLine("Test Complete.");
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
    }
}
