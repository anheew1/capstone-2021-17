using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PatrolState : State
{
    private Vector3 patrolPos;    

    public PatrolState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.InitializeAll();          //���� �ʱ�ȭ        
        enemy.hasDestination = true;    //�ȴ� �ִϸ��̼� ����
        enemy.MoveToWayPoint();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();        
        enemy.FindTargets();
        //���� �Ÿ��� ������ �ٽ� ���� ����Ʈ ����
        if (enemy.DistanceXZ() <= enemy.minErrorWayPoint)
        {
            enemy.ChangeToPatrol();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

    }

    
}
