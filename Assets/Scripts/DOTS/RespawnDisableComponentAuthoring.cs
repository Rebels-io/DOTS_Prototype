using Unity.Entities;
using UnityEngine;
/// <summary>
/// Use this component to respawn an object if it gets disabled (by using the Disabled component). You will still need to reset any values if necessary like position/health/etc
/// </summary>
public class RespawnDisableComponentAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    [Header("When disabling this object, it gets re-enabled after a chosen amount of seconds.")]
    public float TotalSecondsToRespawn = 20f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (TotalSecondsToRespawn <= 0 )
        {
            Debug.LogError("Entity has a RespawnDisableComponent, but the total time to respawn is 0 or lower.", gameObject);
        }
        dstManager.AddComponentData(entity, new RespawnDisableComponent(TotalSecondsToRespawn));
        dstManager.AddComponentData(entity, new RespawnTime(TotalSecondsToRespawn));
    }
}
public struct RespawnTime : IComponentData
{
    public float TotalSecondsToRespawn;

    public RespawnTime(float totalSecondsToRespawn)
    {
        TotalSecondsToRespawn = totalSecondsToRespawn;
    }
}
public struct RespawnDisableComponent : IComponentData
{
    public float SecondsToRespawn;

    public RespawnDisableComponent(float totalSecondsToRespawn)
    {
        SecondsToRespawn = totalSecondsToRespawn;
    }
}
/// <summary>
/// Disables entities with RespawnDisableComponent.
/// </summary>
public class RespawnDisableSystem : SystemBase 
{ 
    EntityCommandBufferSystem _entityCommandBufferSystem;
    protected override void OnUpdate()
    {
        if (_entityCommandBufferSystem == null)
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
        }
        var ecb = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var deltaTime = Time.DeltaTime;
        Entities
            .WithAll<Disabled>()
            .ForEach((Entity e, int entityInQueryIndex, ref RespawnDisableComponent disableComponent, in RespawnTime respawnTime)=> 
            {
                float remainingSeconds = disableComponent.SecondsToRespawn;
                remainingSeconds -= deltaTime;
                disableComponent.SecondsToRespawn = remainingSeconds;
                if (remainingSeconds <= 0f)
                {
                    ecb.RemoveComponent<Disabled>(entityInQueryIndex, e);
                    disableComponent.SecondsToRespawn = respawnTime.TotalSecondsToRespawn;
                }
            })
            .ScheduleParallel();

        // Make sure that the ECB system knows about our job: if you forget this you get a whole bunch of exceptions that don't really tell you how to fix it: for your own sanity, remember to add this line any time you need an ECB
        _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
    }
}