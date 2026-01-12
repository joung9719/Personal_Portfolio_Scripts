# 개인 프로젝트 포트폴리오 스크립트

개인 프로젝트에서 제가 구현한 기능을 정리한 스크립트 입니다

## 담당 기능

-메인 화면 UI(시작/갤러리/종료)

-게임 선택->캐릭터 선택->게임 진입 흐름 구성

-4개의 미니게임 진행/스테이지 관리

-클리어 시 이미지 언락 및 갤러리 저장(Perfect)포함

##게임 구성

-**Qix**:라인으로 영역을 차지하면 커버를 지워 뒷 이미지 공개

-**Block_Out**:벽돌깨기+스테이지 클리어마다 Show씬 커버 제거

-**Ping_pong**:라운드 승리->Show 씬->커버 제거(총 3스테이지)

-**MatchGame(사천성)**:짝 맞추기(2~3층 레이어)+매칭 불가 시 자동 셔플

## 폴더 구조

-"01.Main_Scene_Scripts":매인 화면 (시작/갤러리/종료)

-"02.Loby_Scene_Scriprs":로비/개임 선택/캐릭터 선택/갤러리

-"03.Qix_Scripts":Qix 관련 스크립트

-"04.Block_out_Scripts":Block_Out+Show(커버 제거)관련 스크립트

-"05.Ping_Pong_Scripts":Ping_Pong+Show(커버 제거)관련 스크립트

-"Match_Game_Scripts":MatchGame(사천성)관련 스크립트

## 주요 코드

-Main/Loby/Gallery

 -"GameStart":메인 버튼(시작/갤러리/종료)

 -"LobbyManager":로비->게임 선택 이동

 -"GalleryManager":갤러리 저장/언락(Perfect클리어)

-Qix
  -"QixGameManager":진행/클리어/씬 이동
  
  -"CoverMackInit":커버 마스크 제거/퍼센트 계산

-Block_out

 -"BlockOutGameManager":진행/클리어/스테이지 관리

 -"BlockOutShowManager":Show씬 커버 제거

-Ping_Pong

 -"PingPongGameManager":점수/승패/라이프/씬 이동

 -"EnemyAi":공 추적 기반 적 AI

-MatchGame

 -"MatchGameManager":생성/매칭/레이어/셔플/클리어

 -"Tile":타일 클릭/제거 처리

## 사용 기술

-Unity,C#

 
