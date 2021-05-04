using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using Assets.Scripts.Structs;

/// <summary>
/// 
/// </summary>
public class GameController : MonoBehaviour
{
    public static event Action OnFireAllowed;
    public static event Action<int> OnGameEnd;
    public static event Action<int> OnCometSpawnRequest;

    [SerializeField] private GameObject endOfRoundPanel;
    [SerializeField] private GameObject cityPrefab;
    [SerializeField] private Texture2D cursor;
    [SerializeField] private int startMissileNum;
    [SerializeField] private int startCometNum;
    [SerializeField] private float difficultyScalingFactor;
    [SerializeField] private GameInfo info;

    [SerializeField] private TextMeshProUGUI mScoreText;
    [SerializeField] private TextMeshProUGUI mMoneyText;
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
    private static string MONEY_TEXT = "Money: ${0}";
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
    private float reloadTime = 3f;
    private int cityCount;
    private int bonusMultiplier;
    private AudioManager mAudioManager;
    private int pointsToNextCity = 10000;
    private GameObject[] cityContainers;
    private MenuManager mMenuManager;

    public bool AllowTargeting { get; private set; } = true;
    public int Money { get; private set; }
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
        PlayerController.OnPlayerFire += PlayerCanShoot;
        PlayerController.OnPlayerPause += TogglePause;
        CometBehaviour.OnCityHit += CityDestroyed;
        CometBehaviour.OnPlayerHit += PlayerHit;
        CometBehaviour.OnPointsAwarded += AddScorePoints;
        CometBehaviour.OnCometDestroyed += HandleCometDestroyed;
        CometBehaviour.OnCometSplit += HandleCometSplit;
        CometControl.OnCometSpawn += HandleCometSpawn;
        UpgradeController.OnUpgrade += Upgrade;

        cursorHotspot = new Vector2(cursor.width / 2f, cursor.height / 2f);
        Cursor.SetCursor(cursor, cursorHotspot, CursorMode.Auto);

        mCometController = FindObjectOfType<CometControl>();
        mAudioManager = FindObjectOfType<AudioManager>();
        mMenuManager = FindObjectOfType<MenuManager>();

        cityCount = FindObjectsOfType<CityBehaviour>().Length;
        cityContainers = GameObject.FindGameObjectsWithTag("CityContainer");

        //make sure all cities are in the correct state.
        bool cityExists;
        for (int i = 0; i < cityContainers.Length; i++)
        {
            cityExists = false;
            GameObject city = cityContainers[i].GetComponentInChildren<CityBehaviour>().gameObject;
            for (int j = 0; j < info.CityPositions().Length; j++)
            {
                if (i == info.CityPositions()[j])
                {
                    cityExists = true;
                }
            }

            if (!cityExists)
            {
                Destroy(city);
                CityDestroyed(city);
            }
        }

        //TODO: save upgrades
        MissilesRemaining = startMissileNum;
        CometsRemainingInRound = startCometNum;

        Score = info.Score();
        Money = info.Money();
        RoundNumber = info.RoundNumber();

        StartRound();
    }

    private void OnDestroy()
    {
        PlayerController.OnPlayerFire -= PlayerCanShoot;
        PlayerController.OnPlayerPause -= TogglePause;
        CometBehaviour.OnCityHit -= CityDestroyed;
        CometBehaviour.OnPlayerHit -= PlayerHit;
        CometBehaviour.OnPointsAwarded -= AddScorePoints;
        CometBehaviour.OnCometDestroyed -= HandleCometDestroyed;
        CometBehaviour.OnCometSplit -= HandleCometSplit;
        CometControl.OnCometSpawn -= HandleCometSpawn;
        UpgradeController.OnUpgrade -= Upgrade;
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
            info.setScore(Score);
            FileManager.AddEntryToLeaderboard(info.GetStats());
            OnGameEnd?.Invoke(Score);
            SceneManager.LoadScene("GameOver");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdateUI()
    {
        mScoreText.text = string.Format(STRING_TEXT, Score);
        mMoneyText.text = string.Format(MONEY_TEXT, Money);
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
        int seed = UnityEngine.Random.Range(1000, 10000);
        UnityEngine.Random.InitState(seed);
        info.setSeed(seed);

        endOfRoundPanel.SetActive(false);
        RoundNumber++;

        CometSpeed = 1f * Mathf.Pow(difficultyScalingFactor, RoundNumber);

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
        OnCometSpawnRequest?.Invoke(CometsRemainingInRound);
    }

    /// <summary>
    /// 
    /// </summary>
    private void HandleCometSpawn()
    {
        CometsRemainingInRound--;
    }

    /// <summary>
    /// 
    /// </summary>
    private void HandleCometDestroyed()
    {
        cometsAlive--;
        UpdateUI();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="numNewComets"></param>
    private void HandleCometSplit(int numNewComets)
    {
        cometsAlive += numNewComets;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private void PlayerCanShoot()
    {
        if ((numMissilesInLauncher > 0) && roundActive)
        {
            OnFireAllowed?.Invoke();
            HandlePlayerShot();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void PlayerHit()
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
    private void CityDestroyed(GameObject go)
    {
        cityCount--;
    }

    /// <summary>
    /// 
    /// </summary>
    private void HandlePlayerShot()
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
    private void AddScorePoints(int pointsToAdd, int money)
    {
        Score += pointsToAdd;
        Money += money;
        UpdateUI();
    }

    /// <summary>
    /// 
    /// </summary>
    public void ContinueClick()
    {
        Debug.Log("Click!");
        endOfRoundPanel.SetActive(false);

        //calculate new round settings
        if (RoundNumber % 5 == 0)
        {
            //every 5 rounds the number of comets increases.
            startCometNum += 5;
        }

        StartRound();
    }

    /// <summary>
    /// 
    /// </summary>
    public void SaveClick()
    {
        //get all active city positions
        List<int> cityIndicies = new List<int>();
        GameObject go;
        for (int i = 0; i < cityContainers.Length; i++)
        {
            go = cityContainers[i];
            if (go.GetComponentInChildren<CityBehaviour>() != null)
            {
                cityIndicies.Add(i);
            }
        }

        info.setCities(cityIndicies.ToArray());
        info.setScore(Score);
        info.setRoundNumber(RoundNumber);
        info.setNumMissiles(startMissileNum);
        if (AllowTargeting)
        {
            info.enableTargeting();
        }
        info.setReloadSpeed(reloadTime);


        FileManager.SaveInfoToSlot(info);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pauseState"></param>
    private void TogglePause(bool pauseState)
    {
        if (pauseState)
        {
            Time.timeScale = 0f;
            //show pause menu
        }
        else
        {
            Time.timeScale = 1f;
            //hide pause menu
        }
    }

    private void Upgrade(Upgrades u) 
    {
        switch (u)
        {
            case Upgrades.FasterReload:
                if (reloadTime > 0.5f)
                {
                    reloadTime -= 0.5f;
                }
                break;
            case Upgrades.MoreMissilesInStockpile:
                startMissileNum += 5;
                break;
            case Upgrades.CometPrediction:
                AllowTargeting = true;
                break;
            case Upgrades.CityRebuild:
                TryRebuildCity();
                break;
        }
    }

    private bool TryRebuildCity()
    {
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
            return cityCreated;
        }

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndOFRound()
    {
        yield return new WaitForSeconds(1);
        int numCitiesAlive = FindObjectsOfType<CityBehaviour>().Length;
        int missileBonus = (MissilesRemaining + numMissilesInLauncher) * REMAINING_MISSILES_BONUS;
        int cityBonus = numCitiesAlive * REMAINING_CITIES_BONUS;
        int totalBonus = missileBonus + cityBonus;

        totalBonus *= bonusMultiplier;

        Score += totalBonus;
        Money += (numCitiesAlive * 100);

        mCityBonus.text = cityBonus.ToString();
        mMissileBonus.text = missileBonus.ToString();
        mBonusMult.text = "x" + bonusMultiplier;
        mTotalBonus.text = totalBonus.ToString();
        mTotalScore.text = Score.ToString();

        TryRebuildCity();


        endOfRoundPanel.SetActive(true);
    }
}