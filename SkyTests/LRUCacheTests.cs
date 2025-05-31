using System;
using Xunit;
using WPFComponents.Model;
using Accord.Math;

namespace SkyTests
{
    public class LRUCacheTests
    {
        [Fact]
        public void Add_And_Get_Value()
        {
            var cache = new LRUCache<string, int>(3);
            cache.AddOrUpdate("one", 1);
            cache.AddOrUpdate("two", 2);

            Assert.True(cache.TryGetValue("one", out var value1));
            Assert.Equal(1, value1);

            Assert.True(cache.TryGetValue("two", out var value2));
            Assert.Equal(2, value2);
        }

        [Fact]
        public void Update_Existing_Value()
        {
            var cache = new LRUCache<string, string>(2);
            cache.AddOrUpdate("key", "initial");
            cache.AddOrUpdate("key", "updated");

            Assert.True(cache.TryGetValue("key", out var value));
            Assert.Equal("updated", value);
        }

        [Fact]
        public void Remove_Oldest_When_Exceeding_Capacity()
        {
            var cache = new LRUCache<int, string>(2);
            cache.AddOrUpdate(1, "A");
            cache.AddOrUpdate(2, "B");
            cache.AddOrUpdate(3, "C"); // вытеснит 1

            Assert.False(cache.TryGetValue(1, out _));
            Assert.True(cache.TryGetValue(2, out var val2));
            Assert.Equal("B", val2);

            Assert.True(cache.TryGetValue(3, out var val3));
            Assert.Equal("C", val3);
        }

        [Fact]
        public void Most_Recently_Used_Is_Not_Evicted()
        {
            var cache = new LRUCache<string, string>(2);
            cache.AddOrUpdate("A", "1");
            cache.AddOrUpdate("B", "2");

            Assert.True(cache.TryGetValue("A", out _));

            cache.AddOrUpdate("C", "3");

            Assert.True(cache.TryGetValue("A", out _));
            Assert.False(cache.TryGetValue("B", out _));
            Assert.True(cache.TryGetValue("C", out _));
        }

        [Fact]
        public void Clear_Empties_The_Cache()
        {
            var cache = new LRUCache<int, string>(3);
            cache.AddOrUpdate(1, "A");
            cache.AddOrUpdate(2, "B");

            cache.Clear();

            Assert.False(cache.TryGetValue(1, out _));
            Assert.False(cache.TryGetValue(2, out _));
        }
    }
}