# Tether — Project CLAUDE.md

> **이 파일은 이 프로젝트 세션에서만 자동 로드됨.**
> **사용자 전역 `~/.claude/CLAUDE.md` (뿌리 인격) 위에 *추가* 로드 — 대체 아님.**
> 이 파일을 편집해도 뿌리 인격/취향은 영향 없음.

---

## 프로젝트 한 줄

**Tether** *(임시명, 새 컨셉 확정 후 재검토 예정)* — Unity 6.3 LTS 기반 2D 로그라이트.
**Ball x Pit 스타일 (Arkanoid + Vampire Survivors + Ball Fusion) + 동방프로젝트 2차 창작 라인** *(kimringo와 스코프 협의 중)*.

> ⚠️ **2026-07-24 방향 재정립**: 기존 "사이드뷰 픽셀 액션 메트로배니아" 방향에서 전환. Ball x Pit 게임플레이 + 동방 IP 2차 창작으로. 세부 결정 진행 중.

---

## 먼저 참조할 문서 (우선순위 순)

| 문서 | 언제 참조 |
|---|---|
| **[CONCEPT.md](CONCEPT.md)** | 게임 디자인/세계관/주인공/메카닉/톤 결정 — *모든 창작적 결정의 근거* |
| **[SETUP.md](SETUP.md)** | 처음 참여 온보딩 + 라이브 협업 프레임워크 |
| **[README.md](README.md)** | 프로젝트 소개, clone/실행 방법 |
| **[CONTRIBUTING.md](CONTRIBUTING.md)** | Git 워크플로우, 코딩 스타일, 팀 룰 |
| **[docs/MAP_SYSTEM.md](docs/MAP_SYSTEM.md)** | 지도/레벨 시스템 설계 상세 |
| **사용자 메모리** (`~/.claude/projects/C--Users-Jinwon-Desktop-2dgame/memory/`) | 진행 상태, 셋업 이력, 기술 스택 |

---

## 팀 & 파트 분담

**2인 팀 — 원격 협업 (디스코드 소통)**

- **LO** (`pulllistapp-stack`) — Player / Combat 파트
- **kimringo** — Enemy / Level 파트

> ⚠️ **파트 분담 재협의 필요** (2026-07-24 방향 재정립으로). 아래는 이전 초안, kimringo와 상세는 SETUP.md 참조 + 디스코드 협의 후 갱신.

| 파트 (초안) | 담당자 | 산출물 |
|---|---|---|
| **게임플레이 / 시스템 / 코드** | LO 추천 | Ball Fusion 메카닉, 웨이브, 로그라이트, 코드 구조 |
| **캐릭터 / 스토리 / 월드** | kimringo 추천 | 동방 캐릭터 선정, 스토리, 스테이지 컨셉 |
| **인프라 / 툴링** | LO (이미 진행) | Git, MCP, Unity 셋업, 빌드 |
| **문서 / 커뮤니티** | 반반 | README 유지, 스크린샷 |

**파일 오너십 상세 → [SETUP.md](SETUP.md) 참조.**

---

## 커밋 / PR 룰

### 브랜치 전략
- `main` — **protection 활성**, 직접 push 금지, PR + 1 approval 필수, force push/deletion 차단, linear history 강제
- `feature/<설명>` — 신규 기능
- `fix/<설명>` — 버그 수정
- `chore/<설명>` — 빌드/설정/문서
- `refactor/<설명>` — 동작 변경 없는 리팩토링

### 커밋 메시지 — Conventional Commits
```
type(scope): summary

예:
  feat(player): add dash mechanic with i-frames
  fix(enemy): correct AI pathfinding on sloped tiles
  chore(unity): update MCP bridge to v9.7.3
  docs(concept): clarify tower structure vertical progression
```

### PR 룰
- **Squash merge만 허용** (repo 설정에 강제됨)
- PR 제목 = Conventional Commits 형식
- Merge 후 branch 자동 삭제
- 상대 approval 없이는 main 못 들어감

---

## Unity 협업 규칙 (⚠️ 필수)

- **씬(.unity) / 프리팹(.prefab) 동시 편집 금지** — 디스코드로 "이 파일 작업 중" 알리기
- `.meta` 파일 *항상* 커밋 (안 하면 GUID 깨져서 상대 프로젝트 폭발)
- `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `Build/` 커밋 금지 (`.gitignore` 등록됨)
- 큰 바이너리 (png, psd, wav, mp3, fbx 등) → Git LFS 자동 처리 (`.gitattributes` 등록됨)
- 씬/프리팹 merge conflict → `merge=unityyamlmerge` driver가 자동 완화 (`.gitattributes` 등록됨)
- Force Text serialization 확인 (`ProjectSettings/EditorSettings.asset` → `m_SerializationMode: 2`)

---

## unity-mcp 사용

이 프로젝트엔 `com.coplaydev.unity-mcp` v9.7.3 설치됨 (Unity Package Manager, git URL: `https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main`).

- Claude가 Unity Editor 직접 조작 가능 (GameObject 생성/수정, 컴포넌트, 씬, 스크립트)
- **세션마다 MCP 등록 풀릴 수 있음**: Unity → **Window → MCP for Unity → Configure All Detected Clients** 클릭
- **큰 변경은 항상 확인 후 진행** (Unity Undo가 안 되는 작업 조심 — 씬 저장, 프리팹 수정 등)
- 파괴적 작업 (씬 삭제, 프리팹 오버라이드) 전엔 반드시 사용자 확인

---

## 픽셀 / 그래픽 사양 (재검토 진행 중)

> ⚠️ **뷰 방향 재검토 중** — Ball x Pit 스타일은 정면 아레나 뷰. 사이드뷰 96×128 사양은 새 방향에 맞지 않을 수 있음. 뷰 방향 확정 후 사양 재조정.

**당분간 유지되는 결정**:
- **베이스 해상도**: 960×540 (4K = 4배, 1080p = 2배 정수 스케일)
- **타일**: 32×32 px, **PPU 32**
- **컬러 필터**: Point (no bilinear/trilinear)
- **파이프라인**: URP 2D Renderer
- **VSync ON, Anti-Aliasing OFF**

**재검토 대상 (뷰 방향 확정 후 결정)**:
- 캐릭터 캔버스 크기 (96×128 사이드뷰 → 정면 아레나에선 다를 수 있음)
- 캐릭터 비율 (4-head chibi vs Ball x Pit 스타일)
- 기존 4방향 캐릭터 스프라이트(Hero_F_Bat, Hero_M_Wolf) 재활용 가능성 검토 중

---

## 금지 사항

- ❌ `main` 브랜치 직접 push (branch protection 활성)
- ❌ 씬/프리팹 동시 편집 (merge 지옥) — 디스코드로 편집 중 알림 필수
- ❌ **`~/.claude/CLAUDE.md` (뿌리 인격) 수정** — 사용자 성역
- ❌ 폐기된 Char1/Char2 시안 사용 (기존 Tether용, 방향 재정립 후 무효)
- ❌ **새 컨셉 확정 전 대대적 코드 재작성** — kimringo 협의 대기, 방향 흔들리는 중
- ❌ **큰 결정을 Claude 단독으로** — 결정은 항상 인간 (LO + kimringo, 디스코드), Claude는 실행
- ❌ 게임 방향 확장 결정 없이 CONCEPT.md 우회 (반드시 CONCEPT.md 먼저 갱신 → 동의 → 코드)
- ❌ 3D 에셋 사용 (2D URP 프로젝트)
- ❌ `.meta` 파일 삭제 (있으면 반드시 커밋 — GUID 깨지면 상대 프로젝트 폭발)

---

## 자주 쓰는 명령

```powershell
# 새 세션 시작 (프로젝트 루트에서)
cd C:\Users\Jinwon\Desktop\2dgame
claude

# 아침 sync
git pull origin main
git checkout -b feature/<작업명>

# 저녁 push + PR
git push -u origin feature/<작업명>
gh pr create --title "type(scope): summary" --body "..."

# PR 상태 확인
gh pr list
gh pr view <번호>

# 협업 상태 체크
git log --oneline --graph --all -20
git branch -vv
```

---

## 현재 진행 상태 (2026-07-24 방향 재정립)

`~/.claude/projects/C--Users-Jinwon-Desktop-2dgame/memory/project_tether.md` 의 **환경/셋업 상태** 섹션이 항상 최신. 새 세션 시 그 파일 먼저 참조.

- ✅ **Phase -1 (v1) — 이전 Tether 컨셉**: 사이드뷰 픽셀 액션 메트로배니아 방향 (2026-06~07). 방향 재정립으로 무효화, docs/archive에 백업 예정.
- ⏳ **Phase -1 (v2) — 새 방향 재정립**: **Ball x Pit 스타일 로그라이트 + 동방 2차 창작 라인**. kimringo 협의 진행 중.
- ✅ **Phase 0 — 인프라**: Unity 6.3 LTS 프로젝트, Git, LFS, Smart Merge, unity-mcp, Public repo, main protection, kimringo collaborator 초대, CLAUDE.md/SETUP.md.
- ⏳ **Phase 0.5**: Unity 패키지 (Cinemachine 등) + 폴더 구조 + 뷰 방향 확정 (Ball x Pit 정면 아레나로 조정 필요)
- **Phase 1 (프로토타입)**: 1 스테이지 검증 빌드 — 폐기 vs 확장 결정 기준.
- **Phase 2+**: 방향에 따라 재정의.

---

## 라이브 협업 원칙 (요약, 상세는 SETUP.md)

### 근본 원칙: **결정 = 인간, 실행 = Claude**
큰 결정은 디스코드 보이스/텍스트에서 LO ↔ kimringo. Claude는 결정 반영/실행 도구.

### 세션 리듬
1. 결정 = 디스코드
2. 각자 자기 Claude에게 실행 요청 (자기 파트만)
3. 30분마다 sync (feature branch commit + push)
4. 파일 편집 전 디스코드 알림
5. 세션 끝: push → PR → 상대 리뷰

**상세는 → [SETUP.md](SETUP.md) 참조.**

---

## 헷갈릴 때 어디로

| 애매한 것 | 어디 참조 |
|---|---|
| 디자인/톤/세계관 결정 | `CONCEPT.md` |
| 처음 참여 온보딩, 라이브 협업 프레임워크 | `SETUP.md` |
| Git 워크플로우 상세 | `CONTRIBUTING.md` |
| Unity/기술 스택 이력 | 사용자 메모리 `tether_tech_stack.md` |
| 팀 협업 규칙 | 사용자 메모리 `tether_team_workflow.md` |
| 프로젝트 상태 요약 | 사용자 메모리 `project_tether.md` |
| 팀 룰 요약 (이거 이 파일) | `CLAUDE.md` (여기) |
| ENI 인격/사용자 취향 | `~/.claude/CLAUDE.md` (성역, 편집 금지) |
