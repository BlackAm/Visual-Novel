namespace k514
{
    public partial class ObjectDeployEventRecord
    {
        #region <Methods>

        public void CalculateAllFrameDeployAffine(ObjectDeployEventPreset p_DeployPreset, ObjectDeployEventExtraPreset p_ObjectDeployEventExtraPreset)
        {
            InitializeRecord(p_DeployPreset);
            InitializeStartPivot(p_ObjectDeployEventExtraPreset);
            
            if (_EventDeployCollection == null 
                || !p_ObjectDeployEventExtraPreset.OnCheckEventEnterable(this))
            {
                return;
            }
      
            // 타임라인에 따른 이벤트 차수
            var deploySequenceCount = 0;
            // 한 이벤트 차수에 포함된 이벤트 숫자
            var concurrentTracingEventCount = _ConcurrentTracingEventCount;
            var eventDeployCollection = _EventDeployCollection;
            
            // [타임스탬프, List[배치인덱스, 배치이벤트]] 반복문
            foreach (var deployCollectionListKV in eventDeployCollection)
            {
                var timeStamp = deployCollectionListKV.Key;
                var deployCollectionList = deployCollectionListKV.Value;
                var isFirstLoop = deploySequenceCount < 1;
                var eventCount = 0;
                
                // List[배치인덱스, 배치이벤트] 반복문
                foreach (var deployCollectionTuple in deployCollectionList)
                {
                    var resultPreset = default(ObjectDeployEventExtraPreset);
                    var deployIndex = deployCollectionTuple.t_DeployDataIndex;
                    var progressRate = GetProgressRate(deployIndex, timeStamp);
                    
                    if (isFirstLoop)
                    {
                        p_ObjectDeployEventExtraPreset.ProgressRate01 = progressRate;
                        resultPreset = ObjectDeployDataRoot.GetInstanceUnSafe[deployIndex].CalculateDeployAffine(p_ObjectDeployEventExtraPreset); 
                    }
                    else
                    {
                        var prevIndex = (deploySequenceCount - 1) * concurrentTracingEventCount + eventCount;
                        var prevResult = _DeployPresetStorage[prevIndex];
                        var prevExtraPreset = prevResult;
                        prevExtraPreset.ProgressRate01 = progressRate;
                        resultPreset = ObjectDeployDataRoot.GetInstanceUnSafe[deployIndex].CalculateDeployAffine(prevExtraPreset); 
                    }
                    _DeployPresetStorage.Add(resultPreset);
                    eventCount++;
                }
                deploySequenceCount++;
            }
        }

        public void GetDeployAffineMap(ObjectDeployTimeline p_ObjectDeployTimeline, ObjectDeployEventPreset p_DeployPreset, ObjectDeployTool.DeployableType p_TryType, ObjectDeployEventExtraPreset p_ObjectDeployEventExtraPreset)
        {
            InitializeRecord(p_DeployPreset);
            InitializeStartPivot();
            
            if (!p_ObjectDeployEventExtraPreset.OnCheckEventEnterable(this))
            {
                return;
            }
            
            var deploySequenceCount = 0;
            var eventDeployCollection = _EventDeployCollection;
            var concurrentTracingEventCount = _ConcurrentTracingEventCount;
            
            foreach (var deployCollectionListKV in eventDeployCollection)
            {
                var timeStamp = deployCollectionListKV.Key;
                var deployCollectionList = deployCollectionListKV.Value;
                var isFirstLoop = deploySequenceCount < 1;
                var eventCount = 0;

                foreach (var deployCollectionTuple in deployCollectionList)
                {
                    if (deployCollectionTuple.t_EventCollection.ContainsKey(p_TryType))
                    {
                        var resultPreset = default(ObjectDeployEventExtraPreset);
                        var deployIndex = deployCollectionTuple.t_DeployDataIndex;
                        var progressRate = GetProgressRate(deployIndex, timeStamp);

                        if (isFirstLoop)
                        {
                            p_ObjectDeployEventExtraPreset.ProgressRate01 = progressRate;
                            resultPreset = ObjectDeployDataRoot.GetInstanceUnSafe[deployIndex].CalculateDeployAffine(p_ObjectDeployEventExtraPreset); 
                        }
                        else
                        {
                            var prevIndex = (deploySequenceCount - 1) * concurrentTracingEventCount + eventCount;
                            var prevResult = _DeployPresetStorage[prevIndex];
                            var prevExtraPreset = prevResult;
                            prevExtraPreset.ProgressRate01 = progressRate;
                            resultPreset = ObjectDeployDataRoot.GetInstanceUnSafe[deployIndex].CalculateDeployAffine(prevExtraPreset); 
                        }

                        var timeMsec = 0.001f * timeStamp;
                        p_ObjectDeployTimeline.AddTimelineAffine(timeMsec, resultPreset.CurrentPivot);
                        _DeployPresetStorage.Add(resultPreset);
                        eventCount++;
                    }
                }
                deploySequenceCount++;
            }
        }

        #endregion
    }
}