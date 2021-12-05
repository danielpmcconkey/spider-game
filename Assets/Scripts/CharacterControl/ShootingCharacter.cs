using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CharacterControl
{
    public class ShootingCharacter : ControllableCharacter
    {
        [Range(0, 1f)] [SerializeField] public float primaryFiringWeaponMaxDistancePercent = 0.5f;
        [SerializeField] public LayerMask whatIsEnemy;
        [SerializeField] public GameObject bulletImpactPrefab;
        [SerializeField] public GameObject muzzleFlash;
        [SerializeField] public LineRenderer bulletTraceLinePrefab;

        private float _firingTimer = 0f;
        private int _bulletCount = 0;

        const float maxPrimaryFiringWeaponMaxDistance = 30f;
        const float minPrimaryFiringWeaponMaxDistance = 1f;

        protected override void Update()
        {
            base.Update();

            // check for firing control
            if(userInput.isPrimaryFireButtonPressed || userInput.isPrimaryFireButtonHeldDown)
            {
                // how rapid is our rapid fire?
                // 10 times / second?
                _firingTimer += Time.deltaTime;
                if(_firingTimer >= 0.1f)
                {
                    bool shouldTrace = false; // whether we should show a line along the bullet path
                    if (_bulletCount % 3 == 0) shouldTrace = true; // 33% of the time
                    Fire(shouldTrace);
                }
                muzzleFlash.SetActive(true);
                //RotateMuzzleFlash();
            }
            else
            {
                _firingTimer = 0f;
                bulletTraceLinePrefab.enabled = false;
                muzzleFlash.SetActive(false);
            }
        }
        protected void Fire(bool shouldTrace)
        {
            _firingTimer = 0f;


            // calculate our max distance
            float primaryFiringWeaponMaxDistance = minPrimaryFiringWeaponMaxDistance +
                (primaryFiringWeaponMaxDistancePercent * 
                (maxPrimaryFiringWeaponMaxDistance - minPrimaryFiringWeaponMaxDistance));

            RaycastHit2D hitInfoPlatform = Raycaster.FireAtTargetPoint(ceilingCheckTransform.position,
                targetingReticuleTransform.position, primaryFiringWeaponMaxDistance, whatIsPlatform);
            RaycastHit2D hitInfoEnemy = Raycaster.FireAtTargetPoint(ceilingCheckTransform.position,
                targetingReticuleTransform.position, primaryFiringWeaponMaxDistance, whatIsEnemy);


            bool didHitSomething = false;
            Vector2 impactPoint = Vector2.zero;
            if (hitInfoEnemy )
            {
                didHitSomething = true;
                impactPoint = hitInfoEnemy.point;

                // todo: make damage happen
            }
            if(hitInfoPlatform)
            {
                didHitSomething = true;
                impactPoint = hitInfoPlatform.point;
            }
            if (didHitSomething)
            {
                // instantiate an impact bullet at the impact point
                GameObject bullet = Instantiate(bulletImpactPrefab, impactPoint, Quaternion.identity);
                if(shouldTrace)
                {
                    bulletTraceLinePrefab.enabled = true;
                    bulletTraceLinePrefab.SetPosition(0, ceilingCheckTransform.position);
                    bulletTraceLinePrefab.SetPosition(1, targetingReticuleTransform.position);
                }
                else bulletTraceLinePrefab.enabled = false;
            }

            _bulletCount++;
        }
        private void RotateMuzzleFlash()
        {
            const float rotationsPerSecond = 0.1f;
            float flashZ = muzzleFlash.transform.eulerAngles.z;
            flashZ += rotationsPerSecond * 360 * Time.deltaTime;
            if (flashZ > 360) flashZ -= 360;
            muzzleFlash.transform.eulerAngles = new Vector3(0, 0, flashZ);
        }
    }
}
