# Tether — Art & FX Spec

> **목적**: 이 문서만 보고 에셋을 뽑으면 바로 프로젝트에 꽂히도록.
> **현재 상태**: `Assets/Sprites/Characters/` 안의 스프라이트는 전부 **AI 임시 플레이스홀더**.
> 같은 파일명으로 덮어쓰면 프리팹/데이터 재배선 없이 즉시 교체됨.

---

## 0. 기술 사양 (반드시 지킬 것)

| 항목 | 값 |
|---|---|
| **PPU (Pixels Per Unit)** | **32** |
| **Filter Mode** | Point (no filter) |
| **Compression** | None / Uncompressed |
| **Alpha Is Transparency** | ✅ |
| **Mesh Type** | Full Rect |
| **Extrude Edges** | 0 |
| **Pivot** | Center |
| **포맷** | PNG, 투명 배경 |
| **게임 해상도 기준** | 960×540 base |
| **아레나 크기** | 12 × 15 월드 유닛 |

> **PPU 32의 의미**: 32픽셀 = 게임 안에서 1 유닛.
> 예) 48px 스프라이트 → 화면에서 1.5 유닛 크기.
> 코드의 `sizeMultiplier`가 여기에 추가로 곱해짐.

### 색 팔레트 (홍마관 톤)

| 역할 | HEX | 용도 |
|---|---|---|
| 배경 딥 | `#0A0D17` | 화면 밖 여백 |
| 아레나 바닥 | `#1A1F2E` | 플레이 영역 |
| 벽 | `#59668C` | 아레나 테두리 |
| 사쿠야 은발 | `#D8DCE8` | 머리 |
| 사쿠야 드레스 | `#3B4A7A` | 메이드복 |
| 스칼렛 (적) | `#E64059` | 일반 적 |
| 골드 (코인/UI) | `#FFD84D` | 코인, 강조 |
| XP 시안 | `#5AD9FF` | XP 오브 |
| 시간정지 마젠타 | `#FF59BF` | 시간정지 UI/이펙트 |

---

## 1. 캐릭터 스프라이트

파일 위치: `Assets/Sprites/Characters/`
**같은 파일명으로 덮어쓰기 = 자동 반영**

### 1-1. 플레이어

| 파일명 | 권장 크기 | 설명 |
|---|---|---|
| `Sakuya.png` | **48×48** | 이자요이 사쿠야. 은발 트윈 브레이드, 흰 헤드밴드, 청색 메이드복 + 흰 앞치마. 손에 은나이프. 정면(south) 1방향만 필요. |

**주의**: 게임은 항상 정면 뷰만 씀 (회전 없음). 4방향 필요 없음.
아래를 향하는 **high top-down** 앵글이 현재 씬 카메라와 맞음.

### 1-2. 보스 3종

| 파일명 | 권장 크기 | 설명 | 게임 내 크기 |
|---|---|---|---|
| `Boss_Cirno.png` | **64×64** | 얼음 요정. 단발 하늘색 머리 + 큰 리본, 파란 원피스 + 흰 앞치마, **육각 얼음 결정 날개 6장** | 2.2 유닛 |
| `Boss_Patchouli.png` | **64×64** | 병약한 보라 마녀. 긴 보라 머리 + 나이트캡, 초승달/별 무늬 파자마 로브, **펼친 마법서** | 2.4 유닛 |
| `Boss_Yukari.png` | **64×64** | 금발 웨이브 + 보라 리본, 보라/핑크 드레스, **닫힌 파라솔**, 미소 | 2.3 유닛 |

**보스 특징 어필 포인트**: 실루엣이 멀리서 봐도 구분되게. Cirno=뾰족한 날개, Patchouli=둥근 로브+책, Yukari=긴 파라솔 실루엣.

### 1-3. 일반 적

| 파일명 | 권장 크기 | 설명 | 게임 내 크기 |
|---|---|---|---|
| `Fairy_Grunt.png` | **32×32** | 기본 요정. 빨간 원피스, 작은 투명 나비 날개, 화난 표정 | 0.55 유닛 |
| `Fairy_Tank.png` | **48×48** | 뚱뚱한 장갑 요정. 짙은 붉은 판금 갑옷, 찢어진 날개 | 0.7 유닛 |

**추가로 뽑으면 좋은 것 (현재는 Grunt 스프라이트 공용):**

| 파일명 | 크기 | 설명 |
|---|---|---|
| `Fairy_Sidewinder.png` | 32×32 | 노란색 계열, 날렵한 실루엣 (위빙 이동) |
| `Fairy_Shooter.png` | 32×32 | 녹색 계열, 원거리 무기/지팡이 들고 있음 |
| `Fairy_Splitter.png` | 32×32 | 보라 계열, 몸이 둘로 갈라질 것처럼 균열 |
| `Fairy_Teleporter.png` | 32×32 | 보라/자주, 반투명하거나 흐릿한 잔상 |
| `Fairy_Healer.png` | 32×32 | 녹색, 십자가나 회복 심볼 |

> 새 파일 추가시 `Assets/EnemyData/Enemy_XXX.asset` 의 `spriteOverride` 필드에 드래그하면 끝.

---

## 2. 게임플레이 오브젝트

파일 위치: `Assets/Sprites/Prototype/` (현재 절차적 생성 도형)

| 파일명 | 현재 | 교체 제안 | 크기 |
|---|---|---|---|
| `Circle_16.png` | 흰 원 | **볼** — 사쿠야의 나이프/보주 느낌. 흰색으로 그리면 코드가 볼 종류별 색 틴트 자동 적용 | 16×16 |
| `Square_32.png` | 흰 사각 | **벽** — 홍마관 석재 타일. Sliced 스프라이트로 늘어남 (9-slice 권장) | 32×32 |
| `Square_16.png` | 흰 사각 | (레거시, 안 써도 됨) | 16×16 |
| `Scope_32.png` | 링 + 십자 | **조준 크로스헤어** — 회중시계 문양이면 사쿠야 테마 딱 | 32×32 |

**중요**: 볼 스프라이트는 **흰색/회색조로** 그려야 함. `Ball.Configure()`가 BallData의 `tintColor`를 곱해서 종류별 색을 입힘.

### 볼 종류별 색 (자동 틴트, 참고용)

| 볼 | HEX |
|---|---|
| Normal | `#FFD84D` 노랑 |
| Split | `#99D9FF` 하늘 |
| Explosive | `#FF6640` 주황 |
| Piercing | `#E5D9FF` 연보라 |
| Homing | `#FF8CD9` 핑크 |
| Lightning | `#B3F2FF` 밝은 하늘 |
| Freeze | `#8CD9FF` 얼음 |

---

## 3. UI 에셋

현재 전부 Unity 기본 흰 사각형 + LegacyRuntime 폰트. 교체하면 체감 큼.

### 3-1. 우선순위 높음

| 에셋 | 용도 | 권장 |
|---|---|---|
| **버튼 나인슬라이스** | START RUN / SHOP / QUIT / 카드 / 노드 | 9-slice PNG, 최소 32×32, 모서리 8px |
| **패널 나인슬라이스** | 상점 배경, 게임오버 패널, 업그레이드 모달 | 9-slice PNG, 어두운 반투명 + 금색 테두리 |
| **폰트** | 전체 UI | 픽셀 폰트 (한글 지원되면 더 좋음). TMP 에셋으로 변환 필요 |

### 3-2. 아이콘 (있으면 좋음)

| 아이콘 | 용도 | 크기 |
|---|---|---|
| HP 하트 | HP 표시를 숫자→하트로 | 16×16 |
| 코인 | COINS 라벨 옆 | 16×16 |
| XP 오브 | LVL 라벨 옆 | 16×16 |
| 시계 | TIME STOP 게이지 | 16×16 |
| 유물 5종 | 획득 유물 HUD 스트립 | 24×24 |
| 볼 7종 | 볼 슬롯 표시 | 24×24 |
| 노드 5종 (Combat/Elite/Rest/Shop/Boss) | 맵 화면 | 32×32 |

### 3-3. 무료 리소스 추천

- **Kenney.nl** (CC0, 상업 사용 무제한) — UI Pack, Particle Pack, Game Icons
  - https://kenney.nl/assets/ui-pack
  - https://kenney.nl/assets/particle-pack
- **itch.io** 픽셀 UI 팩 검색 (CC0/CC-BY 필터)
- **OpenGameArt.org** — 사운드/폰트/스프라이트

---

## 4. VFX / 이펙트 (코드로 구현 가능 — 요청하면 바로)

이 항목들은 **에셋 없이 코드만으로** 구현 가능. 필요하면 말해줘.

### 4-1. 현재 이미 있는 것 ✅

| 이펙트 | 구현 |
|---|---|
| 화면 흔들림 | `CameraShaker` — Perlin noise 기반 trauma 시스템 |
| Hitstop | `HitStop` — 적 처치시 0.04초 프레임 정지 |
| 적 죽음 파편 | 5조각 스프라이트가 방사형으로 흩어지며 페이드 |
| 볼 트레일 | `TrailRenderer` — 볼 색상 그라디언트 |
| 번개 arc | Lightning 볼 체인시 `LineRenderer` 0.15초 페이드 |
| 조준선 | `LineRenderer` — 마우스 방향 |
| Nova 폭발 | 확대되는 원 스프라이트 페이드 |
| 피격 플래시 | 플레이어 무적 중 색 깜빡임 |

### 4-2. 추가 가능 (요청시 구현) 🔧

| 이펙트 | 설명 | 난이도 |
|---|---|---|
| **데미지 숫자 팝업** | 적 맞을 때 숫자가 튀어오르며 페이드 | 쉬움 |
| **적 HP 바** | 머리 위 작은 게이지 (피격시만 표시) | 쉬움 |
| **코인 픽업 반짝** | 흡수될 때 작은 스파클 파티클 | 쉬움 |
| **웨이브 시작 배너** | "WAVE 2" 텍스트가 화면 가로질러 슬라이드 | 쉬움 |
| **콤보 팝** | 콤보 수 오를 때 숫자가 커졌다 작아짐 | 쉬움 |
| **시간정지 화면 효과** | 발동시 화면 전체 마젠타 틴트 + 비네트 + 시계 링 확산 | 중간 |
| **볼 잔상** | 고속 이동시 반투명 잔상 여러 개 | 중간 |
| **적 스폰 워프** | 위에서 등장할 때 페이드인 + 스케일업 | 쉬움 |
| **벽 임팩트 링** | 볼이 벽에 부딪힌 지점에 확산되는 링 | 쉬움 |
| **화면 비네트** | HP 낮을 때 화면 가장자리 붉게 | 중간 |
| **URP 2D 라이팅** | 볼/적/플레이어에 광원 부착 (프로젝트에 URP 이미 있음) | 중간 |
| **포스트 프로세싱 블룸** | 밝은 오브젝트 발광 (URP Volume) | 중간 |
| **파티클 시스템 전환** | 현재 스프라이트 파편 → 진짜 ParticleSystem | 중간 |

---

## 5. 사운드 (현재 100% 절차적 생성)

`ProceduralAudio.cs`가 런타임에 파형 합성. **외부 파일 0개.**

| ID | 현재 소리 | 교체시 넣을 위치 |
|---|---|---|
| `ball_fire` | 사각파 상승 chirp | `AudioManager.GenerateLibrary()` |
| `ball_bounce` | 사인파 낮은 blip | 〃 |
| `ball_hit_enemy` | 노이즈 + 사각파 | 〃 |
| `enemy_die` | 톱니파 하강 스윕 | 〃 |
| `player_hit` | 노이즈 + 삼각파 | 〃 |
| `player_die` | 긴 하강 스윕 | 〃 |
| `wave_start` | 2음 스팅어 | 〃 |
| `wave_clear` | 3음 상승 (C-E-G) | 〃 |
| `upgrade_pick` | 스파클 상승 | 〃 |
| `bgm_loop` | 삼각파 8음 루프 | 〃 |

**실제 오디오 파일로 바꾸려면**: `Assets/Audio/` 폴더 만들고 WAV/OGG 넣은 뒤,
`AudioManager`에 `[SerializeField] AudioClip[]` 추가해서 절차적 생성 대신 참조하면 됨.
(요청하면 그 구조로 리팩터링해줄게)

---

## 6. 교체 워크플로우

### 스프라이트 교체 (가장 간단)
1. 같은 파일명으로 `Assets/Sprites/Characters/`에 덮어쓰기
2. Unity가 자동 리임포트
3. Import Settings 확인: **Sprite / PPU 32 / Point / Uncompressed**
4. 끝. 프리팹 재배선 불필요.

### 새 적 스프라이트 추가
1. `Assets/Sprites/Characters/Fairy_XXX.png` 저장
2. Import Settings 위와 동일하게
3. `Assets/EnemyData/Enemy_XXX.asset` 인스펙터 열기
4. `Sprite Override` 필드에 드래그
5. 필요하면 `Size Multiplier` 조정

### UI 나인슬라이스 적용
1. PNG를 `Assets/Sprites/UI/`에 저장
2. Import Settings: Sprite (2D and UI)
3. **Sprite Editor** 열어서 Border 값 설정 (예: L8 R8 T8 B8)
4. UI Image 컴포넌트의 Source Image에 배정 + Image Type = **Sliced**

### 폰트 교체
1. TTF/OTF를 `Assets/Fonts/`에 넣기
2. 현재 UI는 **Legacy Text** 사용 중 → 그냥 Font 필드에 배정
3. TextMeshPro로 갈 거면 전체 UI 코드 마이그레이션 필요 (요청하면 해줄게)

---

## 7. 현재 파일 인벤토리

```
Assets/Sprites/
├── Characters/          ← AI 플레이스홀더, 교체 대상
│   ├── Sakuya.png              (68×68 캔버스, 48px 캐릭터)
│   ├── Boss_Cirno.png          (92×92)
│   ├── Boss_Patchouli.png      (92×92)
│   ├── Boss_Yukari.png         (92×92)
│   ├── Fairy_Grunt.png         (48×48)
│   └── Fairy_Tank.png          (68×68)
└── Prototype/           ← 절차적 생성 도형
    ├── Circle_16.png           (볼, 코인, XP 오브 공용)
    ├── Square_16.png           (레거시)
    ├── Square_32.png           (벽, 바닥)
    └── Scope_32.png            (조준 크로스헤어)
```

---

## 8. 우선순위 제안

**1순위 (체감 최대)**
- `Sakuya.png` — 플레이어는 항상 화면에 있음
- `Fairy_Grunt.png` — 가장 많이 나오는 적
- UI 버튼/패널 나인슬라이스 — 모든 메뉴에 영향

**2순위**
- 보스 3종 — 임팩트 순간
- `Circle_16.png` (볼) — 항상 여러 개가 날아다님
- 픽셀 폰트

**3순위**
- 적 종류별 개별 스프라이트
- 아이콘 세트
- 벽/바닥 타일 텍스처

---

*코드 쪽 FX는 이 문서 4-2 목록에서 골라서 말해주면 바로 구현 들어감.*
