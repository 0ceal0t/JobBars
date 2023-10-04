using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Numerics;

namespace JobBars.Helper {
    public static unsafe partial class AtkHelper {
        public static unsafe Vector2 GetNodePosition(AtkResNode* node) {
            var pos = new Vector2(node->X, node->Y);
            var par = node->ParentNode;
            while (par != null) {
                pos *= new Vector2(par->ScaleX, par->ScaleY);
                pos += new Vector2(par->X, par->Y);
                par = par->ParentNode;
            }
            return pos;
        }

        public static unsafe Vector2 GetNodeScale(AtkResNode* node) {
            if (node == null) return new Vector2(1, 1);
            var scale = new Vector2(node->ScaleX, node->ScaleY);
            while (node->ParentNode != null) {
                node = node->ParentNode;
                scale *= new Vector2(node->ScaleX, node->ScaleY);
            }
            return scale;
        }

        public static void Hide(AtkResNode* node) {
            node->NodeFlags &= ~NodeFlags.Visible;
            node->DrawFlags |= 0x1;
        }

        public static void Show(AtkResNode* node) {
            node->NodeFlags |= NodeFlags.Visible;
            node->DrawFlags |= 0x1;
        }

        public static void Update(AtkResNode* node) {
            node->DrawFlags |= 0x1;
        }

        public static void SetVisibility(AtkResNode* node, bool visiblity) {
            if (visiblity) Show(node);
            else Hide(node);
        }

        public static void SetSize(AtkResNode* node, int? width, int? height) {
            if (width != null && width >= ushort.MinValue && width <= ushort.MaxValue) node->Width = (ushort)width.Value;
            if (height != null && height >= ushort.MinValue && height <= ushort.MaxValue) node->Height = (ushort)height.Value;
            node->DrawFlags |= 0x1;
        }

        public static void SetPosition(AtkResNode* node, float? x, float? y) {
            if (x != null) node->X = x.Value;
            if (y != null) node->Y = y.Value;
            node->DrawFlags |= 0x1;
        }

        public static void SetScale(AtkResNode* atkUnitBase, float? scaleX, float? scaleY) {
            atkUnitBase->ScaleX = scaleX.Value;
            atkUnitBase->ScaleY = scaleY.Value;
            atkUnitBase->DrawFlags |= 0x1;
            atkUnitBase->DrawFlags |= 0x4;
        }

        public static void SetRotation(AtkResNode* node, float rotation) {
            node->Rotation = rotation;
            node->DrawFlags |= 0x1;
        }

        public static void Link(AtkResNode* next, AtkResNode* prev) {
            if (next == null || prev == null) return;
            next->PrevSiblingNode = prev;
            prev->NextSiblingNode = next;
        }

        public static void Attach(AtkUnitBase parent, AtkResNode* child) => Attach(parent.RootNode, child);

        public static void Attach(AtkUnitBase* parent, AtkResNode* child) => Attach(parent->RootNode, child);

        public static void Attach(AtkResNode* rootNode, AtkResNode* child) {
            var lastNode = rootNode->ChildNode;
            if (lastNode == null) {
                return;
            }

            while (lastNode->PrevSiblingNode != null) {
                if (lastNode == child) {
                    return;
                }
                lastNode = lastNode->PrevSiblingNode;
            }

            if (lastNode == child) {
                return;
            }

            Link(lastNode, child);
        }

        public static void Detach(AtkResNode* node) {
            if (node->NextSiblingNode != null && node->NextSiblingNode->PrevSiblingNode == node) {
                node->NextSiblingNode->PrevSiblingNode = null; // unlink
            }
            node->NextSiblingNode = null;
        }
    }

    public unsafe struct LayoutNode {
        public AtkResNode* Node;
        public LayoutNode[] Childen;

        public LayoutNode(AtkImageNode* node) : this((AtkResNode*)node) { }
        public LayoutNode(AtkNineGridNode* node) : this((AtkResNode*)node) { }
        public LayoutNode(AtkTextNode* node) : this((AtkResNode*)node) { }
        public LayoutNode(AtkResNode* node) {
            Node = node;
            Childen = null;
        }

        public LayoutNode(AtkResNode* node, LayoutNode[] children) {
            Node = node;
            Childen = children;
        }

        public LayoutNode(AtkResNode* node, AtkResNode*[] children) {
            Node = node;
            Childen = new LayoutNode[children.Length];
            for (int i = 0; i < Childen.Length; i++) {
                Childen[i] = new LayoutNode(children[i]);
            }
        }

        public int Setup() {
            if (Childen == null || Childen.Length == 0) return Node->ChildCount; // just the node

            var count = Childen.Length;
            for (int i = 0; i < Childen.Length; i++) {
                Childen[i].Node->ParentNode = Node;
                count += Childen[i].Setup();
                if (i < Childen.Length - 1) AtkHelper.Link(Childen[i].Node, Childen[i + 1].Node);
            }
            Node->ChildNode = Childen[0].Node;
            Node->ChildCount = (ushort)count;
            return count;
        }

        public void Cleanup() {
            if (Childen != null) {
                for (int i = 0; i < Childen.Length; i++) {
                    Childen[i].Cleanup();
                }
            }
            Node = null;
        }
    }
}
