using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class Enemy : MonoBehaviour
{

    [Range(0, 360)] public float viewAngle;
    public float viewRadius;

    public bool hasDestination = false;   //Walk �ִϸ��̼��� ����ϱ� ���� ����           
    public float minErrorWayPoint = 0.5f;   //���� �����Ÿ��� �ּ� ����    
    public Transform[] wayPoint;        //WayPoint - public EnemySpawnManager���� ���� �Ҵ��� �̷�����ߵ�.        
    private int randomIndex;
    //���� ������� ���� public���� ������ �͵� �Դϴ�.
    #region Debuging    
    private bool findTargetVision = false;   //�þ߿� ���� ���Դ��� üũ
    private bool findTargetSound = false;    //����� ������ ���� ���� �ƴ���
    private float dis;   //�÷��̾���� �Ÿ�    
    
    private StateMachine enemyStateMachine;
    [SerializeField] private NavMeshAgent navMeshAgent;   //AI    
    private PatrolState patrol;
    private List<Transform> visibleTargets = new List<Transform>();  //�þ߿� ���� ������ List
    [SerializeField] private Transform target;            //Ÿ���� ��ġ
    #endregion

    private IdleState idle;
    private AttackState attack;
    private DizzyState dizzy;
    private ChaseState chase;
    
    //���� �Ǵ� �ٰ�, ��ֹ����� �÷��̾�����
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private AnimationSoundEvent[] animationEvent;  //����� ������ ���� �ִϸ��̼� �̺�Ʈ        
    [SerializeField] private EnemyNetBehaviour enemyNet;        
    [SerializeField] private EnemyAnimation anim;       //���ʹ��� ���ϸ��̼��� ��Ʈ���ϴ� Ŭ����
    [SerializeField] private AudioSource siren;           //���̷� ����� �ҽ�

    private int targetsLength;  //Ÿ�� ����Ʈ�� ����
    private int animationEventLength;   //AnimationSoundEvent ������Ʈ�� ���� ������Ʈ�� legnth    
    private Collider[] targetsInViewRadius = new Collider[4];   //OverlapSphereNonAlloc�� ���� ���
    private System.Random random = new System.Random();

    public void FindTargets()   //���� ������ �þ߷� �÷��̾ ã�´�.
    {
        //�������� �۵�
        FindVisibleTargets();
        FindTargetWithSound();
        //������ ���� �÷��̾ ������
        if (findTargetVision || findTargetSound)
        {
            //�� �÷��̾ Ÿ������ ��´�.
            SetTargetWithSensor();
        }
    }

    
    public void SetTargetWithSensor()   //�þ߿� ���ο� �÷��̾ ������ ���� ���� �� ���� ����� Ÿ������ Ÿ�� ����
    {
        //�þ߿� ���� �������Ƿ� ���� Ž���ϴ� ���� �ʱ�ȭ
        findTargetVision = false;
        findTargetSound = false;
        //Ÿ���� ���ϱ� ���� �ε��� ����
        int targetIndex = 0;

        //Ÿ�ٵ��� �Ÿ� ��
        dis = Vector3.Distance(transform.position, visibleTargets[0].position);

        //���� ª�� �Ÿ��� ã�� ���� for��
        for (int i = 1; i < visibleTargets.Count; i++)
        {
            float temp = Vector3.Distance(transform.position, visibleTargets[i].position);
            if (dis > temp)
            {
                dis = temp;
                targetIndex = i;
            }
        }
        target = visibleTargets[targetIndex];
        //���� ���� ����
        //���� ���� ������ Attack ������Ʈ��, �ƴϸ� �״�� �߰�
        if (dis <= 1.5f)
        {
            enemyStateMachine.ChangeState(attack);
        }        
    }

    public void FindVisibleTargets()     //�þ߿� �÷��̾ �ִ��� ������ ã�´�.
    {
        //�þ߿� ���� Ÿ�ٵ��� �ʱ�ȭ
        visibleTargets.Clear();
        //�ֺ� �þ� ������ ���� Ÿ�ٵ��� ã�´�.
        targetsLength = Physics.OverlapSphereNonAlloc(transform.position, viewRadius, targetsInViewRadius, targetMask);

        //Ÿ�ٵ��� ũ�⸸ŭ for���� ���鼭 Ÿ���� �����ϰ� �þ߿� ���� ������ List�� �ִ´�.
        for (int i = 0; i < targetsLength; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                    //���� Ž���ϴ� ���� ����
                    findTargetVision = true;
                }
            }
        }
    }

    public void FindTargetWithSound()    //�ֺ��� �Ҹ��� ������ Ȯ���Ѵ�.
    {
        //����� �̺�Ʈ�� �߻��ϸ�
        if (findTargetSound)
        {
            //Ÿ�ٸ���Ʈ�� �߰� -> �ӽ� ���� (���� �÷��̾��� ���带 Ž��)
            for (int i = 0; i < animationEventLength; i++)
            {
                if (animationEvent[i].isInArea)
                {
                    Transform target = animationEvent[i].transform;
                    visibleTargets.Add(target);
                }
            }
        }
    }

    //������ ���ϸ��̼ǿ� ����ϴ� ��� �� �ʱ�ȭ
    public void InitializeAll()
    {
        hasDestination = false;
        findTargetVision = false;
        findTargetSound = false;
        navMeshAgent.speed = 0.5f;
        visibleTargets.Clear();
    }    

    //�÷��̾� Ÿ���� ��ġ�� �̵��մϴ�.
    public void MoveToTarget()
    {
        navMeshAgent.speed += navMeshAgent.speed * 0.0005f;     //���ʹ��� �ӵ��� ���� ������ŵ�ϴ�.
        navMeshAgent.SetDestination(target.position);
    }

    //���� �� ����ϴ� ��������Ʈ�� �̵��մϴ�.
    public void MoveToWayPoint()
    {
        randomIndex = random.Next() % wayPoint.Length;
        navMeshAgent.SetDestination(wayPoint[randomIndex].position);
    }

    //Scene���� �þ߰��� ���� ��ġ�� �մ� ���� �ߴ´�.
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void OnTriggerEnter(Collider other)
    {
        /*if (CompareTag("Bullet"))
        {            
        }*/
    }

    //NavMeshAgent�� �������̸� ���߰� ���������� �����Ų��.
    public void ControlNavMesh()
    {
        navMeshAgent.isStopped = navMeshAgent.isStopped ? true : false;
    }
    public void ChangeToIdle()
    {
        enemyStateMachine.ChangeState(idle);
    }
    public void ChangeToPatrol()
    {
        enemyStateMachine.ChangeState(patrol);
    }
    public void ChangeToChase()
    {
        enemyStateMachine.ChangeState(chase);
    }
    public void ChangeToDizzy()
    {
        enemyStateMachine.ChangeState(dizzy);
    }
    public void ChangeToAttack()
    {
        enemyStateMachine.ChangeState(attack);
    }

    public void SirenPlay()
    {
        if (!siren.isPlaying)
        {
            siren.Play();
        }
    }

    public void SirenStop()
    {
        siren.Stop();
    }

    public void SoundSensorDetect()
    {
        findTargetSound = true;
    }

    public float DistanceXZ()
    {
        Vector3 enemyPos = transform.position;
        Vector3 wayPointPos = wayPoint[randomIndex].position;
        enemyPos.y = 0.0f;
        wayPointPos.y = 0.0f;

        return Vector3.Distance(enemyPos, wayPointPos);
    }

    private void Awake()
    {
        animationEventLength = animationEvent.Length;
        enemyStateMachine = new StateMachine();
        idle = new IdleState(this);
        patrol = new PatrolState(this);
        attack = new AttackState(this, anim);
        dizzy = new DizzyState(this, anim);
        chase = new ChaseState(this);

        enemyStateMachine.Initialize(idle);
    }

    private void Update()
    {
        if (enemyNet != null && !NetworkServer.active) // Client������ Enemy�� �������� ����
        {
            return;
        }        
        enemyStateMachine.currentState.LogicUpdate();        
    }
}