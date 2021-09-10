using Dota.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingBarManager : MonoBehaviour
{
    [SerializeField] VisionChecker visibilityChecker = null;
    [SerializeField] PlayerManager playerManager = null;

    [SerializeField] Transform floatingBarParent = null;
    [SerializeField] GameObject floatingBarPrefab = null;

    Dictionary<Health, FloatingBar> floatingBars = new Dictionary<Health, FloatingBar>();

    private void Awake()
    {
        visibilityChecker.OnVisionEntityAdded += VisibilityChecker_OnVisionEntityAdded;
        visibilityChecker.OnVisionEntityRemoved += VisibilityChecker_OnVisionEntityRemoved;
        visibilityChecker.OnVisionEntityEnter += VisibilityChecker_OnVisionEntityEnter;
        visibilityChecker.OnVisionEntityExit += VisibilityChecker_OnVisionEntityExit;
    }

    private void VisibilityChecker_OnVisionEntityExit(VisionEntity obj)
    {
        Health health = obj.GetComponent<Health>();
        if (floatingBars.ContainsKey(health))
        {
            floatingBars[health].gameObject.SetActive(false);
        }
    }

    private void VisibilityChecker_OnVisionEntityEnter(VisionEntity obj)
    {
        Health health = obj.GetComponent<Health>();
        if (floatingBars.ContainsKey(health))
        {
            floatingBars[health].gameObject.SetActive(true);
        }
    }

    private void VisibilityChecker_OnVisionEntityRemoved(VisionEntity obj)
    {
        floatingBars.Remove(obj.GetComponent<Health>());
    }

    private void VisibilityChecker_OnVisionEntityAdded(VisionEntity obj)
    {
        Team localPlayerTeam = playerManager.GetLocalChampion().GetTeam();

        Health health = obj.GetComponent<Health>();
        Mana mana = obj.GetComponent<Mana>();
        ITeamMember teamMember = obj.GetComponent<ITeamMember>();

        FloatingBar floatingBarInstance = Instantiate(floatingBarPrefab, floatingBarParent).GetComponent<FloatingBar>();

        floatingBarInstance.Setup(health, mana, localPlayerTeam, teamMember.GetTeam(), Vector3.up * 4);
        
        floatingBars.Add(obj.GetComponent<Health>(), floatingBarInstance);
    }
}
