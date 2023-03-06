using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace k514
{
    public partial class Unit
    {
        #region <Fields>

        protected Dictionary<UnitAttachPoint, Transform> _AttachPointRecord;

        #endregion

        #region <Enums>

        public enum UnitAttachPoint
        {
            /// <summary>
            /// 스크립트 컴포넌트가 붙어있는 래퍼
            /// </summary>
            MainTransform,
            
            /// <summary>
            /// 애니메이션의 주체가 되는 래퍼
            /// </summary>
            AnimationRootNode,

            /// <summary>
            /// 애니메이션 모델의 중심 본
            /// </summary>
            BoneCenterNode,
            
            /// <summary>
            /// 주 공격 모델을 표시하는 래퍼
            /// </summary>
            MainWeapon,
        }

        #endregion

        #region <Indexer>

        public Transform this[UnitAttachPoint p_AttachPoint] => _AttachPointRecord[p_AttachPoint];

        #endregion
        
        #region <Callbacks>

        protected void OnAwakeAttachPoint()
        {
            _AttachPointRecord = new Dictionary<UnitAttachPoint, Transform>();
            var modelTableIndex = _PrefabModelKey.Item1 ? _PrefabModelKey.Item2 : 0;
            var modelRecord = (UnitModelDataRecordBridge)PrefabModelDataRoot.GetInstanceUnSafe[modelTableIndex];
            var attachPointNameMap = UnitAttachPointData.GetInstanceUnSafe[modelRecord.AttachPointIndex].AttachPointNameMap;
            
            if (SystemTool.TryGetEnumEnumerator<UnitAttachPoint>(SystemTool.GetEnumeratorType.GetAll, out var o_Enumerator))
            {
                foreach (var attachPoint in o_Enumerator)
                {
                    switch (attachPoint)
                    {
                        case UnitAttachPoint.MainTransform:
                            _AttachPointRecord.Add(attachPoint, _Transform);
                            break;
                        default:
                            if (attachPointNameMap.TryGetValue(attachPoint, out var o_BoneNameList) && o_BoneNameList.CheckGenericCollectionSafe())
                            {
                                var (valid, bone) = _Transform.FindRecursiveInclude(o_BoneNameList, false);
                                if (valid)
                                {
                                    _AttachPointRecord.Add(attachPoint, bone);
                                }
                                else
                                {
                                    _AttachPointRecord.Add(attachPoint, _Transform);
                                }
                            }
                            else
                            {
                                _AttachPointRecord.Add(attachPoint, _Transform);
                            }
                            break;
                    }
                }
            }
        }

        private void OnPoolingAttachPoint()
        {
        }

        private void OnRetrieveAttachPoint()
        {
        }

        #endregion
        
        #region <Methods>

        public void SetAttachPoint(UnitAttachPoint p_TargetPoint, Transform p_ReplaceTransform)
        {
            if (p_TargetPoint != UnitAttachPoint.MainTransform && !ReferenceEquals(null, p_ReplaceTransform))
            {
                _AttachPointRecord[p_TargetPoint] = p_ReplaceTransform;
            }
        }
        
        public Transform GetAttachPoint(UnitAttachPoint p_TargetPoint)
        {
            return _AttachPointRecord[p_TargetPoint];
        }

        public Vector3 GetAttachPosition(UnitAttachPoint p_TargetPoint)
        {
            return _AttachPointRecord[p_TargetPoint].position;
        }
        
        #endregion
    }
}