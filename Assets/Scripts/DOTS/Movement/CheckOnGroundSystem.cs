using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateAfter(typeof(EndFramePhysicsSystem))]

public class CheckOnGroundSystem : SystemBase
{
    //Systems
    BuildPhysicsWorld BuildPhysicsWorldSystem;
    StepPhysicsWorld StepPhysicsWorldSystem;
    EndSimulationEntityCommandBufferSystem ecbSys;
    //Query
    EntityQuery _checkOnGroundGroup;


    protected override void OnCreate()
    {
        _checkOnGroundGroup = GetEntityQuery(new EntityQueryDesc
        {
            None = new ComponentType[]
            {
                typeof(OnGroundComponentTag)
            },
            All = new ComponentType[]
            {
                typeof(CheckEntityOnGroundColliderComponentTag)
            }
        });
        BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
    }
    protected override void OnUpdate()
    {
        if (_checkOnGroundGroup.CalculateEntityCount() == 0)
        {
            return;
        }
        UnityEngine.Debug.Log("Updating CheckOnGroundSystem");
        ecbSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        var entityCommandBuffer = ecbSys
            .CreateCommandBuffer();
        Dependency = new GroundColliderJob
        {
            CheckGroundComponents = GetComponentDataFromEntity<CheckEntityOnGroundColliderComponentTag>(true),
            OnGroundComponents = GetComponentDataFromEntity<OnGroundComponentTag>(true),
            GroundComponents = GetComponentDataFromEntity<GroundTag>(true),
            VelocityComponents = GetComponentDataFromEntity<PhysicsVelocity>(true),
            Buffer = entityCommandBuffer
        }
        .Schedule(StepPhysicsWorldSystem.Simulation, ref BuildPhysicsWorldSystem.PhysicsWorld, Dependency);
        // Make sure that the ECB system knows about our job
        ecbSys.AddJobHandleForProducer(Dependency);
    }

    //[BurstCompile]
    //public struct GroundColliderJob : ICollisionEventsJob
    public struct GroundColliderJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<CheckEntityOnGroundColliderComponentTag> CheckGroundComponents;
        [ReadOnly] public ComponentDataFromEntity<GroundTag> GroundComponents;
        [ReadOnly] public ComponentDataFromEntity<OnGroundComponentTag> OnGroundComponents;
        [ReadOnly] public ComponentDataFromEntity<PhysicsVelocity> VelocityComponents;
        public EntityCommandBuffer Buffer;

        public void Execute(TriggerEvent collEvent)
        //public void Execute(CollisionEvent collEvent)
        {
            //UnityEngine.Debug.Log($"Collision between A: {World.DefaultGameObjectInjectionWorld.EntityManager.GetName(collEvent.EntityA)} and B: {World.DefaultGameObjectInjectionWorld.EntityManager.GetName(collEvent.EntityB)}");
            bool entAIsChecker = CheckGroundComponents.HasComponent(collEvent.EntityA);

            bool entBIsChecker = CheckGroundComponents.HasComponent(collEvent.EntityB);

            bool entAIsGround = GroundComponents.HasComponent(collEvent.EntityA);
            bool entBIsGround = GroundComponents.HasComponent(collEvent.EntityB);
            Entity checkerEntity, groundEntity;

            if (entAIsChecker && entBIsGround)
            {
                checkerEntity = collEvent.EntityA;
                groundEntity = collEvent.EntityB;
            }
            else if (entAIsGround && entBIsChecker)
            {
                checkerEntity = collEvent.EntityB;
                groundEntity = collEvent.EntityA;
            }
            else return;


            //Entity rootEntity = CheckGroundComponents[checkerEntity].RootEntity;
            Entity rootEntity = checkerEntity;

            UnityEngine.Debug.Log($"Collision between checker: {checkerEntity} and ground: {groundEntity}");

             if (!OnGroundComponents.HasComponent(rootEntity))
            {

                if (!VelocityComponents.HasComponent(rootEntity))
                {
                    UnityEngine.Debug.LogError("root entity has no velocity!" + rootEntity);
                    return;
                }
                if (VelocityComponents[rootEntity].Linear.y < 0.01f)
                {
                    UnityEngine.Debug.Log("Falling on platform:" + nameof(OnGroundComponentTag) + " should be added soon");
                    Buffer.AddComponent<OnGroundComponentTag>(rootEntity, new OnGroundComponentTag() { });
                }
            }
            else
            {
                const string message = "Falling on platform(while already on ground)";
                UnityEngine.Debug.Log(message);
            }
        }
    }
}