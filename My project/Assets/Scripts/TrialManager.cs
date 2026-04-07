using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class TrialManager : MonoBehaviour
{
    [Header("References")]
    public TargetSpawner targetSpawner;
    public DataLogger dataLogger;

    [Header("HUD")]
    public TextMeshProUGUI trialNumberText;
    public TextMeshProUGUI techniqueText;
    public TextMeshProUGUI resultText;

    [Header("Trial List")]
    public List<Trial> trials = new List<Trial>();

    private int currentIndex = 0;
    private float trialStartTime = 0f;
    private bool experimentRunning = false;

    void Start()
    {
        SetupTrials();
        StartCoroutine(BeginExperiment());
    }

    IEnumerator BeginExperiment()
    {
        yield return new WaitForSeconds(1f);
        experimentRunning = true;

        // Show the first ready sphere immediately
        ShowReadySphere();
    }

    // Called by TargetSpawner when the ready sphere is pressed
    public void OnReadySpherePressed()
    {
        if (!experimentRunning) return;
        targetSpawner.DestroyReadySphere();
        LoadTrial();
    }

    void LoadTrial()
    {
        if (currentIndex >= trials.Count)
        {
            EndExperiment();
            return;
        }

        Trial t = trials[currentIndex];

        if (trialNumberText != null)
            trialNumberText.text = "Trial: " + (currentIndex + 1) + " / " + trials.Count;
        if (techniqueText != null)
            techniqueText.text = "Use: " + t.technique;
        if (resultText != null)
            resultText.text = "";

        // Start timing as soon as the target spawns
        trialStartTime = Time.time;
        targetSpawner.SpawnTarget(t);
    }

    // Called by SelectionHandler on hit
    public void RecordResult(bool hit)
    {
        if (!experimentRunning) return;

        float mt = Time.time - trialStartTime;
        Trial t = trials[currentIndex];

        float A = t.actualAmplitude > 0 ? t.actualAmplitude : t.amplitude;
        float W = t.width;
        float ID = Mathf.Log((A / W) + 1f, 2f);
        float TP = mt > 0 ? ID / mt : 0f;

        dataLogger.LogRow(currentIndex + 1, t, mt, hit, ID, TP);

        if (resultText != null)
        {
            resultText.text = hit ? "HIT" : "MISS";
            resultText.color = hit ? Color.green : Color.red;
        }

        currentIndex++;

        // Small delay to show result, then present the ready sphere
        StartCoroutine(ShowReadySphereAfterDelay());
    }

    IEnumerator ShowReadySphereAfterDelay()
    {
        yield return new WaitForSeconds(1.0f);

        if (currentIndex >= trials.Count)
        {
            EndExperiment();
            yield break;
        }

        ShowReadySphere();
    }

    void ShowReadySphere()
    {
        // Update HUD for the upcoming trial
        Trial next = trials[currentIndex];

        if (trialNumberText != null)
            trialNumberText.text = "Trial: " + (currentIndex + 1) + " / " + trials.Count;
        if (techniqueText != null)
            techniqueText.text = "Use: " + next.technique;
        if (resultText != null)
        {
            resultText.text = "Press the sphere to begin";
            resultText.color = Color.white;
        }

        targetSpawner.SpawnReadySphere();
    }

    void EndExperiment()
    {
        experimentRunning = false;
        targetSpawner.DestroyCurrentSphere();

        if (trialNumberText != null)
            trialNumberText.text = "Experiment Complete!";
        if (techniqueText != null)
            techniqueText.text = "";
        if (resultText != null)
        {
            resultText.text = "CSV saved to:\n" + dataLogger.FilePath;
            resultText.color = Color.white;
        }

        Debug.Log("Experiment complete. CSV at: " + dataLogger.FilePath);
    }

    // --- SetupTrials unchanged ---
    void SetupTrials()
    {
        trials.Clear();

        // Technique A: RayController — Rep 1
        trials.Add(new Trial { technique = "RayController", amplitude = 0.5f, width = 0.1f,  direction = new Vector3(0f, 0f, 1f),    repetition = 1 });
        trials.Add(new Trial { technique = "RayController", amplitude = 1.0f, width = 0.1f,  direction = new Vector3(0.5f, 0f, 1f),  repetition = 1 });
        trials.Add(new Trial { technique = "RayController", amplitude = 0.5f, width = 0.05f, direction = new Vector3(-0.5f, 0f, 1f), repetition = 1 });
        trials.Add(new Trial { technique = "RayController", amplitude = 1.0f, width = 0.05f, direction = new Vector3(0f, 0.4f, 1f),  repetition = 1 });

        // Technique A: RayController — Rep 2
        trials.Add(new Trial { technique = "RayController", amplitude = 0.5f, width = 0.1f,  direction = new Vector3(0f, 0f, 1f),    repetition = 2 });
        trials.Add(new Trial { technique = "RayController", amplitude = 1.0f, width = 0.1f,  direction = new Vector3(0.5f, 0f, 1f),  repetition = 2 });
        trials.Add(new Trial { technique = "RayController", amplitude = 0.5f, width = 0.05f, direction = new Vector3(-0.5f, 0f, 1f), repetition = 2 });
        trials.Add(new Trial { technique = "RayController", amplitude = 1.0f, width = 0.05f, direction = new Vector3(0f, 0.4f, 1f),  repetition = 2 });

        // Technique B: HandPinch — Rep 1
        trials.Add(new Trial { technique = "HandPinch", amplitude = 0.5f, width = 0.1f,  direction = new Vector3(0f, 0f, 1f),    repetition = 1 });
        trials.Add(new Trial { technique = "HandPinch", amplitude = 1.0f, width = 0.1f,  direction = new Vector3(0.5f, 0f, 1f),  repetition = 1 });
        trials.Add(new Trial { technique = "HandPinch", amplitude = 0.5f, width = 0.05f, direction = new Vector3(-0.5f, 0f, 1f), repetition = 1 });
        trials.Add(new Trial { technique = "HandPinch", amplitude = 1.0f, width = 0.05f, direction = new Vector3(0f, 0.4f, 1f),  repetition = 1 });

        // Technique B: HandPinch — Rep 2
        trials.Add(new Trial { technique = "HandPinch", amplitude = 0.5f, width = 0.1f,  direction = new Vector3(0f, 0f, 1f),    repetition = 2 });
        trials.Add(new Trial { technique = "HandPinch", amplitude = 1.0f, width = 0.1f,  direction = new Vector3(0.5f, 0f, 1f),  repetition = 2 });
        trials.Add(new Trial { technique = "HandPinch", amplitude = 0.5f, width = 0.05f, direction = new Vector3(-0.5f, 0f, 1f), repetition = 2 });
        trials.Add(new Trial { technique = "HandPinch", amplitude = 1.0f, width = 0.05f, direction = new Vector3(0f, 0.4f, 1f),  repetition = 2 });
    }
}