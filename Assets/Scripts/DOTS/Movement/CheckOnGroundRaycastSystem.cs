//#define DEBUG_RAYCAST
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using Gizmos = UnityEngine.Gizmos;
using Color = UnityEngine.Color;

[UpdateAfter(typeof(EndFramePhysicsSystem))]

public class CheckOnGroundRaycastSystem : SystemBase
{

    //Systems
    //EndSimulationEntityCommandBufferSystem _ecbSys;
    //Query
    EntityQuery _checkOnGroundGroup;

#if DEBUG_RAYCAST
    UnityEngine.Mesh _debugMesh;
    UnityEngine.Material _debugMat;
#endif
    protected override void OnCreate()
    {
        _checkOnGroundGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                typeof(CheckEntityOnGroundRaycastComponentData)
            }
        });
        //_ecbSys = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
#if DEBUG_RAYCAST

        UnityEngine.GameObject gameObject = UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube);
        UnityEngine.MeshFilter meshFilter = gameObject.GetComponent<UnityEngine.MeshFilter>();
        _debugMesh = meshFilter.sharedMesh;
        _debugMat = gameObject.GetComponent<UnityEngine.Renderer>().material;
        UnityEngine.GameObject.Destroy(gameObject);
#endif

    }
    protected override void OnUpdate()
    {
        int entitiesCount = _checkOnGroundGroup.CalculateEntityCount();
        if (entitiesCount == 0)
        {
            //UnityEngine.Debug.LogError("no raycast entities!");
            return;
        }
        //UnityEngine.Debug.Log("Raycast entities count: " + entitiesCount);
        Entities
            .WithNone<OnGroundComponentTag>()
            .ForEach((Entity e, in PhysicsVelocity vel, in Translation t, in CheckEntityOnGroundRaycastComponentData raycastComponentData) =>
            {
                Entity foundEntity = Raycast(t, raycastComponentData);
                if (!foundEntity.Equals(Entity.Null))
                {
                    if (vel.Linear.y < 0.01f)//check if falling or standing
                    {
#if DEBUG_RAYCAST
                        _debugMat.color = Color.green;
                        UnityEngine.Graphics.DrawMesh(_debugMesh, t.Value, UnityEngine.Quaternion.identity, _debugMat, 0);
                        UnityEngine.Debug.LogError("Raycast hit! Entity: " + EntityManager.GetName(e) + "Hit:" + EntityManager.GetName(foundEntity));
#endif
                        EntityManager.AddComponentData(e, new OnGroundComponentTag());
                    }
#if DEBUG_RAYCAST
                    else
                    {
                        _debugMat.color = Color.magenta;
                        UnityEngine.Graphics.DrawMesh(_debugMesh, t.Value, UnityEngine.Quaternion.identity, _debugMat, 0);
                    }
#endif
                }
                else
                {
#if DEBUG_RAYCAST
                    UnityEngine.Debug.LogError("Raycast did not hit. Entity: " + EntityManager.GetName(e));
                    _debugMat.color = Color.red;
                    UnityEngine.Graphics.DrawMesh(_debugMesh, t.Value, UnityEngine.Quaternion.identity, _debugMat, 0);
#endif
                }
            }).WithStructuralChanges().Run();

        Entities
            .WithAll<OnGroundComponentTag>()
            .ForEach((Entity e, in Translation t, in CheckEntityOnGroundRaycastComponentData raycastComponentData) =>
            {
                Entity foundEntity = Raycast(t, raycastComponentData);
                if (foundEntity.Equals(Entity.Null))
                {
                    EntityManager.RemoveComponent<OnGroundComponentTag>(e);
#if DEBUG_RAYCAST
                    UnityEngine.Debug.LogError("Raycast did not hit. Entity: " + EntityManager.GetName(e));
                    _debugMat.color = Color.green;
                    UnityEngine.Graphics.DrawMesh(_debugMesh, t.Value, UnityEngine.Quaternion.identity, _debugMat, 0);
#endif
                }
                else
                {
#if DEBUG_RAYCAST
                    UnityEngine.Debug.LogError("Raycast hit while on ground! Entity: " + EntityManager.GetName(e) + "Hit:" + EntityManager.GetName(foundEntity));
                    _debugMat.color = Color.blue;
                    UnityEngine.Graphics.DrawMesh(_debugMesh, t.Value, UnityEngine.Quaternion.identity, _debugMat, 0);
#endif
                }

            }).WithStructuralChanges().Run();
    }

    private Entity Raycast(Translation t, CheckEntityOnGroundRaycastComponentData raycastComponentData)
    {
        float3 position, direction;
        position = t.Value;
        direction = t.Value + raycastComponentData.GroundDirection;
        Entity foundEntity = Raycast(position, direction);
        return foundEntity;
    }

    Entity Raycast(float3 RayFrom, float3 RayTo)
    {
        var physicsWorldSystem = World.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
        var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;
        RaycastInput input = new RaycastInput()
        {
            Start = RayFrom,
            End = RayTo,
            Filter = new CollisionFilter()
            {
                BelongsTo = ~0u,
                CollidesWith = 1,
                GroupIndex = 0
            }
        };

        bool haveHit = collisionWorld.CastRay(input, out RaycastHit hit);
        if (haveHit)
        {
            // see hit.Position
            // see hit.SurfaceNormal
            Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
            return e;
        }
        return Entity.Null;
    }
}