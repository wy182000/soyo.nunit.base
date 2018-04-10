using System;
using System.IO;
using System.Text;
using Soyo.Base.Text;
using NUnit.Framework;

namespace UnitTest.Base.Log.DateFormatter {

  [TestFixture]
  public class AbsoluteTimeDateFormatterTest {

    [TearDown]
    public void resetCounts() {
      FormatterOne.invocations = 0;
    }

    [Test]
    public void CacheWorksForSameTicks() {
      StringWriter sw = new StringWriter();
      FormatterOne f1 = new FormatterOne();
      FormatterOne f2 = new FormatterOne();
      DateTime dt = DateTime.Now;
      f1.Format(dt, sw);
      f2.Format(dt, sw);
      Assert.AreEqual(1, FormatterOne.invocations);
    }

    [Test]
    public void CacheWorksForSameSecond() {
      StringWriter sw = new StringWriter();
      FormatterOne f1 = new FormatterOne();
      FormatterOne f2 = new FormatterOne();
      DateTime dt1 = DateTime.Today;
      DateTime dt2 = dt1.AddMilliseconds(600);
      f1.Format(dt1, sw);
      f2.Format(dt2, sw);
      Assert.AreEqual(1, FormatterOne.invocations);
    }

    [Test]
    public void CacheExpiresWhenCrossingSecond() {
      StringWriter sw = new StringWriter();
      FormatterOne f1 = new FormatterOne();
      FormatterOne f2 = new FormatterOne();
      DateTime dt1 = DateTime.Today.AddMinutes(1);
      DateTime dt2 = dt1.AddMilliseconds(1100);
      f1.Format(dt1, sw);
      f2.Format(dt2, sw);
      Assert.AreEqual(2, FormatterOne.invocations);
    }

    [Test]
    public void CacheIsLocalToSubclass() {
      StringWriter sw = new StringWriter();
      FormatterOne f1 = new FormatterOne();
      FormatterTwo f2 = new FormatterTwo();
      DateTime dt1 = DateTime.Today.AddMinutes(10);
      f1.Format(dt1, sw);
      f2.Format(dt1, sw);
      Assert.AreEqual(2, FormatterOne.invocations);
    }
  }

  internal class FormatterOne : DateFormatAbsoluteTime {
    internal static int invocations = 0;

    override protected void formatDateWithoutMillis(DateTime dateToFormat,
                                                    StringBuilder buffer) {
      invocations++;
    }

  }

  internal class FormatterTwo : FormatterOne {
  }
}
