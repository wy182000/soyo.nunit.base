using System;
using System.Collections.Generic;

using Soyo.Base;

using NUnit.Framework;

namespace UnitTest.Base.Util {
  [TestFixture]
  [Category("Soyo.Base")]
  internal class NodeTest {
    [OneTimeSetUp]
    public void Init() {
    }

    private void checkInitialize(INode node) {
      Assert.IsNotNull(node);
      Assert.IsNull(node.Parent);
      Assert.IsNotNull(node.ChildSet);
      Assert.AreEqual(node.ChildCount, 0);
    }

    private INode createNode() {
      var node =  new Node();
      checkInitialize(node);
      return node;
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

    private void checkApi(INodeHook hook) {
      INode node = Service<INode>.New();
      INode parent = Service<INode>.New();

      node.SetParent(parent);
      checkParent(node, parent);
      checkChild(parent, node);
      checkNodeInfo(node, hook, 1, 0, 0);
      checkNodeInfo(parent, hook, 0, 1, 0);

      node.SetParent(null);
      checkParent(node, null);
      checkNotChild(parent, node);
      checkInitialize(node);
      checkInitialize(parent);
      checkNodeInfo(node, hook, 2, 0, 0);
      checkNodeInfo(parent, hook, 0, 1, 1);

      parent.AddChild(node);
      checkParent(node, parent);
      checkChild(parent, node);
      checkNodeInfo(node, hook, 3, 0, 0);
      checkNodeInfo(parent, hook, 0, 2, 1);

      parent.RemoveChild(node);
      checkParent(node, null);
      checkNotChild(parent, node);
      checkInitialize(node);
      checkInitialize(parent);
      checkNodeInfo(node, hook, 4, 0, 0);
      checkNodeInfo(parent, hook, 0, 2, 2);

      node.SetParent(parent);
      checkParent(node, parent);
      checkChild(parent, node);
      checkNodeInfo(node, hook, 5, 0, 0);
      checkNodeInfo(parent, hook, 0, 3, 2);

      node.Clear();
      checkParent(node, null);
      checkNotChild(parent, node);
      checkInitialize(node);
      checkInitialize(parent);
      checkNodeInfo(node, hook, 6, 0, 0);
      checkNodeInfo(parent, hook, 0, 3, 3);

      parent.AddChild(node);
      checkParent(node, parent);
      checkChild(parent, node);
      checkNodeInfo(node, hook, 7, 0, 0);
      checkNodeInfo(parent, hook, 0, 4, 3);

      parent.Clear();
      checkParent(node, null);
      checkNotChild(parent, node);
      checkInitialize(node);
      checkInitialize(parent);
      checkNodeInfo(node, hook, 8, 0, 0);
      checkNodeInfo(parent, hook, 0, 4, 4);
    }

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

    [Test]
    public void TestNode() {
      var rc = Service<INode>.RegisterType<Node>();
      Assert.IsTrue(rc);

      var hook = new INodeHook();
      rc = Service<INode>.RegisterHook(hook);
      Assert.IsTrue(rc);

      checkApi(hook);
    }
  }
}
