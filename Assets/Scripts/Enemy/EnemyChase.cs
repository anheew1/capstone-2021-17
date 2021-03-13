using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    //���� �þ�
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    //���� �Ǵ� �ٰ�, ��ֹ����� �÷��̾�����
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    //���� ���µ�
    enum State 
    {
        Idle,
        Patrol,        
        Move,
        Attack
    }

    private State state;

    //�þ߿� ���� ������ List
    public List<Transform> visibleTargets = new List<Transform>();
    
    // Initialize Players Position
    GameObject[] player;
    //has Path
    public bool hasP = false;
    public bool setTarget = false;
    //�÷��̾ ��Ҵ��� üũ
    public bool isCatched = false;
    //�þ߿� ���� ���Դ��� üũ
    public bool findTargetVision = false;
    //���� ������ üũ
    public bool isPatrol = false;
    //���� ��ġ ���
    public Vector3 patrolPos;
    //AI
    NavMeshAgent enemy;        
    //For Sort by distance
    Dictionary<string, float> distanceTarget;
    //WayPoint
    [SerializeField]
    private Transform[] wayPoint;
    // target
    public GameObject target;

    void Awake()
    {        
        //�⺻ ����
        state = State.Idle;
        //���� �ý����� �̿��ϱ� ���� �ʱ�ȭ
        enemy = GetComponent<NavMeshAgent>();        
        //�ڷ�ƾ�� �����ؼ� FSM�� ����.
        StartCoroutine("Run");
    }

    IEnumerator Run()
    {
        //�׽� �þ߰� �����ȴ�.
        StartCoroutine("FindTargetsWithDelay",0f);

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
        yield return new WaitForSeconds(5f);
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
            //������ 
            Debug.Log(wayPoint[random].gameObject.name);
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
                enemy.SetDestination(target.transform.position);
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
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {            
            FindVisibleTargets();
            //�þ߿� ���� ���� ������
            if (findTargetVision)
            {
                isPatrol = false;
                //�� ���� Ÿ������ ��´�.
                SetTargetWithVision();
            }
            yield return new WaitForSeconds(delay);
        }
    }

    //�þ߿� ���ο� ���� ������ ���� ���� �� ���� ����� Ÿ������ Ÿ�� ����
    void SetTargetWithVision()
    {
        //�þ߿� ���� �������Ƿ� ���� Ž���ϴ� ���� �ʱ�ȭ
        findTargetVision = false;

        //Ÿ�ٰ� ���� ��ġ�� �̸��� �°� �����ϱ� ���� ��ųʸ�
        distanceTarget = new Dictionary<string, float>();

        //���� ���� Ÿ�ٸ�ŭ ��ųʸ��� �߰�
        for (int i = 0; i < visibleTargets.Count; i++)
        {
            distanceTarget.Add(visibleTargets[i].gameObject.name, Vector3.Distance(transform.position, visibleTargets[i].position));
        }                        
        //��ųʸ��� �Ÿ��� ���� ����
        var ordered = distanceTarget.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        target = GameObject.Find(ordered.First().Key);        
        setTarget = true;
        hasP = true;
        state = State.Move;
    }
    

    //�þ߿� ���� �ִ��� ������ ã�´�.
    void FindVisibleTargets()
    {        
        //�þ߿� ���� Ÿ�ٵ��� �ʱ�ȭ
        visibleTargets.Clear();        
        //�ֺ� �þ� ������ ���� Ÿ�ٵ��� ã�´�.
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        //Ÿ�ٵ��� ũ�⸸ŭ for���� ���鼭 Ÿ���� �����ϰ� �þ߿� ���� ������ List�� �ִ´�.
        for (int i = 0; i < targetsInViewRadius.Length; i++)
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