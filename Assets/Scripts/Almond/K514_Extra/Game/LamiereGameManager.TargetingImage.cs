#if !SERVER_DRIVE
using System.Collections.Generic;

using UnityEngine;
using UI2020;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;

namespace k514
{
    public partial class LamiereGameManager
    {
        /*#region <Consts>

        public const float MAX_MOVEMENTSPEED_VALUE = 6.5f;
        public int MAX_CALL_COUNT = 10;
        public int MOVEMENT = 25;

        #endregion
        
        #region <Fields>

        private SimpleProjector _TargetingEnemyDecal;
        private PetPetProjector _TargetingMovePointDecal;
        private WayProjector _TargetingMoveWayPointDecal;
        public  List<WayProjector> _TargetingMoveWayPointList;
        public LamiereUnit _CurrentTarget;
        public float _CachedTime;
        public float _BeforeTime;
        public Vector3 _Position;
        public Vector3 _Direction;
        public float _MaxEffectSpeed;
        public int _CurrentCorner;
        public bool _ShowEffect;
        public NavMeshPath _CurrentPath;

        #endregion

        #region <Callbacks>

        private async void OnAwakeTargetingDecal()
        {

            try
            {
                await UniTask.SwitchToMainThread();
                /*
                _TargetingEnemyDecal = (
                    await ProjectorSpawnData.GetInstanceUnSafe.SpawnPrefabAsync<SimpleProjector>(1, Vector3.zero,
                        ObjectDeployTool.ObjectDeploySurfaceDeployType.None, ResourceLifeCycleType.Free_Condition)).Item2;

                _TargetingMovePointDecal = (
                    await ProjectorSpawnData.GetInstanceUnSafe.SpawnPrefabAsync<PetPetProjector>(3, Vector3.zero,
                        ObjectDeployTool.ObjectDeploySurfaceDeployType.None, ResourceLifeCycleType.Free_Condition)).Item2;
                        #1#
                _TargetingEnemyDecal =
                    ProjectorSpawnData.GetInstanceUnSafe.SpawnPrefab<SimpleProjector>(1, Vector3.zero,
                        ObjectDeployTool.ObjectDeploySurfaceDeployType.None, ResourceLifeCycleType.Free_Condition).Item2;

                _TargetingMovePointDecal =
                    ProjectorSpawnData.GetInstanceUnSafe.SpawnPrefab<PetPetProjector>(3, Vector3.zero,
                        ObjectDeployTool.ObjectDeploySurfaceDeployType.None, ResourceLifeCycleType.Free_Condition).Item2;

                _TargetingMoveWayPointList = new List<WayProjector>();

                _TargetingMovePointDecal._Transform.SetParent(SystemBoot.GetInstance._Transform, false);

#if UNITY_EDITOR
                _TargetingEnemyDecal.name = "TargetingEnemyProjector";
                _TargetingMovePointDecal.name = "TargetingPivotProjector";
#endif
                SetEnemyTargetingDecalDisable();
                _CurrentCorner = 1;
                _ShowEffect = false;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError(e.StackTrace);
            }


        }

        #endregion

        #region <Methods>

        public void SetEnemyTargetingDecalEnable(LamiereUnit p_TargetUnit)
        {
            
            if(ReferenceEquals(p_TargetUnit,null) && !ReferenceEquals(_TourchTarget,null))
            {
                p_TargetUnit = _TourchTarget as LamiereUnit;
            }

            if (p_TargetUnit.IsValid())
            {
                if (_CurrentTarget.IsValid())
                {
                    _CurrentTarget.OnUnitSelectedAutoPlayerTarget(false);
                }

                _CurrentTarget = p_TargetUnit;
                MainGameUI.Instance.mainUI.SetTargetSearchMonsterUI(_CurrentTarget);
                _CurrentTarget.OnUnitSelectedAutoPlayerTarget(true);
    
                _TargetingEnemyDecal.SetParent(p_TargetUnit._Transform, false);
                _TargetingEnemyDecal._Transform.localPosition = Vector3.zero;
                if (p_TargetUnit._PrefabExtraDataRecord.TargetingEffectScale != 0)
                {
                    _TargetingEnemyDecal.SetScale(p_TargetUnit._PrefabExtraDataRecord.TargetingEffectScale);
                }
                else
                {
                    _TargetingEnemyDecal.SetScale(p_TargetUnit._RangeObject.Radius.CurrentValue);
                }
                _TargetingEnemyDecal._Transform.localPosition = p_TargetUnit._PrefabExtraDataRecord.TargetingEffectPosition;
                _TargetingEnemyDecal.SetProjectionStart();
                
                MainGameUI.Instance.mainUI.SetTargetUnit(_CurrentTarget);
            }
            else
            {
                SetEnemyTargetingDecalDisable();
            }
        }
        public void ClearTargetUnit(Unit unit)
        {
            if(!ReferenceEquals(_TourchTarget,null) && unit.EventKey == _TourchTarget.EventKey)
            {
                ClearTargetUnit();
            }
        }
        public void ClearTargetUnit()
        {
            _TourchTarget = null;
            SetEnemyTargetingDecalDisable();
        }
        public void TargetDistanceCheck()
        {
            if (ReferenceEquals(_TourchTarget, null) || ReferenceEquals(_ClientPlayer,null)) return;

            var distance = System.Math.Pow(25, 2);
            if(UnitInteractManager.GetInstance.GetSqrDistanceBetween(_ClientPlayer, _TourchTarget) > distance)
            {
                ClearTargetUnit(_TourchTarget);
            }
        }

        public void SetEnemyTargetingDecalDisable()
        {
            if (_CurrentTarget.IsValid())
            {
                _CurrentTarget.OnUnitSelectedAutoPlayerTarget(false);
                _CurrentTarget = null;
                _TourchTarget = null;
            }
            _TargetingEnemyDecal.SetParent(SystemBoot.GetInstance._Transform, true);
            _TargetingEnemyDecal.SetProjectionFadeOut();
                                
            if (MainGameUI.Instance)
            {
                MainGameUI.Instance.mainUI.SetTargetUnit(null);
            }
        }
        
        public void SetMovePointTargetingDecalEnable(Vector3 p_TargetPosition)
        {
            _TargetingMovePointDecal._Transform.localPosition = p_TargetPosition;
            _TargetingMovePointDecal.SetProjectionStart();
        }

        public void SetMovePointTargetingDecalDisable()
        {
            _TargetingMovePointDecal.SetProjectionFadeOut();
        }

        public void SetMovePointWayTargetingDecalEnable(Vector3 p_TargetPosition, Vector3 p_TargetDirection, int p_Count, bool p_ResetTime)
        {
            if (p_ResetTime)
            {
                ResetCachedTime();
            }
            
            _TargetingMoveWayPointDecal = ProjectorSpawnData.GetInstanceUnSafe.SpawnPrefab<WayProjector>(5, p_TargetPosition,
                ObjectDeployTool.ObjectDeploySurfaceDeployType.None, ResourceLifeCycleType.Scene).Item2;
            _TargetingMoveWayPointDecal.SetProjectionStart();
            RotateMoveWay(_TargetingMoveWayPointDecal, p_TargetDirection);
            _TargetingMoveWayPointList.Add(_TargetingMoveWayPointDecal);
            _ClientPlayer.SetEffectCount(p_Count);
            
            if ((p_TargetPosition - _ClientPlayer.GetTargetPosition()).magnitude < 4)
            {
                _ClientPlayer.SetShowingWay(false);
            }
            
            if (p_ResetTime)
            {
                ResetCachedTime();
            }
        }

        public void SetMovePointWayTargetingDecalDisable()
        {
            _ShowEffect = true;
            foreach (var decal in _TargetingMoveWayPointList)   
            {
                decal.SetProjectionTerminate();
                decal.RetrieveObject();
            }
            _TargetingMoveWayPointList.Clear();
            _ClientPlayer?.SetShowingWay(false);
            _ClientPlayer?.SetEffectCount(0);
            ResetCachedTime();
            ResetCachedPosition();
            SetBeforeTime(0f);
            _CurrentCorner = 1;
        }

        public void RotateMoveWay(WayProjector p_WayProjector, Vector3 p_Direction)
        {
            var angle = Mathf.Atan2(p_Direction.z, p_Direction.x) * Mathf.Rad2Deg;
            var rotation = new Vector3(0, 90 - angle);
            
            p_WayProjector._Transform.Rotate(rotation);
        }

        public void SetPositionVector(Vector3 p_Position)
        {
            _Position = p_Position;
        }
        
        public void SetPositionVector(Vector3 p_Position, Vector3 p_Direction)
        {
            _Position = p_Position;
            _Direction = p_Direction;
        }

        public Vector3 GetCachedPosition()
        {
            return _Position;
        }
        
        public void ResetCachedPosition()
        {
            _Position = Vector3.zero;
        }

        public void ResetCachedTime()
        {
            _CachedTime = 0f;   
        }

        public float IsCachedTime()
        {
            return _CachedTime;
        }

        public void SetBeforeTime(float p_Value)
        {
            _BeforeTime = p_Value;
        }

        public float GetBeforeTime()
        {
            return _BeforeTime;
        }

        #endregion*/
    }
}
#endif