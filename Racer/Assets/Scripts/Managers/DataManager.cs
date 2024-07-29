using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [SerializeField] private SO_Item_Database chasiesDB;
    [SerializeField] private SO_Item_Database enginesDB;
    [SerializeField] private SO_Item_Database tiresDB;
    [SerializeField] private SO_Item_Database powerSteeringDB;
    [SerializeField] private SO_Item_Database shockAbsorbersDB;
    [SerializeField] private SO_Item_Database brakesDB;


    private DirectoryInfo modeInfo_Indy = new DirectoryInfo("Assets/Resources/PlayerData/IndyMode");

    public SO_Item_Database Engines()
    {
        return enginesDB;
    }

    public SO_Item_Database Chasies()
    {
        return chasiesDB;
    }

    public SO_Item_Database Tires()
    {
        return tiresDB;
    }

    public SO_Item_Database PowerSteering()
    {
        return powerSteeringDB;
    }

    public SO_Item_Database ShockAbsorbers()
    {
        return shockAbsorbersDB;
    }

    public SO_Item_Database Brakes()
    {
        return brakesDB;
    }

    public SO_Item_Database SearchItems()
    {
        //return itemsDB;
        return null;
    }

    public List<string> GetAllIndyModeSaves()
    {
        FileInfo[] files = modeInfo_Indy.GetFiles();

        List<string> saves = new List<string>();

        foreach (FileInfo file in files)
        {
            saves.Add(file.Name);
        }
        return saves;
    }


    public bool CreateNewSave_IndyMode(string saveName, ref BaseCar car)
    {
        bool saveCreated = true;
        try
        {
            if (File.Exists(modeInfo_Indy.FullName + "/" + saveName + ".json"))
            {
                saveCreated = false;
            }
            else
            {
                string path = modeInfo_Indy.FullName + "/" + saveName + ".json";

                //IndyData data = new IndyData();
                //data.FileName = saveName + " Testing";

                string json = JsonUtility.ToJson(car);

                using (FileStream stream = File.Create(path))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(json);
                    }
                }

            }
        }
        catch
        {
            saveCreated = false;
        }


        return saveCreated;
    }

    public BaseCar LoadIndyModeSave(string saveName)
    {
        string path = modeInfo_Indy.FullName + "/" + saveName + ".json";

        string json = "";

        using (FileStream stream = File.Open(path, FileMode.Open))
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                json = reader.ReadToEnd();
            }
        }
        BaseCar car = ScriptableObject.CreateInstance<BaseCar>();
        //car = JsonUtility.FromJson<BaseCar>(json);
        JsonUtility.FromJsonOverwrite(json, car);
        //CarModel data = JsonUtility.FromJson<CarModel>(json);

        AssembleCar(ref car);

        return car;
    }

    private BaseCar AssembleCar(ref BaseCar car)
    {

        //car.engine = itemsDB.GetEngine(car.engineID);

        return car;
    }


}
