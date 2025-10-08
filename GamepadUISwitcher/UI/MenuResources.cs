// Ripped straight from https://github.com/hk-modding/api/blob/master/Assembly-CSharp/Menu/MenuResources.cs
using UnityEngine;

/// <summary>
    /// Cached resources for the menu api to use
    /// </summary>
    public static class MenuResources
    {
        // ReSharper disable CS1591
#pragma warning disable 1591

        public static Font TrajanProRegular  { get; private set; }
        public static Font TrajanProBold { get; private set; }
        public static Font Perpetua { get; private set; }
        public static Font NotoSerifCJKSCRegular { get; private set; }

        public static RuntimeAnimatorController MenuTopFleurAnimator { get; private set; }
        public static RuntimeAnimatorController MenuCursorAnimator { get; private set; }
        public static RuntimeAnimatorController MenuButtonFlashAnimator { get; private set; }
        public static AnimatorOverrideController TextHideShowAnimator { get; private set; }

        public static Sprite ScrollbarHandleSprite { get; private set; }
        public static Sprite ScrollbarBackgroundSprite { get; private set; }

        // ReSharper restore CS1591
#pragma warning restore 1591

        static MenuResources()
        {
            ReloadResources();
        }

        /// <summary>
        /// Reloads all resources, searching to find each one again.
        /// </summary>
        public static void ReloadResources()
        {
            foreach (var animator in Resources.FindObjectsOfTypeAll<RuntimeAnimatorController>())
            {
                if (animator != null) switch (animator.name)
                {
                        case "Menu Animate In Out":
                            MenuTopFleurAnimator = animator;
                            break;
                        case "Menu Fleur":
                            MenuCursorAnimator = animator;
                            break;
                        case "Menu Flash Effect":
                            MenuButtonFlashAnimator = animator;
                            break;
                }
            }
            foreach (var animator in Resources.FindObjectsOfTypeAll<AnimatorOverrideController>())
            {
                if (animator != null)
                    TextHideShowAnimator = animator.name switch
                    {
                        "TextHideShow" => animator,
                        _ => TextHideShowAnimator
                    };
            }
            foreach (var font in Resources.FindObjectsOfTypeAll<Font>())
            {
                if (font != null) switch (font.name)
                {
                    case "TrajanPro-Regular":
                        TrajanProRegular = font;
                        break;
                    case "TrajanPro-Bold":
                        TrajanProBold = font;
                        break;
                    case "Perpetua":
                        Perpetua = font;
                        break;
                    case "NotoSerifCJKsc-Regular":
                        NotoSerifCJKSCRegular = font;
                        break;
                    }
            }
            foreach (var sprite in Resources.FindObjectsOfTypeAll<Sprite>())
            {
                if (sprite != null) switch (sprite.name)
                {
                    case "scrollbar_fleur_new":
                        ScrollbarHandleSprite = sprite;
                        break;
                    case "scrollbar_single":
                        ScrollbarBackgroundSprite = sprite;
                        break;
                }
            }
        }
    }