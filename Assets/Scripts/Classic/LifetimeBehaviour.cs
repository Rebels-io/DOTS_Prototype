using UnityEngine;

namespace Assets.Scripts.Classic
{
    public class LifetimeBehaviour : MonoBehaviour
    {
        public float Lifetime = 4f;
        private void Update()
        {
            Lifetime -= Time.deltaTime;
            if (Lifetime < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
