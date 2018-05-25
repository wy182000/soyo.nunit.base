using System.Collections;

using Soyo.Base.LogBase;

using NUnit.Framework;

namespace UnitTest.Base.Log {
  [TestFixture]
  public class LoggerTest {
    private ILogger log;

    // A short message.
    private static string MSG = "M";

    /// <summary>
    /// Any initialization that happens before each test can
    /// go here
    /// </summary>
    [SetUp]
    public void SetUp() {
    }

    /// <summary>
    /// Any steps that happen after each test go here
    /// </summary>
    [TearDown]
    public void TearDown() {
      Utils.GetController().Terminate();
    }

    /// <summary>
    /// Add an appender and see if it can be retrieved.
    /// </summary>
    [Test]
    public void TestAppender1() {
      log = Utils.GetLogger("test").Logger;
      CountingAppender a1 = new CountingAppender();
      a1.Name = "testAppender1";
      log.AddAppender(a1);

      IEnumerator enumAppenders = ((IEnumerable)log.AppenderSet).GetEnumerator();
      Assert.IsTrue(enumAppenders.MoveNext());
      CountingAppender aHat = (CountingAppender)enumAppenders.Current;
      Assert.AreEqual(a1, aHat);
    }

    /// <summary>
    /// Add an appender X, Y, remove X and check if Y is the only
    /// remaining appender.
    /// </summary>
    [Test]
    public void TestAppender2() {
      CountingAppender a1 = new CountingAppender();
      a1.Name = "testAppender2.1";
      CountingAppender a2 = new CountingAppender();
      a2.Name = "testAppender2.2";

      log = Utils.GetLogger("test").Logger;
      log.AddAppender(a1);
      log.AddAppender(a2);

      CountingAppender aHat = (CountingAppender)log.GetAppender(a1.Name);
      Assert.AreEqual(a1, aHat);

      aHat = (CountingAppender)log.GetAppender(a2.Name);
      Assert.AreEqual(a2, aHat);

      log.RemoveAppender("testAppender2.1");

      IEnumerator enumAppenders = ((IEnumerable)log.AppenderSet).GetEnumerator();
      Assert.IsTrue(enumAppenders.MoveNext());
      aHat = (CountingAppender)enumAppenders.Current;
      Assert.AreEqual(a2, aHat);
      Assert.IsTrue(!enumAppenders.MoveNext());

      aHat = (CountingAppender)log.GetAppender(a2.Name);
      Assert.AreEqual(a2, aHat);
    }

    /// <summary>
    /// Test the ability to disable a level of message
    /// </summary>
    [Test]
    public void TestDisable1() {
      CountingAppender caRoot = new CountingAppender();
      ILogger root = Utils.GetController().Root;
      root.AddAppender(caRoot);

      ILoggerController h = Utils.GetController();
      h.Threshold = Level.Info;
      h.Initialize();

      Assert.AreEqual(caRoot.Counter, 0);

      root.Log(Level.Debug, MSG, null);
      Assert.AreEqual(caRoot.Counter, 0);
      root.Log(Level.Info, MSG, null);
      Assert.AreEqual(caRoot.Counter, 1);
      root.Log(Level.Warn, MSG, null);
      Assert.AreEqual(caRoot.Counter, 2);
      root.Log(Level.Warn, MSG, null);
      Assert.AreEqual(caRoot.Counter, 3);

      h.Threshold = Level.Warn;
      root.Log(Level.Debug, MSG, null);
      Assert.AreEqual(caRoot.Counter, 3);
      root.Log(Level.Info, MSG, null);
      Assert.AreEqual(caRoot.Counter, 3);
      root.Log(Level.Warn, MSG, null);
      Assert.AreEqual(caRoot.Counter, 4);
      root.Log(Level.Error, MSG, null);
      Assert.AreEqual(caRoot.Counter, 5);
      root.Log(Level.Assert, MSG, null);
      Assert.AreEqual(caRoot.Counter, 6);
      root.Log(Level.Exception, MSG, null);
      Assert.AreEqual(caRoot.Counter, 7);

      h.Threshold = Level.Off;
      root.Log(Level.Debug, MSG, null);
      Assert.AreEqual(caRoot.Counter, 7);
      root.Log(Level.Info, MSG, null);
      Assert.AreEqual(caRoot.Counter, 7);
      root.Log(Level.Warn, MSG, null);
      Assert.AreEqual(caRoot.Counter, 7);
      root.Log(Level.Error, MSG, null);
      Assert.AreEqual(caRoot.Counter, 7);
      root.Log(Level.Assert, MSG, null);
      Assert.AreEqual(caRoot.Counter, 7);
      root.Log(Level.Exception, MSG, null);
      Assert.AreEqual(caRoot.Counter, 7);
    }

    /// <summary>
    /// Tests the Exists method of the Logger class
    /// </summary>
    [Test]
    public void TestExists() {
      object a = Utils.GetLogger("a");
      object a_b = Utils.GetLogger("a.b");
      object a_b_c = Utils.GetLogger("a.b.c");

      object t;
      t = LogManager.TryGet("xx");
      Assert.IsNull(t);
      t = LogManager.TryGet("a");
      Assert.AreSame(a, t);
      t = LogManager.TryGet("a.b");
      Assert.AreSame(a_b, t);
      t = LogManager.TryGet("a.b.c");
      Assert.AreSame(a_b_c, t);
    }

    /// <summary>
    /// Tests the chained level for a hierarchy
    /// </summary>
    [Test]
    public void TestHierarchy1() {
      ILoggerController h = LogManager.CreateController("");
      h.Root.Level = Level.Error;

      ILogger a0 = h.Get("a");
      Assert.AreEqual("a", a0.Name);
      Assert.IsNull(a0.Level);
      Assert.AreSame(Level.Error, a0.ActiveLevel);

      ILogger a1 = h.Get("a");
      Assert.AreSame(a0, a1);
    }
  }
}