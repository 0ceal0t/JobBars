using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;

namespace JobBars.Atk {
    public unsafe partial class AtkBuilder {
        public static AtkResNode* CreateResNode() {
            var node = AtkHelper.CleanAlloc<AtkResNode>();
            node->Ctor();

            node->NodeId = ( NodeIdx++ );
            node->Type = NodeType.Res;
            node->ScaleX = 1;
            node->ScaleY = 1;
            node->Rotation = 0;
            node->Depth = 0;
            node->Depth_2 = 0;
            node->Color = AtkColor.BYTE_White;
            node->ParentNode = null;
            node->ChildNode = null;
            node->ChildCount = 0;
            node->PrevSiblingNode = null;
            node->NextSiblingNode = null;
            node->NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop;
            node->DrawFlags = 1;
            node->DrawFlags |= 4;

            return node;
        }

        public static AtkTextNode* CreateTextNode() {
            var node = AtkHelper.CleanAlloc<AtkTextNode>();
            node->Ctor();

            node->AtkResNode.NodeId = ( NodeIdx++ );
            node->AtkResNode.Type = NodeType.Text;
            node->AtkResNode.ScaleX = 1;
            node->AtkResNode.ScaleY = 1;
            node->AtkResNode.Width = 17;
            node->AtkResNode.Height = 30;
            node->AtkResNode.Rotation = 0;
            node->AtkResNode.Depth = 0;
            node->AtkResNode.Depth_2 = 0;
            node->AtkResNode.Color = AtkColor.BYTE_White;
            node->AtkResNode.ParentNode = null;
            node->AtkResNode.ChildNode = null;
            node->AtkResNode.ChildCount = 0;
            node->AtkResNode.PrevSiblingNode = null;
            node->AtkResNode.NextSiblingNode = null;
            node->AtkResNode.NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorRight;
            node->AtkResNode.DrawFlags = 8;

            node->TextId = 0;
            node->TextColor = AtkColor.BYTE_White;
            node->EdgeColor = new ByteColor {
                R = 157,
                G = 131,
                B = 91,
                A = 255
            };
            node->BackgroundColor = AtkColor.BYTE_Transparent;
            node->LineSpacing = 18;
            node->AlignmentFontType = 21;
            node->FontSize = 18;
            node->TextFlags = 16;
            node->TextFlags2 = 0;

            node->SetText( "" );

            return node;
        }

        public static AtkImageNode* CreateImageNode() {
            var node = AtkHelper.CleanAlloc<AtkImageNode>();
            node->Ctor();

            node->WrapMode = 1;

            node->AtkResNode.NodeId = ( NodeIdx++ );
            node->AtkResNode.Type = NodeType.Image;
            node->AtkResNode.ScaleX = 1;
            node->AtkResNode.ScaleY = 1;
            node->AtkResNode.Width = 17;
            node->AtkResNode.Height = 30;
            node->AtkResNode.Rotation = 0;
            node->AtkResNode.Depth = 0;
            node->AtkResNode.Depth_2 = 0;
            node->AtkResNode.Color = AtkColor.BYTE_White;
            node->AtkResNode.ParentNode = null;
            node->AtkResNode.ChildNode = null;
            node->AtkResNode.ChildCount = 0;
            node->AtkResNode.PrevSiblingNode = null;
            node->AtkResNode.NextSiblingNode = null;
            node->AtkResNode.NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop;
            node->AtkResNode.DrawFlags = 1;
            node->AtkResNode.DrawFlags |= 4;

            return node;
        }

        public static AtkNineGridNode* CreateNineNode() {
            var node = AtkHelper.CleanAlloc<AtkNineGridNode>();
            node->Ctor();

            node->PartsTypeRenderType = 128;

            node->AtkResNode.NodeId = ( NodeIdx++ );
            node->AtkResNode.Type = NodeType.NineGrid;
            node->AtkResNode.ScaleX = 1;
            node->AtkResNode.ScaleY = 1;
            node->AtkResNode.Width = 17;
            node->AtkResNode.Height = 30;
            node->AtkResNode.Rotation = 0;
            node->AtkResNode.Depth = 0;
            node->AtkResNode.Depth_2 = 0;
            node->AtkResNode.Color = AtkColor.BYTE_White;
            node->AtkResNode.ParentNode = null;
            node->AtkResNode.ChildNode = null;
            node->AtkResNode.ChildCount = 0;
            node->AtkResNode.PrevSiblingNode = null;
            node->AtkResNode.NextSiblingNode = null;
            node->AtkResNode.NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop;
            node->AtkResNode.DrawFlags = 8;

            return node;
        }
    }
}