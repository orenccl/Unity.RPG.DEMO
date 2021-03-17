using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas rootCanvas = null;

        float healthFraction;
        private void Update()
        {
            healthFraction = healthComponent.GetFraction();

            if (Mathf.Approximately(healthFraction, 0) || Mathf.Approximately(healthFraction, 1))
            {
                rootCanvas.enabled = false;
                return;
            }
            rootCanvas.enabled = true;
            foreground.localScale = new Vector3(healthFraction, 1, 1);
       }
    }
}