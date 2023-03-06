/// <summary>
/// 유저 직업 타입
/// </summary>
public enum Vocation
{
    /// <summary>
    /// 직업이 선택되지 않음
    /// </summary>
    NONE,
    
    /// <summary>
    /// 전사 클래스
    /// </summary>
    KNIGHT,
    
    /// <summary>
    /// 궁수 클래스
    /// </summary>
    ARCHER,
    
    /// <summary>
    /// 법사 클래스
    /// </summary>
    MAGICIAN,
    
    /// <summary>
    /// 몬스터들은 해당 직업을 가진다.
    /// </summary>
    MONSTER,

    ANYTHING,
}

public enum TargetType : byte
{
    NONE = 0,
    MONSTER = 1,
    PLAYER = 2,
    OBJECT = 3,
    NPC = 4,
}

public enum StoryType
{
    NONE,
    PROLOGUE,
    MAINSTORY,
    SUBSTORY,
}