using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CometBehaviour : MonoBehaviour
{

    [SerializeField] private Sprite[] sprites;
    [SerializeField] private GameObject explosionAnim;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int cometDestroyedPoints = 25;
    [SerializeField] private float maxSplitDelay;

    private GameObject[] targets;
    private Vector2 targetPos;
    private float speed;

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

        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
        sr.sprite = sprites[textureIndex];
        sr.flipX = flipX;
        sr.flipY = flipY;

        targets = GameObject.FindGameObjectsWithTag("Building");
        int targetIndex = Random.Range(0, targets.Length);
        targetPos = targets[targetIndex].transform.position;
        speed = mGameController.CometSpeed;

        float timer = Random.Range(0.1f, maxSplitDelay);
        timer /= speed;
        Invoke("Split", timer);

    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        if ((Vector2)transform.position == targetPos)
        {
            Explode();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Building")
        {
            Explode();

            if (collision.GetComponent<PlayerController>() != null)
            {
                mGameController.PlayerHit();
                return;
            }

            mGameController.CityDestroyed();
            Destroy(collision.gameObject);
        }
        else if (collision.tag == "Explosion")
        {
            mGameController.AddScorePoints(cometDestroyedPoints);
            Explode();
        }
    }

    private void Explode()
    {
        mGameController.HandleCometDestroyed();
        Instantiate(explosionAnim, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    private void Split()
    {
        float minSplitHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0)).y;

        if (minSplitHeight > transform.position.y)
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
            mGameController.HandleCometSplit(1);
        }
    }
}
