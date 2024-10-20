using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// State-Constant한 ObjectDeployEventPreset 클래스의 상태를 추적하기 위한 종속 클래스
    /// </summary>
    public partial class ObjectDeployEventRecord : PoolingObject<ObjectDeployEventRecord>, IUniTaskDelayPredicate
    {
        #region <Consts>

        private const int MaxConcurrentEventSize = 16;

        #endregion
        
        #region <Fields>

        /// <summary>
        /// 배치 기준이 되는 아핀 프리셋
        /// </summary>
        public TransformTool.AffineCachePreset _ZeroPivotAffine;

        /// <summary>
        /// 수동으로 아핀 프리셋이 지정되었는지 표시하는 플래그
        /// </summary>
        private bool _ManualZeroPivotSetted;
        
        /// <summary>
        /// 연산된 배치 프리셋 리스트
        /// </summary>
        public List<ObjectDeployEventExtraPreset> _DeployPresetStorage;

        /// <summary>
        /// 하나의 타임 스탬프에 다수의 이벤트가 처리되는데, 각 이벤트의 진행 상태를 기술하는 배열
        /// </summary>
        private DeployEventState[] DeployStateStorage;

        /// <summary>
        /// 음원 미디어 트래커
        /// </summary>
        private MediaTool.MediaPreset<AudioClip> _AudioClipMediaTracker;
        
        /* 현재 진행중인 이벤트 정보를 레코드의 ObjectDeployEventPreset로부터 복사해온 필드 목록 */
        private Dictionary<int, ObjectDeployEventProgressControl> _TimeControlUnitCollection;
        private Dictionary<int, List<(int t_DeployDataIndex, Dictionary<ObjectDeployTool.DeployableType, List<int>> t_EventCollection)>> _EventDeployCollection;
        private int _ConcurrentTracingEventCount;
        public ObjectDeployParams _ObjectDeployParams;
        public bool _DiverBackFlag;
        public bool _BlockBusterFlag;
        public bool _UsingPivotFlag;
        public bool _UsingSharingHitFlag;
        public bool _UpdateRecursiveStartPositionFlag;
        public bool _BlockEventWheneNoTargetFlag;
        public bool _BlockFallBackCurrentFocus;
        
        #endregion

        #region <Enums>

        public enum DeploySequenceState
        {
            Valid,
            ReserveLastDeployEvent,
            Blocked
        }

        #endregion

        #region <Callbacks>
        
        public override void OnSpawning()
        {
            _DeployPresetStorage = new List<ObjectDeployEventExtraPreset>();
            DeployStateStorage = 
                Enumerable.Range(0, MaxConcurrentEventSize)
                    .Select(index => DeployEventState.GetDefaultEventState()).ToArray();
        }

        public override void OnPooling()
        {
            _ZeroPivotAffine = TransformTool.AffineCachePreset.GetDefaultAffineCachePreset();
            _ManualZeroPivotSetted = false;

            for (int i = 0; i < MaxConcurrentEventSize; i++)
            {
                DeployStateStorage[i] = default;
            }

            _TimeControlUnitCollection = default;
            _EventDeployCollection = default;
            _ConcurrentTracingEventCount = default;
            _ObjectDeployParams = default;
            
            _DiverBackFlag = default;
            _BlockBusterFlag = default;
            _UsingPivotFlag = default;
            _UsingSharingHitFlag = default;
            _UpdateRecursiveStartPositionFlag = default;
            _BlockEventWheneNoTargetFlag = default;
            _BlockFallBackCurrentFocus = default;
        }

        public override void OnRetrieved()
        {
            foreach (var deployEventExtraPreset in _DeployPresetStorage)
            {
                deployEventExtraPreset.OnRetrievePreset();
            }
            _DeployPresetStorage.Clear();
            
        }

        #endregion
        
        #region <Methods>

        public bool CheckDelayActionValid()
        {
            return PoolState == PoolState.Actived;
        }
        
        #endregion

        #region <Structs>

        /// <summary>
        /// 현재 배치 이벤트의 상태를 기술하는 프리셋
        /// </summary>
        public struct DeployEventState
        {
            #region <Consts>

            public static DeployEventState GetDefaultEventState()
            {
                var result = new DeployEventState
                { 
                    HeightOffset = default, 
                    DeploySequenceState = default,
                };

                return result;
            }

            #endregion
            
            #region <Fields>

            /// <summary>
            /// 배치 이벤트의 높이 변화 값
            /// </summary>
            public float HeightOffset;
            
            /// <summary>
            /// 배치 이벤트의 유효상태
            /// </summary>
            public DeploySequenceState DeploySequenceState;

            #endregion

            #region <Methods>

            /// <summary>
            /// 배치 이벤트의 좌표가 연산됬을 때, 해당 좌표의 높이차이를 비교하는 메서드
            /// 차이가 더 커졌다면 더 저점으로 배치한다는 의미이므로 참을 리턴한다.
            /// </summary>
            public bool CheckDeployTraceUp(float p_NextHeightOffset)
            {
                var result = HeightOffset <= p_NextHeightOffset;
                if (result)
                {
                    HeightOffset = p_NextHeightOffset;
                }
                
                return result;
            }

            #endregion
        }

        #endregion
    }
}