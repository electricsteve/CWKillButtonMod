using UnityEngine;
using Zorro.Settings;

namespace CW_Test_Mod.Keybinds;

public class KillKeybindSetting : KeyCodeSetting, IExposedSetting
{
    protected override KeyCode GetDefaultKey() => KeyCode.K;

    public SettingCategory GetSettingCategory() => SettingCategory.Controls;

    // public string GetDisplayName() => "Kill";
    public string GetDisplayName() => "Kill Key";
}