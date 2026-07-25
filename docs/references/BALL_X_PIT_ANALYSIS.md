# Ball x Pit — Analysis & Mapping Ideas

> **목적**: kimringo와 첫 기획 세션 전에 팀이 *공통 언어*로 얘기할 수 있게 Ball x Pit를 분석. 우리 게임(동방 2차 창작 라인)에 어떤 부분 가져오고 어떤 부분 다르게 할지 매핑 아이디어 초안.
>
> **상태**: 웹 리서치 기반 분석 (플레이 경험 X). 첫 기획 세션에서 논의/조정 대상.
> **작성**: 2026-07-24, 팀 세션 준비용

---

# Part 1 — Ball x Pit 게임 분석

## 1. 게임 개요

| 항목 | 내용 |
|---|---|
| **개발** | Kenny Sun |
| **퍼블리싱** | Devolver Digital |
| **출시** | 2025-10 (PC/PS5/Xbox/Switch), 2026-03 (iOS/Android) |
| **판매** | 1M+ 카피 (2026-01 기준) |
| **최신 업데이트** | Regal Update (2026-01) — 2 새 캐릭터, 8 새 볼, Endless Mode |
| **장르** | 로그라이트 + Arkanoid + Vampire Survivors 하이브리드 |

## 2. 핵심 게임플레이 루프

```
정면 아레나 뷰 (탑에서 적 하강)
        ↓
캐릭터가 볼 발사 → 튀김 → 적 처치
        ↓
Level 3 볼 2개 → Fusion Reactor에서 융합
        ↓
스테이지 클리어 → 홈베이스(New Ballbylon)로 귀환
        ↓
자원(Gold/Wheat/Wood/Stone)으로 건물 짓기
        ↓
다음 스테이지 도전 or 반복 플레이
```

- 매 런 ~20분 안팎
- Auto-fire 옵션 + 게임 속도 조절
- 특수 볼 *잡아서 재사용* (숙련도 요소)

## 3. Ball 시스템

- **60+ 볼 종류**
- 카테고리:
  - **Enablers** — 광역 공격 (레이저, 폭발 등)
  - **Baby Makers** — 새 볼 생성 (분열, 소환)
  - **Effect Balls** — 데미지/속성 부여 (Burn, Poison, Freeze 등)
- 각 볼 **Level 3까지 진화** (레벨 업)
- 예: Burn, Iron, Light, Earthquake, Dark, Bleed, Freeze, Poison, Laser, Bomb…

## 4. Ball Fusion 시스템 (⭐ 게임 핵심)

**Level 3 볼 2개 → Fusion Reactor에서 융합**

- 결과: 두 능력 합쳐진 새 볼 (이름 + 아이콘 변경)
- **42가지 Fusion recipe** 존재
- **런 내 영구** — 한 번 융합하면 못 되돌림
- 예시:
  - `Bomb` = Burn + Iron
  - `Sun` = Burn + Light
  - `Magma` = Burn + Earthquake
  - `Holy Laser` = Light + Light (same-type)
  - `Shadow Ball` = Dark + Dark (same-type)
- **Ultimate 3-ball fusions**: Nosferatu, Plasma (최상급)
- **Enabler + Baby Maker + Effect** 조합이 강력

## 5. 캐릭터 시스템

- **21+ 플레이어블 캐릭터** (2026-05 기준)
- 각자 **고유 능력** + **고유 Base Ball** (예외: The Warrior)
- Regal Update로 2 추가:
  - **The Carouser** — 볼이 캐릭터 주변 궤도 (공/방 필드)
  - **The Falconer** — 팔콘 2마리가 독립적으로 볼 던짐 (companion 시스템)
- **해금 방법**: 특정 건물 짓기 → 그 건물 blueprint는 특정 biome 클리어에서 얻음

## 6. 스테이지 & 웨이브 구조

- **8 스테이지 (레벨)** — 각자 biome/적/보스 다름 (첫 스테이지: The BONExYARD)
- 각 스테이지 구조:
  - **2 stage boss** + **1 final boss**
  - stage boss 격파 후 → **Fusion upgrade 보장** (핵심 리워드)
  - 클리어 시 → **faster version** 잠금해제 (meta progression 필요)
- 다음 biome 잠금:
  - 특정 **gear 수 수집** → **Pit Lift에 넣음** → 새 biome 잠금해제
  - 즉 반복 플레이 강제 (2회+ 필요)

## 7. 시티 빌딩 — New Ballbylon (홈베이스)

- 웨이브 사이 홈베이스로 귀환
- **70+ 건물**
- **자원 4종**: Gold, Wheat, Wood, Stone
- 건물 카테고리:
  - **Warfare** (전투 강화)
  - **Character 해금** (캐릭터별 집)
  - **Gold mines** (자원)
  - **Farming** (자원)
- **Blueprint 얻는 방법**:
  - Pit run 완료 시 자동 드랍
  - 히든 요구사항 (일부)
- **건물 짓기 메카닉**: 캐릭터 발사 → 스캐폴딩에 튕겨서 진행 (⭐ 게임 메카닉 재사용)
- **배치 자유롭게 변경** 가능 (실험 유도)
- **추천 레이아웃**: 3 파트 분할 (한쪽 Warfare/Character, 중앙 Gold Mines, 반대쪽 Farming)

## 8. 로그라이트 진행

- 매 런 **다른 볼 조합** (Ball Fusion으로 매번 다른 빌드)
- **실패해도 base building 유지** → 메타 진행
- **매 스테이지 반복 플레이 요구** (faster version, blueprint 완전 수집 등)

## 9. 재미 요소 & 약점 (리뷰 기반)

### 재미 (강점)
- **Ball 조합 발견의 재미** — "어? 이거랑 이거 합치면?" (실험적 즐거움)
- **약함 → 강함 → 게임 브레이커** 사이클 (Vampire Survivors 라인)
- **20분 런** — 부담 없음, 반복 쉬움
- **큰 규모의 콘텐츠** — 60+ 볼, 21 캐릭터, 70+ 건물, 8 스테이지 = 재플레이 오래 감
- **명상적 분위기** — 몰입 잘 됨 (일부 리뷰)

### 약점 (한계)
- **Holy Laser 등 오버파워 조합** 존재 → 밸런싱 어려움
- **일부 스테이지 반복 강제** → 지루할 수 있음
- **초반 배우기 부담** (60+ 볼 + 42 fusion recipe = 정보 많음)

---

# Part 2 — 우리 프로젝트 매핑 아이디어 (초안)

> **⚠️ 이 파트는 초안**. 첫 기획 세션에서 kimringo와 논의/확정.

## 매핑 표 (아이디어 제안)

| Ball x Pit 요소 | 우리 프로젝트 매핑 후보 |
|---|---|
| **21 캐릭터 로스터** | 동방 캐릭터: 레이무, 마리사, 사쿠야, 유카리, 레미리아, 알리스, 요우무, 파츄리, 산에, 츄루노 등 (초기 3~5명, 확장 20+명) |
| **Base Ball** (캐릭터별 시작 볼) | 각 캐릭터의 상징 — 레이무=음양옥, 마리사=마스터 스파크, 사쿠야=칼, 파츄리=마도서 |
| **60+ 볼 종류** | 동방 스펠카드 / 탄막 패턴 — Burn=진심 폭발, Light=신령 광선, Dark=요괴 그림자 등 |
| **Ball Fusion (42 recipe)** | 스펠카드 조합 — 원작에서 캐릭터가 스펠카드로 이변 해결하는 것 정확히 매핑. "이변 스펠카드" 컨셉 |
| **Fusion Reactor** | 파츄리의 도서관? 마리사의 실험실? 소환 서클? |
| **New Ballbylon (홈베이스)** | **하쿠레이 신사** (레이무 본거지, 환상향 심장). 캐릭터별 집 = 각자의 거점 (홍마관, 영원정, 명계 등) |
| **8 스테이지 biome** | 동방 지역: ① 인간 마을 ② 마법의 숲 ③ 홍마관 ④ 영원정 ⑤ 지령전 ⑥ 성련선 ⑦ 신령묘 ⑧ 천계 (초기엔 3~4개 시작) |
| **4 자원 (Gold/Wheat/Wood/Stone)** | 옵션 A: 그대로 유지 (친숙). 옵션 B: 동방식 (신앙심/쌀/나무/술) |
| **Blueprints** | 스펠카드 카드 or 캐릭터 초대장 (원작 스토리 라인) |
| **Gear + Pit Lift** | 결정 카드? 인장? 환상향 이동 티켓? |
| **웨이브 (적 하강)** | 요괴 무리 or 이변 발생 (원작 이변 = 서브 스토리 훅) |
| **2 stage boss + 1 final boss** | 원작 5~6 스테이지 보스 구조 준수 — 스테이지 중보스 + EX 보스 |

## 결정해야 할 큰 질문들 (기획 세션 아젠다)

1. **동방 IP 스코프**
   - 어느 작품 중심? (홍마향/요요몽/영야초/풍신록 등)
   - 오리지널 캐릭터 넣을지, 원작 100%인지
   - ZUN 가이드라인 얼마나 반영 (팬 프로젝트 vs 수익형)

2. **뷰 방향 & 카메라**
   - Ball x Pit 정면 아레나 그대로?
   - 동방 원작(탄막 슈팅 세로 스크롤)과 하이브리드?

3. **Ball / 스펠카드 시스템 상세**
   - 볼 수 (60개 그대로? 우리는 20~30 정도 시작?)
   - Fusion recipe 수 (42개 그대로? 우리는 15~20?)
   - 동방 스펠카드 시스템 어떻게 결합?

4. **캐릭터 로스터 (초기)**
   - 시작 3~5명 추천: 레이무 + 마리사 (필수) + 2~3명
   - 각 캐릭터의 Base Ball 컨셉

5. **홈베이스 컨셉**
   - 하쿠레이 신사 유력
   - 건물 = 캐릭터 집 vs 자원 시설 vs 스펠카드 도서관

6. **스토리 프레임**
   - 이변 발생 → 레이무/마리사가 조사 → 원인 격퇴 (원작 표준)
   - 또는 오리지널 스토리

7. **프로토타입 스코프 (1 스테이지)**
   - 캐릭터: 1명 (레이무만?)
   - 볼: 10~15종
   - Fusion: 5~10 recipe
   - 웨이브: 3~5분 짜리 미니 아레나
   - 홈베이스: 없이 or 간단히
   - 목표: 2~3주 안에 굴러가는 빌드

## 프로토타입 우선순위 제안

**Phase 1 — 코어 아레나** (1주)
- 캐릭터 1명 (레이무)
- Base Ball 1종 (음양옥)
- 웨이브 시스템 (60초 웨이브 × 3)
- 아레나 정면 뷰 + 볼 발사/튐/충돌

**Phase 2 — Ball 다양성 + Fusion** (1주)
- 추가 볼 10종
- Level up 시스템
- Fusion Reactor + 5 recipe

**Phase 3 — 스테이지 + 홈베이스 최소** (1주)
- 하쿠레이 신사 홈베이스 (건물 3~5개)
- 자원 시스템
- 1 스테이지 클리어 → 홈베이스 귀환 → 다시 도전

이 정도 나오면 "폐기 vs 확장" 판단 가능.

---

# Part 3 — 라이센스 / IP 주의사항

## ZUN 동방프로젝트 2차 창작 가이드라인 (요약)

- **비영리 우선** — 팬 프로젝트로 가는 게 안전
- 소액 유료 (Comiket, Steam 저가) 는 관례적 허용 (예: Touhou Luna Nights $18)
- 캐릭터/음악 자유 사용 (원작 존중)
- **금지**: 큰 상업 프랜차이즈화, 원작 훼손, ZUN 승인 없이 대규모 유통

**우리 방침 (초안)**: 프로토타입은 팬 프로젝트로. 상업화 여부는 프로토타입 완성 후 결정.

---

# 참조

- [BALL x PIT on Steam](https://store.steampowered.com/app/2062430/BALL_x_PIT/)
- [BALL x PIT Wiki](https://ballxpit.wiki.gg/wiki/Balls)
- [Ball x Pit Fusions Guide](https://ballxpit.net/fusions)
- [Complete Beginner Guide To Ball x Pit — The Gamer](https://www.thegamer.com/ball-x-pit-complete-guide/)
- [Regal Update details](https://www.dlcompare.com/gaming-news/ball-x-pit-is-expanding-with-new-characters-and-more)
- [Buildings Guide — Deltia's Gaming](https://deltiasgaming.com/ball-x-pit-buildings-guide/)
- [Levels & Bosses Guide](https://ballxpit.wiki.gg/wiki/Levels)
- [Wikipedia — Ball x Pit](https://en.wikipedia.org/wiki/Ball_x_Pit)

---

# Part 4 — 영상 시청 후 발견 (v2 update, 2026-07-24)

> **출처**: Dry Bear의 "ULTIMATE Beginner's Guide" 영상 (17분) 시청 결과
> **성격**: 웹 리서치로 못 잡았던 UX/시스템 발견들

## ⭐ 캐릭터 조합 시스템 (Character Combo) — 큰 발견

- **1 런 = 2 캐릭터 조합** (Primary + Secondary)
- "Bring a Second Character?" 화면에서 페어 선택
- 두 passive/base ball이 결합 → 완전히 다른 플레이
- 예시 (영상에서 시연):
  - **The Shade × The Embedded** = "볼이 뒤에서 발사 + 치명타 10% + 벽 도달까지 관통"

**우리 프로젝트 매핑 (동방 특화)** — 이거 진짜 강력:
- **동방은 커플/듀오/트리오가 원래 많음** — 팬 서비스 극대화
- 매핑 후보:
  - **레이무 × 마리사** (원작 짝꿍, 필수)
  - **레미리아 × 플랑도르** (스카알렛 자매)
  - **사쿠야 × 홍 메이링** (홍마관 동료)
  - **유유코 × 요우무** (명계 주종)
  - **알리스 × 마리사** (팬 인기)
  - **유카리 × 란** (경계 & 시키가미)
- **팬 서비스 밀도가 원작 오마주 관점에서 최고 지점**

## Attributes 시스템 (스탯)

캐릭터 정보 화면에 6개 스탯:
- **Endurance** (내구)
- **Strength** (힘)
- **Leadership** (지도력)
- **Speed** (속도)
- **Dexterity** (민첩)
- **Intelligence** (지력)

Ball 데미지 계산 외에 캐릭터 스탯 시스템 존재. 우리 초기 분석에 없었던 요소.

**우리 프로젝트 결정 필요**: 도입 여부. 도입 시 캐릭터별 개성 명확화. 미도입 시 Base Ball + passive만으로 차이 냄.

## Baby balls vs Main balls (핵심 시스템)

**Main balls** (기본):
- **Boomerang 형태** — 나갔다가 돌아옴
- **잡아야 함** (숙련도 요소!) — 못 잡으면 딜레이
- 우리 게임에서: 나비욧드 강점 (숙련도 게임 필/타이밍)

**Baby balls** (파생):
- 여러 spawn 방법 (특정 볼/이벤트로)
- **자유롭게 bounce & damage** — 잡을 필요 X
- Vampire Survivors식 자동 공격 요소

**빌드 개념 = 이 둘의 조합.** Main 위주 = 정밀 컨트롤, Baby 위주 = 자동 자원 확장.

**우리 매핑**: 
- Main = 캐릭터의 스펠카드 (원작 대표기)
- Baby = 파생 탄막 (원작 부속 탄막)
- 예: 마리사 Main = 마스터 스파크 / Baby = 별탄

## Passives 시스템 (부적/유물)

Fusion recipe와 별도로 **Passives 그리드 컬렉션** 존재:
- Passives 화면에 여러 아이템 그리드 (한 페이지 4×5 = 20+)
- 언락형 (일부는 ??? 표시)
- Hollow Knight 부적 시스템과 유사

**우리 프로젝트 매핑**: 우리 컨셉의 "유물 슬롯"이 이거. 확정.

## In-match Level Up + 3-choice

- Blue gem 수집 = XP
- Level up 시 **3 offering** 선택 (item + ball 섞임)
- **매 레벨업이 결정 순간** — 게임 리듬의 핵심
- Vampire Survivors 라인 정확

## 시각/UI 발견

- **정면 세로 스크롤 아레나** — 우리 뷰 방향 확정 (변경 X)
- **환경 다양성 (biome)** — 각 스테이지마다 색조/톤 명확
- **홈베이스 UI (New Ballbylon)** — 격자 배치, 자원 4종 (Gold/Wheat/Wood/Stone) 상단 표시, 좌측 도구 버튼
- **캐릭터 선택 UI** — 그리드 15+ 아이콘, 언락 %표시 (+20%, +25% 등)
- **UI 톤** — Dark theme + Gold accent + Gothic. 우리 UI 참고 (근데 우리는 동방 톤 필요)

---

# Part 5 — 세션 논의 추가 항목 (v2, 캐릭터 조합 기반)

기존 Part 2 매핑에 추가로:

## 캐릭터 조합 시스템 도입 결정 (⭐ 핵심)

**세션 아젠다 추가**:
- 캐릭터 조합 시스템 채택 여부
- 채택 시: 초기 로스터에 어떤 페어를 넣을지 (레이무×마리사 필수, 나머지는?)
- Primary + Secondary 뜻이 우리 게임에서 뭐 (같이 싸움? 하나만 활성?)

## Attributes 스탯 시스템 도입 결정

- 도입 vs 스킵
- 도입 시: 6개 그대로 vs 3~4개로 축소 (Simplicity)
- 원작 캐릭터마다 스탯 다르게 (레이무는 균형, 마리사는 Intelligence 높음 등)

## Baby ball 개념

- Baby balls을 우리 게임 어디에 매핑?
  - 동방 미니 탄막 (원작 스펠카드의 파생 탄)
  - 별도 자동 공격 (완전 자동)
- 초기엔 Main만? Baby도 넣기?

---

# Part 6 — 갱신 이력

- **v1 (2026-07-24 초기)**: 웹 리서치 기반. Part 1~3.
- **v2 (2026-07-24 오후)**: 영상 시청 발견 반영. Part 4~5 추가. 특히 캐릭터 조합 시스템 발견은 큰 함의.

