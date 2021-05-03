using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class DizzyState : State
{
    public DizzyState(Enemy enemy, EnemyAnimation anim) : base(enemy, anim)
    {
    }    

    public override void Enter()
    {
        base.Enter();
        enemy.SirenStop();
        enemy.InitializeAll();  //���� �ʱ�ȭ                
        anim.PlayDizzyAnim();    //Dizzy �ִϸ��̼� ����
        enemy.ChangeToIdle();
    }

    public override void Exit()
    {
        base.Exit();
    }
   
}
