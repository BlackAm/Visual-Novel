#if !SERVER_DRIVE
using System;
using System.Collections.Generic;
using System.Resources;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace BlackAm
{
    public class TopMenu : AbstractUI
    {
        private static TopMenu _instance;

        public static TopMenu Instance
        {
            get => _instance;
        }

        public TopMenuType CurrentType;
        
        public Button DialogueHistory, Setting, HideMenu, Back, AutoDialogue, SkipDialogue,
            Save, Load, QuickSave, QuickLoad;

        public Dictionary<TopMenuType, Action> ButtonAction;
        public List<Button> buttonList;

        public enum TopMenuType
        {
            DialogueSkip,
            DialogueHistory,
            Setting,
            HideMenu,
            Back,
            AutoDialogue,

            Save,
            Load,
            QuickSave,
            QuickLoad,
        }

        public override void OnSpawning()
        {
            base.OnSpawning();
            
            if (!ReferenceEquals(null, _UIObject)) return;
            
            _instance = _instance ? _instance : this;

            buttonList = new List<Button>();
            ButtonAction = new Dictionary<TopMenuType, Action>();

            DialogueHistory = _Transform.Find("DialogueHistory").gameObject.GetComponent<Button>();
            AddButtonEvent("DialogueHistory", () => { OnButtonClicked(DialogueHistory, TopMenuType.DialogueHistory); });
            ButtonAction.Add(TopMenuType.DialogueHistory, ShowDialogueHistory);
            buttonList.Add(DialogueHistory);
            
            Setting = _Transform.Find("Setting").gameObject.GetComponent<Button>();
            AddButtonEvent("Setting", () => { OnButtonClicked(Setting, TopMenuType.Setting); });
            ButtonAction.Add(TopMenuType.Setting, ShowSetting);
            buttonList.Add(Setting);

            HideMenu = _Transform.Find("HideMenu").gameObject.GetComponent<Button>();
            AddButtonEvent("HideMenu", () => { OnButtonClicked(HideMenu, TopMenuType.HideMenu); });
            ButtonAction.Add(TopMenuType.HideMenu, HideTopMenuButton);
            
            Back = _Transform.Find("Back").gameObject.gameObject.GetComponent<Button>();
            Back.interactable = false;
            AddButtonEvent("Back", () => { OnButtonClicked(Back, TopMenuType.Back); });
            ButtonAction.Add(TopMenuType.Back, ActionBack);
            buttonList.Add(Back);

            AutoDialogue = _Transform.Find("DialogueAuto").gameObject.GetComponent<Button>();
            AddButtonEvent("DialogueAuto", () => { OnButtonClicked(AutoDialogue, TopMenuType.AutoDialogue); });
            ButtonAction.Add(TopMenuType.AutoDialogue, SwitchDialogueAutoMode);
            
            SkipDialogue = _Transform.Find("DialogueSkip").gameObject.GetComponent<Button>();
            AddButtonEvent("DialogueSkip", () => { OnButtonClicked(SkipDialogue, TopMenuType.DialogueSkip); });
            ButtonAction.Add(TopMenuType.DialogueSkip, SwitchDialogueSkipMode);
            
            Save = _Transform.Find("Save").gameObject.gameObject.GetComponent<Button>();
            AddButtonEvent("Save", () => { OnButtonClicked(Save, TopMenuType.Save); });
            ButtonAction.Add(TopMenuType.Save, ShowSave);
            buttonList.Add(Save);

            Load = _Transform.Find("Load").gameObject.GetComponent<Button>();;
            AddButtonEvent("Load", () => { OnButtonClicked(Load, TopMenuType.Load); });
            ButtonAction.Add(TopMenuType.Load, ShowLoad);
            buttonList.Add(Load);


            QuickSave = _Transform.Find("QuickSave").gameObject.GetComponent<Button>();
            AddButtonEvent("QuickSave", () => { OnButtonClicked(QuickSave, TopMenuType.QuickSave); });
            ButtonAction.Add(TopMenuType.QuickSave, () => ActionQuickSaveLoad(SaveLoad.SaveLoadMode.Save, SaveLoad.SaveLoadType.Quick).Forget());
            buttonList.Add(QuickSave);
            
            QuickLoad = _Transform.Find("QuickLoad").gameObject.GetComponent<Button>();
            AddButtonEvent("QuickLoad", () => { OnButtonClicked(QuickLoad, TopMenuType.QuickLoad); });
            ButtonAction.Add(TopMenuType.QuickLoad, () => ActionQuickSaveLoad(SaveLoad.SaveLoadMode.Load, SaveLoad.SaveLoadType.Quick).Forget());
            buttonList.Add(QuickLoad);
        }

        public void OnButtonClicked(Button p_Button, TopMenuType p_TopMenuType)
        {
            SetInteractable(p_Button);
            switch (p_TopMenuType)
            {
                case TopMenuType.QuickSave:
                case TopMenuType.QuickLoad:
                    p_Button.interactable = true;
                    break;
            }

            ButtonAction[p_TopMenuType]?.Invoke();
            if (p_TopMenuType != TopMenuType.HideMenu)
            {
                CurrentType = p_TopMenuType;
            }
        }

        public void SetInteractable(Button p_Button)
        {
            foreach (var button in buttonList)
            {
                button.interactable = !CheckInteractable(p_Button.name, button.name);
            }
        }

        public bool CheckInteractable(string p_ButtonName, string p_Name)
        {
            return p_ButtonName == p_Name;
        }

        public void SwitchDialogueAutoMode()
        {
            DialogueGameManager.GetInstance.OnTogglePlayMode(DialogueGameManager.DialoguePlayMode.AutoPlay);
        }

        public void SwitchDialogueSkipMode()
        {
            DialogueGameManager.GetInstance.OnTogglePlayMode(DialogueGameManager.DialoguePlayMode.SkipPlay);
        }

        public void ShowDialogueHistory()
        {
            MainGameUI.Instance.functionUI.OpenUI(FunctionUI.UIIndex.DialogueHistory);
        }

        public void ShowSetting()
        {
            MainGameUI.Instance.functionUI.OpenUI(FunctionUI.UIIndex.Setting);
        }

        public void ShowSave()
        {
            QuickSave.interactable = false;
            QuickLoad.interactable = false;
            // HideMenu.interactable = false;
            SaveLoad.Instance.OpenUI(SaveLoad.SaveLoadMode.Save, false);
        }

        public async UniTask ActionQuickSaveLoad(SaveLoad.SaveLoadMode p_SaveLoadMode, SaveLoad.SaveLoadType p_SaveLoadType)
        {
            await SaveLoad.Instance.QuickSaveItem.ActionSaveOrLoad(p_SaveLoadMode, p_SaveLoadType);
        }

        public void ShowLoad()
        {
            QuickSave.interactable = false;
            QuickLoad.interactable = false;
            // HideMenu.interactable = false;
            SaveLoad.Instance.OpenUI(SaveLoad.SaveLoadMode.Load, true);
        }

        public void HideTopMenuButton()
        {
            
        }

        public void ActionBack()
        {
            switch (CurrentType)
            {
                case TopMenuType.Save:
                case TopMenuType.Load:
                    SaveLoad.Instance.CloseUI();
                    break;
                case TopMenuType.Setting:
                case TopMenuType.DialogueHistory:
                    MainGameUI.Instance.functionUI.CloseUI();
                    break;
            }
            
            // TODO<BlackAm> - 메뉴 뒤로 가기, 다른 창일 시 해당 창 종료 및 선택된 창도 닫아야함
        }

        public void HideTopMenu(bool p_Flag)
        {
            gameObject.SetActive(p_Flag);
        }
    }
}
#endif