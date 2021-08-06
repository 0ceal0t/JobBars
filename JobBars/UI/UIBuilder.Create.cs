using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;

namespace JobBars.UI {
    public unsafe partial class UIBuilder {
        public AtkResNode* CreateResNode() {
            var node = (AtkResNode*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkResNode), 8);
            if (node == null) {
                PluginLog.Debug("Failed to allocate memory for res node");
                return null;
            }
            IMemorySpace.Memset(node, 0, (ulong)sizeof(AtkResNode));
            node->Ctor();

            node->NodeID = (NodeIdx++);
            node->Type = NodeType.Res;
            node->ScaleX = 1;
            node->ScaleY = 1;
            node->Rotation = 0;
            node->Depth = 0;
            node->Depth_2 = 0;
            node->Color = UIColor.BYTE_White;
            node->MultiplyRed = 100;
            node->MultiplyRed_2 = 100;
            node->MultiplyGreen = 100;
            node->MultiplyGreen_2 = 100;
            node->MultiplyBlue = 100;
            node->MultiplyBlue_2 = 100;
            node->AddRed = 0;
            node->AddRed_2 = 0;
            node->AddGreen = 0;
            node->AddGreen_2 = 0;
            node->AddBlue = 0;
            node->AddBlue_2 = 0;
            node->Alpha_2 = 255;
            node->ParentNode = null;
            node->ChildNode = null;
            node->ChildCount = 0;
            node->PrevSiblingNode = null;
            node->NextSiblingNode = null;
            node->Flags = 8243;
            node->Flags_2 = 1;
            node->Flags_2 |= 4;
            node->DrawFlags = 0;

            return node;
        }

        public AtkTextNode* CreateTextNode() {
            var node = (AtkTextNode*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkTextNode), 8);
            if (node == null) {
                PluginLog.Debug("Failed to allocate memory for res node");
                return null;
            }
            IMemorySpace.Memset(node, 0, (ulong)sizeof(AtkTextNode));
            node->Ctor();

            node->AtkResNode.NodeID = (NodeIdx++);
            node->AtkResNode.Type = NodeType.Text;
            node->AtkResNode.ScaleX = 1;
            node->AtkResNode.ScaleY = 1;
            node->AtkResNode.Width = 17;
            node->AtkResNode.Height = 30;
            node->AtkResNode.Rotation = 0;
            node->AtkResNode.Depth = 0;
            node->AtkResNode.Depth_2 = 0;
            node->AtkResNode.Color = UIColor.BYTE_White;
            node->AtkResNode.MultiplyRed = 100;
            node->AtkResNode.MultiplyRed_2 = 100;
            node->AtkResNode.MultiplyGreen = 100;
            node->AtkResNode.MultiplyGreen_2 = 100;
            node->AtkResNode.MultiplyBlue = 100;
            node->AtkResNode.MultiplyBlue_2 = 100;
            node->AtkResNode.AddRed = 0;
            node->AtkResNode.AddRed_2 = 0;
            node->AtkResNode.AddGreen = 0;
            node->AtkResNode.AddGreen_2 = 0;
            node->AtkResNode.AddBlue = 0;
            node->AtkResNode.AddBlue_2 = 0;
            node->AtkResNode.Alpha_2 = 255;
            node->AtkResNode.ParentNode = null;
            node->AtkResNode.ChildNode = null;
            node->AtkResNode.ChildCount = 0;
            node->AtkResNode.PrevSiblingNode = null;
            node->AtkResNode.NextSiblingNode = null;
            node->AtkResNode.Flags = 8250;
            node->AtkResNode.Flags_2 = 1;
            node->AtkResNode.Flags_2 |= 4;
            node->AtkResNode.DrawFlags = 8;

            node->TextId = 0;
            node->TextColor = UIColor.BYTE_White;
            node->EdgeColor = new ByteColor {
                R = 157,
                G = 131,
                B = 91,
                A = 255
            };
            node->BackgroundColor = UIColor.BYTE_Transparent;
            node->LineSpacing = 18;
            node->AlignmentFontType = 21;
            node->FontSize = 18;
            node->TextFlags = 16;
            node->TextFlags2 = 0;

            node->SetText("");

            return node;
        }

        public AtkImageNode* CreateImageNode() {
            var node = (AtkImageNode*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkImageNode), 8);
            if (node == null) {
                PluginLog.Debug("Failed to allocate memory for image node");
                return null;
            }
            IMemorySpace.Memset(node, 0, (ulong)sizeof(AtkImageNode));
            node->Ctor();

            node->WrapMode = 1;

            node->AtkResNode.NodeID = (NodeIdx++);
            node->AtkResNode.Type = NodeType.Image;
            node->AtkResNode.ScaleX = 1;
            node->AtkResNode.ScaleY = 1;
            node->AtkResNode.Width = 17;
            node->AtkResNode.Height = 30;
            node->AtkResNode.Rotation = 0;
            node->AtkResNode.Depth = 0;
            node->AtkResNode.Depth_2 = 0;
            node->AtkResNode.Color = UIColor.BYTE_White;
            node->AtkResNode.MultiplyRed = 100;
            node->AtkResNode.MultiplyRed_2 = 100;
            node->AtkResNode.MultiplyGreen = 100;
            node->AtkResNode.MultiplyGreen_2 = 100;
            node->AtkResNode.MultiplyBlue = 100;
            node->AtkResNode.MultiplyBlue_2 = 100;
            node->AtkResNode.AddRed = 0;
            node->AtkResNode.AddRed_2 = 0;
            node->AtkResNode.AddGreen = 0;
            node->AtkResNode.AddGreen_2 = 0;
            node->AtkResNode.AddBlue = 0;
            node->AtkResNode.AddBlue_2 = 0;
            node->AtkResNode.Alpha_2 = 255;
            node->AtkResNode.ParentNode = null;
            node->AtkResNode.ChildNode = null;
            node->AtkResNode.ChildCount = 0;
            node->AtkResNode.PrevSiblingNode = null;
            node->AtkResNode.NextSiblingNode = null;
            node->AtkResNode.Flags = 8243;
            node->AtkResNode.Flags_2 = 1;
            node->AtkResNode.Flags_2 |= 4;
            node->AtkResNode.DrawFlags = 12;

            return node;
        }

        public AtkNineGridNode* CreateNineNode() {

            var addon = (AtkUnitBase*)PluginInterface?.Framework.Gui.GetUiObjectByName("_ParameterWidget", 1);
            var gaugeComp = (AtkComponentNode*)addon->RootNode->ChildNode;
            var node = (AtkNineGridNode*)UIHelper.CloneNode(gaugeComp->Component->UldManager.NodeList[3]);

            node->PartsTypeRenderType = 128;

            node->AtkResNode.NodeID = (NodeIdx++);
            node->AtkResNode.Type = NodeType.NineGrid;
            node->AtkResNode.ScaleX = 1;
            node->AtkResNode.ScaleY = 1;
            node->AtkResNode.Width = 17;
            node->AtkResNode.Height = 30;
            node->AtkResNode.Rotation = 0;
            node->AtkResNode.Depth = 0;
            node->AtkResNode.Depth_2 = 0;
            node->AtkResNode.Color = UIColor.BYTE_White;
            node->AtkResNode.MultiplyRed = 100;
            node->AtkResNode.MultiplyRed_2 = 100;
            node->AtkResNode.MultiplyGreen = 100;
            node->AtkResNode.MultiplyGreen_2 = 100;
            node->AtkResNode.MultiplyBlue = 100;
            node->AtkResNode.MultiplyBlue_2 = 100;
            node->AtkResNode.AddRed = 0;
            node->AtkResNode.AddRed_2 = 0;
            node->AtkResNode.AddGreen = 0;
            node->AtkResNode.AddGreen_2 = 0;
            node->AtkResNode.AddBlue = 0;
            node->AtkResNode.AddBlue_2 = 0;
            node->AtkResNode.Alpha_2 = 255;
            node->AtkResNode.ParentNode = null;
            node->AtkResNode.ChildNode = null;
            node->AtkResNode.ChildCount = 0;
            node->AtkResNode.PrevSiblingNode = null;
            node->AtkResNode.NextSiblingNode = null;
            node->AtkResNode.Flags = 8243;
            node->AtkResNode.Flags_2 = 1;
            node->AtkResNode.Flags_2 |= 4;
            node->AtkResNode.DrawFlags = 8;

            return node;
        }
    }
}
