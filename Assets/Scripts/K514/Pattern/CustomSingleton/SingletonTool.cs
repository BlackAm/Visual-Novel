using System;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// ISingleton의 구현체가 Terminal Generic이기 때문에 추상화할 수 없었던 싱글톤 공통 기능을
    /// 제공하는 정적 클래스
    /// </summary>
    public static class SingletonTool
    {
        #region <SingletonTypeCheck>

        /// <summary>
        /// 인터페이스 종류를 기술하는 타입
        /// </summary>
        private enum BaseClassType
        {
            /// <summary>
            /// 전체 싱글톤
            /// </summary>
            ISingleton,
            
            /// <summary>
            /// ISingleton의 기본 구현체
            /// </summary>
            Singleton,
            
            /// <summary>
            /// ISingleton의 비동기 구현체
            /// </summary>
            AsyncSingleton,
            
            /// <summary>
            /// ISingleton의 유니티 컴포넌트 구현체
            /// </summary>
            UnityComponentSingleton,
                        
            /// <summary>
            /// ISingleton의 유니티 컴포넌트 비동기 구현체
            /// </summary>
            UnityComponentAsyncSingleton,
            
            /// <summary>
            /// _Disposable 인터페이스 구현체
            /// </summary>
            Disposable,
            
            /// <summary>
            /// 게임 테이블 데이터 구현체
            /// </summary>
            GameTableData,
                        
            /// <summary>
            /// 인덱싱 게임 테이블 데이터 구현체
            /// </summary>
            IndexableGameTableData,
        }

        /// <summary>
        /// 각 싱글톤 타입의 자료형을 가지는 컬렉션
        /// </summary>
        private static Dictionary<BaseClassType, Type> SingletonTypeCollection 
            = new Dictionary<BaseClassType, Type>
            {
                {BaseClassType.ISingleton, typeof(ISingleton)},
                {BaseClassType.Singleton, typeof(Singleton<>)},
                {BaseClassType.AsyncSingleton, typeof(AsyncSingleton<>)},
                {BaseClassType.UnityComponentSingleton, typeof(UnitySingleton<>)},
                {BaseClassType.UnityComponentAsyncSingleton, typeof(UnityAsyncSingleton<>)},
                {BaseClassType.GameTableData, typeof(ITableBase)},
                {BaseClassType.IndexableGameTableData, typeof(MultiTableProxy<,,,,>)},
                {BaseClassType.Disposable, typeof(_IDisposable)},
            };

        /// <summary>
        /// 지정한 타입이 싱글톤 인터페이스를 구현하는지 검증하는 메서드
        /// </summary>
        public static bool IsISingleton(Type p_Type) => p_Type.IsSubInterfaceOf(SingletonTypeCollection[BaseClassType.ISingleton].Name);

        /// <summary>
        /// 지정한 타입이 Singleton를 상속받는지 검증하는 메서드
        /// </summary>
        public static bool IsSingleton(Type p_Type) => p_Type.IsSubclassOfRawGeneric(SingletonTypeCollection[BaseClassType.Singleton]);

        /// <summary>
        /// 지정한 타입이 Singleton를 상속받는지 검증하는 메서드
        /// </summary>
        public static bool IsAsyncSingleton(Type p_Type) => p_Type.IsSubclassOfRawGeneric(SingletonTypeCollection[BaseClassType.AsyncSingleton]);

        /// <summary>
        /// 지정한 타입이 UnitySingleton를 상속받는지 검증하는 메서드
        /// </summary>
        public static bool IsUnityComponentSingleton(Type p_Type) => p_Type.IsSubclassOfRawGeneric(SingletonTypeCollection[BaseClassType.UnityComponentSingleton]);
        
        /// <summary>
        /// 지정한 타입이 UnitySingleton를 상속받는지 검증하는 메서드
        /// </summary>
        public static bool IsUnityComponentAsyncSingleton(Type p_Type) => p_Type.IsSubclassOfRawGeneric(SingletonTypeCollection[BaseClassType.UnityComponentAsyncSingleton]);

        /// <summary>
        /// 지정한 타입이 ITableData를 상속받는지 검증하는 메서드
        /// </summary>
        public static bool IsTableData(Type p_Type) => p_Type.IsSubInterfaceOf(SingletonTypeCollection[BaseClassType.GameTableData].Name);

        /// <summary>
        /// 지정한 타입이 IndexableRoot를 상속받는지 검증하는 메서드
        /// </summary>
        public static bool IsTableDataRoot(Type p_Type) => p_Type.IsSubclassOfRawGeneric(SingletonTypeCollection[BaseClassType.IndexableGameTableData]);

        /// <summary>
        /// 지정한 타입이 _Disposable를 상속받는지 검증하는 메서드
        /// </summary>
        public static bool IsDisposable(Type p_Type) => p_Type.IsSubInterfaceOf(SingletonTypeCollection[BaseClassType.Disposable].Name);

        #endregion

        #region <SingletonLifeCycle>

        /// <summary>
        /// 싱글톤 생성을 제한하는 플래그
        /// </summary>
        public static bool SingletonRestrictFlag { get; private set; }

        /// <summary>
        /// 현재 활성화된 싱글톤 그룹
        /// 
        /// 각 싱글톤 클래스의 인스턴스 생성 로직에서 각 리스트에 등록이 되며,
        /// 해당 클래스의 ClearActivedSingleton에 의해 일괄 해제된다.
        /// </summary>
        private static readonly List<ISingleton> ActiveSingletonSet = new List<ISingleton>();

        /// <summary>
        /// 싱글톤이 생성된 경우 호출되는 콜백
        /// </summary>
        public static void OnSingletonSpawned(ISingleton p_Spawned)
        {
            ActiveSingletonSet.Add(p_Spawned);
        }
        
        /// <summary>
        /// 싱글톤이 파기된 경우 호출되는 콜백
        /// </summary>
        public static void OnSingletonDisposed(ISingleton p_Disposed)
        {
            ActiveSingletonSet.Remove(p_Disposed);
        }
        
        /// <summary>
        /// 싱글톤이 다른 오브젝트에 의해 수명을 제어받는 경우 호출되는 콜백
        /// </summary>
        public static void OnSingletonAttached(ISingleton p_Disposed)
        {
            ActiveSingletonSet.Remove(p_Disposed);
        }

        /// <summary>
        /// 현재 활성화된 게임 테이블 싱글톤을 일괄 파기시키는 메서드
        /// 싱글톤은 서로 종속관계에 있는 경우가 많으므로, 활성화된 순서의 역순으로 제거한다.
        /// </summary>
        public static void ClearActiveSingleton()
        {
            SingletonRestrictFlag = true;
            var currentSingletonNumber = ActiveSingletonSet.Count;
            for (int i = currentSingletonNumber - 1; i > -1; i--)
            {
                var targetSingleton = ActiveSingletonSet[i];
                try
                {
                    targetSingleton.Dispose();
                }
#if UNITY_EDITOR
                catch(Exception e)
                {
                    Debug.LogError($"[Singleton Release Failed] : {e.Message}\n{e.StackTrace}");
                }
#else
                catch
                {
                    // do nothing
                }
#endif
            }
            SingletonRestrictFlag = false;
        }

        public static bool HasActiveSingleton()
        {
            return ActiveSingletonSet.Count > 0;
        }

#if UNITY_EDITOR
        public static void PrintActiveSingleton()
        {
            var currentSingletonNumber = ActiveSingletonSet.Count;
            if (currentSingletonNumber > 0)
            {
                Debug.LogError($"Singleton Remaind : {currentSingletonNumber}");
                for (int i = currentSingletonNumber - 1; i > -1; i--)
                {
                    var targetSingleton = ActiveSingletonSet[i];
                    Debug.LogError(targetSingleton.GetType().Name);
                }
            }
            else
            {
                Debug.LogWarning($"Singleton Released Successfully");
            }
        }
#endif

        #endregion
        
        #region <CreateSingleton>

        /// <summary>
        /// 싱글톤을 초기화시키는 메서드
        /// </summary>
        public static object CreateSingleton(Type p_Type)
        {
            switch (p_Type)
            {
                case var _ when IsSingleton(p_Type):
                case var _ when IsUnityComponentSingleton(p_Type):
                {
                    return p_Type
                        .GetProperty(
                            "GetInstance",
                            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy
                        ).GetValue(null);
                }
                default :
                    return null;
            }
        }
        
        /// <summary>
        /// 싱글톤을 초기화시키는 메서드
        /// </summary>
        public static async UniTask<object> CreateAsyncSingleton(Type p_Type)
        {
            switch (p_Type)
            {
                case var _ when IsAsyncSingleton(p_Type):
                case var _ when IsUnityComponentAsyncSingleton(p_Type):
                {
                    var method = p_Type
                        .GetMethod(
                            "GetInstanceUnSafeWaiting",
                            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy
                        );
                    var asyncMethod = (UniTask<object>) method.Invoke(null, null);
                    return await asyncMethod;
                }
                default :
                    return CreateSingleton(p_Type);
            }
        }
        
        /// <summary>
        /// 싱글톤을 초기화시키는 메서드
        /// </summary>
        public static void CreateSingletonSet(Type[] p_TypeSet)
        {
            foreach (var type in p_TypeSet)
            {
                CreateSingleton(type);
            }
        }
        
        /// <summary>
        /// 싱글톤을 초기화시키는 메서드
        /// </summary>
        public static async UniTask CreateAsyncSingletonSet(Type[] p_TypeSet)
        {
            foreach (var type in p_TypeSet)
            {
                await CreateAsyncSingleton(type);
            }
        }

        #endregion
    }
}