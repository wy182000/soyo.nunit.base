using System;
using System.Threading;

using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util.Collection {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class CircularBufferTest {
    private static void checkBuffer(CircularBuffer<int> buffer, int capacity, int count, int startValue, int endValue) {
      var array = buffer.ToArray();

      Assert.AreEqual(buffer.Capacity, capacity, "capacity must be " + capacity);
      Assert.AreEqual(buffer.Count, count, "Count must be " + count);
      if (count == 0) {
        Assert.IsTrue(buffer.Empty, "Empty must be true");
      } else {
        Assert.IsFalse(buffer.Empty, "Empty must be false");
      }
      if (count == capacity) {
        Assert.IsTrue(buffer.Full, "Full must be true");
      } else {
        Assert.IsFalse(buffer.Full, "Full must be false");
      }
      Assert.AreEqual(buffer.Front(), startValue, "Front must be " + startValue); 
      Assert.AreEqual(buffer.Back(), endValue, "Back must be " + endValue);

      int value = startValue;
      foreach (var i in buffer) {
        Assert.AreEqual(i, value, "buffer element must be " + value);
        value++;
      }

      if (buffer.Empty) {
        Assert.IsNull(array, "ToArray must be null");
      } else {
        Assert.AreEqual(array.Length, count, "array length must be " + count);
        for (int i = 0; i < array.Length; i++) {
          Assert.AreEqual(array[i], buffer[i], "array item must be equal as buffer item, index: " + i);
        }
      }
    }

    private static void checkBuffer(CircularBuffer buffer, int capacity, int count, object startValue, object endValue) {
      var array = buffer.ToArray();

      Assert.AreEqual(buffer.Capacity, capacity, "capacity must be " + capacity);
      Assert.AreEqual(buffer.Count, count, "Count must be " + count);
      if (count == 0) {
        Assert.IsTrue(buffer.Empty, "Empty must be true");
      } else {
        Assert.IsFalse(buffer.Empty, "Empty must be false");
      }
      if (count == capacity) {
        Assert.IsTrue(buffer.Full, "Full must be true");
      } else {
        Assert.IsFalse(buffer.Full, "Full must be false");
      }
      Assert.AreEqual(buffer.Front(), startValue, "Front must be " + startValue);
      Assert.AreEqual(buffer.Back(), endValue, "Back must be " + endValue);

      int value = startValue != null? (Int32)startValue: 0;
      foreach (var i in buffer) {
        Assert.AreEqual(i, value, "buffer element must be " + value);
        value++;
      }

      if (buffer.Empty) {
        Assert.IsNull(array, "ToArray must be null");
      } else {
        Assert.AreEqual(array.Length, count, "array length must be " + count);
        for (int i = 0; i < array.Length; i++) {
          Assert.AreEqual(array[i], buffer[i], "array item must be equal as buffer item, index: " + i);
        }
      }
    }

    [Test]
    public void TestCircularBuffer_T() {
      CircularBuffer<int> buffer = new CircularBuffer<int>();

      int capacity = CircularBuffer<int>.DefaultCapacity;
      int count = 0;
      int startValue = default(int);
      int endValue = default(int);
      checkBuffer(buffer, capacity, count, startValue, endValue);

      int halfSize = capacity / 2;
      count = halfSize;
      startValue = halfSize + 1;
      for (int i = 0; i < count; i++) {
        buffer.PushBack(startValue + i);
      }
      endValue = startValue + halfSize - 1;
      checkBuffer(buffer, capacity, count, startValue, endValue);

      for (int i = count; i > 0; i--) {
        buffer.PushFront(i);
      }
      startValue = 1;
      endValue = buffer.Capacity;
      count += count;
      checkBuffer(buffer, capacity, count, startValue, endValue);

      int popValue = buffer.PopFront();
      Assert.AreEqual(popValue, startValue, "popValue must be " + startValue);
      startValue++;
      count--;
      checkBuffer(buffer, capacity, count, startValue, endValue);

      popValue = buffer.PopBack();
      Assert.AreEqual(popValue, endValue, "popValue must be " + endValue);
      endValue--;
      count--;
      checkBuffer(buffer, capacity, count, startValue, endValue);

      endValue++;
      var discardValue = buffer.PushBack(endValue);
      count++;
      Assert.AreEqual(discardValue, default(int), "discardValue must be " + default(int));
      checkBuffer(buffer, capacity, count, startValue, endValue);

      startValue--;
      discardValue = buffer.PushFront(startValue);
      count++;
      Assert.AreEqual(discardValue, default(int), "discardValue must be " + default(int));
      checkBuffer(buffer, capacity, count, startValue, endValue);

      endValue++;
      discardValue = buffer.PushBack(endValue);
      Assert.AreEqual(discardValue, startValue, "discardValue must be " + startValue);
      startValue++;
      checkBuffer(buffer, capacity, count, startValue, endValue);

      var array = buffer.PopAll();
      var value = startValue;
      Assert.AreEqual(array.Length, count, "array length must be " + count);
      for (int i = 0; i < array.Length; i++) {
        Assert.AreEqual(array[i], startValue, "array item must be " + startValue);
        startValue++;
      }
      count = 0;
      startValue = default(int);
      endValue = default(int);
      checkBuffer(buffer, capacity, count, startValue, endValue);

      count = capacity;
      for (int i = 1; i <= count; i++) {
        buffer.PushBack(i);
      }
      startValue = 1;
      endValue = count;
      checkBuffer(buffer, capacity, count, startValue, endValue);

      int incValue = 8;
      for (int i = 0; i < count; i++) {
        buffer[i] += incValue;
      }
      startValue += incValue;
      endValue += incValue;
      checkBuffer(buffer, capacity, count, startValue, endValue);

      capacity *= 2;
      var discardArray = buffer.Resize(capacity);
      Assert.IsNull(discardArray, "discardArray must be null");
      checkBuffer(buffer, capacity, count, startValue, endValue);

      capacity /= 2;
      discardArray = buffer.Resize(capacity);
      Assert.IsNull(discardArray, "discardArray must be null");
      checkBuffer(buffer, capacity, count, startValue, endValue);

      capacity /= 2;
      discardArray = buffer.Resize(capacity);
      count = capacity;
      endValue -= capacity;
      Assert.AreEqual(discardArray.Length, count, "discardArray length must be " + count);
      value = endValue + 1;
      for (int i = 0; i < count; i++) {
        Assert.AreEqual(discardArray[i], value, "array item must be equal as buffer item, index: " + i);
        value++;
      }
      checkBuffer(buffer, capacity, count, startValue, endValue);

      buffer.Clear();
      count = 0;
      startValue = default(int);
      endValue = default(int);
      checkBuffer(buffer, capacity, count, startValue, endValue);
    }

    [Test]
    public void TestCircularBuffer() {
      CircularBuffer buffer = new CircularBuffer();

      int capacity = CircularBuffer.DefaultCapacity;
      int count = 0;
      Int32 startValue = default(Int32);
      Int32 endValue = default(Int32);
      checkBuffer(buffer, capacity, count, null, null);

      int halfSize = capacity / 2;
      count = halfSize;
      startValue = halfSize + 1;
      for (int i = 0; i < count; i++) {
        buffer.PushBack(startValue + i);
      }
      endValue = startValue + halfSize - 1;
      checkBuffer(buffer, capacity, count, startValue, endValue);

      for (int i = count; i > 0; i--) {
        buffer.PushFront(i);
      }
      startValue = 1;
      endValue = buffer.Capacity;
      count += count;
      checkBuffer(buffer, capacity, count, startValue, endValue);

      Int32 popValue = (Int32)buffer.PopFront();
      Assert.AreEqual(popValue, startValue, "popValue must be " + startValue);
      startValue++;
      count--;
      checkBuffer(buffer, capacity, count, startValue, endValue);

      popValue = (Int32)buffer.PopBack();
      Assert.AreEqual(popValue, endValue, "popValue must be " + endValue);
      endValue--;
      count--;
      checkBuffer(buffer, capacity, count, startValue, endValue);

      endValue++;
      var discardValue = buffer.PushBack(endValue);
      count++;
      Assert.AreEqual(discardValue, null, "discardValue must be null");
      checkBuffer(buffer, capacity, count, startValue, endValue);

      startValue--;
      discardValue = buffer.PushFront(startValue);
      count++;
      Assert.AreEqual(discardValue, null, "discardValue must be null");
      checkBuffer(buffer, capacity, count, startValue, endValue);

      endValue++;
      discardValue = buffer.PushBack(endValue);
      Assert.AreEqual(discardValue, startValue, "discardValue must be " + startValue);
      startValue++;
      checkBuffer(buffer, capacity, count, startValue, endValue);

      discardValue = buffer.PushBack(null);
      Assert.IsNull(discardValue, "discardValue must be null");

      discardValue = buffer.PushFront(null);
      Assert.IsNull(discardValue, "discardValue must be null");

      var array = buffer.PopAll();
      var value = startValue;
      Assert.AreEqual(array.Length, count, "array length must be " + count);
      for (int i = 0; i < array.Length; i++) {
        Assert.AreEqual(array[i], startValue, "array item must be " + startValue);
        startValue++;
      }
      count = 0;
      checkBuffer(buffer, capacity, count, null, null);

      count = capacity;
      for (int i = 1; i <= count; i++) {
        buffer.PushBack(i);
      }
      startValue = 1;
      endValue = count;
      checkBuffer(buffer, capacity, count, startValue, endValue);

      Int32 incValue = 8;
      for (int i = 0; i < count; i++) {
        buffer[i] = (Int32)buffer[i] + incValue;
      }
      startValue += incValue;
      endValue += incValue;
      checkBuffer(buffer, capacity, count, startValue, endValue);

      capacity *= 2;
      var discardArray = buffer.Resize(capacity);
      Assert.IsNull(discardArray, "discardArray must be null");
      checkBuffer(buffer, capacity, count, startValue, endValue);

      capacity /= 2;
      discardArray = buffer.Resize(capacity);
      Assert.IsNull(discardArray, "discardArray must be null");
      checkBuffer(buffer, capacity, count, startValue, endValue);

      capacity /= 2;
      discardArray = buffer.Resize(capacity);
      count = capacity;
      endValue -= capacity;
      Assert.AreEqual(discardArray.Length, count, "discardArray length must be " + count);
      value = endValue + 1;
      for (int i = 0; i < count; i++) {
        Assert.AreEqual(discardArray[i], value, "array item must be equal as buffer item, index: " + i);
        value++;
      }
      checkBuffer(buffer, capacity, count, startValue, endValue);

      buffer.Clear();
      count = 0;
      checkBuffer(buffer, capacity, count, null, null);
    }
  }
}
