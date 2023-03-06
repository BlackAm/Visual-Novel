#if !SERVER_DRIVE
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 스프라이트 리스트를 일정 간격으로 정해진만큼 순회하는 클래스
    /// </summary>
    public class SpriteIterator : TimerIteratorBase
    {
        #region <Fields>
        
        /// <summary>
        /// 에셋 프리셋
        /// </summary>
        private List<AssetPreset> _AssetPresetGroup;
        
        /// <summary>
        /// 스프라이트 리스트
        /// </summary>
        private List<Sprite> _SpriteList;

        /// <summary>
        /// 현재 애니메이션 클립 인덱스
        /// </summary>
        private int _ClipIndex;
        
        /// <summary>
        /// 전체 애니메이션 클립 갯수
        /// </summary>
        private int _ClipCount;

        /// <summary>
        /// 클립 인덱스 상한
        /// </summary>
        private int _ClipIndexUpperBound;

        /// <summary>
        /// 클립 인덱스 하한
        /// </summary>
        private int _ClipIndexLowerBound;
                
        /// <summary>
        /// 구간 반복 인덱스
        /// </summary>
        private int _IntervalBound;

        #endregion
        
        #region <Constructors>

        public SpriteIterator(UITool.AnimationSpriteType p_Type)
        {
            if (p_Type != UITool.AnimationSpriteType.None)
            {
                _SpriteList = new List<Sprite>();
                _AssetPresetGroup = new List<AssetPreset>();
                
                Reset();
                
                var targetRecord = AnimationSpriteData.GetInstanceUnSafe.GetTableData(p_Type);
                _ClipCount = targetRecord.SpriteNumber;
                _IteratorTimer.Initialize(targetRecord.AnimationDuration / _ClipCount);
            
                var recordLoopCount = targetRecord.LoopCount;
                if (recordLoopCount < 1)
                {
                    ProgressFlagMask.AddFlag(IteratorProgressFlag.LoopInfinity);
                }
                else
                {
                    _LoopCount = (0, recordLoopCount);
                }
                
                var spriteForm = targetRecord.SpriteNameFormat;
                var indexForm = targetRecord.IndexNumberLength;
                for (int i = 0; i < _ClipCount; i++)
                {
                    var trySpriteAssetName = string.Format(spriteForm, i.ToString().ZeroPadding(indexForm));
                    var resultTuple = LoadAssetManager.GetInstanceUnSafe
                        .LoadAsset<Sprite>
                        (
                            ResourceType.Image,
                            ResourceLifeCycleType.Free_Condition, trySpriteAssetName
                        );
                    
                    _AssetPresetGroup.Add(resultTuple.Item1);
                    _SpriteList.Add(resultTuple.Item2);
                }
                
                SetPlaySpeed(1f);
                ResetIntervalBound();
            }
        }

        public SpriteIterator(List<Sprite> p_SpriteSet, float p_AnimationDuration, int p_LoopCount)
        {
            _SpriteList = p_SpriteSet;
            
            Reset();
        
            _ClipCount = p_SpriteSet.Count;
            _IteratorTimer.Initialize(p_AnimationDuration / _ClipCount);
            
            if (p_LoopCount < 1)
            {
                ProgressFlagMask.AddFlag(IteratorProgressFlag.LoopInfinity);
            }
            else
            {
                _LoopCount = (0, p_LoopCount);
            }
            
            SetPlaySpeed(1f);
            ResetIntervalBound();
        }
        
        #endregion

        #region <Methods>

        public override void Reset()
        {
            base.Reset();
            
            _ClipIndex = 0;
        }
        
        public override IterateResultType ProgressIterating(float p_DeltaTime)
        {
            if (IsLoopOver())
            {
                return IterateResultType.ProgressOverButNotChange;
            }
            else
            {
                switch (_PingPongPhase)
                {
                    default:
                    case PingPongPhase.NormalPhase:
                    {
                        if (_IteratorTimer.IsOver())
                        {
                            _IteratorTimer.Reset();

                            var prevIndex = _ClipIndex;
                            if (_ClipIndex < _ClipIndexUpperBound)
                            {
                                _ClipIndex++;
                                
                                return prevIndex != _ClipIndex ? IterateResultType.ProgressNext : IterateResultType.ProgressNextButNotChange;
                            }
                            else
                            {
                                if (ProgressFlagMask.HasAnyFlagExceptNone(IteratorProgressFlag.LoopInterval))
                                {
                                    _PingPongPhase = PingPongPhase.UpperBound;
                                }
                                else if (ProgressFlagMask.HasAnyFlagExceptNone(IteratorProgressFlag.PingPong))
                                {
                                    _PingPongPhase = PingPongPhase.ReversePhase;
                                }
                                else
                                {
                                    _ClipIndex = 0;
                                }

                                if (CheckLoopOver())
                                {
                                    return prevIndex != _ClipIndex ? IterateResultType.ProgressOver : IterateResultType.ProgressOverButNotChange;
                                }
                                else
                                {
                                    return prevIndex != _ClipIndex ? IterateResultType.ProgressNext : IterateResultType.ProgressNextButNotChange;
                                }
                            }
                        }
                        else
                        {
                            _IteratorTimer.Progress(p_DeltaTime * _PlaySpeed);
                            
                            return IterateResultType.Progressing;
                        }
                    }
                    case PingPongPhase.UpperBound:
                    {
                        if (_UpperBoundTimer.IsOver())
                        {
                            if (ProgressFlagMask.HasAnyFlagExceptNone(IteratorProgressFlag.LoopIntervalRandomize))
                            {
                                _UpperBoundTimer = _UpperBoundRandomIntervalSeed.GetRandom();
                            }
                            else
                            {
                                _UpperBoundTimer.Reset();
                            }

                            if (ProgressFlagMask.HasAnyFlagExceptNone(IteratorProgressFlag.PingPong))
                            {
                                _PingPongPhase = PingPongPhase.ReversePhase;
                            }
                            else
                            {
                                _PingPongPhase = PingPongPhase.NormalPhase;
                                _ClipIndex = 0;
                            }
                        }
                        else
                        {
                            _UpperBoundTimer.Progress(p_DeltaTime);
                        }
                        
                        return IterateResultType.Progressing;
                    }
                    case PingPongPhase.ReversePhase:
                    {
                        if (_IteratorTimer.IsOver())
                        {
                            _IteratorTimer.Reset();
                            
                            var prevIndex = _ClipIndex;
                            if (_ClipIndex >= _ClipIndexLowerBound)
                            {
                                _ClipIndex--;
                                return prevIndex != _ClipIndex ? IterateResultType.ProgressNext : IterateResultType.ProgressNextButNotChange;
                            }
                            else
                            {
                                _PingPongPhase = PingPongPhase.LowerBound;
                        
                                if (CheckLoopOver())
                                {
                                    return prevIndex != _ClipIndex ? IterateResultType.ProgressOver : IterateResultType.ProgressOverButNotChange;
                                }
                                else
                                {
                                    return prevIndex != _ClipIndex ? IterateResultType.ProgressNext : IterateResultType.ProgressNextButNotChange;
                                }
                            }
                        }
                        else
                        {
                            _IteratorTimer.Progress(p_DeltaTime * _PlaySpeed);
                            
                            return IterateResultType.Progressing;
                        }
                    }
                    case PingPongPhase.LowerBound:
                    {
                        if (_LowerBoundTimer.IsOver())
                        {
                            if (ProgressFlagMask.HasAnyFlagExceptNone(IteratorProgressFlag.LoopIntervalRandomize))
                            {
                                _LowerBoundTimer = _LowerBoundRandomIntervalSeed.GetRandom();
                            }
                            else
                            {
                                _LowerBoundTimer.Reset();
                            }

                            _PingPongPhase = PingPongPhase.NormalPhase;
                        }
                        else
                        {
                            _LowerBoundTimer.Progress(p_DeltaTime);
                        }
                        
                        return IterateResultType.Progressing;
                    }
                }
            }
        }

        public void SetIntervalBound(int p_UpperBound, int p_LowerBound)
        {
            _ClipIndexUpperBound = p_UpperBound - 1;
            _ClipIndexLowerBound = p_LowerBound + 1;
        }

        public void ResetIntervalBound()
        {
            SetIntervalBound(_ClipCount - 1, 0);
        }
        
        public Sprite GetCurrentSprite()
        {
            return _SpriteList[_ClipIndex];
        }

        #endregion

        #region <Disposable>

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
            if (!ReferenceEquals(null, _AssetPresetGroup))
            {
                foreach (var assetPreset in _AssetPresetGroup)
                {
                    LoadAssetManager.GetInstanceUnSafe?.UnloadAsset(assetPreset);
                }
                _AssetPresetGroup.Clear();
                _AssetPresetGroup = null;
            }
        }

        #endregion
    }
}
#endif