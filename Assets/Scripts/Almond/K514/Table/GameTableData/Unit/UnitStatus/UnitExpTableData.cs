using System.Collections.Generic;

namespace k514
{
    public class UnitExpTableData : GameTable<UnitExpTableData, int, UnitExpTableData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            /// <summary>
            /// 현제 레벨 이상일 시 다음 단계 성장
            /// </summary>
            public List<int> LVC { private set; get; }

            /// <summary>
            /// 다음 레벨에 필요한 경험치 요구량 ,레벨 구간 별 경험치 요구량 증가값
            /// </summary>
            public List<float> EXP{ get; private set; }

            public float AddExperience(float p_Left,float p_Right, ref float addExpValue){
                float p_Total = 0;
                p_Total = p_Left * p_Right;
                addExpValue += p_Total;
                return p_Total;
            }
        }
/*
        /// <summary>
        /// 상대 플레이어 및 몹을 죽일 시 경험치 획득량 (ExpAdditiveList에 해당하는 키만 참조가능)
        /// </summary>
        public float AddNeedExp(int p_SeedLevel, int p_Index){
        
            var targetRecord = GetTableData(p_Index);
            var seed = p_SeedLevel / 10;
            var expScale = targetRecord.EXP.Count - 2;
            var remaind = p_SeedLevel % 10;
            if (seed > expScale)
            {
                var weight = seed - expScale;
                var targetExpAdditive = ((seed < targetRecord.EXP.Count)) ? targetRecord.EXP[seed] : targetRecord.EXP[targetRecord.EXP.Count - 1];
                var result = targetExpAdditive * (1f + 0.1f * weight + 0.01f * remaind);
                return (int) result;
            }
            else
            {
                var weight = remaind * 0.1f;
                var targetExpAdditive = targetRecord.EXP[seed];
                var nextExpAdditive = ((seed + 1) < (targetRecord.EXP.Count)) ? targetRecord.EXP[seed + 1] : targetRecord.EXP[targetRecord.EXP.Count - 1];
                var result = targetExpAdditive * (1f - weight) + nextExpAdditive * weight;
                return (int) result;
            }
            if(p_Index.Equals(0)) return default;
        }
*/
        /// <summary>
        /// 경험치 요구량 불러오기 및 변경
        /// </summary>
        public float GetNeedExp(int p_SeedLevel, int p_Key, int p_GrowKey)
        {
            var targetRecord = GetTableData(p_Key);
            GrowExp(p_GrowKey, out float[] GrowExpLvc, out float[] GrowOutExps);
            int expX = GrowExpLvc.Length;
            int[] expY = new int[expX];
            //경험치
            for (int i = p_SeedLevel; i > 1; i--)
            {
                for (int j = expX; j > 0; j--)
                {
                    if(GrowExpLvc[j - 1] <= i){
                        expY[j - 1]++;
                        break;
                    }
                }
            }
            float expValue = targetRecord.EXP[0];
            float addExpValue = 0;
            for (int k = 0; k < expY.Length; k++)
            {
                if(!expY[k].Equals(0)){
                    for (int l = 0; l < expY[k]; l++)
                    {
                        expValue += targetRecord.AddExperience(expValue , GrowOutExps[k], ref addExpValue);
                    }
                }
            }
            return addExpValue;
        }

        /// <summary>
        /// 레벨업 시 다음 경험치 요구량을 불러오게 된다.
        /// </summary>
        public float NextLevelNeedExp(int p_SeedLevel, int p_Key, int p_GrowKey, float expValue)
        {
            var targetRecord = GetTableData(p_Key);
            GrowExp(p_GrowKey, out float[] GrowExpLvc, out float[] GrowOutExps);
            int expX = GrowExpLvc.Length;
            int[] expY = new int[expX];
                //경험치
                for (int j = expX; j > 0; j--)
                {
                    if(GrowExpLvc[j - 1] <= p_SeedLevel){
                        expY[j - 1]++;
                        break;
                    }
                }
            float addExpValue = 0;
            for (int k = 0; k < expY.Length; k++)
            {
                if(!expY[k].Equals(0)){
                    for (int l = 0; l < expY[k]; l++)
                    {
                        expValue += targetRecord.AddExperience(expValue , GrowOutExps[k], ref addExpValue);
                    }
                }
            }
            return addExpValue;
        }

        /// <summary>
        /// 경험치 요구량 가져오기
        /// </summary>
        public void GetExp(int p_Key, out float exp)
        {
            var targetRecord = GetTableData(p_Key);
            exp = targetRecord.EXP[0];
        }
        /// <summary>
        /// 레벨마다 증가하는 경험치 요구량 가져오기
        /// </summary>
        public void GrowExp(int p_Key, out float[] lvc, out float[] exp)
        {
            var targetRecord = GetTableData(p_Key);
            lvc = new float[targetRecord.LVC.Count];
            exp = new float[targetRecord.LVC.Count];
            for (int i = 0; i < lvc.Length; i++)
            {
                lvc[i] = targetRecord.LVC[i];
                exp[i] = targetRecord.EXP[i];
            }
        }

        protected override string GetDefaultTableFileName()
        {
            return "UnitExpTable";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}