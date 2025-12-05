using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private List<SpawnItem> spawnItems;
    [SerializeField] private float spawnRateEnd;
    [SerializeField] private float spawnRateStart;
    [SerializeField] private float timeMax;

    private GameManager gameManager;
    private Coroutine spawnerCoroutine;
    public float spawnRate;

    public List<GameObject> spawnedObjects;



    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.OnCountTime += HandleTimeUpdate;
    }

    void Start()
    {
        spawnedObjects = new List<GameObject>();
        spawnRate = spawnRateStart;
        spawnerCoroutine = StartCoroutine(SpawnCoroutine());
    }

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            //spawnRate -> GameHandler
            yield return new WaitForSeconds(spawnRate);
            Spawn();
        }
    }

    private void HandleTimeUpdate(int time)
    {
        spawnRate = spawnRateStart + Mathf.Min(time / timeMax, 1) * (spawnRateEnd - spawnRateStart);
    }

    private void Spawn()
    {
        float random = Random.Range(0, spawnItems.Sum(item => item.probability));
        for (int i = 0; i < spawnItems.Count; i++)
        {
            if (random <= spawnItems.Take(i+1).Sum(item => item.probability))
            {
                //GameObject gameObject = Instantiate(spawnItems[i].prefab, transform.position, Quaternion.identity);
                GameObject gameObject = Instantiate(spawnItems[i].prefab, transform, false);
                spawnedObjects.Add(gameObject);
                SpawnableObject spawnableObject = gameObject.GetComponent<SpawnableObject>();
                gameObject.transform.position += Vector3.up * spawnableObject.GetHeight();
                return;
            }
        }
    }

    private void OnDestroy()
    {
        StopCoroutine(spawnerCoroutine);
    }

}
