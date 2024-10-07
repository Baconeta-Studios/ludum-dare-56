using UnityEngine;

namespace _Scripts.Cards.Sabotage
{
    public class SabotageComponent : MonoBehaviour
    {
        [SerializeField] private GameObject sabotageObjPrefabRef;
        
        [Header("Sabotage Settings")]
        [SerializeField] private float durationOfSlow = 3f;
        [SerializeField] private float maxSpeedDuring = 3f;
        [SerializeField] private bool isAffectingRacer;
        [SerializeField] private float deceleration = 0.3f;
        [SerializeField] private Transform sabotageSpawnPos;
        

        private float timeRemaining;
        private bool isFinishing;

        public float Deceleration => deceleration;
        public float MaxSpeed => maxSpeedDuring;
        public bool IsAffectingRacer
        {
            get => isAffectingRacer;
            set => isAffectingRacer = value;
        }
        
        private void Update()
        {
            if (isAffectingRacer)
            {
                timeRemaining -= Time.deltaTime;
                if (timeRemaining <= 0)
                {
                    EndOverride();
                }
            }
        }
        
        [ContextMenu("Activate Override")]
        public void StartOverride()
        {
            if (isAffectingRacer)
            {
                timeRemaining += durationOfSlow;
            }
            else
            {
                timeRemaining = durationOfSlow;
            }

            isAffectingRacer = true;
        }

        private void EndOverride(bool forceFinish = false)
        {
            isAffectingRacer = false;
            timeRemaining = 0f;

            if (forceFinish)
            {
                OverrideFinished();
            }
            else
            {
                isFinishing = true;
            }
        }

        private void OverrideFinished()
        {
            isFinishing = false;
        }

        public void CreateNewSabotage()
        {
            Instantiate(sabotageObjPrefabRef, sabotageSpawnPos.position, sabotageSpawnPos.rotation);
        }
    }
}
