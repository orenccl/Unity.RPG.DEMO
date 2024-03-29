﻿using RPG.Quests;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] Transform objectiveContainer;
        [SerializeField] GameObject objectivePrefab;
        [SerializeField] GameObject objectiveIncompletePrefab;
        [SerializeField] TextMeshProUGUI rewardText;

        public void Setup(QuestStatus status)
        {
            Quest quest = status.GetQuest();
            title.text = quest.GetTitle();

            foreach (Transform child in objectiveContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (Quest.Objective objective in quest.GetObjectives())
            {
                GameObject prefab = status.IsObjectiveComplete(objective.reference) ? objectivePrefab : objectiveIncompletePrefab;
                GameObject objectiveInstance = Instantiate(prefab, objectiveContainer);
                objectiveInstance.GetComponentInChildren<TextMeshProUGUI>().text = objective.description;
            }

            rewardText.text = GetRewardText(quest);       
        }

        private string GetRewardText(Quest quest)
        {
            string rewardText = "";
            foreach (var reward in quest.GetRewards())
            {
                if(rewardText != "")
                {
                    rewardText += ", ";
                }

                if (reward.number > 1)
                {
                    rewardText += reward.number + " ";
                }

                rewardText += reward.item.GetDisplayName();
            }

            if(rewardText == "")
            {
                rewardText = "No reward";
            }
            rewardText += ".";
            return rewardText;
        }
    }
}