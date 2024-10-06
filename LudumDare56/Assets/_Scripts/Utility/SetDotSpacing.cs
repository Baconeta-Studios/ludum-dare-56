using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class SetDotSpacing : MonoBehaviour
    {
        public HorizontalLayoutGroup layoutGroup;
        public float dotWidth = 32f;
        public RectTransform lengthRectTransform;

        private void Update()
        {
            if (transform.childCount > 2)
            {
                var dotCount = transform.childCount;
                layoutGroup.spacing = (lengthRectTransform.rect.width - (dotCount * dotWidth)) / (dotCount - 1);
            }
            else
            {
                layoutGroup.spacing = 100f;
            }
        }

        private void OnValidate()
        {
            layoutGroup = GetComponent<HorizontalLayoutGroup>();
            lengthRectTransform = GetComponent<RectTransform>();
        }
    }
}