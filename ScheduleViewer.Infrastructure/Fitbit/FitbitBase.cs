using System.Reflection;

namespace ScheduleViewer.Infrastructure.Fitbit;

/// <summary>
/// Fitbit Base
/// </summary>
internal abstract class FitbitBase
{
    /// <summary> クラス名 </summary>
    private static string ClassName => MethodBase.GetCurrentMethod().DeclaringType.Name;

    /// <summary> アクセストークン </summary>
    private string _accessToken = string.Empty;

    /// <summary> リフレッシュトークン </summary>
    private string _refreshToken = string.Empty;

    /// <summary> アクセストークンの有効期限 </summary>
    private DateTime _accessTokenExpiry = DateTime.Now;

    /// <summary> スコープ </summary>
    private string[] _scopes = { "activity", "irregular_rhythm_notifications", "profile", "social", "cardio_fitness", "location", "respiratory_rate", "temperature", "electrocardiogram", "nutrition", "settings", "weight", "heartrate", "oxygen_saturation", "sleep" };

    /// <summary> 認証URL </summary>
    private string _authorizationUri = "https://www.fitbit.com/oauth2/authorize";

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns>void</returns>
    /// <remarks>
    /// デフォルトブラウザで認証URLを開き、認証を開始する。
    /// </remarks>
    internal async Task Initialize()
    {
        string codeVerifier  = StringUtils.Encode(128);
        string codeChallenge = codeVerifier.ToHash();

        LogUtils.Debug(ClassName, "エンコード文字列: " + codeChallenge);

        if (string.IsNullOrEmpty(Shared.Fitbit_ClientId))
        {
            LogUtils.Error(ClassName, "クライアントIDが未設定です。");
        }

        if (string.IsNullOrEmpty(Shared.Fitbit_RedirectUri))
        {
            LogUtils.Error(ClassName, "リダイレクトURLが未設定です。");
        }

        Uri authUrl = new Uri($"{_authorizationUri}?client_id={Shared.Fitbit_ClientId}&response_type=code&redirect_uri={Shared.Fitbit_RedirectUri}&scope={string.Join("+", _scopes)}&code_challenge={codeChallenge}&code_challenge_method=S256");

        LogUtils.Debug(ClassName, "認証URL: " + authUrl);

        var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = authUrl.ToString(),
            UseShellExecute = true
        });

        await Authorize(codeVerifier);
    }

    /// <summary>
    /// 認証
    /// </summary>
    /// <param name="codeVerifier">コード認証文字</param>
    /// <returns>void</returns>
    /// <remarks>
    /// HttpListenerでローカルサーバーを起動してコールバックを待機する。
    /// </remarks>
    private async Task Authorize(string codeVerifier)
    {
        using (var listener = new HttpListener())
        {
            listener.Prefixes.Add(Shared.Fitbit_RedirectUri);
            listener.Start();

            // リクエストを1回受信
            var context = await listener.GetContextAsync();
            var code = context.Request.QueryString["code"];
            
            if (string.IsNullOrEmpty(code))
            {
                LogUtils.Error(ClassName, "認証コードが取得できませんでした");
                listener.Stop();

                return;
            }

            LogUtils.Debug(ClassName, $"認証コード: {code}");

            // レスポンスを返す
            context.Response.ContentType = "text/html";
            using (var writer = new System.IO.StreamWriter(context.Response.OutputStream, Encoding.UTF8))
            {
                writer.WriteLine("<html><body>認証成功、このウィンドウは閉じてください。.</body></html>");
            }

            context.Response.Close();

            await FetchTokensFromCodeAsync(code, codeVerifier);

            listener.Stop();
        }
    }

    /// <summary>
    /// コードからアクセストークンを取得する。
    /// </summary>
    /// <param name="code">コード</param>
    /// <param name="codeVerifier">コード認証用の文字列</param>
    /// <returns>void</returns>
    private async Task FetchTokensFromCodeAsync(string code, string codeVerifier)
    {
        // トークンリクエストの内容を設定
        var requestData = new Dictionary<string, string>
            {
                { "client_id", Shared.Fitbit_ClientId },
                { "grant_type", "authorization_code" },
                { "redirect_uri", Shared.Fitbit_RedirectUri },
                { "code", code },
                { "code_verifier", codeVerifier }
            };

        await this.GetTokensAsync(requestData);
    }

    /// <summary>
    /// アクセストークンを再取得する。
    /// </summary>
    /// <param name="refreshToken">リフレッシュトークン</param>
    /// <returns>void</returns>
    private async Task RefreshAccessTokenAsync(string refreshToken)
    {
        var requestData = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken }
        };

        await this.GetTokensAsync(requestData);
    }

    /// <summary>
    /// トークンを取得する
    /// </summary>
    /// <param name="requestData">リクエスト</param>
    /// <returns>void</returns>
    private async Task GetTokensAsync(Dictionary<string, string> requestData)
    {
        using (var client = new HttpClient())
        {
            // Basic 認証ヘッダーを設定
            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Shared.Fitbit_ClientId}:{Shared.Fitbit_ClientSecret}"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

            var requestContent = new FormUrlEncodedContent(requestData);

            // POSTリクエストを送信
            var response = await client.PostAsync(Shared.Fitbit_TokenRequestUri, requestContent);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                LogUtils.Error(ClassName, $"トークンの取得に失敗しました: {responseBody}");
                return;
            }

            // JSONレスポンスからトークン情報を取得
            var tokenResponse = JsonConvert.DeserializeObject<JSONProperty_TokenResponce>(responseBody);

            LogUtils.Debug(ClassName, $"アクセストークン: {tokenResponse.AccessToken}");
            LogUtils.Debug(ClassName, $"リフレッシュトークン: {tokenResponse.RefreshToken}");

            _accessToken  = tokenResponse.AccessToken;
            _refreshToken = tokenResponse.RefreshToken;

            _accessTokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
        }
    }

    /// <summary>
    /// フェッチ
    /// </summary>
    /// <param name="endpoint">エンドポイント</param>
    /// <returns>Fitbitデータ</returns>
    /// <exception cref="Exception">アクセストークンが未設定</exception>
    /// <remarks>
    /// 指定されたエンドポイントから、登録されているFitbit情報を取得する。
    /// </remarks>
    protected async Task<string> FetchAsync(string endpoint)
    {
        if (string.IsNullOrEmpty(_accessToken))
        {
            LogUtils.Error(ClassName, "アクセストークンが設定されていません。");

            return string.Empty;
        }

        if (DateTime.UtcNow >= _accessTokenExpiry)
        {
            LogUtils.Info(ClassName, "アクセストークンの有効期限が切れました。");

            await RefreshAccessTokenAsync(_refreshToken);
        }

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

            var response = await client.GetAsync(endpoint);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
