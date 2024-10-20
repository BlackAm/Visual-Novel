using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public class CubicBezierManager : SceneChangeEventSingleton<CubicBezierManager>
    {
        #region <Fields>

        private ObjectPooler<CubicBezierPreset> BezierPresetPool;

        #endregion

        #region <Callbacks>

        protected override void OnCreated()
        {
            BezierPresetPool = new ObjectPooler<CubicBezierPreset>();
            BezierPresetPool.PreloadPool(8, 8);
        }

        public override void OnInitiate()
        {
        }

        public override async UniTask OnScenePreload()
        {
            await UniTask.CompletedTask;
        }

        public override void OnSceneStarted()
        {
        }

        public override void OnSceneTerminated()
        {
            BezierPresetPool.RetrieveAllObject();
        }

        public override void OnSceneTransition()
        {
        }

        #endregion

        #region <Methods>

        public CubicBezierPreset GetBezierPreset()
        {
            return BezierPresetPool.GetObject();
        }

        #endregion
    }

    /// <summary>
    /// 3차 베지어 곡선을 기술하기 위한 데이터 셋
    /// </summary>
    public class CubicBezierPreset : PoolingObject<CubicBezierPreset>
    {
        #region <Fields>

        public Vector3[] basePivotGroup { get; private set; }
        public Vector3[] calculatePivotGroup { get; private set; }

        #endregion

        #region <PoolingObject>

        public override void OnSpawning()
        {
            basePivotGroup = new Vector3[4];
            calculatePivotGroup = new Vector3[4];
        }

        public override void OnPooling()
        {
        }

        public override void OnRetrieved()
        {
        }

        #endregion

        #region <Methods>

        public void SyncPivotSet()
        {
            calculatePivotGroup[0] = basePivotGroup[0];
            calculatePivotGroup[1] = basePivotGroup[1];
            calculatePivotGroup[2] = basePivotGroup[2];
            calculatePivotGroup[3] = basePivotGroup[3];   
        }

        public void SetFirstPivot(Vector3 p_Pivot0)
        {
            basePivotGroup[0] = p_Pivot0;
        }
        
        public void SetPivot(Vector3 p_Pivot0, Vector3 p_Pivot1, Vector3 p_Pivot2, Vector3 p_Pivot3)
        {
            basePivotGroup[0] = p_Pivot0;
            basePivotGroup[1] = p_Pivot1;
            basePivotGroup[2] = p_Pivot2;
            basePivotGroup[3] = p_Pivot3;
        }

        public Vector3 GetBezierPosition(float p_ProgressRate)
        {
            return this.BezierCurveInterpolation(p_ProgressRate);
        }

        #endregion
    }
}