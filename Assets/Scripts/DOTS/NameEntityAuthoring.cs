using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
/// <summary>
/// Use to rename an entity, if you want the GameObject to have a different name than the Entity it converts to.
/// </summary>
public class NameEntityAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public string Name;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
// When you use the function EntityManager.SetName, use #if UNITY_EDITOR, otherwise builds will fail. 
#if UNITY_EDITOR
        if (!string.IsNullOrWhiteSpace(Name))
        {
            dstManager.SetName(entity, Name);
            Destroy(this);
        }
#endif
    }
}
