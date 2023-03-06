using System.Collections.Generic;
using System.Linq;

namespace k514
{
    public interface IUnitModuleCluster : _IDisposable
    {
        IIncarnateUnit CurrentSelectedModule { get; }

        void SetModuleHandleIndex(int p_Index);
    }

    public class UnitModuleCluster<ModuleLabelType, Module> : IUnitModuleCluster where ModuleLabelType : struct where Module : class, IIncarnateUnit 
    {
        // 유니티 컴포넌트의 경우에는 소멸자를 쓰지 말고, OnDestory를 통해 소멸자 메서드를 호출할 것
        ~UnitModuleCluster()
        {
            Dispose();
        }

        #region <Fields>

        /// <summary>
        /// 마스터 노드
        /// </summary>
        private Unit _MasterNode;

        /// <summary>
        /// 해당 유닛에 등록된 모듈
        /// </summary>
        public IIncarnateUnit CurrentSelectedModule { get; private set; }

        /// <summary>
        /// 해당 유닛에 포함된 액션 그룹
        /// </summary>
        private Dictionary<ModuleLabelType, Dictionary<int, Module>> _ModuleTable;

        /// <summary>
        /// 현재 참조 중인 액션 모듈 인덱스
        /// </summary>
        private int _DefaultKey, _CurrentKey;

        /// <summary>
        /// 현재 참조 중인 모듈 테이블
        /// </summary>
        private UnitModuleDataTool.UnitModuleType _UnitModuleType;

        /// <summary>
        /// 유닛 모듈 리스트에 등록되어 있는 해당 모듈의 고유 인덱스
        /// </summary>
        private int HandleIndex = -1;
        
        #endregion

        #region <Constructor>

        public UnitModuleCluster(Unit p_Unit, UnitModuleDataTool.UnitModuleType p_UnitModuleType, List<int> p_LoadModuleIndexList)
        {
            _MasterNode = p_Unit;
            _UnitModuleType = p_UnitModuleType;
            _ModuleTable = new Dictionary<ModuleLabelType, Dictionary<int, Module>>();
            
            if (!ReferenceEquals(null, p_LoadModuleIndexList))
            {
                foreach (var moduleIndex in p_LoadModuleIndexList)
                {
                    AddModule(moduleIndex, true);
                }
            }
        }

        #endregion

        #region <Callbacks>

        private void OnModuleSwitched(Module p_Transition)
        {
            if (CurrentSelectedModule != p_Transition)
            {
                CurrentSelectedModule?.OnMasterNodeRetrieved();
                CurrentSelectedModule = p_Transition;
            }
            
            if (ReferenceEquals(null, CurrentSelectedModule))
            {
                _MasterNode.RemoveModule(HandleIndex);
            }
            else
            {
                CurrentSelectedModule.OnMasterNodePooling();
            }
        }

        private void OnModuleSelected(int p_Index, Module p_Module, bool p_UpdateDefaultIndex)
        {
            if (ReferenceEquals(null, CurrentSelectedModule))
            {
                CurrentSelectedModule = p_Module;
                _CurrentKey = p_Index;
                if (p_UpdateDefaultIndex)
                {
                    _DefaultKey = p_Index;
                }
                var resultHandleIndex = _MasterNode.AddModule(this);
                SetModuleHandleIndex(resultHandleIndex);
            }
        }

        #endregion
        
        #region <Methods>

        public void SetModuleHandleIndex(int p_Index)
        {
            HandleIndex = p_Index;
        }

        private bool AddModule(int p_Index, bool p_UpdateDefaultIndex)
        {
            // 유닛 모듈은 중복해서 가질 수 없으므로, 타입을 체크해준다.
            var (valid, tryModuleType) = UnitModuleDataTool.GetIncarnateUnitTable<ModuleLabelType>(_UnitModuleType).GetModuleType(p_Index);
            if (valid)
            {
                var tryModule = default(Module);
                if (_ModuleTable.TryGetValue(tryModuleType, out var o_InnerTable))
                {
                    if (o_InnerTable.TryGetValue(p_Index, out tryModule))
                    {
                    }
                    else
                    {
                        var (spawnedModuleType, spawnedModule) = UnitModuleDataTool.GetIncarnateUnitTable<ModuleLabelType>(_UnitModuleType).SpawnModule(_MasterNode, p_Index);
                        if (ReferenceEquals(null, spawnedModule))
                        {
                            return false;
                        }
                        else
                        {
                            tryModule = (Module) spawnedModule;
                            o_InnerTable.Add(p_Index, tryModule);
                        }
                    }
                }
                else
                {
                    var (spawnedModuleType, spawnedModule) = UnitModuleDataTool.GetIncarnateUnitTable<ModuleLabelType>(_UnitModuleType).SpawnModule(_MasterNode, p_Index);
                    if (ReferenceEquals(null, spawnedModule))
                    {
                        return false;
                    }
                    else
                    {
                        tryModule = (Module) spawnedModule;
                        var _innerTable = new Dictionary<int, Module>();
                        _innerTable.Add(p_Index, tryModule);
                        _ModuleTable.Add(spawnedModuleType, _innerTable);
                    }
                }
                
                // 현재 선정된 모듈이 없는 경우, 추가된 모듈을 현재 모듈로 선정해준다.
                OnModuleSelected(p_Index, tryModule, p_UpdateDefaultIndex);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 최초에 등록된 모듈을 리턴한다.
        /// </summary>
        public Module SwitchModule()
        {
            return SwitchModule(_DefaultKey);
        }
        
        /// <summary>
        /// 지정한 타입의 모듈 중 가장 먼저 등록된 모듈을 리턴한다.
        /// </summary>
        public Module SwitchModule(ModuleLabelType p_ModuleType)
        {
            if (_ModuleTable.TryGetValue(p_ModuleType, out var o_InnerTable))
            {
                var firstIndex = o_InnerTable.Keys.First();
                return SwitchModule(firstIndex);
            }
            else
            {
                var (valid, fallbackIndex) = UnitModuleDataTool.GetIncarnateUnitTable<ModuleLabelType>(_UnitModuleType)?.GetDefaultFallbackIndex(p_ModuleType) ?? default;
                return valid ? SwitchModule(fallbackIndex) : default;
            }
        }

        public Module SwitchModule(int p_ModuleKey)
        {
            var (valid, _ModuleType) = UnitModuleDataTool.GetIncarnateUnitTable<ModuleLabelType>(_UnitModuleType).GetModuleType(p_ModuleKey);
            if (valid)
            {
                // 지정한 타입의 사고모듈을 가지는 경우
                if (_ModuleTable.TryGetValue(_ModuleType, out var o_InnerTable))
                {
                    if (o_InnerTable.TryGetValue(p_ModuleKey, out var o_TargetModule))
                    {
                        OnModuleSelected(p_ModuleKey, o_TargetModule, false);
                        OnModuleSwitched(o_TargetModule);
                        
                        return (Module) CurrentSelectedModule;
                    }
                    else
                    {
                        if (AddModule(p_ModuleKey, false))
                        {
                            var tryModule = o_InnerTable[p_ModuleKey];
                            OnModuleSwitched(tryModule);
                            
                            return (Module) CurrentSelectedModule;
                        }
                        else
                        {
                            _CurrentKey = p_ModuleKey;
                            OnModuleSwitched(default);
                            
                            return default;
                        }
                    }
                }
                // 지정한 타입의 사고모듈을 가지지 않는 경우
                else
                {
                    var _innerTable = new Dictionary<int, Module>();
                    _ModuleTable.Add(_ModuleType, _innerTable);

                    if (AddModule(p_ModuleKey, false))
                    {
                        var tryModule = _innerTable[p_ModuleKey];
                        OnModuleSwitched(tryModule);
                            
                        return (Module) CurrentSelectedModule;
                    }
                    else
                    {
                        _CurrentKey = p_ModuleKey;
                        OnModuleSwitched(default);
                        
                        return default;
                    }
                }
            }
            else
            {
                _CurrentKey = p_ModuleKey;
                OnModuleSwitched(default);
                
                return default;
            }
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
        protected void DisposeUnManaged()
        {
            if (_ModuleTable != null)
            {
                foreach (var moduleTableKV in _ModuleTable)
                {
                    var innerTable = moduleTableKV.Value;
                    foreach (var module in innerTable)
                    {
                        module.Value.Dispose();
                    }
                }
                _ModuleTable = null;
            }
            CurrentSelectedModule = null; 
        }

        #endregion
    }
}