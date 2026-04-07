using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Hands;

public class PinchRaySelector : MonoBehaviour
{
    [Header("Settings")]
    public bool isRightHand = true;
    public float pinchThreshold = 0.03f;    // meters between index tip and thumb tip
    public float rayDistance = 10f;
    public LayerMask sphereLayer;

    [Header("Visual (optional)")]
    public LineRenderer lineRenderer;

    private XRHandSubsystem handSubsystem;
    private bool wasPinching = false;

    void Start()
    {
        var subsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(subsystems);
        if (subsystems.Count > 0)
            handSubsystem = subsystems[0];

        if (lineRenderer != null)
            lineRenderer.positionCount = 2;
    }

    void Update()
    {
        if (handSubsystem == null) return;

        XRHand hand = isRightHand ? handSubsystem.rightHand : handSubsystem.leftHand;
        if (!hand.isTracked)
        {
            wasPinching = false;
            if (lineRenderer != null) lineRenderer.enabled = false;
            return;
        }

        bool isPinching = DetectPinch(hand);
        Vector3 rayOrigin = GetRayOrigin(hand);
        Vector3 rayDirection = GetRayDirection(hand);

        // Update line visual
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, rayOrigin);
            lineRenderer.SetPosition(1, rayOrigin + rayDirection * rayDistance);
        }

        // Hover — highlight sphere under ray
        UpdateHover(rayOrigin, rayDirection);

        // Pinch started this frame = select
        if (isPinching && !wasPinching)
            TrySelect(rayOrigin, rayDirection);

        wasPinching = isPinching;
    }

    bool DetectPinch(XRHand hand)
    {
        var indexTipJoint = hand.GetJoint(XRHandJointID.IndexTip);
        var thumbTipJoint = hand.GetJoint(XRHandJointID.ThumbTip);

        if (!indexTipJoint.TryGetPose(out Pose indexPose) ||
            !thumbTipJoint.TryGetPose(out Pose thumbPose))
            return false;

        return Vector3.Distance(indexPose.position, thumbPose.position) < pinchThreshold;
    }

    Vector3 GetRayOrigin(XRHand hand)
    {
        // Use wrist as origin
        var wristJoint = hand.GetJoint(XRHandJointID.Wrist);
        if (wristJoint.TryGetPose(out Pose wristPose))
            return wristPose.position;
        return Vector3.zero;
    }

    Vector3 GetRayDirection(XRHand hand)
    {
        // Aim from wrist toward index finger tip
        var wristJoint = hand.GetJoint(XRHandJointID.Wrist);
        var indexTipJoint = hand.GetJoint(XRHandJointID.IndexTip);

        if (wristJoint.TryGetPose(out Pose wristPose) &&
            indexTipJoint.TryGetPose(out Pose indexPose))
        {
            Vector3 dir = (indexPose.position - wristPose.position).normalized;
            if (dir != Vector3.zero) return dir;
        }

        // Fallback: use wrist forward
        if (wristJoint.TryGetPose(out Pose fallback))
            return fallback.forward;

        return Vector3.forward;
    }

    void UpdateHover(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, rayDistance, sphereLayer))
        {
            var handler = hit.collider.GetComponent<SelectionHandler>();
            if (handler != null)
                handler.SetHover(true);
        }
        else
        {
            // Clear all hovers
            var handlers = FindObjectsOfType<SelectionHandler>();
            foreach (var h in handlers)
                h.SetHover(false);
        }
    }

    void TrySelect(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, rayDistance, sphereLayer))
        {
            var handler = hit.collider.GetComponent<SelectionHandler>();
            if (handler != null)
                handler.SelectSphere();
        }
    }
}
