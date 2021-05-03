using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class CometControl : MonoBehaviour
{
    [SerializeField] private GameObject cometPrefab;
    

    private float minX, maxX, yVal;
    private GameController mGameController;

    public int numCometsToSpawnInRound = 10;
    public float spawnDelay = 1f;

    // Start is called before the first frame update
    void Start()
    {
        mGameController = FindObjectOfType<GameController>();

        //calculate upper and lower x bounds of the screen
        minX = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 1)).x;
        maxX = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 1)).x;

        //calculate upper y val of screen for spawning
        yVal = Camera.main.ViewportToWorldPoint(new Vector2(0, 1)).y;

        StartCoroutine(SpawnComet());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator SpawnComet()
    {
        while (mGameController.canSpawnComet())
        {
            //get new random x value between bounds
            float randX = Random.Range(minX, maxX);
            Instantiate(cometPrefab, new Vector3(randX, yVal, 0), Quaternion.identity);

            mGameController.handleCometSpawn();

            yield return new WaitForSeconds(spawnDelay);
        }
    }

}
