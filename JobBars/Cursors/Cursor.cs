using JobBars.Data;
using JobBars.Helper;
using System;

namespace JobBars.Cursors {
    public enum CursorType {
        None,
        GCD,
        CastTime,
        MpTick,
        ActorTick,
        DoT_Tick,
        StaticCircle,
        StaticRing,
        StatusTime,
        Slidecast
    }

    public class Cursor {
        public static readonly CursorType[] ValidCursorType = ( CursorType[] )Enum.GetValues( typeof( CursorType ) );

        private readonly string Name;
        private readonly string InnerName;
        private readonly string OuterName;

        private CursorType InnerType;
        private CursorType OuterType;

        private ItemData InnerStatus;
        private ItemData OuterStatus;
        private float InnerStatusDuration;
        private float OuterStatusDuration;

        public Cursor( JobIds job, CursorType inner, CursorType outer ) {
            Name = $"{job}";
            InnerName = Name + "_Inner";
            OuterName = Name + "_Outer";

            InnerType = JobBars.Configuration.CursorType.Get( InnerName, inner );
            OuterType = JobBars.Configuration.CursorType.Get( OuterName, outer );

            InnerStatus = JobBars.Configuration.CursorStatus.Get( InnerName, new ItemData {
                Name = "[NONE]",
                Data = new Item()
            } );
            OuterStatus = JobBars.Configuration.CursorStatus.Get( OuterName, new ItemData {
                Name = "[NONE]",
                Data = new Item()
            } );
            InnerStatusDuration = JobBars.Configuration.CursorStatusDuration.Get( InnerName, 5f );
            OuterStatusDuration = JobBars.Configuration.CursorStatusDuration.Get( OuterName, 5f );
        }

        public float GetInner() => GetValue( InnerType, InnerStatus, InnerStatusDuration );
        public float GetOuter() => GetValue( OuterType, OuterStatus, OuterStatusDuration );

        private static float GetValue( CursorType type, ItemData status, float statusDuration ) => type switch {
            CursorType.None => 0,
            CursorType.GCD => AtkHelper.GetGCD( out var _, out var _ ),
            CursorType.CastTime => AtkHelper.GetCastTime( out var _, out var _ ),
            CursorType.MpTick => AtkHelper.GetMpTick(),
            CursorType.ActorTick => AtkHelper.GetActorTick(),
            CursorType.StaticCircle => 2, // just a placeholder value, doesn't actually matter
            CursorType.StaticRing => 1,
            CursorType.StatusTime => GetStatusTime( status, statusDuration ),
            CursorType.DoT_Tick => AtkHelper.GetDoTTick(),
            CursorType.Slidecast => GetSlidecastTime(),
            _ => 0
        };

        private static float GetStatusTime( ItemData status, float statusDuration ) {
            if( statusDuration == 0 ) return 0;
            if( status.Data.Id == 0 ) return 0;
            var ret = ( AtkHelper.PlayerStatus.TryGetValue( status.Data, out var value ) ? ( value.RemainingTime > 0 ? value.RemainingTime : value.RemainingTime * -1 ) : 0 ) / statusDuration;
            return Math.Min( ret, 1f );
        }

        private static float GetSlidecastTime() {
            if( JobBars.Configuration.GaugeSlidecastTime <= 0f ) return AtkHelper.GetCastTime( out var _, out var _ );

            var isCasting = AtkHelper.GetCurrentCast( out var currentTime, out var totalTime );
            if( !isCasting || totalTime == 0 ) return 0;
            var slidecastTime = totalTime - JobBars.Configuration.GaugeSlidecastTime;
            if( currentTime > slidecastTime ) return 0;
            return currentTime / slidecastTime;
        }

        public void Draw( string _ID ) {
            if( JobBars.Configuration.CursorType.Draw( $"Inner type{_ID}", InnerName, ValidCursorType, InnerType, out var newInnerValue ) ) {
                InnerType = newInnerValue;
            }

            if( InnerType == CursorType.StatusTime ) {
                if( JobBars.Configuration.CursorStatus.Draw( $"Inner status{_ID}", InnerName, AtkHelper.StatusList, InnerStatus, out var newInnerStatus ) ) {
                    InnerStatus = newInnerStatus;
                }
                if( JobBars.Configuration.CursorStatusDuration.Draw( $"Inner status duration{_ID}", InnerName, InnerStatusDuration, out var newInnerStatusDuration ) ) {
                    InnerStatusDuration = newInnerStatusDuration;
                }
            }

            if( JobBars.Configuration.CursorType.Draw( $"Outer type{_ID}", OuterName, ValidCursorType, OuterType, out var newOuterValue ) ) {
                OuterType = newOuterValue;
            }

            if( OuterType == CursorType.StatusTime ) {
                if( JobBars.Configuration.CursorStatus.Draw( $"Outer status{_ID}", OuterName, AtkHelper.StatusList, OuterStatus, out var newOuterStatus ) ) {
                    OuterStatus = newOuterStatus;
                }
                if( JobBars.Configuration.CursorStatusDuration.Draw( $"Outer status duration{_ID}", OuterName, OuterStatusDuration, out var newOuterStautsDuration ) ) {
                    OuterStatusDuration = newOuterStautsDuration;
                }
            }
        }
    }
}
