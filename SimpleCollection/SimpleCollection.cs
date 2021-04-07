using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCollection
{
    // TODO: inherit from list due large memory consumption, bad enumerator and using overheaded ValuePairs
    public sealed class SimpleCollection<TId, TName, TValue> : Dictionary<IdNamePair<TId, TName>, TValue>, IEnumerable<IdNameValuePair<TId, TName, TValue>>
    {
        public new IEnumerator<IdNameValuePair<TId, TName, TValue>> GetEnumerator()
        {
            return new Enumerator(base.GetEnumerator());
        }

        public new void Add(IdNamePair<TId, TName> id, TValue value)
        {
            if(id == null)
                throw new ArgumentNullException(nameof(id));
            
            base.Add(id, value);
        }

        public new void Remove(IdNamePair<TId, TName> id)
        {
            if(id == null)
                throw new ArgumentNullException(nameof(id));
            
            base.Remove(id);
        }

        public new bool TryGetValue(IdNamePair<TId, TName> id, out TValue? value)
        {
            if(id == null)
                throw new ArgumentNullException(nameof(id));
            
            return base.TryGetValue(id, out value);
        }

        public bool TryGetValue(TId id, out TValue? value)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            value = default;
            var key = Keys.FirstOrDefault(x => x.Id != null && x.Id.Equals(id));

            return key != null && base.TryGetValue(key, out value);
        }

        public bool TryGetValue(TName name, out TValue? value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            value = default;
            var key = Keys.FirstOrDefault(x => x.Name != null && x.Name.Equals(name));

            return key != null && base.TryGetValue(key, out value);
        }

        private new struct Enumerator : IEnumerator<IdNameValuePair<TId, TName, TValue>>
        {
            private Dictionary<IdNamePair<TId, TName>, TValue>.Enumerator baseEnumerator;

            public Enumerator(Dictionary<IdNamePair<TId, TName>, TValue>.Enumerator enumerator)
            {
                baseEnumerator = enumerator;
            }

            public bool MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            public void Reset()
            {
                baseEnumerator = default;
            }

            public IdNameValuePair<TId, TName, TValue> Current => new(baseEnumerator.Current.Key.Id, baseEnumerator.Current.Key.Name, baseEnumerator.Current.Value);

            object IEnumerator.Current => Current;

            public void Dispose() { }
        }
    }
}