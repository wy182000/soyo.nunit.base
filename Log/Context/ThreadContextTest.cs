using System;
using System.Threading;

using Soyo.Base;
using Soyo.Base.Text;
using Soyo.Base.Log;
using Soyo.Base.Log.Config;
using Soyo.Base.Log.Repository;
using UnitTest.Base.Log.Appender;

using NUnit.Framework;

namespace UnitTest.Base.Log.Context {
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

      ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
      BasicConfigurator.Configure(rep, stringAppender);

      ILog log1 = LogManager.GetLogger(rep.Name, "TestThreadProperiesPattern");

      log1.Info("TestMessage");
      Assert.AreEqual(Soyo.Base.Log.Util.SystemInfo.NullText, stringAppender.GetString(), "Test no thread properties value set");
      stringAppender.Reset();

      ThreadContext.PropertySet[Utils.PROPERTY_KEY] = "val1";

      log1.Info("TestMessage");
      Assert.AreEqual("val1", stringAppender.GetString(), "Test thread properties value set");
      stringAppender.Reset();

      ThreadContext.PropertySet.Remove(Utils.PROPERTY_KEY);

      log1.Info("TestMessage");
      Assert.AreEqual(Soyo.Base.Log.Util.SystemInfo.NullText, stringAppender.GetString(), "Test thread properties value removed");
      stringAppender.Reset();
    }

    [Test]
    public void TestThreadStackPattern() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%property{" + Utils.PROPERTY_KEY + "}");

      ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
      BasicConfigurator.Configure(rep, stringAppender);

      ILog log1 = LogManager.GetLogger(rep.Name, "TestThreadStackPattern");

      log1.Info("TestMessage");
      Assert.AreEqual(Soyo.Base.Log.Util.SystemInfo.NullText, stringAppender.GetString(), "Test no thread stack value set");
      stringAppender.Reset();

      using (ThreadContext.Stacks[Utils.PROPERTY_KEY].Push("val1")) {
        log1.Info("TestMessage");
        Assert.AreEqual("val1", stringAppender.GetString(), "Test thread stack value set");
        stringAppender.Reset();
      }

      log1.Info("TestMessage");
      Assert.AreEqual(Soyo.Base.Log.Util.SystemInfo.NullText, stringAppender.GetString(), "Test thread stack value removed");
      stringAppender.Reset();
    }

    [Test]
    public void TestThreadStackPattern2() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%property{" + Utils.PROPERTY_KEY + "}");

      ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
      BasicConfigurator.Configure(rep, stringAppender);

      ILog log1 = LogManager.GetLogger(rep.Name, "TestThreadStackPattern");

      log1.Info("TestMessage");
      Assert.AreEqual(Soyo.Base.Log.Util.SystemInfo.NullText, stringAppender.GetString(), "Test no thread stack value set");
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
      Assert.AreEqual(Soyo.Base.Log.Util.SystemInfo.NullText, stringAppender.GetString(), "Test thread stack value removed");
      stringAppender.Reset();
    }

    [Test]
    public void TestThreadStackPatternNullVal() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%property{" + Utils.PROPERTY_KEY + "}");

      ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
      BasicConfigurator.Configure(rep, stringAppender);

      ILog log1 = LogManager.GetLogger(rep.Name, "TestThreadStackPattern");

      log1.Info("TestMessage");
      Assert.AreEqual(Soyo.Base.Log.Util.SystemInfo.NullText, stringAppender.GetString(), "Test no thread stack value set");
      stringAppender.Reset();

      using (ThreadContext.Stacks[Utils.PROPERTY_KEY].Push(null)) {
        log1.Info("TestMessage");
        Assert.AreEqual(Soyo.Base.Log.Util.SystemInfo.NullText, stringAppender.GetString(), "Test thread stack value set");
        stringAppender.Reset();
      }

      log1.Info("TestMessage");
      Assert.AreEqual(Soyo.Base.Log.Util.SystemInfo.NullText, stringAppender.GetString(), "Test thread stack value removed");
      stringAppender.Reset();
    }

    [Test]
    public void TestThreadStackPatternNullVal2() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%property{" + Utils.PROPERTY_KEY + "}");

      ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
      BasicConfigurator.Configure(rep, stringAppender);

      ILog log1 = LogManager.GetLogger(rep.Name, "TestThreadStackPattern");

      log1.Info("TestMessage");
      Assert.AreEqual(Soyo.Base.Log.Util.SystemInfo.NullText, stringAppender.GetString(), "Test no thread stack value set");
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
      Assert.AreEqual(Soyo.Base.Log.Util.SystemInfo.NullText, stringAppender.GetString(), "Test thread stack value removed");
      stringAppender.Reset();
    }

    private static string TestBackgroundThreadContextPropertyRepository;

    [Test]
    public void TestBackgroundThreadContextProperty() {
      StringAppender stringAppender = new StringAppender();
      stringAppender.Layout = new LayoutPattern("%property{DateTimeTodayToString}");

      ILoggerRepository rep = LogManager.CreateRepository(TestBackgroundThreadContextPropertyRepository = "TestBackgroundThreadContextPropertyRepository" + Guid.NewGuid().ToString());
      BasicConfigurator.Configure(rep, stringAppender);

      System.Threading.Thread thread = new System.Threading.Thread(new ThreadStart(ExecuteBackgroundThread));
      thread.Start();

      System.Threading.Thread.CurrentThread.Join(2000);
    }

    private static void ExecuteBackgroundThread() {
      ILog log = LogManager.GetLogger(TestBackgroundThreadContextPropertyRepository, "ExecuteBackGroundThread");
      ThreadContext.PropertySet["DateTimeTodayToString"] = DateTime.Today.ToString();

      log.Info("TestMessage");

      Soyo.Base.Log.Repository.Hierarchy.Hierarchy hierarchyLoggingRepository = (Soyo.Base.Log.Repository.Hierarchy.Hierarchy)log.Logger.Repository;
      StringAppender stringAppender = (StringAppender)hierarchyLoggingRepository.Root.Appenders[0];

      Assert.AreEqual(DateTime.Today.ToString(), stringAppender.GetString());
    }
  }
}