using System;

/// <summary>
/// ExecuteAfterAttribute, ExecuteBeforeAttribute 와 사용하는 경우, 이쪽이 적용되지 않으며
/// 우선도 order가 작을 수록 먼저 초기화 된다.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
public class ExecutionOrderAttribute : Attribute
{
    public int order;

    public ExecutionOrderAttribute(int order)
    {
        this.order = order;
    }
}

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
public class ExecuteAfterAttribute : Attribute
{
    public Type targetType;
    public int orderIncrease;

    public ExecuteAfterAttribute(Type targetType)
    {
        this.targetType = targetType;
        this.orderIncrease = 10;
    }
}

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
public class ExecuteBeforeAttribute : Attribute
{
    public Type targetType;
    public int orderDecrease;

    public ExecuteBeforeAttribute(Type targetType)
    {
        this.targetType = targetType;
        this.orderDecrease = 10;
    }
}