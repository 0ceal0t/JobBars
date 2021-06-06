using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Data {
    public class Animation {

        public static List<Animation> Anims = new List<Animation>();
        public static void AddAnim(Action<float> function, float duration, float startValue, float endValue) {
            Anims.Add(new Animation(function, duration, startValue, endValue));
        }
        
        public static void Tick() {
            var currentTime = DateTime.Now;

            foreach(var item in Anims) {
                var timeElapsed = (currentTime - item.Start).TotalSeconds;
                if(timeElapsed > item.Duration) {
                    timeElapsed = item.Duration;
                }

                var value = item.StartValue + (item.EndValue - item.StartValue) * (timeElapsed / item.Duration);
                item.F((float) value);
            }

            Anims.RemoveAll(x => (currentTime - x.Start).TotalSeconds >= x.Duration);
        }

        public Action<float> F;
        public DateTime Start;
        public float Duration;
        public float StartValue;
        public float EndValue;

        public Animation(Action<float> function, float duration, float startValue, float endValue) {
            F = function;
            Start = DateTime.Now;
            Duration = duration;
            StartValue = startValue;
            EndValue = endValue;
        }
    }
}
