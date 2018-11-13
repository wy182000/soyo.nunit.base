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
      var iClass = Service<IClass>.Get();
      Assert.IsNotNull(iClass, "service IClass attribute get null");

      var value = iClass.get();
      Assert.AreEqual(value, 2, "service IClass get is not 2");

      ret = Service<IClass>.Has();
      Assert.IsTrue(ret, "service IClass has is not true");

      Service<IClass>.RetireInstance();

      ret = Service<IClass>.Has();
      Assert.IsFalse(ret, "service IClass has is not false");

      // retire and get will recreate from service base data
      iClass = Service<IClass>.Get();
      Assert.IsNotNull(iClass, "service IClass is null");

      value = iClass.get();
      Assert.AreEqual(value, 2, "service IClass get is not 2");

      ret = Service<IClass>.Has();
      Assert.IsTrue(ret, "service IClass has is not true");

      Service<IClass>.RegisterInstance((IClass)null);

      ret = Service<IClass>.Has();
      Assert.IsFalse(ret, "service IClass has is not false");

      // register null and get will get cache null
      iClass = Service<IClass>.Get();
      Assert.IsNull(iClass, "service IClass is not null");

      Service<IClass>.RegisterInstance(new ClassInstance());

      ret = Service<IClass>.Has();
      Assert.IsTrue(ret, "service IClass has is not true");

      iClass = Service<IClass>.Get();
      Assert.IsNotNull(iClass, "service IClass is null");

      value = iClass.get();
      Assert.AreEqual(value, 2, "service IClass get is not 2");

      var iClassA = Service<IClassA>.Get();
      Assert.IsNull(iClassA, "service IClassA attribute get not null");

      ret = Service.Has<IClassA>();
      Assert.IsFalse(ret, "service IClass has is not false");

      Service.RegisterInstance(typeof(IClassA), new ClassInstance());
      iClass = Service<IClass>.Get();
      Assert.IsNotNull(iClass, "service IClass is null");

      value = iClass.get();
      Assert.AreEqual(value, 2, "service IClass get is not 2");

      Service<IClass>.Retire();

      ret = Service<IClass>.Has();
      Assert.IsFalse(ret, "service IClass has is not false");

      iClass = Service<IClass>.Get();
      Assert.IsNull(iClass, "service IClass is not null");

      Service<IClass>.RegisterType<ClassInstance>();

      ret = Service<IClass>.Has();
      Assert.IsFalse(ret, "service IClass has is not false");

      iClass = Service<IClass>.Get();
      Assert.IsNotNull(iClass, "service IClass is null");

      value = iClass.get();
      Assert.AreEqual(value, 2, "service IClass get is not 2");

      // register error class and get null
      iClassA = Service<IClassA>.Get();
      Assert.IsNull(iClassA, "service IClassA attribute get not null");

      Service<IClassA>.RegisterInstance(new ClassInstanceAA());

      iClassA = Service<IClassA>.Get();
      Assert.IsNotNull(iClassA, "service IClassA attribute get null");

      value = iClassA.get();
      Assert.AreEqual(value, 22, "service IClassA get is not 22");

      Service<IClassA>.RetireInstance();

      ret = Service<IClassA>.Has();
      Assert.IsFalse(ret, "service IClassA has is not false");

      // retire and get will recreate but has no service base data
      iClassA = Service<IClassA>.Get();
      Assert.IsNull(iClassA, "service IClassA is not null");

      // class singleton
      var iClassS = Service<IClassS>.Get();
      Assert.IsNotNull(iClassS, "service IClassS is not null");

      value = iClassS.get();
      Assert.AreEqual(value, 23, "service IClassS get is not 23");

      Assert.AreSame(iClassS, ClassInstanceS.Instance, "service IClassS is not same as singleton");

      // assembly
      var iAssembly = Service<IAssembly>.Get();
      Assert.IsNotNull(iAssembly, "service IAssembly get null");

      value = iAssembly.get();
      Assert.AreEqual(value, 1, "service IAssembly get is not 1");

      // assembly attribute must be full type name, like "UnitTest.Base.IAssembly"
      var iAssemblyA = Service<IAssemblyA>.Get();
      Assert.IsNull(iAssemblyA, "service IAssemblyA get not null");

      // constructor
      var iConstructor = Service<IConstructor>.Get();
      Assert.IsNotNull(iConstructor, "service IConstructor get null");

      value = iConstructor.get();
      Assert.AreEqual(value, 3, "service IConstructor get is not 3");

      var iConstructorA = Service<IConstructorA>.Get();
      Assert.IsNotNull(iConstructorA, "service IConstructorA get null");

      value = iConstructorA.get();
      Assert.AreEqual(value, 31, "service IConstructorA get is not 31");

      var iConstructorB = Service<IConstructorB>.Get();
      Assert.IsNull(iConstructorB, "service IConstructorB attribute get not null");

      Service<IConstructorB>.RegisterInstance(new ConstructorInstanceB());

      iConstructorB = Service<IConstructorB>.Get();
      Assert.IsNotNull(iConstructorB, "service IConstructorB get null");

      value = iConstructorB.get();
      Assert.AreEqual(value, 32, "service IConstructorA get is not 32");

      Service<IConstructorB>.RetireInstance();

      iConstructorB = Service<IConstructorB>.Get();
      Assert.IsNull(iConstructorB, "service IConstructorB attribute get not null");

      Service<IConstructorA>.Retire();

      ret = Service<IConstructorA>.Has();
      Assert.IsFalse(ret, "service IConstructorA has is not false");

      iConstructorA = Service<IConstructorA>.Get();
      Assert.IsNull(iConstructorA, "service IConstructorA is not null");

      Service<IConstructorA>.RegisterConstructor(ConstructorInstanceA.create);

      ret = Service<IConstructorA>.Has();
      Assert.IsTrue(ret, "service IConstructorA has is not true");

      iConstructorA = Service<IConstructorA>.Get();
      Assert.IsNotNull(iConstructorA, "service IConstructorA is null");

      value = iConstructorA.get();
      Assert.AreEqual(value, 31, "service IConstructorA get is not 31");

      var iSelector = Service<ISelector>.Get();
      Assert.IsNotNull(iSelector, "service ISelector get null");

      value = iSelector.get();
      Assert.AreEqual(value, 4, "service ISelector get is not 4");

      Service<ISelector>.Retire();

      ret = Service<ISelector>.Has();
      Assert.IsFalse(ret, "service ISelector has is not false");

      iSelector = Service<ISelector>.Get();
      Assert.IsNull(iSelector, "service ISelector is not null");

      Service<ISelector>.RegisterSelector(SelectorInstance.selector);

      ret = Service<ISelector>.Has();
      Assert.IsFalse(ret, "service ISelector has is not false");

      iSelector = Service<ISelector>.Get();
      Assert.IsNotNull(iSelector, "service ISelector is null");

      value = iSelector.get();
      Assert.AreEqual(value, 4, "service ISelector get is not 4");
    }

    [Test]
    public void TestServiceBase() {
    }
  }
}