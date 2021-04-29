using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{

    public MoveState(Enemy enemy, StateMachine stateMachine, EnemyAnimation ani) : base(enemy, stateMachine, ani)
    {
    }

    public override void Enter()
    {
        base.Enter();
        siren = enemy.siren;
    }

    public override void Exit()
    {
        base.Exit();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        //�������̸�
        if (enemy.isPatrol)
        {
            //��θ� �����ְ�
            enemy.hasDestination = true;
            //��ο� �����ϸ�
            /*���� ������ ��Ȯ�ϰ� �����ϴ� �� �������� �ɸ� 
             * ����� ������ ���� �ʴ� ��찡 ���� 
             * ���� ������ �����ؼ� ������ �����ϵ��� �Ѵ�.
             */
            if (enemy.DistanceXZ(enemy.transform.position, enemy.patrolPos) <= enemy.minErrorWayPoint)
            {
                //�ʱ�ȭ
                enemy.hasDestination = false;
                enemy.isPatrol = false;
                stateMachine.ChangeState(enemy.patrol);
            }
        }
        else
        {
            if (!siren.isPlaying)
            {
                siren.Play();
            }
            //����ؼ� ��θ� �����ؼ� �÷��̾ �������� �� ��θ� �ٽ� �����Ѵ�.
            enemy.navMeshAgent.SetDestination(enemy.target.position);
            //Ÿ���� ���������Ƿ� Ÿ�� ���� ���� �ʱ�ȭ
            /* NavMesh���� �÷��̾ ���� ������
            if (!enemy.hasPath)
            {
                state = State.Idle;
            }
            */
            
        }
    }

}
