using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.Attributes;

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
        Health health = obj.GetComponent<Health>();

        if(health == null) { return; }

        if (floatingBars.ContainsKey(health))
        {
            FloatingBar floatingBar = floatingBars[health];

            floatingBars.Remove(health);

            Destroy(floatingBar.gameObject);
        }
    }

    private void VisibilityChecker_OnVisionEntityAdded(VisionEntity obj)
    {
        Team localPlayerTeam = playerManager.GetLocalChampion().GetTeam();

        Health health = obj.GetComponent<Health>();

        Mana mana = obj.GetComponent<Mana>();

        ITeamMember teamMember = obj.GetComponent<ITeamMember>();

        if (floatingBars.ContainsKey(health))
        {
            Debug.Log("Already Contains Key For: " + obj.name);
            return;
        }

        FloatingBar floatingBarInstance = Instantiate(floatingBarPrefab, floatingBarParent).GetComponent<FloatingBar>();

        floatingBarInstance.Setup(health, mana, localPlayerTeam, teamMember.GetTeam(), Vector3.up * 3);
        
        floatingBars.Add(health, floatingBarInstance);
    }
}
