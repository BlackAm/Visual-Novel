using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 특정한 프리팹을 PrefabPooling 매니저로부터 생성/프리로드하는 기능을 가지는 테이블 클래스
    /// </summary>
    public abstract class PrefabSpawnDataBase<M, K> : GameTable<M, int, K> where M : PrefabSpawnDataBase<M, K>, new() where K : PrefabSpawnDataBase<M, K>.SpawnTableRecordBase, new()
    {
        #region <Class/Record>

        public abstract class SpawnTableRecordBase : GameTableRecordBase
        {
            /// <summary>
            /// 생성할 프리팹 인덱스
            /// </summary>
            public int ModelIndex { get; protected set; }
            
            /// <summary>
            /// 생성한 프리팹에 추가할 데이터 인덱스
            /// </summary>
            public int ExtraDataIndex { get; protected set; }
        }

        #endregion

        #region <Methods>
        
        /// <summary>
        /// 지정한 키 값의 테이블 레코드를 참조하여, 프리팹 풀링 매니저로부터 프리로드를 수행함
        /// </summary>
        public (PrefabPoolingTool.PrefabIdentifyKey, List<PrefabInstance>) PreloadPrefab(int p_SpawnedKey, ResourceLifeCycleType p_LifeType, int p_LoadCount)
        {
            if (p_SpawnedKey == default) return default;
            
            var tableRecord = GetTableData(p_SpawnedKey);
            var modelKey = tableRecord.ModelIndex;
            var extraDataKey = tableRecord.ExtraDataIndex;
            var modelRecord = PrefabModelDataRoot.GetInstanceUnSafe[modelKey];
            var prefabName = modelRecord.GetPrefabName();

            return PrefabPoolingManager.GetInstance.PreloadInstance(prefabName, p_LifeType, ResourceType.GameObjectPrefab, p_LoadCount, (modelKey, extraDataKey));
        }
        
        /// <summary>
        /// 지정한 키 값의 테이블 레코드를 참조하여, 프리팹 풀링 매니저로부터 오브젝트를 풀링하여 리턴함
        /// </summary>
        public (bool, T) SpawnPrefab<T>(int p_SpawnedKey, TransformTool.AffineCachePreset p_AffineCachePreset, ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask, ResourceLifeCycleType p_LifeType) where T : Object, IDeployee
        {
            if (p_SpawnedKey == default) return default;
            var (isValid, spawnAffine) = ObjectDeployTool.CorrectAffinePreset(p_AffineCachePreset, p_DeployFlagMask);

            if (isValid)
            {
                var tableRecord = GetTableData(p_SpawnedKey);
                var modelKey = tableRecord.ModelIndex;
                var extraDataKey = tableRecord.ExtraDataIndex;
                var modelRecord = PrefabModelDataRoot.GetInstanceUnSafe[modelKey];
                var prefabName = modelRecord.GetPrefabName();
                var spawned = PrefabPoolingManager.GetInstance.PoolInstance<T>(prefabName,
                    p_LifeType, ResourceType.GameObjectPrefab, spawnAffine, (modelKey, extraDataKey)).Item1;

                return (!ReferenceEquals(null, spawned), spawned);
            }
            else
            {
                return default;
            }
        }
        
        /// <summary>
        /// 지정한 키 값의 테이블 레코드를 참조하여, 프리팹 풀링 매니저로부터 오브젝트를 풀링하여 리턴함.
        /// 프리팹을 생성하는 로직을 비동기로 수행함
        /// </summary>
        public async UniTask<(bool, T)> SpawnPrefabAsync<T>(int p_SpawnedKey, TransformTool.AffineCachePreset p_AffineCachePreset, ObjectDeployTool.ObjectDeploySurfaceDeployType p_DeployFlagMask, ResourceLifeCycleType p_LifeType) where T : Object, IDeployee
        {
            if (p_SpawnedKey == default) return default;
            var (isValid, spawnAffine) = ObjectDeployTool.CorrectAffinePreset(p_AffineCachePreset, p_DeployFlagMask);

            if (isValid)
            {
                var tableRecord = GetTableData(p_SpawnedKey);
                var modelKey = tableRecord.ModelIndex;
                var extraDataKey = tableRecord.ExtraDataIndex;
                var modelRecord = PrefabModelDataRoot.GetInstanceUnSafe[modelKey];
                var prefabName = modelRecord.GetPrefabName();
                var spawned = (await PrefabPoolingManager.GetInstance.PoolInstanceAsync<T>(prefabName,
                    p_LifeType, ResourceType.GameObjectPrefab, spawnAffine, (modelKey, extraDataKey))).Item1;

                return (!ReferenceEquals(null, spawned), spawned);
            }
            else
            {
                return default;
            }
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }

        #endregion
    }
}