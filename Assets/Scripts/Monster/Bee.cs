using BeeState;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class Bee : MonoBehaviour
{
    // ���� ����
    private State curState;

    // �÷��̾� ��ġ
    public Transform player;

    // ���ư� ��ġ
    public Vector3 returnPosition;

    // ���� �ε���
    public int patrolIndex = 0;

    public StateBase[] states;        // dictionary�� ���� ��쵵 ����


    // �÷��̾���� �Ÿ�
    [SerializeField]
    public float detectRange;
    // �̵��ӵ�
    [SerializeField]
    public float moveSpeed;
    // ���ݹ���
    [SerializeField]
    public float attackRange;
    // ��������
    [SerializeField]
    public Transform[] patrolPoints;

    private void Awake()
    {
        // ���� �ִ� �������� ������ŭ �迭�� ����
        states = new StateBase[(int)State.Size];
        states[(int)State.Idle] = new IdleState(this);
        states[(int)State.Trace] = new TraceState(this);
        states[(int)State.Return] = new ReturnState(this);
        states[(int)State.Attack] = new AttackState(this);
        states[(int)State.Patrol] = new PatrolState(this);
    }

    // ���� ����
    private void Start()
    {
        // ������ ���� ������ ����
        curState = State.Idle;

        states[(int)curState].Enter();

        // �±׷� �÷��̾� ��ġ ã��
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // ���Ͱ� ���ư� ��ġ ����
        returnPosition = transform.position;
    }

    private void Update()
    {
        states[(int)curState].Update();
    }

    public void ChangeState(State state)
    {
        states[(int)curState].Exit();
        curState = state;
        states[(int)curState].Enter();
    }


}

namespace BeeState
{
    // size�� ���´� �ƴϰ�, �������� �� �ڿ� �־��Ѵ�(�������� ��ȣ�� ũ�⸦ �˾Ƴ��� ����)
    public enum State { Idle, Trace, Return, Attack, Patrol, Size }

    public class IdleState : StateBase
    {
        private Bee bee;
        private float idleTime = 0;

        public IdleState(Bee bee)
        {
        this.bee = bee;
        }
        public override void SetUp()
        {

        }
        public override void Enter()
        {

        }
        public override void Update()
        {
            idleTime += Time.deltaTime;

            // �ƹ��͵� ���� ä�� 2�� �̻� �����ٸ� �������� ��ȯ
            if (idleTime > 2)
            {
                idleTime = 0;
                bee.patrolIndex = (bee.patrolIndex + 1) % bee.patrolPoints.Length;
                bee.ChangeState(State.Patrol);
            }

            // �÷��̾�� ��������� ��
            else if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.detectRange)
            {
                // ���� ���¸� �������·� ����
                bee.ChangeState(State.Trace);
            }
        }
        public override void Exit()
        {

        }

    }


    public class TraceState : StateBase
    {
        private Bee bee;
        public TraceState(Bee bee)
        {
            this.bee = bee;
        }
        public override void SetUp()
        {

        }
        public override void Enter()
        {

        }
        public override void Update()
        {
            // �÷��̾� �Ѿư��� (Translate�� �̵���Ŵ)
            // ������ - ����� = ��������� �ɸ��� �Ÿ�
            // normalized�� ����ȭ�ؼ� ũ�⸦ 1�� ����� �ش�
            Vector2 dir = (bee.player.position - bee.transform.position).normalized;
            bee.transform.Translate(dir * bee.moveSpeed * Time.deltaTime);

            // �÷��̾ �־����� ��
            if (Vector2.Distance(bee.player.position, bee.transform.position) > bee.detectRange)
            {
                bee.ChangeState(State.Return);
            }

            // ���ݹ��� �ȿ� ���� ��
            else if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.attackRange)
            {
                bee.ChangeState(State.Attack);
            }
        }
        public override void Exit()
        {

        }
    }

    public class ReturnState : StateBase
    {
        private Bee bee;

        public ReturnState (Bee bee)
        {
            this.bee = bee;
        }

        public override void SetUp()
        {

        }

        public override void Enter()
        {

        }

        public override void Update()
        {
            // ���� �ڸ��� ���ư���
            Vector2 dir = (bee.patrolPoints[bee.patrolIndex].position - bee.transform.position).normalized;
            bee.transform.Translate(dir * bee.moveSpeed * Time.deltaTime);

            // ���� �ڸ��� ����������
            if (Vector2.Distance(bee.transform.position, bee.returnPosition) < 0.02f)
            {
                bee.ChangeState(State.Idle);
            }

            else if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.detectRange)
            {
                bee.ChangeState(State.Trace);
            }
        }

        public override void Exit()
        {

        }
    }

    public class AttackState : StateBase
    {
        private Bee bee;

        public AttackState(Bee bee)
        {
            this.bee = bee;
        }

        public override void SetUp()
        {
         
        }

        public override void Enter()
        {

        }

        public override void Exit()
        {

        }

        public override void Update()
        {
            float lastAttackTime = 0;
            float delayTime = 0;

            // �����ϱ�
            if (lastAttackTime > 3)
            {
                Debug.Log("����");
                lastAttackTime = 0;
            }
            // �ð� ���� ���
            lastAttackTime += Time.deltaTime;

            if (Vector2.Distance(bee.player.position, bee.transform.position) > bee.attackRange)
            {
                bee.ChangeState(State.Trace);

            }

        }
    }

    public class PatrolState : StateBase
    {
        private Bee bee;

        public PatrolState(Bee bee)
        {
            this.bee = bee;
        }

        public override void SetUp()
        {
         
        }

        public override void Enter()
        {
            bee.patrolIndex = (bee.patrolIndex + 1) % bee.patrolPoints.Length;
        }

        public override void Exit()
        {

        }
    
        public override void Update()
        {
            // ���� ����
            // patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            Vector2 dir = (bee.patrolPoints[bee.patrolIndex].position - bee.transform.position).normalized;
            bee.transform.Translate(dir * bee.moveSpeed * Time.deltaTime);

            if (Vector2.Distance(bee.transform.position, bee.patrolPoints[bee.patrolIndex].position) < 0.02f)
            {
                bee.ChangeState(State.Idle);
            }

            else if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.detectRange)
            {
                bee.ChangeState(State.Trace);
            }
        }
    }
}