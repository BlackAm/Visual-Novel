using UnityEngine;
using BDG;

namespace k514
{
    public class LamiereComputerBossDerived : LamiereUnit
    {
        #region <Callbacks>

        public override void OnPooling()
        {
            base.OnPooling();
            AddState(UnitStateType.SUPERARMOR);
        }

        #endregion
    }
}