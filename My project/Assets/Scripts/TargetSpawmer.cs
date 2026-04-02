using UnityEngine;
 
public class TargetSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject spherePrefab;
    public Transform cameraTransform;
    public TrialManager trialManager;
 
    private GameObject currentSphere;
    private Vector3 lastSpherePosition;
    private bool hasLastPosition = false;
 
    public void SpawnTarget(Trial trial)
    {
        // Clean up previous sphere
        if (currentSphere != null)
            Destroy(currentSphere);
 
        // Build world space direction relative to camera
        Vector3 worldDirection = GetWorldDirection(trial.direction);
 
        // Calculate spawn position
        Vector3 spawnPos = cameraTransform.position 
                           + worldDirection * trial.amplitude;
 
        // Calculate actual amplitude for Fitts Law
        if (hasLastPosition)
            trial.actualAmplitude = Vector3.Distance(
                                        lastSpherePosition, spawnPos);
        else
            trial.actualAmplitude = trial.amplitude;
 
        lastSpherePosition = spawnPos;
        hasLastPosition = true;
 
        // Spawn sphere
        currentSphere = Instantiate(spherePrefab, 
                                    spawnPos, 
                                    Quaternion.identity);
 
        // Set diameter
        currentSphere.transform.localScale = 
            Vector3.one * trial.width;
 
        // Reset sphere state
        SelectionHandler handler = 
            currentSphere.GetComponent<SelectionHandler>();
        if (handler != null)
            handler.ResetSphere();
 
        // Tell TrialManager the clock starts now
        trialManager.OnSphereReady(Time.time);
    }
 
    private Vector3 GetWorldDirection(Vector3 trialDirection)
    {
        Vector3 local = trialDirection.normalized;
 
        // Enforce minimum forward component
        // so sphere stays in field of view
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
}