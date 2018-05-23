using System;
using System.Collections;
using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class CoroutineTst {
    private string checkString = string.Empty;
    private int checkCount = 0;

    private IEnumerator testEnumerator() {
      checkString = "testEnumerator1";
      yield return 1;
      checkString = "testEnumerator2";
      yield return true;
      checkString = "testEnumerator3";
      yield return "end";
      checkString = "testEnumerator4";
    }

    private IEnumerator nestEnumerator() {
      checkString = "nestEnumerator1";
      yield return true;
      checkString = "nestEnumerator2";
      yield return testEnumerator();
      checkString = "nestEnumerator3";
      yield return new Coroutine(testEnumerator());
      checkString = "nestEnumerator4";
      yield break;
    }

    private bool conditionFunction() {
      return checkCount++ > 3;
    }

    [Test]
    public void TestCoroutine() {
      checkString = string.Empty;

      var coroutine = new Coroutine(testEnumerator());
      Assert.IsNotNull(coroutine);
      Assert.IsNull(coroutine.Current);
      Assert.AreEqual(checkString, string.Empty);

      var rc = coroutine.MoveNext();
      Assert.IsTrue(rc);
      Assert.AreEqual(checkString, "testEnumerator1");
      Assert.AreEqual(coroutine.Current, 1);

      rc = coroutine.MoveNext();
      Assert.IsTrue(rc);
      Assert.AreEqual(checkString, "testEnumerator2");
      Assert.AreEqual(coroutine.Current, true);

      rc = coroutine.MoveNext();
      Assert.IsTrue(rc);
      Assert.AreEqual(checkString, "testEnumerator3");
      Assert.AreEqual(coroutine.Current, "end");

      rc = coroutine.MoveNext();
      Assert.IsFalse(rc);
      Assert.AreEqual(checkString, "testEnumerator4");
      Assert.AreEqual(coroutine.Current, "end");

      checkString = string.Empty;

      coroutine = new Coroutine(nestEnumerator());
      Assert.IsNotNull(coroutine);
      Assert.IsNull(coroutine.Current);
      Assert.AreEqual(checkString, string.Empty);

      rc = coroutine.MoveNext();
      Assert.IsTrue(rc);
      Assert.AreEqual(checkString, "nestEnumerator1");
      Assert.AreEqual(coroutine.Current, true);

      rc = coroutine.MoveNext();
      Assert.IsTrue(rc);
      Assert.AreEqual(checkString, "nestEnumerator2");
      IEnumerator enumerator = coroutine.Current as IEnumerator;
      Assert.IsNotNull(enumerator);
      Assert.IsNull(enumerator.Current);

      rc = coroutine.MoveNext();
      Assert.IsTrue(rc);
      Assert.AreEqual(checkString, "testEnumerator1");
      enumerator = coroutine.Current as IEnumerator;
      Assert.IsNotNull(enumerator);
      Assert.AreEqual(enumerator.Current, 1);

      rc = coroutine.MoveNext();
      Assert.IsTrue(rc);
      Assert.AreEqual(checkString, "testEnumerator2");
      enumerator = coroutine.Current as IEnumerator;
      Assert.IsNotNull(enumerator);
      Assert.AreEqual(enumerator.Current, true);

      rc = coroutine.MoveNext();
      Assert.IsTrue(rc);
      Assert.AreEqual(checkString, "testEnumerator3");
      enumerator = coroutine.Current as IEnumerator;
      Assert.IsNotNull(enumerator);
      Assert.AreEqual(enumerator.Current, "end");

      // checkString = testEnumerator4 run, but replace by next routine 
      rc = coroutine.MoveNext();
      Assert.IsTrue(rc);
      Assert.AreEqual(checkString, "nestEnumerator3");
      var co = coroutine.Current as Coroutine;
      Assert.IsNotNull(co);
      Assert.IsNull(co.Current);

      rc = coroutine.MoveNext();
      Assert.IsTrue(rc);
      Assert.AreEqual(checkString, "testEnumerator1");
      co = coroutine.Current as Coroutine;
      Assert.IsNotNull(co);
      Assert.AreEqual(co.Current, 1);

      rc = coroutine.MoveNext();
      Assert.IsTrue(rc);
      Assert.AreEqual(checkString, "testEnumerator2");
      co = coroutine.Current as Coroutine;
      Assert.IsNotNull(co);
      Assert.AreEqual(co.Current, true);

      rc = coroutine.MoveNext();
      Assert.IsTrue(rc);
      Assert.AreEqual(checkString, "testEnumerator3");
      co = coroutine.Current as Coroutine;
      Assert.IsNotNull(co);
      Assert.AreEqual(co.Current, "end");

      // checkString = testEnumerator4 run, but replace by next routine 
      rc = coroutine.MoveNext();
      Assert.IsFalse(rc);
      Assert.AreEqual(checkString, "nestEnumerator4");
      co = coroutine.Current as Coroutine;
      Assert.IsNotNull(co);
      Assert.AreEqual(co.Current, "end");

      coroutine = new Coroutine(new WaitCoroutine(conditionFunction));
      rc = coroutine.MoveNext();
      Assert.IsTrue(rc);
      Assert.AreEqual(checkCount, 1);
      rc = coroutine.MoveNext();
      Assert.IsTrue(rc);
      Assert.AreEqual(checkCount, 2);
      rc = coroutine.MoveNext();
      Assert.IsTrue(rc);
      Assert.AreEqual(checkCount, 3);
      rc = coroutine.MoveNext();
      Assert.IsTrue(rc);
      Assert.AreEqual(checkCount, 4);
      rc = coroutine.MoveNext();
      Assert.IsFalse(rc);
      Assert.AreEqual(checkCount, 5);

      coroutine = new Coroutine(new WaitTimeCoroutine(10));
      var count = 0;
      while (coroutine.MoveNext()) {
        count++;
        Assert.LessOrEqual(0, (double)coroutine.Current);
        Assert.LessOrEqual((double)coroutine.Current, 10);
      }
      Assert.LessOrEqual((double)coroutine.Current, 0);
      Assert.Less(0, count);
    }

    [Test]
    public void TestCoroutineManager() {
      checkString = string.Empty;

      var coroutine = CoroutineManager.Instance.StartCoroutine(nestEnumerator());

      CoroutineManager.Instance.UpdateCocroutine();
      Assert.AreEqual(checkString, "nestEnumerator1");
      Assert.AreEqual(coroutine.Current, true);

      CoroutineManager.Instance.UpdateCocroutine();
      Assert.AreEqual(checkString, "nestEnumerator2");
      IEnumerator enumerator = coroutine.Current as IEnumerator;
      Assert.IsNotNull(enumerator);
      Assert.IsNull(enumerator.Current);

      CoroutineManager.Instance.UpdateCocroutine();
      Assert.AreEqual(checkString, "testEnumerator1");
      enumerator = coroutine.Current as IEnumerator;
      Assert.IsNotNull(enumerator);
      Assert.AreEqual(enumerator.Current, 1);

      CoroutineManager.Instance.UpdateCocroutine();
      Assert.AreEqual(checkString, "testEnumerator2");
      enumerator = coroutine.Current as IEnumerator;
      Assert.IsNotNull(enumerator);
      Assert.AreEqual(enumerator.Current, true);

      CoroutineManager.Instance.UpdateCocroutine();
      Assert.AreEqual(checkString, "testEnumerator3");
      enumerator = coroutine.Current as IEnumerator;
      Assert.IsNotNull(enumerator);
      Assert.AreEqual(enumerator.Current, "end");

      // checkString = testEnumerator4 run, but replace by next routine 
      CoroutineManager.Instance.UpdateCocroutine();
      Assert.AreEqual(checkString, "nestEnumerator3");
      var co = coroutine.Current as Coroutine;
      Assert.IsNotNull(co);
      Assert.IsNull(co.Current);

      CoroutineManager.Instance.UpdateCocroutine();
      Assert.AreEqual(checkString, "testEnumerator1");
      co = coroutine.Current as Coroutine;
      Assert.IsNotNull(co);
      Assert.AreEqual(co.Current, 1);

      CoroutineManager.Instance.UpdateCocroutine();
      Assert.AreEqual(checkString, "testEnumerator2");
      co = coroutine.Current as Coroutine;
      Assert.IsNotNull(co);
      Assert.AreEqual(co.Current, true);

      CoroutineManager.Instance.UpdateCocroutine();
      Assert.AreEqual(checkString, "testEnumerator3");
      co = coroutine.Current as Coroutine;
      Assert.IsNotNull(co);
      Assert.AreEqual(co.Current, "end");

      // checkString = testEnumerator4 run, but replace by next routine 
      CoroutineManager.Instance.UpdateCocroutine();
      Assert.AreEqual(checkString, "nestEnumerator4");
      co = coroutine.Current as Coroutine;
      Assert.IsNotNull(co);
      Assert.AreEqual(co.Current, "end");
    }
  }
}
