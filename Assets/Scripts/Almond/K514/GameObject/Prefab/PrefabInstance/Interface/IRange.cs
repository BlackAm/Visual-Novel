using UnityEngine;

namespace k514
{
    public interface IVirtualRange
    {
        Collider Collider { get; }
        FloatProperty_Inverse_Sqr Radius { get; }
        FloatProperty Height { get; }
        float PhysicsScaleFactor { get; set; }
    }
}