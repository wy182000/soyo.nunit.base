using System.Collections.Generic;
using System.Diagnostics;
using Soyo.Base.Log;
using NUnit.Framework;

namespace UnitTest.Base.Log {
  [TestFixture]
  public class LogInternalTest {
    [Test]
    public void TraceListenerCounterTest() {
      TraceListenerCounter listTraceListener = new TraceListenerCounter();

      Trace.Listeners.Clear();
      Trace.Listeners.Add(listTraceListener);

      Trace.Write("Hello");
      Trace.Write("World");

      Assert.AreEqual(2, listTraceListener.Count);
    }

    [Test, Ignore("trace write ignore on .net core")]
    public void EmitInternalMessages() {
      TraceListenerCounter listTraceListener = new TraceListenerCounter();
      Trace.Listeners.Clear();
      Trace.Listeners.Add(listTraceListener);
      LogInternal.Error(GetType(), "Hello");
      LogInternal.Error(GetType(), "World");
      Trace.Flush();
      Assert.AreEqual(2, listTraceListener.Count);

      try {
        LogInternal.InternalMessage = false;

        LogInternal.Error(GetType(), "Hello");
        LogInternal.Error(GetType(), "World");
        Assert.AreEqual(2, listTraceListener.Count);
      } finally {
        LogInternal.InternalMessage = true;
      }
    }

    [Test]
    public void LogReceivedAdapter() {
      var messages = new List<LogInternal>();

      using (new LogInternal.LogReceivedAdapter(messages)) {
        LogInternal.Debug(GetType(), "Won't be recorded");
        LogInternal.Error(GetType(), "This will be recorded.");
        LogInternal.Error(GetType(), "This will be recorded.");
      }

      Assert.AreEqual(2, messages.Count);
    }
  }

  public class TraceListenerCounter : TraceListener {
    private int count = 0;

    public override void Write(string message) {
      count++;
    }

    public override void WriteLine(string message) {
      Write(message);
    }

    public void Reset() {
      count = 0;
    }

    public int Count {
      get { return count; }
    }
  }
}
