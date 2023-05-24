using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 여러 상태들을 상속시킬 부모클래스 생성
public abstract class StateBase : MonoBehaviour
{
    public abstract void SetUp();
    public abstract void Enter();       // 진입
    public abstract void Update();      // 동작
    public abstract void Exit();        // 탈출
}
