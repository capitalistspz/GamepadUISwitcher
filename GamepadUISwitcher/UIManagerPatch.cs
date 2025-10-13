using HarmonyLib;

namespace GamepadUISwitcher;

// Here we add the objects into the existing UI
public class UIManagerPatch
{
    [HarmonyPatch(typeof(UIManager), nameof(UIManager.Awake))]
    [HarmonyPostfix]
    public static void OnAwake()
    {
        GamepadUISwitcherPlugin.OnUILoad();
    }
}