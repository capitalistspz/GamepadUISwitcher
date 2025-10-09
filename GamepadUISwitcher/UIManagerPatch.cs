using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GamepadUISwitcher;

// Here we add the objects into the existing UI
public class UIManagerPatch
{
    [HarmonyPatch(typeof(UIManager), nameof(UIManager.Awake))]
    [HarmonyPostfix]
    public static void OnAwake()
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
        var profilesTransform = contentTransform.Find("ControllerProfiles") as RectTransform;
        contentTransform.sizeDelta = profilesTransform.sizeDelta;
        
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
        
        var skinOptionObj = UI.Objects.CreateBepinexConfigOption(
            GamepadUISwitcherPlugin.GamepadSkinOptions, GamepadUISwitcherPlugin.gamepadSkinConfig,
            "GamepadSkinOptionPopup", "Gamepad Skin", "Choose what the gamepad UI should look like", GamepadUISwitcherPlugin.SkinOptToString);
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

        var swapButtonOptionObj = new GameObject("GamepadButtonSwapOption");
        swapButtonOptionObj.AddComponentIfNotPresent<RectTransform>();
        swapButtonOptionObj.transform.SetParent(controlsTransform, false);
        swapButtonOptionObj.transform.SetSiblingIndex(uiSkinSettingObj.transform.GetSiblingIndex() + 1);
        
        var swapButtonObj = UI.Objects.CreateMenuButton("GamepadButtonSwapButton", "Swap Face Button Icons", MenuButton.MenuButtonType.Activate, _ => 
            GamepadUISwitcherPlugin.SwapXY_AB());
        swapButtonObj.transform.SetParent(swapButtonOptionObj.transform, false);
        
        var swapButtonEntry = new MenuButtonList.Entry
        {
            selectable = swapButtonObj.GetComponent<MenuSelectable>(),
            alsoAffectParent = true,
            forceEnable = false,
        };
        UI.Utils.InsertAfter(ref menuButtonListComp.entries, skinOptionEntry, swapButtonEntry);
    }
}