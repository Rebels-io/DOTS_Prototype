//#define DEBUG_TRIGGER
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Defines an entity as being a "PickupTrigger", which will collide with a PickupCollector. Set the physics body to "Raise Trigger", make sure the "Belongs to" and "Collides with" settings are correct
/// </summary>
public class PickupTriggerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public PickupType PickupType;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<PickupTrigger>(entity);
        dstManager.SetComponentData(entity, new PickupTrigger() { type = PickupType });
    }
}

public struct PickupTrigger : IComponentData
{
    public PickupType type;
}
public enum PickupType { RedFruit, Blueberry, TropicalFruit, InstantKill, Other }

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class PickupTriggerSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem _ecbSystem;
    public BuildPhysicsWorld _buildPhysicsWorldSystem;
    public StepPhysicsWorld _stepPhysicsWorldSystem;
    public EntityQuery PickupGroup;

    struct PickupJob : ITriggerEventsJob
    //struct PickupJob : ICollisionEventsJob
    {
        [ReadOnly] ComponentDataFromEntity<PickupTrigger> PickupTriggerComponents;
        ComponentDataFromEntity<PickupCollector> PickupCollectorComponents;

        EntityCommandBuffer processPickupECB;

        public PickupJob(ComponentDataFromEntity<PickupTrigger> pickupTriggerComponents, 
            ComponentDataFromEntity<PickupCollector> pickupCollectorComponents,
            EntityCommandBuffer entityCommandBuffer)
        {
            PickupTriggerComponents = pickupTriggerComponents;
            PickupCollectorComponents = pickupCollectorComponents;
            processPickupECB = entityCommandBuffer;
        }

        //public void Execute(CollisionEvent triggerEvent)
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;
            
            bool entityAIsPickup = PickupTriggerComponents.HasComponent(entityA);
            bool entityAIsCollector = PickupCollectorComponents.HasComponent(entityA);
            bool entityBIsPickup = PickupTriggerComponents.HasComponent(entityB);
            bool entityBIsCollector = PickupCollectorComponents.HasComponent(entityB);

            Entity pickup;
            if (entityAIsPickup && entityBIsCollector)
            {
                pickup = entityA;
            }
            else if (entityAIsCollector && entityBIsPickup)
            {
                pickup = entityB;
            }
            else
            { 
                return;
            }
            Entity collector = pickup == entityA ? entityB : entityA;
#if DEBUG_TRIGGER
            Debug.Log(string.Format("Pickup collision occured.\nPickup: {0} || Collector: {1}!", pickup, collector));
#endif
            PickupTrigger t = PickupTriggerComponents[pickup];
            PickupCollector c = PickupCollectorComponents[collector];
            switch (t.type)
            {
                case PickupType.RedFruit:
                    c.RedPickupsAmt++;
                    break;
                case PickupType.Blueberry:
                    c.BluePickupsAmt++;
                    break;
                case PickupType.TropicalFruit:
                    c.YellowPickupsAmt++;
                    break;
                case PickupType.InstantKill:
                    RestartLevelSystem.Restart();
                    return;
                case PickupType.Other:
                    break;
            }
            PickupCollectorComponents[collector] = c;
            //processPickupECB.SetComponent(collector, c);
            processPickupECB.AddComponent<Disabled>(pickup);
        }
    }

    protected override void OnCreate()
    {
        PickupGroup= GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                typeof(PickupTrigger)
            },
            None = new ComponentType[]
            {
                typeof(PickedUpComponent)
            }
        });
        _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        _buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override void OnUpdate()
    {
        if (PickupGroup.CalculateEntityCount() == 0)
        {
            return;
        }

        Dependency = new PickupJob(
            GetComponentDataFromEntity<PickupTrigger>(true),
            GetComponentDataFromEntity<PickupCollector>(),
            _ecbSystem.CreateCommandBuffer())
        .Schedule(_stepPhysicsWorldSystem.Simulation, ref _buildPhysicsWorldSystem.PhysicsWorld, Dependency);

        // Make sure that the ECB system knows about our job
        _ecbSystem.AddJobHandleForProducer(Dependency);
    }
}

public struct PickedUpComponent : IComponentData
{
    public Entity PickupEntity;
}

