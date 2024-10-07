using UnityEngine;

namespace _Scripts.Cards.Sabotage
{
    public class SabotageComponent : MonoBehaviour
    {
        [SerializeField] private GameObject sabotageObjPrefabRef;

        public void CreateNewSabotage()
        {
            Vector3 position = transform.parent.transform.position;
            Instantiate(sabotageObjPrefabRef, position, Quaternion.identity);
        }
    }
}
