namespace k514
{
    public static partial class CustomMath
    {
        /// <summary>
        /// x:[0, b] 정의역에 대해서 특정 치역의 값 a를 기준으로 꺾이는 직선 방정식을 기술하는 구조체
        /// 이때 공역은 y:[0, 1]이다.
        /// </summary>
        public struct EdgedLinearEquation
        {
            #region <Fields>

            public float _InverseRatePoint;
            private float _EdgeDomainFactor;
            private float _EdgeYIntercept;

            #endregion
            
            #region <Operator>

            public static implicit operator EdgedLinearEquation(float p_InverseRate)
            {
                var result = new EdgedLinearEquation();
                result.Initialize(p_InverseRate);
                return result;
            }

            #endregion
        
            #region <Methods>

            public void Initialize(float p_InverseRatePoint)
            {
                _InverseRatePoint = p_InverseRatePoint;
                _EdgeDomainFactor = _InverseRatePoint / (_InverseRatePoint - 1f);
                _EdgeYIntercept = _InverseRatePoint - _InverseRatePoint * _EdgeDomainFactor;
            }

            public float GetCodomain(float p_Domain01)
            {
                if (p_Domain01 > _InverseRatePoint)
                {
                    return _EdgeDomainFactor * p_Domain01 + _EdgeYIntercept;
                }
                else
                {
                    return p_Domain01;
                }
            }

            #endregion
        }
    }
}