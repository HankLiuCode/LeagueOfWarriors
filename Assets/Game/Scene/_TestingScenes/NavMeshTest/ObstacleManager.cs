using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : NetworkBehaviour
{
    public static ObstacleManager instance;

    [SerializeField] MinionManager minionManager = null;
    [SerializeField] PlayerManager playerManager = null;
    [SerializeField] List<Obstacle> obstacles = new List<Obstacle>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        if(minionManager != null)
        {
            minionManager.OnMinionAdded += MinionManager_OnMinionAdded;
            minionManager.OnMinionRemoved += MinionManager_OnMinionRemoved;
        }
    }

    public static ObstacleManager GetInstance()
    {
        return instance;
    }

    private void MinionManager_OnMinionAdded(Minion minion)
    {
        Obstacle obstacle = minion.GetComponent<Obstacle>();
        obstacles.Add(obstacle);
    }

    private void MinionManager_OnMinionRemoved(Minion minion)
    {
        Obstacle obstacle = minion.GetComponent<Obstacle>();
        obstacles.Remove(obstacle);
    }

    public List<Obstacle> GetObstaclesExcept(Obstacle obstacle)
    {
        List<Obstacle> obstaclesExcept = new List<Obstacle>(obstacles);
        obstaclesExcept.Remove(obstacle);
        return obstaclesExcept;
    }

    public List<Obstacle> GetObstacles()
    {
        return obstacles;
    }
}
