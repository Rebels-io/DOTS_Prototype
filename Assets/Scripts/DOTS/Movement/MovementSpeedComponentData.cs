using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct MovementSpeedComponentData : IComponentData
{
    public float Speed;

    public MovementSpeedComponentData(float movementSpeed)
    {
        Speed = movementSpeed;
    }

    public static implicit operator float(MovementSpeedComponentData c) => c.Speed;
}