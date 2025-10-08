using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GlobalEnums;
using HarmonyLib;

namespace GamepadUISwitcher;

[BepInAutoPlugin(id: "io.github.capitalistspz.gamepaduiswitcher")]
public partial class GamepadUISwitcherPlugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;
    internal static Harmony harmony;
    
    private const GamepadType Auto = (GamepadType)(-1);

    internal static readonly GamepadType[] GamepadSkinOptions =
    [
        Auto, 
        GamepadType.XBOX_360, GamepadType.XBOX_ONE, GamepadType.XBOX_SERIES_X,
        GamepadType.PS4, GamepadType.PS5,
        GamepadType.SWITCH_JOYCON_DUAL, GamepadType.SWITCH2_JOYCON_DUAL, 
        GamepadType.SWITCH_PRO_CONTROLLER, GamepadType.SWITCH2_PRO_CONTROLLER, 
        GamepadType.STEAM_DECK
    ];
    
    internal static ConfigEntry<GamepadType> gamepadSkinConfig;
    internal static GamepadType SelectedGamepadType => (gamepadSkinConfig.Value == Auto || gamepadSkinConfig.Value == GamepadType.UNKNOWN) ? UIManager.instance.ih.activeGamepadType : gamepadSkinConfig.Value;

    internal static string SkinOptToString(GamepadType type) => type switch
    {
        Auto => "Auto",
        GamepadType.XBOX_360 => "Xbox 360",
        GamepadType.XBOX_ONE => "Xbox One",
        GamepadType.XBOX_SERIES_X => "Xbox Series X",
        GamepadType.PS4 => "DualShock 4",
        GamepadType.PS5 => "DualSense",
        GamepadType.SWITCH_JOYCON_DUAL => "Switch Joycons",
        GamepadType.SWITCH2_JOYCON_DUAL => "Switch 2 Joycons",
        GamepadType.SWITCH_PRO_CONTROLLER => "Switch Pro",
        GamepadType.SWITCH2_PRO_CONTROLLER => "Switch 2 Pro",
        GamepadType.STEAM_DECK => "Steam Deck",
        _ => "Unknown"
    };
    
    private void Awake()
    {
        Logger = base.Logger;
        
        gamepadSkinConfig = Config.Bind("UI", "Gamepad Skin", Auto);
        gamepadSkinConfig.SettingChanged += (_, _) =>
        {
            UIManager.instance.controllerDetect.ShowController(SelectedGamepadType);
            UIManager.instance.uiButtonSkins.RefreshButtonMappings();
        };
        
        harmony = Harmony.CreateAndPatchAll(typeof(UIManagerPatch));
        harmony.PatchAll(typeof(ButtonSkinsPatch));
        harmony.PatchAll(typeof(ControllerDetectPatch));
        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");
    }
    
}