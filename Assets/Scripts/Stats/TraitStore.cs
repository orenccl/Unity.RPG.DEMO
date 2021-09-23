using GameDevTV.Saving;
using RPG.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class TraitStore : MonoBehaviour, IModifierProvider, ISaveable, IPredicateEvaluator
    {
        [SerializeField] TraitBonus[] bonusConfig;
        [System.Serializable]
        class TraitBonus
        {
            public Trait trait;
            public Stat stats;
            public float additiveBonusPerPoint = 0;
            public float percentageBonusPerPoint = 0;
        }

        Dictionary<Trait, int> assignedPoints = new Dictionary<Trait, int>();
        Dictionary<Trait, int> stagedPoints = new Dictionary<Trait, int>();

        Dictionary<Stat, Dictionary<Trait, float>> additiveBonusCache;
        Dictionary<Stat, Dictionary<Trait, float>> percentageBonusCache;

        private void Awake()
        {
            additiveBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();
            percentageBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();
            foreach (var bonus in bonusConfig)
            {
                if (!additiveBonusCache.ContainsKey(bonus.stats))
                {
                    additiveBonusCache[bonus.stats] = new Dictionary<Trait, float>();

                }

                if (!percentageBonusCache.ContainsKey(bonus.stats))
                {
                    percentageBonusCache[bonus.stats] = new Dictionary<Trait, float>();

                }
                additiveBonusCache[bonus.stats][bonus.trait] = bonus.additiveBonusPerPoint;
                percentageBonusCache[bonus.stats][bonus.trait] = bonus.percentageBonusPerPoint;
            }
        }

        public int GetProposedPoints(Trait trait)
        {
            return GetAssignedPoints(trait) + GetStagedPoints(trait);
        }

        public int GetTotalProposedPoints()
        {
            int total = 0;
            foreach (var points in assignedPoints.Values)
            {
                total += points;
            }
            foreach (var points in stagedPoints.Values)
            {
                total += points;
            }
            return total;
        }

        public int GetAssignedPoints(Trait trait)
        {
            int points = 0;
            assignedPoints.TryGetValue(trait, out points);
            return points;
        }

        public int GetStagedPoints(Trait trait)
        {
            int points = 0;
            stagedPoints.TryGetValue(trait, out points);
            return points;
        }

        public void StagePoints(Trait trait, int points)
        {
            if (!CanAssignPoints(trait, points)) { return; }

            stagedPoints[trait] = GetStagedPoints(trait) + points;
        }

        public bool CanAssignPoints(Trait trait, int points)
        {
            if(GetStagedPoints(trait) + points < 0) { return false; }
            if(GetUnassignedPoints() < points) { return false; }

            return true;
        }

        public int GetUnassignedPoints()
        {
            return GetAssignablePoints() - GetTotalProposedPoints();
        }

        public void Commit()
        {
            foreach (Trait trait in stagedPoints.Keys)
            {
                assignedPoints[trait] = GetProposedPoints(trait);
            }

            stagedPoints.Clear();
        }

        public int GetAssignablePoints()
        {
            return (int)GetComponent<BaseStats>().GetStat(Stat.TotalTraitPoints);
        }

        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
            if (!additiveBonusCache.ContainsKey(stat)) { yield break; }

            foreach (Trait trait in additiveBonusCache[stat].Keys)
            {
                float bonus = additiveBonusCache[stat][trait];
                yield return bonus * GetAssignedPoints(trait);
            }


        }

        public IEnumerable<float> GetPercentageModifier(Stat stat)
        {
            if (!percentageBonusCache.ContainsKey(stat)) { yield break; }

            foreach (Trait trait in percentageBonusCache[stat].Keys)
            {
                float bonus = percentageBonusCache[stat][trait];
                yield return bonus * GetAssignedPoints(trait);
            }
        }

        public object CaptureState()
        {
            return assignedPoints;
        }

        public void RestoreState(object state)
        {
            assignedPoints = new Dictionary<Trait, int>((Dictionary<Trait, int>)state);
        }

        public bool? Evaluate(string predicate, string[] parameters)
        {
            switch (predicate)
            {
                case "MinimumTrait":
                    if(Enum.TryParse<Trait>(parameters[0], out Trait trait))
                    {
                        return GetAssignedPoints(trait) >= Int32.Parse(parameters[1]);
                    }
                    break;
            }
            return null;
        }
    }
}