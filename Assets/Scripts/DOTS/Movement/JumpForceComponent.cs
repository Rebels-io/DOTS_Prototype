using Unity.Entities;

namespace Movement
{
    internal struct JumpForceComponent : IComponentData
    {
        public float JumpForce;

        public JumpForceComponent(float jumpForce)
        {
            JumpForce = jumpForce;
        }

        public static implicit operator float(JumpForceComponent c) => c.JumpForce;
    }
}