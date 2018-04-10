﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Soyo.Base.Log.Util;

using NUnit.Framework;

namespace UnitTest.Base.Log.Util {
  /// <summary>
  /// Used for internal unit testing the <see cref="PropertiesDictionary"/> class.
  /// </summary>
  /// <remarks>
  /// Used for internal unit testing the <see cref="PropertiesDictionary"/> class.
  /// </remarks>
  [TestFixture]
  public class PropertiesDictionaryTest {
    [Test]
    public void TestSerialization() {
      PropertiesDictionary pd = new PropertiesDictionary();

      for (int i = 0; i < 10; i++) {
        pd[i.ToString()] = i;
      }

      Assert.AreEqual(10, pd.Count, "Dictionary should have 10 items");

      // Serialize the properties into a memory stream
      BinaryFormatter formatter = new BinaryFormatter();
      MemoryStream memory = new MemoryStream();
      formatter.Serialize(memory, pd);

      // Deserialize the stream into a new properties dictionary
      memory.Position = 0;
      PropertiesDictionary pd2 = (PropertiesDictionary)formatter.Deserialize(memory);

      Assert.AreEqual(10, pd2.Count, "Deserialized Dictionary should have 10 items");

      foreach (string key in pd.GetKeys()) {
        Assert.AreEqual(pd[key], pd2[key], "Check Value Persisted for key [{0}]", key);
      }
    }
  }
}