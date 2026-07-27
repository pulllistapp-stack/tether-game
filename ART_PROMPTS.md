# Tether — 캐릭터 스프라이트 프롬프트 모음

> 게임은 **정면 뷰**야. 캐릭터가 카메라를 바라봐야 함 (탑다운 아님).
> 회전 방향 필요 없음 — south(정면) 1장만 있으면 돼.
> 기술 사양은 [ART_SPEC.md](ART_SPEC.md) 참고 (PPU 32, Point filter, 투명 배경).

---

## 0. 공통 스타일 앵커

모든 프롬프트 앞이나 뒤에 붙이면 톤이 통일돼. 셋 중 하나 골라 쓰기:

**A. 동방 원작 느낌 (추천)**
```
Touhou Project fangame pixel art, chibi proportions, front-facing view,
clean pixel edges, limited palette, soft cel shading, black outline,
transparent background, centered, full body
```

**B. 레트로 게임 느낌**
```
16-bit SNES-era pixel art sprite, chibi anime character, front view,
crisp pixels, dithered shading, dark outline, transparent background,
game sprite sheet style
```

**C. 모던 인디 픽셀아트**
```
modern indie pixel art, anime chibi character, front-facing idle pose,
selective outline, vibrant limited palette, subtle rim light,
transparent background, clean silhouette
```

---

## 1. 사쿠야 (플레이어) — `Sakuya.png` / 48×48

가장 중요한 스프라이트. 항상 화면에 있음. **실루엣이 명확해야 함.**

### 1-A. 기본
```
Izayoi Sakuya from Touhou Project. Silver-white hair in twin braids,
white maid headband with frills. Deep blue maid dress with white apron
and puffy short sleeves. Red eyes. Holding a silver throwing knife in
each hand, arms slightly out. Calm confident expression.
Front-facing idle pose, chibi proportions, pixel art, transparent background
```

### 1-B. 전투 자세 강조
```
Izayoi Sakuya, the Perfect and Elegant Maid. Silver braided hair,
blue and white maid uniform, ribbon at the collar. Both hands raised
holding fanned silver knives ready to throw. Confident smirk.
Slight forward lean, dynamic idle. Chibi pixel art, front view,
black outline, transparent background
```

### 1-C. 시간정지 테마 강조
```
Izayoi Sakuya, time-manipulating maid. Silver twin-braided hair,
navy blue maid dress with white lace apron. A small pocket watch
chain hanging at her waist, faintly glowing. Silver knives floating
in a small arc around her. Serene expression, eyes half-closed.
Chibi pixel art, front-facing, transparent background
```

### 1-D. 미니멀 (32px 이하 갈 때)
```
tiny chibi maid sprite, silver hair, blue dress white apron,
two dots for eyes, simple readable silhouette, 3-4 colors only,
pixel art, front view, transparent background
```

---

## 2. 보스 3종 / 64×64

**핵심: 멀리서 실루엣만 봐도 셋이 구분돼야 해.**
Cirno = 뾰족한 날개 / Patchouli = 둥근 로브 + 책 / Yukari = 긴 파라솔

### 2-1. 치르노 `Boss_Cirno.png`

**A. 기본**
```
Cirno the ice fairy from Touhou. Short pale blue bob hair with a large
green ribbon. Blue dress with white apron, red ribbon tie. Six large
angular ice crystal wings, sharp hexagonal shards, translucent pale cyan.
Arms crossed, smug confident grin. Chibi pixel art, front view,
transparent background
```

**B. 공격 자세**
```
Cirno, arrogant ice fairy. Sky blue hair, blue and white dress,
six jagged ice crystal wings spread wide. One hand raised with a
glowing ice shard forming above her palm. Determined shouting expression.
Frost particles around her feet. Chibi pixel art, front-facing,
transparent background
```

**C. 실루엣 강조**
```
ice fairy boss sprite, dominant six-pointed crystal wing silhouette,
small figure in blue dress at center, wings much larger than body,
pale cyan and white palette with dark blue outline, pixel art,
front view, transparent background
```

### 2-2. 파츄리 널리지 `Boss_Patchouli.png`

**A. 기본**
```
Patchouli Knowledge from Touhou. Long straight purple hair, purple
nightcap with a crescent moon charm. Loose pink and purple pajama-like
robe patterned with crescent moons and stars. Holding a large open
grimoire that glows faintly. Tired half-lidded eyes, sickly pale.
Chibi pixel art, front view, transparent background
```

**B. 마법 시전**
```
Patchouli Knowledge, the Unmoving Great Library. Purple hair, moon-and-star
robe, floating cross-legged. A thick spellbook open in front of her,
pages glowing. Five small elemental orbs orbiting her — red, blue, green,
white, brown. Sleepy but focused expression. Chibi pixel art, front-facing,
transparent background
```

**C. 실루엣 강조**
```
purple witch boss sprite, wide rounded robe silhouette, tall pointed
nightcap, large open book held at chest height. Purple and pink palette,
crescent moon motifs. Bulky readable shape. Pixel art, front view,
transparent background
```

### 2-3. 야쿠모 유카리 `Boss_Yukari.png`

**A. 기본**
```
Yakumo Yukari from Touhou. Long wavy blonde hair with a purple ribbon
headband. Purple dress with pink frilled trim and a white mob cap.
Holding a closed purple parasol resting on her shoulder. Half-lidded
knowing smile, violet eyes. Chibi pixel art, front view, transparent background
```

**B. 경계 능력 강조**
```
Yakumo Yukari, youkai of boundaries. Blonde wavy hair, purple gown,
white frilled cap. Standing in front of a torn violet rift lined with
watching eyes and ribbons. Parasol open behind her. Mysterious smile.
Chibi pixel art, front-facing, transparent background
```

**C. 실루엣 강조**
```
elegant youkai boss sprite, long parasol as the dominant silhouette
line, flowing blonde hair, wide purple dress. Purple, magenta and
cream palette. Tall graceful shape distinct from round or spiky
enemies. Pixel art, front view, transparent background
```

---

## 3. 일반 적 요정 / 32×32

전부 **같은 요정 골격**에 색과 실루엣 힌트만 바꾸는 게 제일 깔끔해.
한 장 잘 나오면 색만 바꿔서 파생시켜도 됨.

### 3-1. Grunt `Fairy_Grunt.png` — 기본 잡몹

```
tiny angry fairy enemy sprite, messy short dark hair, simple red dress,
small translucent butterfly wings, scowling face, clenched fists.
Very simple readable shape, 4-5 colors. Chibi pixel art, front view,
transparent background
```

### 3-2. Tank `Fairy_Tank.png` — 크고 튼튼

```
chunky armored fairy enemy, stout heavy build, dark crimson plate armor
over the chest and shoulders, small tattered wings barely holding her up,
grumpy determined face. Bulky wide silhouette. Chibi pixel art,
front view, transparent background
```

### 3-3. Sidewinder `Fairy_Sidewinder.png` — 빠르고 날렵

```
swift slender fairy enemy, sleek yellow-gold dress, long streaming
ribbons trailing behind, sharp narrow dragonfly wings, sly grin,
leaning forward mid-dash. Thin agile silhouette. Chibi pixel art,
front view, transparent background
```

### 3-4. Shooter `Fairy_Shooter.png` — 원거리

```
ranged fairy enemy, green robe and hood, holding a short crooked wand
with a glowing tip, aiming forward. Small leaf-shaped wings.
Focused squinting expression. Chibi pixel art, front view,
transparent background
```

### 3-5. Splitter `Fairy_Splitter.png` — 죽으면 분열

```
unstable fairy enemy, violet dress with visible glowing cracks running
down the body like it is about to split apart, mismatched uneven wings,
strained grimacing face. Chibi pixel art, front view, transparent background
```

### 3-6. Teleporter `Fairy_Teleporter.png` — 순간이동

```
phasing fairy enemy, deep purple dress, body semi-transparent at the
edges with faint afterimage trails, blurred outline, wings dissolving
into particles. Eerie blank smile. Chibi pixel art, front view,
transparent background
```

### 3-7. Healer `Fairy_Healer.png` — 아군 회복

```
support fairy enemy, mint green nurse-like dress with a small cross
emblem, holding a glowing green orb in cupped hands, gentle closed-eye
smile, soft round wings with a faint glow. Chibi pixel art, front view,
transparent background
```

---

## 4. 게임 오브젝트

### 4-1. 볼 `Circle_16.png` / 16×16

⚠️ **반드시 흰색/회색조로** — 코드가 볼 종류별 색을 곱해서 입힘.

```
small round glowing orb sprite, pure white and light grey only,
soft inner highlight top-left, subtle outer glow ring, perfectly
circular, pixel art, transparent background
```

**대안 (나이프 컨셉)**
```
tiny silver throwing knife sprite viewed from the side, white and grey
metallic only, sharp point, small hilt, clean readable at 16 pixels,
pixel art, transparent background
```

### 4-2. 벽 `Square_32.png` / 32×32 (9-slice)

```
seamless dark stone brick wall tile, gothic mansion masonry,
muted blue-grey palette, subtle mortar lines, tileable on all edges,
pixel art texture, no transparency
```

### 4-3. 조준 크로스헤어 `Scope_32.png` / 32×32

```
pocket watch crosshair reticle, thin circular ring with roman numeral
tick marks, small clock hands crossing at center, pale gold and cream,
thin clean lines, pixel art, transparent background
```

### 4-4. 코인 / XP 오브

```
small spinning gold coin sprite, crescent moon emblem stamped on face,
warm yellow and amber, bright rim highlight, pixel art, transparent background
```
```
small glowing cyan soul orb, soft radial gradient core, faint wisp
trails rising, pale blue and white, pixel art, transparent background
```

---

## 5. 네거티브 프롬프트 (Stable Diffusion 계열 쓸 때)

```
blurry, anti-aliased, smooth gradient, 3d render, realistic, photograph,
watermark, signature, text, multiple characters, cropped, cut off,
white background, checkered background, jpeg artifacts, extra limbs,
full scene, landscape, busy background
```

---

## 6. 도구별 팁

### pixellab MCP (프로젝트에 이미 연결됨)
- `mode: "v3"` 가 품질 제일 좋음 (8방향 강제, 2-9 generation 소모)
- `mode: "pro"` 도 좋지만 20-40 generation 소모
- `view: "high top-down"` 이 현재 카메라랑 제일 잘 맞았음
- `proportions: {"type":"preset","name":"chibi"}` 로 두등신 만들기
- `reference_image_base64` + `mode:"v3"` → 직접 그린 스프라이트를 8방향으로 회전시킬 수 있음

### Midjourney
- `--niji 6` 붙이면 애니 스타일 강해짐
- `--style raw` 로 과한 해석 억제
- 뒤에 `pixel art, 32x32 sprite, transparent background` 명시
- 결과물이 크게 나오므로 다운스케일 + 색 정리 필요

### Aseprite / 직접 작업
- 캔버스는 지정 크기 그대로 (48×48 등)
- 팔레트 제한 걸어놓고 시작하면 톤 유지됨 ([ART_SPEC.md](ART_SPEC.md)의 HEX 참고)
- 실루엣 먼저 → 명암 2단계 → 하이라이트 순서

---

## 7. 뽑고 나서

1. `Assets/Sprites/Characters/` 에 **같은 파일명으로 덮어쓰기**
2. Unity가 자동 리임포트
3. Import Settings 확인: **Sprite / PPU 32 / Point / Uncompressed / Full Rect**
4. 끝 — 프리팹 재배선 필요 없음

새 파일명으로 추가했으면 `Assets/EnemyData/Enemy_XXX.asset` 의
`Sprite Override` 필드에 드래그하면 됨.
