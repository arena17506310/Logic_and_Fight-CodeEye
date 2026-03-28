# Logic & Resource Evolution — 프로젝트 총정리

## 프로젝트 개요

테크트리 기반 자원 채굴 코딩 배틀 게임. 플레이어가 직접 설계한 DSL 코드로 자원 채굴을 자동화하고, 수집한 자원으로 DSL 문법을 단계적으로 해금하며 상대방과 경쟁하는 1vs1 전략 배틀 게임 플랫폼.

**모티브:** The Farmer Was Replaced (자동화 파밍) + RPG 테크트리 성장 + 1vs1 PvP 전투

---

## 기술 스택

| 분야 | 기술 |
|---|---|
| 게임 엔진 | Unity 6.3 LTS (URP 3D 템플릿) |
| 클라이언트 언어 | C# |
| DSL 인터프리터 | C# (Unity 내장) |
| 멀티플레이 서버 | Java (TCP 소켓 직접 구현) |
| 데이터베이스 | 미정 |
| 비주얼 스타일 | 로우폴리 3D + URP Post-Processing (Bloom, Fog, Vignette) |
| 3D 모델링 | Blender (bpy 스크립트 활용 가능 혹은 개발자 본인이 디자인) |

---

## 게임 구조

### 핵심 루프

```
① 파밍 페이즈
   └─ 각자 독립 필드에서 DSL 코드로 채굴 자동화 → 자원 수집
   └─ 
   └─ 상점에서 방해 아이템 구매 → 상대 필드에 사용 가능

② 테크 페이즈
   └─ 수집 자원으로 DSL 문법/함수 해금
   └─ 해금 = 인터프리터 AST 허용 노드 동적 확장

③ 전투 페이즈
   └─ 플레이어 직접 조작 실시간 1vs1 전투
   └─ 파밍으로 모은 자원으로 구매한 스킬 사용

④ 반복
```

### 파밍 페이즈 상세

- 두 플레이어는 **완전히 분리된 독립 필드**를 가짐
- 자원은 필드에 한정적으로 스폰 (광물, 에너지, 데이터)
- 플레이어가 DSL 코드를 작성 → 실행 → 자동으로 자원 수집(The Farmer was Replaced와 비슷한 방식) 
- 상점에서 방해 아이템 구매 가능 → 상대 필드에 영향
- 상점에서 필드 및 자원 채굴 관련 업그레이드 가능 → 필드 크기 증가, 자원 채굴량, 속도 증가 등(추가 기능 고안 필요)

**파밍 페이즈 긴장감 유지 장치:**
1. 상대방이 방해 아이템으로 내 자원 채굴/필드를 방해 가능

### 상점 방해 아이템

| 분류 | 아이템 | 효과 |
|---|---|---|
| 로봇 방해 | EMP 탄 | 상대 채굴 로봇 전체 일정 시간 마비 |
| 로봇 방해 | 해킹 바이러스 | 상대 드론 하나 코드 일시 오작동 |
| 필드 방해 | 자원 오염 | 상대 필드 특정 구역 수집 불가 |
| 필드 방해 | 장애물 설치 | 상대 필드 이동 경로 차단 |
| 테크 방해 | 테크 잠금 | 상대 특정 테크 일정 시간 비활성화 |
| 방어 | 방어막 | 내 필드 일정 시간 방해 면역 |
(추가 기능 고안 가능)

---

## DSL 인터프리터 구조

### 핵심 설계 원칙

DSL은 **명령어 호출만 담당**, 실제 게임 로직은 **C# 내장 함수로 구현**.
인터프리터 완성도와 무관하게 C# 내장 함수를 풍부하게 만들수록 게임 깊이가 증가.

```
플레이어 DSL 코드
    ↓ 인터프리터 해석 (렉서 → 파서 → AST → 실행기)
C# 내장 함수 호출 (RobotAPI)
    ↓
Unity 게임 오브젝트 직접 제어
```

### 인터프리터 구현 단계

```
렉서(Lexer)     : DSL 문자열 → 토큰 리스트
파서(Parser)    : 토큰 리스트 → AST(추상 구문 트리)
실행기(Evaluator): AST 순회 → RobotAPI 함수 호출
```

### 테크트리 — DSL 문법 단계별 해금

| 단계 | 해금 내용 | 필요 자원 |
|---|---|---|
| Lv.0 (기본) | move_to(), collect(), return_to_base() | 무료 |
| Lv.1 | IF / ELSE 조건문 | 광물 ×20 |
| Lv.2 | FOR / WHILE 반복문 | 광물 ×40 |
| Lv.3 | 함수 정의 (def) | 광물 ×60 + 에너지 ×20 |
| Lv.4 | 방어·전투 함수 해금 | 광물 ×80 + 데이터 ×30 |
| Lv.5 | 병렬 드론 제어 (async) | 전 자원 ×50 |

**테크 해금 = AST 허용 노드 동적 확장**
미해금 문법 사용 시 인터프리터가 오류 반환.

### DSL 예시 문법(추후 고안해 변경 예정)

```
// Lv.0 — 기본
move_to(nearest_resource)
collect()
return_to_base()

// Lv.1 — IF 해금 후
IF resource.type == "mineral" THEN collect()
IF base.storage > 80% THEN return_to_base()

// Lv.2 — FOR 해금 후
FOR resource IN nearby_resources:
    IF resource.value > 50 THEN collect(resource)

// Lv.4 — 방어 함수 해금 후
IF detect(enemy_robot) THEN flee_to_base()
IF base.under_attack THEN call_guard()
```

### RobotAPI — C# 내장 함수 구조

여러가지 프로그래밍의 기본적인 문법들을 통한 로봇의 움직임 제어 코드 구현

## 멀티플레이 서버 구조

### 통신 구조

```
Unity 클라이언트 (C#, TcpClient)
        ↕ TCP 소켓, JSON 패킷, \n 구분자
Java 서버 (ServerSocket)
        ↕ TCP 소켓, JSON 패킷, \n 구분자
Unity 클라이언트 (C#, TcpClient)
```


## 협업 파트 분류

| 파트 | 역할 | 핵심 기술 |
|---|---|---|
| A | DSL 인터프리터 | C# 렉서·파서·AST·실행기·샌드박스 |
| B | 멀티플레이 서버 | Java TCP 소켓, 패킷 설계, 멀티스레드 |
| C | 게임 클라이언트 (파밍·시스템) | Unity C#, 필드 시스템, DSL 에디터 UI, 상점·테크트리 UI |
| D | 게임 클라이언트 (전투) | Unity C#, 플레이어 조작, 스킬 시스템, 전투 판정 |
| E | 아키텍처·통합·AI 분석 | 시스템 설계, 데이터베이스(종류 미정), Python 데이터 분석, 대시보드 |

### 파트 간 의존성

```
파트 A (인터프리터) (팀장)
    └─→ 파트 C에 인터프리터 모듈 제공
    └─→ RobotAPI 인터페이스 파트 C와 공동 설계
    └─→ 파트 간 통합 테스트 총괄

파트 B (서버)
    └─→ 파트 C·D에 NetworkManager 연동
    └─→ 패킷 포맷 전체 파트와 공동 확정

파트 E (통합)
    └─→ 전체 모듈 인터페이스 설계
    └─→ 데이터베이스 스키마 관리
    
```

---

## 비주얼 & 렌더링

### 스타일 방향

- **로우폴리 3D** — 단순한 면 구성, 복잡한 텍스처 없음
- **2.5D 느낌(The Farmer was Replaced의 레벨 및 오브젝트 디자인 채택)** — 3D 오브젝트 + 고정 카메라 앵글 
- **Post-Processing** — Bloom + Fog + Vignette (마인크래프트 쉐이더 느낌)

### URP Post-Processing 설정 목표

```
Bloom     : 밝은 UI·오브젝트 주변 빛 번짐
Vignette  : 화면 가장자리 어둡게 처리
Fog       : 필드 원거리 뿌옇게 처리
Glow      : DSL 에디터 UI, 버튼 테두리 발광
```

### 에셋 전략

| 분류 | 방법 |
|---|---|
| 핵심 오브젝트 (드론, 기지) | Blender 직접 제작 (bpy 스크립트 활용 가능) |
| 지형·타일·반복 오브젝트 | Blender bpy 스크립트로 자동 생성 |
| 부가 요소 | Unity Asset Store 무료 로우폴리 팩 |

---

## UI 설계 방향

### DSL 코드 에디터
- 다크 테마 (어두운 배경 + 밝은 텍스트)
- 해금된 키워드 색상 강조 (Syntax Highlighting)
- 미해금 키워드 회색 처리
- Unity TextMeshPro + InputField 조합

### 테크트리 UI
- 노드 → 연결선 → 노드 구조
- 상태: 해금됨(밝은 색) / 해금 가능(테두리만) / 잠김(어두운 색)

### 상점 UI
- 아이템 카드 형식
- 현재 자원량 상단 항시 표시
- 구매 불가 항목 비활성화 처리

---

## 디렉토리 구조(완전 프로토 타입으로 프로젝트 진행상황에 따라 언제든 변경 가능, 클로드 코드는 이 파일을 읽고 이상적인 파일 구조를 재설정해주길 바람)

```
Assets/
├── Scripts/
│   ├── DSL/
│   │   ├── Lexer.cs
│   │   ├── Parser.cs
│   │   ├── ASTNode.cs
│   │   ├── Interpreter.cs
│   │   ├── RobotAPI.cs
│   │   └── SandboxedInterpreter.cs
│   ├── Network/
│   │   └── NetworkManager.cs
│   ├── Farming/
│   │   ├── FieldManager.cs
│   │   ├── RobotController.cs
│   │   ├── ResourceManager.cs
│   │   └── ShopManager.cs
│   ├── TechTree/
│   │   └── TechTreeManager.cs
│   ├── Battle/
│   │   ├── PlayerController.cs
│   │   ├── SkillSystem.cs
│   │   └── BattleManager.cs
│   └── UI/
│       ├── DSLEditorUI.cs
│       ├── ShopUI.cs
│       └── TechTreeUI.cs
├── Models/          # Blender에서 임포트한 로우폴리 에셋
├── Materials/       # URP 머티리얼
└── Scenes/
    ├── FarmingScene.unity
    └── BattleScene.unity

JavaServer/
├── src/
│   ├── GameServer.java
│   ├── ClientHandler.java
│   └── GameStateManager.java
└── build/
```

---

## 개발 우선순위

### MVP (핵심 완성 목표)
1. DSL 인터프리터 기초
2. 드론 자동 이동·수집 동작
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
- **운영체제:** 드론별 독립 샌드박스, 타임아웃·메모리 제한, 무한루프 방지
- **네트워크:** Java TCP 소켓 직접 구현, JSON 패킷 설계, 멀티스레드 서버
- **알고리즘:** 드론 경로 탐색, 자원 우선순위 정렬, 충돌 감지
- **소프트웨어 공학:** 클라이언트-서버 2-tier 아키텍처, 모듈화, RobotAPI 인터페이스 설계
