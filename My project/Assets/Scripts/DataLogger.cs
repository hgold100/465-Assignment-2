using UnityEngine;
using System.IO;
 
public class DataLogger : MonoBehaviour
{
    public string FilePath { get; private set; }
 
    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // On Quest, write to the public Downloads folder so you can access it
        // via USB file manager without ADB.
        FilePath = "/sdcard/Download/YourGroupName_Outputfile.csv";
#else
        FilePath = Application.persistentDataPath
                   + "/YourGroupName_Outputfile.csv";
#endif
 
        // Write header row
        string header = "TrialID,Technique,Amplitude,Width," +
                        "DirX,DirY,DirZ,MT,Hit,ID,Throughput,Repetition";
        File.WriteAllText(FilePath, header + "\n");
 
        Debug.Log("DataLogger ready. Saving to: " + FilePath);
    }
 
    public void LogRow(int trialID, Trial t, float mt, 
                       bool hit, float ID, float TP)
    {
        string row = $"{trialID}," +
                     $"{t.technique}," +
                     $"{t.amplitude}," +
                     $"{t.width}," +
                     $"{t.direction.x:F2},{t.direction.y:F2},{t.direction.z:F2}," +
                     $"{mt:F4}," +
                     $"{(hit ? 1 : 0)}," +
                     $"{ID:F4}," +
                     $"{TP:F4}," +
                     $"{t.repetition}";
 
        File.AppendAllText(FilePath, row + "\n");
        Debug.Log("Logged row: " + row);
    }
}