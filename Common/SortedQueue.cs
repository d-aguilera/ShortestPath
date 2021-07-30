using System;
using System.Collections.Generic;
using System.Linq;

namespace ShortestPath
{
    public class SortedQueue<T, TSort, TKey>
    {
        private readonly IDictionary<TSort, IDictionary<TKey, T>> buckets;
        private readonly IDictionary<TKey, T> allItems;

        public SortedQueue(Func<T, TSort> sortPropertyGetter, Action<T, TSort> sortPropertySetter, Func<T, TKey> keyPropertyGetter)
        {
            GetSort = sortPropertyGetter ?? throw new ArgumentNullException(nameof(sortPropertyGetter));
            SetSort = sortPropertySetter ?? throw new ArgumentNullException(nameof(sortPropertySetter));
            GetKey = keyPropertyGetter ?? throw new ArgumentNullException(nameof(keyPropertyGetter));

            buckets = new SortedDictionary<TSort, IDictionary<TKey, T>>();
            allItems = new SortedDictionary<TKey, T>();
        }

        public int Count => allItems.Count;

        public Func<T, TSort> GetSort { get; }
        public Action<T, TSort> SetSort { get; }
        public Func<T, TKey> GetKey { get; }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return allItems.ContainsKey(key);
        }

        public void Enqueue(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var sortKey = GetSort(item);

            if (!buckets.ContainsKey(sortKey))
            {
                buckets.Add(sortKey, new SortedDictionary<TKey, T>());
            }

            var itemKey = GetKey(item);

            buckets[sortKey].Add(itemKey, item);
            allItems.Add(itemKey, item);
        }

        public T Dequeue()
        {
            var sortKey = buckets.Keys.First();
            var bucket = buckets[sortKey];
            var itemKey = bucket.Keys.First();
            var item = bucket[itemKey];

            Remove(sortKey, itemKey);

            return item;
        }

        public void UpdateSortKey(T item, TSort newSortKey)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var sortKey = GetSort(item);

            if (Equals(newSortKey, sortKey))
            {
                return;
            }

            var itemKey = GetKey(item);

            Remove(sortKey, itemKey);
            SetSort(item, newSortKey);
            Enqueue(item);
        }

        private void Remove(TSort sortKey, TKey itemKey)
        {
            if (!buckets.ContainsKey(sortKey) || !buckets[sortKey].ContainsKey(itemKey))
            {
                throw new ArgumentOutOfRangeException(nameof(sortKey), sortKey, "Item not found in queue.");
            }

            var bucket = buckets[sortKey];

            bucket.Remove(itemKey);

            if (bucket.Count == 0)
            {
                buckets.Remove(sortKey);
            }

            allItems.Remove(itemKey);
        }
    }
}
