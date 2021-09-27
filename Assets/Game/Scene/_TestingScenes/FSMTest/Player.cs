using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdleState : IState
{
    Player player;

    public IdleState(Player player)
    {
        this.player = player;
    }

    public void Enter(params object[] args)
    {

    }

    public void Execute(float dt)
    {
        
    }

    public void Exit()
    {
        
    }

    public void HandleInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            bool attackHasHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit attackHit, Mathf.Infinity, player.attackLayer);
            bool groundHasHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit groundHit, Mathf.Infinity, player.groundLayer);
            if (attackHasHit)
            {
                player.stateMachine.Change("chase", new object[] { attackHit.collider.transform });
            }
            else if (groundHasHit)
            {
                player.stateMachine.Change("walk", new object[] { groundHit.point });
            }
        }
    }
}
public class WalkState : IState
{
    public const float STOP_EPSILON = 0.1f;
    Vector3 targetPoint;
    Player player;

    public WalkState(Player player)
    {
        this.player = player;
    }

    public void Enter(params object[] args)
    {
        targetPoint = (Vector3) args[0];
        this.player.agent.SetDestination(targetPoint);
    }

    public void Execute(float dt)
    {
        if (Input.GetMouseButtonDown(1))
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, player.groundLayer);
            targetPoint = hit.point;
            player.agent.SetDestination(targetPoint);
        }
    }

    public void Exit()
    {
        
    }

    public void HandleInput()
    {
        Vector3 position = new Vector3(this.player.agent.transform.position.x, 0, this.player.agent.transform.position.z);
        if(Vector3.Distance(targetPoint, position) < STOP_EPSILON)
        {
            this.player.stateMachine.Change("idle");
        }

        if (Input.GetMouseButtonDown(1))
        {
            bool attackHasHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit attackHit, Mathf.Infinity, player.attackLayer);
            if (attackHasHit)
            {
                player.stateMachine.Change("chase", new object[] { attackHit.collider.transform });
            }
        }
    }
}
public class ChaseState : IState
{
    Player player;
    Transform target;

    public ChaseState(Player player)
    {
        this.player = player;
    }

    public void Enter(params object[] args)
    {
        target = (Transform) args[0];
    }

    public void Execute(float dt)
    {
        if (Input.GetMouseButtonDown(1))
        {
            bool attackHasHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit attackHit, Mathf.Infinity, player.attackLayer);
            if (attackHasHit)
            {
                target = attackHit.collider.transform;
            }
        }

        Vector3 direction = (target.position - player.transform.position).normalized;
        player.agent.Move(direction * dt);
    }

    public void Exit()
    {
        
    }

    public void HandleInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            bool groundHasHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit groundHit, Mathf.Infinity, player.groundLayer);
            if (groundHasHit)
            {
                player.stateMachine.Change("walk", new object[] { groundHit.point });
            }
        }
    }
}
public class AttackState : IState
{
    Player player;
    public AttackState(Player player)
    {
        this.player = player;
    }

    public void Enter(params object[] args)
    {
        
    }

    public void Execute(float dt)
    {
        
    }

    public void Exit()
    {
        
    }

    public void HandleInput()
    {
        
    }
}

public class Player : MonoBehaviour
{
    public StateMachine stateMachine = new StateMachine();
    public LayerMask groundLayer;
    public LayerMask attackLayer;
    public NavMeshAgent agent;
    public float speed = 3f;

    public string currentState;

    private void Start()
    {
        stateMachine.Add("idle", new IdleState(this));
        stateMachine.Add("walk", new WalkState(this));
        stateMachine.Add("attack", new AttackState(this));
        stateMachine.Add("chase", new ChaseState(this));

        stateMachine.Change("idle");
    }

    private void Update()
    {
        currentState = stateMachine.GetCurrentState();
        stateMachine.HandleInput();
        stateMachine.Execute(Time.deltaTime);
    }
}
