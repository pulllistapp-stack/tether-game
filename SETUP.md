# Tether — 팀 온보딩 & 라이브 협업 가이드

> **이 문서의 두 가지 목적**:
> 1. **Part 1** — 프로젝트에 처음 참여하는 팀원의 환경 셋업 (kimringo 등)
> 2. **Part 2** — LO와 kimringo의 실시간 원격 협업 프레임워크

---

# 📌 Part 1 — 처음 참여하는 팀원 온보딩

## 1. Prerequisites (필요 소프트웨어)

| 소프트웨어 | 버전 | 용도 |
|---|---|---|
| **Git for Windows** | 2.43+ | 버전 관리 |
| **Git LFS** | 3.4+ | 큰 바이너리 (png, psd, fbx 등) |
| **GitHub CLI (`gh`)** | 2.90+ | PR 생성/리뷰 (선택이지만 강력 권장) |
| **Unity Hub** | 최신 | Unity 에디터 관리 |
| **Unity 6.3 LTS** | `6000.3.18f1` | 게임 엔진. 모듈 필수: **Windows Build Support (IL2CPP)** + **Documentation** |
| **Visual Studio 2026 Community** | 최신 | C# 코딩. 워크로드: **Unity를 사용한 게임 개발** + **.NET 데스크톱 개발** |
| **uv** | 0.11+ | Python 패키지 매니저 (Claude Code용) |
| **Claude Code** | 최신 | Anthropic CLI |
| **디스코드** | 최신 | 팀 소통 (라이브 협업 필수) |

---

## 2. Clone (LFS 활성화 후!)

```bash
# LFS 먼저 활성화 (안 하면 큰 바이너리가 stub 파일로 받아짐)
git lfs install

# Clone
git clone https://github.com/pulllistapp-stack/tether-game.git
cd tether-game
```

Repo 로컬 config (본인 GitHub 계정 기준):
```bash
git config user.name "kimringo"
git config user.email "본인@이메일.주소"
```

---

## 3. Unity 프로젝트 첫 실행

1. **Unity Hub** → 좌측 **Projects** → 우상단 **Open** → `tether-game/Tether/` 폴더 선택
2. Unity 6.3 LTS 6000.3.18f1로 자동 열림 (5~10분 임포트 대기)
3. `Assets/Scenes/SampleScene.unity` 씬 더블클릭해서 열기 → 정상 로드 확인

---

## 4. Git Smart Merge 등록 (Unity 씬/프리팹 conflict 자동 완화)

Windows PowerShell (**본인 Unity 설치 경로 기준**):
```powershell
# 본인 Unity 설치 경로 확인 후 수정 필요
$unityYaml = "C:\GAME\CC\6000.3.18f1\Editor\Data\Tools\UnityYAMLMerge.exe"

# 위 경로에 UnityYAMLMerge.exe 실제 존재 확인
Test-Path $unityYaml   # True 나와야 함

# 로컬 git config에 merge driver 등록
git config merge.unityyamlmerge.name "Unity SmartMerge"
git config merge.unityyamlmerge.driver "`"$unityYaml`" merge -p `"%O`" `"%B`" `"%A`" `"%A`""
git config merge.unityyamlmerge.recursive binary
```

`.gitattributes`에 이미 매핑 규칙 있으니 위 명령으로 driver만 등록하면 씬/프리팹 conflict 자동 완화.

---

## 5. Claude Code + unity-mcp Configure

1. **Unity Editor 열려있는 상태**에서 상단 메뉴 → **Window → MCP for Unity → Configure All Detected Clients** 클릭
2. Bridge가 시스템 내 Claude Code 감지 → 자동 등록
3. **새 Claude Code 세션 시작** (기존 세션엔 반영 안 됨)
4. 새 세션에서 unity-mcp 도구 로드 확인 (씬에 큐브 만들기 등 테스트)

---

## 6. Claude에게 첫 프롬프트

Claude Code 처음 열면 프로젝트 컨텍스트 zero. 아래 프롬프트로 파악부터:

```
안녕 Claude. 나는 kimringo, Tether 프로젝트에 방금 참여한 팀원이야.

프로젝트 배경:
- Unity 2D 게임 (URP 2D, 6.3 LTS)
- 방향 (2026-07-24 재정립): "Ball x Pit 스타일 로그라이트 + 동방프로젝트 2차 창작 라인"
- 팀 원격 협업 (나비욧드(`pulllistapp-stack`) + 나(kimringo), 디스코드 소통)
- Repo: https://github.com/pulllistapp-stack/tether-game (Public)

지금 파악해야 할 것:
1. CLAUDE.md 읽고 프로젝트 룰 이해
2. CONCEPT.md 읽고 현재 방향 파악
3. SETUP.md 읽고 협업 프레임워크 파악
4. README.md, CONTRIBUTING.md 훑기
5. docs/ 폴더 참고 문서 확인
6. git log --oneline -30 (최근 이력)
7. 사용자 메모리 확인

파악 끝나면:
- 현재 프로젝트 상태 3~5줄 요약
- Phase 진행도 (0.5 어디쯤?)
- 내가 지금 시작할 만한 작업 후보 2~3개 제안

⚠️ 아직 코드/파일 수정하지 마. 파악만.
```

---

## 7. 첫 리허설 (미니 PR로 워크플로우 확인)

셋업 완료 확인용 미니 워크플로우:

```bash
# 새 feature branch
git checkout -b chore/kimringo-setup-check

# README 아래에 자기 소개 한 줄 추가 (예시)
# ... 편집 ...

# 커밋
git add README.md
git commit -m "chore: add kimringo to team"

# 푸시
git push -u origin chore/kimringo-setup-check

# PR 생성
gh pr create --title "chore: add kimringo to team" \
             --body "First setup PR to verify workflow."
```

→ LO에게 디스코드로 리뷰 요청 → LO가 approve + merge → 워크플로우 완주.

이 리허설이 성공하면 실전 준비 완료.

---

# 🎬 Part 2 — 라이브 협업 프레임워크

## 근본 원칙: **결정 = 인간, 실행 = Claude**

Claude는 결정 파트너가 아니라 *실행 파트너*.
두 사람 + 두 Claude 환경에선 결정을 사람 사이에서 먼저 굳혀야 실행이 안 겹침.

```
┌──────────────────────────────────────────────┐
│  1. 디스코드 (결정 채널)                       │
│     ├─ 큰 방향 논의                            │
│     ├─ 파일 오너십 확인                        │
│     └─ 결정 텍스트로 남김 (#dev-decisions)     │
│                    ↓                         │
│  2. 각자 Claude에게 실행 요청 (자기 파트만)     │
│     ├─ 나비욧드 Claude: 나비욧드 파트 문서/코드 작업 │
│     └─ kimringo Claude: kimringo 파트 작업     │
│                    ↓                         │
│  3. Git commit + push (자기 브랜치)            │
│     └─ 30분마다 sync                          │
│                    ↓                         │
│  4. PR + 상대 리뷰 → merge                     │
└──────────────────────────────────────────────┘
```

---

## 디스코드 채널 구조 (권장)

```
📁 Tether 서버
├── 📢 dev-log        # Git commit/PR 자동 봇 알림 (GitHub webhook)
├── ❓ dev-questions  # 실시간 질문/막힘
├── 📌 dev-decisions  # 큰 결정 아카이브
├── 🎨 art-feedback   # 스크린샷/영상 피드백
├── 🐛 bugs           # 발견한 버그 로그
└── 🎙️ dev-voice     # 페어 세션 (스크린쉐어)
```

**GitHub → Discord webhook**: repo Settings → Webhooks → 디스코드 webhook URL 붙임 → 커밋/PR 자동 알림.

---

## 라이브 기획 세션 방법 (같이 문서 잡을 때)

1. **디스코드 보이스 통화** — 큰 결정 논의
2. **한 사람이 화면 공유** — 문서 초안 보면서 실시간 편집
3. **문서 편집 = 한 사람만** (또는 명확한 섹션 분담)
4. **결정 굳으면 텍스트로 요약** — `#dev-decisions`에 붙이기
5. **각자 Claude에게 반영 요청** — "회의에서 X, Y, Z 결정했어. 반영해줘"

---

## 역할분담 (아직 초안, kimringo 협의 후 확정)

### 콘텐츠 축

| 파트 | 담당 | 산출물 |
|---|---|---|
| 게임플레이/시스템/코드 | 나비욧드 추천 | Ball Fusion 메카닉, 웨이브, 로그라이트, 코드 구조 |
| 캐릭터/스토리/월드 | kimringo 추천 | 동방 캐릭터 선정, 스토리, 스테이지, 비주얼 |
| 인프라/툴링 | 나비욧드 (이미 진행) | Git, MCP, Unity 셋업, 빌드 |
| 문서/커뮤니티 | 반반 | README, 스크린샷 정리 |

### 파일 오너십 축 (예시, 새 방향 확정 후 재조정)

| 폴더/파일 | 오너 | 규칙 |
|---|---|---|
| `Assets/Scripts/Gameplay/` | 나비욧드 | 시스템 코드 |
| `Assets/Scripts/UI/` | 나비욧드 우선, kimringo 협조 | UI 로직 |
| `Assets/Prefabs/Enemies/`, `Levels/` | kimringo | 콘텐츠 배치 |
| `Assets/Sprites/Characters/` | kimringo | 캐릭터 아트/배치 |
| `Assets/Scenes/Levels/` | kimringo | 스테이지 씬 |
| `CONCEPT.md`, `docs/*.md` | **둘 다** (섹션 분담) | 편집 시 디스코드 알림 필수 |
| `CLAUDE.md`, `README.md`, `CONTRIBUTING.md`, `SETUP.md` | 반반, PR로 | 룰 문서 |

---

## Sync 리듬

| 시점 | 액션 |
|---|---|
| **30분마다** | `git add . && git commit -m "wip: ..." && git push` (자기 브랜치) |
| **큰 결정 즉시** | 디스코드 요약 + 상대에게 pull 알림 |
| **파일 편집 전** | 디스코드에 "이 파일 만지는 중" 알림 |
| **세션 끝** | 자기 브랜치 push → PR 생성 → 상대 리뷰 요청 |

---

## Merge Conflict 대응

Unity Smart Merge가 씬/프리팹 conflict를 자동 완화. 실패 시:

1. **디스코드로 상대에게 알림** — 누가 어느 부분 만졌는지
2. `git status` + `git diff --name-only --diff-filter=U` — conflict 파일 확인
3. **씬/프리팹 conflict**:
   - Smart Merge 수동 실행: `git checkout --conflict=merge <file>` 후 3-way tool
   - 또는 Unity Editor에서 상대 변경 반영해 재작업
4. **코드 conflict**:
   - Visual Studio 2026 또는 VS Code의 3-way merge editor
5. 최후: 한 쪽 버전 선택 (`git checkout --theirs <file>` 또는 `--ours <file>`) — 잘못 결정하면 상대 작업 사라짐, 신중히

---

## ⚠️ 자주 하는 실수 & 대응

| 실수 | 결과 | 대응 |
|---|---|---|
| `.meta` 파일 안 커밋 | 상대 프로젝트 GUID 깨져 폭발 | `git status` 확인 후 반드시 커밋 |
| 씬 동시 편집 | 심한 merge conflict | 디스코드로 편집 중 알림 |
| `main` 직접 push 시도 | Branch protection이 막음 | 항상 feature branch |
| LFS 미설치로 clone | 큰 바이너리 stub 파일만 받음 | `git lfs install` 후 다시 clone |
| 한글 폴더/파일 이름 | Git 인코딩 이슈 가능 | 가능한 영어로 |
| Claude가 단독 결정 | 상대 Claude와 어긋남 | 큰 결정은 항상 디스코드에서 사람이 먼저 |

---

# 📚 참고 링크

- [CLAUDE.md](CLAUDE.md) — 프로젝트 룰 (Claude 자동 참조)
- [CONCEPT.md](CONCEPT.md) — 게임 컨셉
- [README.md](README.md) — 프로젝트 소개
- [CONTRIBUTING.md](CONTRIBUTING.md) — 기여 가이드
- [docs/archive/MAP_SYSTEM_tether_v1.md](docs/archive/MAP_SYSTEM_tether_v1.md) — 구 방향 지도/레벨 시스템 설계 (archived)

---

# 📝 문서 갱신 이력

- **2026-07-24**: 초안 작성 (Tether 방향 재정립 + kimringo 온보딩 준비)
