using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AttackState : State
{
    public AttackState(Enemy enemy, StateMachine stateMachine, EnemyAnimation ani, NavMeshAgent navMeshAgent) : base(enemy, stateMachine, ani, navMeshAgent)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        
        enemy.InitializeVar();  //���� �ʱ�ȭ
        enemy.visibleTargets.Clear();   //Ÿ�� �ʱ�ȭ        
        enemy.siren.Stop(); //���̷� ����        
        navMeshAgent.isStopped = true;  //NavMeshAgent ����
        ani.PlayAttAnim();  //���� �ִϸ��̼� ���
        stateMachine.ChangeState(enemy.idle);   //������Ʈ ����
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
