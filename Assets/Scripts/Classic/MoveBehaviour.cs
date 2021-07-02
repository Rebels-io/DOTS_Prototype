using UnityEngine;

namespace Assets.Scripts.Classic
{
    public class MoveBehaviour : MonoBehaviour
    {
        public Vector3 Speed;
        private void Update()
        {
            transform.position += Speed * Time.deltaTime;
        }
    }
}
