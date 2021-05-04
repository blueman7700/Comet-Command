using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UpgradeController : MonoBehaviour
{
    public static event Action<Upgrades> OnUpgrade;

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

    private void BuyMissiles()
    {
        OnUpgrade?.Invoke(Upgrades.MoreMissilesInStockpile);
    }

    private void BetterBoosters()
    {
        OnUpgrade?.Invoke(Upgrades.FasterMissiles);
    }

    private void FasterReloads()
    {
        OnUpgrade?.Invoke(Upgrades.FasterReload);
    }

    private void CityRebuild()
    {
        OnUpgrade?.Invoke(Upgrades.CityRebuild);
    }

    private void Algorithms()
    {
        OnUpgrade?.Invoke(Upgrades.CometPrediction);
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
