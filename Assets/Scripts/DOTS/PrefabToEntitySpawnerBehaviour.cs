using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

namespace Assets
{
    //source used: https://www.youtube.com/watch?v=UHiy-0O1JlE
    public class PrefabToEntitySpawnerBehaviour : MonoBehaviour
    {
        public GameObject[] Prefabs;

        [SerializeField] int _countX;
        [SerializeField] int _countY;
        [SerializeField] float _xDistance = 1;
        [SerializeField] float _yDistance = 1;

        private BlobAssetStore _bas;

        GameObjectConversionSettings _settings;
        private void Update()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }
            if (Prefabs.Length == 0)
            {
                Debug.LogError("Amount of prefabs can't be 0!", gameObject);
            }
            var random = new Unity.Mathematics.Random();
            random.InitState();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;


            //Convert gameobject prefabs to entity prefabs
            Entity[] entityPrefabs = new Entity[Prefabs.Length];
            for (int i = 0; i < Prefabs.Length; i++)
            {
                if (_bas == null)
                {
                    _bas = new BlobAssetStore();
                }
                _settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _bas);
                entityPrefabs[i] = GameObjectConversionUtility.ConvertGameObjectHierarchy(Prefabs[i], _settings);
                //Prefabs[i].SetActive(false);
            }
            
            for (int x = 0; x < _countX; x++)
            {
                for (int y = 0; y < _countY; y++)
                {
                    var prefabIndex = random.NextInt(entityPrefabs.Length);
                    var newEntity = entityManager.Instantiate(entityPrefabs[prefabIndex]);
                    var position = new float3(x * _xDistance, 0/*noise.cnoise(new float2(x, y))*/, y * _yDistance);
                    entityManager.SetComponentData(newEntity, new Translation() { Value = position });
                    

                }
            }
            foreach (var p in entityPrefabs)
            {
                if (p != Entity.Null)
                    entityManager.DestroyEntity(p);
            }
        }
        void OnDestroy()
        {
            _bas.Dispose();
        }

    }
}
