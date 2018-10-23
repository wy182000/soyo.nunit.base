using System;
using System.Collections.Generic;
using System.Text;

using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class BufferTest {
    private const int checkCount = 128; // must be a multiplier of 8 

    private static void checkBufferEmpty(ByteBuffer buffer) {
      Assert.IsTrue(buffer.IsEmpty);
      Assert.IsNull(buffer.Array);
      Assert.AreEqual(buffer.Offset, 0);
      Assert.AreEqual(buffer.Count, 0);
      Assert.IsTrue(buffer == ByteBuffer.Empty);
    }

    private static void checkBufferNotEmpty(ref ByteBuffer buffer, byte[] array, int offset, int size) {
      Assert.IsFalse(buffer.IsEmpty);
      Assert.IsNotNull(buffer.Array);
      if (array != null) Assert.AreEqual(buffer.Array, array);
      Assert.AreEqual(buffer.Offset, offset);
      Assert.AreEqual(buffer.Count, size);
      Assert.IsTrue(buffer != ByteBuffer.Empty);
    }

    [Test]
    public void TestByteBuffer() {
      checkBufferEmpty(ByteBuffer.Empty);

      var buffer = new ByteBuffer();
      checkBufferEmpty(buffer);

      buffer = new ByteBuffer(checkCount);
      checkBufferNotEmpty(ref buffer, null, 0, checkCount);

      var tempBuffer = buffer;

      var array = new byte[checkCount];

      buffer = new ByteBuffer(array);
      checkBufferNotEmpty(ref buffer, array, 0, checkCount);
      Assert.IsTrue(buffer != tempBuffer);

      tempBuffer = buffer;

      buffer = new ByteBuffer(array, 1, 3);
      checkBufferNotEmpty(ref buffer, array, 1, 3);
      Assert.IsTrue(buffer != tempBuffer);

      buffer = new ByteBuffer(new ArraySegment<byte>(array));
      checkBufferNotEmpty(ref buffer, array, 0, checkCount);
      Assert.IsTrue(buffer == tempBuffer);

      buffer = new ByteBuffer(new ByteBufferNode(array));
      checkBufferNotEmpty(ref buffer, array, 0, checkCount);
      Assert.IsTrue(buffer == tempBuffer);

      buffer = new ByteBuffer(new ByteBuffer(array));
      checkBufferNotEmpty(ref buffer, array, 0, checkCount);
      Assert.IsTrue(buffer == tempBuffer);

      buffer = new ByteBuffer(array);
      buffer = new ByteBuffer(ref buffer, 1);
      checkBufferNotEmpty(ref buffer, array, 1, checkCount - 1);
      Assert.IsTrue(buffer != tempBuffer);

      buffer = new ByteBuffer(checkCount);
      checkBufferNotEmpty(ref buffer, null, 0, checkCount);

      for (int i = 0; i < checkCount; i++) {
        buffer[i] = (byte)i;
      }

      for (int i = 0; i < checkCount; i++) {
        Assert.AreEqual(buffer[i], (byte)i);
      }

      int advanceOffset = 10;

      tempBuffer = ByteBuffer.AdvanceOffset(ref buffer, advanceOffset);
      checkBufferNotEmpty(ref tempBuffer, null, advanceOffset, checkCount - advanceOffset);
      Assert.IsTrue(tempBuffer.CheckPtr(0, ref buffer, advanceOffset));

      tempBuffer = ByteBuffer.AdvanceSize(ref buffer, advanceOffset);
      checkBufferNotEmpty(ref tempBuffer, null, 0, checkCount - advanceOffset);
      Assert.IsTrue(tempBuffer.CheckPtr(ref buffer));

      tempBuffer = ByteBuffer.Resize(ref buffer, checkCount - advanceOffset);
      checkBufferNotEmpty(ref tempBuffer, null, 0, checkCount - advanceOffset);
      Assert.IsTrue(tempBuffer.CheckPtr(ref buffer));

      tempBuffer = buffer;
      tempBuffer.AdvanceOffset(advanceOffset);
      checkBufferNotEmpty(ref tempBuffer, null, advanceOffset, checkCount - advanceOffset);
      Assert.IsTrue(tempBuffer.CheckPtr(0, ref buffer, advanceOffset));

      tempBuffer = buffer;
      tempBuffer.AdvanceSize(advanceOffset);
      checkBufferNotEmpty(ref tempBuffer, null, 0, checkCount - advanceOffset);
      Assert.IsTrue(tempBuffer.CheckPtr(ref buffer));

      tempBuffer = buffer;
      tempBuffer.Resize(checkCount - advanceOffset);
      checkBufferNotEmpty(ref tempBuffer, null, 0, checkCount - advanceOffset);
      Assert.IsTrue(tempBuffer.CheckPtr(ref buffer));

      tempBuffer = buffer;
      buffer = new ByteBuffer(checkCount);
      Assert.IsFalse(buffer.CheckPtr(ref tempBuffer));

      tempBuffer.CopyTo(ref buffer);
      for (int i = 0; i < checkCount; i++) {
        Assert.AreEqual(buffer[i], (byte)i);
      }

      tempBuffer.CopyTo(10, ref buffer, 20, 30);

      Assert.AreEqual(buffer[20 - 1], 20 - 1);
      for (int i = 0; i < 30; i++) {
        Assert.AreEqual(buffer[20 + i], (byte)(10 + i));
      }
      Assert.AreEqual(buffer[20 + 30], 20 + 30);

      buffer = new ByteBuffer(checkCount);

      for (int i = 0; i < checkCount; i++) {
        buffer.WriteByte(i, (byte)i);
      }

      for (int i = 0; i < checkCount; i++) {
        Assert.AreEqual(buffer.ReadByte(i), (byte)i);
      }

      var blockBuffer = new BlockBuffer();
      blockBuffer.Add(buffer);
      tempBuffer = blockBuffer.ToBuffer();
      checkBufferNotEmpty(ref tempBuffer, null, 0, buffer.Count);
      Assert.IsTrue(tempBuffer.CheckPtr(ref buffer));

      for (int i = 0; i < checkCount; i++) {
        Assert.AreEqual(tempBuffer.ReadByte(i), (byte)i);
      }

      buffer = new ByteBuffer(checkCount);
      long startValue = Byte.MaxValue * 2;
      int count = checkCount / 2;
      for (int i = 0; i < count; i++) {
        buffer.WriteUint16(i * 2, (ushort)(startValue + i));
      }

      for (int i = 0; i < count; i++) {
        Assert.AreEqual(buffer.ReadUint16(i * 2), (ushort)(startValue + i));
      }

      buffer = new ByteBuffer(checkCount);
      startValue = ushort.MaxValue * 2;
      count = checkCount / 4;
      for (int i = 0; i < count; i++) {
        buffer.WriteUint32(i * 4, (uint)(startValue + i));
      }

      for (int i = 0; i < count; i++) {
        Assert.AreEqual(buffer.ReadUint32(i * 4), (uint)(startValue + i));
      }

      buffer = new ByteBuffer(checkCount);
      startValue = (long)uint.MaxValue * 2;
      count = checkCount / 8;
      for (int i = 0; i < count; i++) {
        buffer.WriteUint64(i * 8, (uint)(startValue + i));
      }

      for (int i = 0; i < count; i++) {
        Assert.AreEqual(buffer.ReadUint64(i * 8), (uint)(startValue + i));
      }

      tempBuffer = buffer;

      buffer = new ByteBuffer(checkCount);
      for (int i = 0; i < count; i++) {
        buffer.WriteBuffer(i * 8, ref tempBuffer, i * 8, 8);
      }

      for (int i = 0; i < count; i++) {
        Assert.AreEqual(buffer.ReadUint64(i * 8), (uint)(startValue + i));
      }

      tempBuffer = buffer;
      buffer = new ByteBuffer(checkCount);

      for (int i = 0; i < count; i++) {
        tempBuffer.ReadBuffer(i * 8, ref buffer, i * 8, 8);
      }

      for (int i = 0; i < count; i++) {
        Assert.AreEqual(buffer.ReadUint64(i * 8), (uint)(startValue + i));
      }

      string checkString = "";
      for (int i = 0; i < checkCount / 2; i++) {
        checkString += i.ToString();
      }

      int stringSize = Encoding.UTF8.GetByteCount(checkString);
      Assert.IsTrue(stringSize > 0 && stringSize < checkCount);

      int index = Rand.Default.RandInt(checkCount - stringSize);
      buffer = new ByteBuffer(checkCount);
      buffer.WriteString(index, checkString);

      int readOffset;
      var value = buffer.ReadString(index, out readOffset);
      Assert.AreEqual(value, checkString);
      Assert.Less(stringSize, readOffset);

      buffer = new ByteBuffer(checkCount);
      count = checkCount / 4;
      float[] checkFloatSet = new float[count];
      for (int i = 0; i < count; i++) {
        checkFloatSet[i] = Rand.Default.RandFloat();
      }

      for (int i = 0; i < count; i++) {
        buffer.WriteFloat(i * 4, checkFloatSet[i]);
      }

      for (int i = 0; i < count; i++) {
        Assert.AreEqual(buffer.ReadFloat(i * 4), checkFloatSet[i]);
      }

      buffer = new ByteBuffer(checkCount);
      count = checkCount / 8;
      double[] checkDoubleSet = new double[count];
      for (int i = 0; i < count; i++) {
        checkDoubleSet[i] = Rand.Default.RandFloat();
      }

      for (int i = 0; i < count; i++) {
        buffer.WriteDouble(i * 8, checkDoubleSet[i]);
      }

      for (int i = 0; i < count; i++) {
        Assert.AreEqual(buffer.ReadDouble(i * 8), checkDoubleSet[i]);
      }

      buffer = new ByteBuffer(checkCount);
      count = checkCount / 5;
      uint[] checkUintSet = new uint[count];
      for (int i = 0; i < count; i++) {
        checkUintSet[i] = Rand.Default.RandUint();
      }

      int offset = 0;
      for (int i = 0; i < count; i++) {
        offset += buffer.WriteVarint32(offset, checkUintSet[i]);
      }

      offset = 0;
      for (int i = 0; i < count; i++) {
        int size;
        Assert.AreEqual(buffer.ReadVarint32(offset, out size), checkUintSet[i]);
        offset += size;
      }

      count = checkCount / 9;
      long[] checkLongSet = new long[count];
      for (int i = 0; i < count; i++) {
        checkLongSet[i] = Rand.Default.RandLong();
      }

      offset = 0;
      for (int i = 0; i < count; i++) {
        offset += buffer.WriteVarint64(offset, (ulong)checkLongSet[i]);
      }

      offset = 0;
      for (int i = 0; i < count; i++) {
        int size;
        Assert.AreEqual(buffer.ReadVarint32(offset, out size), (uint)checkLongSet[i]);
        offset += size;
      }

      offset = 0;
      for (int i = 0; i < count; i++) {
        int size;
        Assert.AreEqual(buffer.ReadVarint64(offset, out size), checkLongSet[i]);
        offset += size;
      }
    }

    private List<object> generateCheckDataByteBuffer() {
      var data = new List<object>();
      data.Add(Rand.Default.Range(0, 2) > 0);    // 0 bool
      data.Add(Rand.Default.RandByte());         // 1 byte
      data.Add((ushort)Rand.Default.RandShort());// 2 uint16
      data.Add(Rand.Default.RandUint());         // 3 uint32
      data.Add((ulong)Rand.Default.RandLong());  // 4 uint64
      data.Add((short)-Rand.Default.RandShort());// 5 int16
      data.Add(-Rand.Default.RandInt());         // 6 int32
      data.Add(-Rand.Default.RandLong());        // 7 int64
      data.Add(Rand.Default.RandUint());         // 8 varint32
      data.Add((ulong)Rand.Default.RandLong());  // 9 varint64
      data.Add(-Rand.Default.RandInt());         // 10 sign varint32
      data.Add(-Rand.Default.RandLong());        // 11 sign varint64
      data.Add(Rand.Default.RandFloat());        // 12 float
      data.Add((double)Rand.Default.RandFloat());// 13 double
      data.Add("please test me.");            // 14 string
      data.Add(new byte[] {
        Rand.Default.RandByte(),
        Rand.Default.RandByte(),
        Rand.Default.RandByte(),
        Rand.Default.RandByte(),
        Rand.Default.RandByte(),
        Rand.Default.RandByte()
      });                                     // 15 byte[]
      data.Add(Rand.Default.Range(0, 64));       // 16 skip
      return data;
    }

    private void writeDataByteBuffer(ByteBufferWriter writer, int index, object data) {
      Assert.IsNotNull(writer);
      Assert.IsNotNull(data);
      writer.WriteVarint32((uint)index);
      switch (index) {
        case 0:
          writer.WriteBool((bool)data);
          break;
        case 1:
          writer.WriteByte((byte)data);
          break;
        case 2:
          writer.WriteUint16((ushort)data);
          break;
        case 3:
          writer.WriteUint32((uint)data);
          break;
        case 4:
          writer.WriteUint64((ulong)data);
          break;
        case 5:
          writer.WriteInt16((short)data);
          break;
        case 6:
          writer.WriteInt32((int)data);
          break;
        case 7:
          writer.WriteInt64((long)data);
          break;
        case 8:
          writer.WriteVarint32((uint)data);
          break;
        case 9:
          writer.WriteVarint64((ulong)data);
          break;
        case 10:
          writer.WriteSVarint32((int)data);
          break;
        case 11:
          writer.WriteSVarint64((long)data);
          break;
        case 12:
          writer.WriteFloat((float)data);
          break;
        case 13:
          writer.WriteDouble((double)data);
          break;
        case 14:
          writer.WriteString((string)data);
          break;
        case 15: {
            var buffer = (byte[])data;
            writer.WriteVarint32((uint)buffer.Length);
            writer.WriteBuffer(buffer, 0, buffer.Length);
          }
          break;
        case 16: {
            writer.WriteVarint32((uint)(int)data);
            writer.Skip((int)data);
          }
          break;
        default:
          Assert.Fail();
          break;
      }
    }

    private int readDataByteBuffer(ByteBufferReader reader, List<object> data) {
      Assert.IsNotNull(reader);
      Assert.IsNotNull(data);
      int count = 0;
      while (reader.Position < reader.Buffer.Count) {
        int index = (int)reader.ReadVarint32();
        object value = null;
        switch (index) {
          case 0:
            value = reader.ReadBool();
            break;
          case 1:
            value = reader.ReadByte();
            break;
          case 2:
            value = reader.ReadUint16();
            break;
          case 3:
            value = reader.ReadUint32();
            break;
          case 4:
            value = reader.ReadUint64();
            break;
          case 5:
            value = reader.ReadInt16();
            break;
          case 6:
            value = reader.ReadInt32();
            break;
          case 7:
            value = reader.ReadInt64();
            break;
          case 8:
            value = reader.ReadVarint32();
            break;
          case 9:
            value = reader.ReadVarint64();
            break;
          case 10:
            value = reader.ReadSVarint32();
            break;
          case 11:
            value = reader.ReadSVarint64();
            break;
          case 12:
            value = reader.ReadFloat();
            break;
          case 13:
            value = reader.ReadDouble();
            break;
          case 14:
            value = reader.ReadString();
            break;
          case 15: {
              int length = (int)reader.ReadVarint32();
              value = new byte[length];
              reader.ReadBuffer((byte[])value, 0, length);
            }
            break;
          case 16: {
              int size = (int)reader.ReadVarint32();
              reader.Skip(size);
              value = size;
            }
            break;
          default:
            Assert.Fail();
            break;
        }

        Assert.AreEqual(value, data[index], "index: "+ index);
        count++;
      }

      return count;
    }

    [Test]
    public void TestByteBufferReaderWriter() {
      var buffer = new ByteBuffer(1000 * 1000);
      var writer = new ByteBufferWriter(ref buffer);
      int loop = 10000;
      var data = generateCheckDataByteBuffer();
      Assert.IsNotNull(data);
      for (int i = 0; i < loop; i++) {
        var index = Rand.Default.RandInt(data.Count);
        writeDataByteBuffer(writer, index, data[index]);
      }

      var reader = new ByteBufferReader(writer.UsedBuffer);
      var count = readDataByteBuffer(reader, data);
      Assert.AreEqual(count, loop);
    }

    [Test]
    public void TestBufferBlock() {
      var data = new byte[checkCount];
      for (int i = 0; i < data.Length; i++) {
        data[i] = Rand.Default.RandByte();
      }
      var block = new BlockBuffer();
      int offset = 0;
      Assert.IsTrue(block.IsEmpty);

      while (offset < data.Length) {
        int size = Rand.Default.RandInt(data.Length - offset + 1);
        var unit = new ByteBufferNode(data, offset, size);
        offset += size;
        block.Add(unit);
      }

      Assert.IsFalse(block.IsEmpty);
      Assert.AreEqual(block.Count, checkCount);

      var array = block.ToArray();
      Assert.IsNotNull(array);
      Assert.AreEqual(array.Length, checkCount);
      for (int i = 0; i < checkCount; i++) {
        Assert.AreEqual(data[i], array[i]);
      }

      for (int i = 0; i < checkCount; i++) {
        block[i] = 0;
      }

      for (int i = 0; i < checkCount; i++) {
        Assert.AreEqual(0, block[i]);
      }

      for (int i = 0; i < checkCount; i++) {
        block[i] = data[i];
      }

      for (int i = 0; i < checkCount; i++) {
        Assert.AreEqual(data[i], block[i]);
      }

      var tempBlock = block;

      block = (BlockBuffer)tempBlock.Clone();
      int checkOffset = 10;
      block.AdvanceOffset(checkOffset);
      Assert.AreEqual(block.Count, checkCount - checkOffset);

      array = block.ToArray();
      Assert.IsNotNull(array);
      Assert.AreEqual(array.Length, checkCount - checkOffset);
      for (int i = 0; i < checkCount - checkOffset; i++) {
        Assert.AreEqual(data[i + checkOffset], array[i]);
      }

      block = (BlockBuffer)tempBlock.Clone();
      block.AdvanceSize(checkOffset);
      Assert.AreEqual(block.Count, checkCount - checkOffset);

      array = block.ToArray();
      Assert.IsNotNull(array);
      Assert.AreEqual(array.Length, checkCount - checkOffset);
      for (int i = 0; i < checkCount - checkOffset; i++) {
        Assert.AreEqual(data[i], array[i]);
      }

      block = (BlockBuffer)tempBlock.Clone();
      block.Resize(checkCount - checkOffset);
      Assert.AreEqual(block.Count, checkCount - checkOffset);

      array = block.ToArray();
      Assert.IsNotNull(array);
      Assert.AreEqual(array.Length, checkCount - checkOffset);
      for (int i = 0; i < checkCount - checkOffset; i++) {
        Assert.AreEqual(data[i], array[i]);
      }

      block = tempBlock;

      for (int i = 0; i < checkCount; i++) {
        int index = Rand.Default.Range(1, checkCount - 1);
        int length = Rand.Default.Range(1, checkCount - index);
        var value = block.ToArray(index ,length);
        Assert.IsNotNull(value);
        Assert.AreEqual(value.Length, length);
        for (int j = 0; j < length; j++) {
          Assert.AreEqual(data[j + index], value[j]);
        }
      }

      block = (BlockBuffer)block.Clone();
      Assert.AreEqual(block.Count, checkCount);

      array = block.ToArray();
      Assert.IsNotNull(array);
      Assert.AreEqual(array.Length, checkCount);
      for (int i = 0; i < checkCount; i++) {
        Assert.AreEqual(data[i], array[i]);
      }
    }

    [Test]
    public void TestBufferPool() {
    }

    [Test]
    public void TestBufferStream() {
      var bufferPool = new BufferPoolSimple();
      var buff = new BlockBufferStream();
      int loops = 1024;
      int length = 0;

      // read write byte[]
      for (int i = 0; i < loops; i++) {
        int result = buff.WriteBuffer(BitConverter.GetBytes(i));
        Assert.AreEqual(4, result);
        length += result;
      }
      Assert.AreEqual(length, buff.Size);

      int pos1 = length / 2;

      // Seek
      Assert.AreEqual(buff.SeekSet(0), 0);
      Assert.AreEqual(buff.SeekSet(pos1), pos1);
      Assert.AreEqual(buff.SeekSet(length), length);
      Assert.AreEqual(buff.SeekSet(length + pos1), length);

      Assert.AreEqual(buff.SeekEnd(0), length);
      Assert.AreEqual(buff.SeekEnd(pos1), length - pos1);
      Assert.AreEqual(buff.SeekEnd(length), 0);
      Assert.AreEqual(buff.SeekEnd(length + pos1), 0);

      Assert.AreEqual(buff.SeekCur(0), 0);
      Assert.AreEqual(buff.SeekCur(pos1), pos1);
      Assert.AreEqual(buff.SeekCur(length + pos1), length);
      Assert.AreEqual(buff.SeekCur(-pos1), length - pos1);
      Assert.AreEqual(buff.SeekCur(-length), 0);

      byte[] data = new byte[4];
      // Read
      for (int i = 0; i < loops; i++) {
        int result = buff.ReadBuffer(data, 0, 4);
        Assert.AreEqual(4, result);
        Assert.AreEqual(BitConverter.ToInt32(data, 0), i);
        length += result;
      }


      // read write BufferUnit
      loops = 10;
      for (int l = 0; l < loops; l++) {
        var arr = new List<int>();
        buff.Clear();
        int size = checkCount;
        size = Rand.Default.RandInt(size);
        for (int i = 0; i < size; i++) {
          Assert.AreEqual(4, buff.WriteBuffer(BitConverter.GetBytes(i)));
          arr.Add(i);
        }

        int push_loops = checkCount;

        for (int i = 0; i < push_loops; i++) {
          var buffer = bufferPool.TakeBuffer(Rand.Default.RandInt(512) + checkCount);
          int push_size = buffer.Count / 4;
          push_size = Rand.Default.RandInt(push_size);
          if (push_size == 0) continue;
          int end = 0;
          for (int j = 0; j < push_size; j++) {
            int num = j + size;
            Buffer.BlockCopy(BitConverter.GetBytes(num), 0, buffer.Array, buffer.Offset + end, 4);
            end += 4;
            arr.Add(num);
          }
          var unit = new ByteBuffer(buffer.Array, buffer.Offset, end);

          Assert.AreEqual(push_size * 4, buff.WriteBuffer(unit));
          bufferPool.Return(buffer);
          size += push_size;
        }

        buff.SeekSet(0);
        byte[] value = new byte[4];
        foreach (var it in arr) {
          Assert.AreEqual(buff.ReadBuffer(value, 0, 4), 4);
          Assert.AreEqual(BitConverter.ToInt32(value, 0), it);
        }
      }

      // read write BufferBlock
      loops = 10;
      for (int l = 0; l < loops; l++) {
        buff.Clear();
        var arr = new List<int>();
        int size = checkCount;
        size = Rand.Default.RandInt(size);
        for (int i = 0; i < size; i++) {
          Assert.AreEqual(4, buff.WriteBuffer(BitConverter.GetBytes(i)));
          arr.Add(i);
        }

        int push_loops = checkCount;

        for (int i = 0; i < push_loops; i++) {
          int push_size = checkCount;
          push_size = Rand.Default.RandInt(push_size);
          var block = new BlockBuffer();
          var pushBuffer = new BlockBufferStream(block);
          for (int j = 0; j < push_size; j++) {
            int num = j + size;
            Assert.AreEqual(4, pushBuffer.WriteBuffer(BitConverter.GetBytes(num)));
            arr.Add(num);
          }

          block = new BlockBuffer();
          pushBuffer.SeekSet(0);
          Assert.AreEqual(pushBuffer.Size, pushBuffer.ReadBlock(block));

          Assert.AreEqual(push_size * 4, buff.WriteBlock(block, push_size * 4));
          size += push_size;
        }

        buff.SeekSet(0);
        byte[] value = new byte[4];
        foreach (var it in arr) {
          Assert.AreEqual(buff.ReadBuffer(value, 0, 4), 4);
          Assert.AreEqual(BitConverter.ToInt32(value, 0), it);
        }
      }
    }

    private List<object> generateCheckData() {
      var data = new List<object>();
      data.Add(Rand.Default.RandByte());         // 0  byte
      data.Add((ushort)Rand.Default.RandShort());// 1  uint16
      data.Add(Rand.Default.RandUint());         // 2  uint32
      data.Add((ulong)Rand.Default.RandLong());  // 3  uint64
      data.Add((short)-Rand.Default.RandShort());// 4  int16
      data.Add(-Rand.Default.RandInt());         // 5  int32
      data.Add(-Rand.Default.RandLong());        // 6  int64
      data.Add(Rand.Default.RandUint());         // 7  varint32
      data.Add((ulong)Rand.Default.RandLong());  // 8  varint64
      data.Add(-Rand.Default.RandInt());         // 9  sign varint32
      data.Add(-Rand.Default.RandLong());        // 10 sign varint64
      data.Add(Rand.Default.RandFloat());        // 11 float
      data.Add((double)Rand.Default.RandFloat());// 12 double
      data.Add(Rand.Default.Range(0, 2) > 0);    // 13 bool
      data.Add("please test me.");            // 14 string
      data.Add(new byte[] {
        Rand.Default.RandByte(),
        Rand.Default.RandByte(),
        Rand.Default.RandByte(),
        Rand.Default.RandByte(),
        Rand.Default.RandByte(),
        Rand.Default.RandByte()
      });                                     // 15 byte[]
      data.Add(Rand.Default.Range(0, 64));       // 16 skip
      return data;
    }

    private void writeData(BlockBufferWriter writer, int index, object data) {
      Assert.IsNotNull(writer);
      Assert.IsNotNull(data);
      writer.WriteVarint32((uint)index);
      switch (index) {
        case 0: writer.WriteByte((byte)data); break;
        case 1: writer.WriteUint16((ushort)data); break;
        case 2: writer.WriteUint32((uint)data); break;
        case 3: writer.WriteUint64((ulong)data); break;
        case 4: writer.WriteInt16((short)data); break;
        case 5: writer.WriteInt32((int)data); break;
        case 6: writer.WriteInt64((long)data); break;
        case 7: writer.WriteVarint32((uint)data); break;
        case 8: writer.WriteVarint64((ulong)data); break;
        case 9: writer.WriteSVarint32((int)data); break;
        case 10: writer.WriteSVarint64((long)data); break;
        case 11: writer.WriteFloat((float)data); break;
        case 12: writer.WriteDouble((double)data); break;
        case 13: writer.WriteBool((bool)data); break;
        case 14: writer.WriteString((string)data); break;
        case 15: {
            writer.WriteVarint32((uint)((byte[])data).Length);
            writer.WriteBuffer((byte[])data);
          }
          break;
        case 16: {
            writer.WriteVarint32((uint)(int)data);
            writer.Skip((int)data);
          }
          break;
        default: Assert.Fail(); break;
      }
    }

    private int readData(BlockBufferReader reader, List<object> data) {
      Assert.IsNotNull(reader);
      Assert.IsNotNull(data);
      int count = 0;
      while (reader.Position < reader.Limit) {
        int index = (int)reader.ReadVarint32();
        object value = null;
        switch (index) {
          case 0: value = reader.ReadByte(); break;
          case 1: value = reader.ReadUint16(); break;
          case 2: value = reader.ReadUint32(); break;
          case 3: value = reader.ReadUint64(); break;
          case 4: value = reader.ReadInt16(); break;
          case 5: value = reader.ReadInt32(); break;
          case 6: value = reader.ReadInt64(); break;
          case 7: value = reader.ReadVarint32(); break;
          case 8: value = reader.ReadVarint64(); break;
          case 9: value = reader.ReadSVarint32(); break;
          case 10: value = reader.ReadSVarint64(); break;
          case 11: value = reader.ReadFloat(); break;
          case 12: value = reader.ReadDouble(); break;
          case 13: value = reader.ReadBool(); break;
          case 14: value = reader.ReadString(); break;
          case 15: {
              int length = (int)reader.ReadVarint32();
              value = new byte[length];
              int ret = reader.ReadBuffer((byte[])value);
              Assert.AreEqual(ret, length);
            }
            break;
          case 16: {
              int size = (int)reader.ReadVarint32();
              value = reader.Skip(size);
            }
            break;
          default: Assert.Fail(); break;
        }

        Assert.AreEqual(value, data[index]);
        count++;
      }

      return count;
    }

    [Test]
    public void TestBufferReaderWriter() {
      var writer = new BlockBufferWriter();
      int loop = 10000;
      var data = generateCheckData();
      Assert.IsNotNull(data);
      for (int i = 0; i < loop; i++) {
        var index = Rand.Default.RandInt(data.Count);
        writeData(writer, index, data[index]);
      }

      var reader = new BlockBufferReader(writer.Block, writer.Position);
      var count = readData(reader, data);
      Assert.AreEqual(count, loop);

      // check read block
      var block = writer.Block;
      var size = writer.Position;
      writer = new BlockBufferWriter();
      writer.WriteBlock(block, size);
      var blockWrite = writer.ToBlock();
      reader = new BlockBufferReader(blockWrite);
      Assert.AreEqual(reader.Limit, size);
      var checkBlock = new BlockBuffer();
      Assert.AreEqual(reader.ReadBlock(checkBlock), size);
      Assert.IsTrue(reader.IsEnd);
      var array = block.ToArray();
      var arrayCheck = checkBlock.ToArray();
      for (int i = 0; i < size; i++) {
        Assert.AreEqual(array[i], arrayCheck[i]);
      }
    }
  }
}
