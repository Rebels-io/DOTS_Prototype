using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
/// <summary>
/// This component will only be added at runtime, and will indicate whether or not an entity is currently on the ground (meaning: an entity with a GroundTag)
/// </summary>
[Serializable]
public struct OnGroundComponentTag : IComponentData{}
