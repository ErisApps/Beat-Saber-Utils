using IPA;
using System;
using Harmony;
using BS_Utils.Gameplay;
using BS_Utils.Utilities;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;
using LogLevel = IPA.Logging.Logger.Level;

namespace BS_Utils
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static bool patched = false;
        internal static HarmonyInstance harmony;
        public static LevelData LevelData = new LevelData();
        public delegate void LevelDidFinish(StandardLevelScenesTransitionSetupDataSO levelScenesTransitionSetupDataSO, LevelCompletionResults levelCompletionResults);
        public static event LevelDidFinish LevelDidFinishEvent;
        
        public delegate void MissionDidFinish(MissionLevelScenesTransitionSetupDataSO missionLevelScenesTransitionSetupDataSO, MissionCompletionResults missionCompletionResults);
        public static event MissionDidFinish MissionDidFinishEvent;

        [OnStart]
        public void OnApplicationStart()
        {
            //Create Harmony Instance
            harmony = HarmonyInstance.Create("com.kyle1413.BeatSaber.BS-Utils");
            BSEvents.OnLoad();
        }

        [Init]
        public void Init(IPALogger logger)
        {
            Logger.log = logger;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        [OnExit]
        public void Exit() => SceneManager.activeSceneChanged -= OnActiveSceneChanged;

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (nextScene.name == "MenuCore")
            {
                if (Gamemode.IsIsolatedLevel) // Only remove is necessary.
                    Logger.Log("Removing Isolated Level");
                Gamemode.IsIsolatedLevel = false;
                Gamemode.IsolatingMod = "";
                LevelData.Clear();
            }
        }

        internal static void TriggerLevelFinishEvent(StandardLevelScenesTransitionSetupDataSO levelScenesTransitionSetupDataSO, LevelCompletionResults levelCompletionResults)
        {
            LevelDidFinishEvent?.Invoke(levelScenesTransitionSetupDataSO, levelCompletionResults);
        }
        internal static void TriggerMissionFinishEvent(MissionLevelScenesTransitionSetupDataSO missionLevelScenesTransitionSetupDataSO, MissionCompletionResults missionCompletionResults)
        {
            MissionDidFinishEvent?.Invoke(missionLevelScenesTransitionSetupDataSO, missionCompletionResults);
        }

        internal static void ApplyHarmonyPatches()
        {
            if (patched) return;
            try
            {
                Logger.Log("Applying Harmony Patches", LogLevel.Debug);
                harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
                patched = true;
            }
            catch (Exception ex)
            {
                Logger.Log("Exception Trying to Apply Harmony Patches", LogLevel.Error);
                Logger.Log(ex.ToString(), LogLevel.Error);
            }
        }

    }
}
