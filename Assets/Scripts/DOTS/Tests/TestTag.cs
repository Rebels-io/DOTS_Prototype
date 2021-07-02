using Unity.Entities;

[GenerateAuthoringComponent]
public struct TestComponent : IComponentData
{
    public int Val;
}