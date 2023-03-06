#if !SERVER_DRIVE
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace k514
{
    /*public partial class ControlNavMesh
    {
        #region <Fields>

        public int CurrentCount;

        #endregion
        
        #region <Methods>

        public override bool CalcPath(Vector3 p_TargetPosition)
        {
            bool valid;
            if (_NavMeshAgent.isOnNavMesh)
            {
                valid = _NavMeshAgent.CalculatePath(p_TargetPosition, _Path);
            }
            else
            {
                valid = NavMesh.CalculatePath(_MasterNode._Transform.position, p_TargetPosition, 0, _Path);
            }
            
#if !SERVER_DRIVE
            LamiereGameManager.GetInstanceUnSafe._CurrentCorner = 1;
#endif
            _MasterNode._PhysicsObject.SetPath(_Path, PhysicsTool.AutonomyPathStoppingDistancePreset.GetStoppingRange(_MasterNode));
            return valid;
        }

        public override void ShowPath(bool p_Valid, Vector3 p_FromVector)
        {
            ShowPathAsync(p_Valid, p_FromVector);
        }

        public async UniTaskVoid ShowPathAsync(bool p_Valid, Vector3 p_FromVector)
        {
#if !SERVER_DRIVE
            var gameManager = LamiereGameManager.GetInstanceUnSafe;
            gameManager._CurrentPath = _Path;
#endif
            var p_TargetPosition = _MasterNode.GetTargetPosition();
            if (p_FromVector == Vector3.zero)
            {
                _NavMeshAgent.CalculatePath(p_TargetPosition, _Path);
            }
            else
            {
                NavMesh.CalculatePath(p_FromVector, p_TargetPosition, NavMesh.AllAreas, _Path);
            }
            // gameManager.SetMovePointWayTargetingDecalDisable();
            CurrentCount = 0;
            _MasterNode.SetShowingWay(true);
            
            // await UniTask.NextFrame();
            
            var pathCount = _Path.corners.Length;
            if (pathCount <= 1)
            {
                _MasterNode.SetTargetPosition(Vector3.zero);
                return;
            }

            if (p_Valid)
            {
                for (var i = 1; i < pathCount; i++)
                {
                    if (pathCount > 2)
                    {
                        if (i > 0)
                        {
                            var position = _Path.corners[i] - _Path.corners[i - 1];
                            var effectPosition = _Path.corners[i - 1];
                            if (position.magnitude > 4)
                            {
                                if ((_Path.corners[i] - effectPosition).magnitude > 4)
                                {
                                    while ((_Path.corners[i] - effectPosition).magnitude > 4/* && CurrentCount < gameManager.MAX_CALL_COUNT#1#)
                                    {
                                        var direction = effectPosition.GetDirectionVectorTo(_Path.corners[i]).normalized * LamiereGameManager.GetInstanceUnSafe.MOVEMENT * 0.1f;
                                        effectPosition += new Vector3(direction.x, direction.y, direction.z);
                                        SetMovePoint(effectPosition, direction, false);
                                    }
                                }
                            }

                            if (gameManager.GetCachedPosition() == Vector3.zero)
                            {
                                gameManager.SetPositionVector(_MasterNode._Transform.position);
                            }
                            
                            if ((_Path.corners[i] - gameManager._Position).magnitude <= 2f)
                            {
                                gameManager._CurrentCorner = i + 1;
                            }
                            else if ((_Path.corners[i] - gameManager._Position).magnitude > 2f)
                            {
                                if ((p_TargetPosition - _Path.corners[i]).magnitude > 0.1f/* && CurrentCount < gameManager.MAX_CALL_COUNT#1#)
                                {
                                    var direction = effectPosition.GetDirectionVectorTo(_Path.corners[i]).normalized * LamiereGameManager.GetInstanceUnSafe.MOVEMENT * 0.1f;
                                    SetMovePoint(_Path.corners[i], direction, false);
                                    gameManager._CurrentCorner = i + 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        var effectPosition = _MasterNode._Transform.position;

                        if (p_FromVector != Vector3.zero)
                        {
                            effectPosition = p_FromVector;
                        }
                        
                        while ((p_TargetPosition - effectPosition).magnitude > 4/* && CurrentCount < gameManager.MAX_CALL_COUNT#1#)
                        {
                            var direction = effectPosition.GetDirectionVectorTo(p_TargetPosition).normalized * LamiereGameManager.GetInstanceUnSafe.MOVEMENT * 0.1f;
                            effectPosition += new Vector3(direction.x, direction.y, direction.z);
                            SetMovePoint(effectPosition, direction, false);
                        }
                    }
                }

                if (pathCount > 2)
                {
                    if ((p_TargetPosition - _Path.corners[pathCount - 1]).magnitude > 3)
                    {
                        var effectPosition = _Path.corners[pathCount - 1];
                        var direction = effectPosition.GetDirectionVectorTo(p_TargetPosition).normalized * LamiereGameManager.GetInstanceUnSafe.MOVEMENT * 0.1f;
                        while ((p_TargetPosition - effectPosition).magnitude > 3/* && CurrentCount < gameManager.MAX_CALL_COUNT#1#)
                        {
                            effectPosition += new Vector3(direction.x, direction.y, direction.z);
                            SetMovePoint(effectPosition, direction, false);
                        }
                    }
                }
                gameManager._ShowEffect = false;
            }
        }

        public void SetMovePoint(Vector3 p_Position, Vector3 p_Direction, bool p_ResetTime)
        {
            LamiereGameManager.GetInstanceUnSafe.SetMovePointWayTargetingDecalEnable(p_Position, p_Direction, ++CurrentCount, p_ResetTime);
            LamiereGameManager.GetInstanceUnSafe.SetPositionVector(p_Position, p_Direction);
        }

        #endregion
        
    }*/
}
#endif