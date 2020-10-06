using LRUCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LRUCacheTests
{
    public class SimpleLRUNode : LRUCacheNode<int, string>
    {
        private int _key;
        private string _value;


        public SimpleLRUNode(int key, string value)
        {
            _key = key;
            _value = value;
        }
        ~SimpleLRUNode()
        {
            Console.WriteLine("SimpleLRUNode Finalizer called for Key:{0}  Value:{1}", this._key, this._value);
        }
        public override int Key => _key;

        public override string Value => _value;

        static public void DumpCache(LRUCache<int, string> Cache, string Title = null)
        {
            if (Title != null)
            {
                Console.WriteLine(Title);
                Console.WriteLine("++++++++++++++++++++++");
            }
            
            var list = Cache?.ToList(OnlyValid: false);
            foreach(var i in list)
            {
                Console.WriteLine("Key:{0}  Value:{1}", i.Item1, i.Item2);
            }
        }
    }

    [TestClass]
    public class SimpleLRUCacheTests
    {
        public LRUCache<int, string> MakeRainbowCache(int Capacity)
        {
            var c = new LRUCache<int, string>(Capacity);
            c.Put(0, "Red");
            c.Put(1, "Orange");
            c.Put(2, "Yellow");
            c.Put(3, "Green");
            c.Put(4, "Blue");
            c.Put(5, "Indigo");
            c.Put(6, "Violet");
            return c;
        }

        [TestMethod]
        public void CreateLRUCache1()
        {
            var c = new LRUCache<int, string>();
            Console.WriteLine("Created Empty Cache.");
            Assert.AreEqual(0, c.Count(), 0, "Cache size is not zero");
            c.Put(1, "Red");
            Assert.AreEqual(1, c.Count(), 0, "Cache size is not one");
            c.Put(2, "Blue");
            Assert.AreEqual(2, c.Count(), 0, "Cache size is not two");
        }

        [TestMethod]
        public void TestFind()
        {
            var c = MakeRainbowCache(10);
            Assert.AreEqual(7, c.Count(), 0, "Cache size is not 7");
            Assert.AreEqual("Red", c.Get(0), "Cache did not contain key 0");
            Assert.AreEqual("Violet", c.Get(6), "Cache did not contain key 6");
            SimpleLRUNode.DumpCache(c, "Least Used to Most Used");
            Console.WriteLine("Test Complete.");
        }

        [TestMethod]
        public void TestMaxSize_Cleanup()
        {
            var c = MakeRainbowCache(4);
            SimpleLRUNode.DumpCache(c, "After Creation");
            Assert.AreEqual(4, c.Count(), 0, "Cache size is not 4");
            Console.WriteLine("Test Complete.");
        }
        [TestMethod]
        public void TestMaxSize_Find()
        {
            var c = MakeRainbowCache(4);
            SimpleLRUNode.DumpCache(c, "After Creation");
            try
            {
                var val = c.Get(0);
                //Assert.AreEqual(4, c.Count(), 0, "Cache size is not 4");
            } catch
            {
                // Ignore
            }
            finally {
                SimpleLRUNode.DumpCache(c, "\nAfter Find");
                Console.WriteLine("Test Complete.");
            }
        }
    }
}
