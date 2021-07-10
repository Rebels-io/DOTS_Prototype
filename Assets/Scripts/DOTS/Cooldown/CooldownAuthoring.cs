using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Assets.DOTS.Cooldown
{
    [GenerateAuthoringComponent]
    public struct CooldownComponentData : IComponentData
    {
        public float TotalCooldown;
    }
    //no GenerateAuthoringComponent tag: will only be created during runtime
    public struct RemainingCooldownComponentData : IComponentData
    {
        public float RemainingCooldown;
    }
    public class CooldownSystem : SystemBase
    {
        EntityCommandBufferSystem entityCommandBufferSystem;

        protected override void OnCreate()
        {
            entityCommandBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
            const bool TEST_COOLDOWN = true;
        }

        protected override void OnUpdate()
        {
            var c = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var deltaTime = Time.DeltaTime;
            Entities
                .ForEach((int entityInQueryIndex, Entity e, ref RemainingCooldownComponentData cooldownData)=> 
            {
                cooldownData.RemainingCooldown -= deltaTime;
                if (cooldownData.RemainingCooldown <= 0f)
                {
                    c.RemoveComponent<RemainingCooldownComponentData>(entityInQueryIndex, e);
                }
            }).ScheduleParallel();
            entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
    public class CooldownSystemTest : SystemBase
    {
        Entity cooldownEntity;
        protected override void OnCreate()
        {
            cooldownEntity = EntityManager.CreateEntity(typeof(CooldownComponentData));
            EntityManager.AddComponentData(cooldownEntity, new CooldownComponentData() { TotalCooldown = 4f});
        }
        protected override void OnUpdate()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                if (!EntityManager.HasComponent<RemainingCooldownComponentData>(cooldownEntity))
                {
#if DEBUG_COOLDOWN
                    Debug.Log("Cooldown spent!");
#endif
                    var x = EntityManager.GetComponentData<CooldownComponentData>(cooldownEntity);
                    EntityManager.AddComponentData(cooldownEntity, new RemainingCooldownComponentData() { RemainingCooldown = x.TotalCooldown });
                }
#if DEBUG_COOLDOWN
                else
                {
                    Debug.Log("Remaining cooldown: " + EntityManager.GetComponentData<RemainingCooldownComponentData>(cooldownEntity).RemainingCooldown);
                }
#endif
            }
        }
    }
}
