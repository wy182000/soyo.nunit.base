// Generated Code. DO NOT EDIT.
using System;
using Soyo.Base.Proto;

namespace Soyo.Proto {
  class Soyo_Proto_ProtoTestBase_Serializer: IProtoSerializer<Soyo.Proto.ProtoTestBase> {
    public Soyo.Proto.ProtoTestBase Merge(ProtoReader input, Soyo.Proto.ProtoTestBase value) {
      int oldLimit = input.ReadClassBegin();
      while (input.HasField) {
          switch (input.FieldId) {
              case 1: value.byteValue = input.Merge(value.byteValue); break;
              case 2: value.sbyteValue = input.Merge(value.sbyteValue); break;
              case 3: value.shortValue = input.Merge(value.shortValue); break;
              case 4: value.ushortValue = input.Merge(value.ushortValue); break;
              case 5: value.intValue = input.Merge(value.intValue); break;
              case 6: value.uintValue = input.Merge(value.uintValue); break;
              case 7: value.longValue = input.Merge(value.longValue); break;
              case 8: value.ulongValue = input.Merge(value.ulongValue); break;
              case 9: value.floatValue = input.Merge(value.floatValue); break;
              case 10: value.doubleValue = input.Merge(value.doubleValue); break;
              case 11: value.enumValue = (Soyo.Proto.EnumTest)input.Merge((int)value.enumValue); break;
              case 12: value.stringValue = input.Merge(value.stringValue); break;
              case 13: value.bufferValue = input.Merge(value.bufferValue); break;
              case 14: value.byteArrayValue = input.Merge(value.byteArrayValue); break;
              case 15: value.byteNullValue = input.Merge(value.byteNullValue); break;
              case 16: value.sbyteNullValue = input.Merge(value.sbyteNullValue); break;
              case 17: value.shortNullValue = input.Merge(value.shortNullValue); break;
              case 18: value.ushortNullValue = input.Merge(value.ushortNullValue); break;
              case 19: value.intNullValue = input.Merge(value.intNullValue); break;
              case 20: value.uintNullValue = input.Merge(value.uintNullValue); break;
              case 21: value.longNullValue = input.Merge(value.longNullValue); break;
              case 22: value.ulongNullValue = input.Merge(value.ulongNullValue); break;
              case 23: value.floatNullValue = input.Merge(value.floatNullValue); break;
              case 24: value.doubleNullValue = input.Merge(value.doubleNullValue); break;
              case 25: value.bufferNullValue = input.Merge(value.bufferNullValue); break;
              case 26: value.boolListValue = input.Merge(value.boolListValue); break;
              case 27: value.byteListValue = input.Merge(value.byteListValue); break;
              case 28: value.sbyteListValue = input.Merge(value.sbyteListValue); break;
              case 29: value.shortListValue = input.Merge(value.shortListValue); break;
              case 30: value.ushortListValue = input.Merge(value.ushortListValue); break;
              case 31: value.intListValue = input.Merge(value.intListValue); break;
              case 32: value.uintListValue = input.Merge(value.uintListValue); break;
              case 33: value.longListValue = input.Merge(value.longListValue); break;
              case 34: value.ulongListValue = input.Merge(value.ulongListValue); break;
              case 35: value.floatListValue = input.Merge(value.floatListValue); break;
              case 36: value.doubleListValue = input.Merge(value.doubleListValue); break;
              case 37: value.stringListValue = input.Merge(value.stringListValue); break;
              case 38: value.bufferListValue = input.Merge(value.bufferListValue); break;
              case 39: value.byteArrayListValue = input.Merge(value.byteArrayListValue); break;
              case 40: value.boolValue = input.Merge(value.boolValue); break;
              case 41: value.boolNullValue = input.Merge(value.boolNullValue); break;
              default: input.SkipField(); break;
          }
      }
      input.ReadClassEnd(oldLimit);
      return value;
    }
    public void Write(ProtoWriter output, int fieldId, Soyo.Proto.ProtoTestBase value) {
      output.WriteClassBegin(fieldId);
      output.Write(1, value.byteValue);
      output.Write(2, value.sbyteValue);
      output.Write(3, value.shortValue);
      output.Write(4, value.ushortValue);
      output.Write(5, value.intValue);
      output.Write(6, value.uintValue);
      output.Write(7, value.longValue);
      output.Write(8, value.ulongValue);
      output.Write(9, value.floatValue);
      output.Write(10, value.doubleValue);
      output.Write(11, (int)value.enumValue);
      if (value.stringValue != null) output.Write(12, value.stringValue);
      output.Write(13, value.bufferValue);
      if (value.byteArrayValue != null) output.Write(14, value.byteArrayValue);
      if (value.byteNullValue != null) output.Write(15, value.byteNullValue);
      if (value.sbyteNullValue != null) output.Write(16, value.sbyteNullValue);
      if (value.shortNullValue != null) output.Write(17, value.shortNullValue);
      if (value.ushortNullValue != null) output.Write(18, value.ushortNullValue);
      if (value.intNullValue != null) output.Write(19, value.intNullValue);
      if (value.uintNullValue != null) output.Write(20, value.uintNullValue);
      if (value.longNullValue != null) output.Write(21, value.longNullValue);
      if (value.ulongNullValue != null) output.Write(22, value.ulongNullValue);
      if (value.floatNullValue != null) output.Write(23, value.floatNullValue);
      if (value.doubleNullValue != null) output.Write(24, value.doubleNullValue);
      if (value.bufferNullValue != null) output.Write(25, value.bufferNullValue);
      output.WriteArray(26, value.boolListValue);
      output.WriteArray(27, value.byteListValue);
      output.WriteArray(28, value.sbyteListValue);
      output.WriteArray(29, value.shortListValue);
      output.WriteArray(30, value.ushortListValue);
      output.WriteArray(31, value.intListValue);
      output.WriteArray(32, value.uintListValue);
      output.WriteArray(33, value.longListValue);
      output.WriteArray(34, value.ulongListValue);
      output.WriteArray(35, value.floatListValue);
      output.WriteArray(36, value.doubleListValue);
      output.WriteArray(37, value.stringListValue);
      output.WriteArray(38, value.bufferListValue);
      output.WriteArray(39, value.byteArrayListValue);
      output.Write(40, value.boolValue);
      if (value.boolNullValue != null) output.Write(41, value.boolNullValue);
      output.WriteClassEnd();
    }
    public void WriteDiff(ProtoWriter output, int fieldId, Soyo.Proto.ProtoTestBase value, Soyo.Proto.ProtoTestBase basevalue) {
      output.WriteClassBegin(fieldId);
      if (value.byteValue != basevalue.byteValue) output.Write(1, value.byteValue);
      if (value.sbyteValue != basevalue.sbyteValue) output.Write(2, value.sbyteValue);
      if (value.shortValue != basevalue.shortValue) output.Write(3, value.shortValue);
      if (value.ushortValue != basevalue.ushortValue) output.Write(4, value.ushortValue);
      if (value.intValue != basevalue.intValue) output.Write(5, value.intValue);
      if (value.uintValue != basevalue.uintValue) output.Write(6, value.uintValue);
      if (value.longValue != basevalue.longValue) output.Write(7, value.longValue);
      if (value.ulongValue != basevalue.ulongValue) output.Write(8, value.ulongValue);
      if (value.floatValue != basevalue.floatValue) output.Write(9, value.floatValue);
      if (value.doubleValue != basevalue.doubleValue) output.Write(10, value.doubleValue);
      output.Write(11, (int)value.enumValue);
      if (value.stringValue != basevalue.stringValue) output.Write(12, value.stringValue);
      if (value.bufferValue != basevalue.bufferValue) output.Write(13, value.bufferValue);
      if (value.byteArrayValue != basevalue.byteArrayValue) output.Write(14, value.byteArrayValue);
      if (value.byteNullValue != basevalue.byteNullValue) output.Write(15, value.byteNullValue);
      if (value.sbyteNullValue != basevalue.sbyteNullValue) output.Write(16, value.sbyteNullValue);
      if (value.shortNullValue != basevalue.shortNullValue) output.Write(17, value.shortNullValue);
      if (value.ushortNullValue != basevalue.ushortNullValue) output.Write(18, value.ushortNullValue);
      if (value.intNullValue != basevalue.intNullValue) output.Write(19, value.intNullValue);
      if (value.uintNullValue != basevalue.uintNullValue) output.Write(20, value.uintNullValue);
      if (value.longNullValue != basevalue.longNullValue) output.Write(21, value.longNullValue);
      if (value.ulongNullValue != basevalue.ulongNullValue) output.Write(22, value.ulongNullValue);
      if (value.floatNullValue != basevalue.floatNullValue) output.Write(23, value.floatNullValue);
      if (value.doubleNullValue != basevalue.doubleNullValue) output.Write(24, value.doubleNullValue);
      if (value.bufferNullValue != basevalue.bufferNullValue) output.Write(25, value.bufferNullValue);
      if (value.boolListValue != basevalue.boolListValue) output.Write(26, value.boolListValue);
      if (value.byteListValue != basevalue.byteListValue) output.Write(27, value.byteListValue);
      if (value.sbyteListValue != basevalue.sbyteListValue) output.Write(28, value.sbyteListValue);
      if (value.shortListValue != basevalue.shortListValue) output.Write(29, value.shortListValue);
      if (value.ushortListValue != basevalue.ushortListValue) output.Write(30, value.ushortListValue);
      if (value.intListValue != basevalue.intListValue) output.Write(31, value.intListValue);
      if (value.uintListValue != basevalue.uintListValue) output.Write(32, value.uintListValue);
      if (value.longListValue != basevalue.longListValue) output.Write(33, value.longListValue);
      if (value.ulongListValue != basevalue.ulongListValue) output.Write(34, value.ulongListValue);
      if (value.floatListValue != basevalue.floatListValue) output.Write(35, value.floatListValue);
      if (value.doubleListValue != basevalue.doubleListValue) output.Write(36, value.doubleListValue);
      if (value.stringListValue != basevalue.stringListValue) output.Write(37, value.stringListValue);
      if (value.bufferListValue != basevalue.bufferListValue) output.Write(38, value.bufferListValue);
      if (value.byteArrayListValue != basevalue.byteArrayListValue) output.Write(39, value.byteArrayListValue);
      if (value.boolValue != basevalue.boolValue) output.Write(40, value.boolValue);
      if (value.boolNullValue != basevalue.boolNullValue) output.Write(41, value.boolNullValue);
      output.WriteClassEnd();
    }
    public int Size(Soyo.Proto.ProtoTestBase value) {
      int size = 0;
      size += ProtoSerializer.Size(value.byteValue);
      size += ProtoSerializer.Size(value.sbyteValue);
      size += ProtoSerializer.Size(value.shortValue);
      size += ProtoSerializer.Size(value.ushortValue);
      size += ProtoSerializer.Size(value.intValue);
      size += ProtoSerializer.Size(value.uintValue);
      size += ProtoSerializer.Size(value.longValue);
      size += ProtoSerializer.Size(value.ulongValue);
      size += ProtoSerializer.Size(value.floatValue);
      size += ProtoSerializer.Size(value.doubleValue);
      size += ProtoSerializer.Size((int)value.enumValue);
      size += ProtoSerializer.Size(value.stringValue);
      size += ProtoSerializer.Size(value.bufferValue);
      size += ProtoSerializer.Size(value.byteArrayValue);
      size += ProtoSerializer.Size(value.byteNullValue);
      size += ProtoSerializer.Size(value.sbyteNullValue);
      size += ProtoSerializer.Size(value.shortNullValue);
      size += ProtoSerializer.Size(value.ushortNullValue);
      size += ProtoSerializer.Size(value.intNullValue);
      size += ProtoSerializer.Size(value.uintNullValue);
      size += ProtoSerializer.Size(value.longNullValue);
      size += ProtoSerializer.Size(value.ulongNullValue);
      size += ProtoSerializer.Size(value.floatNullValue);
      size += ProtoSerializer.Size(value.doubleNullValue);
      size += ProtoSerializer.Size(value.bufferNullValue);
      size += ProtoSerializer.ArraySize(value.boolListValue);
      size += ProtoSerializer.ArraySize(value.byteListValue);
      size += ProtoSerializer.ArraySize(value.sbyteListValue);
      size += ProtoSerializer.ArraySize(value.shortListValue);
      size += ProtoSerializer.ArraySize(value.ushortListValue);
      size += ProtoSerializer.ArraySize(value.intListValue);
      size += ProtoSerializer.ArraySize(value.uintListValue);
      size += ProtoSerializer.ArraySize(value.longListValue);
      size += ProtoSerializer.ArraySize(value.ulongListValue);
      size += ProtoSerializer.ArraySize(value.floatListValue);
      size += ProtoSerializer.ArraySize(value.doubleListValue);
      size += ProtoSerializer.ArraySize(value.stringListValue);
      size += ProtoSerializer.ArraySize(value.bufferListValue);
      size += ProtoSerializer.ArraySize(value.byteArrayListValue);
      size += ProtoSerializer.Size(value.boolValue);
      size += ProtoSerializer.Size(value.boolNullValue);
      return size;
    }
    public Soyo.Proto.ProtoTestBase DeepCopy(Soyo.Proto.ProtoTestBase dst, Soyo.Proto.ProtoTestBase src) {
      dst.byteValue = src.byteValue;
      dst.sbyteValue = src.sbyteValue;
      dst.shortValue = src.shortValue;
      dst.ushortValue = src.ushortValue;
      dst.intValue = src.intValue;
      dst.uintValue = src.uintValue;
      dst.longValue = src.longValue;
      dst.ulongValue = src.ulongValue;
      dst.floatValue = src.floatValue;
      dst.doubleValue = src.doubleValue;
      dst.enumValue = src.enumValue;
      dst.stringValue = src.stringValue;
      dst.bufferValue = src.bufferValue;
      dst.byteArrayValue = ProtoSerializer.DeepCopy(dst.byteArrayValue, src.byteArrayValue);
      dst.byteNullValue = src.byteNullValue;
      dst.sbyteNullValue = src.sbyteNullValue;
      dst.shortNullValue = src.shortNullValue;
      dst.ushortNullValue = src.ushortNullValue;
      dst.intNullValue = src.intNullValue;
      dst.uintNullValue = src.uintNullValue;
      dst.longNullValue = src.longNullValue;
      dst.ulongNullValue = src.ulongNullValue;
      dst.floatNullValue = src.floatNullValue;
      dst.doubleNullValue = src.doubleNullValue;
      dst.bufferNullValue = src.bufferNullValue;
      dst.boolListValue = ProtoSerializer.DeepCopy(dst.boolListValue, src.boolListValue);
      dst.byteListValue = ProtoSerializer.DeepCopy(dst.byteListValue, src.byteListValue);
      dst.sbyteListValue = ProtoSerializer.DeepCopy(dst.sbyteListValue, src.sbyteListValue);
      dst.shortListValue = ProtoSerializer.DeepCopy(dst.shortListValue, src.shortListValue);
      dst.ushortListValue = ProtoSerializer.DeepCopy(dst.ushortListValue, src.ushortListValue);
      dst.intListValue = ProtoSerializer.DeepCopy(dst.intListValue, src.intListValue);
      dst.uintListValue = ProtoSerializer.DeepCopy(dst.uintListValue, src.uintListValue);
      dst.longListValue = ProtoSerializer.DeepCopy(dst.longListValue, src.longListValue);
      dst.ulongListValue = ProtoSerializer.DeepCopy(dst.ulongListValue, src.ulongListValue);
      dst.floatListValue = ProtoSerializer.DeepCopy(dst.floatListValue, src.floatListValue);
      dst.doubleListValue = ProtoSerializer.DeepCopy(dst.doubleListValue, src.doubleListValue);
      dst.stringListValue = ProtoSerializer.DeepCopy(dst.stringListValue, src.stringListValue);
      dst.bufferListValue = ProtoSerializer.DeepCopy(dst.bufferListValue, src.bufferListValue);
      dst.byteArrayListValue = ProtoSerializer.DeepCopy(dst.byteArrayListValue, src.byteArrayListValue);
      dst.boolValue = src.boolValue;
      dst.boolNullValue = src.boolNullValue;
      return dst;
    }
  }
  
  class Soyo_Proto_ProtoTestClass_Serializer: IProtoSerializer<Soyo.Proto.ProtoTestClass> {
    public Soyo.Proto.ProtoTestClass Merge(ProtoReader input, Soyo.Proto.ProtoTestClass value) {
      int oldLimit = input.ReadClassBegin();
      while (input.HasField) {
          switch (input.FieldId) {
              case 1: value.id = input.Merge(value.id); break;
              case 2: value.items = input.Merge(value.items); break;
              default: input.SkipField(); break;
          }
      }
      input.ReadClassEnd(oldLimit);
      return value;
    }
    public void Write(ProtoWriter output, int fieldId, Soyo.Proto.ProtoTestClass value) {
      output.WriteClassBegin(fieldId);
      output.Write(1, value.id);
      output.WriteArray(2, value.items);
      output.WriteClassEnd();
    }
    public void WriteDiff(ProtoWriter output, int fieldId, Soyo.Proto.ProtoTestClass value, Soyo.Proto.ProtoTestClass basevalue) {
      output.WriteClassBegin(fieldId);
      if (value.id != basevalue.id) output.Write(1, value.id);
      if (value.items != basevalue.items) output.Write(2, value.items);
      output.WriteClassEnd();
    }
    public int Size(Soyo.Proto.ProtoTestClass value) {
      int size = 0;
      size += ProtoSerializer.Size(value.id);
      size += ProtoSerializer.ArraySize(value.items);
      return size;
    }
    public Soyo.Proto.ProtoTestClass DeepCopy(Soyo.Proto.ProtoTestClass dst, Soyo.Proto.ProtoTestClass src) {
      dst.id = src.id;
      dst.items = ProtoSerializer.DeepCopy(dst.items, src.items);
      return dst;
    }
  }
  
  public static class ProtoTest {
    public static void Initialize() {
      ProtoConvert.Register(new Soyo_Proto_ProtoTestBase_Serializer());
      ProtoConvert.Register(new Soyo_Proto_ProtoTestClass_Serializer());
    }
  }
}
