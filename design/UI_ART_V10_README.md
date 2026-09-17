# Pawn & Run UI V10 — Premium Polish / PNG Only

V9의 저가형 픽토그램 인상과 평면적인 화면 계층을 폐기하고 전체 UI를 재폴리싱한 버전입니다.

## 핵심 개선

- 폰의 머리·칼라·몸체·전진 화살표를 하나의 실루엣으로 통합
- 장식 없는 투명 심볼, 워드마크, 1024 앱 아이콘을 별도 제공
- 밝은 정보 영역과 깊은 네이비 플레이 영역을 분리
- 버튼 상단 미세 하이라이트, 얕은 외곽 레이어, 절제된 그림자 적용
- HUD를 독립 카드로 변경하고 세 열의 중심선 통일
- 랭킹의 순위·닉네임·점수 열과 베이스라인 통일
- 게임 보드의 프레임과 입력 버튼 깊이 통일
- SVG 없이 PNG만 제공

## 구성

- `Assets/Art/UI`: Unity용 PNG 리소스
- `Previews`: 화면별 1080×1920 조립본
- `Reference`: 승인 원본과 폴리싱 방향 이미지
- `UI_SPEC_AND_CHECKLIST.md`: 규격 및 검수표
- `CLAUDE_UNITY_APPLY_PROMPT.md`: Claude Code 적용 지시문

Canvas 기준은 1080×1920, Match 0.5입니다. 로고 이외의 텍스트는 Unity TextMeshPro로 표시합니다.

