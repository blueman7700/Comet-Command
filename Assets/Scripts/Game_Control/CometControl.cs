using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System;
using UnityEngine;

public class CometControl : MonoBehaviour
{
    public static event Action OnCometSpawn;

    [SerializeField] private GameObject cometPrefab;

    private float minX, maxX, yVal;
    private GameController mGameController;
    private int numCometsToSpawn;

    public float spawnDelay = 1f;

    void Awake()
    {
        mGameController = FindObjectOfType<GameController>();

        //calculate upper and lower x bounds of the screen
        minX = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 1)).x;
        maxX = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 1)).x;

        //calculate upper y val of screen for spawning
        yVal = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;

    }

    private void Start()
    {
        GameController.OnCometSpawnRequest += BeginSpawning;
    }

    private void OnDestroy()
    {
        GameController.OnCometSpawnRequest -= BeginSpawning;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeginSpawning(int numComets)
    {
        numCometsToSpawn = numComets;
        StartCoroutine(SpawnComets());
    }

    public IEnumerator SpawnComets()
    {
        yield return new WaitForSeconds(1);

        while (numCometsToSpawn > 0)
        {
            //get new random x value between bounds
            float randX = UnityEngine.Random.Range(minX, maxX);
            Instantiate(cometPrefab, new Vector3(randX, yVal, 0), Quaternion.identity);

            OnCometSpawn?.Invoke();
            numCometsToSpawn--;

            yield return new WaitForSeconds(spawnDelay);
        }
    }

}
