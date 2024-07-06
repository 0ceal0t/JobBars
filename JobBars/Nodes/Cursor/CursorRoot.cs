using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Atk;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace JobBars.Nodes.Cursor {
    public unsafe class CursorRoot : NodeBase<AtkResNode> {
        private readonly ImageNode Inner;
        private readonly ImageNode Outer;

        private bool StaticCircleInner = true;
        private bool StaticCircleOuter = true;

        public CursorRoot() : base( NodeType.Res ) {
            NodeID = JobBars.NodeId++;
            Size = new( 44, 48 );
            NodeFlags = NodeFlags.Visible | NodeFlags.Fill | NodeFlags.AnchorLeft | NodeFlags.AnchorTop;

            Inner = new ImageNode() {
                NodeID = JobBars.NodeId++,
                Size = new( 44, 46 ),
                NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop,
                WrapMode = WrapMode.Unknown,
                ImageNodeFlags = 0
            };

            SetPartId( Inner, 79, ref StaticCircleInner );

            Outer = new ImageNode() {
                NodeID = JobBars.NodeId++,
                Size = new( 44, 46 ),
                NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop,
                WrapMode = WrapMode.Unknown,
                ImageNodeFlags = 0
            };

            SetPartId( Outer, 79, ref StaticCircleOuter );

            JobBars.NativeController.AttachToNode( [
                Inner,
                Outer
            ], this, NodePosition.AsLastChild );
        }

        private static void SetPartId( ImageNode node, int partId, ref bool staticCircle ) {
            if( partId == 80 ) { // Placeholder for static circle
                if( !staticCircle ) node.LoadTexture( "ui/uld/CursorLocation.tex" );
                staticCircle = true;

                node.TextureCoordinates = new( 0, 0 );
                node.TextureSize = new( 128, 128 );

            }
            else {
                if( staticCircle ) node.LoadTexture( "ui/uld/IconA_Recast2.tex" );
                staticCircle = false;

                var row = partId % 9;
                var column = ( partId - row ) / 9;
                var u = ( ushort )( 44 * row );
                var v = ( ushort )( 48 * column );

                node.TextureCoordinates = new( u, v );
                node.TextureSize = new( 44, 46 );
            }
        }

        public void ShowInner() { Inner.IsVisible = true; }
        public void ShowOuter() { Outer.IsVisible = true; }
        public void HideInner() { Inner.IsVisible = false; }
        public void HideOuter() { Outer.IsVisible = false; }

        public void SetInner( float percent, float scale ) => SetCursorPercent( Inner, percent, scale, ref StaticCircleInner );
        public void SetOuter( float percent, float scale ) => SetCursorPercent( Outer, percent, scale, ref StaticCircleOuter );

        private static void SetCursorPercent( ImageNode node, float percent, float scale, ref bool staticCircle ) {
            if( percent == 2 ) { // whatever, just use this for the solid circle
                node.Size = new( 128, 128 );
                node.Position = new( -( 128f * scale ) / 2f, -( 128f * scale ) / 2f + 2 );
                node.Scale = new( scale, scale );
                SetPartId( node, 80, ref staticCircle );
            }
            else {
                node.Size = new( 44, 46 );
                node.Position = new( -22f * scale, -20f * scale );
                node.Scale = new( scale, scale );
                SetPartId( node, ( ushort )( percent * 79 ), ref staticCircle );
            }
        }

        public void SetInnerColor( ElementColor color ) {
            var newColor = color;
            newColor.AddRed -= 40;
            newColor.AddBlue += 40;

            Inner.MultiplyColor = color.MultiplyColor;
            Inner.AddColor = color.AddColor;
        }

        public void SetOuterColor( ElementColor color ) {
            var newColor = color;
            newColor.AddRed -= 40;
            newColor.AddBlue += 40;

            Outer.MultiplyColor = color.MultiplyColor;
            Outer.AddColor = color.AddColor;
        }

        protected override void Dispose( bool disposing ) {
            if( disposing ) {
                Inner.Dispose();
                Outer.Dispose();
                base.Dispose( disposing );
            }
        }
    }
}
