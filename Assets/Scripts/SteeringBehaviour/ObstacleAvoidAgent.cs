﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidAgent : MonoBehaviour {
    public AIData m_Data;

    public Transform m_Target;
    public List<Obstacle> obstacles = new List<Obstacle>();

	void Update () {
        m_Data.m_vTarget = m_Target.position;
        if(SteeringBehavior.CollisionAvoid(m_Data, obstacles) == false)
        {
            SteeringBehavior.Seek(m_Data);
        }
        SteeringBehavior.Move(m_Data, obstacles);
    }

    private void OnDrawGizmos()
    {
        if (m_Data != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(this.transform.position, m_Data.m_fRadius);
            Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * this.m_Data.m_fProbeLength);
            Gizmos.color = Color.yellow;
            Vector3 vLeftStart = this.transform.position - this.transform.right * m_Data.m_fRadius;
            Vector3 vLeftEnd = vLeftStart + this.transform.forward * m_Data.m_fProbeLength;
            Gizmos.DrawLine(vLeftStart, vLeftEnd);
            Vector3 vRightStart = this.transform.position + this.transform.right * m_Data.m_fRadius;
            Vector3 vRightEnd = vRightStart + this.transform.forward * m_Data.m_fProbeLength;
            Gizmos.DrawLine(vRightStart, vRightEnd);
            Gizmos.DrawLine(vLeftEnd, vRightEnd);

        }
    }
}
