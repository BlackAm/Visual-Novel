using System;
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 여러 테이블이 어떤 키에 대해서 하나의 인덱스를 통해 관리받아야 하는 경우
    /// 해당 인덱싱을 종합하는 베이스 클래스, 해당 클래스의 인스턴스는 테이블 외부에서 관리해야한다.
    /// </summary>
    /// <typeparam name="Key">키 타입</typeparam>
    /// <typeparam name="Label">라벨 타입</typeparam>
    /// <typeparam name="Record">레코드 타입</typeparam>
    public abstract class MultiTableIndexer<Key, Label, Record> where Label : struct
    {
        public Label[] _LabelEnumerator;
        protected Dictionary<Label, IMultiTable<Key, Label, Record>> _MultiTableIndexMap;

        public IMultiTable<Key, Label, Record> this[Label p_LabelType]
        {
            get
            {
                if (_MultiTableIndexMap.TryGetValue(p_LabelType, out var o_Record))
                {
                    return o_Record;
                }
                else
                {
                    return null;
                }
            }
        }

        public Record this[(Label, Key) pt_SuperKey] => _MultiTableIndexMap[pt_SuperKey.Item1].GetTableData(Convert_To_IndexKey(pt_SuperKey));
        public Record this[Label p_LabelType, Key p_Key] => _MultiTableIndexMap[p_LabelType].GetTableData(Convert_To_IndexKey(p_LabelType, p_Key));
        
        public MultiTableIndexer()
        {
            _LabelEnumerator = SystemTool.GetEnumEnumerator<Label>(SystemTool.GetEnumeratorType.GetAll);
            _MultiTableIndexMap = new Dictionary<Label, IMultiTable<Key, Label, Record>>();
        }

        public void JoinTable(IMultiTable<Key, Label, Record> p_Table, Label p_LabelType)
        {
#if UNITY_EDITOR
            try
            {
                _MultiTableIndexMap.Add(p_LabelType, p_Table);
            }
            catch (Exception e)
            {
                Debug.Log($"[{p_Table.GetType().Name}] 테이블의 라벨 [{p_LabelType}] 멀티테이블로 삽입되는 과정에서 중복키 오류가 발생했습니다. 해당 부분 테이블의 라벨 타입이 중복되었는지 확인하십시오. {e.Message}");
                throw;
            }
#else
            _MultiTableIndexMap.Add(p_LabelType, p_Table);
#endif
        }

        public bool HasKey(Key p_Key)
        {
            var (valid, table) = GetLabeledTable(p_Key);
            if (valid)
            {
                return table.HasKey(p_Key);
            }
            else
            {
                return false;
            }
        }

        public bool GetLabel(Key p_Key, out Label o_ResultLabel)
        {
            var (valid, table) = GetLabeledTable(p_Key);
            o_ResultLabel = table.GetThisLabelType();
            return valid;
        }
        
        public (bool, Label) GetLabel(Key p_Key)
        {
            var (valid, table) = GetLabeledTable(p_Key);

            if (valid)
            {
                return (true, table.GetThisLabelType());
            }
            else
            {
                return default;
            }
        }

        public bool GetTableData(Key p_Key, out Record o_ResultLabel)
        {
            var (valid, table) = GetLabeledTable(p_Key);
            o_ResultLabel = table.GetTableData(p_Key);
            return valid;
        }
        
        public Record GetTableData(Key p_Key)
        {
            var (_, table) = GetLabeledTable(p_Key);
            return table.GetTableData(p_Key);
        }
        
        public abstract (bool, IMultiTable<Key, Label, Record> o_ResultLabel) GetLabeledTable(Key p_Key);

        public abstract Key Convert_To_OrdinalKey(Label p_Label, Key p_Key);
        public abstract Key Convert_To_IndexKey((Label, Key) pt_SuperKey);
        public abstract Key Convert_To_IndexKey(Label p_Label, Key p_Key);
    }

    /// <summary>
    /// 정수 키 값을 기준으로 구현된 MultiGameDataIndex 서브클래스
    /// </summary>
    public class IntegerMultiTableIndexer<Label, Record> : MultiTableIndexer<int, Label, Record> where Label : struct
    {
        public override (bool, IMultiTable<int, Label, Record> o_ResultLabel) GetLabeledTable(int p_Key)
        {
            foreach (var kv in _MultiTableIndexMap)
            {
                var indexableGameData = kv.Value;
                if (indexableGameData.StartIndex <= p_Key && p_Key < indexableGameData.EndIndex)
                {
                    return (true, indexableGameData);
                }
            }
            
            return (false, default);
        }

        public override int Convert_To_OrdinalKey(Label p_Label, int p_Key)
        {
            return p_Key - _MultiTableIndexMap[p_Label].StartIndex;
        }

        public override int Convert_To_IndexKey((Label, int) pt_SuperKey)
        {
            return pt_SuperKey.Item2 + _MultiTableIndexMap[pt_SuperKey.Item1].StartIndex;
        }

        public override int Convert_To_IndexKey(Label p_Label, int p_Key)
        {
            return p_Key + _MultiTableIndexMap[p_Label].StartIndex;
        }
    }
}