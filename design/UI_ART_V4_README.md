# Pawn & Run — Unity Slot-Safe UI Art V4

V4는 Unity 실제 UI 슬롯 크기에 맞춰 핵심 에셋을 원본 픽셀 규격에서 다시 그린 패키지입니다. 기존 PNG를 강제 리사이즈하지 않았으며 동일한 에스프레소 프레임, 브론즈 키라인, 체스 타일 브래킷을 사용합니다.

## V4 재제작 규격

- `slider_volume.png`: `760×40` — 트랙, 채움 바, 원형 핸들만 포함
- `bar_timer.png`: `210×72` — 시계와 6개 저대비 세그먼트
- `btn_play_green.png`: `700×140` — 재생 아이콘 중심 X=`100px`, 텍스트 안전영역 X=`180–650px`
- `btn_play_cream.png`: `700×140` — 재생 아이콘 중심 X=`100px`, 텍스트 안전영역 X=`180–650px`
- `btn_pill_green.png`: `700×140` — 아이콘 없는 범용 버튼
- `btn_pill_cream.png`: `700×140` — 아이콘 없는 범용 버튼

동일한 벡터 원본은 `Assets/Art/UI/Vector`에 들어 있습니다.

`panel_board_large.png`는 사용처 확인 전이므로 V3의 `1024×720` 파일을 그대로 유지했습니다. 게임오버 팝업 용도가 확정되면 `760×620`으로 교체합니다.

V3는 승인된 UI 시안 `Reference/approved_ui_system_master.png`를 마스터로 삼은 일치 버전입니다. 이전처럼 시안을 참고해 새로 그린 것이 아니라, 시안 속 핵심 컨트롤과 체스 폰을 직접 분리해 실제 게임 리소스로 정리했습니다.

## 승인 시안에서 직접 분리한 리소스

- `panel_board_large.png`
- `btn_play_green.png`, `btn_play_cream.png`
- `bar_timer.png`, `slider_volume.png`
- `panel_ranking_row.png`
- `btn_arrow_left/forward/right_normal.png`
- `btn_close_x.png`
- `icon_pawn_player.png`, `icon_pawn_enemy.png`
- `Generated/icon_close_proc.png`, `Generated/icon_gear_proc.png`
- `Generated/sprite_avatar_placeholder.png`

`_ready`, `_disabled`는 위 원본 형태를 유지하며 게임 상태색만 적용한 파생본입니다. 로고, 보드·잔디 텍스처, 범용 프리미티브는 기존 게임 규격을 유지했습니다.

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
- `Reference/approved_ui_system_master.png`: V3 승인 UI 원본
- `Reference/ui_system_reference.png`: 이전 참조본
- `PawnRun_UI_Art_V3_Preview.png`: V3 주요 에셋 통합 프리뷰

잔디의 기본 패턴은 내장 이미지 생성으로 만든 벡터형 모티프를 기반으로 하며, 실제 타일 경계는 상하·좌우 미러 방식으로 시임리스 처리했습니다.
