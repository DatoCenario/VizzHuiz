using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace VizzHuiz
{
    class PriorityQueue<TKey>
    {
        class Item
        {
            public TKey Key;
            public double Priority;
        }

        Dictionary<TKey, int> KeyIndex;
        List<Item> ItemsWithPriority;
        bool Descending;
        public int Count => ItemsWithPriority.Count;
        public PriorityQueue(bool descending = false)
        {
            Descending = descending;
            ItemsWithPriority = new List<Item>();
            KeyIndex = new Dictionary<TKey, int>();
        }

        public void InsertItem(TKey key, double priority)
        {
            if (KeyIndex.ContainsKey(key))
            {
                throw new ArgumentException($"key {key} already contains in queue");
            }
            ItemsWithPriority.Add(new Item() { Key = key, Priority = priority });
            KeyIndex[key] = Count - 1;
            HeapifyUp(Count - 1);
        }

        public bool TryInsertItem(TKey key, double priority)
        {
            if (KeyIndex.ContainsKey(key))
            {
                return false;
            }
            ItemsWithPriority.Add(new Item() { Key = key, Priority = priority });
            KeyIndex[key] = Count - 1;
            HeapifyUp(Count - 1);
            return true;
        }

        public Tuple<TKey, double> ExtractPriority()
        {
            var item = Tuple.Create(ItemsWithPriority[0].Key, ItemsWithPriority[0].Priority);
            ItemsWithPriority[0] = ItemsWithPriority.Last();
            KeyIndex[ItemsWithPriority.Last().Key] = 0;
            HeapifyDown(0);
            KeyIndex.Remove(ItemsWithPriority[Count - 1].Key);
            ItemsWithPriority.RemoveAt(Count - 1);
            return item;
        }

        public void ChangePriority(TKey key, double priority)
        {
            int index = KeyIndex[key];
            ItemsWithPriority[index].Priority = priority;
            HeapifyDown(index);
            HeapifyUp(index);
        }

        public bool TryGetValue(TKey key, out double priority)
        {
            int index;
            bool contains = KeyIndex.TryGetValue(key, out index);
            if (!contains)
            {
                priority = 0;
                return false;
            }
            else
            {
                priority = ItemsWithPriority[index].Priority;
                return true;
            }
        }

        private void HeapifyUp(int index)
        {
            int parent = (index + 1) / 2 - 1;
            while (parent != -1)
            {
                int comparison = ItemsWithPriority[index].Priority
                    .CompareTo(ItemsWithPriority[parent].Priority);
                if ((comparison < 0 && !Descending) || (comparison > 0 && Descending))
                {
                    Swap(index, parent);
                    index = parent;
                    parent = (index + 1) / 2 - 1;
                }
                else
                {
                    parent = -1;
                }
            }
        }

        private void HeapifyDown(int index)
        {
            int child = GetChildIndex(index);
            while (child != -1)
            {
                Swap(index, child);
                index = child;
                child = GetChildIndex(index);
            }
        }

        private int GetChildIndex(int index)
        {
            int left = (index + 1) * 2 - 1;
            int right = (index + 1) * 2;

            int child = index;
            if (left < Count)
            {
                int comparison = ItemsWithPriority[left].Priority
                    .CompareTo(ItemsWithPriority[child].Priority);
                if (comparison < 0 && !Descending || comparison > 0 && Descending)
                {
                    child = left;
                }
            }
            if (right < Count)
            {
                int comparison = ItemsWithPriority[right].Priority
                    .CompareTo(ItemsWithPriority[child].Priority);
                if (comparison < 0 && !Descending || comparison > 0 && Descending)
                {
                    child = right;
                }
            }
            return child == index ? -1 : child;
        }

        private void Swap(int i1, int i2)
        {
            var t = ItemsWithPriority[i1];
            ItemsWithPriority[i1] = ItemsWithPriority[i2];
            ItemsWithPriority[i2] = t;
            KeyIndex[ItemsWithPriority[i2].Key] = i2;
            KeyIndex[ItemsWithPriority[i1].Key] = i1;
        }
    }
}
