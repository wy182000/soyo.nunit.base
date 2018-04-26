using Soyo.Base.Text;
using NUnit.Framework;

namespace UnitTest.Base.Log {
  [TestFixture]
  public class AppenderCollectionTest {
    [Test]
    public void ToArrayTest() {
      AppenderCollection appenderCollection = new AppenderCollection();
      IAppender appender = new AppenderMemory();
      appenderCollection.Add(appender);

      IAppender[] appenderArray = appenderCollection.ToArray();

      Assert.AreEqual(1, appenderArray.Length);
      Assert.AreEqual(appender, appenderArray[0]);
    }

    [Test]
    public void ReadOnlyToArrayTest() {
      AppenderCollection appenderCollection = new AppenderCollection();
      IAppender appender = new AppenderMemory();
      appenderCollection.Add(appender);
      AppenderCollection readonlyAppenderCollection = AppenderCollection.ReadOnly(appenderCollection);

      IAppender[] appenderArray = readonlyAppenderCollection.ToArray();

      Assert.AreEqual(1, appenderArray.Length);
      Assert.AreEqual(appender, appenderArray[0]);
    }
  }
}