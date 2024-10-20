using UnityEngine;
using UnityEngine.Video;

#if !SERVER_DRIVE

namespace BlackAm
{
    public partial class SceneController<M, K, T>
    {
        #region <Consts>

        protected const string _ForegroundImageName = "FG";
        protected const string _BackgroundImageName = "BG";

        #endregion

        #region <Fields>

        [SerializeField] private VideoPlayer _VideoPlayer;

        #endregion

        #region <Callbacks>

        protected virtual void OnCreateAnimation()
        {
        }

        protected abstract void OnUpdateAnimation(float p_DeltaTime);

        #endregion

        #region <Methods>
        
        public void SetPlayVideo()
        {
            try
            {
                _VideoPlayer.Play();
            }
            catch
            {
                //
            }
        }

        #endregion
    }
}
#endif
