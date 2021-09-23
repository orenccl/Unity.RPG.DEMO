using GameDevTV.Inventories;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] List<Objective> objectives = new List<Objective>();
        [SerializeField] List<Reward> rewards = new List<Reward>();

        [System.Serializable]
        public class Reward
        {
            public int number;
            public InventoryItem item;
        }

        [System.Serializable]
        public class Objective
        {
            public string reference;
            public string description;
        }

        public string GetTitle()
        {
            return name;
        }

        public int GetObjectivesCount()
        {
            return objectives.Count;
        }

        // IEnumerable can return array list etc...
        public IEnumerable<Objective> GetObjectives()
        {
            return objectives;
        }

        public IEnumerable<Reward> GetRewards()
        {
            return rewards;
        }

        public bool HasObjective(string objectiveRef)
        {
            foreach (Objective objective in objectives)
            {
                if (objective.reference == objectiveRef)
                    return true;
            }
            return false;
        }

        public static Quest GetByName(string questName)
        {
            foreach (Quest quest in Resources.LoadAll<Quest>(""))
            {
                if(quest.name == questName)
                {
                    return quest;
                }
            }
            return null;
        }
    }
}