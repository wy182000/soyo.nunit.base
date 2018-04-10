using System.Text;
using System.Collections;

using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util.Collection {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class BitFieldTest {
    string toString(BitField bitField) {
      StringBuilder str = new StringBuilder();
      for (int i = 0; i < bitField.Length; i++) {
        if (bitField[i]) str.Append("true,");
        else str.Append("false,");
      }
      return str.ToString();
    }

    string toString(BitArray bitArray) {
      StringBuilder str = new StringBuilder();
      for (int i = 0; i < bitArray.Length; i++) {
        if (bitArray[i]) str.Append("true,");
        else str.Append("false,");
      }
      return str.ToString();
    }

    void check(BitField bitField, BitArray bitArray) {
      Assert.AreEqual(bitField.Length, bitArray.Length);
      for (int i = 0; i < bitField.Length; i++) {
        Assert.AreEqual(bitField[i], bitArray[i], "BitField: " + toString(bitField) + ", bitArray: " + toString(bitArray));
      }
    }

    [Test]
    public void Test() {
      var bitField = new BitField(0, false);
      Assert.AreEqual(bitField.Length, 0);
      Assert.AreEqual(bitField[1], false);

      int size = Rand.Default.Range(1, 256);
      bitField = new BitField(size, false);
      BitArray bitArray = new BitArray(size, false);
      check(bitField, bitArray);

      size = Rand.Default.Range(1, 256);
      bool[] boolData = new bool[size];
      for (int i = 0; i < boolData.Length; i++) {
        boolData[i] = Rand.Default.Range(0, 2) > 0;
      }
      bitField = new BitField(boolData);
      bitArray = new BitArray(boolData);
      check(bitField, bitArray);

      size = Rand.Default.Range(1, 64);
      byte[] byteData = new byte[size];
      for (int i = 0; i < byteData.Length; i++) {
        byteData[i] = Rand.Default.RandByte();
      }
      bitField = new BitField(byteData);
      bitArray = new BitArray(byteData);
      check(bitField, bitArray);

      size = Rand.Default.Range(1, 32);
      int[] intData = new int[size];
      for (int i = 0; i < intData.Length; i++) {
        intData[i] = Rand.Default.RandInt();
      }
      bitField = new BitField(intData);
      bitArray = new BitArray(intData);
      check(bitField, bitArray);

      size = Rand.Default.Range(1, 32);
      intData = new int[size];
      for (int i = 0; i < intData.Length; i++) {
        intData[i] = Rand.Default.RandInt();
      }
      bitArray = new BitArray(intData);
      bitField = new BitField(bitArray);
      check(bitField, bitArray);

      size = Rand.Default.Range(1, 32);
      intData = new int[size];
      for (int i = 0; i < intData.Length; i++) {
        intData[i] = Rand.Default.RandInt();
      }
      bitField = new BitField(intData);
      bitField = new BitField(bitField);
      bitArray = new BitArray(intData);
      check(bitField, bitArray);

      for (int i = 0; i < bitField.Length; i++) {
        bool value = Rand.Default.Range(0, 2) > 0;
        bitField.Set(i, value);
        bitArray.Set(i, value);
      }
      check(bitField, bitArray);

      Assert.IsTrue(bitField.Any());

      bitField.SetAll(false);
      Assert.IsFalse(bitField.Any());

      bitField.SetAll(true);
      bitArray.SetAll(true);
      check(bitField, bitArray);

      size = Rand.Default.Range(1, 32);
      intData = new int[size];
      for (int i = 0; i < intData.Length; i++) {
        intData[i] = Rand.Default.RandInt();
      }
      bitField = new BitField(intData);
      bitArray = new BitArray(intData);
      check(bitField, bitArray);

      bitField = bitField.Not();
      bitArray = bitArray.Not();
      check(bitField, bitArray);

      size = Rand.Default.Range(1, 32);
      intData = new int[size];
      for (int i = 0; i < intData.Length; i++) {
        intData[i] = Rand.Default.RandInt();
      }
      bitField = new BitField(intData);
      bitArray = new BitArray(intData);
      check(bitField, bitArray);

      intData = new int[size];
      for (int i = 0; i < intData.Length; i++) {
        intData[i] = Rand.Default.RandInt();
      }
      var bitFieldTwo = new BitField(intData);
      var bitArrayTwo = new BitArray(intData);
      check(bitFieldTwo, bitArrayTwo);

      bitField = bitField.Or(bitFieldTwo);
      bitArray = bitArray.Or(bitArrayTwo);
      check(bitField, bitArray);

      intData = new int[size];
      for (int i = 0; i < intData.Length; i++) {
        intData[i] = Rand.Default.RandInt();
      }
      bitFieldTwo = new BitField(intData);
      bitArrayTwo = new BitArray(intData);
      check(bitFieldTwo, bitArrayTwo);

      bitField = bitField.Xor(bitFieldTwo);
      bitArray = bitArray.Xor(bitArrayTwo);
      check(bitField, bitArray);

      intData = new int[size];
      for (int i = 0; i < intData.Length; i++) {
        intData[i] = Rand.Default.RandInt();
      }
      bitFieldTwo = new BitField(intData);
      bitArrayTwo = new BitArray(intData);
      check(bitFieldTwo, bitArrayTwo);

      bitField = bitField.And(bitFieldTwo);
      bitArray = bitArray.And(bitArrayTwo);
      check(bitField, bitArray);

      bitFieldTwo = bitField.Clone() as BitField;
      Assert.IsNotNull(bitFieldTwo);
      check(bitFieldTwo, bitArray);

      size = Rand.Default.Range(1, 256);
      boolData = new bool[size];
      for (int i = 0; i < boolData.Length; i++) {
        boolData[i] = Rand.Default.Range(0, 2) > 0;
      }

      bitField = new BitField(boolData);
      var boolDataTwo = new bool[size];
      bitField.CopyTo(boolDataTwo, 0);
      for (int i = 0; i < boolData.Length; i++) {
        Assert.AreEqual(boolData[i], boolDataTwo[i]);
      }

      size = Rand.Default.Range(1, 64);
      byteData = new byte[size];
      for (int i = 0; i < byteData.Length; i++) {
        byteData[i] = Rand.Default.RandByte();
      }

      bitField = new BitField(byteData);
      var byteDataTwo = new byte[size];
      bitField.CopyTo(byteDataTwo, 0);
      for (int i = 0; i < byteData.Length; i++) {
        Assert.AreEqual(byteData[i], byteDataTwo[i]);
      }

      size = Rand.Default.Range(1, 32);
      intData = new int[size];
      for (int i = 0; i < intData.Length; i++) {
        intData[i] = Rand.Default.RandInt();
      }

      bitField = new BitField(intData);
      var intDataTwo = new int[size];
      bitField.CopyTo(intDataTwo, 0);
      for (int i = 0; i < intData.Length; i++) {
        Assert.AreEqual(intData[i], intDataTwo[i]);
      }
    }
  }
}
