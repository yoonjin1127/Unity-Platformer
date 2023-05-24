using BeeState;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class Bee : MonoBehaviour
{
    // 현재 상태
    private State curState;

    // 플레이어 위치
    public Transform player;

    // 돌아갈 위치
    public Vector3 returnPosition;

    // 순찰 인덱스
    public int patrolIndex = 0;

    public StateBase[] states;        // dictionary로 쓰는 경우도 있음


    // 플레이어와의 거리
    [SerializeField]
    public float detectRange;
    // 이동속도
    [SerializeField]
    public float moveSpeed;
    // 공격범위
    [SerializeField]
    public float attackRange;
    // 순찰지점
    [SerializeField]
    public Transform[] patrolPoints;

    private void Awake()
    {
        // 갖고 있는 상태패턴 개수만큼 배열로 만듦
        states = new StateBase[(int)State.Size];
        states[(int)State.Idle] = new IdleState(this);
        states[(int)State.Trace] = new TraceState(this);
        states[(int)State.Return] = new ReturnState(this);
        states[(int)State.Attack] = new AttackState(this);
        states[(int)State.Patrol] = new PatrolState(this);
    }

    // 시작 상태
    private void Start()
    {
        // 시작할 때는 가만히 있음
        curState = State.Idle;

        states[(int)curState].Enter();

        // 태그로 플레이어 위치 찾음
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // 몬스터가 돌아갈 위치 설정
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
    // size는 상태는 아니고, 열거형의 맨 뒤에 둬야한다(사이즈의 번호로 크기를 알아내기 위함)
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

            // 아무것도 안한 채로 2초 이상 지났다면 순찰모드로 전환
            if (idleTime > 2)
            {
                idleTime = 0;
                bee.patrolIndex = (bee.patrolIndex + 1) % bee.patrolPoints.Length;
                bee.ChangeState(State.Patrol);
            }

            // 플레이어와 가까워졌을 때
            else if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.detectRange)
            {
                // 현재 상태를 추적상태로 갱신
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
            // 플레이어 쫓아가기 (Translate로 이동시킴)
            // 도착지 - 출발지 = 출발지까지 걸리는 거리
            // normalized로 정규화해서 크기를 1로 만들어 준다
            Vector2 dir = (bee.player.position - bee.transform.position).normalized;
            bee.transform.Translate(dir * bee.moveSpeed * Time.deltaTime);

            // 플레이어가 멀어졌을 때
            if (Vector2.Distance(bee.player.position, bee.transform.position) > bee.detectRange)
            {
                bee.ChangeState(State.Return);
            }

            // 공격범위 안에 있을 때
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
            // 원래 자리로 돌아가기
            Vector2 dir = (bee.patrolPoints[bee.patrolIndex].position - bee.transform.position).normalized;
            bee.transform.Translate(dir * bee.moveSpeed * Time.deltaTime);

            // 원래 자리에 도착했으면
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

            // 공격하기
            if (lastAttackTime > 3)
            {
                Debug.Log("공격");
                lastAttackTime = 0;
            }
            // 시간 누적 방식
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
            // 순찰 진행
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