using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using SimpleCollection;

namespace Pairs
{
    public record PersonCoder(string name, int age, DateTime lastTimeCoded);

    public class IdNameValue
    {
        [TestCase(1, "s2", true)]
        [TestCase(1, 1.0, 0x1)]
        [TestCase(typeof(PersonCoder), 1.0e04, 0x1)]
        public void Constructor<TKeyId, TKeyName, TKeyValue>(TKeyId id, TKeyName name, TKeyValue value)
        {
            Assert.DoesNotThrow(() => new IdNameValuePair<TKeyId, TKeyName, TKeyValue>(id, name, value));
        }
        
        [Test]
        [SuppressMessage("ReSharper", "OperatorIsCanBeUsed")]
        public void Constructor()
        {
            var peter = new PersonCoder("Peter", 31, DateTime.Now);
            
            var idNameValuePair = new IdNameValuePair<PersonCoder, int, string>(peter, 31, "value");

            Assert.IsTrue(idNameValuePair.Id.GetType() == peter.GetType() &&
                          idNameValuePair.Name.GetType() == typeof(int) &&
                          idNameValuePair.Value.GetType() == typeof(string));
        }

        [Test]
        public void EqualsAnd()
        {
            var peter = new PersonCoder("Peter", 31, DateTime.Now);
            
            var idNameValuePair = new IdNameValuePair<PersonCoder, string, string>(peter, "1.0e04", "0x1");

            Assert.IsTrue(idNameValuePair.Id == peter &&
                          idNameValuePair.Name == "1.0e04" &&
                          idNameValuePair.Value == "0x1");

            Assert.IsTrue(idNameValuePair.Id != null &&
                          idNameValuePair.Name != null &&
                          idNameValuePair.Value != null);
        }
    }
}