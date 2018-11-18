using System;

using Soyo.Base;

using NUnit.Framework;

[assembly: ServiceAssembly("UnitTest.Base.IAssembly", "UnitTest.Base.AssemblyInstance")]
[assembly: ServiceAssembly("IAssemblyA", "AssemblyInstanceA")]

namespace UnitTest.Base {
  public interface IAssembly {
    int get();
  }

  public interface IAssemblyA {
    int get();
  }

  public interface IClass {
    int get();
  }

  public interface IClassA {
    int get();
  }

  public interface IClassS {
    int get();
  }

  public interface IConstructor {
    int get();
  }

  public interface IConstructorA {
    int get();
  }

  public interface IConstructorB {
    int get();
  }

  public interface ISelector {
    int get();
  }

  public interface INull {
    int get();
  }

  public class AssemblyInstance : IAssembly {
    public int get() {
      return 1;
    }
  }

  public class AssemblyInstanceA : IAssemblyA {
    public int get() {
      return 11;
    }
  }


  [Service(typeof(IClass))]
  public class ClassInstance : IClass {
    public virtual int get() {
      return 2;
    }
  }

  public class ClassInstanceInherite : ClassInstance {
    public override int get() {
      return 23;
    }
  }

  [Service(typeof(IClassA))]
  public class ClassInstanceA {
    public int get() {
      return 21;
    }
  }

  public class ClassInstanceAA : IClassA {
    public int get() {
      return 22;
    }
  }

  [Service(typeof(IClassS))]
  public class ClassInstanceS : Singleton<ClassInstanceS>, IClassS {
    protected ClassInstanceS() { }

    public int get() {
      return 23;
    }
  }

  public class ConstructorInstance : IConstructor {
    public int get() {
      return 3;
    }

    [ServiceConstructor(typeof(IConstructor))]
    public static ConstructorInstance create() {
      return new ConstructorInstance();
    }
  }

  public class ConstructorInstanceA : IConstructorA {
    public int get() {
      return 31;
    }

    [ServiceConstructor(typeof(IConstructorA))]
    public static IConstructorA create() {
      return new ConstructorInstanceA();
    }
  }

  public class ConstructorInstanceB : IConstructorB {
    public int get() {
      return 32;
    }

    [ServiceConstructor(typeof(IConstructorB))]
    public static IConstructorA create() {
      return new ConstructorInstanceA();
    }
  }

  public class SelectorInstance : ISelector {
    public int get() {
      return 4;
    }

    [ServiceSelector(typeof(ISelector))]
    public static Type selector() {
      return typeof(SelectorInstance);
    }
  }

  public interface IInvoke {
    bool Pass11 { get; }
    bool Pass21 { get; }
    bool Pass31 { get; }
    bool Pass12 { get; }
    bool Pass22 { get; }
    bool Pass32 { get; }
    bool TestProperty { get; set; }
    void Test11();
    int Test21();
    string Test31();
    void Test12(int i, string s);
    int Test22(int i, string s);
    string Test32(int i, string s);
    void TestException();
  }

  public class Invoke : IInvoke {
    public bool Pass11 { get; private set; } = false;
    public bool Pass21 { get; private set; } = false;
    public bool Pass31 { get; private set; } = false;
    public bool Pass12 { get; private set; } = false;
    public bool Pass22 { get; private set; } = false;
    public bool Pass32 { get; private set; } = false;
    public bool TestProperty { get; set; }

    public virtual void Test11() {
      Pass11 = true;
    }
    public virtual int Test21() {
      Pass21 = true;
      return 2;
    }
    public string Test31() {
      Pass31 = true;
      return "3";
    }
    public void Test12(int i, string s) {
      Pass12 = true;
    }
    public int Test22(int i, string s) {
      Pass22 = true;
      return 2;
    }
    public string Test32(int i, string s) {
      Pass32 = true;
      return "3";
    }
    public void TestException() {
      throw new Exception("test exception");
    }
  }

  public class Hook : InvokeHook {
    public int checkBefore { get; protected set; } = 0;
    public int checkAfter { get; protected set; } = 0;
    public int checkException { get; protected set; } = 0;

    public override bool InvokeBefore(IInvokeInfo info, params object[] args) {
      Assert.IsNotNull(info);
      var ret = base.InvokeBefore(info, args);
      switch (info.Name) {
        case "Test11":
        case "Test21":
        case "Test31":
          Assert.AreEqual(args.Length, 0);
          checkBefore++;
          break;
        case "Test12":
          Assert.AreEqual(args.Length, 2);
          Assert.AreEqual(args[0].GetType(), typeof(int));
          Assert.AreEqual(args[0], 1);
          Assert.AreEqual(args[1], "1");
          checkBefore++;
          break;
        case "Test22":
          Assert.AreEqual(args.Length, 2);
          Assert.AreEqual(args[0].GetType(), typeof(int));
          Assert.AreEqual(args[0], 2);
          Assert.AreEqual(args[1], "2");
          checkBefore++;
          break;
        case "Test32":
          Assert.AreEqual(args.Length, 2);
          Assert.AreEqual(args[0].GetType(), typeof(int));
          Assert.AreEqual(args[0], 3);
          Assert.AreEqual(args[1], "3");
          checkBefore++;
          break;
        case "TestException":
          checkBefore++;
          break;
        default:
          if (info.Name.StartsWith("get_")) {
            Assert.AreEqual(args.Length, 0);
            checkBefore++;
          } else if (info.Name.StartsWith("set_")) {
            Assert.AreEqual(args.Length, 1);
            Assert.AreEqual(args[0].GetType(), typeof(bool));
            Assert.IsTrue((bool)args[0]);
            checkBefore++;
          } else {
            Assert.Fail();
          }
          break;
      }
      return ret;
    }

    public override void InvokeAfter(IInvokeInfo info, object returnValue, params object[] args) {
      Assert.IsNotNull(info);
      base.InvokeAfter(info, returnValue, args);
      switch (info.Name) {
        case "Test11":
          Assert.AreEqual(args.Length, 0);
          Assert.IsNull(returnValue);
          checkAfter++;
          break;
        case "Test21":
          Assert.AreEqual(args.Length, 0);
          Assert.AreEqual(returnValue.GetType(), typeof(int));
          Assert.AreEqual(returnValue, 2);
          checkAfter++;
          break;
        case "Test31":
          Assert.AreEqual(args.Length, 0);
          Assert.AreEqual(returnValue.GetType(), typeof(string));
          Assert.AreEqual(returnValue, "3");
          checkAfter++;
          break;
        case "Test12":
          Assert.AreEqual(args.Length, 2);
          Assert.AreEqual(args[0].GetType(), typeof(int));
          Assert.AreEqual(args[1].GetType(), typeof(string));
          Assert.AreEqual(args[0], 1);
          Assert.AreEqual(args[1], "1");
          Assert.IsNull(returnValue);
          checkAfter++;
          break;
        case "Test22":
          Assert.AreEqual(args.Length, 2);
          Assert.AreEqual(args[0].GetType(), typeof(int));
          Assert.AreEqual(args[1].GetType(), typeof(string));
          Assert.AreEqual(args[0], 2);
          Assert.AreEqual(args[1], "2");
          Assert.AreEqual(returnValue.GetType(), typeof(int));
          Assert.AreEqual(returnValue, 2);
          checkAfter++;
          break;
        case "Test32":
          Assert.AreEqual(args.Length, 2);
          Assert.AreEqual(args[0].GetType(), typeof(int));
          Assert.AreEqual(args[1].GetType(), typeof(string));
          Assert.AreEqual(args[0], 3);
          Assert.AreEqual(args[1], "3");
          Assert.AreEqual(returnValue.GetType(), typeof(string));
          Assert.AreEqual(returnValue, "3");
          checkAfter++;
          break;
        case "TestException":
          Assert.AreEqual(args.Length, 0);
          checkAfter++;
          break;
        default:
          if (info.Name.StartsWith("get_")) {
            Assert.AreEqual(args.Length, 0);
            Assert.AreEqual(returnValue.GetType(), typeof(bool));
            checkAfter++;
          } else if (info.Name.StartsWith("set_")) {
            Assert.AreEqual(args.Length, 1);
            Assert.AreEqual(args[0].GetType(), typeof(bool));
            Assert.IsTrue((bool)args[0]);
            Assert.IsNull(returnValue);
            checkAfter++;
          } else {
            Assert.Fail();
          }
          break;
      }
    }

    public override void InvokeException(IInvokeInfo info, Exception e, params object[] args) {
      Assert.IsNotNull(info);
      Assert.IsNotNull(e);
      Assert.AreEqual(info.Name, "TestException");
      Assert.AreEqual(e.Message, "test exception");
      Assert.AreEqual(args.Length, 0);
      base.InvokeException(info, e, args);
      checkException++;
    }
  }

  public class HookSkip : Hook {
    public override bool InvokeBefore(IInvokeInfo info, params object[] args) {
      base.InvokeBefore(info, args);
      return false;
    }

    public override void InvokeAfter(IInvokeInfo info, object returnValue, params object[] args) {
      Assert.IsNotNull(info);
      switch (info.Name) {
        case "Test11":
          Assert.AreEqual(args.Length, 0);
          Assert.IsNull(returnValue);
          checkAfter++;
          break;
        case "Test21":
          Assert.AreEqual(args.Length, 0);
          Assert.AreEqual(returnValue.GetType(), typeof(int));
          Assert.AreEqual(returnValue, 0);
          checkAfter++;
          break;
        case "Test31":
          Assert.AreEqual(args.Length, 0);
          Assert.IsNull(returnValue);
          checkAfter++;
          break;
        case "Test12":
          Assert.AreEqual(args.Length, 2);
          Assert.AreEqual(args[0].GetType(), typeof(int));
          Assert.AreEqual(args[1].GetType(), typeof(string));
          Assert.AreEqual(args[0], 1);
          Assert.AreEqual(args[1], "1");
          Assert.IsNull(returnValue);
          checkAfter++;
          break;
        case "Test22":
          Assert.AreEqual(args.Length, 2);
          Assert.AreEqual(args[0].GetType(), typeof(int));
          Assert.AreEqual(args[1].GetType(), typeof(string));
          Assert.AreEqual(args[0], 2);
          Assert.AreEqual(args[1], "2");
          Assert.AreEqual(returnValue.GetType(), typeof(int));
          Assert.AreEqual(returnValue, 0);
          checkAfter++;
          break;
        case "Test32":
          Assert.AreEqual(args.Length, 2);
          Assert.AreEqual(args[0].GetType(), typeof(int));
          Assert.AreEqual(args[1].GetType(), typeof(string));
          Assert.AreEqual(args[0], 3);
          Assert.AreEqual(args[1], "3");
          Assert.IsNull(returnValue);
          checkAfter++;
          break;
        case "TestException":
          Assert.AreEqual(args.Length, 0);
          checkAfter++;
          break;
        default:
          if (info.Name.StartsWith("get_")) {
            Assert.AreEqual(args.Length, 0);
            Assert.AreEqual(returnValue.GetType(), typeof(bool));
            checkAfter++;
          } else if (info.Name.StartsWith("set_")) {
            Assert.AreEqual(args.Length, 1);
            Assert.AreEqual(args[0].GetType(), typeof(bool));
            Assert.IsTrue((bool)args[0]);
            Assert.IsNull(returnValue);
            checkAfter++;
          } else {
            Assert.Fail();
          }
          break;
      }
    }
  }

  [TestFixture]
  [Category("Soyo.Base")]
  internal class ServiceTest {
    [OneTimeSetUp]
    public void Init() {
      Service.RetireInstanceAll();
    }

    [Test]
    public void TestService() {
      // check
      var ret = typeof(IClass).IsAssignableFrom(typeof(ClassInstance));
      Assert.IsTrue(ret, "IClass is not assignable from ClassInstance");

      ret = typeof(IClass).IsAssignableFrom(typeof(IClass));
      Assert.IsTrue(ret, "IClass is not assignable from IClass");

      ret = typeof(IClass).IsAssignableFrom(null);
      Assert.IsFalse(ret, "IClass is assignable from null");

      var instance = new ClassInstance();

      ret = typeof(IClass).IsInstanceOfType(instance);
      Assert.IsTrue(ret, "IClass is not instance of type instance");

      ret = typeof(IClass).IsInstanceOfType(null);
      Assert.IsFalse(ret, "IClass is instance of type null");

      // class
      var iClass = Service<IClass>.Instance;
      Assert.IsNotNull(iClass, "service IClass attribute get null");

      var value = iClass.get();
      Assert.AreEqual(value, 2, "service IClass get is not 2");

      ret = Service<IClass>.HasInstance;
      Assert.IsTrue(ret, "service IClass has is not true");

      Service<IClass>.RetireInstance();

      ret = Service<IClass>.HasInstance;
      Assert.IsFalse(ret, "service IClass has is not false");

      // retire and get will recreate from service base data
      iClass = Service<IClass>.Instance;
      Assert.IsNotNull(iClass, "service IClass is null");

      value = iClass.get();
      Assert.AreEqual(value, 2, "service IClass get is not 2");

      ret = Service<IClass>.HasInstance;
      Assert.IsTrue(ret, "service IClass has is not true");

      Service<IClass>.RegisterInstance((IClass)null);

      ret = Service<IClass>.HasInstance;
      Assert.IsFalse(ret, "service IClass has is not false");

      // register null and get will get cache null
      iClass = Service<IClass>.Instance;
      Assert.IsNull(iClass, "service IClass is not null");

      Service<IClass>.RegisterInstance(new ClassInstance());

      ret = Service<IClass>.HasInstance;
      Assert.IsTrue(ret, "service IClass has is not true");

      iClass = Service<IClass>.Instance;
      Assert.IsNotNull(iClass, "service IClass is null");

      value = iClass.get();
      Assert.AreEqual(value, 2, "service IClass get is not 2");

      var iClassA = Service<IClassA>.Instance;
      Assert.IsNull(iClassA, "service IClassA attribute get not null");

      ret = Service.HasInstance<IClassA>();
      Assert.IsFalse(ret, "service IClass has is not false");

      Service.RegisterInstance(typeof(IClassA), new ClassInstance());
      iClass = Service<IClass>.Instance;
      Assert.IsNotNull(iClass, "service IClass is null");

      value = iClass.get();
      Assert.AreEqual(value, 2, "service IClass get is not 2");

      Service<IClass>.Retire();

      ret = Service<IClass>.HasInstance;
      Assert.IsFalse(ret, "service IClass has is not false");

      iClass = Service<IClass>.Instance;
      Assert.IsNull(iClass, "service IClass is not null");

      Service<IClass>.RegisterType<ClassInstance>();

      ret = Service<IClass>.HasInstance;
      Assert.IsFalse(ret, "service IClass has is not false");

      iClass = Service<IClass>.Instance;
      Assert.IsNotNull(iClass, "service IClass is null");

      value = iClass.get();
      Assert.AreEqual(value, 2, "service IClass get is not 2");

      // register error class and get null
      iClassA = Service<IClassA>.Instance;
      Assert.IsNull(iClassA, "service IClassA attribute get not null");

      Service<IClassA>.RegisterInstance(new ClassInstanceAA());

      iClassA = Service<IClassA>.Instance;
      Assert.IsNotNull(iClassA, "service IClassA attribute get null");

      value = iClassA.get();
      Assert.AreEqual(value, 22, "service IClassA get is not 22");

      Service<IClassA>.RetireInstance();

      ret = Service<IClassA>.HasInstance;
      Assert.IsFalse(ret, "service IClassA has is not false");

      // retire and get will recreate but has no service base data
      iClassA = Service<IClassA>.Instance;
      Assert.IsNull(iClassA, "service IClassA is not null");

      // class singleton
      var iClassS = Service<IClassS>.Instance;
      Assert.IsNotNull(iClassS, "service IClassS is not null");

      value = iClassS.get();
      Assert.AreEqual(value, 23, "service IClassS get is not 23");

      Assert.AreSame(iClassS, ClassInstanceS.Instance, "service IClassS is not same as singleton");

      // assembly
      var iAssembly = Service<IAssembly>.Instance;
      Assert.IsNotNull(iAssembly, "service IAssembly get null");

      value = iAssembly.get();
      Assert.AreEqual(value, 1, "service IAssembly get is not 1");

      // assembly attribute must be full type name, like "UnitTest.Base.IAssembly"
      var iAssemblyA = Service<IAssemblyA>.Instance;
      Assert.IsNull(iAssemblyA, "service IAssemblyA get not null");

      // constructor
      var iConstructor = Service<IConstructor>.Instance;
      Assert.IsNotNull(iConstructor, "service IConstructor get null");

      value = iConstructor.get();
      Assert.AreEqual(value, 3, "service IConstructor get is not 3");

      var iConstructorA = Service<IConstructorA>.Instance;
      Assert.IsNotNull(iConstructorA, "service IConstructorA get null");

      value = iConstructorA.get();
      Assert.AreEqual(value, 31, "service IConstructorA get is not 31");

      var iConstructorB = Service<IConstructorB>.Instance;
      Assert.IsNull(iConstructorB, "service IConstructorB attribute get not null");

      Service<IConstructorB>.RegisterInstance(new ConstructorInstanceB());

      iConstructorB = Service<IConstructorB>.Instance;
      Assert.IsNotNull(iConstructorB, "service IConstructorB get null");

      value = iConstructorB.get();
      Assert.AreEqual(value, 32, "service IConstructorA get is not 32");

      Service<IConstructorB>.RetireInstance();

      iConstructorB = Service<IConstructorB>.Instance;
      Assert.IsNull(iConstructorB, "service IConstructorB attribute get not null");

      Service<IConstructorA>.Retire();

      ret = Service<IConstructorA>.HasInstance;
      Assert.IsFalse(ret, "service IConstructorA has is not false");

      iConstructorA = Service<IConstructorA>.Instance;
      Assert.IsNull(iConstructorA, "service IConstructorA is not null");

      Service<IConstructorA>.RegisterConstructor(ConstructorInstanceA.create);

      ret = Service<IConstructorA>.HasInstance;
      Assert.IsTrue(ret, "service IConstructorA has is not true");

      iConstructorA = Service<IConstructorA>.Instance;
      Assert.IsNotNull(iConstructorA, "service IConstructorA is null");

      value = iConstructorA.get();
      Assert.AreEqual(value, 31, "service IConstructorA get is not 31");

      var iSelector = Service<ISelector>.Instance;
      Assert.IsNotNull(iSelector, "service ISelector get null");

      value = iSelector.get();
      Assert.AreEqual(value, 4, "service ISelector get is not 4");

      Service<ISelector>.Retire();

      ret = Service<ISelector>.HasInstance;
      Assert.IsFalse(ret, "service ISelector has is not false");

      iSelector = Service<ISelector>.Instance;
      Assert.IsNull(iSelector, "service ISelector is not null");

      Service<ISelector>.RegisterSelector(SelectorInstance.selector);

      ret = Service<ISelector>.HasInstance;
      Assert.IsFalse(ret, "service ISelector has is not false");

      iSelector = Service<ISelector>.Instance;
      Assert.IsNotNull(iSelector, "service ISelector is null");

      value = iSelector.get();
      Assert.AreEqual(value, 4, "service ISelector get is not 4");
    }

    [Test]
    public void TestServiceProxy() {
      var rc = Service<IInvoke>.RegisterType<Invoke>();
      Assert.IsTrue(rc);
      var hook = new Hook();
      rc = Service<IInvoke>.RegisterHook(hook);
      Assert.IsTrue(rc);
      var value = Service<IInvoke>.New();
      value.Test11();
      value.Test21();
      value.Test31();
      value.Test12(1, "1");
      value.Test22(2, "2");
      value.Test32(3, "3");
      value.TestException();
      value.TestProperty = true;
      Assert.IsTrue(value.Pass11);
      Assert.IsTrue(value.Pass21);
      Assert.IsTrue(value.Pass31);
      Assert.IsTrue(value.Pass12);
      Assert.IsTrue(value.Pass22);
      Assert.IsTrue(value.Pass32);
      Assert.IsTrue(value.TestProperty);
      Assert.AreEqual(hook.checkBefore, 15);
      Assert.AreEqual(hook.checkAfter, 15);
      Assert.AreEqual(hook.checkException, 1);

      hook = new HookSkip();
      rc = Service<IInvoke>.RegisterHook(hook);
      Assert.IsTrue(rc);
      value = Service<IInvoke>.New();
      value.Test11();
      value.Test21();
      value.Test31();
      value.Test12(1, "1");
      value.Test22(2, "2");
      value.Test32(3, "3");
      value.TestException();
      value.TestProperty = true;
      Assert.IsFalse(value.Pass11);
      Assert.IsFalse(value.Pass21);
      Assert.IsFalse(value.Pass31);
      Assert.IsFalse(value.Pass12);
      Assert.IsFalse(value.Pass22);
      Assert.IsFalse(value.Pass32);
      Assert.IsFalse(value.TestProperty);
      Assert.AreEqual(hook.checkBefore, 15);
      Assert.AreEqual(hook.checkAfter, 15);
      Assert.AreEqual(hook.checkException, 0);
    }
  }
}