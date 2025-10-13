using System;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GlobalEnums;
using HarmonyLib;

namespace GamepadUISwitcher;

[BepInAutoPlugin(id: "capitalistspz.gamepaduiswitcher-silksong")]
public partial class GamepadUISwitcherPlugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;

    internal static readonly GamepadButtonSkinOpt[] GamepadSkinOptions =
        Enum.GetValues(typeof(GamepadButtonSkinOpt)).Cast<GamepadButtonSkinOpt>().ToArray();
    
    internal static ConfigEntry<GamepadButtonSkinOpt> gamepadSkinConfig;
    
    internal static GamepadType SelectedGamepadType => gamepadSkinConfig.Value switch
    {
        GamepadButtonSkinOpt.Xbox360 => GamepadType.XBOX_360,
        GamepadButtonSkinOpt.XboxOne => GamepadType.XBOX_ONE,
        GamepadButtonSkinOpt.XboxSeriesX => GamepadType.XBOX_SERIES_X,
        GamepadButtonSkinOpt.DualShock4 => GamepadType.PS4,
        GamepadButtonSkinOpt.DualSense => GamepadType.PS5,
        GamepadButtonSkinOpt.SwitchJoycons => GamepadType.SWITCH_JOYCON_DUAL,
        GamepadButtonSkinOpt.Switch2Joycons => GamepadType.SWITCH2_JOYCON_DUAL,
        GamepadButtonSkinOpt.SwitchPro => GamepadType.SWITCH_PRO_CONTROLLER,
        GamepadButtonSkinOpt.Switch2Pro => GamepadType.SWITCH2_PRO_CONTROLLER,
        GamepadButtonSkinOpt.SteamDeck => GamepadType.STEAM_DECK,
        _ => UIManager.instance.ih.activeGamepadType
    };
    internal static string SkinOptToString(GamepadButtonSkinOpt type) => type switch
    {
        GamepadButtonSkinOpt.Auto => "Auto",
        GamepadButtonSkinOpt.Xbox360 => "Xbox 360",
        GamepadButtonSkinOpt.XboxOne => "Xbox One",
        GamepadButtonSkinOpt.XboxSeriesX => "Xbox Series X",
        GamepadButtonSkinOpt.DualShock4 => "DualShock 4",
        GamepadButtonSkinOpt.DualSense => "DualSense",
        GamepadButtonSkinOpt.SwitchJoycons => "Switch Joycons",
        GamepadButtonSkinOpt.Switch2Joycons => "Switch 2 Joycons",
        GamepadButtonSkinOpt.SwitchPro => "Switch Pro",
        GamepadButtonSkinOpt.Switch2Pro => "Switch 2 Pro",
        GamepadButtonSkinOpt.SteamDeck => "Steam Deck",
        _ => "Unknown"
    };
    
    private void Awake()
    {
        Logger = base.Logger;
        
        gamepadSkinConfig = Config.Bind("UI", "Gamepad Skin", GamepadButtonSkinOpt.Auto);
        gamepadSkinConfig.SettingChanged += (_, _) =>
        {
            var temp = GamepadType.NONE;
            UIManager.instance.controllerDetect.ShowController(temp);
            UIManager.instance.uiButtonSkins.RefreshButtonMappings();
        };
        
        var harmony = Harmony.CreateAndPatchAll(typeof(UIManagerPatch));
        harmony.PatchAll(typeof(ButtonSkinsPatch));
        harmony.PatchAll(typeof(ControllerDetectPatch));
        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");
        
    }
    
    public static void SwapXY_AB()
    {
        string[] buttonsObjNames =
        [
            "Xbox360Buttons", "XboxOneButtons", "PS4Buttons", "SwitchJoyconButtons", "Switch2JoyconButtons",
            "SwitchProControllerButtons", "PS5Buttons", "XboxSeriesXButtons", "SteamDeckButtons"
        ];
        foreach (var name in buttonsObjNames)
        {
            var transform = UIManager.instance.gamepadMenuScreen.transform.Find($"Content/ControllerProfiles/{name}");
            var positions = transform.GetComponent<ControllerButtonPositions>();
            (positions.action1.controllerButton, positions.action2.controllerButton) = (positions.action2.controllerButton, positions.action1.controllerButton);
            (positions.action3.controllerButton, positions.action4.controllerButton) = (positions.action4.controllerButton, positions.action3.controllerButton);
        }

        var skins = UIManager.instance.uiButtonSkins;
        (skins.a, skins.b) = (skins.b, skins.a);
        (skins.x, skins.y) = (skins.y, skins.x);
        (skins.ps4x, skins.ps4circle) = (skins.ps4circle, skins.ps4x);
        (skins.ps4triangle, skins.ps4square) = (skins.ps4square, skins.ps4triangle);
        (skins.switchHidA, skins.switchHidB) = (skins.switchHidB, skins.switchHidA);
        (skins.switchHidX, skins.switchHidY) = (skins.switchHidY, skins.switchHidX);
        (skins.ps5.cross, skins.ps5.circle) = (skins.ps5.circle, skins.ps5.cross);
        (skins.ps5.triangle, skins.ps5.square) = (skins.ps5.square, skins.ps5.triangle);
        
        var temp = GamepadType.NONE;
        UIManager.instance.controllerDetect.ShowController(temp);
    }
}