using System.Collections.Generic;

namespace BlackAm
{
    public interface IObjectPooler<T> : _IDisposable where T : IPoolingObject<T>
    {
        /// <summary>
        /// 활성화된 인스턴스 리스트
        /// </summary>
        List<T> ActivedObjectPool { get; }

        /// <summary>
        /// 재활용될 인스턴스 리스트
        /// </summary>
        List<T> BreakedObjectPool { get; }
        
        /// <summary>
        /// 프리로딩한 오브젝트 리스트
        /// </summary>
        List<T> PreLoadObjectGroup { get; }

        /// <summary>
        /// 오브젝트 풀링으로부터 혹은 오브젝트를 신규 생성하여 리턴하도록 구현할 것
        /// </summary>
        /// <returns>(신규 생성시 true, 풀링 혹은 신규 생성된 인스턴스)</returns>
        (bool, T) GenerateObject();

        /// <summary>
        /// GenerateObject 구현체를 통해 생성된 오브젝트를 초기화 시키도록 구현할 것
        /// </summary>
        T InitObject((bool t_IsNewSpawned, T t_SpawnedObject) p_Tuple);
        
        /// <summary>
        /// GenerateObject 구현체를 통해 생성된 오브젝트를 초기화 시키도록 구현할 것
        /// </summary>
        T InitObject(bool p_IsNewSpawned, T p_SpawnedObject);
        
        /// <summary>
        /// GenerateObject 및 InitObject를 거친 오브젝트를 리턴하도록 구현할 것
        /// </summary>
        /// <returns>풀링 혹은 신규 생성된 인스턴스</returns>
        T GetObject();

        /// <summary>
        /// 풀링할 오브젝트가 없는 경우, 생성자 등을 통해 오브젝트를 신규 생성하여 리턴하도록 구현할 것.
        /// </summary>
        /// <returns>신규 생성된 인스턴스</returns>
        T SpawnObject();

        /// <summary>
        /// 지정한 식별자를 가진 활성화된 오브젝트를 비활성화 시킨다.
        /// </summary>
        void RetrieveObject(T p_Target);

        /// <summary>
        /// 현재 활성화 상태인 모든 오브젝트를 회수하도록 구현할 것.
        /// </summary>
        void RetrieveAllObject();

        /// <summary>
        /// 지정한 갯수만큼의 오브젝트를 프리로딩하도록 구현할 것.
        /// 풀의 오브젝트가 2번째 파라미터보다 많아지지 않을 만큼 프리로드 시킨다.
        /// </summary>
        List<T> PreloadPool(int p_Number, int p_CheckNumber);

        /// <summary>
        /// 해당 오브젝트 풀 및 오브젝트 풀에 담긴 풀링 오브젝트를 파기하여, 프리팹을 등록하면 재사용할 수 있는 상태로 만들도록 구현할 것
        /// </summary>
        void ClearPool();

        /// <summary>
        /// 현재 풀에서 제어하는 오브젝트의 갯수를 리턴하도록 구현할 것
        /// </summary>
        int GetCurrentPoolObjectCount();
        
        /// <summary>
        /// 지정한 오브젝트의 인덱스를 리턴한다.
        /// </summary>
        int GetIndex(T p_Object);
    }

    /// <summary>
    /// 풀링 오브젝트 상태를 기술하는 열거형 상수
    /// </summary>
    public enum PoolState
    {
        None,
        
        /// <summary>
        /// 생성되어 아직 활성화되기 이전의 상태
        /// </summary>
        Spawned,
        
        /// <summary>
        /// 활성화 상태
        /// </summary>
        Actived, 
        
        /// <summary>
        /// 풀링된 상태
        /// </summary>
        Pooled,
        
        /// <summary>
        /// 풀링 회수중
        /// </summary>
        Retrieving,
        
        /// <summary>
        /// 파기된 상태
        /// </summary>
        Disposed,
    }

    public interface IPoolingObject : _IDisposable, ISafeReference<object>
    {
        /// <summary>
        /// 현재 풀링 오브젝트 상태
        /// </summary>
        PoolState PoolState { get; set; }

        /// <summary>
        /// 최초 생성 된 경우, 초기화를 위해 호출되도록 콜백을 구현할 것.
        /// </summary>
        void OnSpawning();
        
        /// <summary>
        /// 풀링된 상태에서 활성화 된 경우, 초기화를 위해 호출되도록 콜백을 구현할 것.
        /// </summary>
        void OnPooling();

        /// <summary>
        /// 활성화 상태에서, 풀링된 상태로 돌아가야하는 경우 호출되도록 콜백을 구현할 것.
        /// </summary>
        void OnRetrieved();

        /// <summary>
        /// 활성화 상태인 해당 오브젝트를 IObjectPooler의 재활용 리스트에 회수되도록 기능을 구현할 것.
        /// </summary>
        void RetrieveObject();
    }

    public interface IPoolingObject<T> : IPoolingObject where T : IPoolingObject<T>
    {
        /// <summary>
        /// 해당 풀링 오브젝트를 관리하고 있는 IObjectPooler의 리스트
        /// </summary>
        IObjectPooler<T> PoolingContainer { get; set; }
    }
}