using Dalamud.Game.ClientState.Actors.Resolvers;
using Dalamud.Plugin;
using JobBars.Data;
using JobBars.Gauges;
using JobBars.Helper;
using JobBars.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Buffs {
    public unsafe class BuffManager {
        public UIBuilder UI;

        public Dictionary<JobIds, Buff[]> JobToBuffs;
        public JobIds CurrentJob = JobIds.OTHER;
        public Buff[] CurrentGauges => JobToBuffs[CurrentJob];
        private DateTime LastTick = DateTime.Now;

        public BuffManager(UIBuilder ui) {
            UI = ui;

            JobToBuffs = new Dictionary<JobIds, Buff[]>();
            JobToBuffs.Add(JobIds.OTHER, new Buff[] { });
            JobToBuffs.Add(JobIds.GNB, new Buff[] {  // <===== GNB
            });
            JobToBuffs.Add(JobIds.PLD, new Buff[]  { // <===== PLD
            });
            JobToBuffs.Add(JobIds.WAR, new Buff[] { // <===== WAR
                new Buff("Inner Release", IconIds.InnerRelease, 10)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.InnerRelease)
                    })
                    .WithCD(90)
            });
            JobToBuffs.Add(JobIds.DRK, new Buff[] { // <===== DRK
                new Buff("Delirium", IconIds.Delirium, 10)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Delirium)
                    })
                    .WithCD(90),
                new Buff("Living Shadow", IconIds.LivingShadow, 24)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.LivingShadow)
                    })
                    .WithCD(120)
            });
            JobToBuffs.Add(JobIds.AST, new Buff[] { // <===== AST
                new Buff("The Balance", IconIds.TheBalance, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Balance)
                    })
                    .WithNoCD(),
                new Buff("The Bole", IconIds.TheBole, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Bole)
                    })
                    .WithNoCD(),
                new Buff("The Spear", IconIds.TheSpear, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Spear)
                    })
                    .WithNoCD(),
                new Buff("The Spire", IconIds.TheSpire, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Spire)
                    })
                    .WithNoCD(),
                new Buff("The Arrow", IconIds.TheArrow, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Arrow)
                    })
                    .WithNoCD(),
                new Buff("The Ewer", IconIds.TheEwer, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Ewer)
                    })
                    .WithNoCD(),
                new Buff("Lady of Crowns", IconIds.LadyOfCrowns, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.LadyOfCrowns)
                    })
                    .WithNoCD(),
                new Buff("Lord of Crowns", IconIds.LordOfCrowns, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.LordOfCrowns)
                    })
                    .WithNoCD(),
                new Buff("Divination", IconIds.Divination, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.Divination)
                    })
                    .WithCD(120)
            });
            JobToBuffs.Add(JobIds.SCH, new Buff[] { });
            JobToBuffs.Add(JobIds.WHM, new Buff[] { });
            JobToBuffs.Add(JobIds.BRD, new Buff[] { });
            JobToBuffs.Add(JobIds.DRG, new Buff[] { });
            JobToBuffs.Add(JobIds.SMN, new Buff[] { });
            JobToBuffs.Add(JobIds.SAM, new Buff[] { });
            JobToBuffs.Add(JobIds.BLM, new Buff[] { });
            JobToBuffs.Add(JobIds.RDM, new Buff[] { });
            JobToBuffs.Add(JobIds.MCH, new Buff[] { });
            JobToBuffs.Add(JobIds.DNC, new Buff[] { });
            JobToBuffs.Add(JobIds.NIN, new Buff[] { });
            JobToBuffs.Add(JobIds.MNK, new Buff[] { });
            JobToBuffs.Add(JobIds.BLU, new Buff[] { });

            SetupUI();
        }

        public void SetupUI() {
            foreach(var jobEntry in JobToBuffs) {
                var buffs = jobEntry.Value;
                foreach(var buff in buffs) {
                    buff.UI = UI.IconToBuff[buff.Icon];
                }
            }
        }

        public void SetJob(JobIds job) {
            CurrentJob = job;
        }

        public void PerformAction(Item action) {

        }

        public void Tick() {
            var currentTime = DateTime.Now;
            float deltaSecond = (float)(currentTime - LastTick).TotalSeconds;



            LastTick = currentTime;
        }
    }
}
