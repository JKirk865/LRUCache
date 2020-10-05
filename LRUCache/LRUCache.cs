using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.InteropServices;

/*
 * MIT License
 * 
 * Copyright (c) 2020 Jerry L. Kirk
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

/// <summary>
/// Q -> What namespace for local projects?
/// Feature -> Cleanup, max size, expiration, multi-thread, github
/// </summary>
namespace LRUCache
{

    public class LRUCacheConfig
    {
        public int MaximumSize { get; set;} = 10;
        public TimeSpan? Expiration { get; set; } = null;
        public TimeSpan? CleanupInterval { get; set; } = null;

    }


    public class LRUCache<K, V>
    {
        private LRUCacheConfig _config;
        private ILRUCacheNode<K,V> _head = null;

        public LRUCache(LRUCacheConfig config) { _config = config; }
        
        public V FindItem(K Item)
        {
            var nextNode = _head;
            ILRUCacheNode<K, V> prevNode = null;
            int nodes = 0;
            while (nextNode != null)
            {
                if (!nextNode.IsExpired() && nextNode.Key.Equals(Item))
                {
                    // Found Match. Move to Front of list, update expiration time, return value
                    if (prevNode != null)
                        prevNode.Next = nextNode.Next;
                    nextNode.Next = _head; // Point this item to the previous Head
                    _head = nextNode; // Make this the new Head
                    if (_config.Expiration.HasValue)
                        nextNode.Expiration = DateTime.Now + _config.Expiration.Value;
                    return nextNode.Value;
                }
                prevNode = nextNode;
                nextNode = nextNode.Next;
                // Have we traversed too many nodes? 
                if (++nodes > _config.MaximumSize) {
                    prevNode.Next = null;
                    //TBD Will C# release the rest of the nodes?
                    break;
                }
            }

            throw new KeyNotFoundException(string.Format("Key Not Found: {0}", Item.ToString()));
        }
        public int Count(bool CountExpired = false)
        {
            int count = 0;
            var nextNode = _head;
            while (nextNode != null)
            {
                if (nextNode.IsExpired())
                {
                    if (CountExpired)
                        count++;
                }
                else
                {
                    count++;
                }
                nextNode = nextNode.Next;
            }
            return count;
        }

        /// <summary>
        /// Adds a new Node to the front of the cache.
        /// Note: It allows duplicate keys in the cache but only the most recent;;y added should be found.
        /// </summary>
        /// <param name="Item"></param>
        public void AddItem(ILRUCacheNode<K, V> Item)
        {
            Item.Next = _head; // Point this item to the previous Head
            _head = Item; // Make this the new Head
            // Update the Expiration information if provided
             if (_config.Expiration.HasValue)
                Item.Expiration = DateTime.Now + _config.Expiration.Value;
            else
                Item.Expiration = null;
        }

        /// <summary>
        /// Remove Expired and Excess items
        /// </summary>
        public void Cleanup()
        {
            var nextNode = _head;
            ILRUCacheNode<K, V> prevNode = null;
            
            // First prune all the Expired and Invalid Nodes from the Cache.
            while (nextNode != null)
            {
                if (nextNode.IsExpired() || !nextNode.IsValid)
                {
                    // Remove this node!
                    nextNode.IsValid = false;
                    if (prevNode == null) {
                        //Remove the First
                        _head = nextNode  = nextNode.Next;
                        continue;
                    }
                    else
                    {
                        //Remove one in the middle
                        prevNode.Next = nextNode.Next;
                    }

                }
                prevNode = nextNode;
                nextNode = nextNode.Next;
            }

            //Second Count the valid ones and Prune at the MaximumSize
            int nodes = 1;
            nextNode = _head;
            while (nextNode != null)
            {
                prevNode = nextNode;
                nextNode = nextNode.Next;
                // Have we traversed too many nodes? 
                if (++nodes > _config.MaximumSize)
                {
                    prevNode.Next = null;
                    //TBD Will C# release the rest of the nodes?
                    break;
                }
            }

        }

        public List<ILRUCacheNode<K, V>> ToList(bool OnlyValid = true)
        {
            var list = new List<ILRUCacheNode<K, V>>();
            var nextNode = _head;
            while (nextNode != null)
            {
                if (!OnlyValid)
                    list.Add(nextNode);
                else if (nextNode.IsValid && !nextNode.IsExpired())
                    list.Add(nextNode);

                nextNode = nextNode.Next;
            }
            return list;
        }
    }
}
