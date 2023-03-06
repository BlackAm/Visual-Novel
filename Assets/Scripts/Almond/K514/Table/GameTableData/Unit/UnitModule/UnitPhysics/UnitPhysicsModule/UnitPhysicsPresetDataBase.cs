using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public abstract class UnitPhysicsPresetDataBase<M, T> : MultiTableBase<M, int, T, UnitPhysicsDataRoot.UnitPhysicsType, IPhysicsTableRecordBridge>, IPhysicsTableBridge
        where M : UnitPhysicsPresetDataBase<M, T>, new()
        where T : UnitPhysicsPresetDataBase<M, T>.PhysicsTableBaseRecord, new()
    {
        public abstract class PhysicsTableBaseRecord : GameTableRecordBase, IPhysicsTableRecordBridge
        {
            public float Mass { get; private set; }

            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();
                
                Mass = Mass.IsReachedValue(0f) ? 1f : Mathf.Max(PhysicsTool.UnitMassLowerBound, Mass);
            }
        }
        
        public override MultiTableIndexer<int, UnitPhysicsDataRoot.UnitPhysicsType, IPhysicsTableRecordBridge> GetMultiGameIndex()
        {
            return UnitPhysicsDataRoot.GetInstanceUnSafe.GameDataTableCluster;
        }
        
        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}