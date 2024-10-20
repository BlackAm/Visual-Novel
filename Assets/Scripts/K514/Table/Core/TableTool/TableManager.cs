using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace BlackAm
{
    /// <summary>
    /// 씬 단위로 수명을 가지는 게임 데이터 테이블 싱글톤들을 제어하는 싱글톤 클래스
    /// 기본적으로 싱글톤들은 게임오브젝트나 프리팹과 달리 생명주기를 제어해주는 객체가 없으므로
    /// 해당 싱글톤 클래스에서 제어해준다.
    /// </summary>
    public class TableManager : SceneChangeEventSingleton<TableManager>
    {
        #region <Fields>

        /// <summary>
        /// 씬 단위 수명을 가지는 현재 활성화된 싱글톤 그룹
        /// </summary>
        private List<ITableBase> _SceneGameDataSingletonGroups;
        
        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
            _SceneGameDataSingletonGroups = new List<ITableBase>();
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
        }

        public override void OnSceneTransition()
        {
        }
        
        /// <summary>
        /// 특정 게임 테이블 데이터 클래스가 파기된 경우 호출되는 콜백
        /// </summary>
        public void OnDisposeScene_LifeCycleType_GameDataTable(ITableBase p_GameDataTable)
        {
            _SceneGameDataSingletonGroups.Remove(p_GameDataTable);
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 씬 단위 테이블 데이터 싱글톤을 등록하는 메서드
        /// </summary>
        public void Subscribe_LifeCycleType_GameDataTable(ITableBase p_GameDataTable)
        {
            if (!_SceneGameDataSingletonGroups.Contains(p_GameDataTable))
            {
                _SceneGameDataSingletonGroups.Add(p_GameDataTable);
            }
        }

        /// <summary>
        /// 현재 활성화 중인 씬 단위 테이블 데이터를 릴리스하는 메서드
        /// </summary>
        public async UniTask Clear_SceneLifeCycle_TableSingleton()
        {
            await UniTask.SwitchToThreadPool();
            
            var singletonCount = _SceneGameDataSingletonGroups.Count;
            for (int i = singletonCount - 1; i > -1; i--)
            {
                var targetSingleton = _SceneGameDataSingletonGroups[i];
                targetSingleton.Dispose();
            }
        }
        
        #endregion
    }
}