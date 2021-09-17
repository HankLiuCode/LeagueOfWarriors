using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : NetworkBehaviour
{
    public static ObstacleManager instance;

    [SerializeField] MinionManager minionManager = null;
    [SerializeField] PlayerManager playerManager = null;
    [SerializeField] List<DotaObstacle> obstacles = new List<DotaObstacle>();

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
        
        minionManager.OnMinionAdded += MinionManager_OnMinionAdded;
        minionManager.OnMinionRemoved += MinionManager_OnMinionRemoved;

    }

    public static ObstacleManager GetInstance()
    {
        return instance;
    }

    private void MinionManager_OnMinionAdded(Minion minion)
    {
        DotaObstacle obstacle = minion.GetComponent<DotaObstacle>();
        obstacles.Add(obstacle);
    }

    private void MinionManager_OnMinionRemoved(Minion minion)
    {
        DotaObstacle obstacle = minion.GetComponent<DotaObstacle>();
        obstacles.Remove(obstacle);
    }

    public List<DotaObstacle> GetObstaclesExcept(DotaObstacle obstacle)
    {
        List<DotaObstacle> obstaclesExcept = new List<DotaObstacle>(obstacles);
        obstaclesExcept.Remove(obstacle);
        return obstaclesExcept;
    }

    public List<DotaObstacle> GetObstacles()
    {
        return obstacles;
    }
}
