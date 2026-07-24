# Tether — Map System Design (ARCHIVED)

> ⚠️ **ARCHIVED — 구 방향 문서 (2026-06 ~ 2026-07-24)**
> 이 문서는 **이전 Tether 방향** (사이드뷰 메트로배니아 + 수직 등반 + 보스 능력 흡수)의 맵 시스템 설계.
> 2026-07-24 방향이 **Ball x Pit + 동방 2차 창작**으로 재정립되면서 이 설계는 무효.
> 참고용 보존만 유지. 새 방향의 맵/레벨 시스템은 kimringo 협의 후 새로 작성 예정.

---

> **버전**: 0.1 (Aestik devlog 라인 + Tether 특화)
> **상태**: 디자인 초안 — 나비욧드 검토 대기
> **레퍼런스**: Aestik (Kiss The Button) 메트로배니아 맵 시스템

---

## 핵심 컨셉

Tether의 맵 시스템은 **수직 탑 단면도** 형태. 메이드 인 어비스의 *깊이 단면도*를 역방향으로 — 위로 갈수록 더 미스터리하고 어려운 영역이 *uncover*되는 구조.

> *"플레이어가 정상에 가까워질수록 신의 그림자가 짙어진다."*

---

## 1. 시각적 형태

### 맵 = 수직 탑 단면도

```
[Floor ∞ - 신의 방]    ← 진엔딩 잠금
[Floor 7 - 정상 사원]
[Floor 6 - 차가운 신성]
─────────────────────
[Floor 5 - 균열의 다리]
[Floor 4 - 잊혀진 폐허]
[Floor 3 - 중층 통로]
─────────────────────
[Floor 2 - 바닥 신전]
[Floor 1 - 시작 마을]   ← 항상 보임
```

- 각 floor는 **횡 단면도 PNG 1장** (Aestik 라인)
- floor 사이 **연결선** = 사다리, 균열, 텔레포터 등
- 화면 우측 **높이 게이지** — 매 런 도달 높이 기록 (메모리 사양)

### 메이드 인 어비스 톤 색 그라데이션

- **바닥 (1-2층)**: 따뜻한 갈색/녹색 (마을, 자연)
- **중층 (3-5층)**: 회색/보라 (폐허, 균열)
- **상층 (6-7층)**: 차가운 시안/푸른 보라 (신성, 공기 희박)
- **신의 방**: 흰색/금색 (절대 빛)

### 발견된 / 미발견 시각 차이

| 상태 | 시각 |
|---|---|
| **미발견** | 어둠 silhouette (윤곽만 살짝, 검은 안개) |
| **발견됨** | 풀 컬러 + 디테일 |
| **현재 위치** | 캐릭터 silhouette + 발광 효과 (남캐는 황색, 여캐는 보라) |
| **귀 흡수 위치** | 작은 동물귀 아이콘 |
| **세이브 포인트** | 작은 빛 아이콘 |

---

## 2. Pause Menu 통합

Aestik 라인 — Pause Menu에서 탭 전환:

```
┌─────────────────────────────────────┐
│  GAME PAUSED                        │
├─────────────────────────────────────┤
│  [Resume]                           │
│  [Map] ← 탭 1                       │
│  [Collection] ← 탭 2 (귀 모음)      │
│  [Controls] ← 탭 3                  │
│  [Quit]                             │
└─────────────────────────────────────┘
```

### 탭별 내용

#### 🗺️ Map 탭
- 수직 탑 단면도 (위에 명시)
- 현재 위치 + 발견한 챔버 + 미발견 silhouette
- 좌측: 미니 정보 (현재 floor, 발견한 챔버 수)
- 우측: 흡수한 귀 가까운 위치 마커

#### 📦 Collection 탭 (Tether 특화)
- 흡수한 *동물귀 목록* (그리드)
- 클릭하면 디테일 (이름, 보스, 능력, 어느 floor에서 얻었는지)
- 미발견 귀는 silhouette
- 메타 진행: 한 번이라도 발견한 귀는 다음 런에 영구 추가

#### ⚙️ Controls 탭
- 키 매핑 표시
- 변경 가능 (Input System Action Rebinding)

---

## 3. 챔버 발견 (Chamber Discovery)

### 진입 시 자동 발견
- 플레이어가 챔버에 처음 들어가면 → 그 챔버 silhouette → 풀 컬러 전환
- 발견 알림 짧게 (오른쪽 위 토스트)

### 데이터 구조 (ScriptableObject)

```csharp
[CreateAssetMenu]
public class ChamberData : ScriptableObject
{
    public string ChamberId;
    public int Floor;
    public Vector2Int MapPosition;  // 탑 단면도 grid 좌표
    public Sprite MapPNG;            // 챔버 모양 PNG
    public Sprite DiscoveredSprite;  // 풀 컬러
    public Sprite UndiscoveredSprite; // silhouette
    public ChamberType Type;         // Normal, Rest, Boss, Secret, NPC
}
```

### 발견 상태 저장
- `PlayerProgressSO` (ScriptableObject) 또는 JSON 세이브
- 키: `chamberId` → 발견 여부
- 메타 시스템 (로그라이트): 한 번 발견하면 영구 저장

---

## 4. 듀얼 캐릭터 시점 (Tether 특화)

### 1회차 — 남캐
- 발견한 챔버에 *남캐 발자국* 트레일 표시
- 남캐 전용 비밀 경로 마크 (짐승화 점프)

### 2회차 — 여캐 (1회차 클리어 후)
- 1회차 발견 챔버는 *유지* — 진행 인계
- 추가 발견 챔버는 *여캐 발자국* 트레일 다른 색
- 여캐 전용 비밀 경로 마크 (텔레포트, 마법 잠금 해제)

### 시점 교차 UI
- 같은 챔버를 1회차 / 2회차 둘 다 방문하면 → 챔버 디테일에서 *두 시점 메모* 둘 다 표시
- 결말은 *두 시점 모두 봐야* 의미 드러남 (CONCEPT.md 진엔딩)

---

## 5. 높이 게이지 (메모리 사양)

화면 우측 항상 표시:

```
   ┌─┐
   │∞│ ← 정상
   │6│
   │5│
   │4│
 ▶ │3│ ← 현재 floor
   │2│
   │1│
   └─┘
```

- 매 런 최대 도달 높이 기록 (런 종료 시 저장)
- 메타 시스템: 최고 기록 표시
- 진엔딩 잠금 — 100% chamber 발견 + 두 시점 클리어

---

## 6. UI 레퍼런스 사이즈

### Map Panel
- 전체 화면 80% 차지
- 폰트: PixelPerfect 픽셀 폰트 (추후 결정)
- 캔버스: 1920×1080 기준 (PixelPerfect)

### 챔버 PNG 사이즈
- 작은 챔버: 64×48 px
- 중간 챔버: 128×96 px
- 큰 챔버 (보스방, NPC 거점): 192×128 px
- 모두 같은 grid 단위 (16×16 cell)

---

## 7. 구현 우선순위 (Task #28)

### Phase A — Core (먼저)
1. **PauseMenuController** — Pause/Resume + 탭 전환
2. **MapPanel UI** — 빈 grid 표시
3. **ChamberData** ScriptableObject 셋업
4. **ChamberDiscoveryTracker** — 현재 위치 + 발견 챔버 set 관리
5. **HeightGauge** — 화면 우측 HUD

### Phase B — Collection
6. **CollectionPanel UI** — 흡수한 귀 그리드
7. **EarData** ScriptableObject (보스 능력 정의)
8. 메타 진행 연결

### Phase C — 듀얼 캐릭터 폴리시
9. 남캐 / 여캐 시점별 발자국 트레일
10. 시점 교차 메모 시스템

### Phase D — Controls Rebinding
11. **ControlsPanel UI** + Input Action Rebinding

---

## 8. 결정 사항 (확정)

- [x] **탑 floor 개수**: **7층** + 신의 방 (진엔딩)
- [x] **챔버 PNG 작가**: **Pixellab MCP** 생성 (메이드 인 어비스 톤 일관성)
- [x] **세이브 시스템**: **JSON 파일** — `Application.persistentDataPath/save.json`
- [x] **Pause 입력**: **Esc** (Input System 표준)
- [x] **Map 탭 zoom/pan**: **추후** — Phase 4 polish 단계
- [x] **HeightGauge 위치**: **우측 세로** (CONCEPT.md 메모리 사양 그대로)

---

## 9. Aestik과의 차이점 (Tether 차별화)

| 요소 | Aestik | Tether |
|---|---|---|
| 맵 형태 | 수평 다층 | **수직 탑 단면도** |
| 시점 | 1캐릭터 | **듀얼 (1회차/2회차)** |
| 메타 | (없음) | **로그라이트 — 영구 컬렉션** |
| HUD | HP + 통화 | HP + **흡수한 귀 슬롯** + **높이 게이지** |
| 챔버 PNG | 단순 박스 → 디테일 | **그라데이션 색 + 톤 별 디테일** |

---

## 참조

- [CONCEPT.md](../CONCEPT.md) — 전체 게임 컨셉
- 영상: Aestik devlog (Kiss The Button) — YouTube XimxutBn5cE
- 분석 결과: `/watch` 스킬로 추출한 인사이트
