using Cysharp.Threading.Tasks;

namespace k514
{
    /// <summary>
    /// 기본 유닛이 가져야할 데이터를 포함하는 추상 테이블 클래스
    /// </summary>
    public abstract class PrefabExtraData_VfxBase<M, T> : PrefabExtraDataIntTable<M, T>, PrefabExtraDataTableBridge where M : PrefabExtraData_VfxBase<M, T>, new() where T : PrefabExtraData_VfxBase<M, T>.VfxTableRecordBase, new()
    {
        public abstract class VfxTableRecordBase : PrefabExtraDataRecord, VfxExtraDataRecordBridge
        {
            public bool FixedRotationFlag { get; protected set; }
            public float SimulateSpeedFactor{ get; protected set; }

            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();
                
                if (SimulateSpeedFactor < CustomMath.Epsilon)
                {
                    SimulateSpeedFactor = 1f;
                }
            }
        }
    }
}