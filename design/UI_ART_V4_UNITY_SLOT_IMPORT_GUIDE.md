# Unity Slot Import Guide

## Native size

| Asset | Pixel size | Recommended use |
|---|---:|---|
| `slider_volume.png` | 760×40 | Slider visual at 1:1 size |
| `bar_timer.png` | 210×72 | Timer background at 1:1 size |
| `btn_play_green.png` | 700×140 | Primary button with play icon |
| `btn_play_cream.png` | 700×140 | Secondary button with play icon |
| `btn_pill_green.png` | 700×140 | Empty primary pill button |
| `btn_pill_cream.png` | 700×140 | Empty secondary pill button |

## Text placement

- Play buttons: anchor the label inside X `180–650`, Y center `70`.
- Recommended text color: cream `#FFF5D6` on green, espresso `#24170F` on cream.
- Timer text: center around X `138`, Y `36`; use espresso `#24170F` for contrast.

## Import settings

- Texture Type: `Sprite (2D and UI)`
- Mesh Type: `Full Rect`
- Filter Mode: `Bilinear`
- Compression: `None` for the thin slider; `High Quality` or `None` for the remaining UI.
- Pixels Per Unit: keep consistent with the project UI canvas; display these assets at their native aspect ratios.

The V4 assets are already delivered at the target slot dimensions, so runtime non-uniform scaling is unnecessary.
