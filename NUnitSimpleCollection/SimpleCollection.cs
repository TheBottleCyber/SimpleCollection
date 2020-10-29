using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SimpleCollection;

namespace NUnitSimpleCollection
{
    public class SimpleCollectionTest
    {
        private SimpleCollection<Guid, string, string> _simpleCollection;

        [Test]
        public void InitializeCollection()
        {
            _simpleCollection = new SimpleCollection<Guid, string, string>();
            Assert.IsNotNull(_simpleCollection);

            var customId = Guid.NewGuid();
            _simpleCollection.Add(customId, "1", "2");

            Assert.IsNotNull(_simpleCollection);
            Assert.IsNotNull(_simpleCollection.Count);
            Assert.AreEqual(_simpleCollection.Count, 1);

            try
            {
                _simpleCollection.Add(customId, "1", "2");
                Assert.Fail("May throw exception");
            }
            catch
            {
                Assert.Pass();
            }
        }

        [Test]
        public void CountItems()
        {
            _simpleCollection = new SimpleCollection<Guid, string, string>
            {
                {Guid.NewGuid(), "1", "2"},
                {Guid.NewGuid(), "3", "4"},
                {Guid.NewGuid(), "5", "6"}
            };
            Assert.AreEqual(_simpleCollection.Count, 3);

            _simpleCollection.Add(Guid.NewGuid(), "7", "8");
            Assert.AreEqual(_simpleCollection.Count, 4);
        }

        [Test]
        public void EnumerateItems()
        {
            _simpleCollection = new SimpleCollection<Guid, string, string>
            {
                {Guid.NewGuid(), "1", "2"},
                {Guid.NewGuid(), "2", "3"},
                {Guid.NewGuid(), "3", "4"}
            };

            var expectedEntities = new System.Collections.Generic.List<string> { "1", "2", "3" };

            Assert.AreEqual(expectedEntities.Count, _simpleCollection.Count);

            for (var i = 0; i < expectedEntities.Count; i++)
            {
                Assert.AreEqual(expectedEntities[i], _simpleCollection[i].Name);
            }
        }

        [Test]
        public void ConvertToArray()
        {
            _simpleCollection = new SimpleCollection<Guid, string, string>
            {
                {Guid.NewGuid(), "1", "2"},
                {Guid.NewGuid(), "2", "3"},
                {Guid.NewGuid(), "3", "4"}
            };

            var simpleCollectionArray = _simpleCollection.ToArray();

            Assert.IsNotNull(simpleCollectionArray);
            Assert.AreEqual(simpleCollectionArray.Length, 3);
        }

        [Test]
        public void RemoveItem()
        {
            var customId = Guid.NewGuid();
            _simpleCollection = new SimpleCollection<Guid, string, string>
            {
                {customId, "1", "2"},
                {Guid.NewGuid(), "2", "3"},
                {Guid.NewGuid(), "3", "4"},
                {Guid.NewGuid(), "4", "5"},
                {Guid.NewGuid(), "5", "4"}
            };

            Assert.AreEqual(_simpleCollection.Count, 5);

            _simpleCollection.Remove("4");
            Assert.AreEqual(_simpleCollection.Count, 4);

            _simpleCollection.Remove(customId);
            Assert.AreEqual(_simpleCollection.Count, 3);

            _simpleCollection.RemoveAt(4);
            Assert.AreEqual(_simpleCollection.Count, 2);

            var expectedEntities = new System.Collections.Generic.List<string> { "2", "3" };

            Assert.AreEqual(expectedEntities.Count, _simpleCollection.Count);

            for (var i = 0; i < expectedEntities.Count; i++)
            {
                Assert.AreEqual(expectedEntities[i], _simpleCollection[i].Name);
            }
        }

        [Test]
        public void FindValue()
        {
            var customId = Guid.NewGuid();
            _simpleCollection = new SimpleCollection<Guid, string, string>
            {
                {customId, "1", "2"},
                {Guid.NewGuid(), "2", "3"},
                {Guid.NewGuid(), "3", "4"},
                {Guid.NewGuid(), "4", "5"}
            };

            Assert.AreEqual(_simpleCollection.FindValue("3"), _simpleCollection[2].Value);
            Assert.AreEqual(_simpleCollection.FindValue(customId), _simpleCollection[0].Value);
            Assert.AreEqual(_simpleCollection.FindValue(customId, "1"), _simpleCollection[0].Value);
        }

        [Test]
        public void FindAll()
        {
            var customId = Guid.NewGuid();
            _simpleCollection = new SimpleCollection<Guid, string, string>
            {
                {customId, "1", "2"},
                {Guid.NewGuid(), "2", "2"},
                {Guid.NewGuid(), "3", "2"},
                {Guid.NewGuid(), "4", "5"},
                {Guid.NewGuid(), "5", "113"},
                {Guid.NewGuid(), "6", "5"}
            };

            var findCollection = _simpleCollection.FindAll(x => x.Value == "2");

            Assert.IsNotNull(findCollection);
            Assert.AreEqual(findCollection.Count, 3);

            var expectedEntities = new System.Collections.Generic.List<string> { "1", "2", "3" };

            for (var i = 0; i < expectedEntities.Count; i++)
            {
                Assert.AreEqual(expectedEntities[i], findCollection[i].Name);
            }
        }

        [Test]
        public void FindItem()
        {
            var customId = Guid.NewGuid();
            _simpleCollection = new SimpleCollection<Guid, string, string>
            {
                {customId, "1", "2"},
                {Guid.NewGuid(), "2", "2"},
                {Guid.NewGuid(), "3", "2"},
                {Guid.NewGuid(), "4", "5"}
            };

            var findItem = _simpleCollection.Find(x => x.Value == "2" && x.Id == customId);

            Assert.IsNotNull(findItem);
            Assert.AreEqual(findItem.Id, customId);
            Assert.AreEqual(findItem.Name, "1");
            Assert.AreEqual(findItem.Value, "2");
        }

        [Test]
        public void FindIndex()
        {
            _simpleCollection = new SimpleCollection<Guid, string, string>
            {
                {Guid.NewGuid(), "1", "2"},
                {Guid.NewGuid(), "2", "2"},
                {Guid.NewGuid(), "3", "2"},
                {Guid.NewGuid(), "4", "5"}
            };

            var index = _simpleCollection.FindIndex(x => x.Name == "1");

            Assert.IsNotNull(index);
            Assert.AreEqual(index, 0);
        }
    }
}