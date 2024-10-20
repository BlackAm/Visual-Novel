using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BlackAm
{
    /// <summary>
    /// 템플릿 placeHolder 제약을 우회해서 각 타입별 UnityObjectPooler<T> 인스턴스를 추상화하기 위한 인터페이스
    /// </summary>
    public interface IUnityObjectPooler
    {
        /// <summary>
        /// 인스턴스화할 프리팹 프리셋
        /// </summary>
        AssetPreset _AssetPreset { get; }

        /// <summary>
        /// 메인 컴포넌트 타입
        /// </summary>
        Type _MainComponentType { get; }
        
        /// <summary>
        /// 프리팹에 붙일 스크립트 컴포넌트 타입
        /// </summary>
        List<Type> _ComponentTypeList { get; }
        
        /// <summary>
        /// 인스턴스화할 프리팹
        /// </summary>
        GameObject _TargetPrefab { get; }

        /// <summary>
        /// 해당 풀러의 프리팹을 구분하는 유니크 키
        /// </summary>
        PrefabPoolingTool.PrefabIdentifyKey GetPrefabKey();
        
        /// <summary>
        /// 해당 풀러의 프리팹의 모델 레코드를 가져오는 메서드
        /// </summary>
        PrefabModelDataRecordBridge GetModelTableRecord();
    }

    /// <summary>
    /// 유니티 컴포넌트를 풀링하기 위한 IObjectPooler 구현체
    /// </summary>
    public class UnityObjectPooler<M> : ObjectPoolBase<M>, IUnityObjectPooler where M : PoolingUnityObject<M>
    {
        #region <Fields>

        /// <summary>
        /// 인스턴스화할 프리팹 프리셋
        /// </summary>
        public AssetPreset _AssetPreset { get; private set; }

        /// <summary>
        /// 메인 컴포넌트 타입
        /// </summary>
        public Type _MainComponentType { get; protected set; }
        
        /// <summary>
        /// 프리팹에 붙일 스크립트 컴포넌트 타입
        /// </summary>
        public List<Type> _ComponentTypeList { get; protected set; }
        
        /// <summary>
        /// 인스턴스화할 프리팹
        /// </summary>
        public GameObject _TargetPrefab { get; private set; }
        
        /// <summary>
        /// 해당 풀러의 프리팹을 구분하기 위한 식별자
        /// </summary>
        private PrefabPoolingTool.PrefabIdentifyKey _PrefabKey;

        #endregion
        
        #region <Constructors>

        public UnityObjectPooler()
        {
            _MainComponentType = typeof(M);
            _ComponentTypeList = new List<Type>();
        }
        
        public UnityObjectPooler(AssetPreset p_AssetPreset, PrefabPoolingTool.PrefabPoolingManagerPreset p_PrefabExtraDataPreset) : this()
        {
            SetPoolerPreset(p_AssetPreset, p_PrefabExtraDataPreset);
        }

        #endregion

        #region <Callbacks>

        /// <summary>
        /// 신규 생성된 오브젝트 초기화 메서드
        /// </summary>
        protected override void OnSpawningObject(M p_TargetObject)
        {
#if UNITY_EDITOR
            p_TargetObject.name = $"Pooled : {_AssetPreset.AssetName}_[{_MainComponentType.Name}]";
#endif
            foreach (var additiveComponent in _ComponentTypeList)
            {
                p_TargetObject.gameObject.AddComponent(additiveComponent);
            }
            
            base.OnSpawningObject(p_TargetObject);
        }
        
        /// <summary>
        /// 풀에서 활성화된 오브젝트 초기화 메서드
        /// </summary>
        protected override void OnActivatePooledObject(M p_TargetObject)
        {
            p_TargetObject.gameObject.SetActiveSafe(true);
            base.OnActivatePooledObject(p_TargetObject);
        }
        
        /// <summary>
        /// 오브젝트 풀링으로부터 혹은 오브젝트를 신규 생성하여 리턴하는 메서드
        /// </summary>
        protected override void OnRetrieveObject(M p_TargetObject)
        {
            base.OnRetrieveObject(p_TargetObject);
            p_TargetObject.gameObject.SetActiveSafe(false);
        }

        #endregion
        
        #region <Methods/PrefabExtraData>

        public PrefabPoolingTool.PrefabIdentifyKey GetPrefabKey()
        {
            return _PrefabKey;
        }

        public PrefabModelDataRecordBridge GetModelTableRecord()
        {
            var modelKeyTuple = GetPrefabKey()._PrefabExtraPreset.PrefabModelKey;
            return modelKeyTuple.Item1 ? PrefabModelDataRoot.GetInstanceUnSafe[modelKeyTuple.Item2] : null;
        }

        /// <summary>
        /// 프리팹 모델 레코드로부터 스케일 배율을 가져오는 메서드
        /// </summary>
        private float GetModelTableRecordScale()
        {
            var modelRecord = GetModelTableRecord();
            return modelRecord?.PrefabModelScale ?? 1f;
        }
        
        /// <summary>
        /// 지정한 타입을 해당 풀러의 오브젝트 추가 컴포넌트 타입에 더하는 메서드
        /// </summary>
        private void AddPrefabExtraType(Type p_Type)
        {
            if (p_Type != null)
            {
                // 현재 오브젝트 풀러의 플레이스 홀더와 지정한 컴포넌트를 비교한다.
                var thisPlaceHolder = _MainComponentType;
                        
                // 같은 타입이거나, 플레이스 홀더 쪽이 자손 클래스 였던 경우
                if (p_Type == thisPlaceHolder || thisPlaceHolder.IsSubclassOf(p_Type))
                {
                    // ignore
                }
                // 플레이스 홀더 쪽이 부모 클래스 였던 경우
                else if(p_Type.IsSubclassOf(thisPlaceHolder))
                {
                    // 지정한 타입이 해당 풀러의 플레이스 홀더가 된다.
                    _MainComponentType = p_Type;
                }
                // 플레이스 어떤 관계도 없는 유니티 컴포넌트 였던 경우
                else if(p_Type.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    // 추가 컴포넌트 리스트에 추가시켜준다.
                    if (!_ComponentTypeList.Contains(p_Type))
                    {
                        _ComponentTypeList.Add(p_Type);
                    }
                }
                // 지정한 타입이 유니티 컴포넌트가 아니었던 경우
                else
                {
                    // ignore
                }
            }
        }

        /// <summary>
        /// 유니티 컴포넌트의 경우에는 일반 인스턴스와는 달리 다른 컴포넌트와 함께 초기화되어 풀링될 수 있기 때문에.
        /// 그러한 추가 컴포넌트 등을 포함한 몇 개의 설정 값을 프리팹 추가 데이터(PrefabExtraData)라고 하여, 전용 테이블로부터
        /// 해당 풀러에 데이터를 가져와야 한다.
        /// </summary>
        public void SetPoolerPreset(AssetPreset p_AssetPreset, PrefabPoolingTool.PrefabIdentifyKey p_Key)
        {
            _PrefabKey = p_Key;
            _PrefabKey.SetPooler(this);
            
            _AssetPreset = p_AssetPreset;
            _TargetPrefab = p_AssetPreset.Asset as GameObject;
            PrefabEventSender.GetInstance.OnPrefabKeyConfirmed(_PrefabKey);
            
            var targetComponent = p_Key._PrefabExtraPreset.ExtraComponentType;
            if (!ReferenceEquals(null, targetComponent))
            {
                AddPrefabExtraType(targetComponent);
            }
        }

        /// <summary>
        /// 추가 데이터가 None타입인 프리팹 추가 데이터 호출
        /// </summary>
        public void SetPoolerPreset(AssetPreset p_AssetPreset, PrefabPoolingTool.PrefabPoolingManagerPreset p_PrefabExtraDataPreset = default)
        {
            SetPoolerPreset(p_AssetPreset, new PrefabPoolingTool.PrefabIdentifyKey(p_AssetPreset.AssetName, p_AssetPreset.ResourceLifeCycleType, p_PrefabExtraDataPreset.ExtraComponentType));
        }

        #endregion
        
        #region <Methods>

        /// <summary>
        /// 오브젝트 풀링으로부터 혹은 오브젝트를 신규 생성하여 리턴하는 내부 메서드
        /// </summary>
        public override (bool, M) GenerateObject() => GenerateObject(new TransformTool.AffineCachePreset(Vector3.zero));
        
        /// <summary>
        /// 오브젝트 풀링으로부터 혹은 오브젝트를 신규 생성하여 리턴하는 내부 메서드
        /// </summary>
        public (bool, M) GenerateObject(TransformTool.AffineCachePreset p_Affine)
        {
            M result;
            if (BreakedObjectPool.Count > 0)
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintObjectPooler)
                {
                    Debug.Log($"Pooled Object Recycled");
                }
#endif
                var lastIndex = BreakedObjectPool.Count - 1;
                result = BreakedObjectPool[lastIndex];
                BreakedObjectPool.RemoveAt(lastIndex);
#if UNITY_EDITOR
                if (result.IsDisposed)
                {
                    Debug.LogError($"파기된 Disposable의 재활성화가 감지되었습니다. 오브젝트 : [{typeof(M)}]");
                }
#endif
                p_Affine.MulScaleFactor(GetModelTableRecordScale());
                result.ApplyAffinePreset(p_Affine);
                return (false, result);
            }
            else
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintObjectPooler)
                {
                    Debug.Log($"new Object Spawned");
                }
#endif
                result = SpawnObject(p_Affine);
                return (true, result);
            }
        }
        
        /// <summary>
        /// 오브젝트 풀링으로부터 혹은 오브젝트를 신규 생성하여 리턴하는 메서드
        /// </summary>
        public override M GetObject() => GetObject(Vector3.zero);

        /// <summary>
        /// 오브젝트 풀링으로부터 혹은 오브젝트를 신규 생성하여 리턴하는 메서드
        /// </summary>
        public M GetObject(Vector3 p_SpawnPos)
        {
            return InitObject(GenerateObject(p_SpawnPos));
        }
        
        /// <summary>
        /// 오브젝트 풀링으로부터 혹은 오브젝트를 신규 생성하여 리턴하는 메서드
        /// </summary>
        public M GetObject(TransformTool.AffineCachePreset p_Affine)
        {
            return InitObject(GenerateObject(p_Affine));
        }
        
        /// <summary>
        /// 오브젝트를 신규 생성시키는 메서드
        /// </summary>
        public override M SpawnObject() => SpawnObject(new TransformTool.AffineCachePreset(Vector3.zero));

        /// <summary>
        /// 오브젝트를 신규 생성시키는 메서드
        /// </summary>
        public M SpawnObject(TransformTool.AffineCachePreset p_Affine)
        {
            GameObject targetModel = null;
            if (ReferenceEquals(null, _TargetPrefab))
            {
                targetModel = new GameObject();
                targetModel.transform.SetPositionAndRotation(p_Affine.Position, p_Affine.Rotation);
            }
            else
            {
                targetModel = Object.Instantiate(_TargetPrefab, p_Affine.Position, p_Affine.Rotation);
            }

            // 안드로이드 플랫폼으로 설정된 유니티 에디터에서는 에셋번들을 통해 랜더러를 로드하면
            // 에디터랑 안드로이드용 에셋번들이 호환이 되지 않는건지, 셰이더가 동작하지 않기 때문에
            // 셰이더를 리로드해준다.
#if UNITY_ANDROID// && UNITY_EDITOR
            // 즉시 머티리얼을 초기화하면, 머티리얼 정보를 가져오지 못하는 경우가 있어서 일정 간격을 두고 머티리얼
            // 초기화를 시킨다.
            if (_AssetPreset.ResourceType.GetResourceLoadType() == AssetLoadType.FromAssetBundle)
            {
                //targetModel.ReloadMaterial();
                
                SystemBoot.SystemEventTimer.RunTimer
                (
                    null,
                    100,
                    (handler) =>
                    {
                        var _targetModel = handler.Arg1;
                        _targetModel.ReloadMaterial();
                        return true;
                    }, 
                    null, targetModel
                );
            }
#endif
            
            // 이미 프리팹이 컴포넌트를 보유하고 있는 경우를 체크해준다.
            M result = targetModel.GetComponent(_MainComponentType) as M ?? targetModel.AddComponent(_MainComponentType) as M;
            result.SetPoolingContainer(this);
            result.InitializeAffine(p_Affine.ScaleFactor * GetModelTableRecordScale());
                
            return result;
        }

        /// <summary>
        /// 오브젝트 풀을 파기시키는 메서드
        /// 파기 이전에 활성화된 오브젝트들을 회수시켜서
        /// 회수 콜백을 먼저 수행한다.
        /// </summary>
        public override void ClearPool()
        {
#if UNITY_EDITOR
            if (CustomDebug.PrintObjectPooler)
            {
                Debug.Log($"clear pool");
            }
#endif
            
            RetrieveAllObject();
            ActivedObjectPool.Clear();
            PreLoadObjectGroup.Clear();

            var breakedPoolScale = BreakedObjectPool.Count;
            for (int i = breakedPoolScale - 1; i > -1; i--)
            {
                var breakedObject = BreakedObjectPool[i];
                breakedObject.Dispose();
            }
            BreakedObjectPool.Clear();

            LoadAssetManager.GetInstanceUnSafe.UnloadAsset(_AssetPreset);
            PrefabEventSender.GetInstance?.OnPrefabKeyExpired(_PrefabKey);
            _AssetPreset = new AssetPreset();
            _MainComponentType = typeof(M);
            _ComponentTypeList.Clear();
        }

        #endregion
    }

    /// <summary>
    /// 유니티 컴포넌트를 풀링하기 위한 IPoolingObject 구현체
    /// 본래라면 유니티 컴포넌트 클래스 Monobehaviour와 상위 클래스 격인 PoolingObject<M>을 상속받아서
    /// 계층구조에 맞게 구현해야 하지만
    /// 
    /// c#에서는 이중 클래스 상속을 할 수 없기 때문에 PoolingObject<M> 클래스와는 서로 기능 및 중복되는 코드를
    /// 가지는 형태로 구현되어 있다.
    /// </summary>
    public abstract class PoolingUnityObject<M> : MonoBehaviour, IPoolingObject<M> where M : PoolingUnityObject<M>
    {
        #region <Finalizer>

        protected void OnDestroy()
        {
            Dispose();
        }

        #endregion

        #region <IPoolingObject>

        public IObjectPooler<M> PoolingContainer { get; set; }
        public PoolState PoolState { get; set; }
        
        public abstract void OnSpawning();

        public abstract void OnPooling();

        public abstract void OnRetrieved();

        public void RetrieveObject()
        {
            if (PoolState != PoolState.None)
            {
                SetCompareKey(null);
                PoolingContainer.RetrieveObject(this as M);
            }
        }

        #endregion
        
        #region <SafeReference>

        public object CompareKey { get; private set; }

        public void SetCompareKey(object p_CompareKey)
        {
            CompareKey = p_CompareKey;
        }

        #endregion

        #region <Objects>

        /// <summary>
        /// 해당 인스턴스의 프리팹을 풀링하는 오브젝트 풀의 키값
        /// </summary>
        protected PrefabPoolingTool.PrefabIdentifyKey _PrefabKey;

        /// <summary>
        /// 해당 오브젝트가 오브젝트 풀러 이외의 방법으로 생성된 경우, 생성 콜백을 수동 호출해주는 메서드
        /// </summary>
        public void CheckAwake()
        {
            if (PoolState == PoolState.None)
            {
                InitializeAffine(transform.localScale.x);
                OnSpawning();
                OnPooling();
            }
        }

        public void SetPoolingContainer(IObjectPooler<M> p_PoolingContainer)
        {
            PoolingContainer = p_PoolingContainer;
            if (PoolingContainer is IUnityObjectPooler c_IUnityObjectPooler)
            {
                _PrefabKey = c_IUnityObjectPooler.GetPrefabKey();
            }
        }
        
        public PrefabPoolingTool.PrefabIdentifyKey GetPrefabKey()
        {
            return _PrefabKey;
        }
        
        #endregion
        
        #region <Affine>

        /// <summary>
        /// 아핀 레퍼런스 객체 캐싱
        /// </summary>
        [NonSerialized] public Transform _Transform;
        
        /// <summary>
        /// 오브잭트 스케일 프리셋
        /// </summary>
        public FloatProperty_Inverse_Sqr_Sqrt ObjectScale { get; protected set; }

        public virtual void ApplyAffinePreset(TransformTool.AffineCachePreset p_Affine)
        {
            _Transform.SetPositionAndRotation(p_Affine.Position, p_Affine.Rotation);
            ObjectScale = new FloatProperty_Inverse_Sqr_Sqrt(p_Affine.ScaleFactor, 0f);
            SetScale(1f);
        }
        
        public virtual void InitializeAffine(float p_Scale)
        {
            _Transform = transform;

            var affine = new TransformTool.AffineCachePreset(_Transform);
            affine.SetScaleFactor(p_Scale);
            ApplyAffinePreset(affine);
        }
        
        public virtual void SetScale(float p_ScaleFactor)
        {
            ObjectScale = ObjectScale.ApplyScale(p_ScaleFactor);
            _Transform.localScale = ObjectScale.CurrentValue * Vector3.one;
        }

        #endregion

        #region <Disposable>

        /// <summary>
        /// dispose 패턴 onceFlag
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// dispose 플래그를 초기화 시키는 메서드
        /// </summary>
        public void Rejunvenate()
        {
            IsDisposed = false;
        }
        
        /// <summary>
        /// 인스턴스 파기 메서드
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            else
            {
                IsDisposed = true;
                DisposeUnManaged();
            }
        }

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected virtual void DisposeUnManaged()
        {
            switch (PoolState)
            {
                case PoolState.None :
                case PoolState.Actived :
                    RetrieveObject();
                    DisposeContainer();
                    break;
                case PoolState.Pooled :
                case PoolState.Retrieving :
                    DisposeContainer();
                    break;
                case PoolState.Disposed :
                    break;
            }
        }

        public virtual void DisposeContainer()
        {
            if (PoolState != PoolState.None)
            {
                PoolingContainer.BreakedObjectPool.Remove(this as M);
            }

            PoolState = PoolState.Disposed;
            Destroy(gameObject);
        }
        
        #endregion
    }
}