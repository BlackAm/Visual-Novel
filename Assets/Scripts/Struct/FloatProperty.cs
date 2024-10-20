using UnityEngine;

namespace BlackAm
{
    public struct FloatProperty
    {
        #region <Fields>

        /// <summary>
        /// 현재 프로퍼티 접근 값
        /// </summary>
        public float CurrentValue => _BackingField_CurrentValue;

        /// <summary>
        /// 명시적 프로퍼티 필드
        /// </summary>
        private float _BackingField_CurrentValue;
        
        /// <summary>
        /// 기본 값
        /// </summary>
        public float _DefaultValue;

        /// <summary>
        /// 기본 오프셋
        /// </summary>
        public float _DefaultOffset;
                      
        /// <summary>
        /// 스케일 값
        /// </summary>
        public float _ScaleFactor;
        
        #endregion

        #region <Constructor>

        public FloatProperty(float p_DefaultValue, float p_DefaultOffset)
        {
            _DefaultValue = p_DefaultValue;
            _DefaultOffset = p_DefaultOffset;
            _BackingField_CurrentValue = _DefaultValue + _DefaultOffset;
            _ScaleFactor = 1f;
        }

        #endregion

        #region <Methods>

        public FloatProperty ApplyScale(float p_ScaleFactor)
        {
            _ScaleFactor = p_ScaleFactor;
            _BackingField_CurrentValue = _DefaultValue * _ScaleFactor + _DefaultOffset;

            return this;
        }

        #endregion
    }
    
    public struct FloatProperty_Inverse
    {
        #region <Fields>

        /// <summary>
        /// 현재 프로퍼티 접근 값
        /// </summary>
        public float CurrentValue => _BackingField_CurrentValue;

        /// <summary>
        /// 현재 프로퍼티 접근 값 역수
        /// </summary>
        public float CurrentInverseValue => _BackingField_CurrentInverseValue;
        
        /// <summary>
        /// 명시적 프로퍼티 필드
        /// </summary>
        private float _BackingField_CurrentValue;
        
        /// <summary>
        /// 명시적 프로퍼티 필드 역수
        /// </summary>
        private float _BackingField_CurrentInverseValue;
        
        /// <summary>
        /// 기본 값
        /// </summary>
        public float _DefaultValue;
        
        /// <summary>
        /// 기본 오프셋
        /// </summary>
        private float _DefaultOffset;
              
        /// <summary>
        /// 스케일 값
        /// </summary>
        public float _ScaleFactor;

        #endregion

        #region <Constructor>

        public FloatProperty_Inverse(float p_DefaultValue, float p_DefaultOffset)
        {
            _DefaultValue = p_DefaultValue;
            _DefaultOffset = p_DefaultOffset;
            _BackingField_CurrentValue = _DefaultValue + _DefaultOffset;
            _BackingField_CurrentInverseValue = 1f / _BackingField_CurrentValue;
            _ScaleFactor = 1f;
        }

        #endregion

        #region <Methods>

        public FloatProperty_Inverse ApplyScale(float p_ScaleFactor)
        {
            _ScaleFactor = p_ScaleFactor;
            _BackingField_CurrentValue = _DefaultValue * _ScaleFactor + _DefaultOffset;
            _BackingField_CurrentInverseValue = 1f / _BackingField_CurrentValue;

            return this;
        }

        #endregion
    }
   
    public struct FloatProperty_Sqr
    {
        #region <Fields>

        /// <summary>
        /// 현재 프로퍼티 접근 값
        /// </summary>
        public float CurrentValue => _BackingField_CurrentValue;

        /// <summary>
        /// 현재 프로퍼티 접근 값 제곱
        /// </summary>
        public float CurrentSqrValue => _BackingField_CurrentSqrValue;
        
        /// <summary>
        /// 명시적 프로퍼티 필드
        /// </summary>
        private float _BackingField_CurrentValue;
        
        /// <summary>
        /// 명시적 프로퍼티 필드 제곱
        /// </summary>
        private float _BackingField_CurrentSqrValue;
        
        /// <summary>
        /// 기본 값
        /// </summary>
        public float _DefaultValue;
        
        /// <summary>
        /// 기본 오프셋
        /// </summary>
        private float _DefaultOffset;
                              
        /// <summary>
        /// 스케일 값
        /// </summary>
        public float _ScaleFactor;
        
        #endregion

        #region <Constructor>

        public FloatProperty_Sqr(float p_DefaultValue, float p_DefaultOffset)
        {
            _DefaultValue = p_DefaultValue;
            _DefaultOffset = p_DefaultOffset;
            _BackingField_CurrentValue = _DefaultValue + _DefaultOffset;
            _BackingField_CurrentSqrValue = _BackingField_CurrentValue * _BackingField_CurrentValue;
            _ScaleFactor = 1f;
        }

        #endregion

        #region <Methods>

        public FloatProperty_Sqr ApplyScale(float p_ScaleFactor)
        {
            _ScaleFactor = p_ScaleFactor;
            _BackingField_CurrentValue = _DefaultValue * _ScaleFactor + _DefaultOffset;
            _BackingField_CurrentSqrValue = _BackingField_CurrentValue * _BackingField_CurrentValue;

            return this;
        }

        #endregion
    }
    
    public struct FloatProperty_Inverse_Sqr
    {
        #region <Fields>

        /// <summary>
        /// 현재 프로퍼티 접근 값
        /// </summary>
        public float CurrentValue => _BackingField_CurrentValue;

        /// <summary>
        /// 현재 프로퍼티 접근 값 제곱
        /// </summary>
        public float CurrentSqrValue => _BackingField_CurrentSqrValue;
        
        /// <summary>
        /// 현재 프로퍼티 접근 값 역수
        /// </summary>
        public float CurrentInverseValue => _BackingField_CurrentInverseValue;
        
        /// <summary>
        /// 명시적 프로퍼티 필드
        /// </summary>
        private float _BackingField_CurrentValue;
        
        /// <summary>
        /// 명시적 프로퍼티 필드 제곱
        /// </summary>
        private float _BackingField_CurrentSqrValue;
        
        /// <summary>
        /// 명시적 프로퍼티 필드 역수
        /// </summary>
        private float _BackingField_CurrentInverseValue;
        
        /// <summary>
        /// 기본 값
        /// </summary>
        public float _DefaultValue;
        
        /// <summary>
        /// 기본 오프셋
        /// </summary>
        public float _DefaultOffset;
                              
        /// <summary>
        /// 스케일 값
        /// </summary>
        public float _ScaleFactor;
        
        #endregion

        #region <Constructor>

        public FloatProperty_Inverse_Sqr(float p_DefaultValue, float p_DefaultOffset)
        {
            _DefaultValue = p_DefaultValue;
            _DefaultOffset = p_DefaultOffset;
            _BackingField_CurrentValue = _DefaultValue + _DefaultOffset;
            _BackingField_CurrentSqrValue = _BackingField_CurrentValue * _BackingField_CurrentValue;
            _BackingField_CurrentInverseValue = 1f / _BackingField_CurrentValue;
            _ScaleFactor = 1f;
        }

        #endregion

        #region <Methods>

        public FloatProperty_Inverse_Sqr ApplyScale(float p_ScaleFactor)
        {
            _ScaleFactor = p_ScaleFactor;
            _BackingField_CurrentValue = _DefaultValue * _ScaleFactor + _DefaultOffset;
            _BackingField_CurrentSqrValue = _BackingField_CurrentValue * _BackingField_CurrentValue;
            _BackingField_CurrentInverseValue = 1f / _BackingField_CurrentValue;

            return this;
        }

        #endregion
    }
    
        public struct FloatProperty_Inverse_Sqr_Sqrt
    {
        #region <Fields>

        /// <summary>
        /// 현재 프로퍼티 접근 값
        /// </summary>
        public float CurrentValue => _BackingField_CurrentValue;
        
        /// <summary>
        /// 현재 프로퍼티 접근 값 역수
        /// </summary>
        public float CurrentInverseValue => _BackingField_CurrentInverseValue;

        /// <summary>
        /// 현재 프로퍼티 접근 값 제곱
        /// </summary>
        public float CurrentSqrValue => _BackingField_CurrentSqrValue;
                        
        /// <summary>
        /// 현재 프로퍼티 접근 값 제곱 역수
        /// </summary>
        public float CurrentInverseSqrValue => _BackingField_CurrentInverseSqrValue;
        
        /// <summary>
        /// 현재 프로퍼티 접근 값 제곱근
        /// </summary>
        public float CurrentSqrtValue => _BackingField_CurrentSqrtValue;
                                
        /// <summary>
        /// 현재 프로퍼티 접근 값 제곱근 역수
        /// </summary>
        public float CurrentInverseSqrtValue => _BackingField_CurrentInverseSqrtValue;
        
        /// <summary>
        /// 명시적 프로퍼티 필드
        /// </summary>
        private float _BackingField_CurrentValue;
        
        /// <summary>
        /// 명시적 프로퍼티 필드 역수
        /// </summary>
        private float _BackingField_CurrentInverseValue;
        
        /// <summary>
        /// 명시적 프로퍼티 필드 제곱
        /// </summary>
        private float _BackingField_CurrentSqrValue;
                
        /// <summary>
        /// 명시적 프로퍼티 필드 제곱 역수
        /// </summary>
        private float _BackingField_CurrentInverseSqrValue;        
        
        /// <summary>
        /// 명시적 프로퍼티 필드 제곱근
        /// </summary>
        private float _BackingField_CurrentSqrtValue;
                
        /// <summary>
        /// 명시적 프로퍼티 필드 제곱근 역수
        /// </summary>
        private float _BackingField_CurrentInverseSqrtValue;
        
        /// <summary>
        /// 기본 값
        /// </summary>
        public float _DefaultValue;
        
        /// <summary>
        /// 기본 오프셋
        /// </summary>
        public float _DefaultOffset;
                              
        /// <summary>
        /// 스케일 값
        /// </summary>
        public float _ScaleFactor;
        
        #endregion

        #region <Constructor>

        public FloatProperty_Inverse_Sqr_Sqrt(float p_DefaultValue, float p_DefaultOffset)
        {
            _DefaultValue = p_DefaultValue;
            _DefaultOffset = p_DefaultOffset;
            _BackingField_CurrentValue = _DefaultValue + _DefaultOffset;
            _BackingField_CurrentInverseValue = 1f / _BackingField_CurrentValue;
            _BackingField_CurrentSqrValue = _BackingField_CurrentValue * _BackingField_CurrentValue;
            _BackingField_CurrentInverseSqrValue = 1f / _BackingField_CurrentSqrValue;
            _BackingField_CurrentSqrtValue = Mathf.Sqrt(_BackingField_CurrentValue);
            _BackingField_CurrentInverseSqrtValue = 1f / _BackingField_CurrentSqrtValue;
            _ScaleFactor = 1f;
        }

        #endregion

        #region <Methods>

        public FloatProperty_Inverse_Sqr_Sqrt ApplyScale(float p_ScaleFactor)
        {
            _ScaleFactor = p_ScaleFactor;
            _BackingField_CurrentValue = _DefaultValue * _ScaleFactor + _DefaultOffset;
            _BackingField_CurrentInverseValue = 1f / _BackingField_CurrentValue;
            _BackingField_CurrentSqrValue = _BackingField_CurrentValue * _BackingField_CurrentValue;
            _BackingField_CurrentInverseSqrValue = 1f / _BackingField_CurrentSqrValue;
            _BackingField_CurrentSqrtValue = Mathf.Sqrt(_BackingField_CurrentValue);
            _BackingField_CurrentInverseSqrtValue = 1f / _BackingField_CurrentSqrtValue;
            
            return this;
        }

        #endregion
    }
}