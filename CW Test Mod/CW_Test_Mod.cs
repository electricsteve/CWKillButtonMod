using System;
using System.Collections;
using BepInEx;
using BepInEx.Logging;
using System.Reflection;
using MonoMod.RuntimeDetour.HookGen;
using CW_Test_Mod.Patches;
using UnityEngine;
using Zorro.Core;

namespace CW_Test_Mod;

[ContentWarningPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION, false)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class CW_Test_Mod : BaseUnityPlugin
{
    public static CW_Test_Mod Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    public static bool JustKilledPlayer = false;

    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        HookAll();

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    internal static void HookAll()
    {
        Logger.LogDebug("Hooking...");

        ExampleShoppingCartPatch.Init();
        GlobalInputHandlerPatch.init();

        Logger.LogDebug("Finished Hooking!");
    }

    internal static void UnhookAll()
    {
        Logger.LogDebug("Unhooking...");

        /*
         *  HookEndpointManager is from MonoMod.RuntimeDetour.HookGen, and is used by the MMHOOK assemblies.
         *  We can unhook all methods hooked with HookGen using this.
         *  Or we can unsubscribe specific patch methods with 'On.Namespace.Type.Method -= CustomMethod;'
         */
        HookEndpointManager.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());

        Logger.LogDebug("Finished Unhooking!");
    }

    private void Update()
    {
        if (GlobalInputHandlerPatch.KillKey.GetKeyDown())
        {
            if (SurfaceNetworkHandler.HasStarted)
            {
                JustKilledPlayer = true;
                Logger.LogDebug("Killing Player");
                Player.localPlayer.CallDie();
                if (PhotonGameLobbyHandler.IsSurface) StartCoroutine(WaitAndRevive(2.0f));
            }
        }
    }

    private IEnumerator WaitAndRevive(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        RetrievableResourceSingleton<TransitionHandler>.Instance.TransitionToBlack(0.0f, (Action) (() =>
        {
            // Player.localPlayer.MoveAllRigsInDirection(Hospital.instance.transform.position + Vector3.up - Player.localPlayer.Center());
            Player.localPlayer.Teleport(Hospital.instance.transform.position, Vector3.back);
            Player.localPlayer.data.fallTime = 0f;
            RetrievableResourceSingleton<TransitionHandler>.Instance.FadeOut((Action) (() => {}), 1f);
        }), 0f);
        yield return new WaitForSeconds(1f);
        Logger.LogDebug("Reviving player...");
        Player.localPlayer.CallRevive();
        Logger.LogDebug("Healing player...");
        float healAmount = Player.PlayerData.maxHealth - Player.localPlayer.data.health;
        Player.localPlayer.CallHeal(healAmount);
    }
}