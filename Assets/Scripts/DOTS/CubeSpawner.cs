//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;
//public class CubeSpawner : MonoBehaviour, IDeclareReferencedPrefabs
//{
//    public GameObject[] CubePrefabs;
//    public int3 size = new int3(10, 10, 10);

//    private BlobAssetStore _blobAssetStore;
//    private Entity[] entities;

//    [SerializeField] float xOffset = .1f;
//    [SerializeField] float yOffset = .1f;
//    protected void Start()
//    {
//        //obj = GameObject.Find("Cube");
//        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
//        _blobAssetStore = new BlobAssetStore();
//        Entity convertedObject = GameObjectConversionUtility.ConvertGameObjectHierarchy(CubePrefabs, GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore));

//        new SpawnCubesJob(entityManager, convertedObject, size, rootPos:transform.position).Schedule();
//    }
//    struct SpawnCubesJob : IJob
//    {
//        EntityManager entityManager;
//        Entity convertedObject;
//        int3 size;
//        float3 scale;
//        float3 rootPos;
//        float timeScale;
//        public SpawnCubesJob(EntityManager entityManager, Entity convertedObject, int3 size, float3? scale = null, float3 rootPos = default, float timeScale = 1f)
//        {
//            if(scale == null)
//            {
//                scale = math.float3(1);
//            }
//            this.scale = scale.Value;
//            this.entityManager = entityManager;
//            this.convertedObject = convertedObject;
//            this.size = size;
//            this.rootPos = rootPos;
//            this.timeScale = timeScale;
//            //EntityCommandBuffer ecb = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();
//        }
//        public void Execute()
//        {
//            for (int x = 0; x < size.x; x++)
//            {
//                for (int y = 0; y < size.y; y++)
//                {
//                    for (int z = 0; z < size.z; z++)
//                    {
//                        InstantiateEntity(x, y, z);
//                    }
//                }
//            }
//        }

//        private void InstantiateEntity(int x, int y, int z)
//        {
//            var newEntity = entityManager.Instantiate(convertedObject);
//            if (!entityManager.HasComponent<Translation>(newEntity))
//            {
//                entityManager.AddComponent<Translation>(newEntity);
//            }
//            entityManager.SetComponentData(newEntity, new Translation() { Value = rootPos + new float3(x, y, z) * 2 * scale });

//            if (!entityManager.HasComponent<SineWaveComponent>(newEntity))
//            {
//                entityManager.AddComponent<SineWaveComponent>(newEntity);
//            }
//            entityManager.SetComponentData(newEntity, new SineWaveComponent() { TimeOffset = x * timeScale + y * timeScale });
//        }
//    }

//    public void DeclareReferencedPrefabs(System.Collections.Generic.List<GameObject> referencedPrefabs)
//    {
//        referencedPrefabs.Add(ConvertedObject);
//    }
//    void OnDestroy()
//    {
//        _blobAssetStore.Dispose();
//    }
//}
////using UnityEngine;
////using Unity.Entities;
////using Unity.Mathematics;
////using Unity.Transforms;
////using Unity.Collections;
////using System.Collections.Generic;

//namespace Assets
//{
//    //source used: https://www.youtube.com/watch?v=UHiy-0O1JlE
//    public class asdf : MonoBehaviour, IDeclareReferencedPrefabs
//    {
//        public GameObject[] Prefabs;

//        [SerializeField] int _countX;
//        [SerializeField] int _countY;
//        [SerializeField] float _xDistance = 1;
//        [SerializeField] float _yDistance = 1;

//        private BlobAssetStore _bas;
//        GameObjectConversionSettings _settings;
//        void Start() {
//            //Convert gameobject prefabs to entity prefabs
//            Entity[] entityPrefabs = new Entity[Prefabs.Length];
//            for (int i = 0; i < Prefabs.Length; i++)
//            {
//                if (_bas == null)
//                {
//                    _bas = new BlobAssetStore();
//                }
//                _settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _bas);
//                entityPrefabs[i] = GameObjectConversionUtility.ConvertGameObjectHierarchy(Prefabs[i], _settings);
//                //Prefabs[i].SetActive(false);
//            }


//            var random = Unity.Mathematics.Random.CreateFromIndex(0);
//            random.InitState();
//            //try for extra performance: nativearray
//            for (int x = 0; x < _countX; x++)
//            {
//                for (int y = 0; y < _countY; y++)
//                {
//                    var prefabIndex = random.NextInt(entityPrefabs.Length);
//                    var newEntity = entityManager.Instantiate(entityPrefabs[prefabIndex]);
//                    var position = new float3(x * _xDistance, 0/*noise.cnoise(new float2(x, y))*/, y * _yDistance);
//                    entityManager.SetComponentData(newEntity, new Translation() { Value = position });


//                }
//            }
//            foreach (var p in entityPrefabs)
//            {
//                if (p != Entity.Null)
//                    entityManager.DestroyEntity(p);
//            }

//            //blobAssetStore.Dispose();
//        }
//        void OnDestroy()
//        {
//            _bas.Dispose();
//        }

//        public void DeclareReferencedPrefabs(System.Collections.Generic.List<GameObject> referencedPrefabs)
//        {
//            foreach (var prefab in Prefabs)
//            {
//                referencedPrefabs.Add(prefab);

//            }
//        }
//    }
//}
