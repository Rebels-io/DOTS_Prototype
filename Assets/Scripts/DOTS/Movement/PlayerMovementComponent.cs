using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using UnityEngine;

namespace Movement
{
    /// <summary>
    /// Add this component to set up a player that is able to move.
    /// This is a good example of a custom authoring component: the player will need both components for its system to function properly. If left as seperate authoring components, a developer might forget to add one of these components and will have to do debugging to find out why the system isn't running. Creating a single authoring component for multiple runtime data components is a good way to prevent this issue.
    /// </summary>
    public class PlayerMovementComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float MovementSpeed;
        public float JumpForce;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new JumpForceComponent(JumpForce));
            dstManager.AddComponentData(entity, new MovementSpeedComponentData(MovementSpeed));

        }
    }
    public class InputToVelocitySystem : SystemBase
    {
        [BurstCompile]
        protected override void OnUpdate()
        {
            var input = new float3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            bool jump = Input.GetButtonDown("Jump");

            Entities
                .WithAll<OnGroundComponentTag>()
                .WithStructuralChanges()//removing ground tag; should be done with an EntityCommandBuffer if done for multiple objects
                .ForEach((Entity e, ref PhysicsVelocity vel, in JumpForceComponent jumpForce, in MovementSpeedComponentData movementSpeed) =>
                {
                    var adjustedInput = input * movementSpeed;
                    
                    if (jump)
                    {
                        EntityManager.RemoveComponent<OnGroundComponentTag>(e);
                        adjustedInput.y = jumpForce;
                    }
                    else 
                        adjustedInput.y = vel.Linear.y;
                    
                    vel.Linear = adjustedInput;
                })
                .Run();

            Entities
                .WithNone<OnGroundComponentTag>()
                .ForEach((Entity e, ref PhysicsVelocity vel, in JumpForceComponent jumpForce, in MovementSpeedComponentData movementSpeed) =>
                {
                    var adjustedInput = input * movementSpeed;
                    adjustedInput.y = vel.Linear.y;
                    vel.Linear = adjustedInput;
                })
                .Run();//Running system on main thread: scheduling worker thread would likely take longer, as there's only one object being moved at a time
        }
    }
}
