using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

    public static Main m_Instance;

    [SerializeField] private List<Obstacle> m_Obstacles;

    private void Awake()
    {
        m_Instance = this;
    }

    public List<Obstacle> GetObstacles(Obstacle except)
    {
        List<Obstacle> obstacles = new List<Obstacle>(m_Obstacles);
        obstacles.Remove(except);
        return obstacles;
    }


}
