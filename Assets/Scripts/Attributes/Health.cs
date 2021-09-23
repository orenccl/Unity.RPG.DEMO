using GameDevTV.Saving;
using GameDevTV.Utils;
using RPG.Core;
using RPG.Stats;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float regenRationPercentage = 70f;
        [SerializeField] private TakeDamageEvent takeDamage;
        [SerializeField] public UnityEvent onDie;

        [Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {

        }

        private LazyValue<float> healthPoints;
        private bool wasDeadLastFrame = false;
        private BaseStats baseStats = null;

        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            healthPoints = new LazyValue<float>(GetMaxHealthPoints);
        }

        private void Start()
        {
            healthPoints.ForceInit();
        }

        private void OnEnable()
        {
            baseStats.onLevelUp += RegenHealth;
        }

        private void OnDisable()
        {
            baseStats.onLevelUp -= RegenHealth;
        }

        public bool IsDead()
        {
            return healthPoints.value <= 0;
        }

        public void TakeDamage(GameObject instgator, float damage)
        {
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);

            if (IsDead())
            {
                onDie.Invoke();
                AwardingExperience(instgator);
            }
            else
            {
                takeDamage.Invoke(damage);
            }
            UpdateState();
        }

        public void Heal(float healthToRestore)
        {
            healthPoints.value = Mathf.Min(healthPoints.value + healthToRestore, GetMaxHealthPoints());
            UpdateState();
        }

        public float GetHealthPoints()
        {
            return healthPoints.value;
        }

        public float GetMaxHealthPoints()
        {
            return baseStats.GetStat(Stat.Health);
        }

        public float GetPercentage()
        {
            return 100 * (healthPoints.value / baseStats.GetStat(Stat.Health));
        }

        public float GetFraction()
        {
            return healthPoints.value / baseStats.GetStat(Stat.Health);
        }

        private void UpdateState()
        {
            Animator animator = GetComponent<Animator>();

            if (!wasDeadLastFrame && IsDead())
            {
                animator.SetTrigger("die");
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }

            if (wasDeadLastFrame && !IsDead())
            {
                animator.Rebind();
            }

            wasDeadLastFrame = IsDead();
        }

        private void AwardingExperience(GameObject instgator)
        {
            Experience xp = instgator.GetComponent<Experience>();
            if (xp == null) return;
            xp.GainExperience(baseStats.GetStat(Stat.XpReward));
        }

        private void RegenHealth()
        {
            float regenHealthPoints = baseStats.GetStat(Stat.Health) * (regenRationPercentage / 100);
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;

            UpdateState();
        }
    }
}