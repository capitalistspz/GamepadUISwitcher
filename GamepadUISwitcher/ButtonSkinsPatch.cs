using GlobalEnums;
using HarmonyLib;

namespace GamepadUISwitcher;


[HarmonyPatch(typeof(UIButtonSkins), nameof(UIButtonSkins.GetButtonSkinFor), typeof(InControl.InputControlType))]
public class ButtonSkinsPatch
{
    [HarmonyPrefix]
    public static void Prefix(ref GamepadType __state, ref InputHandler ___ih)
    {
        __state = ___ih.activeGamepadType;
        if (___ih.activeGamepadType == GamepadType.NONE)
            return;
        ___ih.activeGamepadType = GamepadUISwitcherPlugin.SelectedGamepadType;
    }

    [HarmonyPostfix]
    public static void Postfix(ref GamepadType __state, ref InputHandler ___ih)
    {
        ___ih.activeGamepadType = __state;
    }
    
}