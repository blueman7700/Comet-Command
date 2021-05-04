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

    private List<LeaderboardEntry> entries;

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
        //get up to 5 entries from the leaderboard to display.
        int i = 0;
        int j;
        while (i < entries.Count && i < 5)
        {
            names[i].text = entries[i].Name;
            scores[i].text = entries[i].Score.ToString();
            i++;
        }

        //set default text if there are not enough scores.
        if (i < 5)
        {
            for (j = i; j < 5; j++)
            {
                names[j].text = "- - -";
                scores[j].text = "- - -";
            }
        }
    }
}
