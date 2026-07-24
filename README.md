# Tether

> ⚠️ **방향 재정립 중 (2026-07-24~)**: 아래 내용은 구 방향(사이드뷰 메트로배니아). 새 방향은 **Ball x Pit 스타일 로그라이트 + 동방프로젝트 2차 창작 라인**. 최신 상태는 [CLAUDE.md](CLAUDE.md) / [SETUP.md](SETUP.md) 참조. 이 문서는 kimringo 협의 후 재작성 예정.

> *"두 사람이 정상의 신에게 묻고 싶은 게 있어. 둘 중 하나만 잃어도 다 끝나."*

**Tether** — 2D 사이드뷰 메트로배니아 로그라이트. 수직 등반 + 보스 능력 흡수.

거대한 탑을 올라가는 연인. 한 명이 먼저 가고, 다른 한 명은 같은 길을 다른 발자국으로 따라간다. 보스를 쓰러뜨릴 때마다 그의 *귀*를 얻어 짐승의 능력을 빌려 쓴다. 정상에는 신이 있다 — 그리고 둘 중 하나라도 잃으면, 모든 게 끝난다.

레퍼런스: **Hollow Knight + Dead Cells + Mega Man + Tower of God + Made in Abyss**.

상세 컨셉: [CONCEPT.md](CONCEPT.md)

---

## 기술 스택

| 영역 | 도구 |
|---|---|
| 엔진 | **Unity 6.3 LTS** (6000.3.18f1), Universal 2D 템플릿 |
| 렌더링 | URP 17.3.0, 2D Renderer |
| 입력 | Input System 1.19.0 (신형) |
| 카메라 | Cinemachine + 2D Pixel Perfect Camera |
| 픽셀아트 | Aseprite (소스), pixellab MCP (AI 보조) |
| IDE | Visual Studio 2026 (Unity 워크로드) |
| 버전 관리 | Git + Git LFS, Unity Smart Merge |
| MCP | unity-mcp (CoplayDev), pixellab |

### 픽셀 사양 (고정)

- **베이스 해상도**: 960×540 (1080p = 2x, 4K = 4x 정수 스케일)
- **캐릭터 캔버스**: 96×128 px (4-head chibi 비율)
- **타일**: 32×32 px
- **PPU**: 32

이 사양은 *변경하지 않는다*. 변경 시 모든 에셋 재작업 필요.

---

## 셋업 (신규 개발자)

### 1. 사전 설치

- **Git** ≥ 2.43
- **Git LFS** ≥ 3.4 — `git lfs install` 1회 실행
- **Unity Hub** + **Unity 6.3 LTS (6000.3.18f1)**
- **Visual Studio 2026** (Unity 워크로드 + .NET 데스크톱)

### 2. Clone

```bash
git clone https://github.com/pulllistapp-stack/tether-game.git
cd tether-game
git lfs pull
```

### 3. Unity Smart Merge 등록 (1회)

씬/prefab 머지 충돌을 Unity 자체 도구로 자동 해결. **본인 머신의 UnityYAMLMerge.exe 경로**를 확인 후:

```bash
git config --local merge.unityyamlmerge.name "Unity SmartMerge"
git config --local merge.unityyamlmerge.driver "'<UnityYAMLMerge.exe 경로>' merge -p --force --fallback none %O %B %A %A"
git config --local merge.unityyamlmerge.recursive binary
```

Windows 일반 경로: `C:/Program Files/Unity/Hub/Editor/6000.3.18f1/Editor/Data/Tools/UnityYAMLMerge.exe`

### 4. Unity로 프로젝트 열기

Unity Hub → Add → `tether-game/Tether` 폴더 선택 → Open. Library/ 첫 빌드에 5~10분 소요.

### 5. MCP (선택)

Claude Code / Cursor 등 AI 에디터 사용 시 unity-mcp 활성화:

- Unity → Window → MCP for Unity → Configure All Detected Clients

---

## 폴더 구조

```
tether-game/
├── CONCEPT.md              # 게임 컨셉 (단일 source of truth)
├── README.md               # 이 문서
├── CONTRIBUTING.md         # 협업 가이드
├── .gitignore
├── .gitattributes          # LFS 패턴 + Smart Merge 힌트
└── Tether/                 # Unity 프로젝트
    ├── Assets/
    │   ├── Scripts/        # C# 게임 로직
    │   ├── Sprites/        # PNG / Aseprite 임포트
    │   ├── Prefabs/        # 씬 빌딩 블록
    │   ├── Animations/     # AnimationController, anim
    │   ├── Tilemaps/       # 타일 팔레트 + 룰타일
    │   ├── Audio/          # SFX, BGM
    │   ├── ScriptableObjects/  # 데이터 자산
    │   ├── Scenes/         # 게임 씬
    │   ├── Settings/       # URP 등 설정 자산
    │   └── Tests/          # PlayMode + EditMode 테스트
    ├── Packages/
    └── ProjectSettings/
```

---

## 개발 단계

- [x] **Phase -1** — 컨셉 + 기술 스택 확정
- [x] **Phase 0** — Git/LFS/MCP 셋업
- [ ] **Phase 0.5** — 패키지 추가 (Cinemachine, Pixel Perfect), 폴더 구조, 카메라 설정
- [ ] **Phase 1** — 플레이어 컨트롤러 + 타격감 검증 (placeholder)
- [ ] **Phase 2** — 첫 챔버 + 첫 보스 + 흡수 시스템
- [ ] **Phase 3** — 메타 시스템 (로그라이트 루프, 능력 콜렉션)
- [ ] **Phase 4** — 듀얼 캐릭터 시스템 (2회차)

---

## 기여하기

[CONTRIBUTING.md](CONTRIBUTING.md) 참조.

## 라이선스

미정 (개발 중).
