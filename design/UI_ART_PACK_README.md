# Pawn & Run — Unity UI art pack

이 패키지는 확정된 `PAWN & RUN` 키 비주얼을 기준으로 제작되었습니다. `Assets/Art/UI` 폴더를 Unity 프로젝트의 동일 경로에 복사하면 됩니다.

## Unity import 권장값

- Texture Type: `Sprite (2D and UI)`
- Color Space: `sRGB`
- Alpha Is Transparency: 아이콘·로고·버튼에서 활성화
- Filter Mode: `Bilinear`
- Compression: UI는 `None` 또는 `High Quality`
- `tex_board_checker*.png`, `tex_grass_bg*.png`: Wrap Mode `Repeat`
- `Generated/sprite_rounded_rect.png`: Border `24, 24, 24, 24`
- `Generated/sprite_gloss_rounded_rect.png`: Border `28, 28, 28, 28`

## 버튼 상태

- `_normal`: 기본 사용 가능 상태
- `_ready`: 현재 추천/즉시 실행 가능한 액션
- `_disabled`: 실행 불가 상태
- 접미사 없는 `btn_arrow_forward.png`, `btn_arrow_left.png`, `btn_arrow_right.png`는 normal 상태의 호환용 복사본입니다.

## 테마 파일

- 접미사 없는 배경 텍스처는 라이트 테마 기본값입니다.
- `_light`, `_dark` 파일을 테마별로 사용할 수 있습니다.

## 제작 방식

- 잔디: 내장 이미지 생성으로 만든 원본을 상하·좌우 미러 타일링하여 실제 시임리스 처리
- 로고·아이콘·버튼·폰: 규격 일관성을 위해 벡터 도형으로 제작 후 PNG 렌더링
- `Reference/key_visual_860x440.png`: 전체 리소스의 색상·광원·프레임 기준점
