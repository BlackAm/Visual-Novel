#if !SERVER_DRIVE
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 스캔이펙트를 제어 하는 클래스
    /// </summary>
    public partial class CameraManager
    {
        #region <Fields>

        private PetPetProjector _Scanner;

        #endregion
        
        #region <Callbacks>

        private void OnCreateScanPartial()
        {

        }

        private void OnResetScanner()
        {
            if (_Scanner.IsValid())
            {
                _Scanner.RetrieveObject();
                _Scanner = null;
            }
        }

        #endregion
        
        #region <Methods>
        
        /// <summary>
        /// 스캔이펙트 셋팅
        /// </summary>
        public bool SetScanner(Vector3 position, float scanDistance = 200)
        {
            if (_Scanner.IsValid())
            {
                return false;
            }
            else
            {
#if !SERVER_DRIVE
                _Scanner = ProjectorSpawnManager.GetInstance.Project<PetPetProjector>(4, position).Item2;
                _Scanner.SetProjectionStart();
#endif
                return true;
            }
        }
        
        #endregion
    }
}
#endif