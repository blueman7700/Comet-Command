using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CometBehaviour : MonoBehaviour
{

    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float speed;
    [SerializeField] private GameObject explosionAnim;
    [SerializeField] private int cometDestroyedPoints = 25;

    private GameObject[] targets;
    private Vector2 targetPos;

    //TODO: make scoring use event listeners
    private GameController mGameController;

    // Start is called before the first frame update
    void Start()
    {
        sprites = Resources.LoadAll<Sprite>("Sprites/Projectiles");
        mGameController = FindObjectOfType<GameController>();

        //randomly select and flip a sprite to make it look like they are unique
        int textureIndex = Random.Range(0, sprites.Length - 1);
        bool flipX = (Random.value > 0.5f);
        bool flipY = (Random.value > 0.5f);

        Debug.Log(textureIndex);

        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
        sr.sprite = sprites[textureIndex];
        sr.flipX = flipX;
        sr.flipY = flipY;

        targets = GameObject.FindGameObjectsWithTag("Building");
        int targetIndex = Random.Range(0, targets.Length);
        targetPos = targets[targetIndex].transform.position;
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Building")
        {
            //TODO: switch this over to something better in the future...
            Destroy(collision.gameObject);
            mGameController.HandleCometDestroyed();
            Explode();
        }
        else if (collision.tag == "Explosion")
        {
            mGameController.AddScorePoints(cometDestroyedPoints);
            mGameController.HandleCometDestroyed();
            Explode();
        }
    }

    private void Explode()
    {
        Debug.Log("Collision Detected!");
        Instantiate(explosionAnim, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
