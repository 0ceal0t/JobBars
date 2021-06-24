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
        public Dictionary<JobIds, Buff[]> JobToBuffs;
        public Buff[] CurrentBuffs => JobToBuffs.TryGetValue(CurrentJob, out var gauges) ? gauges : JobToBuffs[JobIds.OTHER];

        private UIBuilder UI;
        private List<Buff> AllBuffs;
        private JobIds CurrentJob = JobIds.OTHER;

        public BuffManager(UIBuilder ui) {
            UI = ui;

            JobToBuffs = new Dictionary<JobIds, Buff[]>();
            JobToBuffs.Add(JobIds.OTHER, new Buff[] { });
            // ======= GNB ==========
            JobToBuffs.Add(JobIds.GNB, new Buff[] {
            });
            // ======= PLD ==========
            JobToBuffs.Add(JobIds.PLD, new Buff[]  {
            });
            // ======= WAR ==========
            JobToBuffs.Add(JobIds.WAR, new Buff[] {
                new Buff("Inner Release", IconIds.InnerRelease, 10)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.InnerRelease)
                    })
                    .WithCD(90)
                    .WithColor(UIColor.Orange)
            });
            JobToBuffs.Add(JobIds.DRK, new Buff[] {
                new Buff("Delirium", IconIds.Delirium, 10)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.Delirium)
                    })
                    .WithCD(90)
                    .WithColor(UIColor.Red),
                new Buff("Living Shadow", IconIds.LivingShadow, 24)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.LivingShadow)
                    })
                    .WithCD(120)
                    .WithColor(UIColor.Purple)
            });
            // ======= AST ==========
            JobToBuffs.Add(JobIds.AST, new Buff[] {
                new Buff("The Balance", IconIds.TheBalance, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.TheBalance)
                    })
                    .WithNoCD()
                    .WithColor(UIColor.Orange),
                new Buff("The Bole", IconIds.TheBole, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.TheBole)
                    })
                    .WithNoCD()
                    .WithColor(UIColor.BrightGreen),
                new Buff("The Spear", IconIds.TheSpear, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.TheSpear)
                    })
                    .WithNoCD()
                    .WithColor(UIColor.DarkBlue),
                new Buff("The Spire", IconIds.TheSpire, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.TheSpire)
                    })
                    .WithNoCD()
                    .WithColor(UIColor.Yellow),
                new Buff("The Arrow", IconIds.TheArrow, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.TheArrow)
                    })
                    .WithNoCD()
                    .WithColor(UIColor.LightBlue),
                new Buff("The Ewer", IconIds.TheEwer, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.TheEwer)
                    })
                    .WithNoCD()
                    .WithColor(UIColor.LightBlue),
                new Buff("Lady of Crowns", IconIds.LadyOfCrowns, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.LadyOfCrowns)
                    })
                    .WithNoCD()
                    .WithColor(UIColor.Purple),
                new Buff("Lord of Crowns", IconIds.LordOfCrowns, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.LordOfCrowns)
                    })
                    .WithNoCD()
                    .WithColor(UIColor.Red),
                new Buff("Divination", IconIds.Divination, 15)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.Divination)
                    })
                    .WithCD(120)
                    .WithColor(UIColor.Yellow)
            });
            // ======= SCH ==========
            JobToBuffs.Add(JobIds.SCH, new Buff[] {
                new Buff("Chain Stratagem", IconIds.ChainStratagem, 15)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.ChainStratagem)
                    })
                    .WithCD(120)
                    .WithColor(UIColor.White)
            });
            // ======= WHM ==========
            JobToBuffs.Add(JobIds.WHM, new Buff[] {
            });
            // ======= BRD ==========
            JobToBuffs.Add(JobIds.BRD, new Buff[] {
                new Buff("Battle Voice", IconIds.BattleVoice, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.BattleVoice)
                    })
                    .WithCD(180)
                    .WithColor(UIColor.Orange),
                new Buff("Raging Strikes", IconIds.RagingStrikes, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.RagingStrikes)
                    })
                    .WithCD(80)
                    .WithColor(UIColor.Yellow)
            });
            // ======= DRG ==========
            JobToBuffs.Add(JobIds.DRG, new Buff[] {
                new Buff("Dragon Sight", IconIds.DragonSight, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.DragonSight)
                    })
                    .WithCD(120)
                    .WithColor(UIColor.Orange),
                new Buff("Battle Litany", IconIds.BattleLitany, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.BattleLitany)
                    })
                    .WithCD(180)
                    .WithColor(UIColor.LightBlue),
                new Buff("Lance Charge", IconIds.LanceCharge, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.LanceCharge)
                    })
                    .WithCD(90)
                    .WithColor(UIColor.Red)
            });
            // ======= SMN ==========
            JobToBuffs.Add(JobIds.SMN, new Buff[] {
                new Buff("Devotion", IconIds.Devotion, 15)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.Devotion)
                    })
                    .WithCD(180)
                    .WithColor(UIColor.Yellow),
                new Buff("Summon Bahamut", IconIds.SummonBahamut, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.SummonBahamut)
                    })
                    .WithCD(120)
                    .WithColor(UIColor.LightBlue),
                new Buff("Firebird Trance", IconIds.FirebirdTrance, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.FirebirdTrance)
                    })
                    .WithCD(120)
                    .WithColor(UIColor.Orange)
            });
            // ======= SAM ==========
            JobToBuffs.Add(JobIds.SAM, new Buff[] {
                new Buff("Double Midare", IconIds.DoubleMidare, 5)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.DoubleMidare)
                    })
                    .WithCD(60)
                    .WithColor(UIColor.DarkBlue)
            });
            // ======= BLM ==========
            JobToBuffs.Add(JobIds.BLM, new Buff[] {
            });
            // ======= RDM ==========
            JobToBuffs.Add(JobIds.RDM, new Buff[] {
                new Buff("Manafication", IconIds.Manafication, 10)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.Manafication)
                    })
                    .WithCD(110)
                    .WithColor(UIColor.DarkBlue),
                new Buff("Embolden", IconIds.Embolden, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.Embolden)
                    })
                    .WithCD(120)
                    .WithColor(UIColor.White)
            });
            // ======= MCH ==========
            JobToBuffs.Add(JobIds.MCH, new Buff[] {
                new Buff("Wildfire", IconIds.Wildfire, 10)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.Wildfire)
                    })
                    .WithCD(120)
                    .WithColor(UIColor.Orange)
            });
            // ======= DNC ==========
            JobToBuffs.Add(JobIds.DNC, new Buff[] {
                new Buff("Technical Finish", IconIds.TechnicalFinish, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.QuadTechFinish)
                    })
                    .WithCD(120)
                    .WithColor(UIColor.PurplePink),
                new Buff("Devilment", IconIds.Devilment, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.Devilment)
                    })
                    .WithCD(120)
                    .WithColor(UIColor.BrightGreen)
            });
            // ======= NIN ==========
            JobToBuffs.Add(JobIds.NIN, new Buff[] {
                new Buff("Trick Attack", IconIds.TrickAttack, 15)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.TrickAttack)
                    })
                    .WithCD(60)
                    .WithColor(UIColor.Yellow),
                new Buff("Bunshin", IconIds.Bunshin, 15)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.Bunshin)
                    })
                    .WithCD(90)
                    .WithColor(UIColor.Orange)
            });
            // ======= MNK ==========
            JobToBuffs.Add(JobIds.MNK, new Buff[] {
                new Buff("Brotherhood", IconIds.Brotherhood, 15)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.Brotherhood)
                    })
                    .WithCD(90)
                    .WithColor(UIColor.Orange),
                new Buff("Riddle of Fire", IconIds.RiddleOfFire, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.RiddleOfFire)
                    })
                    .WithCD(90)
                    .WithColor(UIColor.Red),
                new Buff("Perfect Balance", IconIds.PerfectBalance, 15)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.PerfectBalance)
                    })
                    .WithCD(90)
                    .WithColor(UIColor.Orange)
            });
            // ======= BLU ==========
            JobToBuffs.Add(JobIds.BLU, new Buff[] {
                new Buff("Off-Guard", IconIds.OffGuard, 15)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.OffGuard)
                    })
                    .WithCD(60)
                    .WithColor(UIColor.BrightGreen),
                new Buff("Peculiar Light", IconIds.PeculiarLight, 15)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.PeculiarLight)
                    })
                    .WithCD(60)
                    .WithColor(UIColor.Red)
            });

            AllBuffs = new List<Buff>();
            foreach (var jobEntry in JobToBuffs) {
                foreach (var buff in jobEntry.Value) {
                    buff.UI = UI.IconToBuff[buff.Icon];
                    buff.SetupVisual();
                    AllBuffs.Add(buff);
                }
            }
        }

        public void SetJob(JobIds job) {
            CurrentJob = job;
            foreach(var buff in AllBuffs) {
                buff.State = BuffState.Inactive;
                buff.Enabled = !Configuration.Config.BuffDisabled.Contains(buff.Name);
            }
            UI.HideAllBuffs();
            SetPositionScale();
        }

        public void SetPositionScale() {
            UI.SetBuffPosition(Configuration.Config.BuffPosition);
            UI.SetBuffScale(Configuration.Config.BuffScale);
        }

        public void Reset() {
            SetJob(CurrentJob);
        }

        public void PerformAction(Item action) {
            if (!Configuration.Config.BuffBarEnabled) return;
            foreach (var buff in AllBuffs) {
                if(!buff.Enabled) { continue; }
                buff.ProcessAction(action);
            }
        }

        public void Tick() {
            if (!Configuration.Config.BuffBarEnabled) return;
            var currentTime = DateTime.Now;

            var idx = 0;
            foreach (var buff in AllBuffs) {
                if (!buff.Enabled) { continue; }
                buff.Tick(currentTime);
                if (buff.Visible) {
                    buff.UI.SetPosition(idx);
                    idx++;
                }
            }
        }
    }
}
