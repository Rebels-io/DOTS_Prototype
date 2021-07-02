using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace Assets.Scripts.DOTS.Hybrid.DataSync
{
    /// <summary>
    /// Updates the animator of a MonoBehaviour using the velocity of an entity. This is a hybrid DOTS implementation to get a fully animated character to work with DOTS, as Unity Animation is currently unusable.
    /// </summary>
    public class UpdateAnimatorComponentVelocity : MonoBehaviour
    {
        [SerializeField] EntityReferenceReciever _reciever;
        private EntityManager _em;
        [SerializeField] float _velocityScale = 1f;
        public string AnimatorFloatName = "Velocity";
        [SerializeField] private Animator _animator;
        public void Start()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
                if (_animator == null)
                {
                    Debug.LogError("No animator component found on object!", gameObject);
                }
            }
            if (_reciever == null)
            {
                _reciever = GetComponent<EntityReferenceReciever>();
                if (_reciever == null)
                {

                    Debug.LogError("No Entity reference reciever found on object!", gameObject);
                }
            }
            if (_velocityScale == 0)
            {
                Debug.LogWarning("Your animator's velocity scale is set to zero; velocity will not update", gameObject);
            }
            _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        }
        public void Update()
        {
            Entity velEntity = _reciever.GetEntity();
            if (velEntity == Entity.Null)
            {
                Debug.LogError($"Entity is null in {nameof(UpdateAnimatorComponentVelocity)}", gameObject);
                return;
            }
            float3 vel;
            try
            {
                vel = _em.GetComponentData<PhysicsVelocity>(velEntity).Linear;
            }
            catch (System.Exception e)
            {

                Debug.LogError("Could not get component data from entity" +
#if UNITY_EDITOR
                    //GetName doesn't work when building, have to add a conditional for the compiler
                    ": " + _em.GetName(velEntity) +
#endif
                ". Exception: \n" + e.Message, gameObject);
                return;
            }
            try
            {
                float2 directionalVel = new float2(vel.x, vel.z);
                float magnitude = math.length(directionalVel * _velocityScale);
                //Debug.Log("Updating animator: " + magnitude, gameObject);
                _animator.SetFloat(AnimatorFloatName, magnitude);
                //rotate toward velocity
                if (magnitude >= .1f)
                {
                    _animator.transform.rotation = Quaternion.LookRotation(new Vector3(directionalVel.x, 0, directionalVel.y));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Could not update velocity on animator component: " + e.Message, gameObject);
                return;
            }

        }
    }
}