using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;

namespace JobBars.UI {
    public unsafe partial class UIBuilder {
        private void Init(AtkUnitBase* addon) {
            NodeIdx = NODE_IDX_START;

            GaugeRoot = CreateResNode();
            GaugeRoot->Width = 256;
            GaugeRoot->Height = 100;
            GaugeRoot->Flags = 9395;
            GaugeRoot->Flags_2 = 4;
            GaugeRoot->ParentNode = addon->RootNode;

            BuffRoot = CreateResNode();
            BuffRoot->Width = 256;
            BuffRoot->Height = 100;
            BuffRoot->Flags = 9395;
            BuffRoot->Flags_2 = 4;
            BuffRoot->ParentNode = addon->RootNode;

            // ==== INSERT AT THE END ====
            var lastNode = addon->RootNode->ChildNode;
            while (lastNode->PrevSiblingNode != null) {
                lastNode = lastNode->PrevSiblingNode;
            }

            UIHelper.Link(lastNode, GaugeRoot);
            UIHelper.Link(GaugeRoot, BuffRoot);

            addon->UldManager.UpdateDrawNodeList();
        }

        public AtkResNode* CreateResNode() {
            var node = UIHelper.CleanAlloc<AtkResNode>();
            node->Ctor();

            node->NodeID = (NodeIdx++);
            node->Type = NodeType.Res;
            node->ScaleX = 1;
            node->ScaleY = 1;
            node->Rotation = 0;
            node->Depth = 0;
            node->Depth_2 = 0;
            node->Color = UIColor.BYTE_White;
            node->ParentNode = null;
            node->ChildNode = null;
            node->ChildCount = 0;
            node->PrevSiblingNode = null;
            node->NextSiblingNode = null;
            node->Flags = 8243;
            node->Flags_2 = 1;
            node->Flags_2 |= 4;

            return node;
        }

        public AtkTextNode* CreateTextNode() {
            var node = UIHelper.CleanAlloc<AtkTextNode>();
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
            var node = UIHelper.CleanAlloc<AtkImageNode>();
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
            node->AtkResNode.ParentNode = null;
            node->AtkResNode.ChildNode = null;
            node->AtkResNode.ChildCount = 0;
            node->AtkResNode.PrevSiblingNode = null;
            node->AtkResNode.NextSiblingNode = null;
            node->AtkResNode.Flags = 8243;
            node->AtkResNode.Flags_2 = 1;
            node->AtkResNode.Flags_2 |= 4;

            return node;
        }

        public AtkNineGridNode* CreateNineNode() {
            var addon = (AtkUnitBase*)PluginInterface?.Framework.Gui.GetUiObjectByName("_ParameterWidget", 1);
            var gaugeComp = (AtkComponentNode*)addon->RootNode->ChildNode;
            var node = UIHelper.CloneNode((AtkNineGridNode*)gaugeComp->Component->UldManager.NodeList[3]);

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
            node->AtkResNode.MultiplyRed = 100; // need this since cloning
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