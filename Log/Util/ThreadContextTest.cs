using System;
using System.Threading;

using Soyo.Base;
using Soyo.Base.Text;
using Soyo.Base.Log;

using NUnit.Framework;

namespace UnitTest.Base.Log {
  /// <summary>
  /// Used for internal unit testing the <see cref="ThreadContext"/> class.
  /// </summary>
  /// <remarks>
  /// Used for internal unit testing the <see cref="ThreadContext"/> class.
  /// </remarks>
  [TestFixture]
  public class ThreadContextTest {
    [TearDown]
    public void TearDown() {
      Utils.RemovePropertyFromAllContexts();
    }

    [Test]
    public void TestThreadPropertiesPattern() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%property{" + Utils.PROPERTY_KEY + "}");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestThreadProperiesPattern");

      log1.Info("TestMessage");
      Assert.AreEqual(TextDefault.NullText, stringAppender.GetString(), "Test no thread properties value set");
      stringAppender.Reset();

      ThreadContext.PropertySet[Utils.PROPERTY_KEY] = "val1";

      log1.Info("TestMessage");
      Assert.AreEqual("val1", stringAppender.GetString(), "Test thread properties value set");
      stringAppender.Reset();

      ThreadContext.PropertySet.Remove(Utils.PROPERTY_KEY);

      log1.Info("TestMessage");
      Assert.AreEqual(TextDefault.NullText, stringAppender.GetString(), "Test thread properties value removed");
      stringAppender.Reset();
    }

    [Test]
    public void TestThreadStackPattern() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%property{" + Utils.PROPERTY_KEY + "}");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestThreadStackPattern");

      log1.Info("TestMessage");
      Assert.AreEqual(TextDefault.NullText, stringAppender.GetString(), "Test no thread stack value set");
      stringAppender.Reset();

      using (ThreadContext.Stacks[Utils.PROPERTY_KEY].Push("val1")) {
        log1.Info("TestMessage");
        Assert.AreEqual("val1", stringAppender.GetString(), "Test thread stack value set");
        stringAppender.Reset();
      }

      log1.Info("TestMessage");
      Assert.AreEqual(TextDefault.NullText, stringAppender.GetString(), "Test thread stack value removed");
      stringAppender.Reset();
    }

    [Test]
    public void TestThreadStackPattern2() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%property{" + Utils.PROPERTY_KEY + "}");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestThreadStackPattern");

      log1.Info("TestMessage");
      Assert.AreEqual(TextDefault.NullText, stringAppender.GetString(), "Test no thread stack value set");
      stringAppender.Reset();

      using (ThreadContext.Stacks[Utils.PROPERTY_KEY].Push("val1")) {
        log1.Info("TestMessage");
        Assert.AreEqual("val1", stringAppender.GetString(), "Test thread stack value set");
        stringAppender.Reset();

        using (ThreadContext.Stacks[Utils.PROPERTY_KEY].Push("val2")) {
          log1.Info("TestMessage");
          Assert.AreEqual("val1 val2", stringAppender.GetString(), "Test thread stack value pushed 2nd val");
          stringAppender.Reset();
        }
      }

      log1.Info("TestMessage");
      Assert.AreEqual(TextDefault.NullText, stringAppender.GetString(), "Test thread stack value removed");
      stringAppender.Reset();
    }

    [Test]
    public void TestThreadStackPatternNullVal() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%property{" + Utils.PROPERTY_KEY + "}");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestThreadStackPattern");

      log1.Info("TestMessage");
      Assert.AreEqual(TextDefault.NullText, stringAppender.GetString(), "Test no thread stack value set");
      stringAppender.Reset();

      using (ThreadContext.Stacks[Utils.PROPERTY_KEY].Push(null)) {
        log1.Info("TestMessage");
        Assert.AreEqual(TextDefault.NullText, stringAppender.GetString(), "Test thread stack value set");
        stringAppender.Reset();
      }

      log1.Info("TestMessage");
      Assert.AreEqual(TextDefault.NullText, stringAppender.GetString(), "Test thread stack value removed");
      stringAppender.Reset();
    }

    [Test]
    public void TestThreadStackPatternNullVal2() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%property{" + Utils.PROPERTY_KEY + "}");

      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      ILog log1 = LogManager.Get(rep.Name, "TestThreadStackPattern");

      log1.Info("TestMessage");
      Assert.AreEqual(TextDefault.NullText, stringAppender.GetString(), "Test no thread stack value set");
      stringAppender.Reset();

      using (ThreadContext.Stacks[Utils.PROPERTY_KEY].Push("val1")) {
        log1.Info("TestMessage");
        Assert.AreEqual("val1", stringAppender.GetString(), "Test thread stack value set");
        stringAppender.Reset();

        using (ThreadContext.Stacks[Utils.PROPERTY_KEY].Push(null)) {
          log1.Info("TestMessage");
          Assert.AreEqual("val1 ", stringAppender.GetString(), "Test thread stack value pushed null");
          stringAppender.Reset();
        }
      }

      log1.Info("TestMessage");
      Assert.AreEqual(TextDefault.NullText, stringAppender.GetString(), "Test thread stack value removed");
      stringAppender.Reset();
    }

    private static string TestBackgroundThreadContextPropertyRepository;

    [Test]
    public void TestBackgroundThreadContextProperty() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%property{DateTimeTodayToString}");

      ILoggerController rep = LogManager.CreateController(TestBackgroundThreadContextPropertyRepository = "TestBackgroundThreadContextPropertyRepository" + Guid.NewGuid().ToString());
      BasicConfig.Config(rep, stringAppender);

      System.Threading.Thread thread = new System.Threading.Thread(new ThreadStart(ExecuteBackgroundThread));
      thread.Start();

      System.Threading.Thread.CurrentThread.Join(2000);
    }

    private static void ExecuteBackgroundThread() {
      ILog log = LogManager.Get(TestBackgroundThreadContextPropertyRepository, "ExecuteBackGroundThread");
      ThreadContext.PropertySet["DateTimeTodayToString"] = DateTime.Today.ToString();

      log.Info("TestMessage");

      Soyo.Base.Log.LoggerController hierarchyLoggingRepository = (Soyo.Base.Log.LoggerController)log.Logger.Controller;
      StringAppender stringAppender = (StringAppender)hierarchyLoggingRepository.Root.Appenders[0];

      Assert.AreEqual(DateTime.Today.ToString(), stringAppender.GetString());
    }
  }
}