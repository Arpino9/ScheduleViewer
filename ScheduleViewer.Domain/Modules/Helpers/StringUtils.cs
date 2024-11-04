using System.Security.Cryptography;
using System.Text;

namespace ScheduleViewer.Domain.Modules.Helpers;

/// <summary>
/// 拡張クラス - String
/// </summary>
public static class StringUtils
{
    /// <summary> 区切り文字 </summary>
    private static readonly char Delimiter = ',';

    /// <summary>
    /// 区切り文字をつける
    /// </summary>
    /// <param name="list">リスト</param>
    /// <returns>区切り文字付きの文字列</returns>
    public static string Combine(this List<string> list)
    {
        var str = string.Empty;
        foreach (var item in list)
        {
            str += item + StringUtils.Delimiter;
        }

        // 末尾の「,」は除外
        return str.Substring(0, str.Length - 1);
    }

    /// <summary>
    /// 区切り文字ごとにリスト化する
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static List<string> Separate(this string str)
        => str.Split(StringUtils.Delimiter).ToList();

    /// <summary>
    /// null判定
    /// </summary>
    /// <param name="str">文字列</param>
    /// <returns>True : null以外 / False : null</returns>
    public static bool HasValue(this string str)
        => (str != null);

    /// <summary>
    /// null判定
    /// </summary>
    /// <param name="str">文字列</param>
    /// <returns>True : null / False : null以外</returns>
    public static bool IsNull(this string str)
        => (str is null);

    /// <summary>
    /// 文字列のハッシュ化
    /// </summary>
    /// <param name="str">文字列</param>
    /// <returns>暗号化された文字列</returns>
    public static string ToHash(this string str)
    {
        using (var sha256 = SHA256.Create())
        {
            var hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(str));
            return Convert.ToBase64String(hash).Replace("+", "-")
                                               .Replace("/", "_")
                                               .Replace("=", "");
        }
    }

    /// <summary>
    /// エンコード
    /// </summary>
    /// <returns>コード認証文字</returns>
    /// <remarks>[
    /// 任意の文字数で、ランダムな文字列を生成する
    /// </remarks>
    public static string Encode(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";
        
        var random       = new Random();
        var codeVerifier = new char[length];

        for (int i = 0; i < codeVerifier.Length; i++)
        {
            codeVerifier[i] = chars[random.Next(chars.Length)];
        }

        //return new string(codeVerifier).ToHash();
        return new string(codeVerifier);
    }
}
