namespace BlackAm
{
    public partial class VFXProjectileBase
    {
        /*#region <Callbacks>

        protected virtual void OnPlayUpdateEvent()
        {
            GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(ref _TimerEventHandler, this, SystemBoot.TimerType.GameTimer, true);
            var (_, eventHandler) = _TimerEventHandler.GetValue();
            eventHandler.AddEvent((0, 0, EventTimerTool.EventTimerIntervalType.UpdateEveryFrame), handler => handler.Arg1.OnUpdateParticle(handler.LatestDeltaTime), null, this);
            eventHandler.StartEvent();
        }

        protected bool OnUpdate(float p_DeltaTime)
        {
            _WholeProgressTimer.Progress(p_DeltaTime * _SimulateSpeed.CurrentValue);

            var isLifeSpanOver = _WholeProgressTimer.IsOver();
            if (isLifeSpanOver || UpdateParticleSet())
            {
                if (!IsReservedDeadOrDead())
                {
                    if (isLifeSpanOver)
                    {
                        OnLifeSpanOver();
                    }
                    SetRemove(false, 0);
                }
                
                return false;
            }
            else
            {
                if (!IsReservedDeadOrDead())
                {
                    if (_HasValidParticle && IsTracing)
                    {
                        OnUpdateTraceTargetUnitVector(_SpawnedParticleSet[0]);
                    }
                }
                
                return true;
            }
        }

        protected abstract bool OnUpdateParticle(float p_DeltaTime);

        #endregion*/
    }
}