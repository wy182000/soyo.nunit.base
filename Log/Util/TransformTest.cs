using System;

using Soyo.Base.LogBase;

using NUnit.Framework;

namespace UnitTest.Base.Log {

  [TestFixture]
  public class TransformTest {
    [Test]
    public void MaskXmlInvalidCharactersAllowsJapaneseCharacters() {
      string kome = "\u203B";
      Assert.AreEqual(kome, XmlTransform.MaskInvalidCharacters(kome, "?"));
    }

    [Test]
    public void MaskXmlInvalidCharactersMasks0Char() {
      string c = "\u0000";
      Assert.AreEqual("?", XmlTransform.MaskInvalidCharacters(c, "?"));
    }
  }
}
