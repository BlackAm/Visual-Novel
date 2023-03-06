using System;

#if UNITY_EDITOR

namespace k514
{
    /// <summary>
    /// 열거형 상수를 이용한 플래그 마스크를 추상화할 수 없어서 만든 템플레이트 클래스
    /// 다음과 같은 이슈가 있음
    ///
    ///     1. 상수를 사용하지 않고 열거형 상수를 사용하는 이유
    ///         >> 가독성 면에서 테이블 작업하기 용이하기 때문
    ///
    ///     2. 추상화할 수 없는 이유
    ///         >> 현재 c#에는 enum generic constraint가 추가되었지만, 플래그 연산은 추가되있지 않음
    ///         Convert. 계열 메서드를 통해서 열거형상수를 정수로 변환하여 연산하는 방법이 있지만
    ///         느릴 수 있음.
    /// </summary>
    public class EnumFlagTemplate
    {
        public FlagTemplateEnum FlagTemplateEnumMask;
        
        [Flags]
        public enum FlagTemplateEnum
        {
            None = 0,
            TestA = 1 << 0,
            TestB = 1 << 1
        }
        
        public void AddFlag(FlagTemplateEnum p_TryMask)
        {
            FlagTemplateEnumMask |= p_TryMask;
        }

        public void RemoveFlag(FlagTemplateEnum p_TryMask)
        {
            FlagTemplateEnumMask &= ~p_TryMask;
        }

        public void ClearFlag()
        {
            TurnFlag(FlagTemplateEnum.None);
        }

        public void TurnFlag(FlagTemplateEnum p_TryMask)
        {
            FlagTemplateEnumMask = p_TryMask;
        }
        
        public bool HasFlag_Or(FlagTemplateEnum p_CompareMask)
        {
            return (FlagTemplateEnumMask & p_CompareMask) != FlagTemplateEnum.None;
        }
                
        public FlagTemplateEnum AddFlag(FlagTemplateEnum p_TargetMask, FlagTemplateEnum p_TryMask)
        {
            return p_TargetMask | p_TryMask;
        }

        public FlagTemplateEnum RemoveFlag(FlagTemplateEnum p_TargetMask, FlagTemplateEnum p_TryMask)
        {
            return FlagTemplateEnumMask & ~p_TryMask;
        }
        
        public bool HasFlag_Or(FlagTemplateEnum p_TargetMask, FlagTemplateEnum p_CompareMask)
        {
            return (p_TargetMask & p_CompareMask) != FlagTemplateEnum.None;
        }
    }
}

#endif