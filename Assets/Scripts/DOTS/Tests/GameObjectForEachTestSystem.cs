using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GameObjectForEachTestSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Debug.Log("Running GameObjectForEachTestSystem");

        //Needs "Convert and Inject", or EntityManager.AddComponentObject(entity, GameObjectComponent)
        //Have to use .WithoutBurst() and .Run() to use managed (object) components.
        Entities
            .WithoutBurst()
            .ForEach((GameObjectTestBehaviour b) =>
            {
                b.Val++;
                Debug.Log("Running GameObjectForEachTestSystem: ForEach(Main Thread)");
            }).Run();

        //When iterating over normal (entity) components, you can still use Debug.Log as long as you use .WithoutBurst(). 
        //This works even when not on the main thread, so there is no need to use .Run() instead of .Schedule() or .ScheduleParallel
        Entities
            .ForEach((ref TestComponent t) =>
            {
                t.Val++;
                Debug.Log("Running GameObjectForEachTestSystem: ForEach(Entity)");
            })
            .WithoutBurst()
            .Schedule();

        Entities
            .ForEach((ref TestComponent t) =>
            {
                t.Val = math.mul(t.Val, 2);
                //Debug.Log("Running GameObjectForEachTestSystem: ForEach(Main thread + Burst: Debugging disabled!)");
            }).Run();
    }
}