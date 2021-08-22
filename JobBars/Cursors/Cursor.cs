using JobBars.Data;
using JobBars.Helper;
using System;

namespace JobBars.Cursors {
    public enum CursorType {
        None,
        GCD,
        CastTime,
        MpTick,
        StaticCircle
    }

    public class Cursor {
        public static readonly CursorType[] ValidCursorType = (CursorType[])Enum.GetValues(typeof(CursorType));

        private readonly string Name;
        private readonly string InnerName;
        private readonly string OuterName;

        private CursorType InnerType;
        private CursorType OuterType;

        public Cursor(JobIds job, CursorType inner, CursorType outer) {
            Name = $"{job}";
            InnerName = Name + "_Inner";
            OuterName = Name + "_Outer";

            InnerType = JobBars.Config.CursorType.Get(InnerName, inner);
            OuterType = JobBars.Config.CursorType.Get(OuterName, outer);
        }

        public float GetInner() => GetValue(InnerType);
        public float GetOuter() => GetValue(OuterType);

        private float GetValue(CursorType type) => type switch {
            CursorType.None => 0,
            CursorType.GCD => GetGCD(),
            CursorType.CastTime => GetCastTime(),
            CursorType.MpTick => UIHelper.GetMpTick(),
            CursorType.StaticCircle => 2,
            _ => 0
        };
        
        private static float GetGCD() {
            var recast = UIHelper.GetRecastActiveAndTotal((uint)ActionIds.GoringBlade, out var timeElapsed, out var total);
            if (!recast || total == 0) return 0;
            return timeElapsed / total;
        }

        private static float GetCastTime() {
            var isCasting = UIHelper.GetCurrentCast(out var currentTime, out var totalTime);
            if (!isCasting || totalTime == 0) return 0;
            return currentTime / totalTime;
        }

        public void Draw(string _ID) {
            if(JobBars.Config.CursorType.Draw($"Inner Type{_ID}", InnerName, ValidCursorType, InnerType, out var newInnerValue)){
                InnerType = newInnerValue;
            }

            if (JobBars.Config.CursorType.Draw($"Outer Type{_ID}", OuterName, ValidCursorType, OuterType, out var newOuterValue)) {
                OuterType = newOuterValue;
            }
        }
    }
}
