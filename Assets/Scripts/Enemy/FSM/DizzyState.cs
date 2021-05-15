using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class DizzyState : State
{
    float timer;
    public DizzyState(Enemy enemy, EnemyAnimation anim) : base(enemy, anim)
    {
    }    

    public override void Enter()
    {
        base.Enter();
        enemy.SirenStop();
        enemy.InitializeAll();  //���� �ʱ�ȭ
        anim.PlayDizzyAnim();    //Dizzy �ִϸ��̼� ����        
        enemy.SetNavMeshAgent();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        timer += Time.deltaTime;    //Ÿ�̸ӷ� ������ �ο�
        if (timer > 4f)
        {
            timer = 0.0f;
            anim.PlayDizzyAnim();
            enemy.ChangeToIdle();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }   
}
