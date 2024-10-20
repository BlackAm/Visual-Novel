using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public partial class SceneController<M, K, T>
    {
        #region <Fields>

        /// <summary>
        /// 페이즈별 비동기 작업 시퀀스 테이블
        /// </summary>
        protected Dictionary<K, T> _AsyncTaskTable;

        /// <summary>
        /// 페이즈별 가중치 테이블
        /// </summary>
        protected Dictionary<K, float> _PhaseWeightTable;

        /// <summary>
        /// 페이즈별 가중치 누적 테이블
        /// 이때 누적값은 해당 페이즈가 시작되는 시점의 값으로, 해당 페이즈의 가중치는 포함하지 않은 값이다.
        /// </summary>
        private Dictionary<K, float> _PhaseAccumulatedWeightTable;
        
        /// <summary>
        /// 현재 페이즈
        /// </summary>
        protected K _CurrentPhase;
        
        /// <summary>
        /// 전체 진행률
        /// </summary>
        private float _WholeAccumulatedProgressRate;

        #endregion

        #region <Callbacks>

        protected virtual void OnCreatePhase()
        {
            _AsyncTaskTable = new Dictionary<K, T>();
            _PhaseAccumulatedWeightTable = new Dictionary<K, float>();
            
            var enumerator = SystemTool.GetEnumEnumerator<K>(SystemTool.GetEnumeratorType.ExceptNone);
            var wholeWeight = 0f;
            foreach (var progressPhase in enumerator)
            {
                if (_PhaseWeightTable.ContainsKey(progressPhase))
                {
                    _PhaseAccumulatedWeightTable[progressPhase] = wholeWeight;
                    wholeWeight += _PhaseWeightTable[progressPhase];
                }
            }

            var invWeight = 1f / wholeWeight;
            foreach (var progressPhase in enumerator)
            {
                if (_PhaseWeightTable.ContainsKey(progressPhase))
                {
                    _PhaseWeightTable[progressPhase] *= invWeight;
                    _PhaseAccumulatedWeightTable[progressPhase] *= invWeight;
                }
            }
        }

        #endregion

        #region <Methods>

        protected virtual void SwitchPhase(K p_Type)
        {
            _CurrentPhase = p_Type;

            if (_PhaseAccumulatedWeightTable.TryGetValue(_CurrentPhase, out var o_AccWeight))
            {
                _WholeAccumulatedProgressRate = o_AccWeight;
            }
        }

        #endregion
    }
}