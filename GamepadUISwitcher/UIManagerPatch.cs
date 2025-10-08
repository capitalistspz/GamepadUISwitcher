using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GamepadUISwitcher;

// Here, 
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
        
        // "Content" object is unnecessarily large, shrink it to size of child
        var contentTransform = controllerMenuScreen.transform.Find("Content") as RectTransform;
        var profilesTransform = contentTransform.Find("ControllerProfiles") as RectTransform;
        contentTransform.sizeDelta = profilesTransform.sizeDelta;
        
        var controlsTransform = controllerMenuScreen.transform.Find("Controls");
        var rumbleSettingTransform = controlsTransform.Find("RumbleSetting");

        var uiSkinSetting = new GameObject("GamepadSkinSetting");
        uiSkinSetting.AddComponentIfNotPresent<RectTransform>();
        uiSkinSetting.transform.SetParent(controlsTransform, false);
        uiSkinSetting.transform.SetSiblingIndex(rumbleSettingTransform.GetSiblingIndex() + 1);

        var optionObj = UI.Objects.CreateBepinexConfigOption(
            GamepadUISwitcherPlugin.GamepadSkinOptions, GamepadUISwitcherPlugin.gamepadSkinConfig,
            "GamepadSkinOptionPopup", "Gamepad Skin", "Choose what the gamepad UI should look like", GamepadUISwitcherPlugin.SkinOptToString);
        optionObj.transform.SetParent(uiSkinSetting.transform, false);
        
        var newEntry = new MenuButtonList.Entry
        {
            selectable = optionObj.GetComponent<MenuSelectable>(),
            alsoAffectParent = true,
            forceEnable = false,
            condition = null
        };
        newEntry.selectable.navigation = new Navigation { mode = Navigation.Mode.Explicit };
        
        var menuButtonListComp = controllerMenuScreen.GetComponent<MenuButtonList>();
        var rumblePopupOpt = menuButtonListComp.entries.First(entry => entry.selectable.name == "RumblePopupOption");
        UI.Utils.InsertAfter(ref menuButtonListComp.entries, rumblePopupOpt, newEntry);
    }
}