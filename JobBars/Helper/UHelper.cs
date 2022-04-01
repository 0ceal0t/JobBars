using System;
using System.Numerics;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace JobBars.Helper {
    public static unsafe partial class UIHelper {
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
            node->Flags &= ~0x10;
            node->Flags_2 |= 0x1;
        }

        public static void Show(AtkResNode* node) {
            node->Flags |= 0x10;
            node->Flags_2 |= 0x1;
        }

        public static void Update(AtkResNode* node) {
            node->Flags_2 |= 0x1;
        }

        public static void SetVisibility(AtkResNode* node, bool visiblity) {
            if (visiblity) Show(node);
            else Hide(node);
        }

        public static void SetSize(AtkResNode* node, int? width, int? height) {
            if (width != null && width >= ushort.MinValue && width <= ushort.MaxValue) node->Width = (ushort)width.Value;
            if (height != null && height >= ushort.MinValue && height <= ushort.MaxValue) node->Height = (ushort)height.Value;
            node->Flags_2 |= 0x1;
        }

        public static void SetPosition(AtkResNode* node, float? x, float? y) {
            if (x != null) node->X = x.Value;
            if (y != null) node->Y = y.Value;
            node->Flags_2 |= 0x1;
        }

        public static void SetScale(AtkResNode* atkUnitBase, float? scaleX, float? scaleY) {
            atkUnitBase->ScaleX = scaleX.Value;
            atkUnitBase->ScaleY = scaleY.Value;
            atkUnitBase->Flags_2 |= 0x1;
            atkUnitBase->Flags_2 |= 0x4;
        }

        public static void SetRotation(AtkResNode* node, float rotation) {
            node->Rotation = rotation;
            node->Flags_2 |= 0x1;
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

        public static Vector2 CalculateActualDimensions(AtkResNode* container) {
            if (container == null || !container->IsVisible) return new Vector2(0, 0);

            var maxY = 0f;
            var minY = 0f;
            var maxX = 0f;
            var minX = 0f;

            /*
                XatTopEdge = w * cs      (AE at the picture)
                YatRightEdge = h * cs    (DH)
                XatBottomEdge = h * as   (BG)
                YatLeftEdge = w * as     (AF)

                 H = w * Abs(Sin(Fi)) + h * Abs(Cos(Fi))
                 W = w * Abs(Cos(Fi)) + h * Abs(Sin(Fi))
                 denote 
                 as = Abs(Sin(Fi))
                 cs = Abs(Cos(Fi))
             */

            var node = container->ChildNode;
            while(node != null) {
                if (node->IsVisible) {
                    var angle = node->Rotation;
                    var width = (float)node->Width;
                    var height = (float)node->Height;
                    var x = node->X;
                    var y = node->Y;

                    var c_s = Math.Abs(Math.Cos(angle));
                    var a_s = Math.Abs(Math.Sin(angle));

                    var x1 = width * c_s + x;
                    var y1 = height * c_s + y;
                    var x2 = height * a_s + x;
                    var y2 = width * a_s + y;

                    maxX = (float)Math.Max(maxX, Math.Max(x1, x2));
                    minX = (float)Math.Min(minX, Math.Min(x1, x2));

                    maxY = (float)Math.Max(maxY, Math.Max(y1, y2));
                    minY = (float)Math.Min(minY, Math.Min(y1, y2));
                }
                node = node->PrevSiblingNode;
            }
            return new Vector2(Math.Abs(maxX - minX), Math.Abs(maxY - minY));
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
            if (Childen == null || Childen.Length == 0) return 0; // just the node

            var count = Childen.Length;
            for (int i = 0; i < Childen.Length; i++) {
                Childen[i].Node->ParentNode = Node;
                count += Childen[i].Setup();
                if (i < Childen.Length - 1) UIHelper.Link(Childen[i].Node, Childen[i + 1].Node);
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
