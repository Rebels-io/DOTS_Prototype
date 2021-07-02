using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PrefabConversionScript : MonoBehaviour, IDeclareReferencedPrefabs
{
    public GameObject PlayerPrefab;
    BlobAssetStore _blobAssetStore;
    public Entity e;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        _blobAssetStore = new BlobAssetStore();
        e = GameObjectConversionUtility.ConvertGameObjectHierarchy(PlayerPrefab, GameObjectConversionSettings.FromWorld(dstManager.World, _blobAssetStore));
        
    }
    public Entity GetPrefab()
    {
        return e;
    }
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(PlayerPrefab);
    }
}
