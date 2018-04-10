using System;

using Soyo.Base.Log.Util;

using NUnit.Framework;

namespace UnitTest.Base.Log.Util {

  [TestFixture]
  public class TransformTest {
    [Test]
    public void MaskXmlInvalidCharactersAllowsJapaneseCharacters() {
      string kome = "\u203B";
      Assert.AreEqual(kome, Transform.MaskXmlInvalidCharacters(kome, "?"));
    }

    [Test]
    public void MaskXmlInvalidCharactersMasks0Char() {
      string c = "\u0000";
      Assert.AreEqual("?", Transform.MaskXmlInvalidCharacters(c, "?"));
    }
  }
}
