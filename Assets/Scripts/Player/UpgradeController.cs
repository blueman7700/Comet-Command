using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class UpgradeController : MonoBehaviour
{
    public static event Action<Upgrades> OnUpgrade;

    [SerializeField] private GameObject upgradeScreen;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private GameObject buyMissiles;
    [SerializeField] private GameObject betterBoosters;
    [SerializeField] private GameObject algorithms;
    [SerializeField] private GameObject cityRebuy;
    [SerializeField] private GameObject fastReload;


    private GameController gc;

    // Start is called before the first frame update
    void Start()
    {
        gc = FindObjectOfType<GameController>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuyMissiles()
    {
        OnUpgrade?.Invoke(Upgrades.MoreMissilesInStockpile);
        UpdateUI();
    }

    public void BetterBoosters()
    {
        OnUpgrade?.Invoke(Upgrades.FasterMissiles);
        UpdateUI();
    }

    public void FasterReloads()
    {
        OnUpgrade?.Invoke(Upgrades.FasterReload);
        UpdateUI();
    }

    public void CityRebuild()
    {
        OnUpgrade?.Invoke(Upgrades.CityRebuild);
        UpdateUI();
    }

    public void Algorithms()
    {
        OnUpgrade?.Invoke(Upgrades.CometPrediction);
        UpdateUI();
    }

    public void OpenUI()
    {
        UpdateUI();
        upgradeScreen.SetActive(true);
    }

    private void UpdateUI()
    {
        moneyText.text = string.Format("Money: $" + gc.Money);
        if (gc.AllowTargeting)
        {
            algorithms.SetActive(false);
        }
    }
}

public enum Upgrades 
{
    MoreMissilesInStockpile,
    FasterMissiles,
    CometPrediction,
    FasterReload,
    CityRebuild
}
