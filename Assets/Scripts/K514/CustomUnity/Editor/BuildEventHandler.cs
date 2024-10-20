#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.Rendering;
using UnityEditor;
namespace BlackAm
{
    /// <summary>
    /// 유니티 에디터로부터 콜백을 받아 프로젝트 빌드를 수행할 때, 커스텀 시스템에 의해 정의된 빌드 조건을 만족하지 못한 경우
    /// 유니티 에디터 상으로 빌드에 문제가 없더라도 예외를 발생시켜 빌드를 막는 기능을 수행하는 클래스
    /// </summary>
    public class BuildEventHandler : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }
        public async void OnPreprocessBuild(BuildReport report)
        {
            SystemMaintenance.InitSystemMaintenance();
            SystemMaintenance.InitPlayerSetting();
            SetBuildGraphicsSetting(true, true);
            await SystemFlag.GetInstance();
            if (!SystemFlag.IsSystemReleaseMode())
            {
                throw new BuildFailedException("유니티 에디터 탭의 CustomWindow/SystemFlagSelector의 배포모드가 True일 때만 빌드가 가능합니다.");
            }
            SystemMaintenance.ReleaseSystem();
        }

        /// <summary>
        /// lightMap, Fog 관련 셋팅을 초기화한다
        /// </summary>
        public void SetBuildGraphicsSetting(bool lightMapMode, bool FogMode)
        {
            var graphicsSettingsObj = AssetDatabase.LoadAssetAtPath<GraphicsSettings>("ProjectSettings/GraphicsSettings.asset");
            var serializedObject = new SerializedObject(graphicsSettingsObj);
            var LightMapStripping = serializedObject.FindProperty("m_LightmapStripping");
            var FOGStripping = serializedObject.FindProperty("m_FogStripping");

            LightMapStripping.boolValue = lightMapMode;
            FOGStripping.boolValue = FogMode;

            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }
    }
}
#endif