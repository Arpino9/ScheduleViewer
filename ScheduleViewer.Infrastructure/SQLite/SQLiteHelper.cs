namespace ScheduleViewer.Infrastructure.SQLite;

/// <summary>
/// SQLite - ヘルパークラス
/// </summary>
public class SQLiteHelper
{
    /// <summary>
    /// SQLiteの接続文字列を取得する
    /// </summary>
    /// <returns>接続文字列</returns>
    internal static string GetConnectionString()
        => ($"Data Source={Shared.DatabasePath};Version=3;");

    /// <summary>
    /// クエリ実行
    /// </summary>
    /// <typeparam name="T">型パラメーター</typeparam>
    /// <param name="sql">SQL文</param>
    /// <param name="createEntity">読み込まれるエンティティ</param>
    /// <returns>レコード(複数行)</returns>
    /// <remarks>
    /// 読み込まれるエンティティを引数で指定する。
    /// </remarks>
    internal static IReadOnlyList<T> Query<T>(
    string sql,
        Func<SQLiteDataReader, T> createEntity)
    {
        var result = new List<T>();

        try
        {
            using (var connection = new SQLiteConnection(SQLiteHelper.GetConnectionString()))
            using (var command = new SQLiteCommand(sql, connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(createEntity(reader));
                    }
                }
            }
        } 
        catch (Exception ex)
        {
            new DatabaseException("SQLiteの読込に失敗しました。", ex);
        }
        
        return result;
    }

    /// <summary>
    /// クエリ実行
    /// </summary>
    /// <typeparam name="T">型パラメーター</typeparam>
    /// <param name="sql">SQL文</param>
    /// <param name="createEntity">読み込まれるエンティティ</param>
    /// <param name="nullEntity">nullエンティティ</param>
    /// <returns>レコード(単数行)</returns>
    /// <remarks>
    /// パラメータがない場合に使う。
    /// </remarks>
    internal static T QuerySingle<T>(
    string sql,
        Func<SQLiteDataReader, T> createEntity,
        T nullEntity)
    {
        return QuerySingle<T>(sql, null, createEntity, nullEntity);
    }

    /// <summary>
    /// クエリ実行
    /// </summary>
    /// <typeparam name="T">型パラメーター</typeparam>
    /// <param name="sql">SQL文</param>
    /// <param name="parameters">パラメーター</param>
    /// <param name="createEntity">読み込まれるエンティティ</param>
    /// <param name="nullEntity">nullエンティティ</param>
    /// <returns>レコード(単数行)</returns>
    /// <remarks>
    /// 該当する1レコードのみを取得したい場合に使う。
    /// エンティティクラスには引数0のコンストラクターがないため、
    /// 返すレコードがない場合はnullを返却している。
    /// </remarks>
    internal static T QuerySingle<T>(
        string sql,
    SQLiteParameter[] parameters,
        Func<SQLiteDataReader, T> createEntity,
        T nullEntity)
    {
        using (var connection = new SQLiteConnection(SQLiteHelper.GetConnectionString()))
        using (var command = new SQLiteCommand(sql, connection))
        {
            connection.Open();
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    return createEntity(reader);
                }
            }
        }

        return nullEntity;
    }

    /// <summary>
    /// クエリ実行
    /// </summary>
    /// <typeparam name="T">型パラメーター</typeparam>
    /// <param name="sql">SQL文</param>
    /// <param name="parameters">パラメーター</param>
    /// <param name="createEntity">読み込まれるエンティティ</param>
    /// <returns>エンティティ</returns>
    /// <remarks>
    /// null安全版。なるべくnullEntityありを使いたいところ。
    /// </remarks>
    internal static IReadOnlyList<T> QueryPlural<T>(
    string sql,
    SQLiteParameter[] parameters,
        Func<SQLiteDataReader, T> createEntity)
    {
        var result = new List<T>();

        using (var connection = new SQLiteConnection(SQLiteHelper.GetConnectionString()))
        using (var command = new SQLiteCommand(sql, connection))
        {
            connection.Open();
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(createEntity(reader));
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 実行
    /// </summary>
    /// <param name="sql">SQL文</param>
    /// <param name="parameters">パラメーター</param>
    /// <remarks>
    /// SQLを単体実行したい時に使う。なるべくTransaction版を検討すること。
    /// </remarks>
    internal static void Execute(
    string sql,
    SQLiteParameter[] parameters
    )
    {
        using (var connection = new SQLiteConnection(SQLiteHelper.GetConnectionString()))
        using (var command = new SQLiteCommand(sql, connection))
        {
            connection.Open();
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            command.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// 実行
    /// </summary>
    /// <param name="insert">Insert文</param>
    /// <param name="update">Update文</param>
    /// <param name="parameters">パラメーター</param>
    /// <remarks>
    /// Update文が実行できなかった(対応する行がない)場合にInsert文を実行する。
    /// </remarks>
    internal static void Execute(
        string insert,
    string update,
        SQLiteParameter[] parameters)
    {
        using (var connection = new SQLiteConnection(SQLiteHelper.GetConnectionString()))
        using (var command = new SQLiteCommand(update, connection))
        {
            connection.Open();

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            if (command.ExecuteNonQuery() < 1)
            {
                command.CommandText = insert;
                command.ExecuteNonQuery();
            }
        }
    }
}