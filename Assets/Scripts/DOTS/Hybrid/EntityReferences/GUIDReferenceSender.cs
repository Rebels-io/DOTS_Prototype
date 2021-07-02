using System;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Assets.DOTS.Hybrid
{
    /// <summary>
    ///Use this component to pass an entity reference to a game object even when there is a scene mismatch: copy the GUID and paste it into a GUIDReferenceReciever component. This allows you to use a hybrid DOTS solution while still converting entities before runtime (using SubScenes).
    /// However, it defeats the purpose of using subscenes in the first place: the only reason to use a SubScene is because runtime conversion is too slow. However, trying to find every object with a GUIDReferenceReciever and checking if the GUID is equal is probably much, much slower than runtime conversion.
    /// </summary>
    public class GUIDReferenceSender : MonoBehaviour, IConvertGameObjectToEntity
    {
        public string GUID;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<GUIDReferenceSenderComponentData>(entity);
            dstManager.SetComponentData(entity, new GUIDReferenceSenderComponentData(GUID));
        }

        internal void SetGuid(Guid guid)
        {
            GUID = guid.ToString();
        }
    }
    internal struct GUIDReferenceSenderComponentData : IComponentData
    {
        public FixedString128 GUID;
        public GUIDReferenceSenderComponentData(string guid)
        {
            GUID = guid;
        }
    }

    //[UpdateInGroup(typeof(InitializationSystemGroup))]
    public class EntityGUIDReferenceSenderSystem : SystemBase
    {
        EntityCommandBufferSystem _entityCommandBufferSystem;
        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
        }
        protected override void OnUpdate()
        {
            Debug.Log("Running GUID reference sender");
            EntityCommandBuffer buffer = _entityCommandBufferSystem.CreateCommandBuffer();
            Entities
                .WithoutBurst()
                .ForEach((Entity e, in GUIDReferenceSenderComponentData s) => {
                    Debug.Log($"Running GUID reference sender: ForEach (Entity: {e})");
                    SendEntityByGUID(e, s.GUID);
                    buffer.RemoveComponent<GUIDReferenceSenderComponentData>(e);
                }).Run();//Have to run on main thread for FindObjects

        }
        /// <summary>
        /// Sends an entity reference to all <see cref="GUIDReferenceReciever"/> components in your scene.
        /// </summary>
        public static void SendEntityByGUID(Entity entity, FixedString128 GUID)
        {
            var recievers = Resources.FindObjectsOfTypeAll<GUIDReferenceReciever>().Where((x) =>
            {
                string guidStr = GUID.ToString();
                return (x.GUID).Equals(guidStr);
            });
            if (recievers.Count() == 0)
            {
                Debug.LogError("Reference sender has no recievers! Entity: " + entity);

            }
            else
            {
                foreach (var item in recievers)
                {
                    item.SetEntity(entity);
                }
            }
        }
    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(GUIDReferenceSender))]
    public class GUIDReferenceEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Generate GUID"))
            {
                ((GUIDReferenceSender)target).SetGuid(Guid.NewGuid());
            }

            base.OnInspectorGUI();
        }
    } 
#endif
}

