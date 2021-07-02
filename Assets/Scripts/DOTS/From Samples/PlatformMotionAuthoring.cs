//This code has been copied from the ECS Samples https://github.com/Unity-Technologies/EntityComponentSystemSamples/tree/master/ECSSamples

using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public struct PlatformMotion : IComponentData
{
    public float CurrentTime;
    public float3 InitialPosition;
    public float Distance;
    public float Speed;
    public float3 Direction;
    public float3 Rotation;
}

public class PlatformMotionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float Distance = 1f;
    public float Speed = 1f;
    public float3 Direction = math.up();
    public float3 Rotation = float3.zero;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PlatformMotion
        {
            InitialPosition = transform.position,
            Distance = Distance,
            Speed = Speed,
            Direction = math.normalizesafe(Direction),
            Rotation = Rotation,
        });
    }
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(BuildPhysicsWorld))]
public class PlatformMotionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        Entities
            .WithName("MovePlatforms")
            .WithBurst()
            .ForEach((ref PlatformMotion motion, ref PhysicsVelocity velocity, in Translation position) =>
            {
                motion.CurrentTime += deltaTime;

                var desiredOffset = motion.Distance * math.sin(motion.CurrentTime * motion.Speed);
                var currentOffset = math.dot(position.Value - motion.InitialPosition, motion.Direction);
                velocity.Linear = motion.Direction * (desiredOffset - currentOffset);

                velocity.Angular = motion.Rotation;
            }).ScheduleParallel();
    }
}
