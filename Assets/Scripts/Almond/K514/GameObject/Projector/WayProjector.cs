#if !SERVER_DRIVE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace k514{
    public class WayProjector : SimpleProjector
    {
        /*#region <Fields>
        
        private PetpetIterator _ScaleIterator;
        
        /// <summary>
        /// 최초 스폰되었을 때의 부모 아핀 객체
        /// </summary>
        private Transform _OriginParent;

        private Unit _MasterNode;

        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();

            DeployableType = ObjectDeployTool.DeployableType.Projector;
            _Transform.SetParent(ProjectorSpawnManager.GetInstance._ProjectorObjectWrapper);
            _OriginParent = _Transform.parent;
            SetScale(1f);
        }

        public override void OnPooling()
        {
            base.OnPooling();   

            var gameManager = LamiereGameManager.GetInstanceUnSafe;
            _MasterNode = gameManager._ClientPlayer;

            var duration = CalcProjectTime();

            _ScaleIterator.Reset();
            if (gameManager.IsCachedTime() <= 0f)
            {
                if (duration - gameManager.GetBeforeTime() > gameManager._MaxEffectSpeed)
                {
                    duration = gameManager._CachedTime = gameManager._MaxEffectSpeed;
                }
                else
                {
                    gameManager._CachedTime = duration;
                }
            }
            
            _ProjectTimerTuple.t_Valid = true;
            duration = -1;
            _ProjectTimerTuple.t_Timer.Initialize(duration);
            
            if (duration < gameManager._CachedTime * 10f)
            {
                gameManager.SetBeforeTime(duration);
            }
        }

        protected override void Update()
        {
            switch (_CurrentPhase)
            {
                case ProjectorWorkState.None:
                    break;
                case ProjectorWorkState.Enter:
                    _CurrentPhase++;
                    _InnerProjector.enabled = true;
                    break;
                case ProjectorWorkState.FadeIn:
                    OnFadeIn(Time.deltaTime);
                    break;
                case ProjectorWorkState.Project:
                    OnProjection(Time.deltaTime);
                    _InnerProjector.enabled = true;
                    break;
                case ProjectorWorkState.FadeOut:
                    //OnFadeOut(Time.deltaTime);
                    break;
                case ProjectorWorkState.Terminate:
                    SetProjectionStart();
                    /*RetrieveObject();
                    RespawnEffect();#1#
                    break;
            }
        }

        protected override void OnLoadFirstImage()
        {
            var projectorExtraData = (PrefabExtraData_PetPetProjector.PrefabExtraDataPPProjectorRecord) _PrefabKey._PrefabExtraPreset._PrefabExtraDataRecord;
            _AssetTuple = ImageNameTableData.GetInstanceUnSafe.GetTexture(projectorExtraData.ImageIndex,
                ResourceType.Image, ResourceLifeCycleType.Free_Condition);
            SetTexture(_AssetTuple.Item2);

            _ScaleIterator = new PetpetIterator(projectorExtraData.MinScale, projectorExtraData.MaxScale, projectorExtraData.Interval, projectorExtraData.LoopCount);
            SetScale(_ScaleIterator.GetCurrentScale());
        }

        #endregion

        #region <Methods>

        private float CalcProjectTime()
        {
            var gameManager = LamiereGameManager.GetInstanceUnSafe;
            var currentEffectCount = _MasterNode.GetEffectCount();
            if (currentEffectCount < 0 || gameManager.IsCachedTime() <= 0)
            {
                var stepOffset = _MasterNode.GetComponent<CharacterController>().stepOffset;
                if (gameManager.GetCachedPosition() == Vector3.zero)
                {
                    return (_Transform.position - _MasterNode._Transform.position).magnitude / _MasterNode.GetScaledMovementSpeed() * 0.95f + gameManager.GetBeforeTime();
                }
                else
                {
                    return (_Transform.position - _MasterNode._Transform.position).magnitude / _MasterNode.GetScaledMovementSpeed() * 0.95f;
                }
            }
            else
            {
                return gameManager._CachedTime + gameManager.GetBeforeTime();
            }
        }

        private void RespawnEffect()
        {
            if (!ReferenceEquals(null, _MasterNode))
            {
                LamiereGameManager.GetInstanceUnSafe._TargetingMoveWayPointList.Remove(this);
                
                _MasterNode.SetEffectCount(_MasterNode.GetEffectCount() - 1);
                
                var gameManager = LamiereGameManager.GetInstanceUnSafe;
                var path = _MasterNode._PhysicsObject.GetPath();
                if (_MasterNode.IsShowingWay() && !gameManager._ShowEffect && !gameManager.ReSearchWayPoint)
                {
                    if (path.corners.Length != gameManager._CurrentPath.corners.Length)
                    {
                        _MasterNode._PhysicsObject.SetPath(path, PhysicsTool.AutonomyPathStoppingDistancePreset.GetStoppingRange(_MasterNode));
                        // _MasterNode._PhysicsObject.ShowPath(true);
                        return;
                    }

                    if ((gameManager._Position - path.corners[gameManager._CurrentCorner]).magnitude < 4 &&
                        (path.corners[gameManager._CurrentCorner] - _MasterNode.GetTargetPosition()).magnitude > 4)
                    {
                        gameManager._CurrentCorner++;
                    }
                    
                    var direction = gameManager._Position.GetDirectionVectorTo(path.corners[gameManager._CurrentCorner]).normalized * gameManager.MOVEMENT;
                    var position = gameManager._Position += new Vector3(direction.x * 0.1f, direction.y * 0.1f, direction.z * 0.1f);
                    gameManager.SetMovePointWayTargetingDecalEnable(position, direction, _MasterNode.GetEffectCount() + 1, true);
                    gameManager.SetPositionVector(position, direction);
                }
            }
        }

        #endregion*/
    }
}


#endif