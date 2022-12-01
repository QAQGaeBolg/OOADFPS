using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlimAI : MonoBehaviour
{
    private GameObject playerUnit;
    private Animator anim;
    private Vector3 initialPos;
    private float attackInterval;

    public float wanderRadius;          //���߰뾶���ƶ�״̬�£�����������߰뾶�᷵�س���λ��
    public float alertRadius;         //����뾶����ҽ�������ᷢ�����棬��һֱ�泯���
    public float defendRadius;          //�����뾶����ҽ��������׷����ң�������<����������ᷢ�����������ߴ���ս����
    public float chaseRadius;            //׷���뾶�������ﳬ��׷���뾶������׷��������׷����ʼλ��

    public float attackRange;            //��������
    public float walkSpeed;          //�ƶ��ٶ�
    public float runSpeed;          //�ܶ��ٶ�
    public float turnSpeed;         //ת���ٶȣ�����0.1


    private enum MonsterState
    {
        STAND,      //ԭ�غ���
        CHECK,       //ԭ�ع۲�
        WALK,       //�ƶ�
        WARN,       //�������
        CHASE,      //׷�����
        ATTACK,     //�������
        RETURN      //����׷����Χ�󷵻�
    }
    private MonsterState currentState = MonsterState.STAND;


    public float[] actionWeight = { 3000, 3000, 4000 };         //���ô���ʱ���ֶ�����Ȩ�أ�˳������Ϊ�������۲졢�ƶ�
    public float actRestTme;            //��������ָ��ļ��ʱ��
    private float lastActTime;          //���һ��ָ��ʱ��

    private float diatanceToPlayer;         //��������ҵľ���
    private float diatanceToInitial;         //�������ʼλ�õľ���
    private Quaternion targetRotation;         //�����Ŀ�곯��

    private bool is_Warned = false;
    private bool is_Running = false;

    private void SetAttackInterval()
    {
        attackInterval = Random.value * 3 + 3;
    }
    // Start is called before the first frame update
    void Start()
    {
        playerUnit = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();

        //�����ʼλ����Ϣ
        initialPos = transform.position;

        //��鲢������������
        //1. �����뾶�����ھ���뾶��������޷���������״̬��ֱ�ӿ�ʼ׷����
        defendRadius = Mathf.Min(alertRadius, defendRadius);
        //2. �������벻���������뾶��������޷�����׷��״̬��ֱ�ӿ�ʼս����
        attackRange = Mathf.Min(defendRadius, attackRange);
        //3. ���߰뾶������׷���뾶�����������ܸոտ�ʼ׷���ͷ��س�����
        wanderRadius = Mathf.Min(chaseRadius, wanderRadius);

        //���һ����������
        RandomAction();
    }

    /// <summary>
    /// ����Ȩ���������ָ��
    /// </summary>
    void RandomAction()
    {
        //�����ж�ʱ��
        lastActTime = Time.time;
        //����Ȩ�����
        float number = Random.Range(0, actionWeight[0] + actionWeight[1] + actionWeight[2]);
        if (number <= actionWeight[0])
        {
            currentState = MonsterState.STAND;
            anim.SetTrigger("Stand");
        }
        else if (actionWeight[0] < number && number <= actionWeight[0] + actionWeight[1])
        {
            currentState = MonsterState.CHECK;
            anim.SetTrigger("Check");
        }
        if (actionWeight[0] + actionWeight[1] < number && number <= actionWeight[0] + actionWeight[1] + actionWeight[2])
        {
            currentState = MonsterState.WALK;
            //���һ������
            targetRotation = Quaternion.Euler(0, Random.Range(1, 5) * 90, 0);
            anim.SetTrigger("Walk");
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            //����״̬���ȴ�actRestTme���������ָ��
            case MonsterState.STAND:
                if (Time.time - lastActTime > actRestTme)
                {
                    RandomAction();         //����л�ָ��
                }
                //��״̬�µļ��ָ��
                EnemyDistanceCheck();
                break;

            //����״̬�����ڹ۲춯��ʱ��ϳ�����ϣ�������������ţ��ʵȴ�ʱ���Ǹ���һ�����������Ĳ��ų��ȣ�������ָ����ʱ��
            case MonsterState.CHECK:
                if (Time.time - lastActTime > anim.GetCurrentAnimatorStateInfo(0).length)
                {
                    RandomAction();         //����л�ָ��
                }
                //��״̬�µļ��ָ��
                EnemyDistanceCheck();
                break;

            //���ߣ�����״̬���ʱ���ɵ�Ŀ��λ���޸ĳ��򣬲���ǰ�ƶ�
            case MonsterState.WALK:
                transform.Translate(Vector3.forward * Time.deltaTime * walkSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);

                if (Time.time - lastActTime > actRestTme)
                {
                    RandomAction();         //����л�ָ��
                }
                //��״̬�µļ��ָ��
                WanderRadiusCheck();
                break;

            //����״̬������һ�ξ��涯�����������������������λ��
            case MonsterState.WARN:
                if (!is_Warned)
                {
                    anim.SetTrigger("Warn");
                    gameObject.GetComponent<AudioSource>().Play();
                    is_Warned = true;
                }
                //�����������λ��
                targetRotation = Quaternion.LookRotation(playerUnit.transform.position - transform.position, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);
                //��״̬�µļ��ָ��
                WarningCheck();
                break;

            //׷��״̬�����������ȥ
            case MonsterState.CHASE:
                if (!is_Running)
                {
                    anim.SetTrigger("Run");
                    is_Running = true;
                }
                transform.Translate(Vector3.forward * Time.deltaTime * runSpeed);
                //�������λ��
                targetRotation = Quaternion.LookRotation(playerUnit.transform.position - transform.position, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);
                //��״̬�µļ��ָ��
                ChaseRadiusCheck();
                break;

            case MonsterState.ATTACK:
                if (attackInterval <= 0)
                {
                    anim.SetTrigger("Attack");
                }
                attackInterval -= Time.deltaTime;
                break;

            //����״̬������׷����Χ�󷵻س���λ��
            case MonsterState.RETURN:
                //�����ʼλ���ƶ�
                targetRotation = Quaternion.LookRotation(initialPos - transform.position, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * runSpeed);
                //��״̬�µļ��ָ��
                ReturnCheck();
                break;
        }
    }

    /// <summary>
    /// ԭ�غ������۲�״̬�ļ��
    /// </summary>
    void EnemyDistanceCheck()
    {
        diatanceToPlayer = Vector3.Distance(playerUnit.transform.position, transform.position);
        if (diatanceToPlayer < attackRange)
        {
            currentState = MonsterState.ATTACK;
        }
        else if (diatanceToPlayer < defendRadius)
        {
            currentState = MonsterState.CHASE;
        }
        else if (diatanceToPlayer < alertRadius)
        {
            currentState = MonsterState.WARN;
        }
    }

    /// <summary>
    /// ����״̬�µļ�⣬��������׷����ȡ������״̬
    /// </summary>
    void WarningCheck()
    {
        diatanceToPlayer = Vector3.Distance(playerUnit.transform.position, transform.position);
        if (diatanceToPlayer < defendRadius)
        {
            is_Warned = false;
            currentState = MonsterState.CHASE;
        }

        if (diatanceToPlayer > alertRadius)
        {
            is_Warned = false;
            RandomAction();
        }
    }

    /// <summary>
    /// ����״̬��⣬�����˾��뼰�����Ƿ�Խ��
    /// </summary>
    void WanderRadiusCheck()
    {
        diatanceToPlayer = Vector3.Distance(playerUnit.transform.position, transform.position);
        diatanceToInitial = Vector3.Distance(transform.position, initialPos);

        if (diatanceToPlayer < attackRange)
        {
            currentState = MonsterState.ATTACK;
        }
        else if (diatanceToPlayer < defendRadius)
        {
            currentState = MonsterState.CHASE;
        }
        else if (diatanceToPlayer < alertRadius)
        {
            currentState = MonsterState.WARN;
        }

        if (diatanceToInitial > wanderRadius)
        {
            //�������Ϊ��ʼ����
            targetRotation = Quaternion.LookRotation(initialPos - transform.position, Vector3.up);
        }
    }

    /// <summary>
    /// ׷��״̬��⣬�������Ƿ���빥����Χ�Լ��Ƿ��뿪���䷶Χ
    /// </summary>
    void ChaseRadiusCheck()
    {
        diatanceToPlayer = Vector3.Distance(playerUnit.transform.position, transform.position);
        diatanceToInitial = Vector3.Distance(transform.position, initialPos);

        if (diatanceToPlayer < attackRange)
        {
            currentState = MonsterState.ATTACK;
        }
        //�������׷����Χ���ߵ��˵ľ��볬���������ͷ���
        if (diatanceToInitial > chaseRadius || diatanceToPlayer > alertRadius)
        {
            currentState = MonsterState.RETURN;
        }
    }

    /// <summary>
    /// ����׷���뾶������״̬�ļ�⣬���ټ����˾���
    /// </summary>
    void ReturnCheck()
    {
        diatanceToInitial = Vector3.Distance(transform.position, initialPos);
        //����Ѿ��ӽ���ʼλ�ã������һ������״̬
        if (diatanceToInitial < 0.5f)
        {
            is_Running = false;
            RandomAction();
        }
    }
}
