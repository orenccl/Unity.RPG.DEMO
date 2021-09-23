using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 20f;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 2f;
        [SerializeField] UnityEvent onHit;

        private Health target = null;
        private Vector3 targetPoint;
        private float damage = 10f;
        private GameObject instgator = null;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }
        // Update is called once per frame
        private void Update()
        {
            if (target != null && isHoming && !target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }

        private void OnTriggerEnter(Collider other)
        {
            Health health = other.GetComponent<Health>();

            if(target != null && health != target) { return; }
            if(health == null || health.IsDead()) { return; }
            if(other.gameObject == instgator) { return; }

            health.TakeDamage(instgator, damage);

            speed = 0;

            onHit.Invoke();
            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, lifeAfterImpact);
        }

        public void SetTarget(Health target, float damage, GameObject instgator)
        {
            SetTarget(damage, instgator, target);
        }

        public void SetTarget(Vector3 targetPoint, float damage, GameObject instgator)
        {
            SetTarget(damage, instgator, null, targetPoint);
        }

        public void SetTarget(float damage, GameObject instgator, Health target = null, Vector3 targetPoint = default)
        {
            this.target = target;
            this.targetPoint = targetPoint;
            this.damage = damage;
            this.instgator = instgator;

            Destroy(gameObject, maxLifeTime);
        }

        public void SetHoming(bool homingSetting)
        {
            isHoming = homingSetting;
        }

        private Vector3 GetAimLocation()
        {
            if(target == null)
            {
                return targetPoint;
            }

            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if(targetCapsule == null)
            {
                return target.transform.position;
            }    
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }
    }
}
