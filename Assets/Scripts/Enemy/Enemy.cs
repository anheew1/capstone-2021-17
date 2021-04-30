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
    public bool findTargetVision = false;   //�þ߿� ���� ���Դ��� üũ        
    public bool findTargetSound = false;    //����� ������ ���� ���� �ƴ���        
    public bool isPatrol = false;   //���� ������ üũ
    public bool turnOnSensor = true;  //�þ߿� ����� ���� �¿���.          
    private float dis;   //�÷��̾���� �Ÿ�
    
    public Vector3 patrolPos;   //���� ��ġ ���    

    public Transform target;    //Ÿ���� ��ġ
    
    public NavMeshAgent navMeshAgent;  //AI
    public Transform[] wayPoint;   //WayPoint - public EnemySpawnManager���� ���� �Ҵ��� �̷�����ߵ�.   
    public AudioSource siren;                 //���̷� ����� �ҽ�

    #region Variables
    public float minErrorWayPoint = 0.5f;    //���� �����Ÿ��� �ּ� ���� 

    public StateMachine enemyStateMachine;
    public IdleState idle;
    public PatrolState patrol;
    public AttackState attack;
    public MoveState move;
    public DizzyState dizzy;

    public List<Transform> visibleTargets = new List<Transform>();  //�þ߿� ���� ������ List
    //���� �Ǵ� �ٰ�, ��ֹ����� �÷��̾�����
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private AnimationSoundEvent[] animationEvent;  //����� ������ ���� �ִϸ��̼� �̺�Ʈ        
    [SerializeField] private EnemyNetBehaviour enemyNet;
    

    #endregion
    [SerializeField] private EnemyAnimation anim;

    private Collider[] targetsInViewRadius = new Collider[4];   //OverlapSphereNonAlloc�� ���� ���
    private int targetsLength;  //Ÿ�� ����Ʈ�� ����    
    private int animationEventLength = 0;   //AnimationSoundEvent ������Ʈ�� ���� ������Ʈ�� legnth
    float timer; //�����̸� ���� Ÿ�̸� ����  

    

    public void InitializeVar()
    {
        hasDestination = false;
        findTargetVision = false;
        findTargetSound = false;
        turnOnSensor = false;
        isPatrol = false;
    }
    public float DistanceXZ(Vector3 posFirst, Vector3 posSecond)
    {
        posFirst.y = 0.0f;
        posSecond.y = 0.0f;

        return Vector3.Distance(posFirst, posSecond);
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

    public void FindTargets()
    {
        //������ ������
        if (turnOnSensor)
        {
            //�������� �۵�
            FindVisibleTargets();
            FindTargetWithSound();
            //������ ���� ���� ������
            if (findTargetVision || findTargetSound)
            {
                if (!siren.isPlaying)
                {
                    siren.Play();
                }
                isPatrol = false;
                //�� ���� Ÿ������ ��´�.
                SetTargetWithSensor();
            }
        }
    }

    //�þ߿� ���ο� ���� ������ ���� ���� �� ���� ����� Ÿ������ Ÿ�� ����
    public void SetTargetWithSensor()
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
        else
        {
            hasDestination = true;
            navMeshAgent.speed += navMeshAgent.speed * 0.005f;
            enemyStateMachine.ChangeState(move);
        }
    }

    public void FindTargetWithSound()
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

    //�þ߿� ���� �ִ��� ������ ã�´�.
    public void FindVisibleTargets()
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

    private void Awake()
    {
        animationEventLength = animationEvent.Length;
        enemyStateMachine = new StateMachine();
        idle = new IdleState(this, enemyStateMachine, anim);
        patrol = new PatrolState(this, enemyStateMachine, anim);
        move = new MoveState(this, enemyStateMachine, anim);
        attack = new AttackState(this, enemyStateMachine, anim);
        dizzy = new DizzyState(this, enemyStateMachine, anim);

        enemyStateMachine.Initialize(idle);

    }

    private void Update()
    {
        if (enemyNet != null && !NetworkServer.active) // Client������ Enemy�� �������� ����
        {
            return;
        }
        enemyStateMachine.currentState.LogicUpdate();
        FindTargets();
    }

    private void FixedUpdate()
    {
        enemyStateMachine.currentState.PhysicsUpdate();
    }
}
