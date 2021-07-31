using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Data {
    public class Animation {

        private static List<Animation> Anims = new();

        public static Animation AddAnim(Action<float> function, float duration, float startValue, float endValue) {
            var anim = new Animation(function, duration, startValue, endValue);
            Anims.Add(anim);
            return anim;
        }

        public static void Tick() {
            var currentTime = DateTime.Now;

            foreach (var item in Anims) {
                if (item.Remove) continue;
                var timeElapsed = (currentTime - item.Start).TotalSeconds;
                if (timeElapsed > item.Duration) {
                    timeElapsed = item.Duration;
                }

                var value = item.StartValue + (item.EndValue - item.StartValue) * (timeElapsed / item.Duration);
                item.F((float)value);
            }

            Anims.RemoveAll(x => (currentTime - x.Start).TotalSeconds >= x.Duration || x.Remove);
        }

        public static void Cleanup() {
            Anims = new();
        }

        public Action<float> F;
        public DateTime Start;
        public float Duration;
        public float StartValue;
        public float EndValue;
        public bool Remove = false;

        public Animation(Action<float> function, float duration, float startValue, float endValue) {
            F = function;
            Start = DateTime.Now;
            Duration = duration;
            StartValue = startValue;
            EndValue = endValue;
        }

        public void Delete() {
            Remove = true;
        }
    }
}
