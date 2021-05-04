using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Scripts.Structs;

public static class FileManager
{

    private static string SAVE_PATH = Application.persistentDataPath + "/data.dat";
    private static string SAVE_SLOT_1_PATH = Application.persistentDataPath + "/saves/slot1.dat";
    private static string SAVE_SLOT_2_PATH = Application.persistentDataPath + "/saves/slot2.dat";
    private static string SAVE_SLOT_3_PATH = Application.persistentDataPath + "/saves/slot3.dat";

    public static void SaveLeaderboard(List<LeaderboardEntry> entries)
    {
        //create binary formatter
        BinaryFormatter mFormatter = new BinaryFormatter();

        //dump list to file
        FileStream fs = new FileStream(SAVE_PATH, FileMode.Create);
        mFormatter.Serialize(fs, entries);
        fs.Close();
    }

    public static void SaveInfoToSlot(int slotNum) 
    {
        string pathToUse;
        switch (slotNum)
        {
            case 1:
                pathToUse = SAVE_SLOT_1_PATH;
                break;
            case 2:
                pathToUse = SAVE_SLOT_2_PATH;
                break;
            case 3:
                pathToUse = SAVE_SLOT_3_PATH;
                break;
            default:
                pathToUse = "";
                break;
        }

        BinaryFormatter mFormatter = new BinaryFormatter();

        FileStream fs = new FileStream(pathToUse, FileMode.Create);

    }

    public static GameInfo LoadGame(int slotNum) 
    {
        string pathToUse;
        switch (slotNum)
        {
            case 1:
                pathToUse = SAVE_SLOT_1_PATH;
                break;
            case 2:
                pathToUse = SAVE_SLOT_2_PATH;
                break;
            case 3:
                pathToUse = SAVE_SLOT_3_PATH;
                break;
            default:
                return null;
        }

        if (File.Exists(pathToUse))
        {
            BinaryFormatter mFormatter = new BinaryFormatter();
            FileStream fs = new FileStream(pathToUse, FileMode.Open);

            GameInfo info = mFormatter.Deserialize(fs) as GameInfo;
            fs.Close();
            return info;
        }
        else 
        {
            return null;
        }
    }

    public static List<LeaderboardEntry> LoadLeaderboard()
    {
        if (File.Exists(SAVE_PATH))
        {
            BinaryFormatter mFormatter = new BinaryFormatter();
            FileStream fs = new FileStream(SAVE_PATH, FileMode.Open);

            List<LeaderboardEntry> entires = mFormatter.Deserialize(fs) as List<LeaderboardEntry>;
            fs.Close();
            return entires;
        }
        else
        {
            Debug.LogError("Save File Not Found!");
            return null;
        }
    }
}
