using System;
using System.IO;

using NUnit.Framework;

namespace UnitTest.Base.Log.Util {
  /// <summary>
  /// Used for internal unit testing the <see cref="RandomStringPatternConverter"/> class.
  /// </summary>
  /// <remarks>
  /// Used for internal unit testing the <see cref="RandomStringPatternConverter"/> class.
  /// </remarks>
  [TestFixture]
  public class RandomStringPatternConverterTest {
    [Test]
    public void TestConvert() {
      RandomStringPatternConverter converter = new RandomStringPatternConverter();

      // Check default string length
      StringWriter sw = new StringWriter();
      converter.Convert(sw, null);

      Assert.AreEqual(4, sw.ToString().Length, "Default string length should be 4");

      // Set string length to 7
      converter.Option = "7";
      converter.Activate();

      sw = new StringWriter();
      converter.Convert(sw, null);

      string string1 = sw.ToString();
      Assert.AreEqual(7, string1.Length, "string length should be 7");

      // Check for duplicate result
      sw = new StringWriter();
      converter.Convert(sw, null);

      string string2 = sw.ToString();
      Assert.IsTrue(string1 != string2, "strings should be different");
    }

    private class RandomStringPatternConverter {
      private object target = null;

      public RandomStringPatternConverter() {
        target = Utils.CreateInstance("Soyo.Base.Text.RandomStringPatternConverter");
      }

      public string Option {
        get { return Utils.GetProperty(target, "Option") as string; }
        set { Utils.SetProperty(target, "Option", value); }
      }

      public void Convert(TextWriter writer, object state) {
        Type[] types = { typeof(TextWriter), typeof(Soyo.Base.Text.IRender), typeof(object) };
        Utils.InvokeMethod(target, "format", types, writer, null, state);
      }

      public void Activate() {
        Utils.InvokeMethod(target, "Activate");
      }
    }
  }
}