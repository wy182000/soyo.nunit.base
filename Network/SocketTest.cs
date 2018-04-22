using System;
using System.Collections.Generic;

using Soyo.Base;
using Soyo.Base.Network;

using NUnit.Framework;

namespace UnitTest.Base.Network {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class SocketTest {
    private const int checkCount = 128;
    private const int serverCount = 10;
    private const int clientCount = 10;
    private int port = 9500;

    [OneTimeSetUp]
    public void Init() {
      Thread.Initialize();
    }

    [OneTimeTearDown]
    public void Terminate() {
      Thread.Terminate();
    }

    private List<object> generateCheckData() {
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
      return data;
    }

    private void writeMessage(IBufferWriter writer, int index, object data) {
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
        default:
          Assert.Fail();
          break;
      }
    }

    private int checkMessage(ref Message msg, List<object> data) {
      Assert.IsFalse(msg.IsEmpty);
      Assert.IsNotNull(data);

      var reader = msg.BufferReader;
      Assert.IsNotNull(reader);
      int count = (int)reader.ReadVarint32();
      Assert.Less(count, 128);

      int i = 0;
      while (reader.IsEnd == false) {
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
          default:
            Assert.Fail();
            break;
        }

        Assert.AreEqual(value, data[index], "index: " + index);
        i++;
      }

      Assert.AreEqual(count, i);
      return count;
    }

    private Message generateMessage(List<object> data) {
      Assert.IsNotNull(data);
      Message msg = new Message(8192);
      int count = Rand.Default.RandInt(128);
      var writer = msg.BufferWriter;
      writer.WriteVarint32((uint)count);
      for (int i = 0; i < count; i++) {
        int index = Rand.Default.RandInt(data.Count);
        writeMessage(writer, index, data[index]);
      }
      msg.Resize(writer.Position);
      return msg;
    }

    [Test]
    public void TestSocketPair() {
      var server = SocketBase.Create(SocketType.Pair);
      bool rc = server.Bind("127.0.0.1:" + port);
      Assert.IsTrue(rc);

      Thread.Sleep(10);

      var client = SocketBase.Create(SocketType.Pair);
      rc = client.Connect("127.0.0.1:" + port);
      Assert.IsTrue(rc);

      port++;

      Message empty = new Message(0);
      Assert.IsTrue(empty.IsBuffer);
      Assert.AreEqual(empty.Count, 0);

      rc = Thread.Wait(() => server.ConnectionCount == 1, -1);
      Assert.IsTrue(rc);

      rc = server.TrySend(ref empty, 1000);
      Assert.IsTrue(rc);
      Assert.IsTrue(empty.IsBuffer);
      Assert.AreEqual(empty.Count, 0);

      Message emptyRecv;
      rc = client.TryRecv(out emptyRecv, 1000);
      Assert.IsTrue(rc);
      Assert.IsTrue(emptyRecv.IsBuffer);
      Assert.AreEqual(emptyRecv.Count, 0);

      var checkData = generateCheckData();

      for (int i = 0; i < checkCount; i++) {
        var msg = generateMessage(checkData);
        rc = server.TrySend(ref msg, 1000);
        Assert.IsTrue(rc);
      }

      for (int i = 0; i < checkCount; i++) {
        var msg = generateMessage(checkData);
        rc = client.TrySend(ref msg, 1000);
        Assert.IsTrue(rc);
      }

      int result = 0;

      for (int i = 0; i < checkCount; i++) {
        Message msg;
        rc = client.TryRecv(out msg, 1000);
        Assert.IsTrue(rc);
        checkMessage(ref msg, checkData);
        result++;
      }
      Assert.AreEqual(result, checkCount);

      result = 0;
      for (int i = 0; i < checkCount; i++) {
        Message msg;
        rc = server.TryRecv(out msg, 1000);
        Assert.IsTrue(rc);
        checkMessage(ref msg, checkData);
        result++;
      }
      Assert.AreEqual(result, checkCount);

      Assert.AreEqual(result, checkCount);

      client.Close();
      rc = client.WaitClose(1000);
      Assert.IsTrue(rc);

      server.Close();
      rc = server.WaitClose(1000);
      Assert.IsTrue(rc);
    }

    [Test]
    public void TestSocketPairThread() {
      var server = SocketBase.Create(SocketType.Pair);
      bool rc = server.Bind("127.0.0.1:" + port);
      Assert.IsTrue(rc);

      var client = SocketBase.Create(SocketType.Pair);
      rc = client.Connect("127.0.0.1:" + port);
      Assert.IsTrue(rc);

      port++;

      var checkData = generateCheckData();

      rc = Thread.Wait(() => server.ConnectionCount == 1, 1000);
      Assert.IsTrue(rc);

      var threadSend = Thread.CreateThread("SocketSend");
      var threadRecv = Thread.CreateThread("SocketRecv");

      threadSend.Post(() => {
        for (int i = 0; i < checkCount; i++) {
          var msg = generateMessage(checkData);
          rc = server.TrySend(ref msg, 1000);
          Assert.IsTrue(rc);
        }
        threadSend.Stop();
      });

      int result = 0;
      threadRecv.Send(() => {
        for (int i = 0; i < checkCount; i++) {
          Message msg;
          rc = client.TryRecv(out msg, 1000);
          Assert.IsTrue(rc);
          checkMessage(ref msg, checkData);
          result++;
        }
        threadRecv.Stop();
      });
      
      client.Close();
      rc = client.WaitClose(1000);
      Assert.IsTrue(rc);

      server.Close();
      rc = server.WaitClose(1000);
      Assert.IsTrue(rc);

      Assert.AreEqual(result, checkCount);
    }

    [Test]
    public void TestSocketPushPull() {
      var server = SocketBase.Create(SocketType.Pull);
      bool rc = server.Bind("127.0.0.1:" + port);
      Assert.IsTrue(rc);

      List<SocketBase> clientSet = new List<SocketBase>();
      for (int i = 0; i < clientCount; i++) {
        var client = SocketBase.Create(SocketType.Push);
        rc = client.Connect("127.0.0.1:" + port);
        Assert.IsTrue(rc);
        clientSet.Add(client);
      }

      port++;

      var checkData = generateCheckData();

      for (int c = 0; c < clientCount; c++) {
        for (int i = 0; i < checkCount; i++) {
          var msg = generateMessage(checkData);
          rc = clientSet[c].TrySend(ref msg, 1000);
          Assert.IsTrue(rc, "client id: " + c + ", message id: " + i);
        }
      }

      for (int i = 0; i < clientCount * checkCount; i++) {
        Message msg;
        rc = server.TryRecv(out msg, 1000);
        Assert.IsTrue(rc);
        checkMessage(ref msg, checkData);
      }

      for (int i = 0; i < clientSet.Count; i++) {
        clientSet[i].Close();
        rc = clientSet[i].WaitClose(1000);
        Assert.IsTrue(rc);
      }

      server.Close();
      rc = server.WaitClose(1000);
      Assert.IsTrue(rc);
    }

    [Test]
    public void TestSocketPushPullThread() {
      var server = SocketBase.Create(SocketType.Pull);
      bool rc = server.Bind("127.0.0.1:" + port);
      Assert.IsTrue(rc);

      var checkData = generateCheckData();
      List<SocketBase> clientSet = new List<SocketBase>();
      for (int i = 0; i < clientCount; i++) {
        var thread = Thread.CreateThread("SocketPush: " + i);
        var client = SocketBase.Create(SocketType.Push);
        rc = client.Connect("127.0.0.1:" + port);
        clientSet.Add(client);
        Assert.IsTrue(rc);
        thread.Post(() => {
          for (int m = 0; m < checkCount; m++) {
            var msg = generateMessage(checkData);
            rc = client.TrySend(ref msg, 1000);
            Assert.IsTrue(rc, "client id: " + i + ", message id: " + m);
          }
          thread.Stop();
        });
      }

      port++;

      int result = 0;
      var threadPull = Thread.CreateThread("SocketPull");
      threadPull.Send(() => {
        for (int i = 0; i < clientCount * checkCount; i++) {
          Message msg;
          rc = server.TryRecv(out msg, 1000);
          Assert.IsTrue(rc);
          checkMessage(ref msg, checkData);
          result++;
        }
        threadPull.Stop();
      });

      Assert.AreEqual(result, clientCount * checkCount);

      for (int i = 0; i < clientSet.Count; i++) {
        clientSet[i].Close();
        rc = clientSet[i].WaitClose(1000);
        Assert.IsTrue(rc);
      }

      server.Close();
      rc = server.WaitClose(1000);
      Assert.IsTrue(rc);
    }

    [Test]
    public void TestSocketPubSub() {
      var pub = SocketBase.Create(SocketType.Pub);
      bool rc = pub.Bind("127.0.0.1:" + port);
      Assert.IsTrue(rc);

      List<SocketBase> subSet = new List<SocketBase>();
      for (int i = 0; i < clientCount; i++) {
        var sub = SocketBase.Create(SocketType.Sub);
        rc = sub.Connect("127.0.0.1:" + port);
        Assert.IsTrue(rc);
        subSet.Add(sub);
      }

      port++;

      var checkData = generateCheckData();

      rc = Thread.Wait(() => pub.ConnectionCount == clientCount, 1000);
      Assert.IsTrue(rc);

      for (int i = 0; i < checkCount; i++) {
        var msg = generateMessage(checkData);
        rc = pub.TrySend(ref msg, 1000);
        Assert.IsTrue(rc);
      }

      for (int c = 0; c < clientCount; c++) {
        for (int i = 0; i < checkCount; i++) {
          Message msg;
          rc = subSet[c].TryRecv(out msg, 1000);
          Assert.IsTrue(rc, "client id: " + c + ", message id: " + i);
          checkMessage(ref msg, checkData);
        }
      }

      for (int i = 0; i < subSet.Count; i++) {
        subSet[i].Close();
        rc = subSet[i].WaitClose(1000);
        Assert.IsTrue(rc);
      }

      pub.Close();
      rc = pub.WaitClose(1000);
      Assert.IsTrue(rc);
    }

    [Test]
    public void TestSocketPubSubThread() {
      var pub = SocketBase.Create(SocketType.Pub);
      bool rc = pub.Bind("127.0.0.1:" + port);
      Assert.IsTrue(rc);

      List<SocketBase> subSet = new List<SocketBase>();
      for (int i = 0; i < clientCount; i++) {
        var sub = SocketBase.Create(SocketType.Sub);
        rc = sub.Connect("127.0.0.1:" + port);
        Assert.IsTrue(rc);
        subSet.Add(sub);
      }

      port++;

      var checkData = generateCheckData();

      var threadPool = new ThreadPool(clientCount);

      threadPool.Post(() => {
        rc = Thread.Wait(() => pub.ConnectionCount == clientCount, 1000);
        Assert.IsTrue(rc);
        for (int i = 0; i < checkCount; i++) {
          var msg = generateMessage(checkData);
          rc = pub.TrySend(ref msg, 1000);
          Assert.IsTrue(rc);
        }
      });

      int result = 0;
      for (int c = 0; c < clientCount; c++) {
        var sub = subSet[c];
        threadPool.Post(() => {
          rc = Thread.Wait(() => sub.ConnectionCount == 1, 1000);
          Assert.IsTrue(rc);
          for (int i = 0; i < checkCount; i++) {
            Message msg;
            rc = sub.TryRecv(out msg, 1000);
            Assert.IsTrue(rc);
            checkMessage(ref msg, checkData);
            Atomic.Inc(ref result);
          }
        });
      }

      Thread.Wait(() => result == clientCount * checkCount, -1);

      threadPool.Dispose();
      Assert.AreEqual(result, clientCount * checkCount);

      for (int i = 0; i < subSet.Count; i++) {
        subSet[i].Close();
        rc = subSet[i].WaitClose(1000);
        Assert.IsTrue(rc);
      }

      pub.Close();
      rc = pub.WaitClose(1000);
      Assert.IsTrue(rc);
    }

    [Test]
    public void TestSocketPubSubMultiple() {
      List<SocketBase> pubSet = new List<SocketBase>();
      for (int i = 0; i < serverCount; i++) {
        var pub = SocketBase.Create(SocketType.Pub);
        bool rc = pub.Bind("127.0.0.1:" + (port + i));
        Assert.IsTrue(rc);
        pubSet.Add(pub);
      }

      List<SocketBase> subSet = new List<SocketBase>();
      for (int i = 0; i < clientCount; i++) {
        var sub = SocketBase.Create(SocketType.Sub);
        for (int p = 0; p < serverCount; p++) {
          bool rc = sub.Connect("127.0.0.1:" + (port + p));
          Assert.IsTrue(rc);
        }
        subSet.Add(sub);
      }

      port += serverCount;

      var checkData = generateCheckData();

      var threadPool = new ThreadPool(clientCount);

      for (int p = 0; p < serverCount; p++) {
        var pub = pubSet[p];
        threadPool.Post(() => {
          bool rc = Thread.Wait(() => pub.ConnectionCount == clientCount, 1000);
          Assert.IsTrue(rc);
          for (int i = 0; i < checkCount; i++) {
            var msg = generateMessage(checkData);
            rc = pub.TrySend(ref msg, 1000);
            Assert.IsTrue(rc);
          }
        });
      }

      int result = 0;
      for (int s = 0; s < clientCount; s++) {
        var sub = subSet[s];
        var subId = s;
        threadPool.Post(() => {
          bool rc = Thread.Wait(() => sub.ConnectionCount == serverCount, 1000);
          Assert.IsTrue(rc);
          for (int i = 0; i < checkCount * serverCount; i++) {
            Message msg;
            rc = sub.TryRecv(out msg, 1000);
            Assert.IsTrue(rc, "message id: " + i + ", sub id: " + subId);
            checkMessage(ref msg, checkData);
            Atomic.Inc(ref result);
          }
        });
      }

      Thread.Wait(() => result == clientCount * checkCount * serverCount, -1);

      threadPool.Dispose();
      Assert.AreEqual(result, clientCount * checkCount * serverCount);

      for (int i = 0; i < subSet.Count; i++) {
        subSet[i].Close();
        bool rc = subSet[i].WaitClose(1000);
        Assert.IsTrue(rc);
      }

      for (int i = 0; i < pubSet.Count; i++) {
        pubSet[i].Close();
        bool rc = pubSet[i].WaitClose(1000);
        Assert.IsTrue(rc);
      }
    }

    [Test]
    public void TestSocketServerClient() {
      var server = SocketBase.Create<SocketServer>();
      bool rc = server.Bind("127.0.0.1:" + port);
      Assert.IsTrue(rc);

      List<SocketBase> clientSet = new List<SocketBase>();
      for (int i = 0; i < clientCount; i++) {
        var client = SocketBase.Create(SocketType.Client);
        rc = client.Connect("127.0.0.1:" + port);
        Assert.IsTrue(rc);
        clientSet.Add(client);
      }

      port++;

      var checkData = generateCheckData();

      rc = Thread.Wait(() => server.ConnectionCount == clientCount, 1000);
      Assert.IsTrue(rc);

      string hello = "Hello";
      var helloMessage = new Message(16);
      var writer = helloMessage.BufferWriter;
      writer.WriteString(hello);
      helloMessage.Resize(writer.Position);

      for (int i = 0; i < clientCount; i++) {
        rc = clientSet[i].TrySend(ref helloMessage, 1000);
        Assert.IsTrue(rc);
      }

      List<uint> clientIdSet = new List<uint>(clientCount);
      for (int i = 0; i < clientCount; i++) {
        Message msg;
        rc = server.TryRecv(out msg, 1000);
        Assert.IsTrue(rc);
        Assert.Less(0, msg.RouteId);
        Assert.IsFalse(clientIdSet.Contains(msg.RouteId));
        clientIdSet.Add(msg.RouteId);
        var reader = msg.BufferReader;
        var value = reader.ReadString();
        Assert.AreEqual(value, hello);
      }

      for (int c = 0; c < clientCount; c++) {
        for (int i = 0; i < checkCount; i++) {
          var msg = generateMessage(checkData);
          rc = server.Send(clientIdSet[c], ref msg);
          Assert.IsTrue(rc, "client id: " + clientIdSet[c] + ", message id: " + i);
        }
      }

      for (int c = 0; c < clientCount; c++) {
        for (int i = 0; i < checkCount; i++) {
          Message msg;
          rc = clientSet[c].Recv(out msg);
          Assert.IsTrue(rc, "client id: " + clientIdSet[c] + ", message id: " + i);
          checkMessage(ref msg, checkData);
        }
      }

      for (int i = 0; i < clientSet.Count; i++) {
        clientSet[i].Close();
        rc = clientSet[i].WaitClose(1000);
        Assert.IsTrue(rc);
      }

      server.Close();
      rc = server.WaitClose(1000);
      Assert.IsTrue(rc);
    }

    [Test]
    public void TestSocketServerClientThread() {
      var server = SocketBase.Create<SocketServer>();
      bool rc = server.Bind("127.0.0.1:" + port);
      Assert.IsTrue(rc);

      List<SocketBase> clientSet = new List<SocketBase>();
      for (int i = 0; i < clientCount; i++) {
        var client = SocketBase.Create(SocketType.Client);
        rc = client.Connect("127.0.0.1:" + port);
        Assert.IsTrue(rc);
        clientSet.Add(client);
      }

      port++;

      var checkData = generateCheckData();

      rc = Thread.Wait(() => server.ConnectionCount == clientCount, 1000);
      Assert.IsTrue(rc);

      string hello = "Hello";
      var helloMessage = new Message(16);
      var writer = helloMessage.BufferWriter;
      writer.WriteString(hello);
      helloMessage.Resize(writer.Position);

      for (int i = 0; i < clientCount; i++) {
        rc = clientSet[i].TrySend(ref helloMessage, 1000);
        Assert.IsTrue(rc);
      }

      List<uint> clientIdSet = new List<uint>(clientCount);
      for (int i = 0; i < clientCount; i++) {
        Message msg;
        rc = server.TryRecv(out msg, 1000);
        Assert.IsTrue(rc);
        Assert.Less(0, msg.RouteId);
        Assert.IsFalse(clientIdSet.Contains(msg.RouteId));
        clientIdSet.Add(msg.RouteId);
        var reader = msg.BufferReader;
        var value = reader.ReadString();
        Assert.AreEqual(value, hello);
      }

      var threadPool = new ThreadPool(clientCount);
      threadPool.Send(() => {
        for (int c = 0; c < clientCount; c++) {
          for (int i = 0; i < checkCount; i++) {
            var msg = generateMessage(checkData);
            rc = server.Send(clientIdSet[c], ref msg);
            Assert.IsTrue(rc, "client id: " + clientIdSet[c] + ", message id: " + i);
          }
        }
      });

      int result = 0;
      for (int c = 0; c < clientCount; c++) {
        var client = clientSet[c];
        var clientId = clientIdSet[c];
        threadPool.Post(() => {
          for (int i = 0; i < checkCount; i++) {
            Message msg;
            rc = client.Recv(out msg);
            Assert.IsTrue(rc, "client id: " + clientId + ", message id: " + i);
            checkMessage(ref msg, checkData);
            Atomic.Inc(ref result);
          }
        });
      }

      rc = Thread.Wait(() => result == clientCount * checkCount, -1);
      Assert.IsTrue(rc);

      threadPool.Dispose();
      Assert.AreEqual(result, clientCount * checkCount);

      for (int i = 0; i < clientSet.Count; i++) {
        clientSet[i].Close();
        rc = clientSet[i].WaitClose(1000);
        Assert.IsTrue(rc);
      }

      server.Close();
      rc = server.WaitClose(1000);
      Assert.IsTrue(rc);
    }
  }
}
