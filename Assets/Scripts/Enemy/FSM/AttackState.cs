using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AttackState : State
{    
    public AttackState(Enemy enemy, EnemyAnimation anim) : base(enemy, anim)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SirenStop();
        enemy.InitializeAll();  //���� �ʱ�ȭ        
               
        anim.PlayAttAnim();      //���� �ִϸ��̼� ���
        enemy.ChangeToIdle();
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
