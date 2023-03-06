namespace k514
{
    public abstract class AbstractSceneManagerSingleton<M> : Singleton<M> where M : AbstractSceneManagerSingleton<M>, new()
    {
        public abstract void SwitchScene();
    }
}