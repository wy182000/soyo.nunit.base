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
      Soyo.Base.Log.LogInternal.Error(GetType(), "Hello");
      Soyo.Base.Log.LogInternal.Error(GetType(), "World");
      Trace.Flush();
      Assert.AreEqual(2, listTraceListener.Count);

      try {
        Soyo.Base.Log.LogInternal.InternalMessage = false;

        Soyo.Base.Log.LogInternal.Error(GetType(), "Hello");
        Soyo.Base.Log.LogInternal.Error(GetType(), "World");
        Assert.AreEqual(2, listTraceListener.Count);
      } finally {
        Soyo.Base.Log.LogInternal.InternalMessage = true;
      }
    }

    [Test]
    public void LogReceivedAdapter() {
      var messages = new List<LogInternal>();

      using (new Soyo.Base.Log.LogInternal.LogReceivedAdapter(messages)) {
        Soyo.Base.Log.LogInternal.Debug(GetType(), "Won't be recorded");
        Soyo.Base.Log.LogInternal.Error(GetType(), "This will be recorded.");
        Soyo.Base.Log.LogInternal.Error(GetType(), "This will be recorded.");
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
