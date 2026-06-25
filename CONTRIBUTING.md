# Contributing to Tether

2인 팀이지만, 처음부터 제대로 잡고 가자. 나중에 손 안 다침.

---

## 브랜치 전략

- **`main`** — 항상 동작하는 상태. 직접 push 금지 (팀원 합류 후 protection ON).
- **`feature/<짧은-설명>`** — 신규 기능 (예: `feature/player-controller`)
- **`fix/<짧은-설명>`** — 버그 수정 (예: `fix/jump-buffer-timing`)
- **`chore/<짧은-설명>`** — 빌드/문서/설정 (예: `chore/update-packages`)
- **`refactor/<짧은-설명>`** — 동작 변화 없는 리팩토링

브랜치는 작게, 머지는 자주. 한 PR은 한 가지 일만.

---

## 커밋 컨벤션 (Conventional Commits)

```
<type>(<scope>): <한 줄 요약, 명령형>

<선택: 본문 — 무엇/왜>

<선택: Refs / Closes #이슈>
```

**type**: `feat`, `fix`, `chore`, `refactor`, `docs`, `style`, `test`, `perf`, `build`

예시:
```
feat(player): add coyote time and jump buffer

- 6 프레임 coyote time
- 4 프레임 jump buffer
- Celeste 라인 참고

Closes #12
```

---

## PR 룰

1. **PR 제목**: 커밋 메시지 형식과 동일
2. **본문**: 무엇/왜/스크린샷 또는 GIF (UI/시각적 변경 시 필수)
3. **체크리스트** (PR 템플릿 사용):
   - [ ] 로컬에서 Play Mode 정상 동작 확인
   - [ ] Console 에러/경고 없음
   - [ ] `.meta` 파일 누락 없음 (Unity 자동 생성)
   - [ ] 씬/prefab 동시 작업 충돌 검토함
4. **머지**: Squash merge (커밋 한 개로 통합). main 히스토리 깔끔하게 유지.

팀원 합류 후 `main` 보호 (1 review 필수, force-push 금지) 활성화.

---

## Unity 협업 룰 (중요)

### 씬/prefab 동시작업 금지

씬(`.unity`)과 prefab(`.prefab`)은 머지 도구를 거쳐도 충돌 가능성 높음. **같은 씬/prefab을 두 명이 동시에 편집하지 말 것.**

작업 전 채팅(Discord/Slack)에:
> *"Scene_Forest_01 작업 중"* — 끝나면 *"끝났음, 머지 가능"*

### 씬/prefab 머지 충돌이 발생하면

`.gitattributes`에 등록된 Unity Smart Merge(`UnityYAMLMerge`)가 자동으로 처리. 그래도 실패 시:

```bash
# Unity Editor에서 충돌 파일 열어서 수동 해결
# 또는 한쪽 버전 선택:
git checkout --ours <파일경로>     # 내 변경 유지
git checkout --theirs <파일경로>   # 상대 변경 채택
```

### `.meta` 파일은 *반드시* 커밋

Unity는 모든 에셋에 `.meta`를 생성. 누락되면 다른 팀원 머신에서 GUID가 새로 생겨 *연결 깨짐*. `.gitignore`로 절대 막지 않도록 주의 (이미 설정됨).

### Library/, Temp/, Logs/, UserSettings/는 커밋 금지

`.gitignore`에 등록됨. 만약 잘못 들어오면 즉시 PR로 제거.

### LFS 트래킹 확인

새로운 바이너리 (PNG, WAV, FBX, Aseprite 등) 추가 시 LFS로 들어가는지 확인:

```bash
git lfs ls-files | grep <파일명>
```

`.gitattributes`에 패턴 미리 등록되어 있어 보통 자동.

---

## 코딩 스타일 (C#)

- **클래스/메서드**: `PascalCase`
- **로컬 변수/매개변수**: `camelCase`
- **상수**: `UPPER_SNAKE_CASE`
- **private 필드**: `_camelCase` (언더스코어 프리픽스)
- **public 필드 금지** — `[SerializeField] private`로 인스펙터 노출
- **들여쓰기**: 4 spaces, LF 줄바꿈
- **Allman 스타일** (중괄호 새 줄) — Unity 표준

```csharp
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
}
```

---

## 폴더/파일 네이밍

- **C# 스크립트**: `PascalCase.cs` (`PlayerController.cs`)
- **Prefab**: `PascalCase.prefab` (`Player.prefab`, `Enemy_Wolf.prefab`)
- **씬**: `Scene_<영역>_<번호>.unity` (`Scene_Forest_01.unity`)
- **스프라이트**: `<카테고리>_<이름>_<상태>.png` (`Player_Idle_01.png`)
- **애니메이션**: `<대상>_<동작>.anim` (`Player_Run.anim`)
- **ScriptableObject**: `SO_<이름>.asset` (`SO_PlayerStats.asset`)

---

## 픽셀 사양 (고정 — 절대 변경 금지)

- 960×540 베이스, PPU 32, 32×32 타일, 96×128 캐릭터
- Texture Filter Mode: **Point**, Compression: **None** (또는 RLE)
- Sprite Pixels Per Unit = **32**

자세한 건 [README.md](README.md#픽셀-사양-고정) 참조.

---

## 질문 / 막힘

채팅에 바로. 작은 의문도 막힘이 되기 전에. 우리는 둘뿐이니까.
