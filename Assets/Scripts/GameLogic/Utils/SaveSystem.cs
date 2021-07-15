using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveSystem
{
    public static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";

    public static void init(){
        if (!Directory.Exists(SAVE_FOLDER)){
            Directory.CreateDirectory(SAVE_FOLDER);
        }
    }

    public static void save(string json){
        /*int saveNumber = 1;
        while(File.Exists("save_" + saveNumber + ".txt")){
            saveNumber++;
        }*/
        
        File.WriteAllText(SAVE_FOLDER + "/save.txt", json);
    }

    public static string load(){
        if(File.Exists(SAVE_FOLDER + "/save.txt")){
            string json = File.ReadAllText(SAVE_FOLDER + "/save.txt");
            return json;
        }
        else {
            return null;
        }
        
    }
}
