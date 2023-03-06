namespace k514
{
    public partial class ThinkableBase
    {
        #region <Methods>

        public abstract bool HasAIExtraFlag(ThinkableTool.AIExtraFlag p_Type);
        public abstract void SetAIFlagMask(ThinkableTool.AIExtraFlag p_FlagMask);
        public abstract void SetNeverSleep(bool p_Flag);
        public abstract void SetWander(bool p_Flag);
        public abstract void SetCheckEncounter(bool p_Flag);
        public abstract void SetCounter(bool p_Flag);
        public abstract void SetAggressive(bool p_Flag);
        public abstract void SetJunkYardDog(bool p_Flag);
        public abstract void SetRemoteOrder(bool p_Flag);

        #endregion
    }
}