using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainShoot : MonoBehaviour
{
    [SerializeField] private float _refreshRate = 0.01f;
    [SerializeField] [Range(1, 10)] private int _maximumEnemiesInChain = 3;
    [SerializeField] private float _delayBetweenEachChain = 0.3f;
    [SerializeField] private Transform _playerFirePoint;
    [SerializeField] private EnemyDetector _playerEnemyDetector;
    [SerializeField] private GameObject _lineRendererPrefab;

    private List<GameObject> _spawnedLineRenderers = new List<GameObject>();
    private List<GameObject> _enemiesInChain = new List<GameObject>();
    private List<GameObject> _activeEffect = new List<GameObject>();

    private GameObject _currentClosestEnemy;

    private bool _shooting;
    private bool _shot;
    private float _counter = 1;

    private void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            if (_playerEnemyDetector.GetEnemiesInRange().Count > 0)
            {
                if (!_shooting)
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

    private void StartShooting()
    {
        _shooting = true;

        if (_playerEnemyDetector != null && _playerFirePoint != null && _lineRendererPrefab != null)
        {
            if (!_shot)
            {
                _shot = true;

                _currentClosestEnemy = _playerEnemyDetector.GetClosestEnemy();
                NewLineRenderer(_playerFirePoint, _playerEnemyDetector.GetClosestEnemy().transform, true);

                if (_maximumEnemiesInChain > 1)
                {
                    StartCoroutine(ChainReaction(_playerEnemyDetector.GetClosestEnemy()));
                }
            }
        }
    }

    private void NewLineRenderer(Transform startPos, Transform endPos, bool getClosestEnemyToPlayer = false)
    {
        GameObject lineR = Instantiate(_lineRendererPrefab);
        _spawnedLineRenderers.Add(lineR);
        StartCoroutine(UpdateLineRenderer(lineR, startPos, endPos, getClosestEnemyToPlayer));
    }

    IEnumerator UpdateLineRenderer(GameObject lineR, Transform startPos, Transform endPos, bool getClosestEnemyToPlayer = false)
    {
        if (_shooting && _shot && lineR != null)
        {
            lineR.GetComponent<LineRendererController>().SetPosition(startPos, endPos);

            yield return new WaitForSeconds(_refreshRate);

            if (getClosestEnemyToPlayer)
            {
                StartCoroutine(UpdateLineRenderer(lineR, startPos, _playerEnemyDetector.GetClosestEnemy().transform, true));

                if (_currentClosestEnemy != _playerEnemyDetector.GetClosestEnemy())
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

    private void StopShooting()
    {
        _shooting = false;
        _shot = false;
        _counter = 1;

        for (int i = 0; i < _spawnedLineRenderers.Count; i++)
        {
            Destroy(_spawnedLineRenderers[i]);
        }

        _spawnedLineRenderers.Clear();
        _enemiesInChain.Clear();

        for (int i = 0; i < _activeEffect.Count; i++)
        {
            Destroy(_activeEffect[i]);
        }

        _activeEffect.Clear();
    }

    IEnumerator ChainReaction(GameObject closestEnemy)
    {
        yield return new WaitForSeconds(_delayBetweenEachChain);

        if (_counter == _maximumEnemiesInChain)
        {
            yield return null;
        }
        else
        {
            if (_shooting)
            {
                _counter++;
                _enemiesInChain.Add(closestEnemy);

                if (!_enemiesInChain.Contains(closestEnemy.GetComponent<EnemyDetector>().GetClosestEnemy()))
                {
                    NewLineRenderer(closestEnemy.transform, closestEnemy.GetComponent<EnemyDetector>().GetClosestEnemy().transform);
                    StartCoroutine(ChainReaction(closestEnemy.GetComponent<EnemyDetector>().GetClosestEnemy()));
                }
            }
        }
    }
}
