using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// Follows an entity using an EntityReferenceReciever component.
/// </summary>
[RequireComponent(typeof(EntityReferenceReciever))]
public class EntityReferenceFollower : MonoBehaviour
{
    [SerializeField] Transform _rootTransform;
    [SerializeField] EntityReferenceReciever _reciever;
    private EntityManager _em;
    public void Start()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
    public void Update()
    {
        Entity entityToFollow = _reciever.GetEntity();
        if (entityToFollow == Entity.Null)
        {
            Debug.LogError($"Entity is null when trying to follow transform(frame: {Time.frameCount})", gameObject);
            return;
        }

        //Debug.Log("Moving gameobject to entity");
        if (_em.HasComponent<Translation>(entityToFollow))
        {
            _rootTransform.position = _em.GetComponentData<Translation>(entityToFollow).Value;
        }
    }
}