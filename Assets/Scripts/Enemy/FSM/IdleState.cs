using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class IdleState : State
{
    float timer;

    public IdleState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.InitializeAll();  //���� �ʱ�ȭ
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        timer += Time.deltaTime;    //Ÿ�̸ӷ� ������ �ο�
        if (timer > 5f)
        {
            timer = 0.0f;
            enemy.ControlNavMesh();
            enemy.ChangeToPatrol();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
