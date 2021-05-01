using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class DizzyState : State
{
    public DizzyState(Enemy enemy, StateMachine stateMachine, EnemyAnimation ani, NavMeshAgent navMeshAgent) : base(enemy, stateMachine, ani, navMeshAgent)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.siren.Stop();    //���̷� ����        

        enemy.InitializeVar();  //���� �ʱ�ȭ
        enemy.visibleTargets.Clear();   //Ÿ�� �ʱ�ȭ

        ani.PlayDizzyAnim();    //Dizzy �ִϸ��̼� ����
        stateMachine.ChangeState(enemy.idle);   //������Ʈ ����
    }

    public override void Exit()
    {
        base.Exit();
    }
   
}
