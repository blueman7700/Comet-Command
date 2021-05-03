using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    [SerializeField] private Texture2D cursor;
    [SerializeField] private int startMissileNum;
    [SerializeField] private int startCometNum;
    [SerializeField] private float difficultyScalingFactor;
    [SerializeField] private TextMeshProUGUI mScoreText;
    [SerializeField] private TextMeshProUGUI mRoundText;
    [SerializeField] private TextMeshProUGUI mCometText;
    [SerializeField] private TextMeshProUGUI mMissileText;

    private static string STRING_TEXT = "Score: {0}";
    private static string ROUND_TEXT = "Round: {0}";
    private static string COMET_TEXT = "Comets Remaining:\n{0}/{1}";
    private static string MISSILE_TEXT = "Missiles:\n{0}/{1}";

    private Vector2 cursorHotspot;
    private CometControl mCometController;

    public int Score { get; private set; }
    public int RoundNumber { get; private set; }
    public int MissilesRemaining { get; private set; }
    public int CometsRemainingInRound { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        cursorHotspot = new Vector2(cursor.width / 2f, cursor.height / 2f);
        Cursor.SetCursor(cursor, cursorHotspot, CursorMode.Auto);

        mCometController = FindObjectOfType<CometControl>();

        MissilesRemaining = startMissileNum;
        CometsRemainingInRound = startCometNum;
        Score = 0;
        RoundNumber = 0;

        StartRound();
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateUI()
    {
        mScoreText.text = string.Format(STRING_TEXT, Score);
        mRoundText.text = string.Format(ROUND_TEXT, RoundNumber);
        mCometText.text = string.Format(COMET_TEXT, CometsRemainingInRound, startCometNum);
        mMissileText.text = string.Format(MISSILE_TEXT, MissilesRemaining, startMissileNum);
    }

    private void StartRound()
    {
        RoundNumber++;

        CometsRemainingInRound = startCometNum;
        MissilesRemaining = startMissileNum;

        mCometController.BeginSpawning(CometsRemainingInRound);
    }


    public bool CanSpawnComet()
    {
        return CometsRemainingInRound > 0;
    }

    public void HandleCometSpawn()
    {
        CometsRemainingInRound--;
        //TODO: handle new level and scoring...
        UpdateUI();
    }

    public bool PlayerCanShoot()
    {
        return MissilesRemaining > 0;
    }

    public void HandlePlayerShot()
    {
        MissilesRemaining--;
        //TODO: handle reloading or smth...
        UpdateUI();
    }

    public void AddScorePoints(int pointsToAdd)
    {
        Score += pointsToAdd;
        UpdateUI();
    }
}
