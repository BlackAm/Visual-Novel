namespace k514
{
    /// <summary>
    /// 1차 배열을 논리적으로 2차 삼각 배열로 다루게 하는 프리셋
    /// 삼각배열은 아래쪽으로 넓은 직각 삼각형 꼴을 하고 있다.
    /// </summary>
    public struct TriangleArray<T>
    {
        #region <Fields>

        private int Scale;
        private T[] Array;

        #endregion

        #region <Constructor>

        public TriangleArray(int p_Scale)
        {
            Scale = p_Scale;
            var length = Scale * (Scale + 1) / 2;
            Array = new T[length];
        }

        #endregion

        #region <Operator>

#if UNITY_EDITOR
        public override string ToString()
        {
            var result = string.Empty;
            for (int i = 0; i <= Scale; i++)
            {
                for (int j = 0; j <= Scale; j++)
                {
                    var valid = j < i && IsValid(i, j);
                    if (valid)
                    {
                        var tryElement = GetElement(j, i);
                        result += $"[{tryElement}]  ";
                    }
                    else
                    {
                        result += $"[X]  ";
                    }
                }
                result += "\n";
            }

            return result;
        }
#endif

        #endregion
        
        #region <Methods>

        public void SetElement(int p_Lower, int p_Upper, T p_Element)
        {
            var index = (p_Upper * (p_Upper - 1) / 2) + p_Lower;
            Array[index] = p_Element;
        }
        
        public void SetElement_DiagonalSafe(int p_Row, int p_Column, T p_Element)
        {
            if (p_Row == p_Column)
            {
            }
            else
            {
                var index = (p_Column * (p_Column - 1) / 2) + p_Row;
                Array[index] = p_Element;
            }
        }
        
        public void SetElement_AscendantSafe(int p_Row, int p_Column, T p_Element)
        {
            var (lower, upper) = SystemTool.SortIndexAscendant(p_Row, p_Column);
            var index = (upper * (upper - 1) / 2) + lower;
            Array[index] = p_Element;
        }

        public void SetElement_SafeComplete(int p_Row, int p_Column, T p_Element)
        {
            if (p_Row == p_Column)
            {
            }
            else
            {
                var (lower, upper) = SystemTool.SortIndexAscendant(p_Row, p_Column);
                if (lower < 0 || upper > Scale)
                {
                }
                else
                {
                    var index = (upper * (upper - 1) / 2) + lower;
                    Array[index] = p_Element;
                }
            }
        }

        public T GetElement(int p_Lower, int p_Upper)
        {
            var index = (p_Upper * (p_Upper - 1) / 2) + p_Lower;
            return Array[index];
        }
        
        public T GetElement_DiagonalSafe(int p_Lower, int p_Upper)
        {
            if (p_Lower == p_Upper)
            {
                return default;
            }
            else
            {
                var index = (p_Upper * (p_Upper - 1) / 2) + p_Lower;
                return Array[index];
            }
        }
        
        public T GetElement_AscendantSafe(int p_Row, int p_Column)
        {
            var (lower, upper) = SystemTool.SortIndexAscendant(p_Row, p_Column);
            var index = (upper * (upper - 1) / 2) + lower;
            return Array[index];
        }

        public T GetElement_SafeComplete(int p_Row, int p_Column)
        {
            if (p_Row == p_Column)
            {
                return default;
            }
            else
            {
                var (lower, upper) = SystemTool.SortIndexAscendant(p_Row, p_Column);
                if (lower < 0 || upper > Scale)
                {
                    return default;
                }
                else
                {
                    var index = (upper * (upper - 1) / 2) + lower;
                    return Array[index];
                }
            }
        }
        
        public bool IsValid(int p_Row, int p_Column)
        {
            if (p_Row == p_Column)
            {
                return false;
            }
            else
            {
                var (lower, upper) = SystemTool.SortIndexAscendant(p_Row, p_Column);
                if (lower < 0 || upper > Scale)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public (bool t_Result, int t_Lower, int t_Upper) TryGetIndexSequence(int p_Row, int p_Column)
        {
            if (p_Row == p_Column)
            {
                return default;
            }
            else
            {
                var (lower, upper) = SystemTool.SortIndexAscendant(p_Row, p_Column);
                if (lower < 0 || upper > Scale)
                {
                    return default;
                }
                else
                {
                    return (true, lower, upper);
                }
            }
        }
        
        #endregion
    }
}