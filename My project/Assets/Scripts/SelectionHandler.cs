using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class SelectionHandler : MonoBehaviour
{
    [Header("Materials")]
    public Material blueMaterial;
    public Material greenMaterial;

    private Renderer rend;
    private XRSimpleInteractable interactable;
    private bool hasBeenSelected = false;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        interactable = GetComponent<XRSimpleInteractable>();

        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEnter);
            interactable.hoverExited.AddListener(OnHoverExit);
            interactable.selectEntered.AddListener(OnSelected);
        }
    }

    void Start()
    {
        if (blueMaterial != null)
            rend.material = blueMaterial;
    }

    void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (greenMaterial != null)
            rend.material = greenMaterial;
    }

    void OnHoverExit(HoverExitEventArgs args)
    {
        if (!hasBeenSelected && blueMaterial != null)
            rend.material = blueMaterial;
    }

    void OnSelected(SelectEnterEventArgs args)
    {
        if (hasBeenSelected) return;
        hasBeenSelected = true;

        TrialManager trialManager = Object.FindFirstObjectByType<TrialManager>();
        if (trialManager == null) return;

        if (gameObject.CompareTag("ReadySphere"))
        {
            trialManager.OnReadySpherePressed();
        }
        else
        {
            trialManager.RecordResult(hit: true);
            gameObject.SetActive(false);
        }
    }

    public void SetHover(bool hovering)
    {
        if (hasBeenSelected) return;
        if (hovering && greenMaterial != null)
            rend.material = greenMaterial;
        else if (!hovering && blueMaterial != null)
            rend.material = blueMaterial;
    }

    public void SelectSphere()
    {
        if (hasBeenSelected) return;
        hasBeenSelected = true;

        TrialManager trialManager = Object.FindFirstObjectByType<TrialManager>();
        if (trialManager != null)
            trialManager.RecordResult(hit: true);

        gameObject.SetActive(false);
    }

    public void ResetSphere()
    {
        hasBeenSelected = false;
        if (blueMaterial != null)
            rend.material = blueMaterial;
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEnter);
            interactable.hoverExited.RemoveListener(OnHoverExit);
            interactable.selectEntered.RemoveListener(OnSelected);
        }
    }
}
