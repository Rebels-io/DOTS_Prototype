//#define DEBUG_ENTREF
using Unity.Entities;
using UnityEngine;
/// <summary>
/// Use this class to reference an Entity in a MonoBehaviour.
/// </summary>
public class EntityReferenceReciever : MonoBehaviour
{
    protected Entity? _e;
    public Entity GetEntity() => _e ?? Entity.Null;
    public void SetEntity(Entity entity) => _e = entity;

#if DEBUG_ENTREF
    private void Update()
    {
        if (Time.frameCount % 30 == 0)
        {
            Debug.Log(string.Format("Entity reference: {0}", _e.HasValue ? _e.ToString() : "NOT ASSIGNED"), gameObject);
        }
    }
#endif


    //NOTE: You can't reference a converted GameObject entity in a regular monobehaviour: The GameObject will already be null in Start(), because the GameObjectConversionSystem runs before Start() does. The following code does not work.
    //public GameObject EntityGameObjectReference;
    //public void Start()
    //{
    //    if (EntityGameObjectReference != null)
    //    {
    //        Entity e = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<GameObjectConversionSystem>().GetPrimaryEntity(EntityGameObjectReference);
    //        if(e == Entity.Null)
    //        {
    //            Debug.LogError($"Game object {EntityGameObjectReference}'s entity is null!");
    //        }
    //    }
    //}

}