using Soyo.Base.Text;
using NUnit.Framework;

namespace UnitTest.Base.Log.Appender {
  /// <summary>
  /// Used for internal unit testing the <see cref="AppenderCollection"/> class.
  /// </summary>
  /// <remarks>
  /// Used for internal unit testing the <see cref="AppenderCollection"/> class.
  /// </remarks>
  /// <author>Carlos Muñoz</author>
  [TestFixture]
  public class AppenderCollectionTest {
    /// <summary>
    /// Verifies that ToArray returns the elements of the <see cref="AppenderCollection"/>
    /// </summary>
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