using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Assets.Scripts.Structs;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private TextMeshProUGUI[] scores;
    [SerializeField] private TextMeshProUGUI[] names;
    [SerializeField] private GameInfo info;

    private List<LeaderboardEntry> entries;
    private static MenuManager instance;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GamePlayer");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Leaderboard()
    {
        UpdateLeaderboard();
        leaderboardPanel.SetActive(true);
    }

    public void Options()
    { 
        
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
