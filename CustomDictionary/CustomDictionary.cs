using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomDictionary
{
    public class _Dictionary
    {
        private int[] buckets;
        private Entry[] entries;
        private int freeList;
        private int version;
        private int freeCount;
        private int count;
        private IEqualityComparer<string> comparer;
        private HashSet<string> _keys;

        private struct Entry
        {
            public int hashCode; // Lower 31 bits of hash code, -1 if unused
            public int next; // Index of next entry, -1 if last
            public string key; // Key of entry
            public string value; // Value of entry
        }


        public _Dictionary() : this(0, EqualityComparer<string>.Default)
        {
        }

        public _Dictionary(int capacity) : this(capacity, EqualityComparer<string>.Default)
        {
        }

        public _Dictionary(IEqualityComparer<string> comparer) : this(0, comparer)
        {
        }

        public _Dictionary(int capacity, IEqualityComparer<string> comparer)
        {
            if (capacity < 0) throw new ArgumentException(capacity.ToString());
            if (capacity > 0) Initialize(capacity);
            this.comparer = comparer ?? EqualityComparer<string>.Default;

            this.comparer = comparer;
        }

        public _Dictionary(IDictionary dictionary) : this(dictionary, EqualityComparer<string>.Default)
        {
        }

        public _Dictionary(IDictionary dictionary, IEqualityComparer<string> comparer) :
            this(dictionary != null ? dictionary.Count : 0, comparer)
        {
            if (dictionary == null)
            {
                throw new ArgumentException("Pass existing dictionary");
            }

            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                Add(pair.Key, pair.Value);
            }
        }

        public _Dictionary(_Dictionary dictionary, IEqualityComparer<string> comparer) :
            this(dictionary != null ? dictionary.Count : 0, comparer)
        {
            if (dictionary == null)
            {
                throw new ArgumentException("Pass existing dictionary");
            }

            foreach (KeyValuePair pair in dictionary)
            {
                Add(pair.Key, pair.Value);
            }
        }

        public IEnumerator<KeyValuePair> GetEnumerator()
        {
            foreach (var bucket in buckets)
            {
                if (bucket != -1)
                {
                    var entry = entries[bucket];
                    while (entry.hashCode != 0 && !string.IsNullOrEmpty(entry.key))
                    {
                        yield return new KeyValuePair(entry.key, entry.value);
                        if (entry.next != -1)
                        {
                            entry = entries[entry.next];
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        public List<string> Keys => new List<string>(_keys);

        public List<string> Values
        {
            get
            {
                var values = new List<string>();
                foreach (var key in Keys)
                {
                    values.Add(this[key]);
                }

                return values;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var keyValuePair in this)
            {
                builder.Append($"{keyValuePair.Key} : {keyValuePair.Value}\n");
            }

            return builder.ToString();
        }

        public int Count => count - freeCount;

        public bool ContainsKey(string key) => Keys.Contains(key);

        public bool TryGetValue(string key, out string value)
        {
            int i = FindEntry(key);
            if (i >= 0)
            {
                value = entries[i].value;
                return true;
            }

            value = default;
            return false;
        }

        private int FindEntry(string key)
        {
            if (key == null)
            {
                throw new ArgumentException();
            }

            if (buckets != null)
            {
                int hashCode = key.GetHashCode() & 0x7FFFFFFF;
                for (int i = buckets[hashCode % buckets.Length]; i >= 0; i = entries[i].next)
                {
                    if (entries[i].hashCode == hashCode && string.Equals(entries[i].key, key)) return i;
                }
            }

            return -1;
        }

        public void Add(string key, string value)
        {
            Insert(key, value, true);
            _keys.Add(key);
        }

        public string this[string key]
        {
            get
            {
                int i = FindEntry(key);
                if (i >= 0) return entries[i].value;
                throw new ArgumentException(key);
            }
            set { Insert(key, value, false); }
        }


        private void Insert(string key, string value, bool add)
        {
            if (key == null)
            {
                throw new ArgumentNullException("Key is null");
            }

            if (buckets == null) Initialize(0);
            int hashCode = key.GetHashCode() & 0x7FFFFFFF;
            int targetBucket = hashCode % buckets.Length;

            //only in case of collision
            for (int i = buckets[targetBucket]; i >= 0; i = entries[i].next)
            {
                if (entries[i].hashCode == hashCode && string.Equals(entries[i].key, key))
                {
                    if (add)
                    {
                        throw new ArgumentException(key);
                    }

                    entries[i].value = value;
                    version++;
                    return;
                }
            }

            int index;
            if (freeCount > 0)
            {
                index = freeList;
                freeList = entries[index].next;
                freeCount--;
            }
            else
            {
                if (count == entries.Length)
                {
                    Resize();
                    targetBucket = hashCode % buckets.Length;
                }

                index = count;
                count++;
            }

            entries[index].hashCode = hashCode;
            entries[index].next = buckets[targetBucket];
            entries[index].key = key;
            entries[index].value = value;
            buckets[targetBucket] = index;
            version++;
        }


        private void Resize()
        {
            Resize(_HashHelpers.ExpandPrime(count), false);
        }

        private void Resize(int newSize, bool forceNewHashCodes)
        {
            int[] newBuckets = new int[newSize];
            for (int i = 0; i < newBuckets.Length; i++) newBuckets[i] = -1;
            Entry[] newEntries = new Entry[newSize];
            Array.Copy(entries, 0, newEntries, 0, count);
            if (forceNewHashCodes)
            {
                for (int i = 0; i < count; i++)
                {
                    if (newEntries[i].hashCode != -1)
                    {
                        newEntries[i].hashCode = (newEntries[i].key.GetHashCode() & 0x7FFFFFFF);
                    }
                }
            }

            for (int i = 0; i < count; i++)
            {
                if (newEntries[i].hashCode >= 0)
                {
                    int bucket = newEntries[i].hashCode % newSize;
                    newEntries[i].next = newBuckets[bucket];
                    newBuckets[bucket] = i;
                }
            }

            buckets = newBuckets;
            entries = newEntries;
        }

        private void Initialize(int capacity)
        {
            int size = _HashHelpers.GetPrime(capacity);
            buckets = new int[size];
            for (int i = 0; i < buckets.Length; i++) buckets[i] = -1;
            entries = new Entry[size];
            freeList = -1;
            _keys = new HashSet<string>();
        }

        public void Clear()
        {
            Initialize(0);
            count = freeCount = _HashHelpers.GetPrime(0);
            version = 0;
        }

        public bool ContainsValue(string value1)
        {
            return Values.Contains(value1);
        }

        public void CopyTo(ref KeyValuePair[] keyValuePairs, int index)
        {
            if (index < 0 || index > keyValuePairs.Length - 1)
                throw new IndexOutOfRangeException(index.ToString());
            if (Count + index > keyValuePairs.Length)
            {
                var newKeyValuePairs = new KeyValuePair[Count + index];
                for (int i = 0; i < keyValuePairs.Length; i++)
                {
                    newKeyValuePairs[i] = new KeyValuePair {Key = keyValuePairs[i].Key, Value = keyValuePairs[i].Value};
                }

                keyValuePairs = newKeyValuePairs;
            }

            var keys = Keys;
            for (int i = 0; i < Count; i++)
            {
                keyValuePairs[i + index] = new KeyValuePair() {Key = keys[i], Value = this[keys[i]]};
            }
        }

        public bool Remove(string key)
        {
            if (buckets != null)
            {
                int hashCode = comparer.GetHashCode(key) & 0x7FFFFFFF;
                int bucket = hashCode % buckets.Length;
                int last = -1;
                for (int i = buckets[bucket]; i >= 0; last = i, i = entries[i].next)
                {
                    if (entries[i].hashCode == hashCode && comparer.Equals(entries[i].key, key))
                    {
                        if (last < 0)
                        {
                            buckets[bucket] = entries[i].next;
                        }
                        else
                        {
                            entries[last].next = entries[i].next;
                        }

                        entries[i].hashCode = -1;
                        entries[i].next = freeList;
                        entries[i].key = default;
                        entries[i].value = default;
                        freeList = i;
                        freeCount++;
                        version++;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}