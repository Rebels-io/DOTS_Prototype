using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

/// <summary>
/// Used to freeze the rotation of a physics body, as the Physics Body seemingly doesn't have a "lock velocity" option yet, so moving objects through physics forces causes the entity to spin/flip
/// </summary>
public class FreezeRotationAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<FreezeRotationComponent>(entity);
        dstManager.SetComponentData(entity, new FreezeRotationComponent() { Value = quaternion.Euler(transform.rotation.eulerAngles)});
    }
}

[UpdateInGroup(typeof(TransformSystemGroup))]
public class FreezeRotationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithName("FreezeRotation")
            .ForEach((ref PhysicsVelocity vel, ref Rotation rot, in FreezeRotationComponent c) =>{
                rot.Value = c;
                vel.Angular = 0;
        }).ScheduleParallel();
    }
}
public struct FreezeRotationComponent : IComponentData
{
    public quaternion Value;

    public static implicit operator quaternion(FreezeRotationComponent v) => v.Value;//Not sure if these implicit operators cause any issues. I've seen none so far, but I also haven't seen anyone else use it.
}
