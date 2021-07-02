using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct CheckEntityOnGroundRaycastComponentData : IComponentData
{
    public float3 GroundDirection;
}


