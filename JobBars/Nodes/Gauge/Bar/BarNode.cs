using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Atk;
using JobBars.Data;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using System;
using System.Collections.Generic;

namespace JobBars.Nodes.Gauge.Bar {
    public unsafe class BarNode : GaugeNode {
        private static readonly int MAX_SEGMENTS = 6;

        private readonly ResNode GaugeContainer;

        private readonly SimpleImageNode Background;
        private readonly ResNode BarContainer;
        private readonly SimpleNineGridNode BarSecondary;
        private readonly SimpleNineGridNode BarMain;
        private readonly List<SimpleImageNode> Separators = [];
        private readonly SimpleImageNode Frame;
        private readonly SimpleNineGridNode Indicator;

        private readonly ResNode TextContainer;
        private readonly TextNode Text;
        private readonly SimpleNineGridNode TextBlur;

        private string CurrentText;
        private float LastPercent = 1f;
        private Animation Animation;
        private float[] Segments = null;

        private bool Vertical = false;
        private bool TextSwap = false;

        public BarNode() : base() {
            NodeID = JobBars.NodeId++;
            NodeFlags |= NodeFlags.AnchorLeft | NodeFlags.AnchorTop;
            Size = new( 160, 46 );

            GaugeContainer = new ResNode() {
                NodeID = JobBars.NodeId++,
                Size = new( 160, 32 ),
                NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop,
            };

            Background = new() {
                NodeID = JobBars.NodeId++,
                Size = new( 160, 20 ),
                TextureCoordinates = new( 0, 50 ),
                TextureSize = new( 80, 10 ),
                NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop,
                WrapMode = WrapMode.Unknown,
                ImageNodeFlags = 0,
            };
            Background.LoadTexture( "ui/uld/Parameter_Gauge.tex", JobBars.Configuration.Use4K );

            // ========= BAR ==============

            BarContainer = new ResNode() {
                NodeID = JobBars.NodeId++,
                Size = new( 160, 20 ),
                NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop,
            };

            BarMain = new() {
                NodeID = JobBars.NodeId++,
                Size = new( 160, 20 ),
                TextureCoordinates = new( 0, 20 ),
                TextureSize = new( 80, 10 ),
                LeftOffset = 7,
                RightOffset = 7,
                PartsRenderType = PartsRenderType.RenderType,
                NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop,
            };
            BarMain.LoadTexture( "ui/uld/Parameter_Gauge.tex", JobBars.Configuration.Use4K );

            BarSecondary = new() {
                NodeID = JobBars.NodeId++,
                Size = new( 0, 20 ),
                TextureCoordinates = new( 0, 20 ),
                TextureSize = new( 80, 10 ),
                LeftOffset = 7,
                RightOffset = 7,
                PartsRenderType = PartsRenderType.RenderType,
                NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop,
            };
            BarSecondary.LoadTexture( "ui/uld/Parameter_Gauge.tex", JobBars.Configuration.Use4K );

            for( var i = 0; i < MAX_SEGMENTS - 1; i++ ) {
                Separators.Add( new() {
                    NodeID = JobBars.NodeId++,
                    Rotation = ( float )( Math.PI / 2f ),
                    Size = new( 10, 5 ),
                    Position = new( 0, 5 ),
                    TextureCoordinates = new( 10, 3 ),
                    TextureSize = new( 10, 5 ),
                    NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorTop,
                    WrapMode = WrapMode.Unknown,
                    ImageNodeFlags = 0,
                } );
                Separators[i].LoadTexture( "ui/uld/Parameter_Gauge.tex", JobBars.Configuration.Use4K );
                Separators[i].InternalResNode->DrawFlags = 1 | 4;
            }

            // ======== FRAME ============

            Frame = new() {
                NodeID = JobBars.NodeId++,
                Size = new( 160, 20 ),
                TextureCoordinates = new( 0, 0 ),
                TextureSize = new( 160, 20 ),
                NodeFlags = NodeFlags.Visible,
                WrapMode = WrapMode.Unknown,
                ImageNodeFlags = 0,
            };
            Frame.LoadTexture( "ui/uld/Parameter_Gauge.tex", JobBars.Configuration.Use4K );

            Indicator = new() {
                NodeID = JobBars.NodeId++,
                Size = new( 160, 20 ),
                TextureSize = new( 80, 10 ),
                LeftOffset = 15,
                RightOffset = 15,
                PartsRenderType = PartsRenderType.RenderType,
                NodeFlags = NodeFlags.Visible,
            };
            Indicator.LoadTexture( "ui/uld/Parameter_Gauge.tex", JobBars.Configuration.Use4K );

            // ======= TEXT ==============

            TextContainer = new ResNode() {
                NodeID = JobBars.NodeId++,
                Size = new( 47, 40 ),
                Position = new( 112, 6 ),
                NodeFlags = NodeFlags.Visible,
            };

            Text = new TextNode() {
                NodeID = JobBars.NodeId++,
                Position = new( 14, 5 ),
                Size = new( 17, 30 ),
                FontSize = 18,
                LineSpacing = 18,
                AlignmentFontType = 21,
                NodeFlags = NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.AnchorRight,
                TextColor = new( 1, 1, 1, 1 ),
                TextOutlineColor = new( 157f / 255f, 131f / 255f, 91f / 255f, 1 ),
                TextId = 0,
                TextFlags = TextFlags.Glare,
                TextFlags2 = 0,
                Text = "",
            };

            TextBlur = new() {
                NodeID = JobBars.NodeId++,
                Size = new( 47, 48 ),
                TextureSize = new( 60, 40 ),
                LeftOffset = 28,
                RightOffset = 28,
                PartsRenderType = ( PartsRenderType )128,
                NodeFlags = NodeFlags.Visible | NodeFlags.Fill | NodeFlags.AnchorLeft | NodeFlags.AnchorTop,
            };
            TextBlur.LoadTexture( "ui/uld/JobHudNumBg.tex", JobBars.Configuration.Use4K );

            JobBars.NativeController.AttachToNode( [
                BarSecondary,
                BarMain,
                ..Separators
            ], BarContainer, NodePosition.AsLastChild );

            JobBars.NativeController.AttachToNode( [
                Background,
                BarContainer,
                Frame,
                Indicator,
            ], GaugeContainer, NodePosition.AsLastChild );

            JobBars.NativeController.AttachToNode( [
                TextBlur,
                Text,
            ], TextContainer, NodePosition.AsLastChild );

            JobBars.NativeController.AttachToNode( [
                GaugeContainer,
                TextContainer
            ], this, NodePosition.AsLastChild );
        }

        public void SetText( string text ) {
            if( text != CurrentText ) {
                Text.Text = text;
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
                GaugeContainer.DrawFlags |= 1 | 4;
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
                BarMain.Size = new( ( int )( 160 * value ), 20 );
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

                var fullWidth = ( int )( 160 * fullValue );
                var partialWidth = ( int )( 160 * partialValue );

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

        protected override void Dispose( bool disposing ) {
            if( disposing ) {
                GaugeContainer.Dispose();
                Background.Dispose();
                BarContainer.Dispose();
                BarMain.Dispose();
                BarSecondary.Dispose();
                Frame.Dispose();
                Indicator.Dispose();
                TextContainer.Dispose();
                Text.Dispose();
                TextBlur.Dispose();
                foreach( var item in Separators ) item.Dispose();
                base.Dispose( disposing );
            }
        }
    }
}
