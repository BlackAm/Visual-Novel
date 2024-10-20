namespace BlackAm
{
    public class HomeSceneManager : AbstractSceneManagerSingleton<HomeSceneManager>
    {
        #region <Fields>

        private int _homeSceneIndex;

        #endregion

        protected override void OnCreated()
        {
            // _homeSceneIndex = GlobalData.dataMap
        }

        public override void OnInitiate()
        {
        }

        public override void SwitchScene()
        {
        }
    }
}