using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AttackState : State
{
    float timer;
    public AttackState(Enemy enemy, EnemyAnimation anim) : base(enemy, anim)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SirenStop();
        enemy.InitializeAll();
        anim.PlayAttAnim();      //���� �ִϸ��̼� ���              
        enemy.SetNavMeshAgent();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        timer += Time.deltaTime;    //Ÿ�̸ӷ� ������ �ο�
        if (timer > 1.5f)
        {
            timer = 0.0f;
            anim.PlayAttAnim();
            enemy.ChangeToIdle();
        }
    }

    public override void Exit()
    {
        base.Exit();        
    }
}
