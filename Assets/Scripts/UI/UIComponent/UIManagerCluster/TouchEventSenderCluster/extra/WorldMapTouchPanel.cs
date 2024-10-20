#if !SERVER_DRIVE
using Cysharp.Threading.Tasks;
using UnityEngine;
namespace BlackAm
{
    public class WorldMapTouchPanel : TouchEventSenderCluster
    {
        #region <Fields>

        private float _MaxScaleValue = 4;
        private float _MinScaleValue = 1;
        private float _ScaleSpeed = 2;
        private Vector2 _RectScale;
        private Vector2 _RectPosition;
        private float _MoveSpeed = 50f;

        private Vector2 _lastInputPosition;

        private RectTransform _Canvas;
        public RectTransform _MapCanvas;
        private Vector2 _MapSize;
        #endregion

        #region <Callbacks>

        // 맵이 타겟팅 할 위치입니다.
        private Vector3 field = new Vector3(700, 80);
        private Vector3 snow = new Vector3(265, -270);
        private Vector3 desert = new Vector3(-705, -120);

        public override void OnSpawning()
        {
            base.OnSpawning();
            
            AddInitControlEvent();
            _Canvas = BlackAm.MainGameUI.Instance.gameObject.transform.parent.GetComponent<RectTransform>();
            _MapCanvas = transform.Find("MainGameMapUI/MapControl").GetComponent<RectTransform>();
            _RectScale = _MapCanvas.localScale;
            _RectPosition = _MapCanvas.localPosition;

            _MapSize = new Vector2(_MapCanvas.rect.width, _MapCanvas.rect.height);
            RegistKeyCodeInput<TouchEventDragButtonWorldMapWithGesture>("MainGameMapUI/MapControl", TouchEventRoot.TouchMappingKeyCodeType.ViewControl, 0, ControllerTool.InputEventType.WorldMapControl).OnSpawning();
        }

        public override async UniTask OnScenePreload()
        {
            await UniTask.CompletedTask;
        }

        public void SetTargeting(string position)
        {
            switch (position)
            {
                case "field" :
                    _MapCanvas.localPosition = field;
                    break;
                case "desert" :
                    _MapCanvas.localPosition = desert;
                    break;
                case "snow" :
                    _MapCanvas.localPosition = snow;
                    break;
            }
        }
        
        private void OnEnable()
        {
            SetLastInputPosition(Vector2.zero);
        }
        private void Update()
        {
//#if !UNITY_EDITOR
            if(Input.GetMouseButtonUp(0))
            {
                SetLastInputPosition(Vector2.zero);
            }
//#endif
            OnUpdateUI(Time.deltaTime);
        }

        void AddInitControlEvent()
        {
            ControllerTool.GetInstanceUnSafe
                .GetControllerEventSender(ControllerTool.InputEventType.WorldMapControl)
                ?.GetEventReceiver<ControllerEventReceiver>(ControllerTool.InputEventType.WorldMapControl, OnPropertyModified);
        }

        public void OnPropertyModified(ControllerTool.InputEventType p_Type, ControllerTool.ControlEventPreset p_Preset)
        {
            switch (p_Preset.GestureType)
            {
                case ControllerTool.TouchGestureType.None:
                    WroldMapMove(_MoveSpeed, Time.deltaTime);
                    break;
                case ControllerTool.TouchGestureType.Stable:
                    WroldMapMove(_MoveSpeed, Time.deltaTime);
                    break;
                case ControllerTool.TouchGestureType.Gather:
                    WorldMapScale(-_ScaleSpeed, Time.deltaTime);
                    WroldMapMove(_MoveSpeed, Time.deltaTime);
                    break;
                case ControllerTool.TouchGestureType.Scatter:
                    WorldMapScale(_ScaleSpeed, Time.deltaTime);
                    WroldMapMove(_MoveSpeed, Time.deltaTime);
                    break;
            }
        }

        public void SetLastInputPosition(Vector2 position)
        {
            _lastInputPosition = position;
        }

        private void WroldMapMove(ArrowType p_DirectionType, float speed, float p_DeltaTime)
        {

            if (p_DirectionType == ArrowType.None) return;
            WroldMapMove(speed, p_DeltaTime);
            
        }
        /// <summary>
        /// �̴ϸ� �̵����
        /// </summary>
        private void WroldMapMove(float speed, float p_DeltaTime)
        {
            Vector2 mousePosition =  Input.mousePosition;

            if (_lastInputPosition == Vector2.zero) SetLastInputPosition(mousePosition);

            _RectPosition.x = _RectPosition.x + (mousePosition.x - _lastInputPosition.x) * speed * p_DeltaTime;
            _RectPosition.y = _RectPosition.y + (mousePosition.y - _lastInputPosition.y) * speed * p_DeltaTime;

            float xPos = (_MapSize.x * (_RectScale.x - 1) + (_MapSize.x - _Canvas.rect.width)) * 0.5f;
            float yPos = (_MapSize.y * (_RectScale.y - 1) + (_MapSize.y - _Canvas.rect.height)) * 0.5f;

            if (_RectPosition.x > xPos) _RectPosition.x = xPos;
            if (_RectPosition.x < -xPos) _RectPosition.x = -xPos;

            if (_RectPosition.y > yPos) _RectPosition.y = yPos;
            if (_RectPosition.y < -yPos) _RectPosition.y = -yPos;

            _MapCanvas.localPosition = _RectPosition;
            _lastInputPosition = mousePosition;
        }
        /// <summary>
        /// �̴ϸ� ������ ����
        /// </summary>
        private void WorldMapScale(float speed, float p_DeltaTime)
        {
            _RectScale.x = _RectScale.x - speed * p_DeltaTime;

            if (_RectScale.x > _MaxScaleValue) _RectScale.x = _MaxScaleValue;
            if (_RectScale.x < _MinScaleValue) _RectScale.x = _MinScaleValue;

            _RectScale.y = _RectScale.x;
            _MapCanvas.localScale = _RectScale;
        }
        #endregion
    }
}
#endif