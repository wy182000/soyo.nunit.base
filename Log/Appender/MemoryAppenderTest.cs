﻿using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Soyo.Base.Text;
using Soyo.Base.LogBase;

namespace UnitTest.Base.Log {
  [TestFixture]
  public class MemoryAppenderTest {
    private static int cThreadsRunning;
    private const int cThreadsMax = 10;
    private const int cLogEntriesPerThread = 100;
    private const long cEventsExpected = cLogEntriesPerThread * cThreadsMax;

    [Test]
    public void TestThreadSafety() {
      ILoggerController rep = LogManager.CreateController(Guid.NewGuid().ToString());
      var memoryAppender = new AppenderMemory();
      var patternLayout = new LayoutPattern();
      memoryAppender.Layout = patternLayout;
      memoryAppender.Activate();
      BasicConfig.Config(rep, memoryAppender);

      cThreadsRunning = cThreadsMax;
      var threads = Enumerable.Range(0, cThreadsMax)
          .Select(i => new Thread(LogMessages(rep.Name)))
          .ToList();

      foreach (var thread in threads) {
        thread.Start();
      }

      long cEventsRead = 0;
      while (cThreadsRunning > 0) {
        var events = memoryAppender.PopAllInputs();
        cEventsRead += events.Length;
      }
      foreach (var thread in threads) {
        thread.Join();
      }
      cEventsRead += memoryAppender.PopAllInputs().Length;
      Assert.AreEqual(cEventsExpected, cEventsRead, "Log events were lost.");
    }

    private static ThreadStart LogMessages(string repository) {
      return () => {
        var logger = LogManager.Get(repository, "LoggerThread");
        for (var i = 0; i < cLogEntriesPerThread; i++) {
          logger.Info("Logging message " + i);
        }
        Interlocked.Decrement(ref cThreadsRunning);
      };
    }
  }
}