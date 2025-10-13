using System;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GlobalEnums;
using HarmonyLib;
using InControl;
using UnityEngine;

namespace GamepadUISwitcher;

[BepInAutoPlugin(id: "capitalistspz.gamepaduiswitcher-silksong")]
public partial class GamepadUISwitcherPlugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;
    
    internal static ConfigEntry<GamepadButtonSkinOpt> gamepadSkinConfig;
    internal static ConfigEntry<GamepadButtonSwapOption> gamepadButtonSwapConfig;
    private static FaceButtonSprites sprites;
    
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

        gamepadButtonSwapConfig = Config.Bind("UI", "Button Swap", GamepadButtonSwapOption.None);
        gamepadButtonSwapConfig.SettingChanged += (_, _) => SwapButtons();
        
        var harmony = Harmony.CreateAndPatchAll(typeof(UIManagerPatch));
        harmony.PatchAll(typeof(ButtonSkinsPatch));
        harmony.PatchAll(typeof(ControllerDetectPatch));
        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");
        sprites = new FaceButtonSprites();
    }
    
    public static void SwapButtons()
    {
        string[] buttonsObjNames =
        [
            "Xbox360Buttons", "XboxOneButtons", "PS4Buttons", "SwitchJoyconButtons", "Switch2JoyconButtons",
            "SwitchProControllerButtons", "PS5Buttons", "XboxSeriesXButtons", "SteamDeckButtons"
        ];
        
        var skins = UIManager.instance.uiButtonSkins;

        var positionsComponents = buttonsObjNames
            .Select(objName => UIManager.instance.gamepadMenuScreen.transform
                .Find($"Content/ControllerProfiles/{objName}").GetComponent<ControllerButtonPositions>()).ToArray();
       
        var opt = gamepadButtonSwapConfig.Value;
        switch (opt)
        {
            case GamepadButtonSwapOption.None:
            {
                skins.a = sprites.xbox.a;
                skins.b = sprites.xbox.b;
                skins.x = sprites.xbox.x;
                skins.y = sprites.xbox.y;

                skins.ps4x = sprites.playstation.cross;
                skins.ps4circle = sprites.playstation.circle;
                skins.ps4square = sprites.playstation.square;
                skins.ps4triangle = sprites.playstation.triangle;

                skins.switchHidA = sprites.nintendo.a;
                skins.switchHidB = sprites.nintendo.b;
                skins.switchHidX = sprites.nintendo.x;
                skins.switchHidY = sprites.nintendo.y;
                foreach (var positions in positionsComponents)
                {
                    positions.action1.controllerButton = InputControlType.Action1;
                    positions.action2.controllerButton = InputControlType.Action2;
                    positions.action3.controllerButton = InputControlType.Action3;
                    positions.action4.controllerButton = InputControlType.Action4;
                }
                
                break;
            }
            case GamepadButtonSwapOption.AB_XY:
            {
                skins.a = sprites.xbox.b;
                skins.b = sprites.xbox.a;
                skins.x = sprites.xbox.y;
                skins.y = sprites.xbox.x;

                skins.ps4x = sprites.playstation.circle;
                skins.ps4circle = sprites.playstation.cross;
                skins.ps4square = sprites.playstation.triangle;
                skins.ps4triangle = sprites.playstation.square;
                
                skins.switchHidA = sprites.nintendo.b;
                skins.switchHidB = sprites.nintendo.a;
                skins.switchHidX = sprites.nintendo.y;
                skins.switchHidY = sprites.nintendo.x;

                foreach (var positions in positionsComponents)
                {
                    positions.action1.controllerButton = InputControlType.Action2;
                    positions.action2.controllerButton = InputControlType.Action1;
                    positions.action3.controllerButton = InputControlType.Action4;
                    positions.action4.controllerButton = InputControlType.Action3;
                }

                break;
            }
            case GamepadButtonSwapOption.AB:
            {
                skins.a = sprites.xbox.b;
                skins.b = sprites.xbox.a;
                skins.x = sprites.xbox.x;
                skins.y = sprites.xbox.y;

                skins.ps4x = sprites.playstation.circle;
                skins.ps4circle = sprites.playstation.cross;
                skins.ps4square = sprites.playstation.square;
                skins.ps4triangle = sprites.playstation.triangle;

                skins.switchHidA = sprites.nintendo.b;
                skins.switchHidB = sprites.nintendo.a;
                skins.switchHidX = sprites.nintendo.x;
                skins.switchHidY = sprites.nintendo.y;

                foreach (var positions in positionsComponents)
                {
                    positions.action1.controllerButton = InputControlType.Action2;
                    positions.action2.controllerButton = InputControlType.Action1;
                    positions.action3.controllerButton = InputControlType.Action3;
                    positions.action4.controllerButton = InputControlType.Action4;
                }

                break;
            }
            case GamepadButtonSwapOption.XY:
            {
                skins.a = sprites.xbox.a;
                skins.b = sprites.xbox.b;
                skins.x = sprites.xbox.y;
                skins.y = sprites.xbox.x;

                skins.ps4x = sprites.playstation.cross;
                skins.ps4circle = sprites.playstation.circle;
                skins.ps4square = sprites.playstation.triangle;
                skins.ps4triangle = sprites.playstation.square;

                skins.switchHidA = sprites.nintendo.a;
                skins.switchHidB = sprites.nintendo.b;
                skins.switchHidX = sprites.nintendo.y;
                skins.switchHidY = sprites.nintendo.x;

                foreach (var positions in positionsComponents)
                {
                    positions.action1.controllerButton = InputControlType.Action1;
                    positions.action2.controllerButton = InputControlType.Action2;
                    positions.action3.controllerButton = InputControlType.Action4;
                    positions.action4.controllerButton = InputControlType.Action3;
                }

                break;
            }
            case GamepadButtonSwapOption.AX_BY:
            {
                skins.a = sprites.xbox.x;
                skins.b = sprites.xbox.y;
                skins.x = sprites.xbox.a;
                skins.y = sprites.xbox.b;

                skins.ps4x = sprites.playstation.square;
                skins.ps4circle = sprites.playstation.triangle;
                skins.ps4square = sprites.playstation.cross;
                skins.ps4triangle = sprites.playstation.circle;

                skins.switchHidA = sprites.nintendo.x;
                skins.switchHidB = sprites.nintendo.y;
                skins.switchHidX = sprites.nintendo.a;
                skins.switchHidY = sprites.nintendo.b;

                foreach (var positions in positionsComponents)
                {
                    positions.action1.controllerButton = InputControlType.Action3;
                    positions.action2.controllerButton = InputControlType.Action4;
                    positions.action3.controllerButton = InputControlType.Action1;
                    positions.action4.controllerButton = InputControlType.Action2;
                }

                break;
            }
            case GamepadButtonSwapOption.AX:
            {
                skins.a = sprites.xbox.x;
                skins.b = sprites.xbox.b;
                skins.x = sprites.xbox.x;
                skins.y = sprites.xbox.y;

                skins.ps4x = sprites.playstation.square;
                skins.ps4circle = sprites.playstation.circle;
                skins.ps4square = sprites.playstation.cross;
                skins.ps4triangle = sprites.playstation.triangle;

                skins.switchHidA = sprites.nintendo.x;
                skins.switchHidB = sprites.nintendo.b;
                skins.switchHidX = sprites.nintendo.a;
                skins.switchHidY = sprites.nintendo.y;

                foreach (var positions in positionsComponents)
                {
                    positions.action1.controllerButton = InputControlType.Action3;
                    positions.action2.controllerButton = InputControlType.Action2;
                    positions.action3.controllerButton = InputControlType.Action1;
                    positions.action4.controllerButton = InputControlType.Action4;
                }
                break;
            }
            case GamepadButtonSwapOption.BY:
            {
                skins.a = sprites.xbox.a;
                skins.b = sprites.xbox.y;
                skins.x = sprites.xbox.x;
                skins.y = sprites.xbox.b;

                skins.ps4x = sprites.playstation.cross;
                skins.ps4circle = sprites.playstation.triangle;
                skins.ps4square = sprites.playstation.square;
                skins.ps4triangle = sprites.playstation.circle;

                skins.switchHidA = sprites.nintendo.a;
                skins.switchHidB = sprites.nintendo.y;
                skins.switchHidX = sprites.nintendo.x;
                skins.switchHidY = sprites.nintendo.b;

                foreach (var positions in positionsComponents)
                {
                    positions.action1.controllerButton = InputControlType.Action1;
                    positions.action2.controllerButton = InputControlType.Action4;
                    positions.action3.controllerButton = InputControlType.Action3;
                    positions.action4.controllerButton = InputControlType.Action2;
                }
                break;
            }
            case GamepadButtonSwapOption.BX_AY:
            {
                skins.a = sprites.xbox.y;
                skins.b = sprites.xbox.x;
                skins.x = sprites.xbox.b;
                skins.y = sprites.xbox.a;

                skins.ps4x = sprites.playstation.triangle;
                skins.ps4circle = sprites.playstation.square;
                skins.ps4square = sprites.playstation.circle;
                skins.ps4triangle = sprites.playstation.cross;

                skins.switchHidA = sprites.nintendo.y;
                skins.switchHidB = sprites.nintendo.x;
                skins.switchHidX = sprites.nintendo.b;
                skins.switchHidY = sprites.nintendo.a;

                foreach (var positions in positionsComponents)
                {
                    positions.action1.controllerButton = InputControlType.Action4;
                    positions.action2.controllerButton = InputControlType.Action3;
                    positions.action3.controllerButton = InputControlType.Action2;
                    positions.action4.controllerButton = InputControlType.Action1;
                }

                break;
            }
            case GamepadButtonSwapOption.BX:
            {
                skins.a = sprites.xbox.a;
                skins.b = sprites.xbox.x;
                skins.x = sprites.xbox.b;
                skins.y = sprites.xbox.y;

                skins.ps4x = sprites.playstation.cross;
                skins.ps4circle = sprites.playstation.square;
                skins.ps4square = sprites.playstation.circle;
                skins.ps4triangle = sprites.playstation.triangle;

                skins.switchHidA = sprites.nintendo.a;
                skins.switchHidB = sprites.nintendo.x;
                skins.switchHidX = sprites.nintendo.b;
                skins.switchHidY = sprites.nintendo.y;
                
                foreach (var positions in positionsComponents)
                {
                    positions.action1.controllerButton = InputControlType.Action1;
                    positions.action2.controllerButton = InputControlType.Action3;
                    positions.action3.controllerButton = InputControlType.Action2;
                    positions.action4.controllerButton = InputControlType.Action4;
                }

                break;
            }
            case GamepadButtonSwapOption.AY:
            {
                skins.a = sprites.xbox.y;
                skins.b = sprites.xbox.b;
                skins.x = sprites.xbox.x;
                skins.y = sprites.xbox.a;

                skins.ps4x = sprites.playstation.triangle;
                skins.ps4circle = sprites.playstation.circle;
                skins.ps4square = sprites.playstation.square;
                skins.ps4triangle = sprites.playstation.cross;

                skins.switchHidA = sprites.nintendo.y;
                skins.switchHidB = sprites.nintendo.b;
                skins.switchHidX = sprites.nintendo.x;
                skins.switchHidY = sprites.nintendo.a;
                
                foreach (var positions in positionsComponents)
                {
                    positions.action1.controllerButton = InputControlType.Action4;
                    positions.action2.controllerButton = InputControlType.Action2;
                    positions.action3.controllerButton = InputControlType.Action3;
                    positions.action4.controllerButton = InputControlType.Action1;
                }
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
        Logger.LogInfo("Swapped buttons");
        var temp = GamepadType.NONE;
        UIManager.instance.controllerDetect.ShowController(temp);
    }
}