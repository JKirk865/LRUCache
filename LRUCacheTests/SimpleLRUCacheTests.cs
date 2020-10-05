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

        public override int Key => _key;

        public override string Value => _value;

        static public void DumpCache(LRUCache<int, string> Cache)
        {
            var list = Cache?.ToList(OnlyValid: false);
            foreach(var i in list)
            {
                Console.WriteLine("Key:{0}  Value:{1}  IsExpired:{2}  IsValid:{3}",
                    i.Key, i.Value, i.IsExpired(), i.IsValid);
            }
        }
    }

    [TestClass]
    public class SimpleLRUCacheTests
    {
        public LRUCache<int, string> MakeRainbowCache(LRUCacheConfig Config)
        {
            var c = new LRUCache<int, string>(Config);
            c.AddItem(new SimpleLRUNode(0, "Red"));
            c.AddItem(new SimpleLRUNode(1, "Orange"));
            c.AddItem(new SimpleLRUNode(2, "Yellow"));
            c.AddItem(new SimpleLRUNode(3, "Green"));
            c.AddItem(new SimpleLRUNode(4, "Blue"));
            c.AddItem(new SimpleLRUNode(5, "Indigo"));
            c.AddItem(new SimpleLRUNode(6, "Violet"));
            return c;
        }

        [TestMethod]
        public void CreateLRUCache1()
        {
            var config = new LRUCacheConfig();
            var c = new LRUCache<int, string>(config);
            Console.WriteLine("Created Empty Cache.");
            Assert.AreEqual(0, c.Count(), 0, "Cahe size is not zero");
            c.AddItem(new SimpleLRUNode(1, "Red"));
            Assert.AreEqual(1, c.Count(), 0, "Cahe size is not one");
            c.AddItem(new SimpleLRUNode(2, "Blue"));
            Assert.AreEqual(2, c.Count(), 0, "Cahe size is not two");
        }

        [TestMethod]
        public void TestFind()
        {
            var config = new LRUCacheConfig();
            var c = MakeRainbowCache(config);
            Assert.AreEqual(7, c.Count(), 0, "Cahe size is not 7");
            Assert.AreEqual("Red", c.FindItem(0), "Cache did not contain key 0");
            Assert.AreEqual("Violet", c.FindItem(6), "Cache did not contain key 6");
            SimpleLRUNode.DumpCache(c);
        }
    }
}
