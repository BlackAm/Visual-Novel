using UnityEngine;

namespace BlackAm
{
    public class BossZoneSceneEnvironment : LamierePlayerDeploySceneEnvironment
    {
        public override void OnSceneStarted()
        {
            base.OnSceneStarted();
#if !SERVER_DRIVE
/*
            var spawnEventHandler = TerminateEventReceiverSpawnManager.GetInstanceUnSafe.SpawnEmptyTimer(false);
            spawnEventHandler.AddEvent(
                0, 1000,
                handler =>
                {
                    var botSpawnRandSeed = Random.Range(1, 13);
                    for (int i = 0; i < botSpawnRandSeed; i++)
                    {
                        TestSpawnManager.GetInstanceUnSafe.SpawnUnitManual(Unit.UnitAllyFlagType.Bot, Unit.UnitAllyFlagType.Bot2 | Unit.UnitAllyFlagType.Monster,
                            Random.Range(11, 14));
                    }

                    return true;
                },
                handler => false);

            spawnEventHandler.StartEvent();*/
#endif
        }
    }
}