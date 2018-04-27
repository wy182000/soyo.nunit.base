using System;
using System.Xml;
using NUnit.Framework;

using Soyo.Base.Log;

namespace UnitTest.Base.Log {
  [TestFixture]
  public class XmlConfigTest {
    public string TestProp { set; get; }

    [Test]
    public void EnvironmentOnWindowsIsCaseInsensitive() {
      SetTestPropWithPath();
      Assert.AreNotEqual("Path=", TestProp);
    }

    private void SetTestPropWithPath() {
      XmlDocument doc = new XmlDocument();
      XmlElement el = doc.CreateElement("param");
      el.SetAttribute("name", "TestProp");
      el.SetAttribute("value", "Path=${path}");
      new TestConfigurator().PublicSetParameter(el, this);
    }

    private class TestConfigurator : ControllerXmlConfig {
      public TestConfigurator() : base(null) {
      }
      public void PublicSetParameter(XmlElement element, object target) {
        setParameter(element, target);
      }
    }
  }
}