using Dota.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingBarManager : MonoBehaviour
{
    [SerializeField] VisionChecker visibilityChecker = null;

    [SerializeField] Transform floatingBarParent = null;
    [SerializeField] GameObject floatingBarPrefab = null;

    Dictionary<Health, FloatingBar> floatingBars = new Dictionary<Health, FloatingBar>();

    private void Awake()
    {
        visibilityChecker.OnVisionEntityAdded += VisibilityChecker_OnVisionEntityAdded;
        visibilityChecker.OnVisionEntityRemoved += VisibilityChecker_OnVisionEntityRemoved;
        visibilityChecker.OnVisionEntityVisionUpdated += VisibilityChecker_OnVisionEntityVisionUpdated;
    }

    private void VisibilityChecker_OnVisionEntityVisionUpdated(VisionEntity obj)
    {
        floatingBars[obj.GetComponent<Health>()].gameObject.SetActive(obj.GetVisible());
    }

    private void VisibilityChecker_OnVisionEntityRemoved(VisionEntity obj)
    {
        floatingBars.Remove(obj.GetComponent<Health>());
    }

    private void VisibilityChecker_OnVisionEntityAdded(VisionEntity obj)
    {
        FloatingBar floatingBarInstance = Instantiate(floatingBarPrefab, floatingBarParent).GetComponent<FloatingBar>();
        floatingBarInstance.SetTarget(obj.gameObject, Vector3.up * 4);
        floatingBars.Add(obj.GetComponent<Health>(), floatingBarInstance);
    }
}
