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
        enemy.InitializeAll();  //���� �ʱ�ȭ               
        anim.PlayAttAnim();      //���� �ִϸ��̼� ���
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        timer += Time.deltaTime;    //Ÿ�̸ӷ� ������ �ο�
        if (timer > 1f)
        {
            timer = 0.0f;
            anim.PlayAttAnim();
            enemy.ChangeToIdle();
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemy.SetCollider();
    }
}
