using GlobalEnums;
using HarmonyLib;

namespace GamepadUISwitcher;

public class ControllerDetectPatch
{
    [HarmonyPatch(typeof(ControllerDetect), nameof(ControllerDetect.ShowController))]
    [HarmonyPrefix]
    public static void ShowAltController(ref GamepadType gamepadType)
    {
        if (UIManager.instance.ih.activeGamepadType != GamepadType.NONE)
            gamepadType = GamepadUISwitcherPlugin.SelectedGamepadType;
    }
}