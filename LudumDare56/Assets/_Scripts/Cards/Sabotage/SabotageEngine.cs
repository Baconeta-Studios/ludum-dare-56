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
        }
    }
}
