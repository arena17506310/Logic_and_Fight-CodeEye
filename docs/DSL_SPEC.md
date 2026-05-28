# DSL_SPEC.md — DSL 언어 명세

## 문법 확정 사항

- **소문자 키워드** (`if`, `for`, `while`, `func` 등)
- **중괄호 `{}` 로 블록 구분** (C 스타일)
- **세미콜론 없이 줄바꿈으로 구문 구분** (Python 스타일)
- **타입 선언 없이 대입으로 변수 사용** (`x = 10` 방식, `var` 키워드 없음)
- 변수는 처음 대입 시 자동 생성 (Dictionary 기반 환경 관리)

---

## DSL 예시 문법

```
// Lv.0 — 기본 명령어
move_to(nearest_resource)
collect()
return_to_base()

// Lv.1 — if 해금 후
if (resource.type == "mineral") {
    collect()
}
if (base.storage > 80) {
    return_to_base()
}

// Lv.2 — for 해금 후
for resource in nearby_resources {
    if (resource.value > 50) {
        collect(resource)
    }
}

// Lv.3 — func 해금 후
func mine_all() {
    for resource in nearby_resources {
        collect(resource)
    }
}
mine_all()

// Lv.4 — 방어 함수 해금 후
if (detect(enemy_robot)) {
    flee_to_base()
}
if (base.under_attack) {
    call_guard()
}
```

---

## 인터프리터 구현 단계

```
렉서(Lexer)      : DSL 문자열 → 토큰 리스트
파서(Parser)     : 토큰 리스트 → AST(추상 구문 트리)
실행기(Evaluator): AST 순회 → RobotAPI 함수 호출
```

### 핵심 설계 원칙

DSL은 **명령어 호출만 담당**, 실제 게임 로직은 **C# 내장 함수(RobotAPI)로 구현**.
인터프리터 완성도와 무관하게 C# 내장 함수를 풍부하게 만들수록 게임 깊이가 증가.

```
플레이어 DSL 코드
    ↓ 인터프리터 해석 (렉서 → 파서 → AST → 실행기)
C# 내장 함수 호출 (RobotAPI)
    ↓
Unity 게임 오브젝트 직접 제어
```

---

## RobotAPI — C# 내장 함수 구조 (미정)

```csharp
public class RobotAPI
{
    public void move_to(string target) { ... }
    public void collect() { ... }
    public void return_to_base() { ... }
    public bool detect(string target) { ... }
    public void flee_to_base() { ... }
    public void call_guard() { ... }
    ...

    // 테크 해금 시 함수 활성화
    public void UnlockFunction(string funcName) { ... }
    ...
}
```

**테크 해금 = AST 허용 노드 동적 확장**
미해금 문법 사용 시 인터프리터가 오류 반환.
ㅌ
---

## 샌드박스 처리 (미정)

```csharp
public class SandboxedInterpreter
{
    private const int MAX_OPERATIONS = 10000;
    private int operationCount = 0;

    public void Execute(ASTNode node)
    {
        operationCount++;
        if (operationCount > MAX_OPERATIONS)
            throw new DSLException("실행 한도 초과 (무한루프 방지)");
        // ... 실행 로직
    }
}
```

- 로봇별 독립 실행 환경
- 타임아웃 처리
- 메모리 제한
- 무한루프 방지
