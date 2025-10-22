using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Heavily modified MenuOptionHorizontal

namespace UI;

public class BepinexMenuOptionHorizontal : MenuSelectable, IMoveHandler, IEventSystemHandler, IPointerClickHandler, ISubmitHandler
{
	public enum ApplyOnType
	{
		Scroll,
		Submit
	}

	public Text optionText;
	
	public List<object> optionList;
	
	public string sheetTitle;

	public ConfigEntryBase configEntry; 

	public ApplyOnType applySettingOn;

	public CanvasGroup? applyButton;
	
	public int selectedOptionIndex;
	
	private bool hasApplyButton;

	private int currentActiveIndex;

	private Delegate del;

	private bool justUpdatedEntry = false;

	private void OnConfigEntryUpdate(object obj, System.EventArgs evArgs)
	{
		if (justUpdatedEntry)
		{
			justUpdatedEntry = false;
			return;
		}
		RefreshCurrentIndex();
	}

	private new void Awake()
	{
		hasApplyButton = applyButton != null;
	}

	private new void OnEnable()
	{
		// Event added via reflection because ConfigEntryBase doesn't have ConfigEntry.SettingChanged
		if (configEntry != null)
		{
			var evInfo = configEntry.GetType().GetEvent("SettingChanged");
			del = Delegate.CreateDelegate(typeof(EventHandler), this, nameof(OnConfigEntryUpdate));
			evInfo.AddEventHandler(configEntry, del);
		}
		GameManager.instance.RefreshLanguageText += UpdateText;
		RefreshMenuControls();
		UpdateApplyButton();
	}

	private new void OnDisable()
	{
		if (GameManager.instance != null)
			GameManager.instance.RefreshLanguageText -= UpdateText;
		if (configEntry != null)
		{
			var evInfo = configEntry.GetType().GetEvent("SettingChanged");
			evInfo.RemoveEventHandler(configEntry, del);
		}
		
	}

	public new void OnMove(AxisEventData move)
	{
		if (!interactable || MoveOption(move.moveDir))
			return;
		base.OnMove(move);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!interactable) return;
		PointerClickCheckArrows(eventData);
	}

	public void OnSubmit(BaseEventData eventData)
	{
		if (!interactable)
			return;

		if (applySettingOn == ApplyOnType.Submit)
			ApplySettings();
		else
			MoveOption(MoveDirection.Right);
	}

	protected bool MoveOption(MoveDirection dir)
	{
		switch (dir)
		{
		case MoveDirection.Right:
			IncrementOption();
			break;
		case MoveDirection.Left:
			DecrementOption();
			break;
		default:
			return false;
		}
		if (uiAudioPlayer)
		{
			uiAudioPlayer.PlaySlider();
		}
		return true;
	}

	protected void PointerClickCheckArrows(PointerEventData eventData)
	{
		if (leftCursor && IsInside(leftCursor.gameObject, eventData))
		{
			MoveOption(MoveDirection.Left);
		}
		else if (rightCursor && IsInside(rightCursor.gameObject, eventData))
		{
			MoveOption(MoveDirection.Right);
		}
		else
		{
			MoveOption(MoveDirection.Right);
		}
	}

	private bool IsInside(GameObject obj, PointerEventData eventData)
	{
		var component = obj.GetComponent<RectTransform>();
		if (component && RectTransformUtility.RectangleContainsScreenPoint(component, eventData.position, Camera.main))
		{
			return true;
		}
		return false;
	}

	private string OptionToString(int index)
	{
		var opt = optionList[index];
		return Language.Get(opt.ToString(), sheetTitle);
	}

	public string GetSelectedOptionText()
	{
		return OptionToString(selectedOptionIndex);
	}

	public virtual void SetOptionTo(int optionNumber)
	{
		if (optionNumber >= 0 && optionNumber < optionList.Count)
		{
			selectedOptionIndex = optionNumber;
			UpdateText();
			return;
		}
		Debug.LogErrorFormat("{0} - Trying to select an option outside the list size (index: {1} listsize: {2})", name, optionNumber, optionList.Count);
	}

	protected virtual void UpdateText()
	{
		if (optionList.IsNullOrEmpty() || optionText == null)
			return;
		
		optionText.text = GetSelectedOptionText();
		
		var fixAlignComp = optionText.GetComponent<FixVerticalAlign>();
		if (fixAlignComp)
		{
			fixAlignComp.AlignText();
		}
	}

	protected void UpdateSetting()
	{
		justUpdatedEntry = true;
		configEntry.BoxedValue = optionList[selectedOptionIndex];
	}

	protected void DecrementOption()
	{
		if (selectedOptionIndex > 0)
		{
			selectedOptionIndex--;
			if (applySettingOn == ApplyOnType.Scroll)
			{
				UpdateSetting();
			}
			UpdateText();
		}
		else if (selectedOptionIndex == 0)
		{
			selectedOptionIndex = optionList.Count - 1;
			if (applySettingOn == ApplyOnType.Scroll)
			{
				UpdateSetting();
			}
			UpdateText();
		}
		UpdateApplyButton();
	}

	protected void IncrementOption()
	{
		if (selectedOptionIndex >= 0 && selectedOptionIndex < optionList.Count - 1)
		{
			selectedOptionIndex++;
			if (applySettingOn == ApplyOnType.Scroll)
			{
				UpdateSetting();
			}
			UpdateText();
		}
		else if (selectedOptionIndex == optionList.Count - 1)
		{
			selectedOptionIndex = 0;
			if (applySettingOn == ApplyOnType.Scroll)
			{
				UpdateSetting();
			}
			UpdateText();
		}
		UpdateApplyButton();
	}

	public void RefreshMenuControls()
	{
		RefreshCurrentIndex();
		UpdateText();
	}

	public virtual void ApplySettings()
	{
		if (selectedOptionIndex >= 0)
		{
			UpdateSetting();
			RefreshCurrentIndex();
			HideApplyButton();
		}
	}

	public virtual void UpdateApplyButton()
	{
		if (currentActiveIndex == selectedOptionIndex)
		{
			HideApplyButton();
		}
		else
		{
			ShowApplyButton();
		}
	}

	public virtual void RefreshCurrentIndex()
	{
		if (configEntry != null)
		{
			var index = optionList.IndexOf(configEntry.BoxedValue);
			if (index == -1)
			{
				configEntry.BoxedValue = optionList[0];
				SetOptionTo(0);
			}
			else
				SetOptionTo(index);
			
		}
		currentActiveIndex = selectedOptionIndex;
	}

	protected void HideApplyButton()
	{
		if (hasApplyButton)
		{
			applyButton.alpha = 0f;
			applyButton.interactable = false;
			applyButton.blocksRaycasts = false;
		}
	}

	protected void ShowApplyButton()
	{
		if (applySettingOn != ApplyOnType.Scroll && hasApplyButton)
		{
			applyButton.alpha = 1f;
			applyButton.interactable = true;
			applyButton.blocksRaycasts = true;
		}
	}
}
