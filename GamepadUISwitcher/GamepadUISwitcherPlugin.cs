using System;
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
    internal static GamepadType SelectedGamepadType => gamepadSkinConfig.Value is Auto or GamepadType.UNKNOWN or GamepadType.PS_VITA or GamepadType.WII_U_GAMEPAD or GamepadType.WII_U_PRO_CONTROLLER
        ? UIManager.instance.ih.activeGamepadType : gamepadSkinConfig.Value;

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
        
        UIManager.instance.controllerDetect.ShowController(SelectedGamepadType);
    }
}