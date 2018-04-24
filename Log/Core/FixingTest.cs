﻿using System;
using System.Threading;

using Soyo.Base.Log;
using Soyo.Base.Text;

using NUnit.Framework;

namespace UnitTest.Base.Log.Core {
  /// <summary>
  /// </<summary>
  [TestFixture]
  public class FixingTest {
    const string TEST_REPOSITORY = "Test Repository";

    [OneTimeSetUp]
    public void CreateRepository() {
      bool exists = false;
      Soyo.Base.Log.IRepository[] repositories = LogManager.GetAllRepositories();
      if (repositories != null) {
        foreach (Soyo.Base.Log.IRepository r in repositories) {
          if (r.Name == TEST_REPOSITORY) {
            exists = true;
            break;
          }
        }
      }
      if (!exists) {
        LogManager.CreateRepository(TEST_REPOSITORY);
      }

      // write-once
      if (Thread.CurrentThread.Name == null) {
        Thread.CurrentThread.Name = "Test thread";
      }
    }

    [Test]
    public void TestUnfixedValues() {
      LogObjectData loggingEventData = BuildStandardEventData();

      // LoggingEvents occur at distinct points in time
      LogObject loggingEvent = new LogObject(
        loggingEventData.Location.GetType(),
        LogManager.GetRepository(TEST_REPOSITORY),
        loggingEventData.LoggerName,
        loggingEventData.Level,
        loggingEventData.Message,
        new Exception("This is the exception"));

      AssertExpectedLoggingEvent(loggingEvent, loggingEventData);

      Assert.AreEqual(FixFlags.None, loggingEvent.Fix, "Fixed Fields is incorrect");
    }

    [Test]
    public void TestAllFixedValues() {
      LogObjectData loggingEventData = BuildStandardEventData();

      // LoggingEvents occur at distinct points in time
      LogObject loggingEvent = new LogObject(
        loggingEventData.Location.GetType(),
        LogManager.GetRepository(TEST_REPOSITORY),
        loggingEventData.LoggerName,
        loggingEventData.Level,
        loggingEventData.Message,
        new Exception("This is the exception"));

      AssertExpectedLoggingEvent(loggingEvent, loggingEventData);

      loggingEvent.Fix = FixFlags.All;

      Assert.AreEqual(FixFlags.Location | FixFlags.Identity | FixFlags.Partial | FixFlags.Message | FixFlags.ThreadName | FixFlags.Exception | FixFlags.Domain | FixFlags.PropertySet, loggingEvent.Fix, "Fixed Fields is incorrect");
    }

    [Test]
    public void TestNoFixedValues() {
      LogObjectData loggingEventData = BuildStandardEventData();

      // LoggingEvents occur at distinct points in time
      LogObject loggingEvent = new LogObject(
        loggingEventData.Location.GetType(),
        LogManager.GetRepository(TEST_REPOSITORY),
        loggingEventData.LoggerName,
        loggingEventData.Level,
        loggingEventData.Message,
        new Exception("This is the exception"));

      AssertExpectedLoggingEvent(loggingEvent, loggingEventData);

      loggingEvent.Fix = FixFlags.None;

      Assert.AreEqual(FixFlags.None, loggingEvent.Fix, "Fixed Fields is incorrect");
    }

    private static LogObjectData BuildStandardEventData() {
      LogObjectData loggingEventData = new LogObjectData();
      loggingEventData.LoggerName = typeof(FixingTest).FullName;
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
      Assert.AreEqual("UnitTest.Base.Log.Core.FixingTest", loggingEventData.LoggerName, "LoggerName is incorrect");
      Assert.AreEqual(LogManager.GetRepository(TEST_REPOSITORY), loggingEvent.Repository, "Repository is incorrect");
      Assert.AreEqual(Thread.CurrentThread.Name, loggingEventData.ThreadName, "ThreadName is incorrect");
      // This test is redundant as loggingEventData.TimeStamp is a value type and cannot be null
      // Assert.IsNotNull(loggingEventData.TimeStampUtc, "TimeStamp is incorrect");
      Assert.AreEqual("Logging event works", loggingEvent.RenderedMessage, "Message is incorrect");
    }
  }
}