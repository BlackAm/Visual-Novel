using System;
using Cysharp.Threading.Tasks;

namespace BlackAm
{
    /// <summary>
    /// 프리팹 추가데이터 테이블 클래스의 추상 클래스
    /// </summary>
    public abstract class PrefabExtraDataTable<M, K, T> : MultiTableBase<M, K, T, PrefabExtraDataRoot.PrefabExtraDataType, PrefabExtraDataRecordBridge> 
        where M : PrefabExtraDataTable<M, K, T>, new() 
        where T : PrefabExtraDataTable<M, K, T>.PrefabExtraDataRecord, new()
    {
        /// <summary>
        /// 프리팹 추가데이터 테이블 레코드 클래스의 추상 클래스
        /// </summary>
        public abstract class PrefabExtraDataRecord : GameTableRecordBase, PrefabExtraDataRecordBridge
        {
            public Type ExtraComponentType { get; protected set; }
            public string ExtraDataDescription { get; protected set; }

            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();
                
                if (ExtraComponentType == null)
                {
                    var tryLabelType = GetInstanceUnSafe.GetThisLabelType();
                    
                    switch (tryLabelType)
                    {
                        case PrefabExtraDataRoot.PrefabExtraDataType.AutoMutton_Zero:
                            break;
                        case PrefabExtraDataRoot.PrefabExtraDataType.AutoMutton_NonZero:
                            break;
                        case PrefabExtraDataRoot.PrefabExtraDataType.AutoMutton_Linear:
                            break;
                        case PrefabExtraDataRoot.PrefabExtraDataType.AutoMutton_Square:
                            break;
                        case PrefabExtraDataRoot.PrefabExtraDataType.ProjectorSimple:
                            break;
                        case PrefabExtraDataRoot.PrefabExtraDataType.ProjectorAnimation:
                            break;
                        case PrefabExtraDataRoot.PrefabExtraDataType.ProjectorPetPet:
                            break;
                        case PrefabExtraDataRoot.PrefabExtraDataType.VFX:
                            ExtraComponentType = typeof(VFXUnit);
                            break;
                        case PrefabExtraDataRoot.PrefabExtraDataType.ProjectileVFX:
                            ExtraComponentType = typeof(VFXProjectile_Default);
                            break;
                        case PrefabExtraDataRoot.PrefabExtraDataType.ProjectileVFX_VectorMap:
                            ExtraComponentType = typeof(VFXProjectile_VectorMap);
                            break;
                        case PrefabExtraDataRoot.PrefabExtraDataType.ProjectileVFX_Kinematic:
                            ExtraComponentType = typeof(VFXProjectile_Kinematic);
                            break;
                        case PrefabExtraDataRoot.PrefabExtraDataType.ProjectileVFX_TimeInterpolate:
                            ExtraComponentType = typeof(VFXProjectile_TimeInterpolate);
                            break;
                        case PrefabExtraDataRoot.PrefabExtraDataType.UI:
                            break;
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 프리팹 추가데이터 테이블 클래스의 정수 특정 추상 클래스
    /// </summary>
    public abstract class PrefabExtraDataIntTable<M, T> : PrefabExtraDataTable<M, int, T> where M : PrefabExtraDataIntTable<M, T>, new() where T : PrefabExtraDataTable<M, int, T>.PrefabExtraDataRecord, new()
    {
        public override MultiTableIndexer<int, PrefabExtraDataRoot.PrefabExtraDataType, PrefabExtraDataRecordBridge> GetMultiGameIndex()
        {
            return PrefabExtraDataRoot.GetInstanceUnSafe.GameDataTableCluster;
        }
    }
}