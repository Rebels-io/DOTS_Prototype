using Unity.Entities;
using UnityEngine;

namespace Assets.DOTS.Tests
{
    //adds the "Disabled" component, for some reason there is no built in authoring component for this
    public class DisabledAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<Disabled>(entity);
        }
    }
}
