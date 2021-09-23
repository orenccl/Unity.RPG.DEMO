using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    public class AbilityData : IAction
    {
        private GameObject user;
        private Vector3 targetedPoint;
        private IEnumerable<GameObject> targets;
        private bool cancelled = false;

        public AbilityData(GameObject user)
        {
            this.user = user;
        }

        public void SetTargets(IEnumerable<GameObject> targets)
        {
            this.targets = targets;
        }

        public IEnumerable<GameObject> GetTargets()
        {
            return this.targets;
        }

        public GameObject GetUser()
        {
            return this.user;
        }

        public Vector3 GetTargetedPoint()
        {
            return targetedPoint;
        }

        public void SetTargetedPoint(Vector3 targetedPoint)
        {
            this.targetedPoint = targetedPoint;
        }

        public void StartCoroutine(IEnumerator coroutine)
        {
            this.user.GetComponent<MonoBehaviour>().StartCoroutine(coroutine);
        }

        public void Cancel()
        {
            cancelled = true;
        }

        public bool IsCancelled()
        {
            return cancelled;
        }
    }
}