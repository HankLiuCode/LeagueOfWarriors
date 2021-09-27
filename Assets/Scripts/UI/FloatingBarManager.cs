using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.Attributes;
using Mirror;
using Dota.Networking;

public class FloatingBarManager : NetworkBehaviour
{
    [SerializeField] Team localPlayerTeam;
    [SerializeField] VisionChecker visibilityChecker = null;

    [SerializeField] Transform floatingBarParent = null;
    [SerializeField] GameObject floatingBarPrefab = null;

    Dictionary<Health, FloatingBar> floatingBars = new Dictionary<Health, FloatingBar>();

    private void Awake()
    {
        Team team = NetworkClient.localPlayer.GetComponent<DotaRoomPlayer>().GetTeam();
        localPlayerTeam = team;

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

    private void VisibilityChecker_OnVisionEntityRemoved(VisionEntity visionEntity)
    {
        if(visionEntity == null)
        {
            List<Health> toRemove = new List<Health>();
            foreach(Health h in floatingBars.Keys)
            {
                if(h == null)
                {
                    toRemove.Add(h);
                }
            }

            foreach(Health r in toRemove)
            {
                floatingBars.Remove(r);
            }
            return;
        }

        Health health = visionEntity.GetComponent<Health>();

        if(health == null) { return; }

        if (floatingBars.ContainsKey(health))
        {
            FloatingBar floatingBar = floatingBars[health];

            floatingBars.Remove(health);

            Destroy(floatingBar.gameObject);
        }
    }

    private void VisibilityChecker_OnVisionEntityAdded(VisionEntity visionEntity)
    {
        Health health = visionEntity.GetComponent<Health>();

        Mana mana = visionEntity.GetComponent<Mana>();

        ITeamMember teamMember = visionEntity.GetComponent<ITeamMember>();

        if (floatingBars.ContainsKey(health))
        {
            Debug.Log("Already Contains Key For: " + visionEntity.name);
            return;
        }

        FloatingBar floatingBarInstance = Instantiate(floatingBarPrefab, floatingBarParent).GetComponent<FloatingBar>();

        floatingBarInstance.Setup(health, mana, localPlayerTeam, teamMember.GetTeam(), Vector3.up * health.GetDisplayOffset());

        floatingBarInstance.gameObject.SetActive(false);

        floatingBars.Add(health, floatingBarInstance);
    }
}
