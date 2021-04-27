using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class EnemyControl : MonoBehaviour
{
    //���� ���µ�
    public enum State
    {
        Idle,
        Patrol,
        Move,
        Attack,
        Dizzy
    }    
    public State state;
    //���� �þ�
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public bool hasDestination = false;   //Walk �ִϸ��̼��� ����ϱ� ���� ����                   
    public bool findTargetVision = false;   //�þ߿� ���� ���Դ��� üũ        
    public bool findTargetSound = false;    //����� ������ ���� ���� �ƴ���        
    public bool isPatrol = false;   //���� ������ üũ
    public bool turnOnSensor = true;  //�þ߿� ����� ���� �¿���.      
    public Vector3 patrolPos;   //���� ��ġ ���    
    public float dis;   //�÷��̾���� �Ÿ�
    public Transform target;    //Ÿ���� ��ġ
    public List<Transform> visibleTargets = new List<Transform>();  //�þ߿� ���� ������ List

    [SerializeField] public NavMeshAgent enemy;  //AI  
    [SerializeField] public Transform[] wayPoint;   //WayPoint - public EnemySpawnManager���� ���� �Ҵ��� �̷�����ߵ�.   

    //���� �Ǵ� �ٰ�, ��ֹ����� �÷��̾�����
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private AnimationSoundEvent[] animationEvent;  //����� ������ ���� �ִϸ��̼� �̺�Ʈ    
    [SerializeField] private EnemyAnimation anim;    
    [SerializeField] private EnemyNetBehaviour enemyNet;
    [SerializeField] private float minErrorWayPoint = 0.5f;    //���� �����Ÿ��� �ּ� ���� 
    [SerializeField] private AudioSource siren;                 //���̷� ����� �ҽ�
    private Collider[] targetsInViewRadius = new Collider[4];   //OverlapSphereNonAlloc�� ���� ���
    private int targetsLength;  //Ÿ�� ����Ʈ�� ����    
    private int animationEventLength = 0;   //AnimationSoundEvent ������Ʈ�� ���� ������Ʈ�� legnth
    float timer; //�����̸� ���� Ÿ�̸� ����

    void Awake()
    {
        //animationEvent = FindObjectsOfType<AnimationSoundEvent>();
        animationEventLength = animationEvent.Length;
        Debug.Log(animationEventLength);
        Console.IsOpen = false;        
        //�⺻ ����
        state = State.Dizzy;
    }
    
    void Update()
    {
        if(enemyNet != null && !NetworkServer.active) // Client������ Enemy�� �������� ����
        {
            return;
        }
        
        switch (state)
        {
            case State.Idle:
                IdleState();
                break;
            case State.Patrol:
                PatrolState();
                break;
            case State.Move:
                MoveState();
                break;
            case State.Attack:
                AttackState();
                break;
            case State.Dizzy:
                DizzyState();
                break;
        }
        FindTargets();
    }   
    //Idle State
    void IdleState()
    {        
        //Ÿ�̸�, 5�� ������
        timer += Time.deltaTime;
        if (timer > 5f)
        {
            timer = 0.0f;
            enemy.isStopped = false;
            state = State.Patrol;
        }
    }

    void PatrolState()
    {        
        if (!isPatrol)
        {
            findTargetSound = false;
            //���������� �Ǵ�
            isPatrol = true;            
            turnOnSensor = true;
            //���� ����Ʈ �� �ϳ��� �������� ����
            int random = Random.Range(0, wayPoint.Length);            
            patrolPos = wayPoint[random].position;            
            //���� ����
            enemy.SetDestination(patrolPos);            
            //move state�� ��ȯ
            state = State.Move;            
        }
    }

    void MoveState()
    {
        //�������̸�
        if (isPatrol)
        {
            //��θ� �����ְ�
            hasDestination = true;
            //��ο� �����ϸ�
            /*���� ������ ��Ȯ�ϰ� �����ϴ� �� �������� �ɸ� 
             * ����� ������ ���� �ʴ� ��찡 ���� 
             * ���� ������ �����ؼ� ������ �����ϵ��� �Ѵ�.
             */
            if (DistanceXZ(transform.position, patrolPos) <= minErrorWayPoint)
            {
                //�ʱ�ȭ
                hasDestination = false;
                isPatrol = false;
                state = State.Patrol;                
            }            
        }       
        else
        {            
            //����ؼ� ��θ� �����ؼ� �÷��̾ �������� �� ��θ� �ٽ� �����Ѵ�.
            enemy.SetDestination(target.position);
            //Ÿ���� ���������Ƿ� Ÿ�� ���� ���� �ʱ�ȭ
            /* NavMesh���� �÷��̾ ���� ������
            if (!enemy.hasPath)
            {
                state = State.Idle;
            }
            */
        }
    }
    
    void AttackState()
    {
        siren.Stop();
        //NavMeshAgent ��� ����
        enemy.isStopped = true;
        //Ÿ�� �ʱ�ȭ
        visibleTargets.Clear();
        anim.PlayAttAnim();
        state = State.Idle;
    }

    void DizzyState()
    {
        //������ ����
        turnOnSensor = false;
        siren.Stop();
        visibleTargets.Clear();
        anim.PlayDizzyAnim();
        state = State.Idle;        
    }

    //�þ߿� ���� Ÿ���� ã�´�.
    void FindTargets()
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
    void SetTargetWithSensor()
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
            //�����ʱ�ȭ
            InitializeVar();
            state = State.Attack;
        }        
        else
        {
            hasDestination = true;
            state = State.Move;
        }
    }

    void FindTargetWithSound()
    {
        //����� �̺�Ʈ�� �߻��ϸ�
        if (findTargetSound)
        {
            //Ÿ�ٸ���Ʈ�� �߰� -> �ӽ� ���� (���� �÷��̾��� ���带 Ž��)
            for(int i=0; i<animationEventLength; i++)
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
    void FindVisibleTargets()
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

    //Scene���� �þ߰��� ���� ��ġ�� �մ� ���� �ߴ´�.
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    //���� �ʱ�ȭ �Լ�
    void InitializeVar()
    {
        hasDestination = false;        
        findTargetVision = false;
        findTargetSound = false;
        turnOnSensor = false;
        isPatrol = false;
    }

    //position�� x�� z�� ������ �Ÿ��� üũ�ϴ� �Լ�
    float DistanceXZ(Vector3 posFirst, Vector3 posSecond)
    {
        posFirst.y = 0.0f;
        posSecond.y = 0.0f;

        return Vector3.Distance(posFirst, posSecond);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (CompareTag("Bullet"))
        {
            state = State.Dizzy;
        }
    }
}