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

        List<Buff> AllBuffs;
        public Dictionary<JobIds, Buff[]> JobToBuffs;
        public JobIds CurrentJob = JobIds.OTHER;
        public Buff[] CurrentGauges => JobToBuffs[CurrentJob];
        private DateTime LastTick = DateTime.Now;

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
            });
            JobToBuffs.Add(JobIds.DRK, new Buff[] {
                new Buff("Delirium", IconIds.Delirium, 10)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.Delirium)
                    })
                    .WithCD(90),
                new Buff("Living Shadow", IconIds.LivingShadow, 24)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.LivingShadow)
                    })
                    .WithCD(120)
            });
            // ======= AST ==========
            JobToBuffs.Add(JobIds.AST, new Buff[] {
                new Buff("The Balance", IconIds.TheBalance, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.TheBalance)
                    })
                    .WithNoCD(),
                new Buff("The Bole", IconIds.TheBole, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.TheBole)
                    })
                    .WithNoCD(),
                new Buff("The Spear", IconIds.TheSpear, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.TheSpear)
                    })
                    .WithNoCD(),
                new Buff("The Spire", IconIds.TheSpire, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.TheSpire)
                    })
                    .WithNoCD(),
                new Buff("The Arrow", IconIds.TheArrow, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.TheArrow)
                    })
                    .WithNoCD(),
                new Buff("The Ewer", IconIds.TheEwer, 15)
                    .WithTriggers(new []
                    {
                        new Item(BuffIds.TheEwer)
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
                        new Item(ActionIds.Divination)
                    })
                    .WithCD(120)
            });
            // ======= SCH ==========
            JobToBuffs.Add(JobIds.SCH, new Buff[] {
                new Buff("Chain Stratagem", IconIds.ChainStratagem, 15)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.ChainStratagem)
                    })
                    .WithCD(120)
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
                    .WithCD(180),
                new Buff("Raging Strikes", IconIds.RagingStrikes, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.RagingStrikes)
                    })
                    .WithCD(80)
            });
            // ======= DRG ==========
            JobToBuffs.Add(JobIds.DRG, new Buff[] {
                new Buff("Dragon Sight", IconIds.DragonSight, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.DragonSight)
                    })
                    .WithCD(120),
                new Buff("Battle Litany", IconIds.BattleLitany, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.BattleLitany)
                    })
                    .WithCD(180),
                new Buff("Lance Charge", IconIds.LanceCharge, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.LanceCharge)
                    })
                    .WithCD(90)
            });
            // ======= SMN ==========
            JobToBuffs.Add(JobIds.SMN, new Buff[] {
                new Buff("Devotion", IconIds.Devotion, 15)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.Devotion)
                    })
                    .WithCD(180),
                new Buff("Summon Bahamut", IconIds.SummonBahamut, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.SummonBahamut)
                    })
                    .WithCD(60),
                new Buff("Firebird Trance", IconIds.FirebirdTrance, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.FirebirdTrance)
                    })
                    .WithCD(60)
            });
            // ======= SAM ==========
            JobToBuffs.Add(JobIds.SAM, new Buff[] {
                new Buff("Double Midare", IconIds.DoubleMidare, 5)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.DoubleMidare)
                    })
                    .WithCD(60)
            });
            // ======= BLM ==========
            JobToBuffs.Add(JobIds.BLM, new Buff[] {
            });
            // ======= RDM ==========
            JobToBuffs.Add(JobIds.RDM, new Buff[] {
                new Buff("Manafication", IconIds.Manafication, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.Manafication)
                    })
                    .WithCD(120),
                new Buff("Embolden", IconIds.Embolden, 10)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.Embolden)
                    })
                    .WithCD(110)
            });
            // ======= MCH ==========
            JobToBuffs.Add(JobIds.MCH, new Buff[] {
                new Buff("Wildfire", IconIds.Wildfire, 10)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.Wildfire)
                    })
                    .WithCD(120)
            });
            // ======= DNC ==========
            JobToBuffs.Add(JobIds.DNC, new Buff[] {
                new Buff("Technical Finish", IconIds.TechnicalFinish, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.QuadTechFinish)
                    })
                    .WithCD(120),
                new Buff("Devilment", IconIds.Devilment, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.Devilment)
                    })
                    .WithCD(120)
            });
            // ======= NIN ==========
            JobToBuffs.Add(JobIds.NIN, new Buff[] {
                new Buff("Trick Attack", IconIds.TrickAttack, 15)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.TrickAttack)
                    })
                    .WithCD(60),
                new Buff("Bunshin", IconIds.Bunshin, 15)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.Bunshin)
                    })
                    .WithCD(90)
            });
            // ======= MNK ==========
            JobToBuffs.Add(JobIds.MNK, new Buff[] {
                new Buff("Brotherhood", IconIds.Brotherhood, 15)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.Brotherhood)
                    })
                    .WithCD(90),
                new Buff("Riddle of Fire", IconIds.RiddleOfFire, 20)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.RiddleOfFire)
                    })
                    .WithCD(90),
                new Buff("Perfect Balance", IconIds.PerfectBalance, 15)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.PerfectBalance)
                    })
                    .WithCD(90)
            });
            // ======= BLU ==========
            JobToBuffs.Add(JobIds.BLU, new Buff[] {
                new Buff("Off-Guard", IconIds.OffGuard, 15)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.OffGuard)
                    })
                    .WithCD(60),
                new Buff("Peculiar Light", IconIds.PeculiarLight, 15)
                    .WithTriggers(new []
                    {
                        new Item(ActionIds.PeculiarLight)
                    })
                    .WithCD(60)
            });

            AllBuffs = new List<Buff>();
            foreach (var jobEntry in JobToBuffs) {
                var buffs = jobEntry.Value;
                foreach (var buff in buffs) {
                    AllBuffs.Add(buff);
                }
            }
            SetupUI();
        }

        public void SetupUI() {
            foreach(var buff in AllBuffs) {
                buff.UI = UI.IconToBuff[buff.Icon];
            }
        }

        public void SetJob(JobIds job) {
            Reset();
            CurrentJob = job;

            foreach(var buff in AllBuffs) {
                buff.Enabled = !Configuration.Config.BuffDisabled.Contains(buff.Name);
            }
        }

        public void Reset() {
            foreach (var buff in AllBuffs) {
                buff.State = BuffState.INACTIVE;
            }
            UI.HideAllBuffs();
        }

        public void PerformAction(Item action) {
            foreach (var buff in AllBuffs) {
                if(!buff.Enabled) { continue; }
                buff.ProcessAction(action);
            }
        }

        public void Tick() {
            var currentTime = DateTime.Now;
            float deltaSecond = (float)(currentTime - LastTick).TotalSeconds;

            var idx = 0;
            foreach (var buff in AllBuffs) {
                if (!buff.Enabled) { continue; }
                buff.Tick(currentTime, deltaSecond);
                if (buff.Visible) {
                    buff.UI.SetPosition(idx);
                    idx++;
                }
            }

            LastTick = currentTime;
        }
    }
}
