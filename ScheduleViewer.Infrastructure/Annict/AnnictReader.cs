using System.Text.Json;

namespace ScheduleViewer.Infrastructure.Annict;

/// <summary>
/// Annict - 読込
/// </summary>
/// <remarks>
/// GraphQL APIを用いてアニメ情報を取得する。
/// プロパティ名は固定なので、別クラスに移動したり変更しないこと！
/// </remarks>
public sealed class AnnictReader
{
    /// <summary>
    /// レスポンス
    /// </summary>
    /// <remarks>
    /// デシリアライズ結果の格納用
    /// </remarks>
    public class AnnictResponse
    {
        /// <summary> Workデータ検索用 </summary>
        public SearchWorksData Data { get; set; } = new();
    }

    /// <summary>
    /// Workデータ検索用
    /// </summary>
    /// <remarks>
    /// Workリストの格納元
    /// </remarks>
    public class SearchWorksData
    {
        /// <summary> Works </summary>
        public SearchWorks SearchWorks { get; set; } = new();
    }

    /// <summary>
    /// Works
    /// </summary>
    /// <remarks>
    /// Workを持つリスト
    /// </remarks>
    public class SearchWorks
    {
        public List<WorkNode> Nodes { get; set; } = new();
    }

    /// <summary>
    /// Work - Node
    /// </summary>
    /// <remarks>
    /// アニメ情報のノード
    /// </remarks>
    public class WorkNode
    {
        /// <summary> ID </summary>
        /// <remarks> 基本内部でしか使わないはず </remarks>
        public int AnnictId { get; set; }

        /// <summary> タイトル </summary>
        public string? Title { get; set; }

        /// <summary> 制作年 </summary>
        public int? SeasonYear { get; set; }
        
        /// <summary> 制作シーズン </summary>
        public string? SeasonName { get; set; }

        /// <summary> 公式サイト </summary>
        public string? OfficialSiteUrl { get; set; }

        /// <summary> Wikipedia URL </summary>
        public string? WikipediaUrl { get; set; }

        /// <summary> エピソード数 </summary>
        public int? EpisodesCount { get; set; }

        /// <summary> 画像 </summary>
        public WorkImage? Image { get; set; }

        /// <summary> キャスト </summary>
        public Casts? Casts { get; set; }
    }

    /// <summary>
    /// Work - 画像
    /// </summary>
    /// <remarks>
    /// 各画像URLがない場合はnullが返る。
    /// </remarks>
    public class WorkImage
    {
        /// <summary> サムネイル向けURL </summary>
        public string? RecommendedImageUrl { get; set; }

        /// <summary> TwitterのアバターURL </summary>
        public string? TwitterAvatarUrl { get; set; }
        
        /// <summary> 内部画像データ </summary>
        public string? InternalUrl { get; set; }
    }

    /// <summary>
    /// キャスト
    /// </summary>
    public class Casts
    {
        /// <summary> キャストの一覧 </summary>
        public List<CastNode> Nodes { get; set; } = new();
    }

    /// <summary>
    /// キャスト - Node
    /// </summary>
    public class CastNode
    {
        /// <summary> 役名（「◯◯役」など） </summary>
        public string? Name { get; set; }

        /// <summary> 担当キャラクター </summary>
        /// <remarks> Nameと同じ </remarks>
        public Character? Character { get; set; }

        /// <summary> 声優 </summary>
        public Person? Person { get; set; }
    }

    /// <summary>
    /// キャスト - 担当キャラクター
    /// </summary>
    public class Character
    {
        /// <summary> 担当キャラクター名 </summary>
        public string? Name { get; set; }
    }

    /// <summary>
    /// キャスト - 声優
    /// </summary>
    public class Person
    {
        /// <summary> 声優名 </summary>
        public string? Name { get; set; }
    }

    private static readonly HttpClient _client = new HttpClient();

    /// <summary>
    /// 取得
    /// </summary>
    /// <param name="title">タイトル</param>
    /// <param name="first">取得したいデータ数</param>
    /// <param name="castFirst">取得したいキャスト情報数</param>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>アニメ情報の一覧</returns>
    /// <remarks>
    /// 検索条件に一致するアニメ情報を取得する。
    /// </remarks>
    public static async Task<IReadOnlyList<AnimeEntity>> FetchAsync(
        string title,
        int first = 5,
        int castFirst = 10,
        CancellationToken ct = default)
    {
        var query = @"
query ($titles: [String!], $first: Int, $castFirst: Int) {
  searchWorks(titles: $titles, first: $first) {
    nodes {
      title
      seasonName
      seasonYear
      officialSiteUrl
      wikipediaUrl
      episodesCount
      image {
        recommendedImageUrl
        twitterAvatarUrl
        internalUrl(size: ""large"")
      }
      casts(first: $castFirst, orderBy: { field: SORT_NUMBER, direction: ASC }) {
        nodes {
          name              # 例: 役名（「◯◯役」）
          character { name }# キャラ名
          person { name }   # 声優名
        }
      }
    }
  }
}";

        var payload = new
        {
            query,
            variables = new { titles = new[] { title }, first }
        };

        string json = System.Text.Json.JsonSerializer.Serialize(payload);

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.annict.com/graphql");
        request.Headers.Add("Authorization", $"Bearer {Shared.Annict_Token}");
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _client.SendAsync(request).ConfigureAwait(false);
        string result = await response.Content.ReadAsStringAsync();

        Console.WriteLine("=== API Response ===");
        Console.WriteLine(result);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        AnnictResponse? obj = System.Text.Json.JsonSerializer.Deserialize<AnnictResponse>(result, options);

        var animeEntities = new List<AnimeEntity>();

        if (obj != null)
        {
            foreach (var anime in obj.Data.SearchWorks.Nodes)
            {
                var animeEntity = new AnimeEntity(anime.Title, 
                                                  anime?.SeasonName, 
                                                  anime.SeasonYear, 
                                                  anime?.OfficialSiteUrl, 
                                                  anime?.WikipediaUrl, 
                                                  anime?.EpisodesCount,
                                                  AnnictReader.ConvertCastsToString(anime.Casts?.Nodes), 
                                                  anime.Image?.RecommendedImageUrl,
                                                  0, 
                                                  string.Empty,
                                                  string.Empty,
                                                  string.Empty);

                animeEntities.Add(animeEntity);

                Console.WriteLine($"{anime.Title} ({anime.SeasonYear} {anime.SeasonName}) {anime.OfficialSiteUrl}");
             }
        }

        return animeEntities;
    }

    /// <summary>
    /// キャスト情報を文字列に変換する
    /// </summary>
    /// <param name="casts">キャスト情報</param>
    /// <returns>キャスト情報(文字列)</returns>
    private static string ConvertCastsToString(List<CastNode> casts)
    {
        var list = new List<string>();

        foreach(var cast in casts)
        {
            var character  = cast?.Character?.Name;
            var voiceActor = cast?.Person?.Name;

            list.Add($"{character}:{voiceActor}");
        }

        return string.Join("／", list);
    }
}
