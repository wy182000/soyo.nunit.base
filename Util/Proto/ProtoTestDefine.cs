using Soyo.Base.Proto;
using System.Collections.Generic;

namespace Soyo.Proto {
  public enum EnumTest {
    None,
    Test,
  }

  [ProtoSerializable]
  public class ProtoTestBase {
    [ProtoId(1)] public byte byteValue;
    [ProtoId(2)] public sbyte sbyteValue;
    [ProtoId(3)] public short shortValue;
    [ProtoId(4)] public ushort ushortValue;
    [ProtoId(5)] public int intValue;
    [ProtoId(6)] public uint uintValue;
    [ProtoId(7)] public long longValue;
    [ProtoId(8)] public ulong ulongValue;
    [ProtoId(9)] public float floatValue;
    [ProtoId(10)] public double doubleValue;
    [ProtoId(11)] public EnumTest enumValue;
    [ProtoId(12)] public string stringValue;
    [ProtoId(13)] public Soyo.Base.ByteBuffer bufferValue;
    [ProtoId(14)] public byte[] byteArrayValue;
    [ProtoId(15)] public byte? byteNullValue;
    [ProtoId(16)] public sbyte? sbyteNullValue;
    [ProtoId(17)] public short? shortNullValue;
    [ProtoId(18)] public ushort? ushortNullValue;
    [ProtoId(19)] public int? intNullValue;
    [ProtoId(20)] public uint? uintNullValue;
    [ProtoId(21)] public long? longNullValue;
    [ProtoId(22)] public ulong? ulongNullValue;
    [ProtoId(23)] public float? floatNullValue;
    [ProtoId(24)] public double? doubleNullValue;
    [ProtoId(25)] public Soyo.Base.ByteBuffer? bufferNullValue;
    [ProtoId(26)] public List<bool> boolListValue;
    [ProtoId(27)] public List<byte> byteListValue;
    [ProtoId(28)] public List<sbyte> sbyteListValue;
    [ProtoId(29)] public List<short> shortListValue;
    [ProtoId(30)] public List<ushort> ushortListValue;
    [ProtoId(31)] public List<int> intListValue;
    [ProtoId(32)] public List<uint> uintListValue;
    [ProtoId(33)] public List<long> longListValue;
    [ProtoId(34)] public List<ulong> ulongListValue;
    [ProtoId(35)] public List<float> floatListValue;
    [ProtoId(36)] public List<double> doubleListValue;
    [ProtoId(37)] public List<string> stringListValue;
    [ProtoId(38)] public List<Soyo.Base.ByteBuffer> bufferListValue;
    [ProtoId(39)] public List<byte[]> byteArrayListValue;
    [ProtoId(40)] public bool boolValue;
    [ProtoId(41)] public bool? boolNullValue;
    [ProtoId(42)] public Dictionary<uint, uint> uintUintDictionary;
  }

  [ProtoSerializable]
  public class ProtoTestClass {
    [ProtoId(1)] public int id;
    [ProtoId(2)] public List<ProtoTestBase> items;
  }
}
