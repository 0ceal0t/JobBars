using Dalamud.Game.ClientState.Actors.Resolvers;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Gauges {
    public class ActionGaugeManager {
        public Dictionary<JobIds, ActionGauge[]> JobToGauges;
        public JobIds CurrentJob;
        public ActionGauge[] CurrentGauges => JobToGauges[CurrentJob];

        public ActionGaugeManager() {
            JobToGauges = new Dictionary<JobIds, ActionGauge[]>();
            JobToGauges.Add(JobIds.OTHER, new ActionGauge[] { });
            JobToGauges.Add(JobIds.GNB, new ActionGauge[] { });
            JobToGauges.Add(JobIds.AST, new ActionGauge[] { });
            JobToGauges.Add(JobIds.PLD, new ActionGauge[] { });
            JobToGauges.Add(JobIds.WAR, new ActionGauge[] { });
            JobToGauges.Add(JobIds.DRK, new ActionGauge[] {
                new ActionGaugeGCD("Delirium", 10, 5)
                .WithTriggers(new[]
                {
                    new Item(BuffIds.Delirium)
                })
                .WithIncrement(new[]{ 
                    new Item(ActionIds.BloodSpiller),
                    new Item(ActionIds.Quietus)
                })
            }); ;
            JobToGauges.Add(JobIds.SCH, new ActionGauge[] { });
            JobToGauges.Add(JobIds.WHM, new ActionGauge[] { });
            JobToGauges.Add(JobIds.BRD, new ActionGauge[] { });
            JobToGauges.Add(JobIds.DRG, new ActionGauge[] { });
            JobToGauges.Add(JobIds.SMN, new ActionGauge[] { });
            JobToGauges.Add(JobIds.SAM, new ActionGauge[] { });
            JobToGauges.Add(JobIds.BLM, new ActionGauge[] { });
            JobToGauges.Add(JobIds.RDM, new ActionGauge[] { });
            JobToGauges.Add(JobIds.MCH, new ActionGauge[] { });
            JobToGauges.Add(JobIds.DNC, new ActionGauge[] { });
            JobToGauges.Add(JobIds.NIN, new ActionGauge[] { });
            JobToGauges.Add(JobIds.MNK, new ActionGauge[] { });
            JobToGauges.Add(JobIds.BLU, new ActionGauge[] { });
        }

        public void SetJob(ClassJob job) {
            JobIds _job = job.Id < 19 ? JobIds.OTHER : (JobIds)job.Id;
            if(_job != CurrentJob) {
                CurrentJob = _job;
                // TODO: disable old
                // TODO: switch here
                PluginLog.Log($"SWITCHED JOB TO {CurrentJob}");
            }
        }
    }
}
