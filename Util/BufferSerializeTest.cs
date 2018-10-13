using System;
using System.Collections.Generic;
using System.Text;

using Soyo.Base;
using Soyo.Base.Serialize;

using NUnit.Framework;

namespace UnitTest.Base.Util.Serialize {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class BufferSerializeTest {
    private void checkSerializeValue<T>(T value) {
      var writer = new ByteBufferWriter(BufferSerializer.Size(value));
      value.BufferWrite(writer);
      T valueSerialized = BufferSerializer.Read<T>(writer.GetBufferReader());
      Assert.AreEqual(value, valueSerialized, $"check serialize read failed, value: {value}, serialized value: {valueSerialized}");

      valueSerialized = BufferSerializer.Merge<T>(writer.GetBufferReader(), default(T));
      Assert.AreEqual(value, valueSerialized, $"check serialize merge failed, value: {value}, serialized value: {valueSerialized}");

      writer = new ByteBufferWriter(BufferSerializer.Size<object>(value));
      BufferSerializer.Write<object>(writer, value);
      valueSerialized = (T)BufferSerializer.Read<object>(writer.GetBufferReader());
      Assert.AreEqual(value, valueSerialized, $"check serialize object read failed, value: {value}, serialized value: {valueSerialized}");

      BufferSerializer.Write<object>(writer, value);
      valueSerialized = (T)BufferSerializer.Merge<object>(writer.GetBufferReader(), default(T));
      Assert.AreEqual(value, valueSerialized, $"check serialize object merge failed, value: {value}, serialized value: {valueSerialized}");
    }

    private enum CheckEnum {
      begin,
      one = begin,
      two,
      three,
      fore,
      five,
      six,
      seven,
      eight,
      night,
      ten,
      end
    }

    [Test]
    public void TestSerializeValue() {
      bool boolValue = Rand.Default.Range(0, 2) > 0;
      checkSerializeValue(boolValue);

      byte byteValue = Rand.Default.RandByte();
      checkSerializeValue(byteValue);

      sbyte sbyteValue = (sbyte)Rand.Default.RandByte();
      checkSerializeValue(sbyteValue);

      char charValue = (char)Rand.Default.RandByte();
      checkSerializeValue(charValue);

      short shortValue = Rand.Default.RandShort();
      checkSerializeValue(shortValue);

      ushort ushortValue = (ushort)Rand.Default.RandShort();
      checkSerializeValue(ushortValue);

      int intValue = Rand.Default.RandInt();
      checkSerializeValue(intValue);

      uint uintValue = Rand.Default.RandUint();
      checkSerializeValue(uintValue);

      long longValue = Rand.Default.RandLong();
      checkSerializeValue(longValue);

      ulong ulongValue = (ulong)Rand.Default.RandLong();
      checkSerializeValue(ulongValue);

      float floatValue = Rand.Default.RandFloat();
      checkSerializeValue(floatValue);

      double doubleValue = Rand.Default.RandFloat();
      checkSerializeValue(doubleValue);

      string stringValue = Rand.Default.RandFloat().ToString();
      checkSerializeValue(stringValue);

      CheckEnum enumValue = (CheckEnum)Rand.Default.Range((int)CheckEnum.begin, (int)CheckEnum.end);
      checkSerializeValue(enumValue);

      object nullValue = null;
      checkSerializeValue<object>(nullValue);
    }

    private void checkSerializeCollection<T, CollectionType>(CollectionType value) where CollectionType : ICollection<T> {
      var writer = new ByteBufferWriter(BufferSerializer.Size(value));
      value.BufferWrite(writer);
      CollectionType valueSerialized = BufferSerializer.Read<CollectionType>(writer.GetBufferReader());
      foreach (var i in value) {
        Assert.IsTrue(valueSerialized.Contains(i), $"check serialize read failed, value: {i}");
      }

      valueSerialized = BufferSerializer.Merge(writer.GetBufferReader(), default(CollectionType));
      foreach (var i in value) {
        Assert.IsTrue(valueSerialized.Contains(i), $"check serialize merge failed, value: {i}");
      }

      writer = new ByteBufferWriter(BufferSerializer.Size<object>(value));
      BufferSerializer.Write<object>(writer, value);
      valueSerialized = (CollectionType)BufferSerializer.Read<object>(writer.GetBufferReader());
      foreach (var i in value) {
        Assert.IsTrue(valueSerialized.Contains(i), $"check serialize object read failed, value: {i}");
      }

      BufferSerializer.Write<object>(writer, value);
      valueSerialized = (CollectionType)BufferSerializer.Merge<object>(writer.GetBufferReader(), default(CollectionType));
      foreach (var i in value) {
        Assert.IsTrue(valueSerialized.Contains(i), $"check serialize object merge failed, value: {i}");
      }
    }

    private void checkSerializeDictionary<Key, Value, DictionaryType>(DictionaryType value) where DictionaryType : IDictionary<Key, Value> {
      var writer = new ByteBufferWriter(BufferSerializer.Size(value));
      value.BufferWrite(writer);
      DictionaryType valueSerialized = BufferSerializer.Read<DictionaryType>(writer.GetBufferReader());
      foreach (var i in value) {
        Assert.IsTrue(valueSerialized.ContainsKey(i.Key), $"check serialize read failed, key: {i.Key}");
        Assert.AreEqual(value[i.Key], valueSerialized[i.Key], $"check serialize read failed, value: {value[i.Key]}, serialized value: {valueSerialized[i.Key]}");
      }

      valueSerialized = BufferSerializer.Merge(writer.GetBufferReader(), default(DictionaryType));
      foreach (var i in value) {
        Assert.IsTrue(valueSerialized.ContainsKey(i.Key), $"check serialize merge failed, key: {i.Key}");
        Assert.AreEqual(value[i.Key], valueSerialized[i.Key], $"check serialize merge failed, value: {value[i.Key]}, serialized value: {valueSerialized[i.Key]}");
      }

      writer = new ByteBufferWriter(BufferSerializer.Size<object>(value));
      BufferSerializer.Write<object>(writer, value);
      valueSerialized = (DictionaryType)BufferSerializer.Read<object>(writer.GetBufferReader());
      foreach (var i in value) {
        Assert.IsTrue(valueSerialized.ContainsKey(i.Key), $"check serialize object read failed, key: {i.Key}");
        Assert.AreEqual(value[i.Key], valueSerialized[i.Key], $"check serialize object read failed, value: {value[i.Key]}, serialized value: {valueSerialized[i.Key]}");
      }

      BufferSerializer.Write<object>(writer, value);
      valueSerialized = (DictionaryType)BufferSerializer.Merge<object>(writer.GetBufferReader(), default(DictionaryType));
      foreach (var i in value) {
        Assert.IsTrue(valueSerialized.ContainsKey(i.Key), $"check serialize object merge failed, key: {i.Key}");
        Assert.AreEqual(value[i.Key], valueSerialized[i.Key], $"check serialize object merge failed, value: {value[i.Key]}, serialized value: {valueSerialized[i.Key]}");
      }
    }

    [Test]
    public void TestSerializeCollection() {
      var intList = new List<int>();
      for (int i = 0; i < Rand.Default.Range(100, 200); i++) {
        intList.Add(Rand.Default.RandInt());
      }
      checkSerializeCollection<int, List<int>>(intList);
      checkSerializeCollection<int, ICollection<int>>(intList);

      var objectList = new List<object>();
      for (int i = 0; i < Rand.Default.Range(100, 200); i++) {
        objectList.Add(Rand.Default.RandInt());
      }
      checkSerializeCollection<object, List<object>>(objectList);
      checkSerializeCollection<object, ICollection<object>>(objectList);

      var intDictionary = new Dictionary<int, int>();
      for (int i = 0; i < Rand.Default.Range(100, 200); i++) {
        intDictionary.Add(i, Rand.Default.RandInt());
      }
      checkSerializeDictionary<int, int, Dictionary<int, int>>(intDictionary);
      checkSerializeDictionary<int, int, IDictionary<int, int>>(intDictionary);

      var objectDictionary = new Dictionary<object, object>();
      for (int i = 0; i < Rand.Default.Range(100, 200); i++) {
        objectDictionary.Add(i, Rand.Default.RandInt());
      }
      checkSerializeDictionary<object, object, Dictionary<object, object>>(objectDictionary);
      checkSerializeDictionary<object, object, IDictionary<object, object>>(objectDictionary);

      checkSerializeValue<List<int>>(null);
      checkSerializeValue<ICollection<int>>(null);
      checkSerializeValue<List<object>>(null);
      checkSerializeValue<ICollection<object>>(null);

      checkSerializeValue<Dictionary<int, int>>(null);
      checkSerializeValue<IDictionary<int, int>>(null);
      checkSerializeValue<Dictionary<object, object>>(null);
      checkSerializeValue<IDictionary<object, object>>(null);
    }

    private interface ICheckInt {
      int A { get; set; }
      int B { get; set; }
      int C { get; set; }
    }

    private class CheckClassInt : ICheckInt {
      public int A { get; set; }
      public int B { get; set; }
      public int C { get; set; }
      public int a;
      public int b;
      public int c;
    }

    private struct CheckStructInt : ICheckInt {
      public int A { get; set; }
      public int B { get; set; }
      public int C { get; set; }
      public int a;
      public int b;
      public int c;
    }

    private interface ICheckObject {
      object A { get; set; }
      object B { get; set; }
      object C { get; set; }
    }

    private class CheckClassObject : ICheckObject {
      public object A { get; set; }
      public object B { get; set; }
      public object C { get; set; }
      public object a;
      public object b;
      public object c;
    }

    private struct CheckStructObject : ICheckObject {
      public object A { get; set; }
      public object B { get; set; }
      public object C { get; set; }
      public object a;
      public object b;
      public object c;
    }

    private void checkSerializeInterface<T>(dynamic value) {
      var writer = new ByteBufferWriter(BufferSerializer.Size<T>(value));
      BufferSerializer.Write<T>(writer, value);
      dynamic valueSerialized = BufferSerializer.Read<T>(writer.GetBufferReader());
      Assert.AreEqual(value.A, valueSerialized.A, $"check serialize read failed, value A: {value.A}, serialized value A: {valueSerialized.A}");
      Assert.AreEqual(value.B, valueSerialized.B, $"check serialize read failed, value B: {value.B}, serialized value B: {valueSerialized.B}");
      Assert.AreEqual(value.C, valueSerialized.C, $"check serialize read failed, value C: {value.C}, serialized value C: {valueSerialized.C}");

      valueSerialized = BufferSerializer.Merge<T>(writer.GetBufferReader(), default(T));
      Assert.AreEqual(value.A, valueSerialized.A, $"check serialize merge failed, value A: {value.A}, serialized value A: {valueSerialized.A}");
      Assert.AreEqual(value.B, valueSerialized.B, $"check serialize merge failed, value B: {value.B}, serialized value B: {valueSerialized.B}");
      Assert.AreEqual(value.C, valueSerialized.C, $"check serialize merge failed, value C: {value.C}, serialized value C: {valueSerialized.C}");
    }

    private void checkSerializeClass<T>(dynamic value) {
      var writer = new ByteBufferWriter(BufferSerializer.Size<T>(value));
      BufferSerializer.Write<T>(writer, value);
      dynamic valueSerialized = BufferSerializer.Read<T>(writer.GetBufferReader());
      Assert.AreEqual(value.A, valueSerialized.A, $"check serialize read failed, value A: {value.A}, serialized value A: {valueSerialized.A}");
      Assert.AreEqual(value.a, valueSerialized.a, $"check serialize read failed, value a: {value.a}, serialized value a: {valueSerialized.a}");
      Assert.AreEqual(value.B, valueSerialized.B, $"check serialize read failed, value B: {value.B}, serialized value B: {valueSerialized.B}");
      Assert.AreEqual(value.b, valueSerialized.b, $"check serialize read failed, value b: {value.b}, serialized value b: {valueSerialized.b}");
      Assert.AreEqual(value.C, valueSerialized.C, $"check serialize read failed, value C: {value.C}, serialized value C: {valueSerialized.C}");
      Assert.AreEqual(value.c, valueSerialized.c, $"check serialize read failed, value c: {value.c}, serialized value c: {valueSerialized.c}");

      valueSerialized = BufferSerializer.Merge<T>(writer.GetBufferReader(), default(T));
      Assert.AreEqual(value.A, valueSerialized.A, $"check serialize merge failed, value A: {value.A}, serialized value A: {valueSerialized.A}");
      Assert.AreEqual(value.a, valueSerialized.a, $"check serialize merge failed, value a: {value.a}, serialized value a: {valueSerialized.a}");
      Assert.AreEqual(value.B, valueSerialized.B, $"check serialize merge failed, value B: {value.B}, serialized value B: {valueSerialized.B}");
      Assert.AreEqual(value.b, valueSerialized.b, $"check serialize merge failed, value b: {value.b}, serialized value b: {valueSerialized.b}");
      Assert.AreEqual(value.C, valueSerialized.C, $"check serialize merge failed, value C: {value.C}, serialized value C: {valueSerialized.C}");
      Assert.AreEqual(value.c, valueSerialized.c, $"check serialize merge failed, value c: {value.c}, serialized value c: {valueSerialized.c}");
    }

    [Test]
    public void TestSerializeClass() {
      var intClass = new CheckClassInt();
      intClass.A = Rand.Default.RandInt();
      intClass.a = Rand.Default.RandInt();
      intClass.B = Rand.Default.RandInt();
      intClass.b = Rand.Default.RandInt();
      intClass.C = Rand.Default.RandInt();
      intClass.c = Rand.Default.RandInt();
      checkSerializeClass<CheckClassInt>(intClass);
      checkSerializeClass<object>(intClass);
      checkSerializeInterface<ICheckInt>(intClass);

      var objectClass = new CheckClassObject();
      objectClass.A = Rand.Default.RandInt();
      objectClass.a = Rand.Default.RandInt();
      objectClass.B = Rand.Default.RandInt();
      objectClass.b = Rand.Default.RandInt();
      objectClass.C = Rand.Default.RandInt();
      objectClass.c = Rand.Default.RandInt();
      checkSerializeClass<CheckClassObject>(objectClass);
      checkSerializeClass<object>(objectClass);
      checkSerializeInterface<ICheckObject>(objectClass);

      var intStruct = new CheckStructInt();
      intStruct.A = Rand.Default.RandInt();
      intStruct.a = Rand.Default.RandInt();
      intStruct.B = Rand.Default.RandInt();
      intStruct.b = Rand.Default.RandInt();
      intStruct.C = Rand.Default.RandInt();
      intStruct.c = Rand.Default.RandInt();
      checkSerializeClass<CheckStructInt>(intStruct);
      checkSerializeClass<object>(intStruct);
      checkSerializeInterface<ICheckInt>(intStruct);

      var objectStruct = new CheckStructObject();
      objectStruct.A = Rand.Default.RandInt();
      objectStruct.a = Rand.Default.RandInt();
      objectStruct.B = Rand.Default.RandInt();
      objectStruct.b = Rand.Default.RandInt();
      objectStruct.C = Rand.Default.RandInt();
      objectStruct.c = Rand.Default.RandInt();
      checkSerializeClass<CheckStructObject>(objectStruct);
      checkSerializeClass<object>(objectStruct);
      checkSerializeInterface<ICheckObject>(objectStruct);

      checkSerializeValue<CheckClassInt>(null);
      checkSerializeValue<CheckClassObject>(null);
      checkSerializeValue<ICheckInt>(null);
      checkSerializeValue<ICheckObject>(null);
    }

    private class CheckAttribute {
      [BufferSerializeSkip] public int A { get; set; }
      [BufferSerializeSkip] public int a;

      [BufferSerialize] private int b;

      [BufferSerializeAs("A")] public int C { get; set; }
      [BufferSerializeAs("a")] public int c;

      public int Getb() => b;
      public void Setb(int value) => b = value;
    }

    private class CheckAttributeAs {
      public int A { get; set; }
      public int a;
      public int b;
    }

    [Test]
    public void TestSerializeAttribute() {
      var value = new CheckAttribute();
      value.A = Rand.Default.RandInt();
      value.a = Rand.Default.RandInt();
      value.Setb(Rand.Default.RandInt());
      value.C = Rand.Default.RandInt();
      value.c = Rand.Default.RandInt();

      var writer = new ByteBufferWriter(BufferSerializer.Size(value));
      value.BufferWrite(writer);
      var valueSerialized = BufferSerializer.Read<CheckAttribute>(writer.GetBufferReader());
      Assert.AreNotEqual(value.A, valueSerialized.A, $"check serialize skip failed, value A: {value.A}, serialized value A: {valueSerialized.A}");
      Assert.AreNotEqual(value.a, valueSerialized.a, $"check serialize skip failed, value a: {value.a}, serialized value a: {valueSerialized.a}");
      Assert.AreEqual(value.Getb(), valueSerialized.Getb(), $"check serialize failed, value b: {value.Getb()}, serialized value b: {valueSerialized.Getb()}");
      Assert.AreEqual(value.C, valueSerialized.C, $"check serialize read failed, value C: {value.C}, serialized value C: {valueSerialized.C}");
      Assert.AreEqual(value.c, valueSerialized.c, $"check serialize read failed, value c: {value.c}, serialized value c: {valueSerialized.c}");

      valueSerialized = BufferSerializer.Merge<CheckAttribute>(writer.GetBufferReader(), default(CheckAttribute));
      Assert.AreNotEqual(value.A, valueSerialized.A, $"check serialize skip failed, value A: {value.A}, serialized value A: {valueSerialized.A}");
      Assert.AreNotEqual(value.a, valueSerialized.a, $"check serialize skip failed, value a: {value.a}, serialized value a: {valueSerialized.a}");
      Assert.AreEqual(value.Getb(), valueSerialized.Getb(), $"check serialize failed, value b: {value.Getb()}, serialized value b: {valueSerialized.Getb()}");
      Assert.AreEqual(value.C, valueSerialized.C, $"check serialize read failed, value C: {value.C}, serialized value C: {valueSerialized.C}");
      Assert.AreEqual(value.c, valueSerialized.c, $"check serialize read failed, value c: {value.c}, serialized value c: {valueSerialized.c}");

      var valueAs = BufferSerializer.Read<CheckAttributeAs>(writer.GetBufferReader());
      Assert.AreEqual(value.C, valueAs.A, $"check serialize As failed, value A: {value.C}, serialized value A: {valueAs.A}");
      Assert.AreEqual(value.c, valueAs.a, $"check serialize As failed, value a: {value.c}, serialized value a: {valueAs.a}");
      Assert.AreEqual(value.Getb(), valueAs.b, $"check serialize failed, value b: {value.Getb()}, serialized value b: {valueAs.b}");
    }
  }
}
