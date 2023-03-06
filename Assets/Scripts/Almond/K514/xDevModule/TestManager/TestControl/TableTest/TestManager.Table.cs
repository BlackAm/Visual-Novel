#if UNITY_EDITOR && ON_GUI
using System.Linq;
using UnityEngine;

namespace k514
{
    public partial class TestManager
    {
        #region <Fields>

        #endregion

        #region <Callbacks>

        void OnAwakeTable()
        {
            var targetControlType = TestControlType.TableTest;
            
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.Q, GetTableRecordNumber, "레코드 갯수", $"래코드 갯수를 출력");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.W, AddRecord, "레코드 추가", "더미 데이터를 테이블에 추가");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.E, UpdateTable, "테이블 파일 업데이트", "현재 테이블 상태를 저장");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.R, CopyTable, "테이블 파일 복사", "테이블 복사 파일을 생성하고 저장");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.T, BackUpTable, "테이블 파일 백업", "백업 경로에 테이블 저장");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.A, CheckValue, "레코드 값 확인", "TestGameData:Key:0.Record.Value2:int 확인");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.S, ReloadOriginTable, "테이블 리로드", "Test.xml 디코딩");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.D, ReloadZweiTable, "별명으로 리로드", "Test2.xml 디코딩");
        }

        #endregion

        #region <Methods>

        private void GetTableRecordNumber(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                Debug.Log(TestGameData.GetInstanceUnSafe.GetTable().Count);
            }
        }
        
        private void AddRecord(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                var targetKey = TestGameData.GetInstanceUnSafe.GetTable().Keys.Max() + 1;
                var recordVal = Random.Range(0, 100);
                TestGameData.GetInstanceUnSafe.AddRecord(targetKey, recordVal);
                TestGameData.GetInstanceUnSafe.PrintTable();
            }
        }
        
        private void UpdateTable(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                TestGameData.GetInstanceUnSafe.UpdateTableFile(ExportDataTool.WriteType.Overlap);
            }
        }
        
        private void CopyTable(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                TestGameData.GetInstanceUnSafe.UpdateTableFile(ExportDataTool.WriteType.CopyWithNumbering);
            }
        }
        
        private void BackUpTable(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                TestGameData.GetInstanceUnSafe.UpdateTableFile(ExportDataTool.WriteType.BackUp);
            }
        }

        private void CheckValue(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                Debug.Log(TestGameData.GetInstanceUnSafe[0].value2);
            }
        }

        private void ReloadOriginTable(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                TestGameData.GetInstanceUnSafe.ReloadTable(null);
            }
        }
        
        private void ReloadZweiTable(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                TestGameData.GetInstanceUnSafe.ReloadTable("Test2");
            }
        }
        
        #endregion
    }
}
#endif