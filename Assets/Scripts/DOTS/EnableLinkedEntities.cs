using Unity.Entities;
using Unity.Transforms;
/// <summary>
/// Add to any component in your scene to make it possible to delete/disable an entire parent-child hierarchy using the method EntityManager.
/// </summary>
[GenerateAuthoringComponent]
public struct EnableLinkedEntities : IComponentData { };

public class LinkEntitySystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem _ecbSys;
    bool enabled;
    protected override void OnCreate()
    {
        _ecbSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        EntityQuery entityQuery = GetEntityQuery(ComponentType.ReadOnly<EnableLinkedEntities>());
        if (!entityQuery.IsEmpty)
        {
            enabled = true;
        }
    }
    protected override void OnUpdate()
    {
        if (!enabled) return;
        EntityCommandBuffer ecb = _ecbSys.CreateCommandBuffer();
        Entities.WithNone<LinkedEntityGroup>().ForEach((in Entity e, in DynamicBuffer<Child> c) => {
            //link entities that have children as a LinkedEntityGroup so that disabling/deleting the parents also delete the children
            var group = ecb.AddBuffer<LinkedEntityGroup>(e);
            foreach (var child in c)
            {
                group.Add(child.Value);
            }
        }).Schedule();
    }
}
