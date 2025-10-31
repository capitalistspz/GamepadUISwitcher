using System;
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
            (GamepadButtonSkinOpt[])Enum.GetValues(typeof(GamepadButtonSkinOpt)),
            GamepadUISwitcherPlugin.gamepadSkinConfig);

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
            "GamepadUISwitcher/SwapOptions", "SWAP_OPTION.LABEL", "SWAP_OPTION.DESCRIPTION",
            (GamepadButtonSwapOption[])Enum.GetValues(typeof(GamepadButtonSwapOption)),
            GamepadUISwitcherPlugin.gamepadButtonSwapConfig);
        swapOptionObj.transform.SetParent(swapOptionSetting.transform, false);

        var swapButtonEntry = new MenuButtonList.Entry
        {
            selectable = swapOptionObj.GetComponent<MenuSelectable>(),
            alsoAffectParent = true,
            forceEnable = false,
        };
        UI.Utils.InsertAfter(ref menuButtonListComp.entries, skinOptionEntry, swapButtonEntry);
        GamepadUISwitcherPlugin.SetButtonSwapOptions();
    }
}