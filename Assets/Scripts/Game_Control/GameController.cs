using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 
/// </summary>
public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject endOfRoundPanel;
    [SerializeField] private GameObject cityPrefab;
    [SerializeField] private Texture2D cursor;
    [SerializeField] private int startMissileNum;
    [SerializeField] private int startCometNum;
    [SerializeField] private float difficultyScalingFactor;

    [SerializeField] private TextMeshProUGUI mScoreText;
    [SerializeField] private TextMeshProUGUI mRoundText;
    [SerializeField] private TextMeshProUGUI mCometText;
    [SerializeField] private TextMeshProUGUI mMissileText;
    [SerializeField] private TextMeshProUGUI mStockpileText;

    [SerializeField] private TextMeshProUGUI mCityBonus;
    [SerializeField] private TextMeshProUGUI mMissileBonus;
    [SerializeField] private TextMeshProUGUI mTotalBonus;
    [SerializeField] private TextMeshProUGUI mTotalScore;
    [SerializeField] private TextMeshProUGUI mBonusMult;

    private static string STRING_TEXT = "Score: {0}";
    private static string ROUND_TEXT = "Round: {0}";
    private static string COMET_TEXT = "Comets Remaining:\n{0}/{1}";
    private static string MISSILE_TEXT = "Missiles:\n{0}/{1}";
    private static string STOCKPILE_TEXT = "Missiles In Stockpile: {0}";
    private static int REMAINING_MISSILES_BONUS = 25;
    private static int REMAINING_CITIES_BONUS = 100;

    private Vector2 cursorHotspot;
    private CometControl mCometController;
    private int cometsAlive;
    private bool roundActive = false;
    private bool playerIsDead = false;
    private int numMissilesInLauncher;
    private int missileMagazineSize = 10;
    private int cityCount;
    private int bonusMultiplier;
    private AudioManager mAudioManager;
    private int pointsToNextCity = 10000;
    private GameObject[] cityContainers;

    public int Score { get; private set; }
    public int RoundNumber { get; private set; }
    public int MissilesRemaining { get; private set; }
    public int CometsRemainingInRound { get; private set; }
    public float CometSpeed { get; private set; }

    // Start is called before the first frame update
    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        cursorHotspot = new Vector2(cursor.width / 2f, cursor.height / 2f);
        Cursor.SetCursor(cursor, cursorHotspot, CursorMode.Auto);

        mCometController = FindObjectOfType<CometControl>();
        mAudioManager = FindObjectOfType<AudioManager>();

        cityCount = FindObjectsOfType<CityBehaviour>().Length;

        MissilesRemaining = startMissileNum;
        CometsRemainingInRound = startCometNum;
        Score = 0;
        RoundNumber = 0;
        CometSpeed = 1f;

        cityContainers = GameObject.FindGameObjectsWithTag("CityContainer");

        StartRound();
    }

    // Update is called once per frame
    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (cometsAlive <= 0 && roundActive)
        {
            Debug.Log("Round Over!");
            roundActive = false;
            StartCoroutine(EndOFRound());
        }
        else if (cityCount <= 0 || playerIsDead)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdateUI()
    {
        mScoreText.text = string.Format(STRING_TEXT, Score);
        mRoundText.text = string.Format(ROUND_TEXT, RoundNumber);
        mCometText.text = string.Format(COMET_TEXT, cometsAlive, startCometNum);
        mMissileText.text = string.Format(MISSILE_TEXT, numMissilesInLauncher, missileMagazineSize);
        mStockpileText.text = string.Format(STOCKPILE_TEXT, MissilesRemaining);
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartRound()
    {
        endOfRoundPanel.SetActive(false);
        RoundNumber++;

        CometsRemainingInRound = startCometNum;
        cometsAlive = CometsRemainingInRound;
        MissilesRemaining = startMissileNum;


        if (RoundNumber < 3)
        {
            bonusMultiplier = 1;
        }
        else if (RoundNumber >= 3 && RoundNumber < 5)
        {
            bonusMultiplier = 2;
        }
        else if (RoundNumber >= 5 && RoundNumber < 7)
        {
            bonusMultiplier = 3;
        }
        else if (RoundNumber >= 7 && RoundNumber < 9)
        {
            bonusMultiplier = 4;
        }
        else if (RoundNumber >= 9 && RoundNumber < 11)
        {
            bonusMultiplier = 5;
        }
        else if (RoundNumber >= 11)
        {
            bonusMultiplier = 6;
        }


        roundActive = true;

        PlayerReload();
        mCometController.BeginSpawning(CometsRemainingInRound);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool CanSpawnComet()
    {
        return CometsRemainingInRound > 0;
    }

    /// <summary>
    /// 
    /// </summary>
    public void HandleCometSpawn()
    {
        CometsRemainingInRound--;
    }

    /// <summary>
    /// 
    /// </summary>
    public void HandleCometDestroyed()
    {
        cometsAlive--;
        UpdateUI();
    }

    public void HandleCometSplit(int numNewComets)
    {
        cometsAlive += numNewComets;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool PlayerCanShoot()
    {
        if ((numMissilesInLauncher > 0) && roundActive)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void PlayerHit()
    {
        numMissilesInLauncher = 0;
        if (MissilesRemaining <= 0)
        {
            MissilesRemaining = 0;
            if (numMissilesInLauncher <= 0)
            {
                playerIsDead = true;
            }
        }
        else 
        {
            //force the controller to reload the launcher.
            PlayerReload();
        }   
    }

    /// <summary>
    /// 
    /// </summary>
    public void CityDestroyed()
    {
        cityCount--;
    }

    /// <summary>
    /// 
    /// </summary>
    public void HandlePlayerShot()
    {
        numMissilesInLauncher--;
        if (numMissilesInLauncher <= 0)
        {
            PlayerReload();
        }
        
        UpdateUI();
    }

    /// <summary>
    /// 
    /// </summary>
    private void PlayerReload()
    {
        //TODO make reload delay and animation.
        if (MissilesRemaining < missileMagazineSize)
        {
            numMissilesInLauncher = MissilesRemaining;
        }
        else
        {
            numMissilesInLauncher = missileMagazineSize;
            MissilesRemaining -= missileMagazineSize;
        }
        UpdateUI();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pointsToAdd"></param>
    public void AddScorePoints(int pointsToAdd)
    {
        Score += pointsToAdd;
        UpdateUI();
    }

    /// <summary>
    /// 
    /// </summary>
    public void ContinueClick()
    {
        Debug.Log("Click!");
        endOfRoundPanel.SetActive(false);
        //TODO: calculate new round params

        CometSpeed *= difficultyScalingFactor;

        StartRound();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator EndOFRound()
    {
        yield return new WaitForSeconds(1);
        int numCitiesAlive = FindObjectsOfType<CityBehaviour>().Length;
        int missileBonus = (MissilesRemaining + numMissilesInLauncher) * REMAINING_MISSILES_BONUS;
        int cityBonus = numCitiesAlive * REMAINING_CITIES_BONUS;
        int totalBonus = missileBonus + cityBonus;

        totalBonus *= bonusMultiplier;

        Score += totalBonus;

        mCityBonus.text = cityBonus.ToString();
        mMissileBonus.text = missileBonus.ToString();
        mBonusMult.text = "x" + bonusMultiplier;
        mTotalBonus.text = totalBonus.ToString();
        mTotalScore.text = Score.ToString();

        if (Score > pointsToNextCity)
        {

            bool cityCreated = false;
            int i = 0;
            GameObject go;

            while (i < cityContainers.Length && !cityCreated)
            {
                go = cityContainers[i];
                if (go.GetComponentInChildren<CityBehaviour>() == null && cityCreated == false)
                {
                    Instantiate(cityPrefab, go.transform.position, Quaternion.identity, go.transform);
                    pointsToNextCity += 10000;
                    cityCreated = true;
                }
                i++;
            }
        }


        endOfRoundPanel.SetActive(true);
    }
}