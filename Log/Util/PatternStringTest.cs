using System;
using Soyo.Base.Text;
using NUnit.Framework;
using System.IO;

namespace UnitTest.Base.Log {
  [TestFixture]
  public class PatternStringTest : MarshalByRefObject {
    [Test]
    public void TestEnvironmentFolderPathPatternConverter() {
      string[] specialFolderNames = Enum.GetNames(typeof(Environment.SpecialFolder));

      foreach (string specialFolderName in specialFolderNames) {
        string pattern = "%envFolderPath{" + specialFolderName + "}";

        PatternString patternString = new PatternString(pattern);

        string evaluatedPattern = patternString.Format();

        Environment.SpecialFolder specialFolder =
            (Environment.SpecialFolder)Enum.Parse(typeof(Environment.SpecialFolder), specialFolderName);

        Assert.AreEqual(Environment.GetFolderPath(specialFolder), evaluatedPattern);
      }
    }

    private static string CreateTempConfigFile(string configurationFileContent) {
      string fileName = Soyo.Base.IO.Path.GetTempFile();
      File.WriteAllText(fileName, configurationFileContent);
      return fileName;
    }
  }
}
