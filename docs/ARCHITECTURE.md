# ARCHITECTURE.md — 시스템 설계

## 통신 구조 (미정)

```
Unity 클라이언트 (C#, TcpClient)
        ↕ TCP 소켓, JSON 패킷, \n 구분자
Java 서버 (ServerSocket)
        ↕ TCP 소켓, JSON 패킷, \n 구분자
Unity 클라이언트 (C#, TcpClient)
```

---

## 패킷 포맷 (JSON) (예시)

```json
{ "type": "BUY_ITEM", "data": "EMP" }
{ "type": "TECH_UNLOCK", "data": "IF_STATEMENT" }
{ "type": "PLAYER_POS", "data": {"x": 1.5, "y": 3.2} }
{ "type": "SKILL_USE", "data": "FIREBALL" }
{ "type": "BATTLE_START", "data": "" }
{ "type": "GAME_OVER", "data": "WIN" }
```

---

## Unity 클라이언트 소켓 핵심 구조 (예시)

```csharp
public class NetworkManager : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private Queue<string> receiveQueue = new Queue<string>();

    // 수신 전용 스레드에서 큐에 저장
    void ReceiveLoop() { ... }

    // Update()에서 큐 꺼내서 게임에 반영 (Unity 메인 스레드 규칙 준수)
    void Update()
    {
        while (receiveQueue.Count > 0)
            HandlePacket(receiveQueue.Dequeue());
    }
}
```

---

## 파트 간 의존성

```
파트 A (인터프리터) — 팀장
    └─→ 파트 C에 인터프리터 모듈 제공
    └─→ RobotAPI 인터페이스 파트 C와 공동 설계

파트 B (서버)
    └─→ 파트 C·D에 NetworkManager 연동
    └─→ 패킷 포맷 전체 파트와 공동 확정

파트 E (통합)
    └─→ 전체 모듈 인터페이스 설계
    └─→ 데이터베이스 스키마 관리
    └─→ 파트 간 통합 테스트 총괄
    └─→ Python AI 분석 대시보드 (게임 전투·파밍 데이터 분석)
```

---

## 개발 우선순위

### MVP (핵심 완성 목표)
1. DSL 인터프리터 기초 (Lv.0~2 문법)
2. 로봇 자동 이동·수집 동작
3. Java 소켓 서버 1vs1 연결
4. 파밍 페이즈 기본 동작
5. 전투 페이즈 기본 조작

### 2순위 (MVP 완성 후)
1. 테크트리 UI 및 해금 시스템
2. 상점 방해 아이템 시스템
3. DSL Lv.3~5 고급 문법
4. Post-Processing 비주얼

### 3순위 (시간 여유 시)
1. 파트 E AI 분석 대시보드
2. 로우폴리 커스텀 에셋 교체
3. 사운드 시스템

---

## 주요 CS 구현 포인트

- **컴파일러 이론:** 렉서·파서·AST·인터프리터 직접 구현, 테크 단계별 AST 허용 노드 동적 제어
- **운영체제:** 로봇별 독립 샌드박스, 타임아웃·메모리 제한, 무한루프 방지
- **네트워크:** Java TCP 소켓 직접 구현, JSON 패킷 설계, 멀티스레드 서버
- **알고리즘:** 로봇 경로 탐색, 자원 우선순위 정렬, 충돌 감지
- **소프트웨어 공학:** 클라이언트-서버 2-tier 아키텍처, 모듈화, RobotAPI 인터페이스 설계
- **데이터베이스:** 스키마 설계, 전적·자원 로그 저장 (DB 종류 미정)
