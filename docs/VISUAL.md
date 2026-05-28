# VISUAL.md — 비주얼 & UI 설계

## 스타일 방향

- **로우폴리 3D** — 단순한 면 구성, 복잡한 텍스처 없음
- **2.5D 느낌** — The Farmer Was Replaced의 레벨·오브젝트 디자인 참고, 3D 오브젝트 + 고정 카메라 앵글
- **Post-Processing** — Bloom + Fog + Vignette (마인크래프트 쉐이더 느낌)

---

## URP Post-Processing 설정 목표

```
Bloom     : 밝은 UI·오브젝트 주변 빛 번짐
Vignette  : 화면 가장자리 어둡게 처리
Fog       : 필드 원거리 뿌옇게 처리
Glow      : DSL 에디터 UI, 버튼 테두리 발광
```

---

## 에셋 전략

| 분류 | 방법 |
|---|---|
| 핵심 오브젝트 (로봇, 기지) | Blender 직접 제작 (팀장, bpy 스크립트 활용 가능) |
| 지형·타일·반복 오브젝트 | Blender bpy 스크립트로 자동 생성 |
| 부가 요소 | Unity Asset Store 무료 로우폴리 팩 |

**Blender 내보내기:** .fbx 포맷으로 내보내서 Assets/Models/Exported/ 에 임포트

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
