using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Atk;
using JobBars.Data;
using JobBars.Helper;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using System;
using System.Collections.Generic;

namespace JobBars.Nodes.Gauge.Bar {
    public unsafe class BarNode : GaugeNode {
        private static readonly int MAX_SEGMENTS = 6;

        private readonly ResNode GaugeContainer;

        private readonly ImageNode Background;
        private readonly ResNode BarContainer;
        private readonly NineGridNode BarSecondary;
        private readonly NineGridNode BarMain;
        private readonly List<ImageNode> Separators = [];
        private readonly ImageNode Frame;
        private readonly NineGridNode Indicator;

        private readonly ResNode TextContainer;
        private readonly TextNode Text;
        private readonly NineGridNode TextBlur;

        private string CurrentText;
        private float LastPercent = 1f;
        private Animation Animation;
        private float[] Segments = null;

        private bool Vertical = false;
        private bool TextSwap = false;

        public BarNode() : base() {
            NodeFlags |= NodeFlags.AnchorLeft | NodeFlags.AnchorTop;
            Size = new( 160, 46 );

            GaugeContainer = new ResNode() {
                Size = new( 160, 32 ),
                NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop,
            };

            Background = new SimpleImageNode() {
                Size = new( 160, 20 ),
                TextureCoordinates = new( 0, 100 ),
                TextureSize = new( 160, 20 ),
                NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop,
                WrapMode = WrapMode.Tile,
                ImageNodeFlags = 0,
                TexturePath = "ui/uld/Parameter_Gauge.tex"
            };

            // ========= BAR ==============

            BarContainer = new ResNode() {
                Size = new( 160, 20 ),
                NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop,
            };

            BarMain = new SimpleNineGridNode() {
                Size = new( 148, 20 ),
                Position = new( 6, 0 ),
                TextureCoordinates = new( 6, 40 ),
                TextureSize = new( 148, 20 ),
                PartsRenderType = ( byte )PartsRenderType.RenderType,
                NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop,
                TexturePath = "ui/uld/Parameter_Gauge.tex"
            };

            BarSecondary = new SimpleNineGridNode() {
                Size = new( 0, 20 ),
                Position = new( 6, 0 ),
                TextureCoordinates = new( 6, 40 ),
                TextureSize = new( 148, 20 ),
                PartsRenderType = ( byte )PartsRenderType.RenderType,
                NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop,
                TexturePath = "ui/uld/Parameter_Gauge.tex"
            };

            for( var i = 0; i < MAX_SEGMENTS - 1; i++ ) {
                Separators.Add( new SimpleImageNode() {
                    Rotation = ( float )( Math.PI / 2f ),
                    Size = new( 10, 5 ),
                    Position = new( 0, 5 ),
                    TextureCoordinates = new( 10, 3 ),
                    TextureSize = new( 10, 5 ),
                    NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop,
                    WrapMode = WrapMode.Tile,
                    ImageNodeFlags = 0,
                    TexturePath = "ui/uld/Parameter_Gauge.tex"
                } );
            }

            // ======== FRAME ============

            Frame = new SimpleImageNode() {
                Size = new( 160, 20 ),
                TextureCoordinates = new( 0, 0 ),
                TextureSize = new( 160, 20 ),
                NodeFlags = NodeFlags.Visible,
                WrapMode = WrapMode.Tile,
                ImageNodeFlags = 0,
                TexturePath = "ui/uld/Parameter_Gauge.tex"
            };

            Indicator = new SimpleNineGridNode() {
                Size = new( 160, 20 ),
                TextureSize = new( 160, 20 ),
                TopOffset = 5,
                BottomOffset = 5,
                LeftOffset = 15,
                RightOffset = 15,
                PartsRenderType = ( byte )PartsRenderType.RenderType,
                NodeFlags = NodeFlags.Visible,
                TexturePath = "ui/uld/Parameter_Gauge.tex"
            };

            // ======= TEXT ==============

            TextContainer = new ResNode() {
                Size = new( 47, 40 ),
                Position = new( 112, 6 ),
                NodeFlags = NodeFlags.Visible,
            };

            Text = new TextNode() {
                Position = new( 14, 5 ),
                Size = new( 17, 30 ),
                FontSize = 18,
                LineSpacing = 18,
                NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorRight,
                TextColor = new( 1, 1, 1, 1 ),
                TextOutlineColor = new( 157f / 255f, 131f / 255f, 91f / 255f, 1 ),
                TextId = 0,
                TextFlags = TextFlags.Glare,
                String = "",
            };
            Text.Node->AlignmentFontType = 21;
            Text.AttachNode( TextContainer );

            TextBlur = new SimpleNineGridNode() {
                Size = new( 47, 48 ),
                TextureSize = new( 60, 40 ),
                LeftOffset = 28,
                RightOffset = 28,
                PartsRenderType = 128,
                NodeFlags = NodeFlags.Visible | NodeFlags.Fill | NodeFlags.AnchorLeft | NodeFlags.AnchorTop,
                TexturePath = "ui/uld/JobHudNumBg.tex"
            };
            TextBlur.AttachNode( TextContainer );

            BarSecondary.AttachNode( BarContainer );
            BarMain.AttachNode( BarContainer );
            Separators.ForEach( x => x.AttachNode( BarContainer ) );

            Background.AttachNode( GaugeContainer );
            BarContainer.AttachNode( GaugeContainer );
            Frame.AttachNode( GaugeContainer );
            Indicator.AttachNode( GaugeContainer );

            GaugeContainer.AttachNode( this );
            TextContainer.AttachNode( this );
        }

        public void SetText( string text ) {
            if( text != CurrentText ) {
                Text.String = text;
                CurrentText = text;
            }

            var size = text.Length * 17;
            if( Vertical ) {
                if( TextSwap ) TextContainer.X = ( 8 + 17 ) - size;
            }
            else {
                TextContainer.X = ( 112 + 17 ) - size;
            }

            Text.IsVisible = true;
            Text.X = 14;
            Text.Size = new( size, 30 );
            TextContainer.Size = new( 30 + size, 40 );
            TextBlur.X = 0;
            TextBlur.Size = new( 30 + size, 40 );
        }

        public void SetTextColor( ElementColor color ) {
            color.SetColor( Text );
        }

        public void SetTextVisible( bool visible ) {
            TextContainer.IsVisible = visible;
        }

        public void SetLayout( bool textSwap, bool vertical ) {
            Vertical = vertical;
            TextSwap = textSwap;

            if( vertical ) {
                GaugeContainer.Rotation = ( float )( -Math.PI / 2f );
                GaugeContainer.Position = new( TextSwap ? 42 : 0, 158 );
                TextContainer.Position = new( TextSwap ? 8 : 6, 125 );
            }
            else {
                GaugeContainer.Rotation = 0;
                GaugeContainer.Position = new( 0, TextSwap ? 24 : 0 );
                TextContainer.Position = new( 112, TextSwap ? -3 : 6 );
            }
        }

        public void SetPercent( float value ) {
            if( value > 1 ) value = 1;
            else if( value < 0 ) value = 0;

            var difference = Math.Abs( value - LastPercent );
            if( difference == 0 ) return;

            Animation?.Delete();
            if( difference >= 0.01f ) {
                Animation = Animation.AddAnim( SetPercentInternal, 0.2f, LastPercent, value );
            }
            else {
                SetPercentInternal( value );
            }
            LastPercent = value;
        }

        public void SetIndicatorPercent( float indicatorPercent, float valuePercent ) {
            if( indicatorPercent <= 0f || indicatorPercent >= 1f ) {
                Indicator.IsVisible = false;
                return;
            }

            // var canSlidecast = valuePercent >= ( 1f - indicatorPercent );
            Indicator.IsVisible = true;
            var width = ( int )( 160 * indicatorPercent );
            Indicator.Size = new( width, 20 );
            Indicator.Position = new( 160 - width, 0 );
        }

        public void SetPercentInternal( float value ) {
            if( Segments == null ) {
                BarMain.Size = new( ( int )( 148 * value ), 20 );
                BarSecondary.Size = new( 0, 20 );
            }
            else {
                var fullValue = 0f;
                var partialValue = value;

                var _segment = false;
                for( var i = 0; i < Segments.Length; i++ ) {
                    if( Segments[i] <= value ) fullValue = Segments[i];
                    else {
                        _segment = true;
                        break;
                    }
                }
                if( !_segment ) fullValue = value; // not less than any segment
                if( fullValue == value ) partialValue = 0;

                var fullWidth = ( int )( 148 * fullValue );
                var partialWidth = ( int )( 148 * partialValue );

                BarMain.Size = new( fullWidth, 20 );
                BarSecondary.Size = new( partialWidth, 20 );
            }
        }

        public void SetColor( ElementColor color ) {
            color.SetColor( BarMain );
        }

        public void SetSegments( float[] segments ) {
            if( segments == null ) {
                ClearSegments();
                return;
            }

            Segments = segments;

            for( var i = 0; i < MAX_SEGMENTS - 1; i++ ) {
                if( i < segments.Length && segments[i] > 0f && segments[i] < 1f ) {
                    Separators[i].IsVisible = true;
                    Separators[i].X = 8 + ( int )( 148 * segments[i] );
                }
                else {
                    Separators[i].IsVisible = false;
                }
            }
        }

        public void ClearSegments() {
            Segments = null;
            foreach( var item in Separators ) item.IsVisible = false;
        }
    }
}
