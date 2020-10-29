using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace SimpleCollection
{
    public struct Scheme<T1, T2, T3>
    {
        public T1 Id { get; set; }
        public T2 Name { get; set; }
        public T3 Value { get; set; }
    }

    public interface ISimpleCollection<T1, T2, T3> : IEnumerable<Scheme<T1, T2, T3>>
    {
        Scheme<T1, T2, T3> this[int index] { get; }

        int Count { get; }
        public void Add(Scheme<T1, T2, T3> item);
        public int IndexOf(T1 id, T2 name);
        public void RemoveAt(int index);
        public Scheme<T1, T2, T3> Find(Predicate<Scheme<T1, T2, T3>> match);
        public int FindIndex(int startIndex, int count, Predicate<Scheme<T1, T2, T3>> match);
    }

    /// <summary>
    ///     <br>SimpleCollection class with generic type parameters, inherits own interface</br>
    ///     <br>
    ///         also related
    ///         <see href="https://referencesource.microsoft.com/#mscorlib/system/collections/generic/list.cs">
    ///             list.cs source code
    ///         </see>
    ///     </br>
    /// </summary>
    public class SimpleCollection<T1, T2, T3> : ISimpleCollection<T1, T2, T3>, ICollection
    {
        private const int _defaultCapacity = 4;
        private readonly Scheme<T1, T2, T3>[] _emptySchemeArray = new Scheme<T1, T2, T3>[0];
        private Scheme<T1, T2, T3>[] _scheme;
        private object _syncRoot;
        private int _version;
        private bool _onlyUniqueKey;

        /// <summary>
        ///     SimpleCollection constructor
        /// <br>If using a numeric id, you should use false for onlyUniqueKey for best perfomance</br>
        /// </summary>
        /// <param name="onlyUniqueKey">Should the key be unique</param>
        public SimpleCollection(bool onlyUniqueKey = true)
        {
            _scheme = _emptySchemeArray;
            _onlyUniqueKey = onlyUniqueKey;
        }

        /// <summary>
        ///     Current collection capacity calculates as _scheme.Length * 2;
        /// </summary>
        public int Capacity
        {
            get => _scheme.Length;
            set
            {
                if (value != _scheme.Length)
                {
                    if (value > 0)
                    {
                        var newItems = new Scheme<T1, T2, T3>[value];

                        if (Count > 0)
                        {
                            Array.Copy(_scheme, 0, newItems, 0, Count);
                        }

                        _scheme = newItems;
                    }
                    else
                    {
                        _scheme = _emptySchemeArray;
                    }
                }
            }
        }

        /// <summary>
        ///     Related to ICollection inherit
        /// </summary>
        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
                }

                return _syncRoot;
            }
        }

        /// <summary>
        ///     Related to ICollection inherit
        /// </summary>
        bool ICollection.IsSynchronized => false;

        /// <summary>
        ///     General usage: copy scheme items to related Array
        /// </summary>
        /// <param name="array">Array to copy items</param>
        /// <param name="index">Destination index</param>
        public void CopyTo(Array array, int index)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            Array.Copy(_scheme, 0, array, index, Count);
        }

        /// <summary>
        ///     Get count of all Items
        /// </summary>
        public int Count { get; private set; }

        public Scheme<T1, T2, T3> this[int index]
        {
            get
            {
                if ((uint)index >= (uint)Count) throw new OutOfMemoryException();

                return _scheme[index];
            }
            set
            {
                if ((uint)index >= (uint)Count) throw new OutOfMemoryException();

                _scheme[index] = value;
                _version++;
            }
        }

        IEnumerator<Scheme<T1, T2, T3>> IEnumerable<Scheme<T1, T2, T3>>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        ///     General usage: adds item to collection
        /// </summary>
        /// <param name="item">Initialized Scheme item</param>
        public void Add(Scheme<T1, T2, T3> item)
        {
            Add(item.Id, item.Name, item.Value);
        }

        /// <summary>
        ///     General usage: get index from collection by generic types
        /// </summary>
        /// <param name="id">Generic id type</param>
        /// <param name="name">Generic name type</param>
        public int IndexOf(T1 id, T2 name)
        {
            var idIndex = IndexOf(id);
            var nameIndex = IndexOf(name);

            if (idIndex == nameIndex) return idIndex;

            return -1;
        }

        /// <summary>
        ///     General usage: remove item from collection by index
        /// </summary>
        /// <param name="index">Int32 index</param>
        public void RemoveAt(int index)
        {
            if (index == -1) throw new IndexOutOfRangeException(nameof(index));

            Count--;

            if (index < Count)
            {
                Array.Copy(_scheme, index + 1, _scheme, index, Count - index);
            }

            _scheme[Count] = default;
            _version++;
        }

        /// <summary>
        ///     General usage: update item from collection by match
        /// </summary>
        /// <param name="match">Scheme generic predicate</param>
        /// <param name="item">Scheme item</param>
        public void UpdateItem(Predicate<Scheme<T1, T2, T3>> match, Scheme<T1, T2, T3> item)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));

            var index = FindIndex(match);

            _scheme[index] = item;
        }

        /// <summary>
        ///     General usage: update item from collection by index
        /// </summary>
        /// <param name="index">Scheme generic predicate</param>
        /// <param name="item">Scheme item</param>
        public void UpdateItem(int index, Scheme<T1, T2, T3> item)
        {
            // OutOfRangeException if index wrong
            _scheme[index] = item;
        }

        /// <summary>
        ///     General usage: find one item from collection by predicate
        /// </summary>
        /// <param name="match">Scheme generic predicate</param>
        public Scheme<T1, T2, T3> Find(Predicate<Scheme<T1, T2, T3>> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));

            for (var i = 0; i < Count; i++)
            {
                if (match(_scheme[i]))
                {
                    return _scheme[i];
                }
            }

            return default;
        }

        /// <summary>
        ///     General usage: find index from collection by start, count and predicate
        /// </summary>
        /// <param name="startIndex">Start searching from</param>
        /// <param name="count">How many count needed</param>
        /// <param name="match">Scheme generic predicate</param>
        public int FindIndex(int startIndex, int count, Predicate<Scheme<T1, T2, T3>> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            if ((uint)startIndex > (uint)Count) throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, string.Empty);
            if (count < 0 || startIndex > Count - count) throw new ArgumentOutOfRangeException(nameof(count), count, string.Empty);

            var endIndex = startIndex + count;
            for (var i = startIndex; i < endIndex; i++)
            {
                if (match(_scheme[i])) return i;
            }

            return -1;
        }

        /// <summary>
        ///     General usage: adds item to collection
        /// </summary>
        /// <param name="id">Generic id type</param>
        /// <param name="name">Generic name type</param>
        /// <param name="value">Generic value type</param>
        public void Add(T1 id, T2 name, T3 value)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (_onlyUniqueKey && IndexOf(id, name) >= 0) throw new Exception($"Item has already exists by index of {nameof(id)} and {nameof(name)}");
            if (Count == _scheme.Length) EnsureCapacity(Count + 1);

            var sizeIncrement = Count++;

            _scheme[sizeIncrement].Id = id;
            _scheme[sizeIncrement].Name = name;
            _scheme[sizeIncrement].Value = value;

            _version++;
        }

        /// <summary>
        ///     General usage: get index from collection by generic type
        /// </summary>
        /// <param name="id">Generic id type</param>
        public int IndexOf(T1 id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            for (var i = 0; i < _scheme.Length; i++)
            {
                var item = _scheme[i];
                if (Equals(item.Id, id))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        ///     General usage: get index from collection by generic type
        /// </summary>
        /// <param name="name">Generic name type</param>
        public int IndexOf(T2 name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            for (var i = 0; i < _scheme.Length; i++)
            {
                var item = _scheme[i];
                if (Equals(item.Name, name))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        ///     General usage: remove item from collection by generic type
        /// </summary>
        /// <param name="id">Generic id type</param>
        public bool Remove(T1 id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var index = IndexOf(id);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     General usage: remove item from collection by generic type
        /// </summary>
        /// <param name="name">Generic name type</param>
        public bool Remove(T2 name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var index = IndexOf(name);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     General usage: remove item from collection by generic types
        /// </summary>
        /// <param name="id">Generic id type</param>
        /// <param name="name">Generic name type</param>
        public bool Remove(T1 id, T2 name)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (name == null) throw new ArgumentNullException(nameof(name));

            var index = IndexOf(id, name);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     General usage: find only generic Value from collection by generic type
        /// </summary>
        /// <param name="id">Generic id type</param>
        public T3 FindValue(T1 id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            return _scheme[IndexOf(id)].Value;
        }

        /// <summary>
        ///     General usage: find only generic Value from collection by generic type
        /// </summary>
        /// <param name="name">Generic name type</param>
        public T3 FindValue(T2 name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return _scheme[IndexOf(name)].Value;
        }

        /// <summary>
        ///     General usage: find only generic Value from collection by generic type
        /// </summary>
        /// <param name="id">Generic id type</param>
        /// <param name="name">Generic name type</param>
        public T3 FindValue(T1 id, T2 name)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return _scheme[IndexOf(id, name)].Value;
        }

        /// <summary>
        ///     General usage: find all item from collection by predicate and return new collection with results
        /// </summary>
        /// <param name="match">Scheme generic predicate</param>
        public SimpleCollection<T1, T2, T3> FindAll(Predicate<Scheme<T1, T2, T3>> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));

            var collection = new SimpleCollection<T1, T2, T3>();
            for (var i = 0; i < Count; i++)
            {
                if (match(_scheme[i]))
                {
                    collection.Add(_scheme[i]);
                }
            }

            return collection;
        }

        /// <summary>
        ///     General usage: find index from collection by predicate and startindex
        /// </summary>
        /// <param name="startIndex">Start searching from</param>
        /// <param name="match">Scheme generic predicate</param>
        public int FindIndex(int startIndex, Predicate<Scheme<T1, T2, T3>> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            return FindIndex(startIndex, Count - startIndex, match);
        }

        /// <summary>
        ///     General usage: find index from collection by predicate
        /// </summary>
        /// <param name="match">Scheme generic predicate</param>
        public int FindIndex(Predicate<Scheme<T1, T2, T3>> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            return FindIndex(0, Count, match);
        }

        /// <summary>
        ///     General usage: convert collection to array
        /// </summary>
        public Scheme<T1, T2, T3>[] ToArray()
        {
            var array = new Scheme<T1, T2, T3>[Count];
            Array.Copy(_scheme, 0, array, 0, Count);

            return array;
        }

        private void EnsureCapacity(int min)
        {
            if (_scheme.Length < min)
            {
                var newCapacity = _scheme.Length == 0 ? _defaultCapacity : _scheme.Length * 2;
                if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
                if (newCapacity < min) newCapacity = min;

                Capacity = newCapacity;
            }
        }

        public struct Enumerator : IEnumerator<Scheme<T1, T2, T3>>
        {
            private SimpleCollection<T1, T2, T3> _collection;
            private int index;
            private int version;

            internal Enumerator(SimpleCollection<T1, T2, T3> collection)
            {
                _collection = collection;
                index = 0;
                version = collection._version;

                Current = default;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                var localCollection = _collection;

                // magic versioning from list.cs
                if (version == localCollection._version && (uint)index < (uint)localCollection.Count)
                {
                    Current = localCollection[index];
                    index++;
                    return true;
                }

                index = _collection.Count + 1;
                Current = default;

                return false;
            }

            public Scheme<T1, T2, T3> Current { get; private set; }

            object IEnumerator.Current => Current;

            void IEnumerator.Reset()
            {
                index = 0;
                Current = default;
            }
        }
    }

    //idk how to make it work as thread-safe version 
    //public class SynchronizedSimpleCollection<T1, T2, T3> : ISimpleCollection<T1, T2, T3>
    //{
    //    private SimpleCollection<T1, T2, T3> _collection;
    //    private object _root;

    //    public SynchronizedSimpleCollection(SimpleCollection<T1, T2, T3> collection)
    //    {
    //        _collection = collection;
    //        _root = ((ICollection)_collection).SyncRoot;
    //    }

    //    public int Count
    //    {
    //        get
    //        {
    //            lock (_root) return _collection.Count;
    //        }
    //    }

    //    public void Add(Scheme<T1, T2, T3> item)
    //    {
    //        lock (_root) _collection.Add(item);
    //    }

    //    public int IndexOf(T1 id, T2 name)
    //    {
    //        lock (_root) return _collection.IndexOf(id, name);
    //    }

    //    public void RemoveAt(int index)
    //    {
    //        lock (_root) _collection.RemoveAt(index);
    //    }

    //    public Scheme<T1, T2, T3> Find(Predicate<Scheme<T1, T2, T3>> match)
    //    {
    //        lock (_root) return _collection.Find(match);
    //    }

    //    public int FindIndex(int startIndex, int count, Predicate<Scheme<T1, T2, T3>> match)
    //    {
    //        lock (_root) return _collection.FindIndex(startIndex, count, match);
    //    }

    //    public Scheme<T1, T2, T3> this[int index]
    //    {
    //        get
    //        {
    //            lock (_root) return _collection[index];
    //        }
    //        set
    //        {
    //            lock (_root)
    //            {
    //                _collection[index] = value;
    //            }
    //        }
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        lock (_root)
    //        {
    //            return _collection.GetEnumerator();
    //        }
    //    }

    //    IEnumerator<Scheme<T1, T2, T3>> IEnumerable<Scheme<T1, T2, T3>>.GetEnumerator()
    //    {
    //        lock (_root) return ((IEnumerable<Scheme<T1, T2, T3>>)_collection).GetEnumerator();
    //    }
    //}
}