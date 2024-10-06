using UnityEngine;

namespace _Scripts.Cards.Sabotage
{
    public class SabotageEngine : MonoBehaviour
    {
        [SerializeField]
        private GameObject sabotageObjPrefabRef;

        public void CreateNewSabotage()
        {
            Vector3 position = this.transform.parent.transform.position;
            Instantiate(sabotageObjPrefabRef, position, Quaternion.identity);
            // TODO will need to parent to the spline unless this is a singleton object not parented to a racer
            // Will need to have a box collider built in
        }
    }
}
