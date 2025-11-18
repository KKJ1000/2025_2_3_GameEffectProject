using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    [SerializeField] private float _detectionRadius = 10.0f;
    [SerializeField] private LayerMask _enemyLayer;

    /// <summary>
    /// 가장 가까운 적 오브젝트를 반환합니다.
    /// </summary>
    public GameObject GetClosestEnemy()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, _detectionRadius, _enemyLayer);

        if (enemiesInRange.Length > 0)
        {
            GameObject bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;

            foreach (Collider enemyCollider in enemiesInRange)
            {
                if (enemyCollider.gameObject == this.gameObject) 
                    continue;

                Vector3 directionToTarget = enemyCollider.transform.position - currentPosition;
                float dSqrToToTarget = directionToTarget.sqrMagnitude;

                if (dSqrToToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToToTarget;
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

    /// <summary>
    /// 범위 안에 있는 적들을 리스트에 추가하고 해당 리스트를 반환합니다.
    /// </summary>
    public List<GameObject> GetEnemiesInRange()
    {
        List<GameObject> enemiesList = new List<GameObject>();
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, _detectionRadius, _enemyLayer);

        foreach (Collider enemyCollider in enemiesInRange)
        {
            if (enemyCollider.gameObject != this.gameObject)
            {
                enemiesList.Add(enemyCollider.gameObject);
            }
        }

        return enemiesList;
    }
}
