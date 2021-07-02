using Unity.Entities;

[GenerateAuthoringComponent]
public struct PickupCollector : IComponentData {
    public byte RedPickupsAmt;
    public byte BluePickupsAmt;
    public byte YellowPickupsAmt;
}