using LRUCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using LockFreeDoublyLinkedLists;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace LRUCacheTests
{
    public class LockFreeTestHelpers
    {
    }

    public class ListItemData
    {
        public long NodeId { get; private set; }
        public int Value { get; private set; }
        public override string ToString()
        {
            return "<" + NodeId + ", " + Value.ToString() + ">";
        }
        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            ListItemData objAsLid = obj as ListItemData;

            return NodeId == objAsLid.NodeId
                && Value == objAsLid.Value;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return ((0x51ed270b + Value) * -1521134295
                + NodeId.GetHashCode()) * -1521134295;
        }

        public ListItemData(long nodeId, int value)
        {
            NodeId = nodeId;
            Value = value;
        }
    }

    /// <summary>
    /// Just a class to explore the lockfree functionality, the examples in the documentation are bafflinbg
    /// </summary>
    [TestClass]
    public class LockFreePlayground
    {
        [TestMethod]
        public void Test1()
        {
            ILockFreeDoublyLinkedList< ListItemData> list = LockFreeDoublyLinkedLists.LockFreeDoublyLinkedList.Create<ListItemData>();
            
            // Can I add?
            list.PushRight(new ListItemData(1,1));
            list.PushLeft(new ListItemData(2,2));
            Assert.AreEqual(2, list.Count(), 0, "Cache size is not two");
            
            // Can I remove from one side? (only one side)
            list.PopRightNode();
            Assert.AreEqual(1, list.Count(), 0, "Cache size is not one");

            // Can I find one?
            list.PushLeft(new ListItemData(3, 3));
            ListItemData result = list.First(x => (x.NodeId == 3));
            if (result != null)
            {
                Console.WriteLine("Looking for 3: {0}", result.ToString());
            }
            Assert.AreEqual(3, result.Value, 0, "Did not find 3");         

            Console.WriteLine("Test Complete.");
        }
    }
}