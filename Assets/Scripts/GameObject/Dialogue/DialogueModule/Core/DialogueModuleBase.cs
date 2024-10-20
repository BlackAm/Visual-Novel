using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace BlackAm
{
    public abstract class DialogueModuleBase : IDialogue
    {
        #region <Fields>

        protected bool _ModuleValidationFlag;
        public DialogueModuleDataTool.DialogueModuleType DialogueModuleType { get; protected set; }
        public DialogueGameManager _MasterNode { get; protected set; }

        #endregion

        #region <Callbacks>

        public virtual void OnMasterNodePooling()
        {
            TryModuleNotify();
        }

        public virtual void OnMasterNodeRetrieved()
        {
            TryModuleSleep();
        }

        public abstract void OnUpdate(float p_DeltaTime);

        protected abstract void OnModuleNotify();
        protected abstract void OnModuleSleep();

        #endregion
        
        #region <Methods>
        
        public void TryModuleNotify()
        {
            if (!_ModuleValidationFlag)
            {
                _ModuleValidationFlag = true;
                OnModuleNotify();
            }
        }

        public void TryModuleSleep()
        {
            if (_ModuleValidationFlag)
            {
                _ModuleValidationFlag = false;
                OnModuleSleep();
            }
        }

        

        #endregion
        
        #region <Disposable>
        
        /// <summary>
        /// dispose 패턴 onceFlag
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// dispose 플래그를 초기화 시키는 메서드
        /// </summary>
        public void Rejunvenate()
        {
            IsDisposed = false;
        }
        
        /// <summary>
        /// 인스턴스 파기 메서드
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            else
            {
                IsDisposed = true;
                DisposeUnManaged();
            }
        }

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected abstract void DisposeUnManaged();

        #endregion
    }

}
