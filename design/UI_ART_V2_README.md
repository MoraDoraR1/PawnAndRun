# Pawn & Run — Unified UI Art V2

V2는 확정 키 비주얼의 형태 언어를 모든 UI에 동일하게 적용한 재제작 패키지입니다.

## 공통 디자인 규칙

모든 버튼·패널·원형 컨트롤은 아래 순서를 공유합니다.

1. 에스프레소 블랙 외곽 섀시
2. 브론즈 인셋 키라인
3. 크림 또는 기능별 상태색 면
4. 네 모서리의 작은 체스 타일 브래킷
5. 상단 중앙 방향의 절제된 하이라이트

색상, 모서리 곡률, 프레임 두께, 아이콘 선 굵기를 전 에셋에서 통일했습니다.

## Unity 적용

- ZIP 내부 `Assets/Art/UI`를 프로젝트의 동일 경로에 덮어씁니다.
- Texture Type: `Sprite (2D and UI)`
- Color Space: `sRGB`
- Alpha Is Transparency: 로고·아이콘·버튼에서 활성화
- `tex_board_checker*.png`, `tex_grass_bg*.png`: Wrap Mode `Repeat`
- `Generated/sprite_rounded_rect.png`: Border `26, 26, 26, 26`
- `Generated/sprite_gloss_rounded_rect.png`: Border `26, 26, 26, 26`

## 상태 파일

- `_normal`: 크림 면 + 에스프레소 아이콘
- `_ready`: 초록 면 + 크림 아이콘
- `_disabled`: 동일 구조를 유지한 저채도/저명도 상태
- 접미사 없는 화살표 파일은 normal 상태의 호환용 복사본입니다.

## 참고 자료

- `Reference/key_visual_860x440.png`: 공식 색상·광원·프레임 기준
- `Reference/ui_system_reference.png`: V2 전체 UI 구조 기준
- `PawnRun_UI_Art_Preview.png`: 주요 에셋 통합 프리뷰

잔디의 기본 패턴은 내장 이미지 생성으로 만든 벡터형 모티프를 기반으로 하며, 실제 타일 경계는 상하·좌우 미러 방식으로 시임리스 처리했습니다. 나머지 UI는 규격 일관성을 위해 벡터 도형으로 제작 후 PNG로 렌더링했습니다.
