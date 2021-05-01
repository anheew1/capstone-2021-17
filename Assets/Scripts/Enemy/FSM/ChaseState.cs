using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class ChaseState : State
{
    AudioSource siren;
    public ChaseState(Enemy enemy, StateMachine stateMachine, EnemyAnimation ani, NavMeshAgent navMeshAgent) : base(enemy, stateMachine, ani, navMeshAgent)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        siren = enemy.siren;    //���̷� ����
        enemy.InitializeVar();  //���� �ʱ�ȭ
        enemy.turnOnSensor = true;  //���� ��
        enemy.hasDestination = true;    //�ȴ� �ִϸ��̼� ����
        navMeshAgent.speed += navMeshAgent.speed * 0.005f;  //�ӵ� ������ ����
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (!siren.isPlaying)   //���̷��� �︮�� ���� �ƴϸ� ����
        {
            siren.Play();
        }
        
        navMeshAgent.SetDestination(enemy.target.position);    //Ÿ������ �̵�
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
