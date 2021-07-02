using Unity.Entities;
using UnityEngine;

namespace Assets.DOTS.Hybrid
{
    /// <summary>
    /// Gets an entity reference from a gameobject in your scene that converts at runtime (using ConvertToEntity)
    /// </summary>
    public class EntityReferenceRecieverFromConvertedGameObject : EntityReferenceReciever
    {
        [SerializeField] GameObject _ref;
        private void OnEnable()
        {
            var e = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<GameObjectConversionSystem>()
                .GetPrimaryEntity(_ref);
            if (e != Entity.Null)
            {
                SetEntity(e);
            }
        }
    }
    //should be the same as:
    //[GenerateAuthoringComponent]
    //public struct EntityReferenceAuthoring : IComponentData
    //{
    //    Entity _ref;
    //}
}
