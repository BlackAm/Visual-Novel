using System;
using UnityEngine;

namespace k514
{
    public class DefaultAutoMutton : AutoMuttonBase
    {
        #region <Fields>

        private ObjectDeployTimeline _ObjectDeployTimelinePivot;
        private ObjectDeployTimeline _ObjectDeployTimelineDeploy;

        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();
            
            _ObjectDeployTimelinePivot = new ObjectDeployTimeline();
            _ObjectDeployTimelineDeploy = new ObjectDeployTimeline();
        }

        public override void OnPooling()
        {
            base.OnPooling();

            switch (_Record.AutoMuttonAffineType)
            {
                case AutoMuttonTool.AutoMuttonAffineType.Transform:
                    _ObjectDeployTimelineDeploy.ClearTimeline();                
                    break;
                case AutoMuttonTool.AutoMuttonAffineType.VectorMap:
                    _ObjectDeployTimelinePivot.ClearTimeline();
                    _ObjectDeployTimelineDeploy.ClearTimeline();               
                    break;
            }
        }

        #endregion

        #region <Methods>

        protected override bool OnUpdateAutoMutton(float p_DeltaTime)
        {
            if (OnUpdateTimer(p_DeltaTime))
            {
                var elapsedTime = _WholeProgressTimer.ElapsedTime;
                var (valid, affineSet) = _ObjectDeployTimelineDeploy.GetCurrentAffine(elapsedTime);
                if (valid)
                {
                    foreach (var affine in affineSet)
                    {
                        ObjectDeployLoader.GetInstance.CastDeployEventMap(_Record.DeployEventIndex, MasterNode, affine);
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Run(uint p_PreDelay)
        {
            _ObjectDeployTimelineDeploy.ClearTimeline();
            
            switch (_Record.AutoMuttonAffineType)
            {
                case AutoMuttonTool.AutoMuttonAffineType.Transform:
                {
                    switch (_Record.AutoMuttonSampleType)
                    {
                        case AutoMuttonTool.AutoMuttonSampleType.Transform:
                            _ObjectDeployTimelineDeploy.LoadTransform(_Transform, 0f, true);
                            break;
                        case AutoMuttonTool.AutoMuttonSampleType.WideStream:
                            _ObjectDeployTimelineDeploy.LoadWideStreamSampling(_Transform, 0f, true, _Record.ObjectDeployTimelinePreset);
                            break;
                        case AutoMuttonTool.AutoMuttonSampleType.Blizzard:
                            _ObjectDeployTimelineDeploy.LoadBlizzardSampling(_Record.BlizzardSamplingIndexType, _Transform, 0f, true, _Record.ObjectDeployTimelinePreset);
                            break;
                    }
                }
                    break;
                case AutoMuttonTool.AutoMuttonAffineType.VectorMap:
                {
                    _ObjectDeployTimelinePivot.LoadTimeline(MasterNode, _Transform, _Record.VectorMapIndex);

                    var tryTimeline = _ObjectDeployTimelinePivot._Timeline;
                    var tryTimelinePreset = _Record.ObjectDeployTimelinePreset;
                    var sampleType = _Record.AutoMuttonSampleType;
                    foreach (var kvPair in tryTimeline)
                    {
                        var preDelay = kvPair.Key;
                        var tryAffine = kvPair.Value[0];

                        switch (sampleType)
                        {
                            case AutoMuttonTool.AutoMuttonSampleType.Transform:
                                _ObjectDeployTimelineDeploy.LoadTransform(tryAffine, preDelay, true);
                                break;
                            case AutoMuttonTool.AutoMuttonSampleType.WideStream:
                                _ObjectDeployTimelineDeploy.LoadWideStreamSampling(tryAffine, preDelay, true, tryTimelinePreset);
                                break;
                            case AutoMuttonTool.AutoMuttonSampleType.Blizzard:
                                _ObjectDeployTimelineDeploy.LoadBlizzardSampling(_Record.BlizzardSamplingIndexType, tryAffine, preDelay, true, tryTimelinePreset);
                                break;
                        }
                    }
                }
                    break;
            }
            
            base.Run(p_PreDelay);
        }

        #endregion
    }
}