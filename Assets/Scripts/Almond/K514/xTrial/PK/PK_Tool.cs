namespace k514.xTrial
{
    public static class PK_Tool
    {
        #region <Consts>

        /// <summary>
        /// PK 시스템 적용가능한 레벨 하한
        /// </summary>
        public const int PK_LEVEL_LOWERBOUND = 40;
        
        /// <summary>
        /// 죄업 상태 지속시간 카운트 단위
        /// </summary>
        public const uint PK_Guilty_Mode_Duration_Unit = 1000;

        /// <summary>
        /// 다른 플레이어 공격시 죄업 상태 지속시간 카운트 갱신 값
        /// </summary>
        public const int PK_Guilty_Mode_Duration_HitOther_StackCount = 20;
        
        /// <summary>
        /// 다른 플레이어 PK모드 종료시 죄업 상태 지속시간 카운트 갱신 값
        /// </summary>
        public const int PK_Guilty_Mode_Duration_OffPKMode_StackCount = 10;
        
        /// <summary>
        /// 죄업 상태 지속시간 갱신시간
        /// </summary>
        public const float PK_Guilty_Mode_Additive_Delay = 10f;
        
        /// <summary>
        /// 선성 상태에 도달하기 위한 성향 게이지 전이점
        /// </summary>
        public const int PK_VirtueGauge_Transition_Bound = 500;
        
        /// <summary>
        /// 성향 게이지 상한값
        /// </summary>
        public const int PK_VirtueGauge_UpperBound = 1000;
                
        /// <summary>
        /// 성향 게이지 하한값
        /// </summary>
        public const int PK_VirtueGauge_LowerBound = -1000;
        
        #endregion

        #region <Enums>

        public enum PK_State
        {
            /// <summary>
            /// 중립, 기본 상태
            /// </summary>
            Neutral,
            
            /// <summary>
            /// 선성 상태
            /// 
            /// Vice 상태의 유저나 몬스터를 다수 처치하는 경우에 전이하는 상태
            /// 게임 진행에 메리트 부여
            /// </summary>
            Virtue,
            
            /// <summary>
            /// 죄업 상태
            /// 
            /// PK기능을 이용해 유저를 공격한 경우에 전이하는 상태
            /// 일정 시간이 지나면 기본 상태로 되돌아감
            /// </summary>
            Guilty,
            
            /// <summary>
            /// 악성 상태
            /// 
            /// PK기능을 이용해 유저를 처치한 경우에 전이하는 상태
            /// 게임 진행에 패널티를 받음
            /// </summary>
            Vice,
        }

        #endregion
    }
}