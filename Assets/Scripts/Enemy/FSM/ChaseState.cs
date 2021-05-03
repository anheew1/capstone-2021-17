using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class ChaseState : State
{
    public ChaseState(Enemy enemy) : base(enemy)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        enemy.InitializeAll();          //���� �ʱ�ȭ
        enemy.hasDestination = true;    //�ȴ� �ִϸ��̼� ����
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        enemy.SirenPlay();
        enemy.FindTargets();
        enemy.MoveToTarget();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

}
