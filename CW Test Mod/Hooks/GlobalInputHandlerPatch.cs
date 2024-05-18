using CW_Test_Mod.Keybinds;
using UnityEngine;
using Zorro.Settings;
using Logger = BepInEx.Logging.Logger;

namespace CW_Test_Mod.Patches;

public class GlobalInputHandlerPatch
{
    public static GlobalInputHandler.InputKey KillKey = new GlobalInputHandler.InputKey();

    internal static void init()
    {
        On.GlobalInputHandler.OnCreated += GlobalInputHandlerOnOnCreated;
        On.SettingsHandler.ctor += SettingsHandlerInit;
        On.PhotonGameLobbyHandler.CheckForAllDead += PhotonGameLobbyHandlerOnCheckForAllDead;
    }

    private static void PhotonGameLobbyHandlerOnCheckForAllDead(On.PhotonGameLobbyHandler.orig_CheckForAllDead orig, PhotonGameLobbyHandler self)
    {
        if (CW_Test_Mod.JustKilledPlayer & PhotonGameLobbyHandler.IsSurface)
        {
            CW_Test_Mod.Logger.LogDebug("Player just killed, returning");
            CW_Test_Mod.JustKilledPlayer = false;
            return;
        }
        CW_Test_Mod.Logger.LogDebug("Check for all dead");
        orig(self);
    }

    private static void GlobalInputHandlerOnOnCreated(On.GlobalInputHandler.orig_OnCreated orig, GlobalInputHandler self)
    {
        orig(self);
        KillKey.SetKeybind((KeyCodeSetting) GameHandler.Instance.SettingsHandler.GetSetting<KillKeybindSetting>());
    }

    private static void SettingsHandlerInit(On.SettingsHandler.orig_ctor orig, SettingsHandler self)
    {
        orig(self);
        KillKeybindSetting killKeybindSetting = new KillKeybindSetting();
        killKeybindSetting.Load(self._settingsSaveLoad);
        killKeybindSetting.ApplyValue();
        self.settings.Add(killKeybindSetting);
    }
}