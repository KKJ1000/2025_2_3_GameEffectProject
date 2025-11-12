using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ChainShoot : MonoBehaviour
{
    [SerializeField] float refreshRate = 0.01f;
    [SerializeField] [Range(1, 10)] int maximumEnemiesInChain = 3;
    [SerializeField] float delayBetweenEachChain = 0.3f;
    [SerializeField] Transform playerFirePoint;
    [SerializeField] EnemyDetector playerEnemyDetector;
    [SerializeField] GameObject lineRendererPrefab;

    List<GameObject> spawnedLineRenderers = new List<GameObject>();
    List<GameObject> enemiesInChain = new List<GameObject>();
    List<GameObject> activeEffect = new List<GameObject>();

    GameObject currentClosestEnemy;

    bool shooting;
    bool shot;
    float counter = 1;

    private void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            if (playerEnemyDetector.GetEnemiesInRange().Count > 0)
            {
                if (!shooting)
                {
                    StartShooting();
                }
            }
            else
            {
                StopShooting();
            }

            if (Input.GetButtonUp("Fire1"))
            {
                StopShooting();
            }
        }
    }
    private void StopShooting()
    {
        shooting = false;
        shot = false;
        counter = 1;

        for (int i = 0; i < spawnedLineRenderers.Count; i++)
        {
            Destroy(spawnedLineRenderers[i]);
        }

        spawnedLineRenderers.Clear();
        enemiesInChain.Clear();

        for (int i = 0; i < activeEffect.Count; i++)
        {
            Destroy(activeEffect[i]);
        }

        activeEffect.Clear();
    }

    IEnumerator UpdateLineRenderer(GameObject lineR, Transform startPos, Transform endPos, bool getClosestEnemyToPlayer = false)
    {
        if (shooting && shot && lineR != null)
        {
            lineR.GetComponent<LineRendererController>().SetPosition(startPos, endPos);

            yield return new WaitForSeconds(refreshRate);

            if (getClosestEnemyToPlayer)
            {
                StartCoroutine(UpdateLineRenderer(lineR, startPos, playerEnemyDetector.GetClosestEnemy().transform, true));

                if (currentClosestEnemy != playerEnemyDetector.GetClosestEnemy())
                {
                    StopShooting();
                    StartShooting();
                }
                else
                {
                    StartCoroutine(UpdateLineRenderer(lineR, startPos, endPos));
                }
            }
        }
    }

    IEnumerator ChainReaction(GameObject closestEnemy)
    {
        yield return new WaitForSeconds(delayBetweenEachChain);

        if (counter == maximumEnemiesInChain)
        {
            yield return null;
        }
        else
        {
            if (shooting)
            {
                counter++;
                enemiesInChain.Add(closestEnemy);

                if (!enemiesInChain.Contains(closestEnemy.GetComponent<EnemyDetector>().GetClosestEnemy()))
                {
                    NewLineRenderer(closestEnemy.transform, closestEnemy.GetComponent<EnemyDetector>().GetClosestEnemy().transform);
                    StartCoroutine(ChainReaction(closestEnemy.GetComponent<EnemyDetector>().GetClosestEnemy()));
                }
            }
        }
    }

    private void NewLineRenderer(Transform startPos, Transform endPos, bool getClosestEnemyToPlayer = false)
    {
        GameObject lineR = Instantiate(lineRendererPrefab);
        spawnedLineRenderers.Add(lineR);
        StartCoroutine(UpdateLineRenderer(lineR, startPos, endPos, getClosestEnemyToPlayer));
    }

    private void StartShooting()
    {
        shooting = true;

        if (playerEnemyDetector != null && playerFirePoint != null && lineRendererPrefab != null)
        {
            if (!shot)
            {
                shot = true;

                currentClosestEnemy = playerEnemyDetector.GetClosestEnemy();
                NewLineRenderer(playerFirePoint, playerEnemyDetector.GetClosestEnemy().transform, true);

                if (maximumEnemiesInChain > 1)
                {
                    StartCoroutine(ChainReaction(playerEnemyDetector.GetClosestEnemy()));
                }
            }
        }
    }

}
