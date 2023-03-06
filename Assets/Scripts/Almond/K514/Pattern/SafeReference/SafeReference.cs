using System;

namespace k514
{
    public interface ISafeReference<M>
    {
        M CompareKey { get; }
        void SetCompareKey(M p_CompareKey);
    }

    public struct SafeReferenceIntKey<T> where T : ISafeReference<int>
    {
        private int Key;
        private T Value;
        
        public SafeReferenceIntKey(int p_Key, T p_Value)
        {
            Key = p_Key;
            Value = p_Value;
            
            Value.SetCompareKey(Key);
        }

        public (bool, T) GetValue()
        {
            return Key == Value.CompareKey ? (true, Value) : default;
        }
    }
    
    public struct SafeReference<M, T> where T : ISafeReference<M> where M : class
    {
        private M Key;
        private T Value;
        
        public SafeReference(M p_Key, T p_Value)
        {
            Key = p_Key;
            Value = p_Value;
            
            Value.SetCompareKey(Key);
        }

        public (bool, T) GetValue()
        {
            return !ReferenceEquals(null, Key) && ReferenceEquals(Key, Value.CompareKey) ? (true, Value) : default;
        }
    }
    
    public static class SafeReferenceTool
    {
        public static SafeReferenceIntKey<T> GetSafeReference<T>(this int p_Key, T p_Value) where T : ISafeReference<int>
        {
            return new SafeReferenceIntKey<T>(p_Key, p_Value);
        }
        
        public static SafeReference<M, T> GetSafeReference<M, T>(this M p_Key, T p_Value) where T : ISafeReference<M> where M : class
        {
            return new SafeReference<M, T>(p_Key, p_Value);
        }
    }
}