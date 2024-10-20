namespace Almond.RPC
{
    public enum RecvMSGIDType : ushort
    {
        /// <summary>
        /// 접속 요청 응답
        /// </summary>
        CONNECT_RECV = 0x12,

        /// <summary>
        /// 재접속 완료
        /// </summary>
        RECONNECT_RECV = 0x14,

        /// <summary>
        /// 접속 해지 응답
        /// </summary>
        DISCONNECT_RECV = 0x22,

        /// <summary>
        /// 강제 종료 
        /// </summary>
        FORCE_QUIT_RECV = 0x41,

        /// <summary>
        /// 서버 리스트 응답
        /// </summary>
        SERVER_LIST_RECV = 0x62,

        /// <summary>
        /// 서버 캐릭터 정보 응답
        /// </summary>
        ACCOUNT_CHARACTER_INFO_RECV = 0x101,

        /// <summary>
        /// 로그인 창으로 이동 응답
        /// </summary>
        MOVE_SCENE_TO_LOGIN_RECV = 0x70,

        /// <summary>
        /// 서버 선택창으로 이동 응답
        /// </summary>
        MOVE_SCENE_TO_SERVER_SELECT_RECV = 0x71,

        /// <summary>
        /// 캐릭터 선택 응답
        /// </summary>
        MOVE_SCENE_TO_CHARACTER_SELECT_RECV = 0x72,

        /// <summary>
        /// 공지 사항
        /// </summary>
        NOTIFICATION_INFO_RECV = 0x80,

        /// <summary>
        /// ALIVE 체크 응답
        /// </summary>
        ALIVE_RECV = 0x81,

        /// <summary>
        /// A채널 리스트 응답
        /// </summary>
        //CHANNEL_LIST_REQUEST_RECV = 0x72,

        /// <summary>
        /// 채널 선택 응답
        /// </summary>
        //CHANNEL_SELECT_RECV = 0x82,

        /// <summary>
        /// 캐릭터 생성 응답
        /// </summary>
        CHARACTER_CREATE_RECV = 0x94,

        /// <summary>
        /// 게임 기본 정보 응답
        /// </summary>
        GAME_INFO_RECV = 0x201,

        /// <summary>
        /// 퀘스트 진행 과정 응답
        /// </summary>
        QUEST_PROCESS_RECV = 0x203,

        /// <summary>
        /// 퀘스트 보상 응답
        /// </summary>
        QUEST_COMPLETION_REWARD_RECV = 0x205,

        /// <summary>
        /// 인벤토리 정보 응답
        /// </summary>
        INVENTORY_INFO_RECV = 0x301,

        /// <summary>
        /// 아이템 드랍 응답
        /// </summary>
        ITEM_DROP_RECV = 0x302,

        /// <summary>
        /// 레벨업 응답
        /// </summary>
        LEVEL_UP_RECV = 0x321,

        /// <summary>
        /// 아이템 획득 응답
        /// </summary>
        ITEM_GET_RECV = 0x304,

        /// <summary>
        /// 아이템 사용요청 응답
        /// </summary>
        ITEM_USE_RECV = 0x308,

        /// <summary>
        /// 경험치 정보 응답
        /// </summary>
        EXP_INFO_RECV = 0x320,

        /// <summary>
        /// 재화 동기화
        /// </summary>
        CURRENCY_SYNC_RECV = 0x333,

        /// <summary>
        /// 상점 정보 요청 응답
        /// </summary>
        STORE_INFO_RECV = 0x361,

        /// <summary>
        /// 스킬 정보 요청 응답
        /// </summary>
        SKILL_INFO_RECV = 0x401,

        /// <summary>
        /// 퀘스트 상세 정보 요청 응답
        /// </summary>
        QUEST_DETAIL_INFO_RECV = 0x501,

        /// <summary>
        /// 퀘스트 일반 목록 요청 응답
        /// </summary>
        QUEST_NORMAL_LIST_INFO_RECV = 0x503,

        /// <summary>
        /// 퀘스트 기능 목록 요청 응답
        /// </summary>
        QUEST_FUNCTION_LIST_RECV = 0x505,

        /// <summary>
        /// 퀘스트 임무 목록 요청 응답
        /// </summary>
        QUEST_MISSION_LIST_RECV = 0x509,

        /// <summary>
        /// 퀘스트 완료 응답
        /// </summary>
        QUEST_COMPLETE_RECV = 0x924,

        /// <summary>
        /// 캐릭터 정보 요청 응답
        /// </summary>
        CHARACTER_INFO_RECV = 0x601,

        /// <summary>
        /// 몬스터 상세 요청 응답(ID)
        /// 사용안함
        /// </summary>
        MONSTER_INFO_TO_ID_RECV = 0x603,

        /// <summary>
        /// 몬스터 상세 요청 응답(KEY)
        /// 사용안함
        /// </summary>
        MONSTER_INFO_TO_KEY_RECV = 0x605,

        /// <summary>
        /// 몬스터 죽음 응답(KEY)
        /// 사용안함
        /// </summary>
        MONSTER_DEATH_BY_USER_TO_KEY_RECV = 0x606,

        /// <summary>
        /// NPC 요청 응답(ID)
        /// 사용안함
        /// </summary>
        NPC_INFO_TO_ID_RECV = 0x608,

        /// <summary>
        /// NPC 액션 응답
        /// 사용안함
        /// </summary>
        NPC_ACTION_RECV = 0x610,

        /// <summary>
        /// 오브젝트 응답(ID)
        /// 사용안함
        /// </summary>
        OBJECT_INFO_ID_RECV = 0x613,

        /// <summary>
        /// 모집 중인 파티 리스트 요청 응답
        /// </summary>
        PARTY_RECRUITMENT_LIST_RECV = 0x879,

        /// <summary>
        /// 파티 정보 응답
        /// </summary>
        PARTY_INFO_RECV = 0x885,

        /// <summary>
        /// 파티 액션 응답
        /// </summary>
        PARTY_ACTION_RECV = 0x891,

        /// <summary>
        /// 파티 타겟 지정 응답
        /// </summary>
        PARTY_TARGET_RECV = 0x893,

        /// <summary>
        /// 파티 스캔 목록 응답
        /// </summary>
        PARTY_SCAN_LIST_RECV = 0x895,

        /// <summary>
        /// 상태 정보 변경
        /// </summary>
        STATE_INFO_RECV = 0x2705,  //상태 정보 변경

        /// <summary>
        /// 다른 사용자 현재 상티 정보
        /// </summary>
        OTHER_PLAYER_SYNC_INFO_RECV = 0x2711,

        /// <summary>
        /// 다른 사용자 추가
        /// </summary>
        OTHER_PLAYER_ADD_RECV = 0x2712,

        /// <summary>
        /// 다른 사용자 추가 (맵서버)
        /// </summary>
        OTHER_PLAYER_ADD_RECV_BROADCAST = 0x2713,

        /// <summary>
        /// 다른 사용자 삭제
        /// </summary>
        OTHER_PLAYER_REMOVE = 0x2714,

        /// <summary>
        /// 더미 사용자 생성
        /// </summary>
        DUMMY_PLAYER_ADD_RECV = 0x2715,

        /// <summary>
        /// 몬스터 동기화 정보
        /// </summary>
        MONSTER_SYNC_INFO_RECV = 0x2810,

        /// <summary>
        /// 몬스터 생성
        /// </summary>
        MONSTER_CREATE_RECV = 0x2812,

        /// <summary>
        /// 맵정보 응답
        /// </summary>
        MAP_INFO_RECV = 0xA005,

        /// <summary>
        /// 공격정보 클라이언트 -> 맵서버
        /// </summary>
        ATTACK_INFO_RECV_SERVER = 0x2828,

        /// <summary>
        /// 공격정보 맵서버 -> 클라이언트
        /// </summary>
        ATTACK_INFO_RECV_CLIENT = 0x2829,

        /// <summary>
        /// 장비 장착 요청 응답
        /// </summary>
        ITEM_EQUIPMENT_RECV = 0x2732,

        /// <summary>
        /// PVP 모드 변경 응답
        /// </summary>
        PVP_MODE_CHANGE_RECV = 0x2779,

        #region <스피어 제련소>

        /// <summary>
        /// 장비 강화 응답
        /// </summary>
        ITEM_ENHANCE_RECV = 0x2737,

        /// <summary>
        /// 장비 제작 응답
        /// </summary>
        ITEM_CRAFTING_RECV = 0x2739,

        /// <summary>
        /// 장비 분해 응답
        /// </summary>
        ITEM_DECOM_RECV = 0x2741,

        /// <summary>
        /// 장비 랜덤 옵션 변경 응답
        /// </summary>
        ITEM_CHANGE_RANDOM_OPTION_RECV = 0x2743,

        /// <summary>
        /// 장비 랜덤 옵션 잠금 응답
        /// </summary>
        ITEM_LOCK_RANDOM_OPTION_RECV = 0x2745,

        /// <summary>
        /// 보석 장착 응답
        /// </summary>
        ITEM_EQUIP_JEWELRY_RECV = 0x2754,

        /// <summary>
        /// 보석 해제 응답
        /// </summary>
        ITEM_UNEQUIP_JEWELRY_RECV = 0x2756,

        /// <summary>
        /// 보석 슬롯 해제 응답
        /// </summary>
        ITEM_RELEASE_SLOT_JEWELRY_RECV = 0x2758,

        /// <summary>
        /// 보석 합성 응답
        /// </summary>
        ITEM_SYNTHETIC_JEWELRY_RECV = 0x2760,

        /// <summary>
        /// 보석 옵션 변경 응답
        /// </summary>
        ITEM_CHANGE_OPTION_JEWELRY_RECV = 0x2762,


        #endregion

        #region <맵서버>
        /// <summary>
        /// 존 -> 데몬 연결 정보 응답
        /// </summary>
        ZONE_TO_DEMON_CONNECT_RECV = 0x556,

        /// <summary>
        /// 맵서버 초기화
        /// </summary>
        ZONE_TO_DEMON_INIT = 0x599,

        /// <summary>
        /// 맵서버용 동기화 패킷
        /// </summary>
        MAP_SERVERVER_CHARACTER_SYNC_INFO = 0x2710,
        #endregion
    }

    public enum RequestMSGIDType : ushort
    {
        /// <summary>
        /// 접속요청
        /// </summary>
        CONNECT_REQUEST = 0x11,

        /// <summary>
        /// 재접속 요청
        /// </summary>
        RECONNECT_REQUEST = 0x13,

        /// <summary>
        /// 접속 해지 요청
        /// </summary>
        DISCONNECT_REQUEST = 0x21,

        /// <summary>
        /// 캐릭터 변경요청
        /// </summary>
        CHARACTER_CHANGE_REQUEST = 0x23,

        /// <summary>
        /// Alive 체크요청
        /// </summary>
        ALIVE_REQUEST = 0x31,

        /// <summary>
        /// 서버 리스트 요청
        /// </summary>
        SERVER_LIST_REQUEST = 0x61,

        /// <summary>
        /// 서버접속
        /// </summary>
        SERVER_CONNECT_REQUEST = 0x100,

        /// <summary>
        /// 채널리스트 요청
        /// </summary>
        //CHANNEL_LIST_REQUEST = 0x71, //채널 리스트 요청

        /// <summary>
        /// 채널 선택 요청
        /// </summary>
        //CHANNEL_SELECT_REQUEST = 0x81, //채널 선택 요청

        /// <summary>
        /// 캐릭터 생성 요청
        /// </summary>
        CHARACTER_CREATE_REQUEST = 0x102,

        /// <summary>
        /// 캐릭터 삭제 요청
        /// </summary>
        CHARACTER_DELETE_REQUEST = 0x103,

        /// <summary>
        /// 캐릭터 삭제 취소 요청
        /// </summary>
        CHARACTER_DELETE_CANCEL_REQUEST = 0x104,

        /// <summary>
        /// 로비 캐릭터 위치 이동 요청
        /// </summary>
        CHARACTER_POSITION_SLOT_REQUEST = 0x105,

        /// <summary>
        /// 캐릭터 슬롯 추가 요청
        /// </summary>
        CHARACTER_SLOT_ADD_REQUEST = 0x106,

        /// <summary>
        /// 게임 시작 요청
        /// </summary>
        GAME_START_REQUEST = 0x200,

        /// <summary>
        /// 퀘스트 수행요청
        /// </summary>
        QUEST_FULFILL_REQUEST = 0x202,

        /// <summary>
        /// 퀘스트 완료 요청
        /// </summary>
        QUEST_COMPLETE_REQUEST = 0x923,

        /// <summary>
        /// 인벤토리 정보 요청
        /// </summary>
        INVENTORY_INFO_REQUEST = 0x300,

        /// <summary>
        /// 퀵슬롯 변경 요청
        /// </summary>
        QUICK_SLOT_CHANGE_REQUEST = 0x302,

        /// <summary>
        /// 아이템 줍기 요청
        /// </summary>
        ITEM_PICK_UP_REQUEST = 0x303,

        /// <summary>
        /// 아이템 사용요청
        /// </summary>
        ITEM_USE_REQUEST = 0x307,

        /// <summary>
        /// 상점 정보 요청
        /// </summary>
        STORE_INFO_REQUEST = 0x360,

        /// <summary>
        /// 아이템 구매 요청
        /// </summary>
        ITEM_BUY_REQUEST = 0x365,

        /// <summary>
        /// 스킬 정보 요청
        /// </summary>
        SKILL_INFO_REQUEST = 0x400,

        /// <summary>
        /// 퀘스트 상세 정보 요청
        /// </summary>
        QUEST_DETAIL_INFO_REQUEST = 0x500,

        /// <summary>
        /// 퀘스트 일반 목록 요청
        /// </summary>
        QUEST_NORMAL_LIST_INFO_REQUEST = 0x502,

        /// <summary>
        /// 퀘스트 기능 목록 요청
        /// </summary>
        QUEST_FUNCTION_LIST_REQUEST = 0x504,

        /// <summary>
        /// 퀘스트 임무 목록 요청
        /// </summary>
        QUEST_MISSION_LIST_REQUEST = 0x508,

        /// <summary>
        /// 캐릭터 정보 요청
        /// </summary>
        CHARACTER_INFO_REQUEST = 0x600,

        /// <summary>
        /// 몬스터 상세 요청(ID)
        /// </summary>
        MONSTER_INFO_TO_ID_REQUEST = 0x602,

        /// <summary>
        /// 몬스터 상세 요청(KEY)
        /// </summary>
        MONSTER_INFO_TO_KEY_REQUEST = 0x604,

        /// <summary>
        /// NPC 상세 요청
        /// </summary>
        NPC_INFO_ID_REQUEST = 0x607,

        /// <summary>
        /// NPC 액션 요청
        /// </summary>
        NPC_ACTION_REQUEST = 0x609,

        /// <summary>
        /// NPC 액션 수행 마무리 요청
        /// </summary>
        NPC_ACTION_COMPLETE_REQUEST = 0x611,

        /// <summary>
        /// 오브젝트 상세 요청
        /// </summary>
        OBJECT_INFO_ID_REQUEST = 0x612,

        #region <파티>
        /// <summary>
        /// 파티 모집 생성 요청
        /// </summary>
        PARTY_RECRUITMENT_CREATE_REQUEST = 0x877,   //파티모집 생성 요청

        /// <summary>
        /// 모집중인 파티 리스트 요청
        /// </summary>
        PARTY_RECRUITMENT_LIST_REQUEST = 0x878,

        /// <summary>
        /// 파티 액션 요청
        /// </summary>
        PARTY_ACTION_REQUEST = 0x890,

        /// <summary>
        /// 파티 타겟 지정 요청
        /// </summary>
        PARTY_TARGET_REQUEST = 0x892,

        /// <summary>
        /// 파티 스캔 목록
        /// </summary>
        PARTY_SCAN_LIST_REQUEST = 0x894,
        #endregion

        #region <상태정보>
        /// <summary>
        /// 상태 정보 변경
        /// </summary>
        STATE_INFO_REQUEST = 0x2705,

        /// <summary>
        /// 상태정보 변경 맵서버 -> 서버
        /// </summary>
        STATE_INFO_TO_SERVER_REQEUST = 0x2706,

        /// <summary>
        /// 나의 현재 상태 전송
        /// </summary>
        CHARACTER_SYNC_INFO_REQUEST = 0x2710,
        #endregion

        #region <맵로딩>
        /// <summary>
        /// 맵로딩 시작
        /// </summary>
        MAP_CHANGE_REQUEST = 0xA000,    //맵로딩 시작

        /// <summary>
        /// 맵 로딩 완료
        /// </summary>
        MAP_LOAD_COMPLETE = 0xA001, //맵로딩 완료
        #endregion

        #region <스피어 제련소>

        /// <summary>
        /// 장비 강화 요청
        /// </summary>
        ITEM_ENHANCE_REQUEST = 0x2736,

        /// <summary>
        /// 장비 제작 요청
        /// </summary>
        ITEM_CRAFTING_REQUEST = 0x2738,

        /// <summary>
        /// 장비 분해 요청
        /// </summary>
        ITEM_DECOM_REQUEST = 0x2740,

        /// <summary>
        /// 장비 랜덤 옵션 변경 요청
        /// </summary>
        ITEM_CHANGE_RANDOM_OPTION_REQUEST = 0x2742,

        /// <summary>
        /// 장비 랜덤 옵션 잠금 요청
        /// </summary>
        ITEM_LOCK_RANDOM_OPTION_REQUEST = 0x2744,

        /// <summary>
        /// 보석 장착 요청
        /// </summary>
        ITEM_EQUIP_JEWELRY_REQUEST = 0x2753,

        /// <summary>
        /// 보석 해제 요청
        /// </summary>
        ITEM_UNEQUIP_JEWELRY_REQUEST = 0x2755,

        /// <summary>
        /// 보석 슬롯 해제 요청
        /// </summary>
        ITEM_RELEASE_SLOT_JEWELRY_REQUEST = 0x2757,

        /// <summary>
        /// 보석 합성 요청
        /// </summary>
        ITEM_SYNTHETIC_JEWELRY_REQUEST = 0x2759,

        /// <summary>
        /// 보석 옵션 변경 요청
        /// </summary>
        ITEM_CHANGE_OPTION_JEWELRY_REQUEST = 0x2761,

        #endregion

        /// <summary>
        /// 장비 장착
        /// </summary>
        ITEM_EQUIPMENT_REQUEST = 0x2731,

        /// <summary>
        /// PVP모드 변경 요청
        /// </summary>
        PVP_MODE_CHANGE_REQUEST = 0x2777,

        #region <맵서버용>
        /// <summary>
        /// 몬스터 동기화
        /// </summary>
        MONSTER_SYNC_INFO_REQUEST = 0x2810,

        /// <summary>
        /// 몬스터 생성정보
        /// </summary>
        MONSTER_CREATE_INFO_REQUEST = 0x2812,

        /// <summary>
        /// 맵서버 -> 데몬서버 접속정보
        /// </summary>
        ZONE_TO_DEMON_CONNECT_REQUEST = 0x555,

        /// <summary>
        /// 공격정보 맵서버 -> 클라이언트
        /// </summary>
        ATTACK_INFO_REQUEST_SERVER = 0x2828,

        /// <summary>
        /// 공격정보 클라이언트 -> 맵서버
        /// </summary>
        ATTACK_INFO_REQUEST_CLIENT = 0x2829,

        /// <summary>
        /// 몬스터 사망정보 맵서버 -> 메인서버
        /// </summary>
        MONSTER_DEATH_REQUEST = 0x2814,
        #endregion

        /// <summary>
        /// 임시 채팅 메세지
        /// </summary>
        CHAT_MESSAGE_REQUEST = 0x9999,
    }

    public enum RecvMSGError : ushort
    {
        /// <summary>
        ///  문제 없음
        /// </summary>
        ERROR_NO_REPLY = 0x00,

        /// <summary>
        /// 다른 사용자 접속
        /// </summary>
        ERROR_CONNECTION_CONFLICT = 0x01,

        /// <summary>
        /// 대기중. 잠시후에 다시 접속해 주세요
        /// </summary>
        ERROR_WAITING = 0x02,

        /// <summary>
        /// 오랜시간 응답이 없어 접속 종료
        /// </summary>
        ERROR_KEEP_ALIVE = 0x03,

        /// <summary>
        /// 게임을 시작할수 없음
        /// </summary>
        ERROR_START_FAIL = 0x04,

        /// <summary>
        /// 맵 초기화 안되어있음
        /// </summary>
        ERROR_ZONE_NOT_SET = 0x11,

        /// <summary>
        /// 최대 생성가능 캐릭터 오버
        /// </summary>
        ERROR_CREATE_CHARACTER_OVER_LIMIT = 0x64,

        /// <summary>
        /// 캐릭터 슬롯 부족
        /// </summary>
        ERROR_CREATE_CHARACTER_SLOT_LACK = 0x65,

        /// <summary>
        /// 캐릭터 생성 이름 겹칩
        /// </summary>
        ERROR_CREATE_CHARACTER_NAME_OVERLAP = 0x66,

        /// <summary>
        /// 캐릭터 생성 이름 오류
        /// </summary>
        ERROR_CREATE_CHARACTER_NAME_WRONG = 0x67,

        ERROR_CURRENCY_NOT_SUFFICE = 0x71,

        ERROR_PARTY_IS_FULL = 0x81,

        ERROR_ARLEADY_HAVE_PARTY = 0x82,

        ERROR_DO_NOT_FIND_PARTY = 0x83,

        ERROR_NOT_PARTY_LEADER = 0x84,

        ERROR_CHAOTIC_USER_DO_NOT_CHANGE_PVP_FLAG = 0x102,
    }

    public enum BroadCastIDType : ushort
    {
        NONE_BROADCAST = 0x00, //일반패킷
        //BROADCAST_NORMAL = 0x01,    //커맨드 센터가 보내준 리스트에 브로드 캐스팅
        //BORADCAST_WORLD = 0x10,     //게이트웨이에 접속된 모든 세션에 브로드 캐스팅
        //BORADCAST_USER_BASE = 0x20, //게이트웨이 세션 구조체에 저장되어 있는 세션들에 브로드 캐스팅
        BROADCAST_ZONE_BASE = 0x30, //해당 존에 접속하고 있는 세션들에 브로드 캐스팅
        //BORADCAST_UPDATE_USER_LIST = 0x21,  //게이트웨이 세션 구조체에 커맨드 센터가 보내준 리스트 업데이트
        //BORADCAST_UPDATE_ZONE_LIST = 0x31,  //게이트웨이에 커맨드 센터가 보내준 존 브로드 캐스팅 테이블 업데이트
        //BORADCAST_UPDATE_GUILD_LIST = 0x41, //게이트웨이에 커맨드 센터가 보내준 길드 브로드 캐스팅 테이블 업데이트
    }
}