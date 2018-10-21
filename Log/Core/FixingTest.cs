using System;
using System.Threading;

using Soyo.Base.LogBase;
using Soyo.Base.Text;

using NUnit.Framework;

namespace UnitTest.Base.Log {
  /// <summary>
  /// </<summary>
  [TestFixture]
  public class FixingTest {
    const string TEST_CONTROLLER = "Test Repository";

    [OneTimeSetUp]
    public void CreateRepository() {
      bool exists = false;
      ILoggerController[] repositories = LogManager.GetAllController();
      if (repositories != null) {
        foreach (var r in repositories) {
          if (r.Name == TEST_CONTROLLER) {
            exists = true;
            break;
          }
        }
      }
      if (!exists) {
        LogManager.CreateController(TEST_CONTROLLER);
      }

      // write-once
      if (Thread.CurrentThread.Name == null) {
        Thread.CurrentThread.Name = "Test thread";
      }
    }

    [Test]
    public void TestUnfixedValues() {
      LocationInfo.SKIP_LOGGER = false;
      LogObjectData loggingEventData = BuildStandardEventData();

      LogObject loggingEvent = new LogObject(
        loggingEventData.Location.GetType(),
        LogManager.GetController(TEST_CONTROLLER),
        loggingEventData.LoggerName,
        loggingEventData.Level,
        loggingEventData.Message,
        new Exception("This is the exception"));

      AssertExpectedLoggingEvent(loggingEvent, loggingEventData);

      Assert.AreEqual(FixFlags.None, loggingEvent.Fix, "Fixed Fields is incorrect");
      LocationInfo.SKIP_LOGGER = true;
    }

    [Test]
    public void TestAllFixedValues() {
      LocationInfo.SKIP_LOGGER = false;
      LogObjectData loggingEventData = BuildStandardEventData();

      // LoggingEvents occur at distinct points in time
      LogObject loggingEvent = new LogObject(
        loggingEventData.Location.GetType(),
        LogManager.GetController(TEST_CONTROLLER),
        loggingEventData.LoggerName,
        loggingEventData.Level,
        loggingEventData.Message,
        new Exception("This is the exception"));

      AssertExpectedLoggingEvent(loggingEvent, loggingEventData);

      loggingEvent.Fix = FixFlags.All;

      Assert.AreEqual(FixFlags.Location | FixFlags.Identity | FixFlags.Partial | FixFlags.Message | FixFlags.ThreadName | FixFlags.Exception | FixFlags.Domain | FixFlags.PropertySet, loggingEvent.Fix, "Fixed Fields is incorrect");
      LocationInfo.SKIP_LOGGER = true;
    }

    [Test]
    public void TestNoFixedValues() {
      LocationInfo.SKIP_LOGGER = false;
      LogObjectData loggingEventData = BuildStandardEventData();

      // LoggingEvents occur at distinct points in time
      LogObject loggingEvent = new LogObject(
        loggingEventData.Location.GetType(),
        LogManager.GetController(TEST_CONTROLLER),
        loggingEventData.LoggerName,
        loggingEventData.Level,
        loggingEventData.Message,
        new Exception("This is the exception"));

      AssertExpectedLoggingEvent(loggingEvent, loggingEventData);

      loggingEvent.Fix = FixFlags.None;

      Assert.AreEqual(FixFlags.None, loggingEvent.Fix, "Fixed Fields is incorrect");
      LocationInfo.SKIP_LOGGER = true;
    }

    private static LogObjectData BuildStandardEventData() {
      LogObjectData loggingEventData = new LogObjectData();
      loggingEventData.LoggerName = Soyo.Base.SystemInfo.TypeName<FixingTest>();
      loggingEventData.Level = Level.Warn;
      loggingEventData.Message = "Logging event works";
      loggingEventData.Domain = "ReallySimpleApp";
      loggingEventData.Location = new LocationInfo(typeof(FixingTest).Name, "Main", "Class1.cs", "29"); //Completely arbitary
      loggingEventData.ThreadName = Thread.CurrentThread.Name;
      loggingEventData.Date = DateTime.UtcNow.Date;
      loggingEventData.ExceptionString = "Exception occured here";
      return loggingEventData;
    }

    private static void AssertExpectedLoggingEvent(LogObject loggingEvent, LogObjectData loggingEventData) {
      Assert.AreEqual("ReallySimpleApp", loggingEventData.Domain, "Domain is incorrect");
      Assert.AreEqual("System.Exception: This is the exception", loggingEvent.GetExceptionString(), "Exception is incorrect");
      Assert.AreEqual(null, loggingEventData.Identity, "Identity is incorrect");
      Assert.AreEqual(Level.Warn, loggingEventData.Level, "Level is incorrect");
      Assert.AreEqual("get_Location", loggingEvent.Location.MethodName, "Location Info is incorrect");
      Assert.AreEqual("UnitTest.Base.Log.FixingTest", loggingEventData.LoggerName, "LoggerName is incorrect");
      Assert.AreEqual(LogManager.GetController(TEST_CONTROLLER), loggingEvent.Controller, "Repository is incorrect");
      Assert.AreEqual(Thread.CurrentThread.Name, loggingEventData.ThreadName, "ThreadName is incorrect");
      // This test is redundant as loggingEventData.TimeStamp is a value type and cannot be null
      // Assert.IsNotNull(loggingEventData.TimeStampUtc, "TimeStamp is incorrect");
      Assert.AreEqual("Logging event works", loggingEvent.RenderedMessage, "Message is incorrect");
    }
  }
}