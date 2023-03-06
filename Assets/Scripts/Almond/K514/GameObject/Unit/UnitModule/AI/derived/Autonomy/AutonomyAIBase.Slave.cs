using UnityEngine;

namespace k514
{
    public partial class AutonomyAIBase
    {
        #region <Fields>

        /// <summary>
        /// 종자 유닛 행동 패턴 카운터
        /// </summary>
        private int _SlaveLifeCount;

        #endregion

        #region <Callbacks>

        private void OnUpdateSlaveState()
        {
            
        }

        private void OnUpdateTimeBlockSlaveState()
        {
            /*var masterNode = _MasterNode.MasterNode.Node;

            if (_SlaveLifeCount > ThinkableTool.__SLAVE_AI_ACTION_INTERVAL)
            {
                var sqrDistance = UnitInteractManager.GetInstance.GetSqrDistanceBetween(masterNode, _MasterNode);
                if(sqrDistance > ThinkableTool.__SQR_SLAVE_AI_TRACE_DISTANCE_UPPERBOUND)
                {
                    _SlaveLifeCount = Random.Range(ThinkableTool.__NEGATIVE_SLAVE_AI_ACTION_INTERVAL, 0);
                    UnitInteractManager.GetInstance.UpdateSqrDistance(masterNode, _MasterNode);
                    SwitchStateMove(masterNode.GetRandomAroundPosition(ThinkableTool.__SLAVE_AI_WANDERING_DISTANCE), true, true);
                }
                else if(!HasEnemy() && sqrDistance > ThinkableTool.__SQR_SLAVE_AI_TRACE_DISTANCE_LOWERBOUND)
                {
                    _SlaveLifeCount = Random.Range(ThinkableTool.__NEGATIVE_SLAVE_AI_ACTION_INTERVAL, 0);
                    UnitInteractManager.GetInstance.UpdateSqrDistance(masterNode, _MasterNode);
                    SwitchStateMove(masterNode.GetRandomAroundPosition(ThinkableTool.__SLAVE_AI_WANDERING_DISTANCE), false, false);
                }
            }
            else
            {
                _SlaveLifeCount++;
            }*/
        }
        
        #endregion
        
        #region <Methods>

        public override void SetSlaveMasterUnit(Unit p_Target)
        {
            _MasterNode.MasterNode.SetNode(PrefabInstanceTool.MasterNodeRelateType.Slave, p_Target);
        }

        #endregion
    }
}