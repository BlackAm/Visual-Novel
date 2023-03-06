#if !SERVER_DRIVE
using System;
using System.Collections;
using System.Collections.Generic;
using k514;
using UnityEngine;
using UnityEngine.UI;

namespace UI2020
{
    public class MenuUI : AbstractUI
    {
        private static MenuUI _instance;
        public static MenuUI Instance => _instance;
        public GameObject _backGround, _bgMask1, _bgMask2, touchLock;
        public Title _title;
        // public Login _login;
        //public Top _top;
        public Notice _notice;
        public Character _character;
        public EffectsStore _effect;
        public ExteriorStore _exter;
        //public Spear _spear;

        // 서버에서 캐릭터 정보 수신 여부.
        public bool isCharacterList = false;

        private AbstractUI _curStateUI;
        private MenuUIList? _curState;
        
        public enum MenuUIList
        {
            Title,
            Login,
            Connect,
            ServerChange,
            Notice,
            CreateCharacter,
            CharacterSelect,
            Character,
            EffectsStore,
            ExteriorStore,
            //Spear,
            Friend,
            Guild,
            Mail,
            TouchLock
        }

        private void Awake()
        {
            if (_instance == null) _instance = this;
            _title = AddComponent<Title>("Menu/Title");

            // _login = AddComponent<Login>("Menu/Login");

            //_top = AddComponent<Top>("Menu/Top");
            _notice = AddComponent<Notice>("Menu/Notice");

            _character = AddComponent<Character>("Menu/Character");

            _effect = AddComponent<EffectsStore>("Menu/EffectsStore");

            _exter = AddComponent<ExteriorStore>("Menu/ExteriorStore");

            touchLock = Find("Menu/TouchLock").gameObject;

            // 초기 활성화.
            _title.SetActive(true);
            // _login.SetActive(true);
            _notice.SetActive(true);
            _character.SetActive(true);
            //_spear.SetActive(true);

            _title.Initialize();

            /*EffectsStore,
            ExteriorStore,
            Guild*/
            //_curState = MenuUIList.Title;
            
            _curState = null;
            
            // 테스트
            
            ChangeScene(MenuUIList.Title);
        }
        
        public void ChangeScene(MenuUIList targetScene)
        {
            if (_curState == targetScene) return;
            if (_curStateUI != null) _curStateUI.OnDisable();
            _title.SetActive(targetScene == MenuUIList.Title);
            // _login.SetActive(targetScene == MenuUIList.Login);
            //_top.SetActive(targetScene == MenuUIList.Top);
            _notice.SetActive(targetScene == MenuUIList.Notice);
            _character.SetActive(targetScene == MenuUIList.Character);
            _effect.SetActive(targetScene == MenuUIList.EffectsStore);
            _exter.SetActive(targetScene == MenuUIList.ExteriorStore);
            //_spear.SetActive(targetScene == MenuUIList.Spear);
            switch (targetScene)
            {
                case MenuUIList.Title:
                    _curStateUI = _title;
                    break;
                // case MenuUIList.Login:
                //     _curStateUI = _login;
                //     break;
                case MenuUIList.Notice:
                    _curStateUI = _notice;
                    break;
                case MenuUIList.Character:
                    _curStateUI = _character;
                    break;
                case MenuUIList.EffectsStore:
                    _curStateUI = _effect;
                    break;
                case MenuUIList.ExteriorStore:
                    _curStateUI = _exter;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetScene), targetScene, null);
            }
            _curState = targetScene;
            _curStateUI.OnActive();
        }
        
        public bool GetCompareCurState(MenuUIList compareUI)
        {
            return _curState == compareUI;
        }
        
        /// (현재시간 - 마지막 접속시간)을 출력.
        /// 매개변수로 시간 데이터와 출력될 Text 오브젝트를 받습니다.
        public void PrintLastTime(ulong date, Text target)
        {
            // 시간 - 할 시,
            // (일).시:분:초.ssss.....순으로 출력된다.
            // 일이 없을 경우 그냥 다음 단위부터 나온다.
            string answer = "";
            
            // 접속시간이 0인 경우.
            if (date.Equals(0))
            {
                target.text = "접속 시간 없음";
                return;
            }

            var dateString = string.Empty;
            if(date.ToString().Length > 12)
            {
                dateString = "20" + date.ToString().Substring(1, 12);
            }
            else
            {
                dateString = "20" + date;
            }
            
            var newTime = DateTime.ParseExact(dateString, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
            
            // 현재 시간.
            var dateNow = DateTime.Now;
            
            var answerDate = dateNow - newTime;
            if (!answerDate.Days.Equals(0))
            {
                answer = answerDate.Days + "일";
            } else if (!answerDate.Hours.Equals(0))
            {
                if (answerDate.Hours > 24)
                {
                    answer = answerDate.Hours / 24 + "일";
                }
                else
                {
                    answer = answerDate.Hours + "시간";
                }
            } else if (!answerDate.Minutes.Equals(0))
            {
                if (answerDate.Minutes > 60)
                {
                    answer = answerDate.Minutes / 60 + "시간";
                }
                else
                {
                    answer = answerDate.Minutes + "분";
                }
            }
            else
            {
                answer = "";
            }
            
            if (answer.Equals(""))
            {
                target.text = "1분 이내";
            }
            else
            {
                target.text = answer + " 전";
            }
        }
    }
}
#endif