using ImGuiNET;
using JobBars.Data;
using JobBars.Gauges.Types;
using JobBars.Gauges.Types.Arrow;
using JobBars.Gauges.Types.Bar;
using JobBars.Gauges.Types.BarDiamondCombo;
using JobBars.Gauges.Types.Diamond;
using System;
using System.Linq;
using System.Numerics;

namespace JobBars.Gauges {
    public abstract class GaugeConfig {
        public readonly string Name;
        public GaugeVisualType Type { get; private set; }
        public GaugeTypeConfig TypeConfig { get; private set; }

        public bool Enabled { get; protected set; }
        public int Order { get; private set; }
        public float Scale { get; private set; }
        public bool HideWhenInactive { get; private set; }
        public int SoundEffect { get; private set; }
        public int CompletionSoundEffect { get; private set; }
        public Vector2 Position => JobBars.Configuration.GaugeSplitPosition.Get( Name );

        public static readonly GaugeCompleteSoundType[] ValidSoundType = ( GaugeCompleteSoundType[] )Enum.GetValues( typeof( GaugeCompleteSoundType ) );

        public GaugeConfig( string name, GaugeVisualType type ) {
            Name = name;
            Enabled = JobBars.Configuration.GaugeEnabled.Get( Name );
            Order = JobBars.Configuration.GaugeOrder.Get( Name );
            Scale = JobBars.Configuration.GaugeIndividualScale.Get( Name );
            HideWhenInactive = JobBars.Configuration.GaugeHideInactive.Get( Name );
            SoundEffect = JobBars.Configuration.GaugeSoundEffect_2.Get( Name );
            CompletionSoundEffect = JobBars.Configuration.GaugeCompletionSoundEffect_2.Get( Name );
            SetType( JobBars.Configuration.GaugeType.Get( Name, type ) );
        }

        public abstract GaugeTracker GetTracker( int idx );

        private void SetType( GaugeVisualType type ) {
            var validTypes = GetValidGaugeTypes();
            Type = validTypes.Contains( type ) ? type : validTypes[0];
            TypeConfig = Type switch {
                GaugeVisualType.Bar => new GaugeBarConfig( Name ),
                GaugeVisualType.Diamond => new GaugeDiamondConfig( Name ),
                GaugeVisualType.Arrow => new GaugeArrowConfig( Name ),
                GaugeVisualType.BarDiamondCombo => new GaugeBarDiamondComboConfig( Name ),
                _ => null
            };
        }

        public void Draw( string id, out bool newVisual, out bool reset ) {
            newVisual = reset = false;

            if( JobBars.Configuration.GaugeEnabled.Draw( $"Enabled{id}", Name, Enabled, out var newEnabled ) ) {
                Enabled = newEnabled;
                newVisual = true;
            }

            if( JobBars.Configuration.GaugeHideInactive.Draw( $"Hide when inactive{id}", Name, HideWhenInactive, out var newHideWhenInactive ) ) {
                HideWhenInactive = newHideWhenInactive;
            }

            if( JobBars.Configuration.GaugeIndividualScale.Draw( $"Scale{id}", Name, out var newScale ) ) {
                Scale = Math.Max( 0.1f, newScale );
                newVisual = true;
            }

            if( JobBars.Configuration.GaugePositionType == GaugePositionType.Split ) {
                if( JobBars.Configuration.GaugeSplitPosition.Draw( $"Split position{id}", Name, out var newPosition ) ) {
                    SetSplitPosition( newPosition );
                    newVisual = true;
                }
            }

            if( JobBars.Configuration.GaugeOrder.Draw( $"Order{id}", Name, Order, out var newOrder ) ) {
                Order = newOrder;
                newVisual = true;
            }

            var validTypes = GetValidGaugeTypes();
            if( validTypes.Length > 1 ) {
                if( JobBars.Configuration.GaugeType.Draw( $"Type{id}", Name, validTypes, Type, out var newType ) ) {
                    SetType( newType );
                    reset = true;
                }
            }

            TypeConfig.Draw( id, ref newVisual, ref reset );

            DrawConfig( id, ref newVisual, ref reset );
        }

        protected void DrawSoundEffect( string label = "Progress sound effect" ) {
            if( ImGui.Button( "Test##SoundEffect" ) ) Helper.AtkHelper.PlaySoundEffect( SoundEffect );
            ImGui.SameLine();

            ImGui.SetNextItemWidth( 200f );
            if( JobBars.Configuration.GaugeSoundEffect_2.Draw( $"{label} (0 = off)", Name, SoundEffect, out var newSoundEffect ) ) {
                SoundEffect = newSoundEffect;
            }
            ImGui.SameLine();
            HelpMarker( "For macro sound effects, add 36. For example, <se.6> would be 6+36=42" );
        }

        public void PlaySoundEffect() => Helper.AtkHelper.PlaySoundEffect( SoundEffect );

        protected void DrawCompletionSoundEffect() {
            if( ImGui.Button( "Test##CompletionSoundEffect" ) ) Helper.AtkHelper.PlaySoundEffect( CompletionSoundEffect );
            ImGui.SameLine();

            ImGui.SetNextItemWidth( 200f );
            if( JobBars.Configuration.GaugeCompletionSoundEffect_2.Draw( $"Completion sound effect (0 = off)", Name, CompletionSoundEffect, out var newSoundEffect ) ) {
                CompletionSoundEffect = newSoundEffect;
            }
            ImGui.SameLine();
            HelpMarker( "For macro sound effects, add 36. For example, <se.6> would be 6+36=42" );
        }

        public void PlayCompletionSoundEffect() => Helper.AtkHelper.PlaySoundEffect( CompletionSoundEffect );

        public static void HelpMarker( string text ) {
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.TextDisabled( "(?)" );
            if( ImGui.IsItemHovered() ) {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos( ImGui.GetFontSize() * 35.0f );
                ImGui.TextUnformatted( text );
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }

        public void DrawPositionBox() {
            if( JobBars.DrawPositionView( Name + "##GaugePosition", Position, out var pos ) ) {
                JobBars.Configuration.GaugeSplitPosition.Set( Name, pos );
                SetSplitPosition( pos );
                JobBars.GaugeManager.UpdatePositionScale();
            }
        }

        protected abstract GaugeVisualType[] GetValidGaugeTypes();

        protected abstract void DrawConfig( string id, ref bool newVisual, ref bool reset );

        private void SetSplitPosition( Vector2 pos ) {
            JobBars.SetWindowPosition( Name + "##GaugePosition", pos );
        }
    }
}
