using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aizawa
{
    /// <summary>
    /// 今回獲得スコアとランキングを保持するマネージャークラス
    /// </summary>
    public static class ScoreManager
    {
        /// <summary> 現在のプレイヤーの名前 </summary>
        public static string CurrentName { get; private set; }
        /// <summary> 最後のプレイでの得点 </summary>
        public static int CurrentScore { get; private set; }
        private static Dictionary<string, int> _ranking = new();
        /// <summary> 各名前・順位・得点を包んだリスト </summary>
        public static List<RankingElement> Ranking => GetRanking();

        /// <summary>
        /// ランキングをリセットする。
        /// </summary>
        public static void ResetRanking()
        {
            _ranking.Clear();
        }

        /// <summary>
        /// 新たなプレイヤーを登録する
        /// </summary>
        /// <param name="name">プレイヤー名</param>
        /// <returns>登録できたか</returns>
        public static bool SetPlayer(string name)
        {
            //ランキングに含まれていた場合省く
            if(_ranking.ContainsKey(name))
            {
                Debug.Log("その名前は登録済みです。");
                return false;
            }

            //名前が空欄か空文字のみでも省く
            if(string.IsNullOrWhiteSpace(name))
            {
                Debug.Log("名前が不正です。");
                return false;
            }

            CurrentName = name;
            CurrentScore = 0;

            return true;
        }

        /// <summary>
        /// 新たなスコアを受け取る
        /// </summary>
        /// <param name="score">新スコア</param>
        public static void ReceiveScore(int score)
        {
            CurrentScore = score;
        }

        /// <summary>
        /// 現在の得点に基づきランキングを更新する
        /// </summary>
        public static void UpdateRanking()
        {
            //ランキングに10名以上登録されていて、かつ自身がランキングに入れない場合は省く
            if(_ranking.Count < 10 || _ranking.Any(r => r.Value <= CurrentScore))
            {
                //自身がランキングに登録されていなければ登録
                if(!_ranking.ContainsKey(CurrentName))
                {
                    _ranking.Add(CurrentName, CurrentScore);
                }
                else
                {
                    _ranking[CurrentName] = Math.Max(CurrentScore, _ranking[CurrentName]);
                }

                //ランキングを得点の降順にソート
                _ranking = _ranking.OrderByDescending(r => r.Value).ToDictionary(r => r.Key, r => r.Value);

                //スコアでグループ化
                var scoreGroup = _ranking.GroupBy(r => r.Value);

                //11個目のグループがあれば、その中身の名前をランキングから全削除
                if(scoreGroup.Count() > 10)
                {
                    foreach(var name in scoreGroup.Last().Select(s => s.Key).ToArray())
                    {
                        _ranking.Remove(name);
                    }
                }
            }
        }

        /// <summary>
        /// ランキングの辞書から順位を含む構造体のリストを作る
        /// </summary>
        /// <returns>各名前・順位・得点のリスト</returns>
        private static List<RankingElement> GetRanking()
        {
            var rankingList = new List<RankingElement>();
            var scoreGroup = _ranking.GroupBy(r => r.Value);
            var rank = 1;

            //グループの数だけループ
            for(int i = 0; i < scoreGroup.Count(); i++)
            {
                var sameScores = scoreGroup.ElementAt(i);

                //得点が同じグループは全て同じ順位にしてリストに追加
                foreach(var s in sameScores)
                {
                    rankingList.Add(new(rank, s.Key, s.Value));
                }

                //現在見ていたグループの内容数分順位を上げる
                rank += sameScores.Count();
            }

            return rankingList;
        }
    }

    /// <summary>
    /// ランキングの各要素を整理するための構造体
    /// </summary>
    public struct RankingElement
    {
        public int rank;
        public string name;
        public int score;

        public RankingElement(int rank, string name, int score)
        {
            this.rank = rank;
            this.score = score;
            this.name = new(name);
        }
    }
}