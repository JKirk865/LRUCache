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
            Console.WriteLine("Test Complete.");
        }

        [TestMethod]
        public void TestMaxSize()
        {
            var config = new LRUCacheConfig();
            config.MaximumSize = 4;
            var c = MakeRainbowCache(config);
            SimpleLRUNode.DumpCache(c, "After Creation");
            c.Cleanup();
            SimpleLRUNode.DumpCache(c, "\nAfter Cleanup");
            Assert.AreEqual(4, c.Count(), 0, "Cahe size is not 4");
            Console.WriteLine("Test Complete.");
        }

        [TestMethod]
        public void ExpirationTest1()
        {
            var config = new LRUCacheConfig();
            config.Expiration = new TimeSpan(hours: 0, minutes: 0, seconds: 5);
            var c = MakeRainbowCache(config);
            SimpleLRUNode.DumpCache(c, "After Creation");
            System.Threading.Thread.Sleep(10 * 1000); // 10 second
            c.Cleanup();
            SimpleLRUNode.DumpCache(c, "\nAfter 10 sec Delay");
            Assert.AreEqual(0, c.Count(), 0, "Cahe size is not 0");
            Console.WriteLine("Test Complete.");
        }
        [TestMethod]
        public void ExpirationTest2()
        {
            var config = new LRUCacheConfig();
            config.Expiration = new TimeSpan(hours: 0, minutes: 0, seconds: 5);

            var c = new LRUCache<int, string>(config);
            c.AddItem(new SimpleLRUNode(0, "Red"));    System.Threading.Thread.Sleep(1 * 1000); // 1 second
            c.AddItem(new SimpleLRUNode(1, "Orange")); System.Threading.Thread.Sleep(1 * 1000); // 1 second
            c.AddItem(new SimpleLRUNode(2, "Yellow")); System.Threading.Thread.Sleep(1 * 1000); // 1 second
            c.AddItem(new SimpleLRUNode(3, "Green"));  System.Threading.Thread.Sleep(1 * 1000); // 1 second
            c.AddItem(new SimpleLRUNode(4, "Blue"));   System.Threading.Thread.Sleep(1 * 1000); // 1 second
            c.AddItem(new SimpleLRUNode(5, "Indigo")); System.Threading.Thread.Sleep(1 * 1000); // 1 second
            c.AddItem(new SimpleLRUNode(6, "Violet")); System.Threading.Thread.Sleep(1 * 1000); // 1 second
            System.Threading.Thread.Sleep(2 * 1000); // 10 second
            SimpleLRUNode.DumpCache(c, "\nAfter 2 sec Delay");
            Assert.AreEqual(2, c.Count(), 0, "Cahe size is not 5");
            Console.WriteLine("Test Complete.");
        }
    }
}
