using System;
using System.Collections.Generic;

namespace SimpleCollection
{
    /// <summary>
    /// These structure are designed to extend the basic System.Collections.Generic.KeyValuePair
    /// because structs has no built-in compare
    /// </summary>
    /// <typeparam name="TId">First parameter of Key</typeparam>
    /// <typeparam name="TName">Second parameter of Key</typeparam>
    /// <typeparam name="TValue">Value parameter</typeparam>
    public readonly struct IdNameValuePair<TId, TName, TValue> 
    {
        public TId Id => _id;
        public TName Name => _name;
        public TValue Value => _value;

        private readonly TId _id;
        private readonly TName _name;
        private readonly TValue _value;

        public IdNameValuePair(TId id, TName name, TValue value)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            
            _id = id;
            _name = name;
            _value = value;
        }
        
        public bool EqualsKey(IdNameValuePair<TId, TName, TValue> other)
        {
            return EqualityComparer<TId>.Default.Equals(_id, other._id) && EqualityComparer<TName>.Default.Equals(_name, other._name);
        }

        public bool Equals(IdNameValuePair<TId, TName, TValue> other)
        {
            return EqualityComparer<TId>.Default.Equals(_id, other._id) && EqualityComparer<TName>.Default.Equals(_name, other._name) && EqualityComparer<TValue>.Default.Equals(_value, other._value);
        }

        public override bool Equals(object? obj)
        {
            return obj is IdNameValuePair<TId, TName, TValue> other && Equals(other);
        }

        public static bool operator ==(IdNameValuePair<TId, TName, TValue> currentIdNameValuePair, IdNameValuePair<TId, TName, TValue> anotherIdNameValuePair)
        {
            return Equals(currentIdNameValuePair, anotherIdNameValuePair);
        }

        public static bool operator !=(IdNameValuePair<TId, TName, TValue> currentIdNameValuePair, IdNameValuePair<TId, TName, TValue> anotherIdNameValuePair)
        {
            return !Equals(currentIdNameValuePair, anotherIdNameValuePair);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_id, _name, _value);
        }
    }
}