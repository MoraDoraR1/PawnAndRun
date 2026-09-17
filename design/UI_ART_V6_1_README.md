# Pawn & Run UI Art V6.1 — Warm Neutral Background

V6.1은 Warm Modern UI는 유지하면서 배경만 다시 조율한 패키지입니다. 체크보드의 녹색 채도와 명도 차이를 낮추고 웜 오트밀·그레이시 세이지 조합으로 변경해 UI 카드보다 배경이 먼저 보이지 않도록 했습니다.

## 디자인 특징

- 외곽선은 2–4px의 얇은 웜그레이 선으로 축소
- 넓은 아이보리 면과 충분한 내부 여백 사용
- 주 행동은 짙은 테라코타, 보조 행동은 아이보리 카드로 구분
- 메뉴 배경: 웜 오트밀 `#EDE4D4` + 그레이시 세이지 `#B6C0B4`
- 게임 배경: 저채도 세이지 `#91A191`
- 작은 화면에서도 텍스트가 장식과 겹치지 않도록 전용 안전영역 확보
- 정보 텍스트 `#302A25` / 아이보리 배경 대비 약 `13.5:1`
- 주 버튼 아이보리 텍스트 / 테라코타 대비 약 `4.0:1`

## 주요 파일

- `PawnRun_UI_V6_AllScreens_Preview.png` — 네 화면 통합 검수본
- `PawnRun_UI_V6_AssetSheet.png` — 전체 디자인 시스템 시트
- `UI_SPEC_AND_CHECKLIST.md` — 배치 규격과 누락 검수표
- `CLAUDE_UNITY_APPLY_PROMPT.md` — Unity 적용용 명령 프롬프트

## Unity 기준

- Reference Resolution: `1080×1920`
- Screen Match Mode: `Match Width Or Height`
- Match: `0.5`
- 버튼명, 점수, 타이머, 랭킹 데이터는 TextMeshPro로 배치
