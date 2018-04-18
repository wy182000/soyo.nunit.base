using System;
using System.Collections.Generic;
using System.Text;

using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class TestProto {
    private const int checkCount = 16;

    private Soyo.Proto.ProtoTestBase generateProtoTestBase() {
      var data = new Soyo.Proto.ProtoTestBase();
      data.boolValue = Rand.Default.Range(0, 2) > 0;
      data.byteValue = Rand.Default.RandByte();
      data.sbyteValue = (sbyte)-Rand.Default.RandByte();
      data.shortValue = (short)-Rand.Default.RandShort();
      data.ushortValue = (ushort)Rand.Default.RandShort();
      data.intValue = -Rand.Default.RandInt();
      data.uintValue = Rand.Default.RandUint();
      data.longValue = -Rand.Default.RandLong();
      data.ulongValue = (ulong)Rand.Default.RandLong();
      data.enumValue = (Rand.Default.Range(0, 2) > 0) ? Soyo.Proto.EnumTest.Test : Soyo.Proto.EnumTest.None;
      data.floatValue = Rand.Default.RandFloat();
      data.doubleValue = Rand.Default.RandFloat() * Rand.Default.RandFloat();
      data.stringValue = "please test me.";
      var buffer = new byte[] {
        Rand.Default.RandByte(),
        Rand.Default.RandByte(),
        Rand.Default.RandByte(),
        Rand.Default.RandByte(),
        Rand.Default.RandByte(),
        Rand.Default.RandByte()
      };
      data.byteArrayValue = buffer;
      buffer = new byte[] {
        Rand.Default.RandByte(),
        Rand.Default.RandByte(),
        Rand.Default.RandByte(),
        Rand.Default.RandByte(),
        Rand.Default.RandByte(),
        Rand.Default.RandByte()
      };
      data.bufferValue = new ByteBuffer(buffer);

      // nullable
      bool? boolNull = Rand.Default.Range(0, 2) > 0;
      data.boolNullValue = (Rand.Default.Range(0, 2) > 0) ? boolNull : null;
      byte? byteNull = Rand.Default.RandByte();
      data.byteNullValue = (Rand.Default.Range(0, 2) > 0) ? byteNull : null;
      sbyte? sbyteNull = (sbyte)-Rand.Default.RandByte();
      data.sbyteNullValue = (Rand.Default.Range(0, 2) > 0) ? sbyteNull : null;
      short? shortNull = (short)-Rand.Default.RandShort();
      data.shortNullValue = (Rand.Default.Range(0, 2) > 0) ? shortNull : null;
      ushort? ushortNull = (ushort)Rand.Default.RandShort();
      data.ushortNullValue = (Rand.Default.Range(0, 2) > 0) ? ushortNull : null;
      int? intNull = -Rand.Default.RandInt();
      data.intNullValue = (Rand.Default.Range(0, 2) > 0) ? intNull : null;
      uint? uintNull = Rand.Default.RandUint();
      data.uintNullValue = (Rand.Default.Range(0, 2) > 0) ? uintNull : null;
      long? longNull = -Rand.Default.RandLong();
      data.longNullValue = (Rand.Default.Range(0, 2) > 0) ? longNull : null;
      ulong? ulongNull = (ulong)Rand.Default.RandLong();
      data.ulongNullValue = (Rand.Default.Range(0, 2) > 0) ? ulongNull : null;
      float? floatNull = Rand.Default.RandFloat();
      data.floatNullValue = (Rand.Default.Range(0, 2) > 0) ? floatNull : null;
      double? doubleNull = Rand.Default.RandFloat() * Rand.Default.RandFloat();
      data.doubleNullValue = (Rand.Default.Range(0, 2) > 0) ? doubleNull : null;
      ByteBuffer? bufferNull = new ByteBuffer(new byte[] {
        Rand.Default.RandByte(),
        Rand.Default.RandByte(),
        Rand.Default.RandByte(),
        Rand.Default.RandByte(),
        Rand.Default.RandByte(),
        Rand.Default.RandByte()
      });
      data.bufferNullValue = (Rand.Default.Range(0, 2) > 0) ? bufferNull : null;

      // list
      data.boolListValue = Rand.Default.Range(0, 2) > 0 ? new List<bool>() : null;
      if (data.boolListValue != null) {
        int count = Rand.Default.RandInt(checkCount);
        for (int i = 0; i < count; i++) {
          data.boolListValue.Add(Rand.Default.Range(0, 2) > 0);
        }
      }
      data.byteListValue = Rand.Default.Range(0, 2) > 0 ? new List<byte>() : null;
      if (data.byteListValue != null) {
        int count = Rand.Default.RandInt(checkCount);
        for (int i = 0; i < count; i++) {
          data.byteListValue.Add(Rand.Default.RandByte());
        }
      }
      data.sbyteListValue = Rand.Default.Range(0, 2) > 0 ? new List<sbyte>() : null;
      if (data.sbyteListValue != null) {
        int count = Rand.Default.RandInt(checkCount);
        for (int i = 0; i < count; i++) {
          data.sbyteListValue.Add((sbyte)-Rand.Default.RandByte());
        }
      }
      data.shortListValue = Rand.Default.Range(0, 2) > 0 ? new List<short>() : null;
      if (data.shortListValue != null) {
        int count = Rand.Default.RandInt(checkCount);
        for (int i = 0; i < count; i++) {
          data.shortListValue.Add((short)-Rand.Default.RandShort());
        }
      }
      data.ushortListValue = Rand.Default.Range(0, 2) > 0 ? new List<ushort>() : null;
      if (data.ushortListValue != null) {
        int count = Rand.Default.RandInt(checkCount);
        for (int i = 0; i < count; i++) {
          data.ushortListValue.Add((ushort)Rand.Default.RandShort());
        }
      }
      data.intListValue = Rand.Default.Range(0, 2) > 0 ? new List<int>() : null;
      if (data.intListValue != null) {
        int count = Rand.Default.RandInt(checkCount);
        for (int i = 0; i < count; i++) {
          data.intListValue.Add(-Rand.Default.RandInt());
        }
      }
      data.uintListValue = Rand.Default.Range(0, 2) > 0 ? new List<uint>() : null;
      if (data.uintListValue != null) {
        int count = Rand.Default.RandInt(checkCount);
        for (int i = 0; i < count; i++) {
          data.uintListValue.Add(Rand.Default.RandUint());
        }
      }
      data.longListValue = Rand.Default.Range(0, 2) > 0 ? new List<long>() : null;
      if (data.longListValue != null) {
        int count = Rand.Default.RandInt(checkCount);
        for (int i = 0; i < count; i++) {
          data.longListValue.Add(-Rand.Default.RandLong());
        }
      }
      data.ulongListValue = Rand.Default.Range(0, 2) > 0 ? new List<ulong>() : null;
      if (data.ulongListValue != null) {
        int count = Rand.Default.RandInt(checkCount);
        for (int i = 0; i < count; i++) {
          data.ulongListValue.Add((ulong)Rand.Default.RandLong());
        }
      }
      data.floatListValue = Rand.Default.Range(0, 2) > 0 ? new List<float>() : null;
      if (data.floatListValue != null) {
        int count = Rand.Default.RandInt(checkCount);
        for (int i = 0; i < count; i++) {
          data.floatListValue.Add(Rand.Default.RandFloat());
        }
      }
      data.doubleListValue = Rand.Default.Range(0, 2) > 0 ? new List<double>() : null;
      if (data.doubleListValue != null) {
        int count = Rand.Default.RandInt(checkCount);
        for (int i = 0; i < count; i++) {
          data.doubleListValue.Add(Rand.Default.RandFloat() * Rand.Default.RandFloat());
        }
      }
      data.stringListValue = Rand.Default.Range(0, 2) > 0 ? new List<string>() : null;
      if (data.stringListValue != null) {
        int count = Rand.Default.RandInt(checkCount);
        for (int i = 0; i < count; i++) {
          data.stringListValue.Add("test me" + i);
        }
      }
      data.byteArrayListValue = Rand.Default.Range(0, 2) > 0 ? new List<byte[]>() : null;
      if (data.byteArrayListValue != null) {
        int count = Rand.Default.RandInt(checkCount);
        for (int i = 0; i < count; i++) {
          var tmp = new byte[] {
            Rand.Default.RandByte(),
            Rand.Default.RandByte(),
            Rand.Default.RandByte(),
            Rand.Default.RandByte(),
            Rand.Default.RandByte(),
            Rand.Default.RandByte()
          };
          data.byteArrayListValue.Add(tmp);
        }
      }
      data.bufferListValue = Rand.Default.Range(0, 2) > 0 ? new List<ByteBuffer>() : null;
      if (data.bufferListValue != null) {
        int count = Rand.Default.RandInt(checkCount);
        for (int i = 0; i < count; i++) {
          var tmp = new ByteBuffer(new byte[] {
            Rand.Default.RandByte(),
            Rand.Default.RandByte(),
            Rand.Default.RandByte(),
            Rand.Default.RandByte(),
            Rand.Default.RandByte(),
            Rand.Default.RandByte()
          });
          data.bufferListValue.Add(tmp);
        }
      }
      return data;
    }

    private Soyo.Proto.ProtoTestClass generateProtoTestClass() {
      var data = new Soyo.Proto.ProtoTestClass();
      data.id = Rand.Default.RandInt();
      data.items = new List<Soyo.Proto.ProtoTestBase>();
      var count = Rand.Default.RandInt(checkCount);
      for (int i = 0; i < count; i++) {
        data.items.Add(generateProtoTestBase());
      }
      return data;
    }

    private void checkProteTestBase(Soyo.Proto.ProtoTestBase src, Soyo.Proto.ProtoTestBase dest) {
      Assert.IsNotNull(src);
      Assert.IsNotNull(dest);

      Assert.AreEqual(src.boolValue, dest.boolValue);
      Assert.AreEqual(src.byteValue, dest.byteValue);
      Assert.AreEqual(src.sbyteValue, dest.sbyteValue);
      Assert.AreEqual(src.shortValue, dest.shortValue);
      Assert.AreEqual(src.ushortValue, dest.ushortValue);
      Assert.AreEqual(src.intValue, dest.intValue);
      Assert.AreEqual(src.uintValue, dest.uintValue);
      Assert.AreEqual(src.longValue, dest.longValue);
      Assert.AreEqual(src.ulongValue, dest.ulongValue);
      Assert.AreEqual(src.enumValue, dest.enumValue);
      Assert.AreEqual(src.floatValue, dest.floatValue);
      Assert.AreEqual(src.doubleValue, dest.doubleValue);
      Assert.AreEqual(src.stringValue, dest.stringValue);
      Assert.AreEqual(src.byteArrayValue?.Length ?? 0, dest.byteArrayValue?.Length ?? 0);
      for (int i = 0; i < (src.byteArrayValue?.Length ?? 0); i++) {
        Assert.AreEqual(src.byteArrayValue[i], dest.byteArrayValue[i]);
      }
      Assert.AreEqual(src.bufferValue.Count, dest.bufferValue.Count);
      for (int i = 0; i < src.bufferValue.Count; i++) {
        Assert.AreEqual(src.bufferValue[i], dest.bufferValue[i]);
      }

      Assert.AreEqual(src.boolNullValue, dest.boolNullValue);
      Assert.AreEqual(src.sbyteNullValue, dest.sbyteNullValue);
      Assert.AreEqual(src.shortNullValue, dest.shortNullValue);
      Assert.AreEqual(src.ushortNullValue, dest.ushortNullValue);
      Assert.AreEqual(src.intNullValue, dest.intNullValue);
      Assert.AreEqual(src.uintNullValue, dest.uintNullValue);
      Assert.AreEqual(src.longNullValue, dest.longNullValue);
      Assert.AreEqual(src.ulongNullValue, dest.ulongNullValue);
      Assert.AreEqual(src.floatNullValue, dest.floatNullValue);
      Assert.AreEqual(src.doubleNullValue, dest.doubleNullValue);
      Assert.AreEqual(src.bufferNullValue.GetValueOrDefault().Count, dest.bufferNullValue.GetValueOrDefault().Count);
      for (int i = 0; i < src.bufferNullValue.GetValueOrDefault().Count; i++) {
        Assert.AreEqual(src.bufferNullValue.GetValueOrDefault()[i], dest.bufferNullValue.GetValueOrDefault()[i]);
      }

      Assert.AreEqual(src.boolListValue?.Count ?? 0, dest.boolListValue?.Count ?? 0);
      for (int i = 0; i < (src.boolListValue?.Count ?? 0); i++) {
        Assert.AreEqual(src.boolListValue[i], dest.boolListValue[i]);
      }
      Assert.AreEqual(src.byteListValue?.Count ?? 0, dest.byteListValue?.Count ?? 0);
      for (int i = 0; i < (src.byteListValue?.Count ?? 0); i++) {
        Assert.AreEqual(src.byteListValue[i], dest.byteListValue[i]);
      }
      Assert.AreEqual(src.sbyteListValue?.Count ?? 0, dest.sbyteListValue?.Count ?? 0);
      for (int i = 0; i < (src.sbyteListValue?.Count ?? 0); i++) {
        Assert.AreEqual(src.sbyteListValue[i], dest.sbyteListValue[i]);
      }
      Assert.AreEqual(src.shortListValue?.Count ?? 0, dest.shortListValue?.Count ?? 0);
      for (int i = 0; i < (src.shortListValue?.Count ?? 0); i++) {
        Assert.AreEqual(src.shortListValue[i], dest.shortListValue[i]);
      }
      Assert.AreEqual(src.ushortListValue?.Count ?? 0, dest.ushortListValue?.Count ?? 0);
      for (int i = 0; i < (src.ushortListValue?.Count ?? 0); i++) {
        Assert.AreEqual(src.ushortListValue[i], dest.ushortListValue[i]);
      }
      Assert.AreEqual(src.intListValue?.Count ?? 0, dest.intListValue?.Count ?? 0);
      for (int i = 0; i < (src.intListValue?.Count ?? 0); i++) {
        Assert.AreEqual(src.intListValue[i], dest.intListValue[i]);
      }
      Assert.AreEqual(src.uintListValue?.Count ?? 0, dest.uintListValue?.Count ?? 0);
      for (int i = 0; i < (src.uintListValue?.Count ?? 0); i++) {
        Assert.AreEqual(src.uintListValue[i], dest.uintListValue[i]);
      }
      Assert.AreEqual(src.longListValue?.Count ?? 0, dest.longListValue?.Count ?? 0);
      for (int i = 0; i < (src.longListValue?.Count ?? 0); i++) {
        Assert.AreEqual(src.longListValue[i], dest.longListValue[i]);
      }
      Assert.AreEqual(src.ulongListValue?.Count ?? 0, dest.ulongListValue?.Count ?? 0);
      for (int i = 0; i < (src.ulongListValue?.Count ?? 0); i++) {
        Assert.AreEqual(src.ulongListValue[i], dest.ulongListValue[i]);
      }
      Assert.AreEqual(src.floatListValue?.Count ?? 0, dest.floatListValue?.Count ?? 0);
      for (int i = 0; i < (src.floatListValue?.Count ?? 0); i++) {
        Assert.AreEqual(src.floatListValue[i], dest.floatListValue[i]);
      }
      Assert.AreEqual(src.doubleListValue?.Count ?? 0, dest.doubleListValue?.Count ?? 0);
      for (int i = 0; i < (src.doubleListValue?.Count ?? 0); i++) {
        Assert.AreEqual(src.doubleListValue[i], dest.doubleListValue[i]);
      }
      Assert.AreEqual(src.stringListValue?.Count ?? 0, dest.stringListValue?.Count ?? 0);
      for (int i = 0; i < (src.stringListValue?.Count ?? 0); i++) {
        Assert.AreEqual(src.stringListValue[i], dest.stringListValue[i]);
      }
      Assert.AreEqual(src.bufferListValue?.Count ?? 0, dest.bufferListValue?.Count ?? 0);
      for (int i = 0; i < (src.bufferListValue?.Count ?? 0); i++) {
        Assert.AreEqual(src.bufferListValue[i].Count, dest.bufferListValue[i].Count);
        for (int j = 0; j < src.bufferListValue[i].Count; j++) {
          Assert.AreEqual(src.bufferListValue[i][j], dest.bufferListValue[i][j]);
        }
      }
      Assert.AreEqual(src.byteArrayListValue?.Count ?? 0, dest.byteArrayListValue?.Count ?? 0);
      for (int i = 0; i < (src.byteArrayListValue?.Count ?? 0); i++) {
        Assert.AreEqual(src.byteArrayListValue[i]?.Length, dest.byteArrayListValue[i]?.Length);
        for (int j = 0; j < src.byteArrayListValue[i]?.Length; j++) {
          Assert.AreEqual(src.byteArrayListValue[i][j], dest.byteArrayListValue[i][j]);
        }
      }
    }

    private void checkProteTestClass(Soyo.Proto.ProtoTestClass src, Soyo.Proto.ProtoTestClass dest) {
      Assert.IsNotNull(src);
      Assert.IsNotNull(dest);
      Assert.AreEqual(src.id, dest.id);
      Assert.AreEqual(src.items?.Count ?? 0, dest.items?.Count ?? 0);
      for (int i = 0; i < (src.items?.Count ?? 0); i++) {
        checkProteTestBase(src.items[i], dest.items[i]);
      }
    }

    [Test]
    public void TestSerialize() {
      Soyo.Proto.ProtoTest.Initialize();
      Rand.Default.Seed = (int)Clock.NowSeconds;
      var count = Rand.Default.RandInt(checkCount) + 1;
      Assert.Less(0, count);
      for (int i = 0; i < count; i++) {
        var data = generateProtoTestClass();
        var buffer = Soyo.Base.Proto.ProtoConvert.ToBuffer(data);
        var ret = Soyo.Base.Proto.ProtoConvert.Merge(buffer, new Soyo.Proto.ProtoTestClass());
        checkProteTestClass(data, ret);
        ret = null;
        var block = Soyo.Base.Proto.ProtoConvert.ToBlock(data);
        ret = Soyo.Base.Proto.ProtoConvert.Merge(block, new Soyo.Proto.ProtoTestClass());
        checkProteTestClass(data, ret);
      }
    }
  }
}
