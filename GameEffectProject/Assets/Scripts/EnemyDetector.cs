using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 10.0f;
    [SerializeField] private LayerMask enemyLayer;

    public GameObject GetClosestEnemy()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

        if (enemiesInRange.Length > 0)
        {
            GameObject bestTarget = null;
            float clossetDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;

            foreach (Collider enemyCollider in enemiesInRange)
            {
                if (enemyCollider.gameObject == this)
                    continue;

                Vector3 directionToTarget = enemyCollider.transform.position - currentPosition;
                float dSqrToToTarget = directionToTarget.sqrMagnitude;

                if (dSqrToToTarget < clossetDistanceSqr)
                {
                    clossetDistanceSqr = dSqrToToTarget;
                    bestTarget = enemyCollider.gameObject;
                }
            }

            return bestTarget;
        }
        else
        {
            return null;
        }
    }

    public List<GameObject> GetEnemiesInRange()
    {
        List<GameObject> enemiesList = new List<GameObject>();
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

        foreach (Collider enemyCollider in enemiesInRange)
        {
            if (enemyCollider.gameObject == this.gameObject)
            {
                enemiesList.Add(enemyCollider.gameObject);
            }
        }

        return enemiesList;
    }
}
