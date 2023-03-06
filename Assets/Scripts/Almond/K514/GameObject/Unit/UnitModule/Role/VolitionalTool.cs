namespace k514
{
    public interface IVolitionalTableBridge : ITableBase
    {
    }    
    
    public interface IVolitionalTableRecordBridge : ITableBaseRecord
    {
        int KEY { get; }
    }

    public interface IActorTableBridge : IVolitionalTableBridge
    {
    }    
    
    public interface IActorTableRecordBridge : IVolitionalTableRecordBridge
    {
        VolitionalTool.ActorModuleProgressFlag ActorModuleProgressFlag { get; }
        VolitionalTool.ActorTimeOverEventType ActorTimeOverEventType { get; }
        float LifeSpan { get; }
    }

    public static class VolitionalTool
    {
        #region <Enums>

        public enum ActorModuleProgressFlag
        {
            None = 0,
            AutoRunAct = 1 << 0,
        }

        public enum ActorTimeOverEventType
        {
            /// <summary>
            /// 수명 종료시, 유닛을 파기한다.
            /// </summary>
            DeadEnd,
            
            /// <summary>
            /// 수명 종료 후에도 유닛은 현상을 유지한다.
            /// </summary>
            ShowMustGoOn,
            
            /// <summary>
            /// 수명 종료 후 유닛의 역할 모듈을 기본값으로 되돌린다.
            /// </summary>
            ResetModule,
        }

        #endregion
    }

    public class UnitRoleDataRoot : UnitModuleDataRootBase<UnitRoleDataRoot, UnitRoleDataRoot.UnitRoleType, IVolitionalTableBridge, IVolitionalTableRecordBridge>
    {
        #region <Enums>

        public enum UnitRoleType
        {
            /// <summary>
            /// 일반몹
            /// </summary>
            None,
            
            /// <summary>
            /// 챔피언
            /// </summary>
            Champion,
            
            /// <summary>
            /// 네임드
            /// </summary>
            Hero,
            
            /// <summary>
            /// 보스
            /// </summary>
            Boss,
            
            /// <summary>
            /// NPC
            /// </summary>         
            NPC,
            
            /// <summary>
            /// 종속유닛
            /// </summary>    
            Slave,
            
            /// <summary>
            /// 액션을 하나 실행한 뒤, 파기되는 Actor
            /// </summary>
            OneShotActor,
            
            /// <summary>
            /// 지속시간 동안 특정 유닛에 종속됬다가 파기되는 Actor
            /// </summary>
            SlaveActor,
        }

        #endregion
        
        #region <Methods>

        protected override UnitModuleDataTool.UnitModuleType GetUnitModuleType()
        {
            return UnitModuleDataTool.UnitModuleType.Role;
        }

        public override (UnitRoleType, IIncarnateUnit) SpawnModule(Unit p_Target, int p_TableIndex)
        {
            var (valid, labelType) = GetLabelType(p_TableIndex);
            if (valid)
            {
                var record = this[p_TableIndex];
                switch (labelType)
                {
                    case UnitRoleType.Champion:
                        return (labelType, new VolitionalChamp().OnInitializeRole(labelType, p_Target, record));
                    case UnitRoleType.Hero:
                        return (labelType, new VolitionalHero().OnInitializeRole(labelType, p_Target, record));
                    case UnitRoleType.Boss:
                        return (labelType, new VolitionalBoss().OnInitializeRole(labelType, p_Target, record));
                    case UnitRoleType.Slave:
                        return (labelType, new VolitionalSlave().OnInitializeRole(labelType, p_Target, record));
                    case UnitRoleType.OneShotActor:
                        return (labelType, new VolitionalActor_OneShot().OnInitializeRole(labelType, p_Target, record));
                    case UnitRoleType.SlaveActor:
                        return (labelType, new VolitionalActor_Slave().OnInitializeRole(labelType, p_Target, record));
                }
            }

            return default;
        }

        #endregion
    }
}