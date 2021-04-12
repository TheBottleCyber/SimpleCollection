using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleCollection
{
    /// <summary>
    /// A collection that supports 2 parameters for key and value
    /// </summary>
    /// <typeparam name="TId">First parameter of Key</typeparam>
    /// <typeparam name="TName">Second parameter of Key</typeparam>
    /// <typeparam name="TValue">Value parameter</typeparam>
    public sealed class SimpleCollection<TId, TName, TValue> : IList<IdNameValuePair<TId, TName, TValue>>
    {
        private const int DefaultCapacity = 4;
        private readonly IdNameValuePair<TId, TName, TValue>[] _emptyArray = Array.Empty<IdNameValuePair<TId, TName, TValue>>();
        internal IdNameValuePair<TId, TName, TValue>[] _items;
        internal int _size;

        public int Count => _size;
        public bool IsReadOnly => false;

        public int Capacity
        {
            get => _items.Length;
            set
            {
                if (value < _size)
                    throw new ArgumentOutOfRangeException(nameof(value), $"Value cannot be less than size");

                if (value != _items.Length)
                {
                    if (value > 0)
                    {
                        var newItems = new IdNameValuePair<TId, TName, TValue>[value];

                        if (_size > 0)
                        {
                            Array.Copy(_items, newItems, _size);
                        }

                        _items = newItems;
                    }
                    else
                    {
                        _items = _emptyArray;
                    }
                }
            }
        }

        public SimpleCollection()
        {
            _items = _emptyArray;
        }

        /// <param name="size">Size of new collection</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws if size less than zero</exception>
        public SimpleCollection(int size)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException(nameof(size), "Size cannot be less than zero");

            _items = size == 0 ? _emptyArray : new IdNameValuePair<TId, TName, TValue>[size];
        }

        /// <returns>Boolean of key matching</returns>
        /// <exception cref="ArgumentNullException">Throws if item equals null</exception>
        private bool KeyExistsInArray(IdNameValuePair<TId, TName, TValue> item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return Array.Exists(_items, x => x.EqualsKey(item));
        }

        public IdNameValuePair<TId, TName, TValue> this[int index]
        {
            get => _items[index];
            set
            {
                if (!KeyExistsInArray(value))
                    throw new ArgumentException("No one item matching Key");

                _items[index] = value;
            }
        }

        /// <summary>
        /// Ensure capacity
        /// </summary>
        private void Grow(int capacity)
        {
            int newcapacity = _items.Length == 0 ? DefaultCapacity : 2 * _items.Length;
            if (newcapacity < capacity) newcapacity = capacity;

            Capacity = newcapacity;
        }

        public void Add(IdNameValuePair<TId, TName, TValue> item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (KeyExistsInArray(item))
                throw new ArgumentException("Attempt to add a duplicate", nameof(item));

            int size = _size;
            if ((uint)size < (uint)_items.Length)
            {
                _size = size + 1;
                _items[size] = item;
            }
            else
            {
                Grow(size + 1);
                _size = size + 1;
                _items[size] = item;
            }
        }

        public void Clear()
        {
            int size = _size;
            _size = 0;

            if (size > 0)
            {
                Array.Clear(_items, 0, size);
            }
        }

        public bool Contains(IdNameValuePair<TId, TName, TValue> item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return IndexOf(item) != -1;
        }

        public void CopyTo(IdNameValuePair<TId, TName, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Index cannot be less than zero");

            Array.Copy(_items, 0, array, arrayIndex, _size);
        }

        public bool Remove(IdNameValuePair<TId, TName, TValue> item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            int index = IndexOf(item);

            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        public int IndexOf(IdNameValuePair<TId, TName, TValue> item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return Array.IndexOf(_items, item, 0, _size);
        }

        public void Insert(int index, IdNameValuePair<TId, TName, TValue> item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if ((uint)index > (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index cannot be over than array size");

            if (_size == _items.Length) Grow(_size + 1);
            if (index < _size)
            {
                Array.Copy(_items, index, _items, index + 1, _size - index);
            }

            _items[index] = item;
            _size++;
        }

        public void RemoveAt(int index)
        {
            if ((uint)index >= (uint)_size)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index cannot be over or equal than array size");

            _size--;

            if (index < _size)
            {
                Array.Copy(_items, index + 1, _items, index, _size - index);
            }
        }

        public IEnumerator<IdNameValuePair<TId, TName, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal struct Enumerator : IEnumerator<IdNameValuePair<TId, TName, TValue>>
        {
            private int _index;
            private SimpleCollection<TId, TName, TValue> _collection;
            private IdNameValuePair<TId, TName, TValue> _current;

            public IdNameValuePair<TId, TName, TValue> Current => _current;
            object IEnumerator.Current => Current;

            internal Enumerator(SimpleCollection<TId, TName, TValue> collection)
            {
                _collection = collection;
                _index = 0;
                _current = default;
            }

            public bool MoveNext()
            {
                if ((uint)_index < (uint)_collection._size)
                {
                    _current = _collection._items[_index];
                    _index++;
                    return true;
                }

                _index = _collection._size + 1;
                _current = default;

                return false;
            }

            public void Reset()
            {
                _index = 0;
                _current = default;
            }

            public void Dispose() { }
        }
    }
}