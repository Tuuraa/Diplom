using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFComponents.Model
{
    public class LRUCache<TKey, TValue> where TKey : notnull
    {
        private class CacheItem
        {
            public TKey Key { get; }
            public TValue Value { get; }

            public CacheItem(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }

        private readonly int _capacity;
        private readonly ConcurrentDictionary<TKey, LinkedListNode<CacheItem>> _cache;
        private readonly LinkedList<CacheItem> _list;
        private readonly object _syncRoot = new();

        public LRUCache(int capacity)
        {
            _capacity = capacity > 0 ? capacity : throw new ArgumentException("Capacity must be positive");
            _cache = new ConcurrentDictionary<TKey, LinkedListNode<CacheItem>>();
            _list = new LinkedList<CacheItem>();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_cache.TryGetValue(key, out var node))
            {
                lock (_syncRoot)
                {
                    if (node.List != null) // Проверка, что узел всё ещё в списке
                    {
                        _list.Remove(node);
                        _list.AddFirst(node);
                    }
                }
                value = node.Value.Value;
                return true;
            }
            value = default;
            return false;
        }

        public void AddOrUpdate(TKey key, TValue value)
        {
            lock (_syncRoot)
            {
                if (_cache.TryGetValue(key, out var existingNode))
                {
                    _list.Remove(existingNode);
                }
                else if (_cache.Count >= _capacity)
                {
                    RemoveLast();
                }

                var newNode = new LinkedListNode<CacheItem>(new CacheItem(key, value));
                _list.AddFirst(newNode);
                _cache[key] = newNode;
            }
        }

        private void RemoveLast()
        {
            var lastNode = _list.Last;
            if (lastNode != null)
            {
                _cache.TryRemove(lastNode.Value.Key, out _);
                _list.RemoveLast();
            }
        }

        public void Clear()
        {
            lock (_syncRoot)
            {
                _cache.Clear();
                _list.Clear();
            }
        }
    }
}
