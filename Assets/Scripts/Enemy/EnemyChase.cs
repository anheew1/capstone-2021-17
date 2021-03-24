using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Popcron.Console;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    //���� ���µ�
    public enum State
    {
        Idle,
        Patrol,
        Move,
        Attack
    }

    //���� �þ�
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    //Path�� �ִ���
    public bool hasP = false;
    //�÷��̾ ��Ҵ��� üũ    
    public bool isCatched = false;
    //�þ߿� ���� ������ List
    public List<Transform> visibleTargets = new List<Transform>();

    //���� �Ǵ� �ٰ�, ��ֹ����� �÷��̾�����
    [SerializeField]
    private LayerMask targetMask;
    [SerializeField]
    private LayerMask obstacleMask;
    [SerializeField]
    private AnimationEvent animationEvent;

    private Collider[] targetsInViewRadius = new Collider[4];
    private int targetsLength;
    [Command("state")]
    public State state;
    //Ÿ���� �����ߴ���
    [Command("setTarget")]
    public bool setTarget = false;
    //�þ߿� ���� ���Դ��� üũ    
    [Command("findTargetVision")]
    public bool findTargetVision = false;
    [Command("findTargetSound")]
    public bool findTargetSound = false;
    //���� ������ üũ
    [Command("isPatrol")]
    public bool isPatrol = false;
    //���� ��ġ ���      
    private Vector3 patrolPos;
    //�÷��̾���� �Ÿ�
    [Command("distance")]
    public float dis;    
    //AI
    private NavMeshAgent enemy;
    //Ÿ���� ��ġ
    public Transform target;
    //WayPoint
    [SerializeField]
    private Transform[] wayPoint;

    void Awake()
    {
        Console.IsOpen = false;
        //�⺻ ����
        state = State.Idle;
        //���� �ý����� �̿��ϱ� ���� �ʱ�ȭ
        enemy = GetComponent<NavMeshAgent>();        
        //�ڷ�ƾ�� �����ؼ� FSM�� ����.
        StartCoroutine("Run");
    }

    private void OnEnable()
    {
        Parser.Register(this, "enemy");
    }

    private void OnDisable()
    {
        Parser.Unregister(this);
    }
    IEnumerator Run()
    {
        //�׽� �þ߰� �����ȴ�.
        StartCoroutine("FindTargetsWithDelay");

        //ù �ڷ�ƾ�� �����ϸ� ���������� while���� ����.
        while (true)
        {
            switch (state)
            {
                case State.Idle:
                    yield return StartCoroutine("IdleState");
                    break;
                case State.Patrol:
                    yield return StartCoroutine("PatrolState");
                    break;                
                case State.Move:
                    yield return StartCoroutine("MoveState");
                    break;
                case State.Attack:
                    yield return StartCoroutine("AttackState");
                    break;
            }
        }
    }

    //Idle State
    IEnumerator IdleState()
    {
        yield return null;
        state = State.Patrol;
    }

    IEnumerator PatrolState()
    {
        if (!isPatrol)
        {
            //���� ���� �� 1�� ��ٸ���
            yield return new WaitForSeconds(1f);
            //���� ����Ʈ �� �ϳ��� �������� ����
            int random = Random.Range(0, 26);
            //���������� �Ǵ�
            isPatrol = true;            
            patrolPos = wayPoint[random].position;                        
            //move state�� ��ȯ
            state = State.Move;
            //���� ����
            enemy.SetDestination(patrolPos);            
        }
    }        

    IEnumerator MoveState()
    {                      
        while(state == State.Move)
        {
            //�������̸�
            if (isPatrol)
            {
                //��θ� �����ְ�
                hasP = true;     
                //��ο� �����ϸ�
                if ((int)transform.position.x == (int)patrolPos.x && (int)transform.position.z == (int)patrolPos.z)
                {
                    //�ʱ�ȭ
                    hasP = false;
                    isPatrol = false;
                    state = State.Patrol;
                }
                else
                {
                    yield return null;
                }                                
            }
            //���� ������ �ٽ� Idle ���·� ����.
            else if(isCatched)
            {
                //��ΰ� ���ٰ� �˸�
                // ��� ���� �ʱ�ȭ
                hasP = false;
                isPatrol = false;
                setTarget = false;
                isCatched = false;
                state = State.Idle;
            }
            else
            {
                //����ؼ� ��θ� �����ؼ� �÷��̾ �������� �� ��θ� �ٽ� �����Ѵ�.
                enemy.SetDestination(target.position);
                //Ÿ���� ���������Ƿ� Ÿ�� ���� ���� �ʱ�ȭ
                setTarget = false;
                yield return null;
            }            
        }        
    }
    
    IEnumerator AttackState()
    {
        yield return null;
    }

    //�þ߿� ���� Ÿ���� ã�´�.
    IEnumerator FindTargetsWithDelay()
    {
        while (true)
        {            
            FindVisibleTargets();
            FindTargetWithSound();
            //�þ߿� ���� ���� ������
            if (findTargetVision  || findTargetSound)
            {
                isPatrol = false;
                //�� ���� Ÿ������ ��´�.
                SetTargetWithSensor();
            }          
            yield return null;
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
            if (dis > Vector3.Distance(transform.position, visibleTargets[i].position))
            {
                dis = Vector3.Distance(transform.position, visibleTargets[i].position);
                targetIndex = i;
            }

        }
        target = visibleTargets[targetIndex];
        setTarget = true;
        hasP = true;
        state = State.Move;
    }

    void FindTargetWithSound()
    {
        if (animationEvent.audioEvent)
        {
            Transform target = animationEvent.transform;
            visibleTargets.Add(target);
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
}