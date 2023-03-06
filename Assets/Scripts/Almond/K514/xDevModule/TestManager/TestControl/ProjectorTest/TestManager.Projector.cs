#if UNITY_EDITOR && ON_GUI
using UnityEngine;
using System.Collections.Generic;

namespace k514
{
    public partial class TestManager
    {
        #region <Fields>

        #endregion

        #region <Callbacks>

        void OnAwakeProjector()
        {
            var targetControlType = TestControlType.ProjectorTest;

            SetExtraIntInput(targetControlType, 0);
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.Q, Projecting, "프로젝터 생성");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.W, LineProjecting, "선 프로젝터 생성");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.E, QuestTrigger, "메인 퀘스트 트리거");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.R, QuestTriggerLoad, "메인 퀘스트 트리거 불러오기");
        }

        #endregion

        #region <Methods>
        
        private void Projecting(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease && _SelectedUnit.IsValid())
            {
                var targetPos = _SelectedUnit._Transform.position;
                ProjectorSpawnManager.GetInstance.Project<SimpleProjector>(_CurrentExtraIntInput, targetPos);
            }
        }
        
        private void LineProjecting(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease && _SelectedUnit.IsValid())
            {
                var targetPos = _SelectedUnit._Transform.position;
            }
        }

        private void QuestTrigger(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                //메인 퀘스트 자동 추가
                StoryManager.GetInstance.NewStory(1001);
                StoryManager.GetInstance.NewStory(1);
            }
        }

        private void QuestTriggerLoad(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                //메인 퀘스트 자동 추가
                var story = new ST_StoryQuest();
                story.ullStoryKey = 1001;
                story.ullQuestNum = 1;
                //story.ullInteractionCompletes = new List<byte>();
            
                StoryManager.GetInstance.LoadStory(story);
            }
        }


        #endregion
    }
}
#endif