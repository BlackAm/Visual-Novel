using System;
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public static class PrefabPoolingTool
    {
        #region <Fields>

        /// <summary>
        /// PrefabIdentifyKey 정적 비교자
        /// </summary>
        public static PrefabIdentifyComparer _PrefabIdentifyComparer;

        #endregion

        #region <Enums>

        public enum ObjectSpawnType
        {
            PoolerSpawn,
            AddComponentSpawn,
        }

        #endregion
        
        #region <Constructor>

        static PrefabPoolingTool()
        {
            _PrefabIdentifyComparer = new PrefabIdentifyComparer();
        }

        #endregion

        #region <Class>

        /// <summary>
        /// 프리팹 풀링은 다수의 Value로 구성되는 구조체가 Key 타입이기 때문에
        /// 전용 비교자를 통해서 Key 유효성을 검증해야한다.
        /// </summary>
        public class PrefabIdentifyComparer : IEqualityComparer<PrefabIdentifyKey>
        {
            public bool Equals(PrefabIdentifyKey x, PrefabIdentifyKey y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(PrefabIdentifyKey obj)
            {
                return obj.GetHashCode();
            }
        }

        #endregion
        
        #region <Structs>
 
        /// <summary>
        /// 프리팹 풀링 컬렉션의 슈퍼키를 구성하기 위해 추가된 구조체
        /// </summary>
        public struct PrefabIdentifyKey
        {
            #region <Fields>

            /// <summary>
            /// 프리팹 이름
            /// </summary>
            public string _PrefabName { get; private set; }

            /// <summary>
            /// 프리팹 수명 타입
            /// </summary>
            public ResourceLifeCycleType _ResourceLifeCycleType { get; private set; }

            /// <summary>
            /// 프리팹에 추가될 스크립트 컴포넌트 타입
            /// </summary>
            public PrefabPoolingManagerPreset _PrefabExtraPreset { get; private set; }

            /// <summary>
            /// 해당 프리팹을 풀링하는 풀러
            /// </summary>
            public IUnityObjectPooler _ThisPrefabPooler;
             
            /// <summary>
            /// 해당 프리셋의 해시 값
            /// </summary>
            private int _Hash;

            /// <summary>
            /// 해당 키를 보유한 프리팹이, 유닛 클래스인지 표시하는 플래그
            /// </summary>
            public bool IsUnitPrefab;

            #endregion

            #region <Constructor>

            public PrefabIdentifyKey(string p_PrefabName, ResourceLifeCycleType p_ResourceLifeCycleType, PrefabPoolingManagerPreset p_PrefabPoolingManagerPreset)
            {
                _PrefabName = p_PrefabName;
                _ResourceLifeCycleType = p_ResourceLifeCycleType;
                _PrefabExtraPreset = p_PrefabPoolingManagerPreset;
                _ThisPrefabPooler = default;
                _Hash = _PrefabName.GetHashCode() * (int) (1 + _ResourceLifeCycleType) * (1 + _PrefabExtraPreset.GetHashCode());
                IsUnitPrefab = _PrefabExtraPreset.ExtraComponentType?.IsSubclassOf(typeof(Unit))??false;
            }

            #endregion
             
            #region <Operator>

            public static bool operator ==(PrefabIdentifyKey p_LeftValue, PrefabIdentifyKey p_RightValue)
            {
                return p_LeftValue.Equals(p_RightValue);
            }

            public static bool operator !=(PrefabIdentifyKey p_LeftValue, PrefabIdentifyKey p_RightValue)
            {
                return !(p_LeftValue == p_RightValue);
            }
             
            public override bool Equals(object p_Right)
            {
                return Equals((PrefabIdentifyKey)p_Right);
            }

            public bool Equals(PrefabIdentifyKey r)
            {
                return _PrefabName == r._PrefabName
                       && _ResourceLifeCycleType == r._ResourceLifeCycleType
                       && _PrefabExtraPreset == r._PrefabExtraPreset;
            }

            public override int GetHashCode()
            {
                return _Hash;
            }

            #endregion

            #region <Methods>

            public void SetPooler(IUnityObjectPooler p_Pooler)
            {
                _ThisPrefabPooler = p_Pooler;
            }

            public GameObject GetPrefabObject()
            {
                return _ThisPrefabPooler._TargetPrefab;
            }

#if UNITY_EDITOR
            public override string ToString()
            {
                return
                    $"[{_PrefabName}] / [{_ResourceLifeCycleType}] / [{GetHashCode()}]";
            }
#endif
            #endregion
        }
        
        /// <summary>
        /// 테이블 클래스 타입 및 해당 테이블 클래스 타입의 레코드 객체를 가지는 튜플
        /// </summary>
        public struct PrefabPoolingManagerPreset
        {
            #region <Fields>

            /// <summary>
            /// 프리팹 엑스트라 데이터 레코드 외의 추가 데이터 타입
            /// </summary>
            private PrefabExtraDataAdditiveType _PrefabExtraDataAdditiveType;
        
            /// <summary>
            /// 프리팹 엑스트라 데이터 레코드 인터페이스 브릿지
            /// </summary>
            public PrefabExtraDataRecordBridge _PrefabExtraDataRecord;

            /// <summary>
            /// 추가 컴포넌트
            /// </summary>
            public Type ExtraComponentType;
        
            /// <summary>
            /// UI매니저 추가 데이터
            /// </summary>
            public (bool, RenderMode, Canvas) Canvas;

            /// <summary>
            /// 프리팹 모델 추가 데이터
            /// </summary>
            public (bool, int) PrefabModelKey;

            /// <summary>
            /// 해시 값
            /// </summary>
            private int Hash;
            
            #endregion

            #region <Enums>

            /// <summary>
            /// 해당 구조체는 엑스트라 데이터 테이블의 레코드 참조를 가지는데
            /// 해당 테이블 데이터 외에 추가적인 데이터를 보유하는 경우 그 타입을 표시하는 타입
            /// </summary>
            public enum PrefabExtraDataAdditiveType
            {
                None,
                TableRecordOnly,
                MonoComponent,
                UIComponent,
                PrefabModelData,
            }

            #endregion
        
            #region <Constructors>

            public PrefabPoolingManagerPreset(int p_PrefabModelKey, PrefabExtraDataRecordBridge p_PrefabExtraDataRecord)
            {
                _PrefabExtraDataAdditiveType = PrefabExtraDataAdditiveType.PrefabModelData;
                _PrefabExtraDataRecord = p_PrefabExtraDataRecord;
                ExtraComponentType = _PrefabExtraDataRecord.ExtraComponentType;
                Canvas = default;
                PrefabModelKey = (true, p_PrefabModelKey);
                Hash = default;
                Hash = GenerateHash();
            }
        
            public PrefabPoolingManagerPreset(PrefabExtraDataRecordBridge p_PrefabExtraDataRecord)
            {
                _PrefabExtraDataAdditiveType = PrefabExtraDataAdditiveType.TableRecordOnly;
                _PrefabExtraDataRecord = p_PrefabExtraDataRecord;
                ExtraComponentType = _PrefabExtraDataRecord.ExtraComponentType;
                Canvas = default;
                PrefabModelKey = default;
                Hash = default;
                Hash = GenerateHash();
            }
                
            public PrefabPoolingManagerPreset(Type p_Component)
            {
                _PrefabExtraDataAdditiveType = PrefabExtraDataAdditiveType.MonoComponent;
                _PrefabExtraDataRecord = default;
                ExtraComponentType = p_Component;
                Canvas = default;
                PrefabModelKey = default;
                Hash = default;
                Hash = GenerateHash();
            }
                
            public PrefabPoolingManagerPreset((RenderMode pt_RenderMode, Canvas pt_Canvas, Type pt_MonoComponent) pt_UIExtraData)
            {
                _PrefabExtraDataAdditiveType = PrefabExtraDataAdditiveType.UIComponent;
                _PrefabExtraDataRecord = default;
                ExtraComponentType = pt_UIExtraData.pt_MonoComponent;
                Canvas = (true, pt_UIExtraData.pt_RenderMode, pt_UIExtraData.pt_Canvas);
                PrefabModelKey = default;
                Hash = default;
                Hash = GenerateHash();
            }

            #endregion

            #region <Operator>

            public static implicit operator PrefabPoolingManagerPreset(Type p_MonoComponent)
            {
                return new PrefabPoolingManagerPreset(p_MonoComponent);
            }
        
            public static implicit operator PrefabPoolingManagerPreset(int p_ExtraDataIndex)
            {
                var (valid, tableInstance) = PrefabExtraDataRoot.GetInstanceUnSafe.GameDataTableCluster.GetLabeledTable(p_ExtraDataIndex);
                return valid ? new PrefabPoolingManagerPreset(tableInstance.GetTableData(p_ExtraDataIndex)) : null;
            }
        
            public static implicit operator PrefabPoolingManagerPreset((int pt_ModelDataKey, int pt_ExtraDataKey) pt_ExtraDataTuple)
            {
                var (valid, record) = PrefabExtraDataRoot.GetInstanceUnSafe.GameDataTableCluster.GetLabeledTable(pt_ExtraDataTuple.pt_ExtraDataKey);
                return valid ? new PrefabPoolingManagerPreset(pt_ExtraDataTuple.pt_ModelDataKey, record.GetTableData(pt_ExtraDataTuple.pt_ExtraDataKey)) : null;
            }
        
            public static implicit operator PrefabPoolingManagerPreset((RenderMode pt_RenderMode, Canvas pt_Canvas, Type pt_MonoComponent) pt_ExtraDataTuple)
            {
                return new PrefabPoolingManagerPreset(pt_ExtraDataTuple);
            }

            public static bool operator ==(PrefabPoolingManagerPreset p_LeftValue, PrefabPoolingManagerPreset p_RightValue)
            {
                return p_LeftValue.Equals(p_RightValue);
            }

            public static bool operator !=(PrefabPoolingManagerPreset p_LeftValue, PrefabPoolingManagerPreset p_RightValue)
            {
                return !(p_LeftValue == p_RightValue);
            }
             
            public override bool Equals(object p_Right)
            {
                return Equals((PrefabPoolingManagerPreset)p_Right);
            }

            public bool Equals(PrefabPoolingManagerPreset r)
            {
                if (_PrefabExtraDataAdditiveType == r._PrefabExtraDataAdditiveType)
                {
                    switch (_PrefabExtraDataAdditiveType)
                    {
                        case PrefabExtraDataAdditiveType.None:
                            return true;
                        case PrefabExtraDataAdditiveType.TableRecordOnly:
                            return _PrefabExtraDataRecord == r._PrefabExtraDataRecord;
                        case PrefabExtraDataAdditiveType.MonoComponent:
                            return ExtraComponentType == r.ExtraComponentType;
                        case PrefabExtraDataAdditiveType.UIComponent:
                            return Canvas.Item3 == r.Canvas.Item3 && ExtraComponentType == r.ExtraComponentType;
                        case PrefabExtraDataAdditiveType.PrefabModelData:
                            return PrefabModelKey == r.PrefabModelKey && _PrefabExtraDataRecord == r._PrefabExtraDataRecord;
                    }
                }
                return false;
            }
            
            public override int GetHashCode()
            {
                return Hash;
            }

#if UNITY_EDITOR
            public override string ToString()
            {
                switch (_PrefabExtraDataAdditiveType)
                {
                    case PrefabExtraDataAdditiveType.None:
                        return $"None of Extra Data";
                    case PrefabExtraDataAdditiveType.TableRecordOnly:
                        return $"TableRecordOnly / {ExtraComponentType} : {_PrefabExtraDataRecord.ExtraDataDescription}";
                    case PrefabExtraDataAdditiveType.MonoComponent:
                        return $"MonoComponent / {ExtraComponentType}";
                    case PrefabExtraDataAdditiveType.UIComponent:
                        return $"UIComponent / {Canvas.Item2} : {Canvas.Item3} : {ExtraComponentType}";
                    case PrefabExtraDataAdditiveType.PrefabModelData:
                        return $"PrefabModelData / {PrefabModelKey.Item2} : {ExtraComponentType} : {_PrefabExtraDataRecord.ExtraDataDescription}";
                }

                return string.Empty;
            }
#endif
            
            #endregion

            #region <Methods>

            private int GenerateHash()
            {
                switch (_PrefabExtraDataAdditiveType)
                {
                    case PrefabExtraDataAdditiveType.None:
                        return 0;
                    case PrefabExtraDataAdditiveType.TableRecordOnly:
                        return _PrefabExtraDataRecord.GetHashCode();
                    case PrefabExtraDataAdditiveType.MonoComponent:
                        return ExtraComponentType.GetHashCode();
                    case PrefabExtraDataAdditiveType.UIComponent:
                        return Canvas.Item3.GetHashCode() * ExtraComponentType.GetHashCode();
                    case PrefabExtraDataAdditiveType.PrefabModelData:
                        return PrefabModelKey.Item2 + 1;
                }

                return 0;
            }

            #endregion
        }
        #endregion
    }
}