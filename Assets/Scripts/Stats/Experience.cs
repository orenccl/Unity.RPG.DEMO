using RPG.Saving;
using System;
using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencPoints = 0;

        public event Action onExperienceGained;

        public void GainExperience(float experience)
        {
            experiencPoints += experience;
            onExperienceGained();
        }

        public float GetPoints()
        {
            return experiencPoints;
        }

        public object CaptureState()
        {
            return experiencPoints;
        }

        public void RestoreState(object state)
        {
            experiencPoints = (float)state;
        }
    }
}