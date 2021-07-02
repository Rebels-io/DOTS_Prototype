using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.DOTS.Hybrid.DataSync
{
    /// <summary>
    /// Updates a UI text element to show the amount of pickups a referenced entity currently has.
    /// </summary>
    public class UpdateUIBehaviour : MonoBehaviour
    {
        public Text textElementToUpdate;
        public EntityReferenceReciever e;

        public void Start()
        {
            if (!e) e = GetComponent<EntityReferenceReciever>();
            if (!e)
            {
                Debug.LogError($"No {nameof(EntityReferenceReciever)} set up on {nameof(UpdateUIBehaviour)}", gameObject);
            }
        }
        public void Update()
        {
            PickupCollector coll = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<PickupCollector>(e.GetEntity());
            textElementToUpdate.text = string.Format("<color=blue>{0}</color> <color=red>{1}</color> <color=yellow>{2}</color> ", 
                coll.BluePickupsAmt, coll.RedPickupsAmt, coll.YellowPickupsAmt);
        }
    }
}
