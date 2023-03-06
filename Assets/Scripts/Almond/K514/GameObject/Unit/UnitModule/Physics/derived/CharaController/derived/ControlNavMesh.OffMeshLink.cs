using UnityEngine;

namespace k514
{
    public partial class ControlNavMesh
    {
        #region <Consts>

        /// <summary>
        /// Off-Mesh Link 착지 허용 러프 비율 값
        /// </summary>
        private const float _Off_MeshLink_TerminateLerpRate = 0.95f;

        #endregion
        
        #region <Fields>
        
        /// <summary>
        /// 현재 네브메쉬 에이전트가 Off-Mesh Link를 건너는 중인지 표시하는 플래그
        /// </summary>
        public bool _InOnNavMesh_Off_MeshLink;
        
        /// <summary>
        /// 네브메쉬 Off-Mesh Link 경로 이동 수행 시 시작 및 끝 위치
        /// </summary>
        protected Vector3 _OffMeshLink_StartPosition, _OffMeshLink_EndPosition;

        /// <summary>
        /// 네브메쉬 Off-Mesh Link 경로 이동 수행 중 적용될 속도배율
        /// </summary>
        protected float _OffMeshLink_SpeedRate;
        
        /// <summary>
        /// Off-Mesh Link를 통한 NavMeshAgent 점프 연출 시에 참조할 점프 높이
        /// </summary>
        protected float _JumpHeight;
        
        /// <summary>
        /// 현재 적용중인 Off-Mesh Link 착지 허용 러프 비율 값
        /// </summary>
        private float _Current_Off_MeshLink_TerminateLerpRate = 0.95f;

        /// <summary>
        /// Off-Mesh Link 경로 이동 로직 진행 페이즈
        /// </summary>
        protected OfflinkProcessPhase _OfflinkProcessPhase;

        /// <summary>
        /// Off-Mesh Link 점프 연출 높이 연산자
        /// </summary>
        protected CustomMath.EdgedLinearEquation _OffMeshLink_JumpOrbitEquation;
        
        #endregion

        #region <Enums>
        
        protected enum OfflinkProcessPhase
        {
            /// <summary>
            /// Offlink 상태가 아님
            /// </summary>
            ReadyToOfflink,
            
            /// <summary>
            /// Offlink 진행 중
            /// </summary>
            OfflinkProgressing,
            
            /// <summary>
            /// 한번 시행된 Offlink가 종료된 상태
            /// </summary>
            OfflinkTerminate,
        }

        #endregion
        
        #region <Callbacks>

        private void OnAwakeOffMeshLink()
        {
        }
        
        private void OnPoolingOffMeshLink()
        {
            _OfflinkProcessPhase = OfflinkProcessPhase.ReadyToOfflink;
            SetOffMeshLinkTransition(false);
            On_Off_Mesh_LinkOver();
        }
        
        /// <summary>
        /// Off-Mesh Link 경로 이동 로직 페이즈를 갱신시키는 콜백
        /// 만약 페이즈가 종료된 상태라면 true를 리턴한다.
        /// </summary>
        public override bool OnUpdatePhysicsAutonomyJump(float p_DeltaTime)
        {
            switch (_OfflinkProcessPhase)
            {
                case OfflinkProcessPhase.OfflinkProgressing:
                    if (IsNavmeshAgentEnable)
                    {
                        // 지정한 두 높이 사이에 현재 위치한 높이 비를 역선형결합 값으로 가져온다.
                        var inverseProgressRate = Mathf.Min(1f, Mathf.InverseLerp(_OffMeshLink_StartPosition.y, _OffMeshLink_EndPosition.y, GetNavMeshAgentPosition().y));
                        if (inverseProgressRate > _Current_Off_MeshLink_TerminateLerpRate)
                        {
                            // 일정 비율에 도달한 경우, OffMeshLink를 종료시킨다.
                            On_Off_Mesh_LinkOver();
                        }
                        else
                        {
                            // OffMeshLink가 일정이상 진행된 경우
                            if (inverseProgressRate > _OffMeshLink_JumpOrbitEquation._InverseRatePoint)
                            {
                                // 높이가 변하지 않는다는 것은 더 이상 OffMeshLink를 진행할 수 없는 상태를 의미하므로
                                // 종료 비율을 보정해준다.
                                if (!_MasterNode.IsHeightChanged())
                                {
                                    _Current_Off_MeshLink_TerminateLerpRate -= p_DeltaTime;
                                }
                                
                                // 낙하 속도는 점점 빨라져야 한다.
                                _OffMeshLink_SpeedRate += p_DeltaTime;
                            }
                            
                            SetPhysicsAutonomySpeed(_OffMeshLink_SpeedRate);
                            //SetNavMeshBaseLocalYOffset(_JumpHeight * _OffMeshLink_JumpOrbitEquation.GetCodomain(inverseProgressRate));
                        }
                    }
                    else
                    {
                        // 캐릭터 컨트롤러 물리 모듈의 영향을 받게 된다.
                    }

                    return false;
                default :
                    return true;
            }
        }
        
        /// <summary>
        /// Off-Mesh Link 경로 이동이 시작된 경우 호출되는 콜백
        /// </summary>
        public void On_Off_Mesh_LinkBegin()
        {
            // 플래그를 세트한다.
            _InOnNavMesh_Off_MeshLink = true;

            // 착지 허용 비율을 초기화 시켜준다.
            _Current_Off_MeshLink_TerminateLerpRate = _Off_MeshLink_TerminateLerpRate;
            
            // 인공지능 루프는 일정 주기로 동작하기 때문에, 항상 Off-Mesh Link 경로 이동을 허용하는게 아니라
            // 인공지능 루프에서 Off-Mesh Link를 감지했을 때에만 플래그를 활성화 시킨다.
            SetOffMeshLinkTransition(true);
            
            // Off-Mesh Link 경로 이동 중에는 중력을 무시하나, 이동중 타격받은 경우에는 경로 이동을 중지하고 중력의 영향을 받아 낙하한다.
            SetGravityFlag(PhysicsTool.GravityType.Anti_HitBreak);
            
            // 캐릭터 컨트롤러의 바닥 충돌 판정을 제거해준다.
            TryUpdateCollision(Vector3.up);
        }
        
        /// <summary>
        /// Off-Mesh Link 경로 이동이 종료된 경우 호출되는 콜백
        /// </summary>
        public void On_Off_Mesh_LinkOver()
        {            
            // 페이즈를 갱신한다.
            _OfflinkProcessPhase = _NavMeshAgent.currentOffMeshLinkData.valid ? OfflinkProcessPhase.OfflinkTerminate : OfflinkProcessPhase.ReadyToOfflink;

            // 플래그를 리셋한다.
            _InOnNavMesh_Off_MeshLink = false;
            
            // 중력 적용 타입을 초기화한다.
            SetGravityFlag(PhysicsTool.GravityType.Applied);
            
            // 네브메쉬 에이전트의 y축 변위를 0으로 한다.
            SetNavMeshBaseLocalYOffset(0f);
            
            // 착지 로직이므로, 등록된 속도에서 y성분을 모두 제거해준다.
            Clear_Y_Velocity();
            
            // 사고 상태에 따른 속도 배율을 적용시켜준다.
            SetPhysicsAutonomySpeed(_MasterNode._MindObject.GetAISpeedRate());
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// OffMeshLink 사용 여부를 세트하는 메서드
        /// </summary>
        public void SetOffMeshLinkTransition(bool p_Flag)
        {
            _NavMeshAgent.autoTraverseOffMeshLink = p_Flag;
        }

        #endregion
    }
}