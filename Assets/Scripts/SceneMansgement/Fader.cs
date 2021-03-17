using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        Coroutine currentAvticeFade = null;

        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        
        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1;
        }

        public Coroutine FadeOut(float time)
        {
            return Fade(time, 1);
        }

        public Coroutine FadeIn(float time)
        {
            return Fade(time, 0);
        }

        public Coroutine Fade(float time, float target)
        {
            if (currentAvticeFade != null)
            {
                StopCoroutine(currentAvticeFade);
            }
            currentAvticeFade = StartCoroutine(FadeRoutine(time, target));
            return currentAvticeFade;
        }

        private IEnumerator FadeRoutine(float time, float target)
        {
            while (!Mathf.Approximately(canvasGroup.alpha, target))
            {
                yield return canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / time);
            }
        }
    }

}
