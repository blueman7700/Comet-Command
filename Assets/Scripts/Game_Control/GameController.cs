using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject endOfRoundPanel;
    [SerializeField] private Texture2D cursor;
    [SerializeField] private int startMissileNum;
    [SerializeField] private int startCometNum;
    [SerializeField] private float difficultyScalingFactor;

    [SerializeField] private TextMeshProUGUI mScoreText;
    [SerializeField] private TextMeshProUGUI mRoundText;
    [SerializeField] private TextMeshProUGUI mCometText;
    [SerializeField] private TextMeshProUGUI mMissileText;
    [SerializeField] private TextMeshProUGUI mCityBonus;
    [SerializeField] private TextMeshProUGUI mMissileBonus;
    [SerializeField] private TextMeshProUGUI mTotalBonus;
    [SerializeField] private TextMeshProUGUI mTotalScore;

    private static string STRING_TEXT = "Score: {0}";
    private static string ROUND_TEXT = "Round: {0}";
    private static string COMET_TEXT = "Comets Remaining:\n{0}/{1}";
    private static string MISSILE_TEXT = "Missiles:\n{0}/{1}";
    private static int REMAINING_MISSILES_BONUS = 25;
    private static int REMAINING_CITIES_BONUS = 100;

    private Vector2 cursorHotspot;
    private CometControl mCometController;
    private int cometsAlive;
    private bool roundActive = false;
    

    public int Score { get; private set; }
    public int RoundNumber { get; private set; }
    public int MissilesRemaining { get; private set; }
    public int CometsRemainingInRound { get; private set; }
    public float CometSpeed { get; private set; }


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
        CometSpeed = 1f;

        StartRound();
    }

    // Update is called once per frame
    void Update()
    {
        if (roundActive)
        {
            if (cometsAlive == 0)
            {
                Debug.Log("Round Over!");
                roundActive = false;
                StartCoroutine(EndOFRound());
            }
        }
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
        endOfRoundPanel.SetActive(false);
        RoundNumber++;

        CometsRemainingInRound = startCometNum;
        cometsAlive = CometsRemainingInRound;
        MissilesRemaining = startMissileNum;

        roundActive = true;

        UpdateUI();
        mCometController.BeginSpawning(CometsRemainingInRound);
    }


    public bool CanSpawnComet()
    {
        return CometsRemainingInRound > 0;
    }

    public void HandleCometSpawn()
    {
        CometsRemainingInRound--;
        UpdateUI();
    }

    public void HandleCometDestroyed()
    {
        cometsAlive--;
    }

    public bool PlayerCanShoot()
    {
        return (MissilesRemaining > 0) && roundActive;
    }

    public void HandlePlayerShot()
    {
        MissilesRemaining--;
        UpdateUI();
    }

    public void AddScorePoints(int pointsToAdd)
    {
        Score += pointsToAdd;
        UpdateUI();
    }

    public void OnClick()
    {
        endOfRoundPanel.SetActive(false);
        //TODO: calculate new round params

        cometSpeed *= difficultyScalingFactor;

        StartRound();
    }

    public IEnumerator EndOFRound()
    {
        yield return new WaitForSeconds(1);
        endOfRoundPanel.SetActive(true);
        int numCitiesAlive = FindObjectsOfType<CityBehaviour>().Length;
        int missileBonus = MissilesRemaining * REMAINING_MISSILES_BONUS;
        int cityBonus = numCitiesAlive * REMAINING_CITIES_BONUS;
        int totalBonus = missileBonus + cityBonus;

        Score += totalBonus;

        mCityBonus.text = cityBonus.ToString();
        mMissileBonus.text = missileBonus.ToString();
        mTotalBonus.text = totalBonus.ToString();
        mTotalScore.text = Score.ToString();
    }
}
