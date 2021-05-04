using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Assets.Scripts.Structs;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject savesPanel;
    [SerializeField] private GameObject creditPanel;
    [SerializeField] private TextMeshProUGUI[] scores;
    [SerializeField] private TextMeshProUGUI[] names;
    [SerializeField] private TextMeshProUGUI gameOverScore;
    [SerializeField] private Fade fader;

    [SerializeField] private TextMeshProUGUI slot1Name;
    [SerializeField] private TextMeshProUGUI slot1Score;
    [SerializeField] private TextMeshProUGUI slot1Round;

    [SerializeField] private TextMeshProUGUI slot2Name;
    [SerializeField] private TextMeshProUGUI slot2Score;
    [SerializeField] private TextMeshProUGUI slot2Round;

    [SerializeField] private TextMeshProUGUI slot3Name;
    [SerializeField] private TextMeshProUGUI slot3Score;
    [SerializeField] private TextMeshProUGUI slot3Round;

    private static string ROUND_TEXT = "Round: {0}";
    private static string SCORE_TEXT = "Score: {0}";

    private List<LeaderboardEntry> entries;
    private static MenuManager instance;
    private GameInfo info = new GameInfo();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        entries = new List<LeaderboardEntry>();
        GameController.OnGameEnd += UpdateGameOverUI;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        GameController.OnGameEnd -= UpdateGameOverUI;
    }

    public void StartGame()
    {
        fader.loadScene("GamePlayer");
    }

    public void MainMenu()
    {
        fader.loadScene("MainMenu");
    }

    public void Tutorial()
    {
        fader.loadScene("Tutorial");
    }

    public void Leaderboard()
    {
        UpdateLeaderboard();
        leaderboardPanel.SetActive(true);
    }

    public void Credits()
    {
        creditPanel.SetActive(true);
    }

    public void Saves()
    {

        //set up slot 1
        info = FileManager.LoadGame(1);
        if (info != null)
        {
            slot1Name.text = info.Username();
            slot1Round.text = string.Format(ROUND_TEXT, info.RoundNumber());
            slot1Score.text = string.Format(SCORE_TEXT, info.Score());
        }
        else 
        {
            slot1Name.text = "Empty";
            slot1Round.text = string.Format(ROUND_TEXT, 0);
            slot1Score.text = string.Format(SCORE_TEXT, 0);
        }

        //set up slot 2
        info = FileManager.LoadGame(2);
        if (info != null)
        {
            slot2Name.text = info.Username();
            slot2Round.text = string.Format(ROUND_TEXT, info.RoundNumber());
            slot2Score.text = string.Format(SCORE_TEXT, info.Score());
        }
        else
        {
            slot2Name.text = "Empty";
            slot2Round.text = string.Format(ROUND_TEXT, 0);
            slot2Score.text = string.Format(SCORE_TEXT, 0);
        }

        //set up slot 3
        info = FileManager.LoadGame(2);
        if (info != null)
        {
            slot3Name.text = info.Username();
            slot3Round.text = string.Format(ROUND_TEXT, info.RoundNumber());
            slot3Score.text = string.Format(SCORE_TEXT, info.Score());
        }
        else
        {
            slot3Name.text = "Empty";
            slot3Round.text = string.Format(ROUND_TEXT, 0);
            slot3Score.text = string.Format(SCORE_TEXT, 0);
        }


        savesPanel.SetActive(true);
    }

    public void LoadSlot1()
    {
        info = FileManager.LoadGame(1);
        if (info == null)
        {
            info = new GameInfo("test1", 1, true);
        }
        else 
        {
            info.UpdateActiveInfo();
        }
        StartGame();
    }

    public void LoadSlot2()
    {
        info = FileManager.LoadGame(2);
        if (info == null)
        {
            info = new GameInfo("test2", 2, true);
        }
        else
        {
            info.UpdateActiveInfo();
        }
        StartGame();
    }

    public void LoadSlot3()
    {
        info = FileManager.LoadGame(3);
        if (info == null)
        {
            info = new GameInfo("test3", 3, true);
        }
        else
        {
            info.UpdateActiveInfo();
        }
        StartGame();
    }

    public void UpdateGameOverUI(int score)
    {
        gameOverScore.text = string.Format(SCORE_TEXT, score);
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void UpdateLeaderboard()
    {
        entries = FileManager.LoadLeaderboard();

        //get up to 5 entries from the leaderboard to display.
        int k = 0;
        int l;

        if (entries != null)
        {
            while (k < entries.Count && k < 5)
            {
                names[k].text = entries[k].Name;
                scores[k].text = entries[k].Score.ToString();
                k++;
            }
        }

        //set default text if there are not enough scores.
        if (k < 5)
        {
            for (l = k; l < 5; l++)
            {
                names[l].text = "- - -";
                scores[l].text = "- - -";
            }
        }
    }

    public void SaveLeaderboard()
    {
        //sort the list so highest score is at the top.
        for (int i = 0; i < entries.Count; i++)
        {
            for (int j = i + 1; j < entries.Count; j++)
            {
                if (entries[j].Score > entries[i].Score)
                {
                    //swap values
                    LeaderboardEntry tmp = entries[i];
                    entries[i] = entries[j];
                    entries[j] = tmp;
                }
            }
        }

        FileManager.SaveLeaderboard(entries);
    }

    public void AddNewLeaderboardEntry(LeaderboardEntry entry)
    {
        entries = FileManager.LoadLeaderboard();
        entries.Add(entry);
        FileManager.SaveLeaderboard(entries);
    }
}
