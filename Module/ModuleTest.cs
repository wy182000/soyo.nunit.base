using Soyo.Base;
using Soyo.Base.Json;
using Soyo.Base.Module;

using NUnit.Framework;

namespace UnitTest.Base.Module {
  using Module = Soyo.Base.Module.Module;
  public class ModuleCheck : ModuleBase {
    public static bool initialized = false;
    public static bool terminated = false;
    public static long update = 0;
    public static int checkValue = 0;

    protected override void processText(object text, Mail mail) {
      var value = (int)text;
      Assert.Less(0, value);
      checkValue += value;
    }

    protected override bool onInitialize(Module module, ModuleConfig config) {
      initialized = true;
      UpdateAction = onUpdate;
      return base.onInitialize(module, config);
    }

    protected override void onTerminate() {
      terminated = true;
      base.onTerminate();
    }

    protected void onUpdate(object state) {
      update++;
    }
  }

  [TestFixture]
  [Category("Soyo.Base")]
  internal class ModuleTest {
    [OneTimeSetUp]
    public void Init() {
      Thread.Initialize();
      Mailbox.Initialize();
    }

    [OneTimeTearDown]
    public void Terminate() {
      Thread.Terminate();
    }


    private static string json = @"
      {
        ""type"": ""Soyo.Base.ModuleConfig"",
        ""harbor"": 1,
        ""module"": ""UnitTest.Base.Module.ModuleCheck"",
        ""name"": ""ModuleCheck"",
        ""profile"": true
      }";

    private static string name = "";
    private const int checkCount = 10000;

    [Test]
    public void TestModule() {
      var node = Json.Parse(json);
      Assert.IsNotNull(node);
      var config = node.ToObject<Config>();
      Assert.IsNotNull(config);
      var moduleConfig = config as ModuleConfig;
      Assert.IsNotNull(moduleConfig);
      name = moduleConfig.name;
      config.Apply();

      var handle = Module.Find(name);
      Assert.Less(0, handle);
      var module = Module.Get(handle) as Module;
      Assert.IsNotNull(module);
      Assert.AreEqual(handle, module.Handle);

      ModuleCheck.checkValue = 0;
      for (int i = 1; i <= checkCount; i++) {
        var value = i;
        switch (i % 3) {
          case 0:
            Thread.WorkThread.Post(() => Module.Send("."+name, MailType.Text, value, 0, 0));
            break;
          case 1:
            Thread.WorkThread.Post(() => Module.Send(":"+handle, MailType.Text, value, 0, 0));
            break;
          case 2:
            Thread.WorkThread.Post(() => Module.Send(handle, MailType.Text, value, 0, 0));
            break;
        }
      }

      Thread.Wait(() => ModuleCheck.checkValue == (1 + checkCount) * checkCount / 2, 1000);

      Assert.IsTrue(ModuleCheck.initialized);
      Assert.Less(0, ModuleCheck.update);
      Assert.AreEqual(ModuleCheck.checkValue, (1 + checkCount) * checkCount / 2);

      Mailbox.Remove(handle);
      Thread.Wait(() => ModuleCheck.terminated, 1000);

      Assert.IsTrue(ModuleCheck.terminated);
    }
  }
}
