using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Atk;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Node.Simple;
using KamiToolKit.Timelines;
using System.Numerics;

namespace JobBars.Nodes.Gauge.Arrow {
    public unsafe class ArrowTick : SimpleOverlayNode {
        public readonly ImageNode Background;
        public readonly SimpleOverlayNode SelectedContainer;
        public readonly ImageNode Selected;

        private ElementColor TickColor = ColorConstants.NoColor;

        private bool PrevValue = false;

        public ArrowTick() {
            Size = new( 32, 32 );

            Background = new SimpleImageNode() {
                Size = new( 32, 32 ),
                TextureCoordinates = new( 0, 0 ),
                TextureSize = new( 32, 32 ),
                WrapMode = WrapMode.Tile,
                ImageNodeFlags = 0,
                TexturePath = "ui/uld/JobHudSimple_StackB.tex"
            };

            SelectedContainer = new SimpleOverlayNode() {
                Size = new( 32, 32 ),
                Origin = new( 16, 16 ),
            };

            Selected = new SimpleImageNode() {
                Size = new( 32, 32 ),
                Origin = new( 16, 16 ),
                TextureCoordinates = new( 32, 0 ),
                TextureSize = new( 32, 32 ),
                WrapMode = WrapMode.Tile,
                ImageNodeFlags = 0,
                TexturePath = "ui/uld/JobHudSimple_StackB.tex"
            };

            Background.AttachNode( this );
            SelectedContainer.AttachNode( this );

            Selected.AttachNode( SelectedContainer );

            AddTimeline( new TimelineBuilder()
                .BeginFrameSet( 1, 12 ) // Pop in
                .AddLabel( 1, 1, AtkTimelineJumpBehavior.Start, 0 )
                .AddLabel( 12, 0, AtkTimelineJumpBehavior.PlayOnce, 0 )
                .EndFrameSet()
                .Build()
            );

            SelectedContainer.AddTimeline( new TimelineBuilder()
                .BeginFrameSet( 1, 12 )
                .AddFrame( 1, scale: new Vector2( 2.5f, 2.5f ), alpha: 0, addColor: new Vector3( 80f, 80f, 80f ) )
                .AddFrame( 12, scale: new Vector2( 1f, 1f ), alpha: 255, addColor: new Vector3( 0f, 0f, 0f ) )
                .EndFrameSet()
                .Build()
            );
        }

        public void SetValue( bool value ) {
            Selected.IsVisible = value;
            if( value && !PrevValue ) {
                Timeline?.PlayAnimation( 1 ); // Pop in
            }
            PrevValue = value;
        }

        public void SetColor( ElementColor color ) {
            TickColor = color;
            TickColor.SetColor( Selected );
        }
    }
}
