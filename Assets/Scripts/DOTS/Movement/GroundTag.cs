using Unity.Entities;

/// <summary>
/// Attach this tag to any object that is considered to be the ground, so that it can be used to check if an entity is standing or falling
/// </summary>
[GenerateAuthoringComponent]
public struct GroundTag : IComponentData
{
}
