namespace k514
{
    /// <summary>
    /// 프리팹 로드/릴리스 시, 이벤트를 처리하는 핸들러 싱글톤
    /// </summary>
    public class PrefabLoadEventHandler : Singleton<PrefabLoadEventHandler>
    {
        #region <Fields>

        /// <summary>
        /// 프리팹 로드 시, 로드된 프리팹의 원본 랜더링 정보를 알고 있어야하므로 프리팹 로드 이벤트를 구독한다.
        /// </summary>
        public PrefabLoadEventReceiver _PrefabLoadEventReceiver;

        #endregion

        #region <Callbacks>

        protected override void OnCreated()
        {
            _PrefabLoadEventReceiver = PrefabEventSender.GetInstance.GetEventReceiver(OnPropertyChanged);
        }

        public override void OnInitiate()
        {
        }

        private void OnPropertyChanged(PrefabEventSender.UnityPrefabEventType p_EventType,
            PrefabPoolingTool.PrefabIdentifyKey p_PrefabKey)
        {
            switch (p_EventType)
            {
                case PrefabEventSender.UnityPrefabEventType.OnPrefabLoad:
                    OnPrefabLoaded(p_PrefabKey);
                    break;
                case PrefabEventSender.UnityPrefabEventType.OnPrefabRelease:
                    OnPrefabReleased(p_PrefabKey);
                    break;
            }
        }

        public void OnPrefabLoaded(PrefabPoolingTool.PrefabIdentifyKey p_PrefabKey)
        {
        }
        
        public void OnPrefabReleased(PrefabPoolingTool.PrefabIdentifyKey p_PrefabKey)
        {
        }

        #endregion

        #region <Disposable>

        protected override void DisposeUnManaged()
        {
            _PrefabLoadEventReceiver.Dispose();
            _PrefabLoadEventReceiver = null;
            
            base.DisposeUnManaged();
        }

        #endregion
    }
}