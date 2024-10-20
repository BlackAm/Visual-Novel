using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 각 씬의 고유 데이터를 기술하는 테이블 클래스, 키 값은 SceneLoader의 씬 인덱스와 일치한다.
    /// 키 값(씬 인덱스) 구성은 다음과 같다.
    /// 
    /// 0~99 - 유니티 빌트 씬 인덱스
    /// 100~ - 씬 인덱스 테이블
    /// 
    /// </summary>
    public class SceneSettingData : GameTable<SceneSettingData, int, SceneSettingData.TableRecord>
    {
        #region <Callbacks>

        #endregion
        
        public class TableRecord : GameTableRecordBase
        {
            #region <Fields>

            /// <summary>
            /// SceneDescription
            /// </summary>
            public string SceneDescription { get; private set; }
            
            /// <summary>
            /// CameraConfigureData 레코드 인덱스
            /// </summary>
            public int CameraConfigureIndex { get; private set; }
            
            /// <summary>
            /// MapVariable 레코드 인덱스 리스트
            /// </summary>
            public List<int> SceneVariableIndexList { get; private set; }
            
            /// <summary>
            /// 절대좌표 원점으로부터 대략적인 맵 크기
            /// </summary>
            public float MapRadiusFromCoordZero { get; private set; }

            #endregion

            #region <Indexer>

            public SceneVariableData.TableRecord this[int p_Index]
            {
                get
                {
                    if (SceneVariableIndexList.CheckGenericCollectionSafe(p_Index))
                    {
                        return SceneVariableData.GetInstanceUnSafe[SceneVariableIndexList[p_Index]];
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            #endregion
            
            #region <Callbacks>

            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();
                
                if (MapRadiusFromCoordZero < CustomMath.Epsilon)
                {
                    MapRadiusFromCoordZero = 256f;
                }
            }

            #endregion

            #region <Methods>

            public override async UniTask SetRecord(int p_Key, object[] p_RecordField)
            {
                await base.SetRecord(p_Key, p_RecordField);
                
                SceneDescription = (string)p_RecordField[0];
                CameraConfigureIndex = (int)p_RecordField[1];
                SceneVariableIndexList = (List<int>)p_RecordField[2];
                MapRadiusFromCoordZero = (float)p_RecordField[3];
            }

            #endregion
        }

        protected override string GetDefaultTableFileName()
        {
            return "SceneSetting";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}