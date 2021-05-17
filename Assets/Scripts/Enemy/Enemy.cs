using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class Enemy : MonoBehaviour
{        
    
    [Range(0, 360)] [SerializeField] private float viewAngle;
    [SerializeField] private float viewRadius;
    [SerializeField] private float dis = 1000f;   //�÷��̾���� �Ÿ�  
    private bool hasDestination = false;   //Walk �ִϸ��̼��� ����ϱ� ���� ����    
    private bool findTargetVision = false;   //�þ߿� ���� ���Դ��� üũ
    private bool findTargetSound = false;    //����� ������ ���� ���� �ƴ���      
    private int randomIndex;
    private int targetsLength;  //Ÿ�� ����Ʈ�� ����
    private int animationEventLength;   //AnimationSoundEvent ������Ʈ�� ���� ������Ʈ�� legnth        
    
    //���� �Ǵ� �ٰ�, ��ֹ����� �÷��̾�����
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private Transform[] wayPoint;        //WayPoint - public EnemySpawnManager���� ���� �Ҵ��� �̷�����ߵ�.
    [SerializeField] private List<AnimationSoundEvent> animationEvent = new List<AnimationSoundEvent>();  //����� ������ ���� �ִϸ��̼� �̺�Ʈ
    [SerializeField] private EnemyNetBehaviour enemyNet;
    [SerializeField] private EnemyAnimation anim;       //���ʹ��� ���ϸ��̼��� ��Ʈ���ϴ� Ŭ����
    [SerializeField] private AudioSource siren;           //���̷� ����� �ҽ�
    [SerializeField] private NavMeshAgent navMeshAgent;   //AI
    [SerializeField] private Transform target;            //Ÿ���� ��ġ    
    [SerializeField] private Transform memTarget;
    [SerializeField] private Collider enemyCollider;

    private StateMachine enemyStateMachine;
    private PatrolState patrol;
    [SerializeField] private List<Transform> visibleTargets = new List<Transform>();  //�þ߿� ���� ������ List        
    private IdleState idle;
    private AttackState attack;
    private DizzyState dizzy;
    private ChaseState chase;            
    private Collider[] targetsInViewRadius = new Collider[4];   //OverlapSphereNonAlloc�� ���� ���

    #region Public Methods
    public void AddAnimationSoundEvent(AnimationSoundEvent animationSoundEvent)
    {
        animationEvent.Add(animationSoundEvent);
    }
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

    public void SetAgentPriority(int priority)
    {
        navMeshAgent.avoidancePriority = priority;
    }
    public void SetWayPoints(Transform[] wayPoints)
    {
        wayPoint = wayPoints;
    }
    
    public void SetNavMeshAgent()
    {
        navMeshAgent.isStopped = navMeshAgent.isStopped ? false : true;
    }
    public void InitializeAll()
    {                
        findTargetVision = false;       //���� �ʱ�ȭ
        findTargetSound = false;        //���� �ʱ�ȭ
        navMeshAgent.speed = 0.5f;      //�ӵ� �ʱ�ȭ
        visibleTargets.Clear();         //Ÿ�� ����Ʈ �ʱ�ȭ
        target = null;                  //Ÿ�� �ʱ�ȭ        
    }

    //�÷��̾� Ÿ���� ��ġ�� �̵��մϴ�.
    public void MoveToTarget()
    {        
        navMeshAgent.speed += navMeshAgent.speed * 0.0005f;     //���ʹ��� �ӵ��� ���� ������ŵ�ϴ�.
        anim.SetBlnedTree(navMeshAgent.speed);                  //���� Ʈ�� �� ����
        navMeshAgent.SetDestination(target.position);        
    }

    //���� �� ����ϴ� ��������Ʈ�� �̵��մϴ�.
    public void MoveToWayPoint()
    {
        randomIndex = Random.Range(0, 26);
        anim.SetBlnedTree(navMeshAgent.speed);      //Blend Tree �ʱ�ȭ
        navMeshAgent.SetDestination(wayPoint[randomIndex].position);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            ChangeToDizzy();
        }
    }

    public void ChangeToIdle()
    {
        navMeshAgent.ResetPath();
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

    public void ChangeToAttack()
    {
        if (dis <= 1.5f)
        {
            enemyStateMachine.ChangeState(attack);
        }        
    }

    public void SirenPlay()
    {
        if(enemyNet != null)
        {
            enemyNet.SirenPlay();
        }
        else
        {
            SirenPlaySync();
        }
    }

    public void SirenPlaySync()
    {
        if (!siren.isPlaying)
        {
            siren.Play();
        }
    }

    public void SirenStop()
    {
        if (enemyNet != null)
        {
            enemyNet.SirenStop();
        }
        else
        {
            SirenStopSync();
        }
    }

    public void SirenStopSync()
    {
        siren.Stop();
    }

    public void SoundSensorDetect()
    {
        findTargetSound = true;
    }

    public void SetCollider()
    {
        enemyCollider.isTrigger = enemyCollider.isTrigger ? false : true;
    }

    public float DistanceXZ()
    {
        Vector3 enemyPos = transform.position;
        Vector3 wayPointPos = wayPoint[randomIndex].position;
        enemyPos.y = 0.0f;
        wayPointPos.y = 0.0f;

        return Vector3.Distance(enemyPos, wayPointPos);
    }

    public void SetHasDestination(bool hasDestination)
    {
        this.hasDestination = hasDestination;
    }
    public bool GetHasDestination()
    {
        return hasDestination;
    }

    public void MemoState() //���� ������Ʈ�� ����մϴ�.
    {
        enemyStateMachine.latestState = enemyStateMachine.currentState;
    }

    public bool IsLatestStateAtt()
    {
        return enemyStateMachine.latestState == attack;
    }

    public bool IsLatestStateDizzy()
    {
        return enemyStateMachine.latestState == dizzy;
    }
    #endregion

    #region Private Methods
    private void SetTargetWithSensor()   //�þ߿� ���ο� �÷��̾ ������ ���� ���� �� ���� ����� Ÿ������ Ÿ�� ����
    {
        //������ ���� �������Ƿ� ���� Ž���ϴ� ���� �ʱ�ȭ
        findTargetVision = false;
        findTargetSound = false;
        memTarget = target;
        //Ÿ���� ���ϱ� ���� �ε��� ����
        int targetIndex = 0;
        Debug.Log(9);
        //Ÿ�ٵ��� �Ÿ� ��
        dis = Vector3.Distance(transform.position, visibleTargets[0].position);
        Debug.Log(10);
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
        Debug.Log(12);
        target = visibleTargets[targetIndex];        
        if (target != memTarget)
        {
            ChangeToChase();            
        }        
    }

    private void FindVisibleTargets()     //�þ߿� �÷��̾ �ִ��� ������ ã�´�.
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

    private void FindTargetWithSound()    //�ֺ��� �Ҹ��� ������ Ȯ���Ѵ�.
    {
        //����� �̺�Ʈ�� �߻��ϸ�
        if (findTargetSound)
        {            
            //Ÿ�ٸ���Ʈ�� �߰� -> �ӽ� ���� (���� �÷��̾��� ���带 Ž��)
            for (int i = 0; i < animationEvent.Count; i++)
            {
                Debug.Log(i);Debug.Log( "3" + animationEvent[i].CheckInArea());
                if (animationEvent[i].CheckInArea())
                {                    
                    Transform target = animationEvent[i].transform;
                    visibleTargets.Add(target);
                    findTargetSound = true;
                }
            }
        }
    }

    //������ ���ϸ��̼ǿ� ����ϴ� ��� �� �ʱ�ȭ

    private void ChangeToDizzy()
    {
        enemyStateMachine.ChangeState(dizzy);
    }

    #endregion

    private void Awake()
    {
        /*animationEvent = FindObjectsOfType<AnimationSoundEvent>();*/
        animationEventLength = animationEvent.Count;
        Debug.Log(animationEventLength);
        enemyStateMachine = new StateMachine();
        idle = new IdleState(this);
        patrol = new PatrolState(this);
        attack = new AttackState(this, anim);
        dizzy = new DizzyState(this, anim);
        chase = new ChaseState(this);

        enemyStateMachine.Initialize(idle);
    }

    private void FixedUpdate()
    {
        if (enemyNet != null && !NetworkServer.active) // Client������ Enemy�� �������� ����
        {
            return;
        }        
        enemyStateMachine.currentState.LogicUpdate();
    }
}