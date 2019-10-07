using System;
using System.Collections.Generic;

using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class NodeTest {
    public class INodeHook : InvokeHook {
      public class NodeInfo {
        public int parentChangedCount = 0;
        public int addChildCount = 0;
        public int removeChildCount = 0;

        public void Reset() {
          parentChangedCount = 0;
          addChildCount = 0;
          removeChildCount = 0;
        }
      }

      public Dictionary<INode, NodeInfo> nodeInfoSet_ = new Dictionary<INode, NodeInfo>();

      public NodeInfo GetNodeInfo(INode node) {
        if (node == null) return null;
        NodeInfo info;
        nodeInfoSet_.TryGetValue(node, out info);
        return info;
      }

      public void RemoveNode(INode node) {
        nodeInfoSet_.Remove(node);
      }

      public void ResetNode(INode node) {
        var info = GetNodeInfo(node);
        info?.Reset();
      }

      public void Clear() {
        nodeInfoSet_.Clear();
      }

      public override void InvokeAfter(IInvokeInfo info, object returnValue, params object[] args) {
        Assert.IsNotNull(info);
        var nodeInfo = getNodeInfo((INode)info.Target);
        Assert.IsNotNull(nodeInfo);
        base.InvokeAfter(info, returnValue, args);
        switch (info.Name) {
          case "OnParentChanged":
            nodeInfo.parentChangedCount++;
            break;

          case "OnChildAdd":
            nodeInfo.addChildCount++;
            break;

          case "OnChildRemove":
            nodeInfo.removeChildCount++;
            break;
        }
      }

      private NodeInfo getNodeInfo(INode node) {
        if (node == null) return null;

        NodeInfo info;
        if (nodeInfoSet_.TryGetValue(node, out info) == false) {
          info = new NodeInfo();
          nodeInfoSet_.Add(node, info);
        }
        return info;
      }

    }

    [OneTimeSetUp]
    public void Init() {
    }

    private void checkInitialize(INode node) {
      Assert.IsNotNull(node);
      Assert.IsNull(node.Parent);
      Assert.IsNotNull(node.ChildSet);
      Assert.AreEqual(node.ChildCount, 0);
    }

    private void checkParent(INode child, INode parent) {
      Assert.IsNotNull(child);
      Assert.AreEqual(child.Parent, parent);
    }

    private void checkChild(INode parent, INode child) {
      Assert.IsNotNull(parent);
      Assert.NotZero(parent.ChildCount);
      Assert.IsNotNull(child);
      Assert.IsTrue(parent.Contains(child));
    }

    private void checkNotChild(INode parent, INode child) {
      Assert.IsNotNull(parent);
      Assert.IsNotNull(child);
      Assert.IsFalse(parent.Contains(child));
    }

    private void checkNodeInfo(INode node, INodeHook hook, int parentChangedCount, int addChildCount, int removeChildCount) {
      var info = hook.GetNodeInfo(node);
      Assert.IsNotNull(info);
      Assert.AreEqual(info.parentChangedCount, parentChangedCount);
      Assert.AreEqual(info.addChildCount, addChildCount);
      Assert.AreEqual(info.removeChildCount, removeChildCount);
    }

    private void checkINodeApi<T>() where T : class, INode {
      var hook = new INodeHook();
      var rc = Service<T>.RegisterHook(hook);
      Assert.IsTrue(rc);

      T child = Service<T>.New();
      checkInitialize(child);
      T parent = Service<T>.New();
      checkInitialize(parent);

      child.SetParent(parent);
      checkParent(child, parent);
      checkChild(parent, child);
      checkNodeInfo(child, hook, 1, 0, 0);
      checkNodeInfo(parent, hook, 0, 1, 0);

      child.SetParent(null);
      checkParent(child, null);
      checkNotChild(parent, child);
      checkInitialize(child);
      checkInitialize(parent);
      checkNodeInfo(child, hook, 2, 0, 0);
      checkNodeInfo(parent, hook, 0, 1, 1);

      parent.AddChild(child);
      checkParent(child, parent);
      checkChild(parent, child);
      checkNodeInfo(child, hook, 3, 0, 0);
      checkNodeInfo(parent, hook, 0, 2, 1);

      parent.RemoveChild(child);
      checkParent(child, null);
      checkNotChild(parent, child);
      checkInitialize(child);
      checkInitialize(parent);
      checkNodeInfo(child, hook, 4, 0, 0);
      checkNodeInfo(parent, hook, 0, 2, 2);

      child.SetParent(parent);
      checkParent(child, parent);
      checkChild(parent, child);
      checkNodeInfo(child, hook, 5, 0, 0);
      checkNodeInfo(parent, hook, 0, 3, 2);

      child.Clear();
      checkParent(child, null);
      checkNotChild(parent, child);
      checkInitialize(child);
      checkInitialize(parent);
      checkNodeInfo(child, hook, 6, 0, 0);
      checkNodeInfo(parent, hook, 0, 3, 3);

      parent.AddChild(child);
      checkParent(child, parent);
      checkChild(parent, child);
      checkNodeInfo(child, hook, 7, 0, 0);
      checkNodeInfo(parent, hook, 0, 4, 3);

      T subNode = Service<T>.New();
      checkInitialize(subNode);

      child.AddChild(subNode);
      checkParent(subNode, child);
      checkChild(child, subNode);
      checkNodeInfo(subNode, hook, 1, 0, 0);
      checkNodeInfo(child, hook, 7, 1, 0);

      parent.Clear();
      checkParent(child, null);
      checkParent(subNode, null);
      checkNotChild(parent, child);
      checkNotChild(child, subNode);
      checkInitialize(child);
      checkInitialize(parent);
      checkInitialize(subNode);
      checkNodeInfo(child, hook, 8, 1, 1);
      checkNodeInfo(parent, hook, 0, 4, 4);
      checkNodeInfo(subNode, hook, 2, 0, 0);
    }

    private void checkINodeAsyncApi<T>() where T : class, INodeAsync, IThreadObject {
      var childThread = Thread.CreateThread();
      var parentThread = Thread.CreateThread();

      var hook = new INodeHook();
      var rc = Service<T>.RegisterHook(hook);
      Assert.IsTrue(rc);

      T child = Service<T>.New();
      checkInitialize(child);
      child.Attach(childThread);

      T parent = Service<T>.New();
      checkInitialize(parent);
      parent.Attach(parentThread);

      var task = child.SetParentAsync(parent);
      Assert.IsNotNull(task);
      rc = task.Wait(1000);
      Assert.IsTrue(rc);
      checkParent(child, parent);
      checkNodeInfo(child, hook, 1, 0, 0);
      Thread.Wait(() => parent.State == NodeAsyncState.Idle, 1000);
      checkChild(parent, child);
      checkNodeInfo(parent, hook, 0, 1, 0);

      task = child.SetParentAsync(null);
      Assert.IsNotNull(task);
      rc = task.Wait(1000);
      Assert.IsTrue(rc);
      checkParent(child, null);
      checkInitialize(child);
      checkNodeInfo(child, hook, 2, 0, 0);
      Thread.Wait(() => parent.State == NodeAsyncState.Idle, 1000);
      checkNotChild(parent, child);
      checkInitialize(parent);
      checkNodeInfo(parent, hook, 0, 1, 1);

      task = parent.AddChildAsync(child);
      Assert.IsNotNull(task);
      rc = task.Wait(1000);
      Assert.IsTrue(rc);
      checkChild(parent, child);
      checkNodeInfo(parent, hook, 0, 2, 1);
      Thread.Wait(() => child.State == NodeAsyncState.Idle, 1000);
      checkParent(child, parent);
      checkNodeInfo(child, hook, 3, 0, 0);

      task = parent.RemoveChildAsync(child);
      Assert.IsNotNull(task);
      rc = task.Wait(1000);
      Assert.IsTrue(rc);
      checkNotChild(parent, child);
      checkInitialize(parent);
      checkNodeInfo(parent, hook, 0, 2, 2);
      Thread.Wait(() => child.State == NodeAsyncState.Idle, 1000);
      checkParent(child, null);
      checkInitialize(child);
      checkNodeInfo(child, hook, 4, 0, 0);

      task = child.SetParentAsync(parent);
      Assert.IsNotNull(task);
      rc = task.Wait(1000);
      Assert.IsTrue(rc);
      checkParent(child, parent);
      checkNodeInfo(child, hook, 5, 0, 0);
      Thread.Wait(() => parent.State == NodeAsyncState.Idle, 1000);
      checkChild(parent, child);
      checkNodeInfo(parent, hook, 0, 3, 2);

      task = child.ClearAsync();
      Assert.IsNotNull(task);
      rc = task.Wait(1000);
      Assert.IsTrue(rc);
      checkParent(child, null);
      checkInitialize(child);
      checkNodeInfo(child, hook, 6, 0, 0);
      Thread.Wait(() => parent.State == NodeAsyncState.Idle, 1000);
      checkNotChild(parent, child);
      checkInitialize(parent);
      checkNodeInfo(parent, hook, 0, 3, 3);

      task = parent.AddChildAsync(child);
      Assert.IsNotNull(task);
      rc = task.Wait(1000);
      Assert.IsTrue(rc);
      checkChild(parent, child);
      checkNodeInfo(parent, hook, 0, 4, 3);
      Thread.Wait(() => child.State == NodeAsyncState.Idle, 1000);
      checkParent(child, parent);
      checkNodeInfo(child, hook, 7, 0, 0);

      T subChild = Service<T>.New();
      checkInitialize(subChild);

      task = child.AddChildAsync(subChild);
      Assert.IsNotNull(task);
      rc = task.Wait(1000);
      Assert.IsTrue(rc);
      checkChild(child, subChild);
      checkNodeInfo(child, hook, 7, 1, 0);
      checkParent(subChild, child);
      checkNodeInfo(subChild, hook, 1, 0, 0);

      task = parent.ClearAsync();
      Assert.IsNotNull(task);
      rc = task.Wait(1000);
      Assert.IsTrue(rc);
      checkNotChild(parent, child);
      checkInitialize(parent);
      checkNodeInfo(parent, hook, 0, 4, 4);
      Thread.Wait(() => child.State == NodeAsyncState.Idle, 1000);
      checkParent(child, null);
      checkNotChild(child, subChild);
      checkInitialize(child);
      checkNodeInfo(child, hook, 8, 1, 1);
      checkParent(subChild, null);
      checkInitialize(subChild);
      checkNodeInfo(subChild, hook, 2, 0, 0);

      childThread.Stop();
      parentThread.Stop();
    }

    [Test]
    public void TestNode() {
      var rc = Service<INode>.RegisterType<Node>();
      Assert.IsTrue(rc);

      checkINodeApi<INode>();

      rc = Service<INodeAsync>.RegisterType<NodeAsync>();
      Assert.IsTrue(rc);

      checkINodeApi<INodeAsync>();
      checkINodeAsyncApi<INodeAsync>();
    }
  }
}
