using UnityEngine;

namespace _Scripts.Cards.Sabotage
{
    public class SabotageObject : MonoBehaviour
    {
        [SerializeField] private float startLifetimeSpanSeconds;
        [ReadOnly] private float remainingLifeSpanSeconds;

        private void Start()
        {
            remainingLifeSpanSeconds = startLifetimeSpanSeconds;
        }

        // Update is called once per frame
        private void Update()
        {
            remainingLifeSpanSeconds -= Time.deltaTime;

            if (remainingLifeSpanSeconds <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
