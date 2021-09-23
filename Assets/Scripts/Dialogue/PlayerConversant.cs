using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] string playerName;
        Dialogue currentDialogue = null;
        DialogueNode currentNode = null;
        AIConversant currentConversant = null;
        bool isChoosing = false;

        public event Action OnConversationUpdated;

        public void StartDialogue(AIConversant newConversant,Dialogue newDialogue)
        {
            currentConversant = newConversant;
            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();
            TriggerEnterAction();
            OnConversationUpdated();
        }

        public string GetText()
        {
            if(currentNode == null)
            {
                return "";
            }

            return currentNode.GetText();
        }

        public void Quit()
        {
            currentDialogue = null;
            TriggerExitAction();
            currentNode = null;
            isChoosing = false;
            currentConversant = null;
            OnConversationUpdated();
        }

        public string GetCurrentConversantName()
        {
            if (isChoosing)
                return playerName;
            else 
                return currentConversant.GetName();
        }

        public bool IsActive()
        {
            return currentDialogue != null;
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            TriggerEnterAction();
            Next();
        }

        public void Next()
        {
            int numPlayerResponse = FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)).Count();
            if(numPlayerResponse > 0)
            {
                isChoosing = true;
                TriggerExitAction();
            }
            else
            {
                DialogueNode[] children = FilterOnCondition(currentDialogue.GettAIChildren(currentNode)).ToArray();
                if(children.Count() != 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, children.Count());
                    TriggerExitAction();
                    currentNode = children[randomIndex];
                    TriggerEnterAction();
                    isChoosing = false;
                }
                else 
                {
                    TriggerExitAction();
                }
            }
            OnConversationUpdated();
        }

        public bool HasNext()
        {
            return FilterOnCondition(currentDialogue.GettAllChildren(currentNode)).Count() > 0;
        }

        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode)
        {
            foreach (var node in inputNode)
            {
                if (node.CheckCondition(GetEvaluators()))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IPredicateEvaluator>();
        }

        private void TriggerEnterAction()
        {
            if(currentNode != null && currentNode.GetOnEnterAction() != "")
            {
                TriggerAction(currentNode.GetOnEnterAction());
            }
        }

        private void TriggerExitAction()
        {
            if (currentNode != null && currentNode.GetOnExitAction() != "")
            {
                TriggerAction(currentNode.GetOnExitAction());
            }
        }

        private void TriggerAction(string action)
        {
            if (action == "") return;

            foreach(DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }
    }
}