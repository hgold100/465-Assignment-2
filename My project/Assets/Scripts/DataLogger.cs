using UnityEngine;
using System.IO;
 
public class DataLogger : MonoBehaviour
{
    public string FilePath { get; private set; }
 
    void Start()
    {
        FilePath = Application.persistentDataPath 
                   + "/YourGroupName_Outputfile.csv";
 
        // Write header row
        string header = "TrialID,Technique,Amplitude,Width," +
                        "Direction,MT,Hit,ID,Throughput,Repetition";
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
                     $"{t.direction}," +
                     $"{mt:F4}," +
                     $"{(hit ? 1 : 0)}," +
                     $"{ID:F4}," +
                     $"{TP:F4}," +
                     $"{t.repetition}";
 
        File.AppendAllText(FilePath, row + "\n");
        Debug.Log("Logged row: " + row);
    }
}