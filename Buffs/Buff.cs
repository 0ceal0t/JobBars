using JobBars.Data;
using JobBars.Gauges;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Buffs {
    public class Buff {
        public string Name;
        public IconIds Icon;
        public Item[] Triggers;
        public UIBuff UI = null;

        public bool Active = false;
        public bool Enabled = true;
        public Item LastActiveTrigger;
        public DateTime ActiveTime;

        public float Duration;
        public float CD;
        public bool NoCD = false;

        public Buff(string name, IconIds icon, float duration) {
            Name = name;
            Icon = icon;
            Duration = duration;
            Triggers = new Item[0];
        }

        // ===== BUILDER FUNCS ========
        public Buff WithTriggers(Item[] triggers) {
            Triggers = triggers;
            return this;
        }
        public Buff WithCD(float cd) {
            CD = cd;
            return this;
        }
        public Buff WithNoCD() {
            NoCD = true;
            return this;
        }

        public void SetActive(Item trigger) {
            Active = true;
            LastActiveTrigger = trigger;
            ActiveTime = DateTime.Now;
        }

        public void Setup() {

        }
        public void ProcessAction(Item action) {

        }

        public void Tick(DateTime time, float delta) {

        }
    }
}
