using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PatrolState : State
{
    public Vector3 patrolPos;
    private int randomIndex;
    System.Random random = new System.Random();
    public PatrolState(Enemy enemy, StateMachine stateMachine, EnemyAnimation ani, NavMeshAgent navMeshAgent) : base(enemy, stateMachine, ani, navMeshAgent)
    {
    }
    public float DistanceXZ(Vector3 posFirst, Vector3 posSecond)
    {
        posFirst.y = 0.0f;
        posSecond.y = 0.0f;

        return Vector3.Distance(posFirst, posSecond);
    }

    public override void Enter()
    {
        base.Enter();
        enemy.InitializeVar();  //���� �ʱ�ȭ
        enemy.turnOnSensor = true;  //���� ��
        enemy.hasDestination = true;    //�ȴ� �ִϸ��̼� ����
        navMeshAgent.speed = 0.5f;  // �ӵ� �ʱ�ȭ
        randomIndex = random.Next() % enemy.wayPoint.Length;    //���� ����
        patrolPos = enemy.wayPoint[randomIndex].position;   //���� ����Ʈ ����
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        navMeshAgent.SetDestination(patrolPos); //���� ����Ʈ�� �̵�

        //���� �Ÿ��� ������ �ٽ� ���� ����Ʈ ����
        if (DistanceXZ(enemy.transform.position, patrolPos) <= enemy.minErrorWayPoint)
        {                        
            stateMachine.ChangeState(enemy.patrol); //������Ʈ ����
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

    }
}
