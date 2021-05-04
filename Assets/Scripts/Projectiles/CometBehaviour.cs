using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CometBehaviour : MonoBehaviour
{

    public static event Action<int> OnCometSplit;
    public static event Action OnCometDestroyed;
    public static event Action<int, int> OnPointsAwarded;
    public static event Action OnPlayerHit;
    public static event Action<GameObject> OnCityHit;

    [SerializeField] private Sprite[] sprites;
    [SerializeField] private GameObject explosionAnim;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int cometDestroyedPoints = 25;
    [SerializeField] private int cometDestroyedMoney = 10;
    [SerializeField] private float maxSplitDelay;
    [SerializeField] private GameObject targetPrefab;

    private GameObject[] targets;
    private GameObject targetCrosshair;
    private Vector2 targetPos;
    private float speed;
    private bool allowTargeting;
    private LineRenderer lr;

    private GameController mGameController;

    // Start is called before the first frame update
    void Start()
    {
        sprites = Resources.LoadAll<Sprite>("Sprites/Projectiles");
        mGameController = FindObjectOfType<GameController>();
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.useWorldSpace = true;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;

        //randomly select and flip a sprite to make it look like they are unique
        int textureIndex = UnityEngine.Random.Range(0, sprites.Length - 1);
        bool flipX = (UnityEngine.Random.value > 0.5f);
        bool flipY = (UnityEngine.Random.value > 0.5f);

        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
        sr.sprite = sprites[textureIndex];
        sr.flipX = flipX;
        sr.flipY = flipY;

        targets = GameObject.FindGameObjectsWithTag("Building");
        int targetIndex = UnityEngine.Random.Range(0, targets.Length);
        targetPos = targets[targetIndex].transform.position;
        speed = mGameController.CometSpeed;
        allowTargeting = mGameController.AllowTargeting;

        if (allowTargeting)
        {
            targetCrosshair = Instantiate(targetPrefab, targetPos, Quaternion.identity);
        }

        float timer = UnityEngine.Random.Range(0.1f, maxSplitDelay);
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

    private void FixedUpdate()
    {
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, targetPos);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Building")
        {
            Explode();

            if (collision.GetComponent<PlayerController>() != null)
            {
                OnPlayerHit?.Invoke();
                return;
            }

            OnCityHit?.Invoke(collision.gameObject);
        }
        else if (collision.tag == "Explosion")
        {
            OnPointsAwarded?.Invoke(cometDestroyedPoints, cometDestroyedMoney);
            Explode();
        }
    }

    private void Explode()
    {
        OnCometDestroyed?.Invoke();
        Instantiate(explosionAnim, transform.position, Quaternion.identity);

        if (targetCrosshair != null)
        {
            Destroy(targetCrosshair.gameObject);
        }
        Destroy(this.gameObject);
    }

    private void Split()
    {
        float minSplitHeight = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0)).y;

        if (minSplitHeight > transform.position.y)
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
            OnCometSplit?.Invoke(1);
        }
    }
}
