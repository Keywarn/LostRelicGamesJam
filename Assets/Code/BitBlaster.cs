using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitBlaster : Placeable
{
    public float fireCooldown;
    public float range;
    public int damage;
    private float currentFireTime;

    public GameObject laserPrefab;
    public GameObject laserHitPrefab;
    private GameObject rangeVisual;

    private Manager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = Manager.Instance;
        rangeVisual = transform.GetChild(0).gameObject;
        rangeVisual.transform.localScale = Vector3.one * range * 2f;
    }

    // Update is called once per frame
    void Update()
    {
      // Check we are not building before finding target
      if(!manager.building)
        {
            currentFireTime -= Time.deltaTime;

            if(currentFireTime <= 0)
            {
                GameObject target = GetClosestTargetInRange();

                if(target != null)
                {
                    FireLaser(target);
                    target.GetComponent<Enemy>().TakeDamage(damage);
                    currentFireTime = fireCooldown;
                }
            }
        }
    }

    private GameObject GetClosestTargetInRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject closest = null;
        float distance = Mathf.Infinity;

        Vector3 position = transform.position;
        Vector3 endPosition = manager.GetEndPosition();
        float rangeSqr = range * range;

        foreach(GameObject enemy in enemies)
        {
            // Check if enemy is in range first
            float towerDiff = (enemy.transform.position - position).sqrMagnitude;
            if(towerDiff <= rangeSqr)
            {
                float endDiff = (enemy.transform.position - endPosition).sqrMagnitude;
                if(endDiff < distance)
                {
                    closest = enemy;
                    distance = endDiff;
                }
            }
        }

        return closest;
    }

    private void FireLaser(GameObject target)
    {
        Vector3 direction = target.transform.position - transform.position;
        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.FromToRotation(Vector3.up, direction));
        laser.transform.localScale = new Vector3(laser.transform.localScale.x,direction.magnitude,0);
        Instantiate(laserHitPrefab, target.transform.position, Quaternion.identity);
    }

    void OnMouseOver()
    {
        rangeVisual.SetActive(true);
    }

    void OnMouseExit()
    {
        rangeVisual.SetActive(false);
    }
}
