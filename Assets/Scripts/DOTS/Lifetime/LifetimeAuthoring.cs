//From: https://docs.unity3d.com/Packages/com.unity.entities@0.17/manual/entity_command_buffer.html
using System;
using Unity.Entities;
using UnityEngine;
/// <summary>
/// This class is redundant; it could easily be replaced by using [GenerateAuthoringComponent] on the class Lifetime. This would be the class it would generate, with the only difference being the name of the variable in the inspector.
/// </summary>
public class LifetimeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] float TotalLifeTime;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Lifetime(TotalLifeTime));
    }
}
public struct Lifetime : IComponentData
{
    public float _value;

    public Lifetime(float totalLifeTime)
    {
        _value = totalLifeTime;
    }

    public static implicit operator float(Lifetime l) => l._value;
    public static Lifetime operator -(Lifetime l, float f) 
    { 
        l._value -= f; return l; 
    }
}

partial class LifetimeSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate()
    {
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
        float deltaTime = Time.DeltaTime;
        Entities
            .WithName("LifeTime_Update")
            .ForEach((Entity entity, int entityInQueryIndex, ref Lifetime lifetime) =>
            {
                if (lifetime <= 0)
                {
                    ecb.DestroyEntity(entityInQueryIndex, entity);
                }
                else
                {
                    lifetime -= deltaTime;
                }
            }).ScheduleParallel();
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
    }
}
////From: https://docs.unity3d.com/Packages/com.unity.entities@0.17/manual/entity_command_buffer.html
//using System;
//using Unity.Entities;
//using UnityEngine;

//public class LifetimeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
//{
//    [SerializeField] float TotalLifeTime;
//    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//    {
//        dstManager.AddComponentData(entity, new Lifetime(TotalLifeTime));
//    }
//}
//public struct Lifetime : IComponentData
//{
//    float _value;

//    public Lifetime(float totalLifeTime) : this()
//    {
//        _value = totalLifeTime;
//    }

//    public static implicit operator float(Lifetime l) => l._value;
//    public static Lifetime operator -(Lifetime l, float f) { l._value -= f; return l; }
//    public void Update(float deltaTime) => _value -= deltaTime;
//}

//partial class LifetimeSystem : SystemBase
//{
//    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
//    protected override void OnCreate()
//    {
//        base.OnCreate();
//        // Find the ECB system once and store it for later usage
//        m_EndSimulationEcbSystem = World
//            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
//    }

//    protected override void OnUpdate()
//    {
//        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
//        float deltaTime = Time.DeltaTime;
//        Entities
//            .WithName("LifeTime_Update")
//            .ForEach((Entity entity, int entityInQueryIndex, ref Lifetime lifetime) =>
//            {
//                if (lifetime == 0)
//                {
//                    ecb.DestroyEntity(entityInQueryIndex, entity);
//                }
//                else
//                {
//                    lifetime -= deltaTime;
//                }
//            }).ScheduleParallel();
//        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
//    }
//}