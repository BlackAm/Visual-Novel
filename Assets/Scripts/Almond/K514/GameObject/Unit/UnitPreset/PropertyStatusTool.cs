namespace k514
{
    public static class PropertyStatusTool
    {
        #region <Enums>

        public enum PropertyValueType
        {
            /// <summary>
            /// 고정값
            /// </summary>
            FixedValue, 

            /// <summary>
            /// 퍼센트값
            /// </summary>
            RateValue, 
            
            /// <summary>
            /// 배수값
            /// </summary>
            RateMultiplyValue
        }

        #endregion
    }
}