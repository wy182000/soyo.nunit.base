using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util {
  [TestFixture]
  [Category("Soyo.Base.Util")]
  internal class MailboxTest {
    [OneTimeSetUp]
    public void Init() {
      Thread.Initialize();
    }

    [OneTimeTearDown]
    public void Terminate() {
      Thread.Terminate();
    }

    [Test]
    public void Test() {
      var checkCount = 10000;
      var queueCount = 10;
      var queueSet = new List<Mailbox>(queueCount);
      for (int i = 0; i < queueCount; i++) {
        var mailbox = Mailbox.Create();
        var checkSet = new HashSet<int>();
        mailbox.State = checkSet;
        mailbox.HookDoMailBefore.Add((message, state) => {
          var value = (int)message;
          var set = (HashSet<int>)state;
          var ret = set.Add(value);
          Assert.IsTrue(ret);
        });
        queueSet.Add(mailbox);
      }

      var threadPool = new ThreadPool();
      for (int i = 0; i < queueCount; i++) {
        for (int n = 0; n < checkCount; n++) {
          var queue = queueSet[i];
          var value = n;
          threadPool.Post(() => queue.Recv(value));
        }
      }

      var rc = Thread.Wait(() => {
        var ret = 0;
        for (int i = 0; i < queueCount; i++) {
          var set = (HashSet<int>)queueSet[i].State;
          ret += set.Count;
        }
        return ret == queueCount * checkCount;
      }, 1000);
      Assert.IsTrue(rc);
      for (int i = 0; i < queueCount; i++) {
        var set = (HashSet<int>)queueSet[i].State;
        Assert.AreEqual(set.Count, checkCount, "queue: id: " + i);
        queueSet[i].Dispose();
      }

      threadPool.Stop();
    }
  }
}
