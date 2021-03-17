using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinamatics
{
    public class CinematicTrigger : MonoBehaviour
    {
        private bool triggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player" && triggered == false)
            {
                GetComponent<PlayableDirector>().Play();
                triggered = true;
            }
        }
    }
}