﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Collections;
using System.IO;

namespace EpgTimer
{
    class CommonManager
    {
        public CtrlCmdUtil CtrlCmd
        {
            get;
            set;
        }
        public DBManager DB
        {
            get;
            set;
        }
        public TVTestCtrlClass TVTestCtrl
        {
            get;
            set;
        }
        public System.Diagnostics.Process SrvSettingProcess
        {
            get;
            set;
        }
        public Dictionary<UInt16, ContentKindInfo> ContentKindDictionary
        {
            get;
            set;
        }
        public IEnumerable<ContentKindInfo> ContentKindList
        {
            get
            {
                //「その他」をラストへ。各々大分類を前へ
                return ContentKindDictionary.Values.OrderBy(info => (info.Nibble1 == 0x0F ? 0xF0 : info.Nibble1) << 8 | (info.Nibble2 + 1) & 0xFF);
            }
        }
        public Dictionary<UInt16, string> ComponentKindDictionary
        {
            get;
            set;
        }
        public string[] DayOfWeekArray
        {
            get;
            set;
        }
        public bool NWMode
        {
            get;
            set;
        }
        public List<NotifySrvInfo> NotifyLogList
        {
            get;
            set;
        }
        public NWConnect NW
        {
            get;
            set;
        }
        public List<Brush> CustContentColorList
        {
            get;
            set;
        }
        public SolidColorBrush CustTitle1Color
        {
            get;
            set;
        }
        public SolidColorBrush CustTitle2Color
        {
            get;
            set;
        }
        public List<Brush> CustTimeColorList
        {
            get;
            set;
        }
        public Brush CustServiceColor
        {
            get;
            set;
        }

        private static CommonManager _instance;
        public static CommonManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CommonManager();
                return _instance;
            }
            set { _instance = value; }
        }

        public CommonManager()
        {
            CtrlCmd = new CtrlCmdUtil();
            DB = new DBManager(CtrlCmd);
            TVTestCtrl = new TVTestCtrlClass(CtrlCmd);
            NW = new NWConnect(CtrlCmd);
            {
                ContentKindDictionary = new Dictionary<UInt16, ContentKindInfo>();
                ContentKindDictionary.Add(0x00FF, new ContentKindInfo("ニュース／報道", "", 0x00, 0xFF));
                ContentKindDictionary.Add(0x0000, new ContentKindInfo("ニュース／報道", "定時・総合", 0x00, 0x00));
                ContentKindDictionary.Add(0x0001, new ContentKindInfo("ニュース／報道", "天気", 0x00, 0x01));
                ContentKindDictionary.Add(0x0002, new ContentKindInfo("ニュース／報道", "特集・ドキュメント", 0x00, 0x02));
                ContentKindDictionary.Add(0x0003, new ContentKindInfo("ニュース／報道", "政治・国会", 0x00, 0x03));
                ContentKindDictionary.Add(0x0004, new ContentKindInfo("ニュース／報道", "経済・市況", 0x00, 0x04));
                ContentKindDictionary.Add(0x0005, new ContentKindInfo("ニュース／報道", "海外・国際", 0x00, 0x05));
                ContentKindDictionary.Add(0x0006, new ContentKindInfo("ニュース／報道", "解説", 0x00, 0x06));
                ContentKindDictionary.Add(0x0007, new ContentKindInfo("ニュース／報道", "討論・会談", 0x00, 0x07));
                ContentKindDictionary.Add(0x0008, new ContentKindInfo("ニュース／報道", "報道特番", 0x00, 0x08));
                ContentKindDictionary.Add(0x0009, new ContentKindInfo("ニュース／報道", "ローカル・地域", 0x00, 0x09));
                ContentKindDictionary.Add(0x000A, new ContentKindInfo("ニュース／報道", "交通", 0x00, 0x0A));
                ContentKindDictionary.Add(0x000F, new ContentKindInfo("ニュース／報道", "その他", 0x00, 0x0F));

                ContentKindDictionary.Add(0x01FF, new ContentKindInfo("スポーツ", "", 0x01, 0xFF));
                ContentKindDictionary.Add(0x0100, new ContentKindInfo("スポーツ", "スポーツニュース", 0x01, 0x00));
                ContentKindDictionary.Add(0x0101, new ContentKindInfo("スポーツ", "野球", 0x01, 0x01));
                ContentKindDictionary.Add(0x0102, new ContentKindInfo("スポーツ", "サッカー", 0x01, 0x02));
                ContentKindDictionary.Add(0x0103, new ContentKindInfo("スポーツ", "ゴルフ", 0x01, 0x03));
                ContentKindDictionary.Add(0x0104, new ContentKindInfo("スポーツ", "その他の球技", 0x01, 0x04));
                ContentKindDictionary.Add(0x0105, new ContentKindInfo("スポーツ", "相撲・格闘技", 0x01, 0x05));
                ContentKindDictionary.Add(0x0106, new ContentKindInfo("スポーツ", "オリンピック・国際大会", 0x01, 0x06));
                ContentKindDictionary.Add(0x0107, new ContentKindInfo("スポーツ", "マラソン・陸上・水泳", 0x01, 0x07));
                ContentKindDictionary.Add(0x0108, new ContentKindInfo("スポーツ", "モータースポーツ", 0x01, 0x08));
                ContentKindDictionary.Add(0x0109, new ContentKindInfo("スポーツ", "マリン・ウィンタースポーツ", 0x01, 0x09));
                ContentKindDictionary.Add(0x010A, new ContentKindInfo("スポーツ", "競馬・公営競技", 0x01, 0x0A));
                ContentKindDictionary.Add(0x010F, new ContentKindInfo("スポーツ", "その他", 0x01, 0x0F));

                ContentKindDictionary.Add(0x02FF, new ContentKindInfo("情報／ワイドショー", "", 0x02, 0xFF));
                ContentKindDictionary.Add(0x0200, new ContentKindInfo("情報／ワイドショー", "芸能・ワイドショー", 0x02, 0x00));
                ContentKindDictionary.Add(0x0201, new ContentKindInfo("情報／ワイドショー", "ファッション", 0x02, 0x01));
                ContentKindDictionary.Add(0x0202, new ContentKindInfo("情報／ワイドショー", "暮らし・住まい", 0x02, 0x02));
                ContentKindDictionary.Add(0x0203, new ContentKindInfo("情報／ワイドショー", "健康・医療", 0x02, 0x03));
                ContentKindDictionary.Add(0x0204, new ContentKindInfo("情報／ワイドショー", "ショッピング・通販", 0x02, 0x04));
                ContentKindDictionary.Add(0x0205, new ContentKindInfo("情報／ワイドショー", "グルメ・料理", 0x02, 0x05));
                ContentKindDictionary.Add(0x0206, new ContentKindInfo("情報／ワイドショー", "イベント", 0x02, 0x06));
                ContentKindDictionary.Add(0x0207, new ContentKindInfo("情報／ワイドショー", "番組紹介・お知らせ", 0x02, 0x07));
                ContentKindDictionary.Add(0x020F, new ContentKindInfo("情報／ワイドショー", "その他", 0x02, 0x0F));

                ContentKindDictionary.Add(0x03FF, new ContentKindInfo("ドラマ", "", 0x03, 0xFF));
                ContentKindDictionary.Add(0x0300, new ContentKindInfo("ドラマ", "国内ドラマ", 0x03, 0x00));
                ContentKindDictionary.Add(0x0301, new ContentKindInfo("ドラマ", "海外ドラマ", 0x03, 0x01));
                ContentKindDictionary.Add(0x0302, new ContentKindInfo("ドラマ", "時代劇", 0x03, 0x02));
                ContentKindDictionary.Add(0x030F, new ContentKindInfo("ドラマ", "その他", 0x03, 0x0F));

                ContentKindDictionary.Add(0x04FF, new ContentKindInfo("音楽", "", 0x04, 0xFF));
                ContentKindDictionary.Add(0x0400, new ContentKindInfo("音楽", "国内ロック・ポップス", 0x04, 0x00));
                ContentKindDictionary.Add(0x0401, new ContentKindInfo("音楽", "海外ロック・ポップス", 0x04, 0x01));
                ContentKindDictionary.Add(0x0402, new ContentKindInfo("音楽", "クラシック・オペラ", 0x04, 0x02));
                ContentKindDictionary.Add(0x0403, new ContentKindInfo("音楽", "ジャズ・フュージョン", 0x04, 0x03));
                ContentKindDictionary.Add(0x0404, new ContentKindInfo("音楽", "歌謡曲・演歌", 0x04, 0x04));
                ContentKindDictionary.Add(0x0405, new ContentKindInfo("音楽", "ライブ・コンサート", 0x04, 0x05));
                ContentKindDictionary.Add(0x0406, new ContentKindInfo("音楽", "ランキング・リクエスト", 0x04, 0x06));
                ContentKindDictionary.Add(0x0407, new ContentKindInfo("音楽", "カラオケ・のど自慢", 0x04, 0x07));
                ContentKindDictionary.Add(0x0408, new ContentKindInfo("音楽", "民謡・邦楽", 0x04, 0x08));
                ContentKindDictionary.Add(0x0409, new ContentKindInfo("音楽", "童謡・キッズ", 0x04, 0x09));
                ContentKindDictionary.Add(0x040A, new ContentKindInfo("音楽", "民族音楽・ワールドミュージック", 0x04, 0x0A));
                ContentKindDictionary.Add(0x040F, new ContentKindInfo("音楽", "その他", 0x04, 0x0F));

                ContentKindDictionary.Add(0x05FF, new ContentKindInfo("バラエティ", "", 0x05, 0xFF));
                ContentKindDictionary.Add(0x0500, new ContentKindInfo("バラエティ", "クイズ", 0x05, 0x00));
                ContentKindDictionary.Add(0x0501, new ContentKindInfo("バラエティ", "ゲーム", 0x05, 0x01));
                ContentKindDictionary.Add(0x0502, new ContentKindInfo("バラエティ", "トークバラエティ", 0x05, 0x02));
                ContentKindDictionary.Add(0x0503, new ContentKindInfo("バラエティ", "お笑い・コメディ", 0x05, 0x03));
                ContentKindDictionary.Add(0x0504, new ContentKindInfo("バラエティ", "音楽バラエティ", 0x05, 0x04));
                ContentKindDictionary.Add(0x0505, new ContentKindInfo("バラエティ", "旅バラエティ", 0x05, 0x05));
                ContentKindDictionary.Add(0x0506, new ContentKindInfo("バラエティ", "料理バラエティ", 0x05, 0x06));
                ContentKindDictionary.Add(0x050F, new ContentKindInfo("バラエティ", "その他", 0x05, 0x0F));

                ContentKindDictionary.Add(0x06FF, new ContentKindInfo("映画", "", 0x06, 0xFF));
                ContentKindDictionary.Add(0x0600, new ContentKindInfo("映画", "洋画", 0x06, 0x00));
                ContentKindDictionary.Add(0x0601, new ContentKindInfo("映画", "邦画", 0x06, 0x01));
                ContentKindDictionary.Add(0x0602, new ContentKindInfo("映画", "アニメ", 0x06, 0x02));
                ContentKindDictionary.Add(0x060F, new ContentKindInfo("映画", "その他", 0x06, 0x0F));

                ContentKindDictionary.Add(0x07FF, new ContentKindInfo("アニメ／特撮", "", 0x07, 0xFF));
                ContentKindDictionary.Add(0x0700, new ContentKindInfo("アニメ／特撮", "国内アニメ", 0x07, 0x00));
                ContentKindDictionary.Add(0x0701, new ContentKindInfo("アニメ／特撮", "海外アニメ", 0x07, 0x01));
                ContentKindDictionary.Add(0x0702, new ContentKindInfo("アニメ／特撮", "特撮", 0x07, 0x02));
                ContentKindDictionary.Add(0x070F, new ContentKindInfo("アニメ／特撮", "その他", 0x07, 0x0F));

                ContentKindDictionary.Add(0x08FF, new ContentKindInfo("ドキュメンタリー／教養", "", 0x08, 0xFF));
                ContentKindDictionary.Add(0x0800, new ContentKindInfo("ドキュメンタリー／教養", "社会・時事", 0x08, 0x00));
                ContentKindDictionary.Add(0x0801, new ContentKindInfo("ドキュメンタリー／教養", "歴史・紀行", 0x08, 0x01));
                ContentKindDictionary.Add(0x0802, new ContentKindInfo("ドキュメンタリー／教養", "自然・動物・環境", 0x08, 0x02));
                ContentKindDictionary.Add(0x0803, new ContentKindInfo("ドキュメンタリー／教養", "宇宙・科学・医学", 0x08, 0x03));
                ContentKindDictionary.Add(0x0804, new ContentKindInfo("ドキュメンタリー／教養", "カルチャー・伝統文化", 0x08, 0x04));
                ContentKindDictionary.Add(0x0805, new ContentKindInfo("ドキュメンタリー／教養", "文学・文芸", 0x08, 0x05));
                ContentKindDictionary.Add(0x0806, new ContentKindInfo("ドキュメンタリー／教養", "スポーツ", 0x08, 0x06));
                ContentKindDictionary.Add(0x0807, new ContentKindInfo("ドキュメンタリー／教養", "ドキュメンタリー全般", 0x08, 0x07));
                ContentKindDictionary.Add(0x0808, new ContentKindInfo("ドキュメンタリー／教養", "インタビュー・討論", 0x08, 0x08));
                ContentKindDictionary.Add(0x080F, new ContentKindInfo("ドキュメンタリー／教養", "その他", 0x08, 0x0F));

                ContentKindDictionary.Add(0x09FF, new ContentKindInfo("劇場／公演", "", 0x09, 0xFF));
                ContentKindDictionary.Add(0x0900, new ContentKindInfo("劇場／公演", "現代劇・新劇", 0x09, 0x00));
                ContentKindDictionary.Add(0x0901, new ContentKindInfo("劇場／公演", "ミュージカル", 0x09, 0x01));
                ContentKindDictionary.Add(0x0902, new ContentKindInfo("劇場／公演", "ダンス・バレエ", 0x09, 0x02));
                ContentKindDictionary.Add(0x0903, new ContentKindInfo("劇場／公演", "落語・演芸", 0x09, 0x03));
                ContentKindDictionary.Add(0x0904, new ContentKindInfo("劇場／公演", "歌舞伎・古典", 0x09, 0x04));
                ContentKindDictionary.Add(0x090F, new ContentKindInfo("劇場／公演", "その他", 0x09, 0x0F));

                ContentKindDictionary.Add(0x0AFF, new ContentKindInfo("趣味／教育", "", 0x0A, 0xFF));
                ContentKindDictionary.Add(0x0A00, new ContentKindInfo("趣味／教育", "旅・釣り・アウトドア", 0x0A, 0x00));
                ContentKindDictionary.Add(0x0A01, new ContentKindInfo("趣味／教育", "園芸・ペット・手芸", 0x0A, 0x01));
                ContentKindDictionary.Add(0x0A02, new ContentKindInfo("趣味／教育", "音楽・美術・工芸", 0x0A, 0x02));
                ContentKindDictionary.Add(0x0A03, new ContentKindInfo("趣味／教育", "囲碁・将棋", 0x0A, 0x03));
                ContentKindDictionary.Add(0x0A04, new ContentKindInfo("趣味／教育", "麻雀・パチンコ", 0x0A, 0x04));
                ContentKindDictionary.Add(0x0A05, new ContentKindInfo("趣味／教育", "車・オートバイ", 0x0A, 0x05));
                ContentKindDictionary.Add(0x0A06, new ContentKindInfo("趣味／教育", "コンピュータ・ＴＶゲーム", 0x0A, 0x06));
                ContentKindDictionary.Add(0x0A07, new ContentKindInfo("趣味／教育", "会話・語学", 0x0A, 0x07));
                ContentKindDictionary.Add(0x0A08, new ContentKindInfo("趣味／教育", "幼児・小学生", 0x0A, 0x08));
                ContentKindDictionary.Add(0x0A09, new ContentKindInfo("趣味／教育", "中学生・高校生", 0x0A, 0x09));
                ContentKindDictionary.Add(0x0A0A, new ContentKindInfo("趣味／教育", "大学生・受験", 0x0A, 0x0A));
                ContentKindDictionary.Add(0x0A0B, new ContentKindInfo("趣味／教育", "生涯教育・資格", 0x0A, 0x0B));
                ContentKindDictionary.Add(0x0A0C, new ContentKindInfo("趣味／教育", "教育問題", 0x0A, 0x0C));
                ContentKindDictionary.Add(0x0A0F, new ContentKindInfo("趣味／教育", "その他", 0x0A, 0x0F));

                ContentKindDictionary.Add(0x0BFF, new ContentKindInfo("福祉", "", 0x0B, 0xFF));
                ContentKindDictionary.Add(0x0B00, new ContentKindInfo("福祉", "高齢者", 0x0B, 0x00));
                ContentKindDictionary.Add(0x0B01, new ContentKindInfo("福祉", "障害者", 0x0B, 0x01));
                ContentKindDictionary.Add(0x0B02, new ContentKindInfo("福祉", "社会福祉", 0x0B, 0x02));
                ContentKindDictionary.Add(0x0B03, new ContentKindInfo("福祉", "ボランティア", 0x0B, 0x03));
                ContentKindDictionary.Add(0x0B04, new ContentKindInfo("福祉", "手話", 0x0B, 0x04));
                ContentKindDictionary.Add(0x0B05, new ContentKindInfo("福祉", "文字（字幕）", 0x0B, 0x05));
                ContentKindDictionary.Add(0x0B06, new ContentKindInfo("福祉", "音声解説", 0x0B, 0x06));
                ContentKindDictionary.Add(0x0B0F, new ContentKindInfo("福祉", "その他", 0x0B, 0x0F));

                ContentKindDictionary.Add(0x0FFF, new ContentKindInfo("その他", "", 0x0F, 0xFF));

                ContentKindDictionary.Add(0x70FF, new ContentKindInfo("スポーツ(CS)", "", 0x70, 0xFF));
                ContentKindDictionary.Add(0x7000, new ContentKindInfo("スポーツ(CS)", "テニス", 0x70, 0x00));
                ContentKindDictionary.Add(0x7001, new ContentKindInfo("スポーツ(CS)", "バスケットボール", 0x70, 0x01));
                ContentKindDictionary.Add(0x7002, new ContentKindInfo("スポーツ(CS)", "ラグビー", 0x70, 0x02));
                ContentKindDictionary.Add(0x7003, new ContentKindInfo("スポーツ(CS)", "アメリカンフットボール", 0x70, 0x03));
                ContentKindDictionary.Add(0x7004, new ContentKindInfo("スポーツ(CS)", "ボクシング", 0x70, 0x04));
                ContentKindDictionary.Add(0x7005, new ContentKindInfo("スポーツ(CS)", "プロレス", 0x70, 0x05));
                ContentKindDictionary.Add(0x700F, new ContentKindInfo("スポーツ(CS)", "その他", 0x70, 0x0F));

                ContentKindDictionary.Add(0x71FF, new ContentKindInfo("洋画(CS)", "", 0x71, 0xFF));
                ContentKindDictionary.Add(0x7100, new ContentKindInfo("洋画(CS)", "アクション", 0x71, 0x00));
                ContentKindDictionary.Add(0x7101, new ContentKindInfo("洋画(CS)", "SF／ファンタジー", 0x71, 0x01));
                ContentKindDictionary.Add(0x7102, new ContentKindInfo("洋画(CS)", "コメディー", 0x71, 0x02));
                ContentKindDictionary.Add(0x7103, new ContentKindInfo("洋画(CS)", "サスペンス／ミステリー", 0x71, 0x03));
                ContentKindDictionary.Add(0x7104, new ContentKindInfo("洋画(CS)", "恋愛／ロマンス", 0x71, 0x04));
                ContentKindDictionary.Add(0x7105, new ContentKindInfo("洋画(CS)", "ホラー／スリラー", 0x71, 0x05));
                ContentKindDictionary.Add(0x7106, new ContentKindInfo("洋画(CS)", "ウエスタン", 0x71, 0x06));
                ContentKindDictionary.Add(0x7107, new ContentKindInfo("洋画(CS)", "ドラマ／社会派ドラマ", 0x71, 0x07));
                ContentKindDictionary.Add(0x7108, new ContentKindInfo("洋画(CS)", "アニメーション", 0x71, 0x08));
                ContentKindDictionary.Add(0x7109, new ContentKindInfo("洋画(CS)", "ドキュメンタリー", 0x71, 0x09));
                ContentKindDictionary.Add(0x710A, new ContentKindInfo("洋画(CS)", "アドベンチャー／冒険", 0x71, 0x0A));
                ContentKindDictionary.Add(0x710B, new ContentKindInfo("洋画(CS)", "ミュージカル／音楽映画", 0x71, 0x0B));
                ContentKindDictionary.Add(0x710C, new ContentKindInfo("洋画(CS)", "ホームドラマ", 0x71, 0x0C));
                ContentKindDictionary.Add(0x710F, new ContentKindInfo("洋画(CS)", "その他", 0x71, 0x0F));

                ContentKindDictionary.Add(0x72FF, new ContentKindInfo("邦画(CS)", "", 0x72, 0xFF));
                ContentKindDictionary.Add(0x7200, new ContentKindInfo("邦画(CS)", "アクション", 0x72, 0x00));
                ContentKindDictionary.Add(0x7201, new ContentKindInfo("邦画(CS)", "SF／ファンタジー", 0x72, 0x01));
                ContentKindDictionary.Add(0x7202, new ContentKindInfo("邦画(CS)", "お笑い／コメディー", 0x72, 0x02));
                ContentKindDictionary.Add(0x7203, new ContentKindInfo("邦画(CS)", "サスペンス／ミステリー", 0x72, 0x03));
                ContentKindDictionary.Add(0x7204, new ContentKindInfo("邦画(CS)", "恋愛／ロマンス", 0x72, 0x04));
                ContentKindDictionary.Add(0x7205, new ContentKindInfo("邦画(CS)", "ホラー／スリラー", 0x72, 0x05));
                ContentKindDictionary.Add(0x7206, new ContentKindInfo("邦画(CS)", "青春／学園／アイドル", 0x72, 0x06));
                ContentKindDictionary.Add(0x7207, new ContentKindInfo("邦画(CS)", "任侠／時代劇", 0x72, 0x07));
                ContentKindDictionary.Add(0x7208, new ContentKindInfo("邦画(CS)", "アニメーション", 0x72, 0x08));
                ContentKindDictionary.Add(0x7209, new ContentKindInfo("邦画(CS)", "ドキュメンタリー", 0x72, 0x09));
                ContentKindDictionary.Add(0x720A, new ContentKindInfo("邦画(CS)", "アドベンチャー／冒険", 0x72, 0x0A));
                ContentKindDictionary.Add(0x720B, new ContentKindInfo("邦画(CS)", "ミュージカル／音楽映画", 0x72, 0x0B));
                ContentKindDictionary.Add(0x720C, new ContentKindInfo("邦画(CS)", "ホームドラマ", 0x72, 0x0C));
                ContentKindDictionary.Add(0x720F, new ContentKindInfo("邦画(CS)", "その他", 0x72, 0x0F));

                ContentKindDictionary.Add(0xFFFF, new ContentKindInfo("なし", "", 0xFF, 0xFF));
            }
            {
                ComponentKindDictionary = new Dictionary<UInt16, string>()
                {
                    { 0x0101, "480i(525i)、アスペクト比4:3" },
                    { 0x0102, "480i(525i)、アスペクト比16:9 パンベクトルあり" },
                    { 0x0103, "480i(525i)、アスペクト比16:9 パンベクトルなし" },
                    { 0x0104, "480i(525i)、アスペクト比 > 16:9" },
                    { 0x0191, "2160p、アスペクト比4:3" },
                    { 0x0192, "2160p、アスペクト比16:9 パンベクトルあり" },
                    { 0x0193, "2160p、アスペクト比16:9 パンベクトルなし" },
                    { 0x0194, "2160p、アスペクト比 > 16:9" },
                    { 0x01A1, "480p(525p)、アスペクト比4:3" },
                    { 0x01A2, "480p(525p)、アスペクト比16:9 パンベクトルあり" },
                    { 0x01A3, "480p(525p)、アスペクト比16:9 パンベクトルなし" },
                    { 0x01A4, "480p(525p)、アスペクト比 > 16:9" },
                    { 0x01B1, "1080i(1125i)、アスペクト比4:3" },
                    { 0x01B2, "1080i(1125i)、アスペクト比16:9 パンベクトルあり" },
                    { 0x01B3, "1080i(1125i)、アスペクト比16:9 パンベクトルなし" },
                    { 0x01B4, "1080i(1125i)、アスペクト比 > 16:9" },
                    { 0x01C1, "720p(750p)、アスペクト比4:3" },
                    { 0x01C2, "720p(750p)、アスペクト比16:9 パンベクトルあり" },
                    { 0x01C3, "720p(750p)、アスペクト比16:9 パンベクトルなし" },
                    { 0x01C4, "720p(750p)、アスペクト比 > 16:9" },
                    { 0x01D1, "240p アスペクト比4:3" },
                    { 0x01D2, "240p アスペクト比16:9 パンベクトルあり" },
                    { 0x01D3, "240p アスペクト比16:9 パンベクトルなし" },
                    { 0x01D4, "240p アスペクト比 > 16:9" },
                    { 0x01E1, "1080p(1125p)、アスペクト比4:3" },
                    { 0x01E2, "1080p(1125p)、アスペクト比16:9 パンベクトルあり" },
                    { 0x01E3, "1080p(1125p)、アスペクト比16:9 パンベクトルなし" },
                    { 0x01E4, "1080p(1125p)、アスペクト比 > 16:9" },
                    { 0x0201, "1/0モード（シングルモノ）" },
                    { 0x0202, "1/0＋1/0モード（デュアルモノ）" },
                    { 0x0203, "2/0モード（ステレオ）" },
                    { 0x0204, "2/1モード" },
                    { 0x0205, "3/0モード" },
                    { 0x0206, "2/2モード" },
                    { 0x0207, "3/1モード" },
                    { 0x0208, "3/2モード" },
                    { 0x0209, "3/2＋LFEモード（3/2.1モード）" },
                    { 0x020A, "3/3.1モード" },
                    { 0x020B, "2/0/0-2/0/2-0.1モード" },
                    { 0x020C, "5/2.1モード" },
                    { 0x020D, "3/2/2.1モード" },
                    { 0x020E, "2/0/0-3/0/2-0.1モード" },
                    { 0x020F, "0/2/0-3/0/2-0.1モード" },
                    { 0x0210, "2/0/0-3/2/3-0.2モード" },
                    { 0x0211, "3/3/3-5/2/3-3/0/0.2モード" },
                    { 0x0240, "視覚障害者用音声解説" },
                    { 0x0241, "聴覚障害者用音声" },
                    { 0x0501, "H.264|MPEG-4 AVC、480i(525i)、アスペクト比4:3" },
                    { 0x0502, "H.264|MPEG-4 AVC、480i(525i)、アスペクト比16:9 パンベクトルあり" },
                    { 0x0503, "H.264|MPEG-4 AVC、480i(525i)、アスペクト比16:9 パンベクトルなし" },
                    { 0x0504, "H.264|MPEG-4 AVC、480i(525i)、アスペクト比 > 16:9" },
                    { 0x0591, "H.264|MPEG-4 AVC、2160p、アスペクト比4:3" },
                    { 0x0592, "H.264|MPEG-4 AVC、2160p、アスペクト比16:9 パンベクトルあり" },
                    { 0x0593, "H.264|MPEG-4 AVC、2160p、アスペクト比16:9 パンベクトルなし" },
                    { 0x0594, "H.264|MPEG-4 AVC、2160p、アスペクト比 > 16:9" },
                    { 0x05A1, "H.264|MPEG-4 AVC、480p(525p)、アスペクト比4:3" },
                    { 0x05A2, "H.264|MPEG-4 AVC、480p(525p)、アスペクト比16:9 パンベクトルあり" },
                    { 0x05A3, "H.264|MPEG-4 AVC、480p(525p)、アスペクト比16:9 パンベクトルなし" },
                    { 0x05A4, "H.264|MPEG-4 AVC、480p(525p)、アスペクト比 > 16:9" },
                    { 0x05B1, "H.264|MPEG-4 AVC、1080i(1125i)、アスペクト比4:3" },
                    { 0x05B2, "H.264|MPEG-4 AVC、1080i(1125i)、アスペクト比16:9 パンベクトルあり" },
                    { 0x05B3, "H.264|MPEG-4 AVC、1080i(1125i)、アスペクト比16:9 パンベクトルなし" },
                    { 0x05B4, "H.264|MPEG-4 AVC、1080i(1125i)、アスペクト比 > 16:9" },
                    { 0x05C1, "H.264|MPEG-4 AVC、720p(750p)、アスペクト比4:3" },
                    { 0x05C2, "H.264|MPEG-4 AVC、720p(750p)、アスペクト比16:9 パンベクトルあり" },
                    { 0x05C3, "H.264|MPEG-4 AVC、720p(750p)、アスペクト比16:9 パンベクトルなし" },
                    { 0x05C4, "H.264|MPEG-4 AVC、720p(750p)、アスペクト比 > 16:9" },
                    { 0x05D1, "H.264|MPEG-4 AVC、240p アスペクト比4:3" },
                    { 0x05D2, "H.264|MPEG-4 AVC、240p アスペクト比16:9 パンベクトルあり" },
                    { 0x05D3, "H.264|MPEG-4 AVC、240p アスペクト比16:9 パンベクトルなし" },
                    { 0x05D4, "H.264|MPEG-4 AVC、240p アスペクト比 > 16:9" },
                    { 0x05E1, "H.264|MPEG-4 AVC、1080p(1125p)、アスペクト比4:3" },
                    { 0x05E2, "H.264|MPEG-4 AVC、1080p(1125p)、アスペクト比16:9 パンベクトルあり" },
                    { 0x05E3, "H.264|MPEG-4 AVC、1080p(1125p)、アスペクト比16:9 パンベクトルなし" },
                    { 0x05E4, "H.264|MPEG-4 AVC、1080p(1125p)、アスペクト比 > 16:9" }
                };
            }
            DayOfWeekArray = new string[] { "日", "月", "火", "水", "木", "金", "土" };
            NWMode = false;
            NotifyLogList = new List<NotifySrvInfo>();
            CustContentColorList = new List<Brush>();
            CustTimeColorList = new List<Brush>();
        }

        public static UInt64 Create64Key(UInt16 ONID, UInt16 TSID, UInt16 SID)
        {
            UInt64 key = ((UInt64)ONID) << 32 | ((UInt64)TSID) << 16 | (UInt64)SID;
            return key;
        }

        public static UInt64 Create64PgKey(UInt16 ONID, UInt16 TSID, UInt16 SID, UInt16 EventID)
        {
            UInt64 key = ((UInt64)ONID) << 48 | ((UInt64)TSID) << 32 | ((UInt64)SID) << 16 | (UInt64)EventID;
            return key;
        }

        public static EpgServiceInfo ConvertChSet5To(ChSet5Item item)
        {
            EpgServiceInfo info = new EpgServiceInfo();
            try
            {
                info.ONID = item.ONID;
                info.TSID = item.TSID;
                info.SID = item.SID;
                info.network_name = item.NetworkName;
                info.partialReceptionFlag = item.PartialFlag;
                info.remote_control_key_id = item.RemoconID;
                info.service_name = item.ServiceName;
                info.service_provider_name = item.NetworkName;
                info.service_type = (byte)item.ServiceType;
                info.ts_name = item.NetworkName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            } 
            return info;
        }

        public static String ReplaceUrl(String url)
        {
            string retText = url;

            retText = retText.Replace("ａ", "a");
            retText = retText.Replace("ｂ", "b");
            retText = retText.Replace("ｃ", "c");
            retText = retText.Replace("ｄ", "d");
            retText = retText.Replace("ｅ", "e");
            retText = retText.Replace("ｆ", "f");
            retText = retText.Replace("ｇ", "g");
            retText = retText.Replace("ｈ", "h");
            retText = retText.Replace("ｉ", "i");
            retText = retText.Replace("ｊ", "j");
            retText = retText.Replace("ｋ", "k");
            retText = retText.Replace("ｌ", "l");
            retText = retText.Replace("ｍ", "m");
            retText = retText.Replace("ｎ", "n");
            retText = retText.Replace("ｏ", "o");
            retText = retText.Replace("ｐ", "p");
            retText = retText.Replace("ｑ", "q");
            retText = retText.Replace("ｒ", "r");
            retText = retText.Replace("ｓ", "s");
            retText = retText.Replace("ｔ", "t");
            retText = retText.Replace("ｕ", "u");
            retText = retText.Replace("ｖ", "v");
            retText = retText.Replace("ｗ", "w");
            retText = retText.Replace("ｘ", "x");
            retText = retText.Replace("ｙ", "y");
            retText = retText.Replace("ｚ", "z");
            retText = retText.Replace("Ａ", "A");
            retText = retText.Replace("Ｂ", "B");
            retText = retText.Replace("Ｃ", "C");
            retText = retText.Replace("Ｄ", "D");
            retText = retText.Replace("Ｅ", "E");
            retText = retText.Replace("Ｆ", "F");
            retText = retText.Replace("Ｇ", "G");
            retText = retText.Replace("Ｈ", "H");
            retText = retText.Replace("Ｉ", "I");
            retText = retText.Replace("Ｊ", "J");
            retText = retText.Replace("Ｋ", "K");
            retText = retText.Replace("Ｌ", "L");
            retText = retText.Replace("Ｍ", "M");
            retText = retText.Replace("Ｎ", "N");
            retText = retText.Replace("Ｏ", "O");
            retText = retText.Replace("Ｐ", "P");
            retText = retText.Replace("Ｑ", "Q");
            retText = retText.Replace("Ｒ", "R");
            retText = retText.Replace("Ｓ", "S");
            retText = retText.Replace("Ｔ", "T");
            retText = retText.Replace("Ｕ", "U");
            retText = retText.Replace("Ｖ", "V");
            retText = retText.Replace("Ｗ", "W");
            retText = retText.Replace("Ｘ", "X");
            retText = retText.Replace("Ｙ", "Y");
            retText = retText.Replace("Ｚ", "Z");
            retText = retText.Replace("＃", "#");
            retText = retText.Replace("＄", "$");
            retText = retText.Replace("％", "%");
            retText = retText.Replace("＆", "&");
            retText = retText.Replace("’", "'");
            retText = retText.Replace("（", "(");
            retText = retText.Replace("）", ")");
            retText = retText.Replace("～", "~");
            retText = retText.Replace("＝", "=");
            retText = retText.Replace("｜", "|");
            retText = retText.Replace("＾", "^");
            retText = retText.Replace("￥", "\\");
            retText = retText.Replace("＠", "@");
            retText = retText.Replace("；", ";");
            retText = retText.Replace("：", ":");
            retText = retText.Replace("｀", "`");
            retText = retText.Replace("｛", "{");
            retText = retText.Replace("｝", "}");
            retText = retText.Replace("＜", "<");
            retText = retText.Replace("＞", ">");
            retText = retText.Replace("？", "?");
            retText = retText.Replace("＿", "_");
            retText = retText.Replace("＋", "+");
            retText = retText.Replace("－", "-");
            retText = retText.Replace("＊", "*");
            retText = retText.Replace("／", "/");
            retText = retText.Replace("．", ".");
            retText = retText.Replace("０", "0");
            retText = retText.Replace("１", "1");
            retText = retText.Replace("２", "2");
            retText = retText.Replace("３", "3");
            retText = retText.Replace("４", "4");
            retText = retText.Replace("５", "5");
            retText = retText.Replace("６", "6");
            retText = retText.Replace("７", "7");
            retText = retText.Replace("８", "8");
            retText = retText.Replace("９", "9");

            return retText;
        }

        public String ConvertReserveText(ReserveData reserveInfo)
        {
            String view = "";
            view = reserveInfo.StartTime.ToString("yyyy/MM/dd(ddd) HH:mm:ss ～ ");
            DateTime endTime = reserveInfo.StartTime + TimeSpan.FromSeconds(reserveInfo.DurationSecond);
            view += endTime.ToString("yyyy/MM/dd(ddd) HH:mm:ss") + "\r\n";

            String recMode = "";
            switch (reserveInfo.RecSetting.RecMode)
            {
                case 0:
                    recMode = "全サービス";
                    break;
                case 1:
                    recMode = "指定サービス";
                    break;
                case 2:
                    recMode = "全サービス（デコード処理なし）";
                    break;
                case 3:
                    recMode = "指定サービス（デコード処理なし）";
                    break;
                case 4:
                    recMode = "視聴";
                    break;
                case 5:
                    recMode = "無効";
                    break;
                default:
                    break;
            } 
            String tuijyu = "";
            if (reserveInfo.RecSetting.TuijyuuFlag == 0)
            {
                tuijyu = "しない";
            }
            else if (reserveInfo.RecSetting.TuijyuuFlag == 1)
            {
                tuijyu = "する";
            }
            String pittari = "";
            if (reserveInfo.RecSetting.PittariFlag == 0)
            {
                pittari = "しない";
            }
            else if (reserveInfo.RecSetting.PittariFlag == 1)
            {
                pittari = "する";
            }

            view += reserveInfo.StationName;
            if (0x7880 <= reserveInfo.OriginalNetworkID && reserveInfo.OriginalNetworkID <= 0x7FE8)
            {
                view += " (地デジ)";
            }
            else if (reserveInfo.OriginalNetworkID == 0x0004)
            {
                view += " (BS)";
            }
            else if (reserveInfo.OriginalNetworkID == 0x0006)
            {
                view += " (CS1)";
            }
            else if (reserveInfo.OriginalNetworkID == 0x0007)
            {
                view += " (CS2)";
            }
            else
            {
                view += " (その他)";
            }
            view += "\r\n";

            view += reserveInfo.Title + "\r\n\r\n";
            view += "録画モード : " + recMode + "\r\n";
            view += "優先度 : " + reserveInfo.RecSetting.Priority.ToString() + "\r\n";
            view += "追従 : " + tuijyu + "\r\n";
            view += "ぴったり（？） : " + pittari + "\r\n";
            if ((reserveInfo.RecSetting.ServiceMode & 0x01) == 0)
            {
                view += "指定サービス対象データ : デフォルト\r\n";
            }
            else
            {
                view += "指定サービス対象データ : ";
                if ((reserveInfo.RecSetting.ServiceMode & 0x10) > 0)
                {
                    view += "字幕含む ";
                }
                if ((reserveInfo.RecSetting.ServiceMode & 0x20) > 0)
                {
                    view += "データカルーセル含む";
                }
                view += "\r\n";
            }

            view += "録画実行bat : " + reserveInfo.RecSetting.BatFilePath + "\r\n";

            if (reserveInfo.RecSetting.RecFolderList.Count == 0)
            {
                view += "録画フォルダ : デフォルト\r\n";
            }
            else
            {
                view += "録画フォルダ : \r\n";
                foreach (RecFileSetInfo info in reserveInfo.RecSetting.RecFolderList)
                {
                    view += info.RecFolder + " (WritePlugIn:" + info.WritePlugIn + " ファイル名PlugIn:" + info.RecNamePlugIn + ")\r\n";
                }
            }

            if (reserveInfo.RecSetting.UseMargineFlag == 0)
            {
                view += "録画マージン : デフォルト\r\n";
            }
            else
            {
                view += "録画マージン : 開始 " + reserveInfo.RecSetting.StartMargine.ToString() +
                    " 終了 " + reserveInfo.RecSetting.EndMargine.ToString() + "\r\n";
            }

            if (reserveInfo.RecSetting.SuspendMode == 0)
            {
                view += "録画後動作 : デフォルト\r\n";
            }
            else
            {
                view += "録画後動作 : ";
                switch (reserveInfo.RecSetting.SuspendMode)
                {
                    case 1:
                        view += "スタンバイ";
                        break;
                    case 2:
                        view += "休止";
                        break;
                    case 3:
                        view += "シャットダウン";
                        break;
                    case 4:
                        view += "何もしない";
                        break;
                }
                if (reserveInfo.RecSetting.RebootFlag == 1)
                {
                    view += " 復帰後再起動する";
                }
                view += "\r\n";
            }
            if (reserveInfo.RecSetting.PartialRecFlag == 0)
            {
                view += "部分受信 : 同時出力なし\r\n";
            }
            else
            {
                view += "部分受信 : 同時出力あり\r\n";
                view += "部分受信　録画フォルダ : \r\n";
                foreach (RecFileSetInfo info in reserveInfo.RecSetting.PartialRecFolder)
                {
                    view += info.RecFolder + " (WritePlugIn:" + info.WritePlugIn + " ファイル名PlugIn:" + info.RecNamePlugIn + ")\r\n";
                }
            }
            if (reserveInfo.RecSetting.ContinueRecFlag == 0)
            {
                view += "連続録画動作 : 分割\r\n";
            }
            else
            {
                view += "連続録画動作 : 同一ファイル出力\r\n";
            }
            if (reserveInfo.RecSetting.TunerID == 0)
            {
                view += "使用チューナー強制指定 : 自動\r\n";
            }
            else
            {
                view += "使用チューナー強制指定 : ID:" + reserveInfo.RecSetting.TunerID.ToString("X8") + "\r\n";
            }

            view += "予約状況 : " + reserveInfo.Comment;
            view += "\r\n\r\n"; 
            
            view += "OriginalNetworkID : " + reserveInfo.OriginalNetworkID.ToString() + " (0x" + reserveInfo.OriginalNetworkID.ToString("X4") + ")\r\n";
            view += "TransportStreamID : " + reserveInfo.TransportStreamID.ToString() + " (0x" + reserveInfo.TransportStreamID.ToString("X4") + ")\r\n";
            view += "ServiceID : " + reserveInfo.ServiceID.ToString() + " (0x" + reserveInfo.ServiceID.ToString("X4") + ")\r\n";
            view += "EventID : " + reserveInfo.EventID.ToString() + " (0x" + reserveInfo.EventID.ToString("X4") + ")\r\n";

            return view;
        }

        public String ConvertProgramText(EpgEventInfo eventInfo, EventInfoTextMode textMode)
        {
            string retText = "";
            string basicInfo = "";
            string extInfo = "";
            if (eventInfo != null)
            {
                UInt64 key = ((UInt64)eventInfo.original_network_id) << 32 |
                    ((UInt64)eventInfo.transport_stream_id) << 16 |
                    ((UInt64)eventInfo.service_id);
                if (ChSet5.Instance.ChList.ContainsKey(key) == true)
                {
                    basicInfo += ChSet5.Instance.ChList[key].ServiceName + "(" + ChSet5.Instance.ChList[key].NetworkName + ")" + "\r\n";
                }

                if (eventInfo.StartTimeFlag == 1)
                {
                    basicInfo += eventInfo.start_time.ToString("yyyy/MM/dd(ddd) HH:mm:ss ～ ");
                }
                else
                {
                    basicInfo += "未定 ～ ";
                }
                if (eventInfo.DurationFlag == 1)
                {
                    DateTime endTime = eventInfo.start_time + TimeSpan.FromSeconds(eventInfo.durationSec);
                    basicInfo += endTime.ToString("yyyy/MM/dd(ddd) HH:mm:ss") + "\r\n";
                }
                else
                {
                    basicInfo += "未定\r\n";
                }

                if (eventInfo.ShortInfo != null)
                {
                    basicInfo += eventInfo.ShortInfo.event_name + "\r\n\r\n";
                    extInfo += eventInfo.ShortInfo.text_char + "\r\n\r\n";
                }

                if (eventInfo.ExtInfo != null)
                {
                    extInfo += eventInfo.ExtInfo.text_char + "\r\n\r\n";
                }

                //ジャンル
                extInfo += "ジャンル :\r\n";
                if (eventInfo.ContentInfo != null)
                {
                    foreach (EpgContentData info in eventInfo.ContentInfo.nibbleList)
                    {
                        String content = "";
                        int nibble1 = info.content_nibble_level_1;
                        int nibble2 = info.content_nibble_level_2;
                        if (nibble1 == 0x0E && nibble2 == 0x01)
                        {
                            nibble1 = info.user_nibble_1 | 0x70;
                            nibble2 = info.user_nibble_2;
                        }
                        if (ContentKindDictionary.ContainsKey((ushort)(nibble1 << 8 | 0xFF)))
                        {
                            content += ContentKindDictionary[(ushort)(nibble1 << 8 | 0xFF)].ContentName;
                        }
                        else
                        {
                            content += "(0x" + nibble1.ToString("X2") + ")";
                        }
                        if (ContentKindDictionary.ContainsKey((ushort)(nibble1 << 8 | nibble2)))
                        {
                            content += " - " + ContentKindDictionary[(ushort)(nibble1 << 8 | nibble2)].SubName;
                        }
                        else if (nibble1 != 0x0F)
                        {
                            content += " - (0x" + nibble2.ToString("X2") + ")";
                        }
                        extInfo += content + "\r\n";
                    }
                }
                extInfo += "\r\n";

                //映像
                extInfo += "映像 :";
                if (eventInfo.ComponentInfo != null)
                {
                    int streamContent = eventInfo.ComponentInfo.stream_content;
                    int componentType = eventInfo.ComponentInfo.component_type;
                    UInt16 componentKey = (UInt16)(streamContent << 8 | componentType);
                    if (ComponentKindDictionary.ContainsKey(componentKey) == true)
                    {
                        extInfo += ComponentKindDictionary[componentKey];
                    }
                    if (eventInfo.ComponentInfo.text_char.Length > 0)
                    {
                        extInfo += "\r\n";
                        extInfo += eventInfo.ComponentInfo.text_char;
                    }
                }
                extInfo += "\r\n";

                //音声
                extInfo += "音声 :\r\n";
                if (eventInfo.AudioInfo != null)
                {
                    foreach (EpgAudioComponentInfoData info in eventInfo.AudioInfo.componentList)
                    {
                        int streamContent = info.stream_content;
                        int componentType = info.component_type;
                        UInt16 componentKey = (UInt16)(streamContent << 8 | componentType);
                        if (ComponentKindDictionary.ContainsKey(componentKey) == true)
                        {
                            extInfo += ComponentKindDictionary[componentKey];
                        }
                        if (info.text_char.Length > 0)
                        {
                            extInfo += "\r\n";
                            extInfo += info.text_char;
                        }
                        extInfo += "\r\n";
                        extInfo += "サンプリングレート :";
                        switch (info.sampling_rate)
                        {
                            case 1:
                                extInfo += "16kHz";
                                break;
                            case 2:
                                extInfo += "22.05kHz";
                                break;
                            case 3:
                                extInfo += "24kHz";
                                break;
                            case 5:
                                extInfo += "32kHz";
                                break;
                            case 6:
                                extInfo += "44.1kHz";
                                break;
                            case 7:
                                extInfo += "48kHz";
                                break;
                            default:
                                break;
                        }
                        extInfo += "\r\n";
                    }
                }
                extInfo += "\r\n";

                //スクランブル
                if (!(0x7880 <= eventInfo.original_network_id && eventInfo.original_network_id <= 0x7FE8))
                {
                    if (eventInfo.FreeCAFlag == 0)
                    {
                        extInfo += "無料放送\r\n";
                    }
                    else
                    {
                        extInfo += "有料放送\r\n";
                    }
                    extInfo += "\r\n";
                }

                //イベントリレー
                if (eventInfo.EventRelayInfo != null)
                {
                    if (eventInfo.EventRelayInfo.eventDataList.Count > 0)
                    {
                        extInfo += "イベントリレーあり：\r\n";
                        foreach (EpgEventData info in eventInfo.EventRelayInfo.eventDataList)
                        {
                            key = ((UInt64)info.original_network_id) << 32 |
                                ((UInt64)info.transport_stream_id) << 16 |
                                ((UInt64)info.service_id);
                            if (ChSet5.Instance.ChList.ContainsKey(key) == true)
                            {
                                extInfo += ChSet5.Instance.ChList[key].ServiceName + "(" + ChSet5.Instance.ChList[key].NetworkName + ")" + " ";
                            }
                            else
                            {
                                extInfo += "OriginalNetworkID : " + info.original_network_id.ToString() + " (0x" + info.original_network_id.ToString("X4") + ") ";
                                extInfo += "TransportStreamID : " + info.transport_stream_id.ToString() + " (0x" + info.transport_stream_id.ToString("X4") + ") ";
                                extInfo += "ServiceID : " + info.service_id.ToString() + " (0x" + info.service_id.ToString("X4") + ") ";
                            }
                            extInfo += "EventID : " + info.event_id.ToString() + " (0x" + info.event_id.ToString("X4") + ")\r\n";
                            extInfo += "\r\n";
                        }
                        extInfo += "\r\n";
                    }
                }

                extInfo += "OriginalNetworkID : " + eventInfo.original_network_id.ToString() + " (0x" + eventInfo.original_network_id.ToString("X4") + ")\r\n";
                extInfo += "TransportStreamID : " + eventInfo.transport_stream_id.ToString() + " (0x" + eventInfo.transport_stream_id.ToString("X4") + ")\r\n";
                extInfo += "ServiceID : " + eventInfo.service_id.ToString() + " (0x" + eventInfo.service_id.ToString("X4") + ")\r\n";
                extInfo += "EventID : " + eventInfo.event_id.ToString() + " (0x" + eventInfo.event_id.ToString("X4") + ")\r\n";

            }

            if (textMode == EventInfoTextMode.All || textMode == EventInfoTextMode.BasicOnly)
            {
                retText = basicInfo;
            }
            if (textMode == EventInfoTextMode.All || textMode == EventInfoTextMode.ExtOnly)
            {
                retText += extInfo;
            }
            return retText;
        }

        public void FilePlay(uint reserveID)
        {
            if (Settings.Instance.FilePlayOnAirWithExe && (NWMode == false || Settings.Instance.FilePlayExe.Length != 0))
            {
                //ファイルパスを取得するため開いてすぐ閉じる
                var info = new NWPlayTimeShiftInfo();
                if (CtrlCmd.SendNwTimeShiftOpen(reserveID, ref info) == ErrCode.CMD_SUCCESS)
                {
                    CtrlCmd.SendNwPlayClose(info.ctrlID);
                    if (info.filePath != "")
                    {
                        FilePlay(info.filePath);
                        return;
                    }
                }
                MessageBox.Show("録画ファイルの場所がわかりませんでした。", "追っかけ再生", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                TVTestCtrl.StartTimeShift(reserveID);
            }
        }

        public void FilePlay(String filePath)
        {
            try
            {
                if (NWMode == false || Settings.Instance.FilePlayExe.Length != 0)
                {
                    System.Diagnostics.Process process;
                    if (Settings.Instance.FilePlayExe.Length == 0)
                    {
                        process = System.Diagnostics.Process.Start(filePath);
                    }
                    else
                    {
                        String cmdLine = Settings.Instance.FilePlayCmd;
                        //'$'->'\t'は再帰的な展開を防ぐため
                        cmdLine = cmdLine.Replace("$FileNameExt$", Path.GetFileName(filePath).Replace('$', '\t'));
                        cmdLine = cmdLine.Replace("$FilePath$", filePath).Replace('\t', '$');
                        process = System.Diagnostics.Process.Start(Settings.Instance.FilePlayExe, cmdLine);

                    }
                }
                else
                {
                    TVTestCtrl.StartStreamingPlay(filePath, NW.ConnectedIP, NW.ConnectedPort);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        public void ReloadCustContentColorList()
        {
            try
            {
                CustContentColorList.Clear();
                string name;
                SolidColorBrush brush;
                for (int i = 0; i < Settings.Instance.ContentColorList.Count; i++)
                {
                    name = Settings.Instance.ContentColorList[i];
                    brush = name == "カスタム" ? null : ColorDef.BrushFromName(name);
                    if (Settings.Instance.EpgGradation == false)
                    {
                        if (brush == null)
                        {
                            (brush = new SolidColorBrush(ColorDef.FromUInt(Settings.Instance.ContentCustColorList[i]))).Freeze();
                        }
                        CustContentColorList.Add(brush);
                    }
                    else
                    {
                        CustContentColorList.Add(ColorDef.GradientBrush(brush == null ? ColorDef.FromUInt(Settings.Instance.ContentCustColorList[i]) : brush.Color));
                    }
                }
                name = Settings.Instance.ReserveRectColorNormal;
                brush = name == "カスタム" ? null : ColorDef.BrushFromName(name);
                if (brush == null)
                {
                    (brush = new SolidColorBrush(ColorDef.FromUInt(Settings.Instance.ContentCustColorList[0x11]))).Freeze();
                }
                CustContentColorList.Add(brush);
                name = Settings.Instance.ReserveRectColorNo;
                brush = name == "カスタム" ? null : ColorDef.BrushFromName(name);
                if (brush == null)
                {
                    (brush = new SolidColorBrush(ColorDef.FromUInt(Settings.Instance.ContentCustColorList[0x12]))).Freeze();
                }
                CustContentColorList.Add(brush);
                name = Settings.Instance.ReserveRectColorNoTuner;
                brush = name == "カスタム" ? null : ColorDef.BrushFromName(name);
                if (brush == null)
                {
                    (brush = new SolidColorBrush(ColorDef.FromUInt(Settings.Instance.ContentCustColorList[0x13]))).Freeze();
                }
                CustContentColorList.Add(brush);
                name = Settings.Instance.ReserveRectColorWarning;
                brush = name == "カスタム" ? null : ColorDef.BrushFromName(name);
                if (brush == null)
                {
                    (brush = new SolidColorBrush(ColorDef.FromUInt(Settings.Instance.ContentCustColorList[0x14]))).Freeze();
                }
                CustContentColorList.Add(brush);

                name = Settings.Instance.TitleColor1;
                brush = name == "カスタム" ? null : ColorDef.BrushFromName(name);
                if (brush == null)
                {
                    (brush = new SolidColorBrush(ColorDef.FromUInt(Settings.Instance.TitleCustColor1))).Freeze();
                }
                CustTitle1Color = brush;
                name = Settings.Instance.TitleColor2;
                brush = name == "カスタム" ? null : ColorDef.BrushFromName(name);
                if (brush == null)
                {
                    (brush = new SolidColorBrush(ColorDef.FromUInt(Settings.Instance.TitleCustColor2))).Freeze();
                }
                CustTitle2Color = brush;
                CustTimeColorList.Clear();
                for (int i = 0; i < Settings.Instance.TimeColorList.Count; i++)
                {
                    name = Settings.Instance.TimeColorList[i];
                    brush = name == "カスタム" ? null : ColorDef.BrushFromName(name);
                    if (Settings.Instance.EpgGradationHeader == false)
                    {
                        if (brush == null)
                        {
                            (brush = new SolidColorBrush(ColorDef.FromUInt(Settings.Instance.TimeCustColorList[i]))).Freeze();
                        }
                        CustTimeColorList.Add(brush);
                    }
                    else
                    {
                        CustTimeColorList.Add(ColorDef.GradientBrush(brush == null ? ColorDef.FromUInt(Settings.Instance.TimeCustColorList[i]) : brush.Color, 0.9, 1.1));
                    }
                }
                name = Settings.Instance.ServiceColor;
                brush = name == "カスタム" ? null : ColorDef.BrushFromName(name);
                if (Settings.Instance.EpgGradationHeader == false)
                {
                    if (brush == null)
                    {
                        (brush = new SolidColorBrush(ColorDef.FromUInt(Settings.Instance.ServiceCustColor))).Freeze();
                    }
                    CustServiceColor = brush;
                }
                else
                {
                    CustServiceColor = ColorDef.GradientBrush(brush == null ? ColorDef.FromUInt(Settings.Instance.ServiceCustColor) : brush.Color, 1.0, 2.0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
        }


        private SolidColorBrush resDefBackColor = null;
        public SolidColorBrush ResDefBackColor
        {
            get
            {
                if (resDefBackColor == null)
                {
                    Color item = Color.FromArgb(0xFF, Settings.Instance.ResDefColorR, Settings.Instance.ResDefColorG, Settings.Instance.ResDefColorB);
                    resDefBackColor = new SolidColorBrush();
                    resDefBackColor.Color = item;
                    resDefBackColor.Freeze();
                }
                return resDefBackColor;
            }
        }
        private SolidColorBrush resErrBackColor = null;
        public SolidColorBrush ResErrBackColor
        {
            get
            {
                if (resErrBackColor == null)
                {
                    Color item = Color.FromArgb(0xFF, Settings.Instance.ResErrColorR, Settings.Instance.ResErrColorG, Settings.Instance.ResErrColorB);
                    resErrBackColor = new SolidColorBrush();
                    resErrBackColor.Color = item;
                    resErrBackColor.Freeze();
                }
                return resErrBackColor;
            }
        }
        private SolidColorBrush resWarBackColor = null;
        public SolidColorBrush ResWarBackColor
        {
            get
            {
                if (resWarBackColor == null)
                {
                    Color item = Color.FromArgb(0xFF, Settings.Instance.ResWarColorR, Settings.Instance.ResWarColorG, Settings.Instance.ResWarColorB);
                    resWarBackColor = new SolidColorBrush();
                    resWarBackColor.Color = item;
                    resWarBackColor.Freeze();
                }
                return resWarBackColor;
            }
        }
        private SolidColorBrush resNoBackColor = null;
        public SolidColorBrush ResNoBackColor
        {
            get
            {
                if (resNoBackColor == null)
                {
                    Color item = Color.FromArgb(0xFF, Settings.Instance.ResNoColorR, Settings.Instance.ResNoColorG, Settings.Instance.ResNoColorB);
                    resNoBackColor = new SolidColorBrush();
                    resNoBackColor.Color = item;
                    resNoBackColor.Freeze();
                }
                return resNoBackColor;
            }
        }

        private SolidColorBrush recEndDefBackColor = null;
        public SolidColorBrush RecEndDefBackColor
        {
            get
            {
                if (recEndDefBackColor == null)
                {
                    Color item = Color.FromArgb(0xFF, Settings.Instance.RecEndDefColorR, Settings.Instance.RecEndDefColorG, Settings.Instance.RecEndDefColorB);
                    recEndDefBackColor = new SolidColorBrush();
                    recEndDefBackColor.Color = item;
                    recEndDefBackColor.Freeze();
                }
                return recEndDefBackColor;
            }
        }
        
        private SolidColorBrush recEndErrBackColor = null;
        public SolidColorBrush RecEndErrBackColor
        {
            get
            {
                if( recEndErrBackColor == null ){
                    Color item = Color.FromArgb(0xFF, Settings.Instance.RecEndErrColorR, Settings.Instance.RecEndErrColorG, Settings.Instance.RecEndErrColorB);
                    recEndErrBackColor = new SolidColorBrush();
                    recEndErrBackColor.Color = item;
                    recEndErrBackColor.Freeze();
                }
                return recEndErrBackColor;
            }
        }

        private SolidColorBrush recEndWarBackColor = null;
        public SolidColorBrush RecEndWarBackColor
        {
            get
            {
                if (recEndWarBackColor == null)
                {
                    Color item = Color.FromArgb(0xFF, Settings.Instance.RecEndWarColorR, Settings.Instance.RecEndWarColorG, Settings.Instance.RecEndWarColorB);
                    recEndWarBackColor = new SolidColorBrush();
                    recEndWarBackColor.Color = item;
                    recEndWarBackColor.Freeze();
                }
                return recEndWarBackColor;
            }
        }

        private SolidColorBrush epgTipsBackColor = null;
        public SolidColorBrush EpgTipsBackColor
        {
            get
            {
                if (epgTipsBackColor == null)
                {
                    Color item = Color.FromArgb(0xFF, Settings.Instance.EpgTipsBackColorR, Settings.Instance.EpgTipsBackColorG, Settings.Instance.EpgTipsBackColorB);
                    epgTipsBackColor = new SolidColorBrush();
                    epgTipsBackColor.Color = item;
                    epgTipsBackColor.Freeze();
                }
                return epgTipsBackColor;
            }
        }
        private SolidColorBrush epgTipsForeColor = null;
        public SolidColorBrush EpgTipsForeColor
        {
            get
            {
                if (epgTipsForeColor == null)
                {
                    Color item = Color.FromArgb(0xFF, Settings.Instance.EpgTipsForeColorR, Settings.Instance.EpgTipsForeColorG, Settings.Instance.EpgTipsForeColorB);
                    epgTipsForeColor = new SolidColorBrush();
                    epgTipsForeColor.Color = item;
                    epgTipsForeColor.Freeze();
                }
                return epgTipsForeColor;
            }
        }
        private SolidColorBrush epgBackColor = null;
        public SolidColorBrush EpgBackColor
        {
            get
            {
                if (epgBackColor == null)
                {
                    Color item = Color.FromArgb(0xFF, Settings.Instance.EpgBackColorR, Settings.Instance.EpgBackColorG, Settings.Instance.EpgBackColorB);
                    epgBackColor = new SolidColorBrush(item);
                    epgBackColor.Freeze();
                }
                return epgBackColor;
            }
        }

        public void AddNotifySave(NotifySrvInfo notifyInfo)
        {
            if (Settings.Instance.AutoSaveNotifyLog == 1)
            {
                String filePath = SettingPath.ModulePath;
                filePath += "\\Log";
                Directory.CreateDirectory(filePath);
                filePath += "\\EpgTimerNotify_" + DateTime.UtcNow.AddHours(9).ToString("yyyyMMdd") + ".txt";
                StreamWriter file = new StreamWriter(filePath, true, System.Text.Encoding.GetEncoding("shift_jis"));
                file.WriteLine(new NotifySrvInfoItem(notifyInfo));
                file.Close();
            }
        }
    }
}
