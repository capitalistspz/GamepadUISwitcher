using System;
using System.Linq;
using BepInEx.Configuration;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI;

public static class Objects
{
    public static class FontSize
    {
        public const int OptionLabel = 46;
        public const int OptionText = 38;
        public const int OptionDescription = 41;
        public const int MenuButton = 45;
    }

    public static GameObject CreateMenuButton(string buttonId, string label, MenuButton.MenuButtonType actionType,
        Action<BaseEventData> submitAction)
    {
        var buttonObj = new GameObject(buttonId);

        buttonObj.AddComponent<RectTransform>();
        var menuButtonComp = buttonObj.AddComponent<MenuButton>();
        menuButtonComp.buttonType = actionType;
        menuButtonComp.navigation = new Navigation { mode = Navigation.Mode.Explicit };

        var eventTriggerComp = buttonObj.AddComponent<EventTrigger>();
        var triggerEntrySubmit = new EventTrigger.Entry();
        triggerEntrySubmit.eventID = EventTriggerType.Submit;
        triggerEntrySubmit.callback.AddListener(data =>
        {
            submitAction(data);
        });
        var triggerEntryClick = new EventTrigger.Entry();
        triggerEntryClick.eventID = EventTriggerType.PointerClick;
        triggerEntryClick.callback.AddListener(data =>
        {
            submitAction(data);
        });
        
        eventTriggerComp.triggers.AddRange([triggerEntrySubmit, triggerEntryClick]);
        {
            var menuButtonTextObj = new GameObject("Menu Button Text");

            var menuButtonTextTransform = menuButtonTextObj.AddComponent<RectTransform>();
            menuButtonTextTransform.anchorMax = Vector2.one;
            menuButtonTextTransform.anchorMin = Vector2.zero;
            
            var textComp = menuButtonTextObj.AddComponent<Text>();
            textComp.font = MenuResources.TrajanProBold;
            textComp.fontSize = FontSize.MenuButton;
            textComp.lineSpacing = -0.33f;
            textComp.text = label;
            textComp.alignment = TextAnchor.MiddleCenter;

            var fontScaleComp = menuButtonTextObj.AddComponent<ChangeTextFontScaleOnHandHeld>();
            fontScaleComp.normalSize = FontSize.MenuButton;
            fontScaleComp.handHeldSize = FontSize.MenuButton;

            menuButtonTextObj.AddComponent<FixVerticalAlign>();
            
            var fitterComp = menuButtonTextObj.AddComponent<ContentSizeFitter>();
            fitterComp.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitterComp.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            
            menuButtonTextObj.transform.SetParent(buttonObj.transform);
            
            {
                var cursorRightObj = new GameObject("CursorRight");

                var rectTransform = cursorRightObj.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(164f, 119f);
                rectTransform.anchorMin = new Vector2(1f, 0.5f);
                rectTransform.anchorMax = new Vector2(1f, 0.5f);
                rectTransform.anchoredPosition = new Vector2(70f, 0f);
                rectTransform.localScale = new Vector2(-0.9f, 0.9f);

                cursorRightObj.AddComponent<Image>();
            
                var animatorComp = cursorRightObj.AddComponent<Animator>();
                animatorComp.runtimeAnimatorController = MenuResources.MenuCursorAnimator;
                animatorComp.updateMode = AnimatorUpdateMode.UnscaledTime;
            
                menuButtonComp.rightCursor = animatorComp;
                
                cursorRightObj.transform.SetParent(menuButtonTextObj.transform, false);
            }
            {
                var cursorLeftObj = new GameObject("CursorLeft");

                var rectTransform = cursorLeftObj.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(164f, 119f);
                rectTransform.anchorMin = new Vector2(0f, 0.5f);
                rectTransform.anchorMax = new Vector2(0f, 0.5f);
                rectTransform.anchoredPosition = new Vector2(-70f, 0f);
                rectTransform.localScale = new Vector2(0.9f, 0.9f);

                cursorLeftObj.AddComponent<Image>();
            
                var animatorComp = cursorLeftObj.AddComponent<Animator>();
                animatorComp.runtimeAnimatorController = MenuResources.MenuCursorAnimator;
                animatorComp.updateMode = AnimatorUpdateMode.UnscaledTime;
            
                menuButtonComp.leftCursor = animatorComp;
                
                cursorLeftObj.transform.SetParent(menuButtonTextObj.transform, false);
            }
        }
        return buttonObj;
    }
    
    public static GameObject CreateBepinexConfigOptionTranslated<T>(string objectId, string tlSheet, string labelKey, string descriptionKey,
         T[] options, ConfigEntry<T> entry)
    {
        var optionObj = new GameObject(objectId);
        optionObj.SetActive(false);
        
        var rect = optionObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(1100f, 60f);
        
        optionObj.AddComponent<CanvasRenderer>();
        var menuOptionComp = optionObj.AddComponent<BepinexMenuOptionHorizontal>();
        menuOptionComp.optionList = options.Cast<object>().ToList();
        menuOptionComp.transition = Selectable.Transition.None;
        menuOptionComp.configEntry = entry;
        menuOptionComp.navigation = new Navigation { mode = Navigation.Mode.Explicit };
        menuOptionComp.sheetTitle = tlSheet;
        
        {
            var menuOptionLabelObj = new GameObject("Menu Option Label");
            // Has to be disabled to avoid triggering AutoLocalizeTextUI.OnEnable, and possibly other components,
            // when component is added 
            menuOptionLabelObj.SetActive(false);
            
            var transformComp = menuOptionLabelObj.AddComponent<RectTransform>();
            transformComp.anchoredPosition = new Vector2(450f, 0f);
            transformComp.sizeDelta = new Vector2(900f, 1f);
            transformComp.anchorMax = new Vector2(0f, 1f);
            transformComp.anchorMin = new Vector2(0f, 0f);
            
            var textComp = menuOptionLabelObj.AddComponent<Text>();
            textComp.alignment = TextAnchor.MiddleLeft;
            textComp.font = MenuResources.TrajanProBold;
            textComp.fontSize = FontSize.OptionLabel;
            textComp.lineSpacing = -0.33f;
            textComp.horizontalOverflow = HorizontalWrapMode.Overflow;
            
            var autoLocalizeComp = menuOptionLabelObj.AddComponent<AutoLocalizeTextUI>();
            autoLocalizeComp.textField = textComp;
            autoLocalizeComp.text = new LocalisedString(tlSheet, labelKey);
            
            var fontScaleComp = menuOptionLabelObj.AddComponent<ChangeTextFontScaleOnHandHeld>();
            fontScaleComp.handHeldSize = FontSize.OptionLabel;
            fontScaleComp.normalSize = FontSize.OptionLabel;
                
            menuOptionLabelObj.AddComponent<FixVerticalAlign>();
            
            menuOptionLabelObj.transform.SetParent(optionObj.transform, false);
            menuOptionLabelObj.SetActive(true);
        }
        {
            var menuOptionTextObj = new GameObject("Menu Option Text");
            menuOptionTextObj.SetActive(false);
            var transformComp = menuOptionTextObj.AddComponent<RectTransform>();
            transformComp.anchoredPosition = new Vector2(-100f, 0f);
            transformComp.sizeDelta = new Vector2(200f, 1f);
            transformComp.anchorMax = new Vector2(1f, 1f);
            transformComp.anchorMin = new Vector2(1f, 0f);
            
            var textComp = menuOptionTextObj.AddComponent<Text>();
            textComp.alignment = TextAnchor.MiddleRight;
            textComp.font = MenuResources.TrajanProBold;
            textComp.fontSize = FontSize.OptionText;
            textComp.lineSpacing = -0.33f;
            textComp.horizontalOverflow = HorizontalWrapMode.Overflow;
            
            var fontScaleComp = menuOptionTextObj.AddComponent<ChangeTextFontScaleOnHandHeld>();
            fontScaleComp.handHeldSize = FontSize.OptionText;
            fontScaleComp.normalSize = FontSize.OptionText;
                
            menuOptionTextObj.AddComponent<FixVerticalAlign>();
            
            menuOptionTextObj.transform.SetParent(optionObj.transform, false);

            menuOptionComp.optionText = textComp;
            menuOptionTextObj.SetActive(true);
        }
        {
            var cursorRightObj = new GameObject("CursorRight");

            var rectTransform = cursorRightObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(164f, 119f);
            rectTransform.anchorMin = new Vector2(1f, 0.5f);
            rectTransform.anchorMax = new Vector2(1f, 0.5f);
            rectTransform.anchoredPosition = new Vector2(70f, 0f);
            rectTransform.localScale = new Vector2(-0.9f, 0.9f);

            cursorRightObj.AddComponent<Image>();
            
            var animatorComp = cursorRightObj.AddComponent<Animator>();
            animatorComp.runtimeAnimatorController = MenuResources.MenuCursorAnimator;
            animatorComp.updateMode = AnimatorUpdateMode.UnscaledTime;
            
            menuOptionComp.rightCursor = animatorComp;
            cursorRightObj.transform.SetParent(optionObj.transform, false);

        }
        {
            var cursorLeftObj = new GameObject("CursorLeft");

            var rectTransform = cursorLeftObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(164f, 119f);
            rectTransform.anchorMin = new Vector2(0f, 0.5f);
            rectTransform.anchorMax = new Vector2(0f, 0.5f);
            rectTransform.anchoredPosition = new Vector2(-70f, 0f);
            rectTransform.localScale = new Vector2(0.9f, 0.9f);

            cursorLeftObj.AddComponent<Image>();
            
            var animatorComp = cursorLeftObj.AddComponent<Animator>();
            animatorComp.runtimeAnimatorController = MenuResources.MenuCursorAnimator;
            animatorComp.updateMode = AnimatorUpdateMode.UnscaledTime;
            
            menuOptionComp.leftCursor = animatorComp;
            cursorLeftObj.transform.SetParent(optionObj.transform, false);
        }
        {
            var descriptionObj = new GameObject("Description");
            descriptionObj.SetActive(false);
            
            var transformComp = descriptionObj.AddComponent<RectTransform>();
            transformComp.anchoredPosition = new Vector2(2f, -55.8f);
            transformComp.sizeDelta = new Vector2(875.1f, 61f);
            transformComp.anchorMax = new Vector2(0f, 0.5f);
            transformComp.anchorMin = new Vector2(0f, 0.5f);
            transformComp.pivot = new Vector2(0f, 0.5f);
            
            var textComp = descriptionObj.AddComponent<Text>();
            textComp.alignment = TextAnchor.MiddleLeft;
            textComp.font = MenuResources.Perpetua;
            textComp.fontSize = FontSize.OptionDescription;
            textComp.lineSpacing = 1;
            textComp.horizontalOverflow = HorizontalWrapMode.Overflow;
            
            var autoLocalizeComp = descriptionObj.AddComponent<AutoLocalizeTextUI>();
            autoLocalizeComp.textField = textComp;
            autoLocalizeComp.text = new LocalisedString(tlSheet, descriptionKey);

            var animator = descriptionObj.AddComponent<Animator>();
            animator.runtimeAnimatorController = MenuResources.TextHideShowAnimator;
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            
            var fontScaleComp = descriptionObj.AddComponent<ChangeTextFontScaleOnHandHeld>();
            fontScaleComp.handHeldSize = FontSize.OptionDescription;
            fontScaleComp.normalSize = FontSize.OptionDescription;
            
            descriptionObj.transform.SetParent(optionObj.transform, false);
            menuOptionComp.descriptionText = animator;
            descriptionObj.SetActive(true);
        }
        optionObj.SetActive(true);
        return optionObj;
    }
}