using System;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GlobalEnums;
using HarmonyLib;
using InControl;
using UnityEngine;
using UnityEngine.UI;

namespace GamepadUISwitcher;

[BepInAutoPlugin(id: "capitalistspz.gamepaduiswitcher-silksong")]
public partial class GamepadUISwitcherPlugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;
    
    internal static ConfigEntry<GamepadButtonSkinOpt> gamepadSkinConfig;
    internal static ConfigEntry<GamepadButtonSwapOption> gamepadButtonSwapConfig;
    
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
        gamepadButtonSwapConfig.SettingChanged += (_, _) =>
        {;
            SetButtonSwapOptions();
            UIManager.instance.controllerDetect.ShowController(GamepadType.NONE);
        };
        
        var harmony = Harmony.CreateAndPatchAll(typeof(UIManagerPatch));
        harmony.PatchAll(typeof(ButtonSkinsPatch));
        harmony.PatchAll(typeof(ControllerDetectPatch));
        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");
    }

    public static void OnUILoad()
    {
        InsertSettingsUI();
        SetButtonSwapOptions();
    }

    public static void SetButtonSwapOptions()
    {
        string[] buttonsObjNames =
        [
            "Xbox360Buttons", "XboxOneButtons", "PS4Buttons", "SwitchJoyconButtons", "Switch2JoyconButtons",
            "SwitchProControllerButtons", "PS5Buttons", "XboxSeriesXButtons", "SteamDeckButtons"
        ];
        
        var skins = UIManager.instance.uiButtonSkins;

        var positionsComponents = buttonsObjNames
            .Select(objName => UIManager.instance.gamepadMenuScreen.transform
                .Find($"Content/ControllerProfiles/{objName}").GetComponent<ControllerButtonPositions>());
       
        var opt = gamepadButtonSwapConfig.Value;
        switch (opt)
        {
            case GamepadButtonSwapOption.None:
            {
                skins.a = FaceButtonSprites.xbox.a;
                skins.b = FaceButtonSprites.xbox.b;
                skins.x = FaceButtonSprites.xbox.x;
                skins.y = FaceButtonSprites.xbox.y;

                skins.ps4x = FaceButtonSprites.playstation.cross;
                skins.ps4circle = FaceButtonSprites.playstation.circle;
                skins.ps4square = FaceButtonSprites.playstation.square;
                skins.ps4triangle = FaceButtonSprites.playstation.triangle;

                skins.switchHidA = FaceButtonSprites.nintendo.a;
                skins.switchHidB = FaceButtonSprites.nintendo.b;
                skins.switchHidX = FaceButtonSprites.nintendo.x;
                skins.switchHidY = FaceButtonSprites.nintendo.y;
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
                skins.a = FaceButtonSprites.xbox.b;
                skins.b = FaceButtonSprites.xbox.a;
                skins.x = FaceButtonSprites.xbox.y;
                skins.y = FaceButtonSprites.xbox.x;

                skins.ps4x = FaceButtonSprites.playstation.circle;
                skins.ps4circle = FaceButtonSprites.playstation.cross;
                skins.ps4square = FaceButtonSprites.playstation.triangle;
                skins.ps4triangle = FaceButtonSprites.playstation.square;
                
                skins.switchHidA = FaceButtonSprites.nintendo.b;
                skins.switchHidB = FaceButtonSprites.nintendo.a;
                skins.switchHidX = FaceButtonSprites.nintendo.y;
                skins.switchHidY = FaceButtonSprites.nintendo.x;

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
                skins.a = FaceButtonSprites.xbox.b;
                skins.b = FaceButtonSprites.xbox.a;
                skins.x = FaceButtonSprites.xbox.x;
                skins.y = FaceButtonSprites.xbox.y;

                skins.ps4x = FaceButtonSprites.playstation.circle;
                skins.ps4circle = FaceButtonSprites.playstation.cross;
                skins.ps4square = FaceButtonSprites.playstation.square;
                skins.ps4triangle = FaceButtonSprites.playstation.triangle;

                skins.switchHidA = FaceButtonSprites.nintendo.b;
                skins.switchHidB = FaceButtonSprites.nintendo.a;
                skins.switchHidX = FaceButtonSprites.nintendo.x;
                skins.switchHidY = FaceButtonSprites.nintendo.y;

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
                skins.a = FaceButtonSprites.xbox.a;
                skins.b = FaceButtonSprites.xbox.b;
                skins.x = FaceButtonSprites.xbox.y;
                skins.y = FaceButtonSprites.xbox.x;

                skins.ps4x = FaceButtonSprites.playstation.cross;
                skins.ps4circle = FaceButtonSprites.playstation.circle;
                skins.ps4square = FaceButtonSprites.playstation.triangle;
                skins.ps4triangle = FaceButtonSprites.playstation.square;

                skins.switchHidA = FaceButtonSprites.nintendo.a;
                skins.switchHidB = FaceButtonSprites.nintendo.b;
                skins.switchHidX = FaceButtonSprites.nintendo.y;
                skins.switchHidY = FaceButtonSprites.nintendo.x;

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
                skins.a = FaceButtonSprites.xbox.x;
                skins.b = FaceButtonSprites.xbox.y;
                skins.x = FaceButtonSprites.xbox.a;
                skins.y = FaceButtonSprites.xbox.b;

                skins.ps4x = FaceButtonSprites.playstation.square;
                skins.ps4circle = FaceButtonSprites.playstation.triangle;
                skins.ps4square = FaceButtonSprites.playstation.cross;
                skins.ps4triangle = FaceButtonSprites.playstation.circle;

                skins.switchHidA = FaceButtonSprites.nintendo.x;
                skins.switchHidB = FaceButtonSprites.nintendo.y;
                skins.switchHidX = FaceButtonSprites.nintendo.a;
                skins.switchHidY = FaceButtonSprites.nintendo.b;

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
                skins.a = FaceButtonSprites.xbox.x;
                skins.b = FaceButtonSprites.xbox.b;
                skins.x = FaceButtonSprites.xbox.x;
                skins.y = FaceButtonSprites.xbox.y;

                skins.ps4x = FaceButtonSprites.playstation.square;
                skins.ps4circle = FaceButtonSprites.playstation.circle;
                skins.ps4square = FaceButtonSprites.playstation.cross;
                skins.ps4triangle = FaceButtonSprites.playstation.triangle;

                skins.switchHidA = FaceButtonSprites.nintendo.x;
                skins.switchHidB = FaceButtonSprites.nintendo.b;
                skins.switchHidX = FaceButtonSprites.nintendo.a;
                skins.switchHidY = FaceButtonSprites.nintendo.y;

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
                skins.a = FaceButtonSprites.xbox.a;
                skins.b = FaceButtonSprites.xbox.y;
                skins.x = FaceButtonSprites.xbox.x;
                skins.y = FaceButtonSprites.xbox.b;

                skins.ps4x = FaceButtonSprites.playstation.cross;
                skins.ps4circle = FaceButtonSprites.playstation.triangle;
                skins.ps4square = FaceButtonSprites.playstation.square;
                skins.ps4triangle = FaceButtonSprites.playstation.circle;

                skins.switchHidA = FaceButtonSprites.nintendo.a;
                skins.switchHidB = FaceButtonSprites.nintendo.y;
                skins.switchHidX = FaceButtonSprites.nintendo.x;
                skins.switchHidY = FaceButtonSprites.nintendo.b;

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
                skins.a = FaceButtonSprites.xbox.y;
                skins.b = FaceButtonSprites.xbox.x;
                skins.x = FaceButtonSprites.xbox.b;
                skins.y = FaceButtonSprites.xbox.a;

                skins.ps4x = FaceButtonSprites.playstation.triangle;
                skins.ps4circle = FaceButtonSprites.playstation.square;
                skins.ps4square = FaceButtonSprites.playstation.circle;
                skins.ps4triangle = FaceButtonSprites.playstation.cross;

                skins.switchHidA = FaceButtonSprites.nintendo.y;
                skins.switchHidB = FaceButtonSprites.nintendo.x;
                skins.switchHidX = FaceButtonSprites.nintendo.b;
                skins.switchHidY = FaceButtonSprites.nintendo.a;

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
                skins.a = FaceButtonSprites.xbox.a;
                skins.b = FaceButtonSprites.xbox.x;
                skins.x = FaceButtonSprites.xbox.b;
                skins.y = FaceButtonSprites.xbox.y;

                skins.ps4x = FaceButtonSprites.playstation.cross;
                skins.ps4circle = FaceButtonSprites.playstation.square;
                skins.ps4square = FaceButtonSprites.playstation.circle;
                skins.ps4triangle = FaceButtonSprites.playstation.triangle;

                skins.switchHidA = FaceButtonSprites.nintendo.a;
                skins.switchHidB = FaceButtonSprites.nintendo.x;
                skins.switchHidX = FaceButtonSprites.nintendo.b;
                skins.switchHidY = FaceButtonSprites.nintendo.y;
                
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
                skins.a = FaceButtonSprites.xbox.y;
                skins.b = FaceButtonSprites.xbox.b;
                skins.x = FaceButtonSprites.xbox.x;
                skins.y = FaceButtonSprites.xbox.a;

                skins.ps4x = FaceButtonSprites.playstation.triangle;
                skins.ps4circle = FaceButtonSprites.playstation.circle;
                skins.ps4square = FaceButtonSprites.playstation.square;
                skins.ps4triangle = FaceButtonSprites.playstation.cross;

                skins.switchHidA = FaceButtonSprites.nintendo.y;
                skins.switchHidB = FaceButtonSprites.nintendo.b;
                skins.switchHidX = FaceButtonSprites.nintendo.x;
                skins.switchHidY = FaceButtonSprites.nintendo.a;
                
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
    }

    public static void InsertSettingsUI()
    {
        var controllerMenuScreen = UIManager.instance.gamepadMenuScreen;
        
        // So that items don't overlap
        var layoutGroupComp = controllerMenuScreen.gameObject.AddComponentIfNotPresent<VerticalLayoutGroup>();
        layoutGroupComp.childAlignment = TextAnchor.MiddleCenter;
        layoutGroupComp.childForceExpandHeight = false;
        layoutGroupComp.childForceExpandWidth = false;
        layoutGroupComp.childScaleHeight = true;
        layoutGroupComp.childControlHeight = false;
        
        // "Content" object is unnecessarily large, shrink it to size of its child
        var contentTransform = controllerMenuScreen.transform.Find("Content") as RectTransform;
        var profilesTransform = contentTransform!.Find("ControllerProfiles") as RectTransform;
        contentTransform.sizeDelta = profilesTransform!.sizeDelta;
        
        // Shrink spacing so that more elements can be added
        var controlsTransform = controllerMenuScreen.transform.Find("Controls");
        var controlsLayoutGroupComp = controlsTransform.GetComponent<VerticalLayoutGroup>();
        controlsLayoutGroupComp.spacing *= 0.75f;
        controlsLayoutGroupComp.childAlignment = TextAnchor.MiddleCenter;
        
        var rumbleSettingTransform = controlsTransform.Find("RumbleSetting");

        var uiSkinSettingObj = new GameObject("GamepadSkinSetting");
        uiSkinSettingObj.AddComponentIfNotPresent<RectTransform>();
        uiSkinSettingObj.transform.SetParent(controlsTransform, false);
        uiSkinSettingObj.transform.SetSiblingIndex(rumbleSettingTransform.GetSiblingIndex() + 1);
        
        var skinOptionObj = UI.Objects.CreateBepinexConfigOptionTranslated("GamepadSkinOption",
            "GamepadUISwitcher/SkinOptions", "SKIN_OPT_LABEL", "SKIN_OPT_DESCRIPTION",
            (GamepadButtonSkinOpt[])Enum.GetValues(typeof(GamepadButtonSkinOpt)), GamepadUISwitcherPlugin.gamepadSkinConfig);
        
        skinOptionObj.transform.SetParent(uiSkinSettingObj.transform, false);
        
        var skinOptionEntry = new MenuButtonList.Entry
        {
            selectable = skinOptionObj.GetComponent<MenuSelectable>(),
            alsoAffectParent = true,
            forceEnable = false,
            condition = null
        };
        
        var menuButtonListComp = controllerMenuScreen.GetComponent<MenuButtonList>();
        var rumblePopupOpt = menuButtonListComp.entries.First(entry => entry.selectable.name == "RumblePopupOption");
        UI.Utils.InsertAfter(ref menuButtonListComp.entries, rumblePopupOpt, skinOptionEntry);

        var swapOptionSetting = new GameObject("GamepadButtonSwapOption");
        swapOptionSetting.AddComponentIfNotPresent<RectTransform>();
        swapOptionSetting.transform.SetParent(controlsTransform, false);
        swapOptionSetting.transform.SetSiblingIndex(uiSkinSettingObj.transform.GetSiblingIndex() + 1);
        
        var swapOptionObj = UI.Objects.CreateBepinexConfigOptionTranslated("GamepadSwapOption", 
            "GamepadUISwitcher/SwapOptions", "SWAP_OPT_LABEL", "SWAP_OPT_DESCRIPTION",
            (GamepadButtonSwapOption[])Enum.GetValues(typeof(GamepadButtonSwapOption)), GamepadUISwitcherPlugin.gamepadButtonSwapConfig);
        swapOptionObj.transform.SetParent(swapOptionSetting.transform, false);
        
        var swapButtonEntry = new MenuButtonList.Entry
        {
            selectable = swapOptionObj.GetComponent<MenuSelectable>(),
            alsoAffectParent = true,
            forceEnable = false,
        };
        UI.Utils.InsertAfter(ref menuButtonListComp.entries, skinOptionEntry, swapButtonEntry);
    }
}