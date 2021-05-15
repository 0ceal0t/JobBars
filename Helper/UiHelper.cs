using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.FFXIV.Component.GUI.ULD;

namespace JobBars.Helper {
    public static unsafe partial class UiHelper {
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

        public static unsafe bool GetNodeVisible(AtkResNode* node) {
            if (node == null) return false;
            while (node != null) {
                if ((node->Flags & (short)NodeFlags.Visible) != (short)NodeFlags.Visible) return false;
                node = node->ParentNode;
            }
            return true;
        }

        public static void Hide(AtkResNode* node) {
            node->Flags &= ~0x10;
            node->Flags_2 |= 0x1;
        }
        public static void Show(AtkResNode* node) {
            node->Flags |= 0x10;
            node->Flags_2 |= 0x1;
        }

        public static void SetText(AtkTextNode* textNode, SeString str) {
            if (!Ready) return;
            var bytes = str.Encode();
            var ptr = Marshal.AllocHGlobal(bytes.Length + 1);
            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            Marshal.WriteByte(ptr, bytes.Length, 0);
            _atkTextNodeSetText(textNode, (byte*) ptr);
            Marshal.FreeHGlobal(ptr);
        }

        public static void SetText(AtkTextNode* textNode, string str) {
            if (!Ready) return;
            var seStr = new SeString(new Payload[] { new TextPayload(str) });
            SetText(textNode, seStr);
        }

        public static void SetSize(AtkResNode* node, int? width, int? height) {
            if (width != null && width >= ushort.MinValue && width <= ushort.MaxValue) node->Width = (ushort) width.Value;
            if (height != null && height >= ushort.MinValue && height <= ushort.MaxValue) node->Height = (ushort) height.Value;
            node->Flags_2 |= 0x1;
        }

        public static void SetPosition(AtkResNode* node, float? x, float? y) {
            if (x != null) node->X = x.Value;
            if (y != null) node->Y = y.Value;
            node->Flags |= 0x10;
            node->Flags_2 |= 0x1;

        }

        public static void SetScale(AtkResNode* atkUnitBase, float? scaleX, float? scaleY) {
            atkUnitBase->ScaleX = scaleX.Value;
            atkUnitBase->ScaleY = scaleY.Value;
            atkUnitBase->Flags |= 0x10;
            atkUnitBase->Flags_2 |= 0x1;
        }

        public static void ExpandNodeList(AtkComponentNode* componentNode, ushort addSize) {
            var newNodeList = ExpandNodeList(componentNode->Component->UldManager.NodeList, componentNode->Component->UldManager.NodeListCount, (ushort) (componentNode->Component->UldManager.NodeListCount + addSize));
            componentNode->Component->UldManager.NodeList = newNodeList;
        }

        public static void ExpandNodeList(AtkUnitBase* atkUnitBase, ushort addSize) {
            var newNodeList = ExpandNodeList(atkUnitBase->UldManager.NodeList, atkUnitBase->UldManager.NodeListCount, (ushort)(atkUnitBase->UldManager.NodeListCount + addSize));
            atkUnitBase->UldManager.NodeList = newNodeList;
        }

        private static AtkResNode** ExpandNodeList(AtkResNode** originalList, ushort originalSize, ushort newSize = 0) {
            if (newSize <= originalSize) newSize = (ushort)(originalSize + 1);
            var oldListPtr = new IntPtr(originalList);
            var newListPtr = Alloc((ulong)((newSize + 1) * 8));
            var clone = new IntPtr[originalSize];
            Marshal.Copy(oldListPtr, clone, 0, originalSize);
            Marshal.Copy(clone, 0, newListPtr, originalSize);
            return (AtkResNode**)(newListPtr);
        }

        public static AtkUldPart* ExpandPartList(AtkUldManager manager, ushort addSize) {
            var oldLength = manager.PartsList->PartCount;
            var newLength = oldLength + addSize + 1;

            var oldSize = oldLength * 0x10;
            var newSize = newLength * 0x10;
            var newListPtr = Alloc(newSize);
            var oldListPtr = new IntPtr(manager.PartsList->Parts);
            byte[] oldData = new byte[oldSize];
            Marshal.Copy(oldListPtr, oldData, 0, (int)oldSize);
            Marshal.Copy(oldData, 0, newListPtr, (int)oldSize);
            return (AtkUldPart*)newListPtr;
        }

        public static AtkResNode* CloneNode(AtkResNode* original) {
            var size = original->Type switch
            {
                NodeType.Res => sizeof(AtkResNode),
                NodeType.Image => sizeof(AtkImageNode),
                NodeType.Text => sizeof(AtkTextNode),
                NodeType.NineGrid => sizeof(AtkNineGridNode),
                NodeType.Counter => sizeof(AtkCounterNode),
                NodeType.Collision => sizeof(AtkCollisionNode),
                _ => throw new Exception($"Unsupported Type: {original->Type}")
            };

            var allocation = Alloc((ulong)size);
            var bytes = new byte[size];
            Marshal.Copy(new IntPtr(original), bytes, 0, bytes.Length);
            Marshal.Copy(bytes, 0, allocation, bytes.Length);

            var newNode = (AtkResNode*)allocation;
            newNode->ParentNode = null;
            newNode->ChildNode = null;
            newNode->ChildCount = 0;
            newNode->PrevSiblingNode = null;
            newNode->NextSiblingNode = null;
            return newNode;
        }

        public static T* CleanAlloc<T>() where T : unmanaged {
            var alloc = Alloc(sizeof(T));
            Marshal.Copy(new byte[sizeof(T)], 0, alloc, sizeof(T));
            return (T*) alloc;
        }
    }
}
