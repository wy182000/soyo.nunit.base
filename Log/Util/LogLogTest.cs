using System.Collections;
using System.Diagnostics;
using Soyo.Base.Log;
using NUnit.Framework;

namespace UnitTest.Base.Log.Util {
  [TestFixture]
  public class LogLogTest {
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
      Soyo.Base.Log.Log.Error(GetType(), "Hello");
      Soyo.Base.Log.Log.Error(GetType(), "World");
      Trace.Flush();
      Assert.AreEqual(2, listTraceListener.Count);

      try {
        Soyo.Base.Log.Log.InternalMessage = false;

        Soyo.Base.Log.Log.Error(GetType(), "Hello");
        Soyo.Base.Log.Log.Error(GetType(), "World");
        Assert.AreEqual(2, listTraceListener.Count);
      } finally {
        Soyo.Base.Log.Log.InternalMessage = true;
      }
    }

    [Test]
    public void LogReceivedAdapter() {
      ArrayList messages = new ArrayList();

      using (new Soyo.Base.Log.Log.LogReceivedAdapter(messages)) {
        Soyo.Base.Log.Log.Debug(GetType(), "Won't be recorded");
        Soyo.Base.Log.Log.Error(GetType(), "This will be recorded.");
        Soyo.Base.Log.Log.Error(GetType(), "This will be recorded.");
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
