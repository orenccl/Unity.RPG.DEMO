using GameDevTV.Inventories;
using GameDevTV.Saving;
using GameDevTV.Utils;
using RPG.Attributes;
using RPG.Core;
using RPG.Movement;
using RPG.Stats;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeaponConfig = null;
        [SerializeField] float autoAttackRange = 4f;

        Health target;
        Equipment equipment;
        float timeSinceLastAttack = Mathf.Infinity;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        private void Awake()
        {
            currentWeaponConfig = defaultWeaponConfig;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            equipment = GetComponent<Equipment>();
            if (equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeaponConfig);
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) { return; }

            if (target.IsDead()) 
            {
                target = FindNewTargetInRange();
                if (target == null) { return; }
            }

            if (!GetIsInRange(target.transform))
            { 
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehavior();
            }
        }

        public void EquipWeapon(WeaponConfig weaponConfig)
        {
            currentWeaponConfig = weaponConfig;
            currentWeapon.value = AttachWeapon(weaponConfig);
        }

        private void UpdateWeapon()
        {
            WeaponConfig weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if(weapon == null)
            {
                EquipWeapon(defaultWeaponConfig);
            }
            else
            {
                EquipWeapon(weapon);
            }
        }

        private Weapon AttachWeapon(WeaponConfig weaponConfig)
        {
            Animator animator = GetComponent<Animator>();
            return weaponConfig.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget()
        {
            return target;
        }

        public Transform GetHandTransform(bool isRightHand)
        {
            return isRightHand ? rightHandTransform : leftHandTransform;
        }

        private void AttackBehavior()
        {
            transform.LookAt(target.transform);


            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                //This will trigger hit event
                GetComponent<Animator>().ResetTrigger("stopAttack");
                GetComponent<Animator>().SetTrigger("attack");
                timeSinceLastAttack = 0;
            }
        }

        private Health FindNewTargetInRange()
        {
            Health bestCandidate = null;
            float bestDistance = Mathf.Infinity;
            foreach (var candidate in FindAllTargetsInRange())
            {
                float candidateDistance = Vector3.Distance(transform.position, candidate.transform.position);

                if(candidateDistance < bestDistance)
                {
                    bestCandidate = candidate;
                    bestDistance = candidateDistance;
                }
            }

            return bestCandidate;
        }

        private IEnumerable<Health> FindAllTargetsInRange()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, autoAttackRange, Vector3.up, 0);

            foreach (RaycastHit hit in hits)
            {
                Health health = hit.transform.gameObject.GetComponent<Health>();
                if (health == null) { continue; }
                if (health.IsDead()) { continue; }
                if (health.gameObject == gameObject) { continue; }
                yield return health;
            }
        }

        //Animation event
        void Hit()
        {
            if (target == null) return;

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            BaseStats targetBaseStats = target.GetComponent<BaseStats>();
            if(targetBaseStats != null)
            {
                float defence = target.GetComponent<BaseStats>().GetStat(Stat.Defence);
                damage /= 1 + defence / damage;
            }

            if (currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }

            if (currentWeaponConfig.HasProjectile())
            {
                //currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject);
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {
                //target.TakeDamage(gameObject, currentWeaponConfig.GetDamage());
                target.TakeDamage(gameObject, damage);
            }
        }

        void Shoot()
        {
            Hit();
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < currentWeaponConfig.GetRange();
        }

        public bool CanAttack(GameObject combarTarget)
        {
            if (combarTarget == null) return false;

            if (!GetComponent<Mover>().CanMoveTo(combarTarget.transform.position) && !GetIsInRange(combarTarget.transform))
            {
                return false;
            }

            Health targetToTest = combarTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combarTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combarTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
            target = null;
        }
    }
}

