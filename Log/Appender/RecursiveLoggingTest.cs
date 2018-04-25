﻿using System;
using Soyo.Base.Log;
using NUnit.Framework;
using System.Globalization;

namespace UnitTest.Base.Log {
  [TestFixture]
  public class RecursiveLoggingTest {
    private EventRaisingAppender m_eventRaisingAppender;
    private Soyo.Base.Log.LoggerController m_hierarchy;
    private int m_eventCount;
    private ILogger m_logger;
    private const int MaxRecursion = 3;

    private void SetupRepository() {
      m_hierarchy = new Soyo.Base.Log.LoggerController();

      m_eventRaisingAppender = new EventRaisingAppender();
      m_eventRaisingAppender.LoggingEventAppended += eventRaisingAppender_LoggingEventAppended;

      m_hierarchy.Root.Level = Level.All;
      m_hierarchy.Root.AddAppender(m_eventRaisingAppender);

      BasicConfig.Config(m_hierarchy, m_eventRaisingAppender);

      m_logger = m_hierarchy.Get("test");

    }

    void eventRaisingAppender_LoggingEventAppended(object sender, LoggingEventEventArgs e) {
      if (m_eventCount < MaxRecursion && m_logger != null) {
        m_eventCount++;
        string message = String.Format(CultureInfo.CurrentCulture, "Log event {0} from EventRaisingAppender", m_eventCount);
        Console.WriteLine("Logging message: " + message);
        m_logger.Log(typeof(RecursiveLoggingTest), Level.Warn, message, null);
      }
    }

    [Test]
    public void TestAllowRecursiveLoggingFromAppender() {
      SetupRepository();

      m_eventCount = 0;
      m_logger.Log(typeof(RecursiveLoggingTest), Level.Warn, "Message logged", null);

      Assert.AreEqual(MaxRecursion, m_eventCount, "Expected MaxRecursion recursive calls");
    }

  }
}