#if !SERVER_DRIVE
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 카메라의 아핀 변환(스케일을 제외한 평행이동, 회전)은 각각의 Transform이 서로의 모델계에 영향을 주는 방식으로
    /// 구현해야하는데, 해당 변환은 크게 '테이블에 의해 초기화 되는 변환', '특정 이벤트를 처리하기 위해 임시적으로 적용되는 변환'
    /// 등으로 나뉜다.
    ///
    /// 해당 클래스는 각 케이스에 공통적으로 적용할 아핀 변환 관련 기능을 기술하는 클래스이다.
    /// 
    /// </summary>
    public class CameraAffineTransform
    {
        #region <Fields>

        /// <summary>
        /// 해당 아핀 변환 제어 오브젝트의 래퍼 타입
        /// </summary>
        private CameraManager.CameraWrapperType _ThisType;

        /// <summary>
        /// 해당 오브젝트가 보유한 변환 이벤트 핸들러 컬렉션
        /// 브로드 캐스트 용으로 활용
        /// </summary>
        public Dictionary<CameraManager.CameraAffineTransformType, CameraAffineTransformPreset> _AffineBasisCollection;

        /// <summary>
        /// 회전 변환 이벤트 핸들러
        /// </summary>
        public DirectionLerpEventTimerHandler RotateEventHandler;

        /// <summary>
        /// View 벡터 변환 이벤트 핸들러
        /// </summary>
        public FloatLerpEventTimerHandler ZoomEventHandler;

        /// <summary>
        /// 카메라 초점 Offset 좌표 변환 이벤트 핸들러
        /// </summary>
        public PositionLerpEventTimerHandler FocusEventHandler;

        /// <summary>
        /// 해당 오브젝트의 최상위 래퍼 Transform
        /// </summary>
        public Transform Head;

        /// <summary>
        /// 해당 오브젝트의 아핀 래퍼 Transform
        /// </summary>
        public Transform RotateWrapper, ZoomWrapper, FocusWrapper;

        /// <summary>
        /// 해당 오브젝트의 최하위 래퍼 Transform
        /// </summary>
        public Transform Rear;

        /// <summary>
        /// 앙각 회전은 극점에 도달하면 격한 회전을 하기 때문에, 극점에 도달하기전
        /// 상한/하한값을 지정해줘야한다.
        /// </summary>
        private float _RotateZBound;

        /// <summary>
        /// 인벤토리를 열 경우 자동으로 저장해 주는 Transform
        /// </summary>
        private (Vector3 _SaveRotate, float _SaveZoom, Vector3 _SaveFocus) _SaveOffset;

        #endregion

        #region <Constructor>

        public CameraAffineTransform(CameraManager.CameraWrapperType p_WrapperType)
        {
            _ThisType = p_WrapperType;
            
            _AffineBasisCollection = new Dictionary<CameraManager.CameraAffineTransformType, CameraAffineTransformPreset>();
            var enumerator = CameraManager.GetInstanceUnSafe.CameraAffineTypeEnumerator;
            var typeCount = enumerator.Length;
            
            for (int i = 0; i < typeCount; i++)
            {
                var targetType = enumerator[i];
                var spawnedTransform = new GameObject($"{_ThisType}_{targetType}_Wrapper").transform;

                switch (targetType)
                {
                    case CameraManager.CameraAffineTransformType.Focus:
                        FocusWrapper = spawnedTransform;
                        FocusEventHandler = new PositionLerpEventTimerHandler(SystemBoot.GameEventTimer);
                        FocusEventHandler.SetValueChangedCallback(OnFocusChanged);
                        _AffineBasisCollection.Add(targetType, new CameraAffineTransformPreset(spawnedTransform, FocusEventHandler));
                        break;
                    case CameraManager.CameraAffineTransformType.Rotate:
                        RotateWrapper = spawnedTransform;
                        RotateEventHandler = new DirectionLerpEventTimerHandler(SystemBoot.GameEventTimer);
                        RotateEventHandler.SetValueChangedCallback(OnRotateChanged);
                        _AffineBasisCollection.Add(targetType, new CameraAffineTransformPreset(spawnedTransform, RotateEventHandler));
                        break;
                    case CameraManager.CameraAffineTransformType.Zoom:
                        ZoomWrapper = spawnedTransform;
                        ZoomEventHandler = new FloatLerpEventTimerHandler(SystemBoot.GameEventTimer);
                        ZoomEventHandler.SetValueChangedCallback(OnZoomChanged);
                        _AffineBasisCollection.Add(targetType, new CameraAffineTransformPreset(spawnedTransform, ZoomEventHandler));
                        break;
                }

                if (i == 0)
                {
                    Head = spawnedTransform;
                }
                else
                {
                    var prevType = enumerator[i - 1];
                    var prevTransform = _AffineBasisCollection[prevType]._Transform;
                    spawnedTransform.SetParent(prevTransform, false);
                }

                if (i == typeCount - 1)
                {
                    Rear = spawnedTransform;
                }
            }
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 파라미터로 받은 테이블 레코드 값을 기준으로 아핀 값을 세트하는 메서드.
        /// 해당 메서드 호출 시, 자동으로 Apply_AffineFeature_From_CurrentMapSetting 메서드도 호출된다.
        /// </summary>
        public void SetAffineTransform(SceneVariableData.TableRecord p_SceneVariableRecord)
        {
            // 최초 생성 기본 래퍼를 기준으로 Transform Tracker 초기화
            foreach (var affineTransformPreset in _AffineBasisCollection)
            {
                affineTransformPreset.Value.ResetTransformPreset();
            }

            // 해당 회전 래퍼의 초기 회전도를 씬 테이블로부터 계산해준다.
            var rotateTransform = _AffineBasisCollection[CameraManager.CameraAffineTransformType.Rotate]._Transform;
            var defaultForward = rotateTransform.forward
                .VectorRotationUsingCrossingProduct(Vector3.down, p_SceneVariableRecord.CameraRootWrapperTiltDegree)
                .VectorRotationUsingCrossingProduct(Vector3.right, p_SceneVariableRecord.CameraRootWrapperSightDegree);

            // 카메라 회전 이벤트 객체 초기화
            RotateEventHandler.SetDefaultValue(defaultForward);

            // 카메라 거리
            ZoomEventHandler.SetDefaultValue(p_SceneVariableRecord.CameraDistance);

            // 카메라 초점
            FocusEventHandler.SetDefaultValue(p_SceneVariableRecord.TraceOffset);

            // 선정된 초기 변환값을 각 래퍼에 적용해준다.
            Apply_AffineFeature_From_CurrentMapSetting();

            // 초기화된 래퍼를 기준으로 Transform Tracker 초기화
            foreach (var affineTransformPreset in _AffineBasisCollection)
            {
                affineTransformPreset.Value.UpdateTransformPreset();
            }
        }

        /// <summary>
        /// 모든 핸들러를 초기화 시키는 메서드
        /// </summary>
        public void ResetAllAffineTransform()
        {
            var enumerator = CameraManager.GetInstanceUnSafe.CameraAffineTypeEnumerator;
            foreach (var cameraAffineTransformType in enumerator)
            {
                _AffineBasisCollection[cameraAffineTransformType]._EventHandler.ResetValue();
                _AffineBasisCollection[cameraAffineTransformType]._TransformPreset.ResetAffine();
            }
        }

        /// <summary>
        /// 모든 카메라 이벤트를 종료시킨다.
        /// </summary>
        public void CancelAllCameraEvent()
        {
            var enumerator = CameraManager.GetInstanceUnSafe.CameraAffineTypeEnumerator;
            foreach (var cameraAffineTransformType in enumerator)
            {
                _AffineBasisCollection[cameraAffineTransformType]._EventHandler.CancelEvent();
            }
        }

        /// <summary>
        /// 현재 아핀 변환을 수행중인 핸들러가 있는지 체크하는 논리 메서드
        /// </summary>
        public bool IsOnAffineTransform()
        {
            return FocusEventHandler.IsEventValid() || RotateEventHandler.IsEventValid() ||
                   ZoomEventHandler.IsEventValid();
        }

        #endregion

        #region <Method/EventBind>

        public void Apply_AffineFeature_From_CurrentMapSetting()
        {
            OnRotateChanged();
            OnZoomChanged();
            OnFocusChanged();
        }

        //향후 조정은 xml처리 해야됨
        public void Apply_AffineFeature_From_InventoryMapSettingOn()
        {
            InventoryOnRotateChanged();
            InventoryOnZoomChanged();
            InventoryOnFocusChanged();
        }

        public void Apply_AffineFeature_From_InventoryMapSettingOff()
        {
            InventoryOffRotateChanged();
            InventoryOffZoomChanged();
            InventoryOffFocusChanged();
            //기존 카메라 Affine세팅을 로드했을 경우 바로 폐기
            _SaveOffset = default;
        }

        public void OnRotateChanged()
        {
            if (CameraManager.GetInstanceUnSafe.Ancher) return;
            var rotateTransform = _AffineBasisCollection[CameraManager.CameraAffineTransformType.Rotate]._Transform;
            rotateTransform.forward = RotateEventHandler._CurrentValue;
        }

        public void OnZoomChanged()
        {
            if (CameraManager.GetInstanceUnSafe.Ancher) return;
            var targetTransform = _AffineBasisCollection[CameraManager.CameraAffineTransformType.Zoom]._Transform;
            targetTransform.localPosition = Vector3.Scale(Vector3.forward, targetTransform.InverseTransformVector(RotateWrapper.forward) * -ZoomEventHandler._CurrentValue);
        }

        public void OnFocusChanged()
        {
            if (CameraManager.GetInstanceUnSafe.Ancher) return;
            var targetTransform = _AffineBasisCollection[CameraManager.CameraAffineTransformType.Focus]._Transform;
            targetTransform.localPosition = Vector3.Scale(new Vector3(1f, 1f, 0f), FocusEventHandler._CurrentValue);
        }

        public void InventoryOnRotateChanged()
        {
            var rotateTransform = _AffineBasisCollection[CameraManager.CameraAffineTransformType.Rotate]._Transform;

            _SaveOffset._SaveRotate = rotateTransform.localRotation.eulerAngles;
#if UNITY_EDITOR
            if (CustomDebug.PrintGameSystemLog)
                Debug.Log($"카메라 세팅 세이브 _LockVector._LockRotate : ({rotateTransform.localRotation.eulerAngles}) / ({_SaveOffset._SaveRotate})");
#endif
            rotateTransform.localRotation = Quaternion.Euler(0, (LamiereGameManager.GetInstanceUnSafe._ClientPlayer._Transform.localRotation.eulerAngles.y) - 174.5f, 0);
#if UNITY_EDITOR
            if (CustomDebug.PrintGameSystemLog)
                Debug.Log($"카메라 인벤토리 세팅 OnRotateChanged : ({rotateTransform.rotation.eulerAngles})");
#endif
        }

        public void InventoryOffRotateChanged()
        {
            var rotateTransform = _AffineBasisCollection[CameraManager.CameraAffineTransformType.Rotate]._Transform;
            if (CameraManager.GetInstanceUnSafe.Ancher) rotateTransform.localRotation = Quaternion.Euler(_SaveOffset._SaveRotate);

#if UNITY_EDITOR
            if (CustomDebug.PrintGameSystemLog)
                Debug.Log($"카메라 세팅 로드 _LockVector._LockRotate : ({_SaveOffset._SaveRotate})");
#endif
            if (CameraManager.GetInstanceUnSafe.Ancher) rotateTransform.localRotation = Quaternion.Euler(_SaveOffset._SaveRotate);

#if UNITY_EDITOR
            if (CustomDebug.PrintGameSystemLog)
                Debug.Log($"카메라 기존 세팅 OnRotateChanged : ({rotateTransform.rotation.eulerAngles})");
#endif
        }



        public void InventoryOnZoomChanged()
        {
            var targetTransform = _AffineBasisCollection[CameraManager.CameraAffineTransformType.Zoom]._Transform;

            float _z = targetTransform.localPosition.z;
            _SaveOffset._SaveZoom = _z;
#if UNITY_EDITOR
            if (CustomDebug.PrintGameSystemLog)
                Debug.Log($"카메라 세팅 세이브 _LockVector._LockZoom : ({targetTransform.localPosition.z}) / ({_SaveOffset._SaveZoom})");
#endif
            targetTransform.localPosition = new Vector3(0, 0, -3f);

#if UNITY_EDITOR
            if (CustomDebug.PrintGameSystemLog)
                Debug.Log($"카메라 인벤토리 세팅 OnZoomChanged : ({targetTransform.localPosition})");
#endif
        }

        public void InventoryOffZoomChanged()
        {
            var targetTransform = _AffineBasisCollection[CameraManager.CameraAffineTransformType.Zoom]._Transform;

            if (CameraManager.GetInstanceUnSafe.Ancher) targetTransform.localPosition = new Vector3(0, 0, _SaveOffset._SaveZoom);
#if UNITY_EDITOR
            if (CustomDebug.PrintGameSystemLog)
                Debug.Log($"카메라 세팅 로드 _LockVector._LockZoom : ({_SaveOffset._SaveZoom})");
#endif
            if (CameraManager.GetInstanceUnSafe.Ancher) targetTransform.localPosition = new Vector3(0, 0, _SaveOffset._SaveZoom);

#if UNITY_EDITOR
            if (CustomDebug.PrintGameSystemLog)
                Debug.Log($"카메라 기존 세팅 OnZoomChanged : ({targetTransform.localPosition})");
#endif
        }

        public void InventoryOnFocusChanged()
        {
            var targetTransform = _AffineBasisCollection[CameraManager.CameraAffineTransformType.Focus]._Transform;

            _SaveOffset._SaveFocus = targetTransform.localPosition;
#if UNITY_EDITOR
            if (CustomDebug.PrintGameSystemLog)
                Debug.Log($"카메라 세팅 세이브 _LockVector._LockFocus : ({targetTransform.localPosition}) / ({_SaveOffset._SaveFocus})");
#endif
            targetTransform.localPosition = new Vector3(0, 0.8f, 0);

#if UNITY_EDITOR
            if (CustomDebug.PrintGameSystemLog)
                Debug.Log($"카메라 인벤토리 세팅 OnFocusChanged : ({targetTransform.localPosition})");
#endif
        }

        public void InventoryOffFocusChanged()
        {
            var targetTransform = _AffineBasisCollection[CameraManager.CameraAffineTransformType.Focus]._Transform;

            if (CameraManager.GetInstanceUnSafe.Ancher) targetTransform.localPosition = _SaveOffset._SaveFocus;
#if UNITY_EDITOR
            if (CustomDebug.PrintGameSystemLog)
                Debug.Log($"카메라 세팅 로드 _LockVector._LockFocus : ({_SaveOffset._SaveFocus})");
#endif
            if (CameraManager.GetInstanceUnSafe.Ancher) targetTransform.localPosition = _SaveOffset._SaveFocus;

#if UNITY_EDITOR
            if (CustomDebug.PrintGameSystemLog)
                Debug.Log($"카메라 기존 세팅 OnFocusChanged : ({targetTransform.localPosition})");
#endif
        }

        #endregion

        #region <Structs>

        public class CameraAffineTransformPreset
        {
            #region <Fields>

            public Transform _Transform;
            public TransformTool.TransformPreset _TransformPreset;
            public TransformTool.TransformPreset _DefaultTransformPreset;
            public ILerpEventTimerHandler _EventHandler;

            #endregion

            #region <Constructor>

            public CameraAffineTransformPreset(Transform p_Transform, ILerpEventTimerHandler p_EventHandler)
            {
                _Transform = p_Transform;
                _DefaultTransformPreset = _TransformPreset = _Transform.GetTransformPreset();
                _EventHandler = p_EventHandler;
            }

            #endregion

            #region <Methods>

            public void UpdateTransformPreset()
            {
                _TransformPreset = _Transform.GetTransformPreset();
            }

            public void ResetTransformPreset()
            {
                _DefaultTransformPreset.ResetAffine();
            }

            #endregion
        }

        #endregion
    }
}
#endif