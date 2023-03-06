using System;
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 시간 서순에 따른 아핀값을 가지는 프리셋
    /// </summary>
    public class ObjectDeployTimeline
    {
        #region <Fields>

        /// <summary>
        /// 아핀 계산의 기준이 되는 유닛
        /// </summary>
        private Unit _MasterNode;
        
        /// <summary>
        /// 아핀 계산의 시작점
        /// </summary>
        private TransformTool.AffineCachePreset _StartPivot;
        
        /// <summary>
        /// [타임스탬프, 아핀] 컬렉션
        /// </summary>
        public Dictionary<float, List<TransformTool.AffineCachePreset>> _Timeline;

        /// <summary>
        /// 위 컬렉션의 키셋
        /// </summary>
        public List<float> _TimelineKeySet;

        /// <summary>
        /// 벡터맵 키셋 인덱스
        /// </summary>
        private int _VectorMapKeySetCurrentIndex;
                
        /// <summary>
        /// 등록된 방향맵 레코드 숫자 역수
        /// </summary>
        private float _InvDirectionMapKeySetCount;
        
        #endregion

        #region <Constructors>

        public ObjectDeployTimeline()
        {
            _Timeline = new Dictionary<float, List<TransformTool.AffineCachePreset>>();
            _TimelineKeySet = new List<float>();
        }

        #endregion

        #region <Methods>

        private void UpdateKeySetPreset()
        {
            _VectorMapKeySetCurrentIndex = 0;
            _InvDirectionMapKeySetCount = 1f / _TimelineKeySet.Count; 
        }

        public void AddTimelineAffine(float p_TimeStamp, TransformTool.AffineCachePreset p_Affine)
        {
            if (_Timeline.TryGetValue(p_TimeStamp, out var o_List))
            {
                o_List.Add(p_Affine);
            }
            else
            {
                _Timeline.Add(p_TimeStamp, o_List = new List<TransformTool.AffineCachePreset>());
                _TimelineKeySet.Add(p_TimeStamp);
                o_List.Add(p_Affine);
            }
        }
        
        public void LoadTimeline(Unit p_MasterNode, TransformTool.AffineCachePreset p_StartPivot, int p_ObjectDeployIndex)
        {
            ClearTimeline();
            _MasterNode = p_MasterNode;
            _StartPivot = p_StartPivot;
            
            var deployRecorder = ObjectDeployLoader.GetInstance.GetObjectDeployEventRecord(_MasterNode, _StartPivot);
            var targetPreset = ObjectVectorMapData.GetInstanceUnSafe[p_ObjectDeployIndex];
            deployRecorder.GetDeployAffineMap(this, targetPreset.DeployEventPresetMap, ObjectDeployTool.DeployableType.None, default);
            deployRecorder.RetrieveObject();
            
            UpdateKeySetPreset();
        }

        public void LoadTransform(TransformTool.AffineCachePreset p_StartPivot, float p_Predelay, bool p_Accumulate)
        {
            if (!p_Accumulate)
            {
                ClearTimeline();
            }
            _StartPivot = p_StartPivot;

            AddTimelineAffine(p_Predelay, p_StartPivot);
            
            UpdateKeySetPreset();
        }

        public void LoadBlizzardSampling(SamplePositionTool.BlizzardSamplingIndexType p_Type, TransformTool.AffineCachePreset p_StartPivot, float p_Predelay, bool p_Accumulate, ObjectDeployTimelinePreset p_ObjectDeployTimelinePreset)
        {
            if (!p_Accumulate)
            {
                ClearTimeline();
            }
            _StartPivot = p_StartPivot;

            SamplePositionTool.GetBlizzardSampling(this, p_Type, _StartPivot, p_Predelay, p_ObjectDeployTimelinePreset);
            
            UpdateKeySetPreset();
        }

        public void LoadWideStreamSampling(TransformTool.AffineCachePreset p_StartPivot, float p_Predelay, bool p_Accumulate, ObjectDeployTimelinePreset p_ObjectDeployTimelinePreset)
        {
            if (!p_Accumulate)
            {
                ClearTimeline();
            }
            _StartPivot = p_StartPivot;

            SamplePositionTool.GetWideStreamSampling(this, _StartPivot, p_Predelay, p_ObjectDeployTimelinePreset);
            
            UpdateKeySetPreset();
        }
        
        public void ClearTimeline()
        {
            _StartPivot = default;
            _Timeline.Clear();
            _TimelineKeySet.Clear();
            _VectorMapKeySetCurrentIndex = 0;
            _InvDirectionMapKeySetCount = 0f; 
        }

        /// <summary>
        /// 타임라인상에서 지정한 타임스탬프에 가장 가까운 아핀값을 리턴하는 메서드
        /// </summary>
        public List<TransformTool.AffineCachePreset> GetCeilingTimelineAffine(float p_TimeStamp)
        {
            var tryKey = _TimelineKeySet.FindCeilingIndex(p_TimeStamp);
            return _Timeline[tryKey];
        }
        
        /// <summary>
        /// 현재 인덱스가 가리키는 타임스탬프 값보다 큰 시간값이 파라미터로 넘어온 경우, 참을 리턴하고
        /// 인덱스를 하나 더한다.
        /// </summary>
        public (bool, List<TransformTool.AffineCachePreset>) GetCurrentAffine(float p_TimeStamp)
        {
            if (_TimelineKeySet.CheckGenericCollectionSafe(_VectorMapKeySetCurrentIndex))
            {
                var tryMsec = _TimelineKeySet[_VectorMapKeySetCurrentIndex];
                if (p_TimeStamp > tryMsec)
                {
                    _VectorMapKeySetCurrentIndex++;
                    return (true, _Timeline[tryMsec]);
                }
                else
                {
                    return default;
                }
            }
            else
            {
                return default;
            }
        }

        public float GetCurrentProgressRate()
        {
            return _VectorMapKeySetCurrentIndex * _InvDirectionMapKeySetCount;
        }

        #endregion
    }
}