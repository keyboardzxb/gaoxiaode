namespace Arrowgene.O2Jam.Server.Packet
{
    public enum PacketId : ushort
    {
        Unknown = 0,

        // --- 登录、星球、频道 (1xxx) ---
        LoginReq = 1000,
        LoginRes = 1001,
        PlanetReq = 1002,
        PlanetRes = 1003,
        ChannelReq = 1004,
        ChannelRes = 1005,
        RemoteControlReq = 1006, // GM命令请求
        RemoteControlRes = 1007, // GM命令响应

        // --- 注册 (新增) ---
        RegisterReq = 1010, // 注册请求 (选用一个当前未使用的ID)
        RegisterRes = 1011, // 注册响应

        // --- 大厅、房间管理 (2xxx) ---
        CharacterReq = 2000,
        CharacterRes = 2001,
        RoomListReq = 2002,
        RoomListRes = 2003,
        CreateRoomReq = 2004,
        AnnounceRoomRes = 2005,      // 对房间内所有人的广播（如新玩家加入）
        CreateRoomRes = 2006,        // 创建房间的直接响应
        RoomInfoReq = 2007,          // 请求房间详细信息
        RoomInfoRes = 2008,          // 响应房间详细信息
        PlayerLeaveRoomNotify = 2009,// 玩家离开房间通知
        Room1Req = 2010,
        Room1Res = 2011,
        UnkRes = 2026,             // 0x07EA
        LobbyChatReq = 2012,
        LobbyChatRes = 2013,
        LobbyBackButtonReq = 2021,
        LobbyBackButtonRes = 2022,
        RoomSongSelectButton1Req = 2030,
        RoomSongSelectButton1Res = 2031,

        // --- 房间内操作 (3xxx) ---
        RoomChatReq = 3007, // 房间内聊天请求
        RoomChatRes = 3008, // 房间内聊天响应
        RoomBackButtonReq = 3005,
        RoomBackButtonRes = 3006,

        // --- 游戏准备与进行 (4xxx) ---
        RoomSongSelectReq = 4000,
        RoomSongSelectRes = 4001,
        ToggleReadyReq = 4002,      // 玩家准备/取消准备
        ToggleReadyRes = 4003,      // 准备状态变更通知
        RoomColorSelectReq = 4004,
        RoomColorSelectRes = 4005,
        StartGameReq = 4010,
        StartGameRes = 4011,
        GameStartCountdown = 4012,  // 游戏开始倒计时
        NoteStreamData = 4014,      // 核心：音符数据流
        PlayerUpdate = 4015,        // 核心：游戏内玩家状态更新（分数、连击等）
        Resalt1Req = 4016,          // 游戏结算请求1
        Resalt1Res = 4017,          // 游戏结算响应1
        Resalt2Res = 4018,          // 游戏结算响应2
        JamCombo = 4020,            // Jam Combo攻击通知
        InGameBackButtonReq = 4021,
        InGameBackButtonRes = 4022,
        RoomUnknown1Req = 4023,
        RoomUnknown1Res = 4024,
        RoomUnknown2Req = 4025,
        RoomUnknown2Res = 4026,
        MusicListReq = 4030,
        MusicListRes = 4031,
        GameCheck2Req = 4048,
        GameCheck2Res = 4049,
        InGameStartRes = 4050,
        RoomSongSelectButton2Req = 4051,
        RoomSongSelectButton2Res = 4052,
        RoomSongSelectCheckButtonReq = 4053,
        RoomSongSelectCheckButtonRes = 4054,
        GameCheck1Req = 4055,
        GameCheck1Res = 4056,

        // --- 商店与现金 (5xxx) ---
        ShoppingMallReq = 5000,     // 进入商城请求
        ShoppingMallRes = 5001,     // 进入商城响应
        CashReq = 5028,
        CashRes = 5029,

        // --- 系统与其他 (6xxx) ---
        PingReq = 6001,
        PingRes = 6001,
        DisconnectReq = 65520,

        // ...其他测试或未明确ID...
        testReq = 5015,
        testRes = 5016,
        test1Req = 5006,
        test1Res = 5007,
        test2Req = 5000, // 这很可能就是ShoppingMallReq
        test2Res = 5001, // 这很可能就是ShoppingMallRes
        test3Req = 5025,
        test3Res = 5026,
        // --- 新增：根据抓包数据和日志进行的最终修复 ---
        EnterLobbyRequest = 0x2819, // (0x2819) 客户端实际发送的进入大厅请求
        LobbyInfoResponse = 0x1C00,  // (0x1C00) 我们必须回复的大厅信息包

        LobbyPlayerInfoRes = 2014, // 大厅内玩家信息更新/广播
    }
}
