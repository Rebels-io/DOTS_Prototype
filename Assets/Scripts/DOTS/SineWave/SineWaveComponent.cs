#define TEST_MB
//#define TEST_USELESSDATA
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SineWaveComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public bool RandomOffset = false;
    public float TimeOffset;
    public float3 StartPos;
    public float3 EndPos;
    public float Speed = 1;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        float3 position = transform.position;
        dstManager.AddComponentData(entity, GetSineComponent(position));
    }

    private SineWaveComponentData GetSineComponent(float3 position)
    {
        return new SineWaveComponentData()
        {
            Speed = Speed,
            Min = position + StartPos,
            Max = position + EndPos,
            TimeOffset =
                    RandomOffset
                    ? UnityEngine.Random.Range(0, 9000f)
                    : TimeOffset * math.PI
        };
    }
    private SineWaveComponentData2 GetSineComponent2(Vector3 position)
    {
        return new SineWaveComponentData2()
        {
            Speed = Speed,
            Min = position + (Vector3)StartPos,
            Max = position + (Vector3)EndPos,
            TimeOffset =
                    RandomOffset
                    ? UnityEngine.Random.Range(0, 9000f)
                    : TimeOffset * math.PI
        };
    }
#if TEST_MB
    private SineWaveComponentData2 sine;
    private void Start()
    {
        sine = GetSineComponent2(transform.position);
    }
    public void Update()
    {
        transform.position = Vector3.Lerp(sine.Min, sine.Max, GetTime(sine.TimeOffset, Speed, Time.time));

        static float GetTime(float offset, float speed, float time)
        {
            return (Mathf.Sin(time * speed + offset) + 1) / 2;
        }
    }
#endif
}

[Serializable]
public struct SineWaveComponentData : IComponentData
{
    public float3 Min;
    public float3 Max;
    public float TimeOffset;
    public float Speed; 
}
public struct SineWaveComponentData2 : IComponentData
{
    public Vector3 Min;
    public Vector3 Max;
    public float TimeOffset;
    public float Speed; 
}

public class SineWaveMoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float time = (float)Time.ElapsedTime;
        Entities
            .WithName("MoveSineWaveObjects")
            .ForEach((ref Translation translation, in SineWaveComponentData sine) =>
            {
                translation.Value = math.lerp(sine.Min, sine.Max, GetTime(sine.TimeOffset, sine.Speed, time));
            })
#if DISABLE_PARALLEL_SCHEDULING //for testing performance of schedule vs scheduleparallel
            .Schedule();
#else
            .ScheduleParallel();

        static float GetTime(float offset, float speed, float time)
        {
            return (math.sin(time * speed + offset) + 1) / 2;
        }
#endif
    }
}
