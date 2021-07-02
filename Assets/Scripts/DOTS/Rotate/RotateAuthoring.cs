using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//[DisallowMultipleComponent]
//public class RotateAuthoring : MonoBehaviour, IConvertGameObjectToEntity { 
//    public float RotationSpeed;
//    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//    {
//        dstManager.AddComponentData(entity, new RotateComponent{ RotationSpeed = RotationSpeed });
//    }
//}

[GenerateAuthoringComponent]
public struct RotateComponent:  IComponentData {
    public float3 RotationEulerSpeed;
}

public class RotateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity e, ref Rotation r, in RotateComponent c) =>
        {
            quaternion rot = r.Value;
            r.Value = math.mul(rot, quaternion.EulerXYZ(c.RotationEulerSpeed));
        }).Schedule();
    }
}