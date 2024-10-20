#if UNITY_EDITOR
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 디버그 출력을 플래그로 제어하는 정적 클래스
    /// </summary>
    public static partial class CustomDebug
    {
        #region <Extend>

        public static bool PrintItemLog = false;

        #endregion

        #region <GUI Flag>

        /// <summary>
        /// 컨트롤러에 의해 이동되는 방향을 그리는 플래그
        /// </summary>
        public static bool DrawControllerDirectionGreen = false;

        #endregion

        #region <VirtualSpace>

        public static bool DrawUnitFilter = false;
        public static bool DrawUnitFilterRestrict = false;
        public static bool DrawPivot = false;
        public static bool DrawRayPhysicsCheck = false;
        public static bool DrawSpherePhysicsCheck = false;

        #endregion

        #region <Loading>

        /// <summary>
        /// 싱글톤 로딩 여부를 출력하는 플래그
        /// </summary>
        public static bool PrintSingletonLoading = true;

        /// <summary>
        /// 로드 에셋 매니저의 작업 개시를 출력하는 플래그
        /// </summary>
        public static bool PrintLoadAssetManager = false;

        /// <summary>
        /// 씬 로딩 진행 및 페이즈를 출력하는 플래그
        /// </summary>
        public static bool PrintSceneLoadProgress = false;

        /// <summary>
        /// 패치 관련 로그를 출력하는 플래그
        /// </summary>
        public static bool PrintPatchFlag = false;

        #endregion

        #region <Input>

        /// <summary>
        /// 커맨드 입력 상태를 출력하는 플래그
        /// </summary>
        public static bool PrintCommandState = false;

        /// <summary>
        /// 특정 키를 계속 누르고 있는 경우를 출력하는 플래그
        /// </summary>
        public static bool PrintCommandPushingState = false;

        #endregion

        #region <Logic>

        /// <summary>
        /// 오브젝트 풀러 관련 로그를 출력하는 플래그
        /// </summary>
        public static bool PrintObjectPooler = false;

        /// <summary>
        /// 현재 SceneChangeEventSender에 등록된 SceneObserver들의 이벤트를 처리할 때, 그 순서를 출력하는 플래그
        /// </summary>
        public static bool PrintSceneChangeObserverPriority = false;

        /// <summary>
        /// SceneEnvironment에 관한 로그를 출력하는 플래그
        /// </summary>
        public static bool PrintSceneEnvironment = false;

        /// <summary>
        /// Async Pool 내부 로그를 출력하는 플래그
        /// </summary>
        public static bool PrintAsyncPoolTask = false;

        /// <summary>
        /// AssetBundleBuilder에 관한 로그를 출력하는 플래그
        /// </summary>
        public static bool PrintAssetBundleBuilder = false;

        /// <summary>
        /// EventTimer 기반으로 구현된 코루틴의 작업 시작/소멸에 관한 로그를 출력하는 플래그
        /// </summary>
        public static bool PrintEventTimerBasedCoroutine = false;

        /// <summary>
        /// PrefabPoolingManager 내부 로그를 출력하는 플래그
        /// </summary>
        public static bool PrintPrefabPolling = false;

        /// <summary>
        /// 패치 파일 생성에 관련된 로그를 출력하는 플래그
        /// </summary>
        public static bool PrintPatchFileBuild = true;

        /// <summary>
        /// 프리팹 셰이더 관리자 로그를 출력하는 플래그
        /// </summary>
        public static bool PrintShaderStorage = false;

        #endregion

        #region <Methods>

        /// <summary>
        /// 유니티 디버그는 벡터를 2자리까지 밖에 출력하지 않으므로, 다음과 같이 실수 문자열로 변환시켜 출력하면 6자리까지 각 원소 값을 출력할 수있다.
        /// </summary>
        public static string ToVectorString(this Vector3 p_Target) => $"x : {p_Target.x} | y : {p_Target.y} | z : {p_Target.z}";

        #endregion

        #region <Unit>

        /// <summary>
        /// 유닛 컴포넌트의 생성, 풀링, 제거에 관한 로그를 출력하는 플래그
        /// </summary>
        public static bool PrintUnitLifeCycleLog = false;

        /// <summary>
        /// 유닛 상태 변화를 출력하는 플래그
        /// </summary>
        public static bool PrintUnitState = false;

        /// <summary>
        /// 모든 스킬 쿨타임을 없앤다.
        /// </summary>
        public static bool SkillCoolTimeZero = false; // MadeInHeaven -> SkillCoolTimeZero

        /// <summary>
        /// 모든 전투 데미지를 1로 고정시킨다.
        /// </summary>
        public static bool TinyTinySwing = false;
        
        /// <summary>
        /// 플레이어의 데미지가 피격 유닛의 최대 체력의 2할로 고정된다.
        /// </summary>
        public static bool Calling20Percent = false; // CallingBadEnd -> Calling20Percent

        /// <summary>
        /// 플레이어의 데미지가 피격 유닛의 최대 체력의 10할로 고정된다.
        /// </summary>
        public static bool FatalFury = false;
        
        /// <summary>
        /// 플레이어가 입는 피해를 1로 고정시킨다.
        /// </summary>
        public static bool PlayerHittingOneDamage = false; // SecondHome -> PlayerHittingOneDamage
        
        /// <summary>
        /// 타격시 발생하는 힘에 관한 디버그 플래그
        /// </summary>
        public static bool PrintAddForceFlag = false;
        
        /// <summary>
        /// 게임 시스템 관련 로그를 출력하는 플래그
        /// </summary>
        public static bool PrintGameSystemLog = false;
                
        /// <summary>
        /// 게임 피격 시스템 관련 로그를 출력하는 플래그
        /// </summary>
        public static bool PrintUnitHitLog = false;

        #endregion

        #region <Compress>

        /// <summary>
        /// 파일의 압축 및 해제 로그를 출력하는 플래그
        /// </summary>
        public static bool PrintDataIO = false;

        #endregion

        #region <Animation>

        /// <summary>
        /// 애니메이션 모션 전이에 관한 로그를 출력하는 플래그
        /// </summary>
        public static bool AnimationTransition = false;

        /// <summary>
        /// 인공지능 상태 전이에 관한 로그를 출력하는 플래그
        /// </summary>
        public static bool AIStateTransition = false;

        /// <summary>
        /// 유닛 관리 ID 값을 이름에 표시하는 플래그
        /// </summary>
#if SERVER_DRIVE
        public static bool AIStateName = true;
#else
        public static bool AIStateName = false;
#endif

        #endregion

        #region <Table>

        public static bool PrintDecode = false;
        public static bool PrintDecodeFail = false;
        public static bool PrintDecodeError = true;

        #endregion

        #region <ServerDrive>

#if SERVER_DRIVE
        public static bool VisualizeServerNode = true;
#endif

        #endregion

        #region <EventTimer>

        /// <summary>
        /// 파일의 압축 및 해제 로그를 출력하는 플래그
        /// </summary>
        public static bool PrintEventTimerHandlerProgressFlag = false;

        #endregion

        #region <UI>
        
        /// UI 관련 작업 플래그.
        public static bool PrintUIObjectFlag = false;

        #endregion
    }
}
#endif