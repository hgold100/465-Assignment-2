using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject spherePrefab;
    public Transform cameraTransform;
    public TrialManager trialManager;

    [Header("Ready Sphere")]
    public GameObject readySpherePrefab;
    public Vector3 readySphereOffset = new Vector3(0f, 0f, 1.5f);

    private GameObject currentSphere;
    private GameObject currentReadySphere;
    private Vector3 lastSpherePosition;
    private bool hasLastPosition = false;

    public void SpawnTarget(Trial trial)
    {
        if (currentSphere != null)
            Destroy(currentSphere);

        Vector3 worldDirection = GetWorldDirection(trial.direction);
        Vector3 spawnPos = cameraTransform.position
                           + worldDirection * trial.amplitude;

        if (hasLastPosition)
            trial.actualAmplitude = Vector3.Distance(lastSpherePosition, spawnPos);
        else
            trial.actualAmplitude = trial.amplitude;

        lastSpherePosition = spawnPos;
        hasLastPosition = true;

        currentSphere = Instantiate(spherePrefab, spawnPos, Quaternion.identity);
        currentSphere.transform.localScale = Vector3.one * trial.width;

        SelectionHandler handler = currentSphere.GetComponent<SelectionHandler>();
        if (handler != null)
            handler.ResetSphere();

        // Timing is started in TrialManager.LoadTrial() — no OnSphereReady call needed
    }

    private Vector3 GetWorldDirection(Vector3 trialDirection)
    {
        Vector3 local = trialDirection.normalized;

        if (local.z < 0.5f)
        {
            local.z = 0.5f;
            local = local.normalized;
        }

        return cameraTransform.TransformDirection(local);
    }

    public void DestroyCurrentSphere()
    {
        if (currentSphere != null)
            Destroy(currentSphere);
    }

    public void SpawnReadySphere()
    {
        DestroyReadySphere();

        Vector3 pos = cameraTransform.position
                      + cameraTransform.TransformDirection(readySphereOffset);

        currentReadySphere = Instantiate(readySpherePrefab, pos, Quaternion.identity);
        currentReadySphere.tag = "ReadySphere";
    }

    public void DestroyReadySphere()
    {
        if (currentReadySphere != null)
        {
            Destroy(currentReadySphere);
            currentReadySphere = null;
        }
    }
}