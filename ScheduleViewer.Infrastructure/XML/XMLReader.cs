namespace ScheduleViewer.Infrastructure.XML;

/// <summary>
/// XML読み込み
/// </summary>
/// <remarks>
/// 指定されたパスのXMLの読み込み、破棄を行う。
/// コンストラクタ呼び出し時にusingを行うため、Repositoryは不要。
/// </remarks>
public sealed class XMLReader : IDisposable
{
    /// <summary> シリアライザー </summary>
    private XmlSerializer _xmlSerializer;

    /// <summary> リーダー </summary>
    private StreamReader _reader;

    public XMLReader(string filePath, Type type)
    {
        try
        {
            if (File.Exists(filePath))
            {
                _xmlSerializer = new XmlSerializer(type);
                _reader = new StreamReader(filePath, new UTF8Encoding(false));
            }
        }
        catch (Exception ex)
        {
            throw new FileWriterException("XMLファイルの取得に失敗しました。", ex);
        }
    }

    /// <summary>
    /// デシリアライズ
    /// </summary>
    /// <returns>デシリアライズ化されたオブジェクト</returns>
    /// <exception cref="FileWriterException">書き込み失敗</exception>
    public object Deserialize()
    {
        try
        {
            return _xmlSerializer?.Deserialize(_reader);
        }
        catch (Exception ex)
        {
            throw new FileReaderException("XMLファイルの読み込みに失敗しました。", ex);
        }
    }

    /// <summary>
    /// 破棄
    /// </summary>
    public void Dispose()
    {
        _reader?.Close();
    }
}