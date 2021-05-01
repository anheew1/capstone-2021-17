using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class IdleState : State
{
    float timer;
    public IdleState(Enemy enemy, StateMachine stateMachine, EnemyAnimation ani, NavMeshAgent navMeshAgent) : base(enemy, stateMachine, ani, navMeshAgent)
    { 
    }
        
    public override void Enter()
    {
        base.Enter();
        enemy.InitializeVar();  //���� �ʱ�ȭ
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        timer += Time.deltaTime;    //Ÿ�̸ӷ� ������ �ο�
        if (timer > 5f)
        {
            timer = 0.0f;
            navMeshAgent.isStopped = false; //NavMeshAgent �����
            stateMachine.ChangeState(enemy.patrol); //������Ʈ ����
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
