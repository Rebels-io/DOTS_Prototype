using Unity.Entities;

namespace Movement
{
    [GenerateAuthoringComponent]
    internal struct PlayerInput : IComponentData
    {
        byte inputIndex;
        public static implicit operator byte(PlayerInput i) => i.inputIndex;
    }
}