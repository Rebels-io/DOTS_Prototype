using Unity.Entities;
using UnityEngine;

public class EntityReferenceSenderAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public EntityReferenceReciever[] recievers;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var e = conversionSystem.GetPrimaryEntity(gameObject);
        if (e == Entity.Null)
        {

            Debug.LogError($"{gameObject.name} does not convert to an entity! ", gameObject);
        }
        foreach (var r in recievers)
        {
            r.SetEntity(e);
        }
    }
}