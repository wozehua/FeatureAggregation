using MimeKit;

namespace CommonFeatrue
{
    /// <summary>
    /// 附件模型
    /// </summary>
    public sealed class Attachment : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// 附件类型/MIME，比如txt/csv
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件传输编码方式，默认ContentEncoding.Default
        /// </summary>
        public ContentEncoding ContentTransferEncoding { get; set; } = ContentEncoding.Default;

        /// <summary>
        /// 文件数组
        /// </summary>
        public byte[] Data { get; set; }

        private Stream _stream;

        /// <summary>
        /// 文件数据流，获取数据时优先采用此部分
        /// </summary>
        public Stream Stream
        {
            get
            {
                if (_stream != null || Data == null) return _stream;
                using var stream = new MemoryStream(Data);
                _stream = stream;
                return _stream;
            }
            set => _stream = value;
        }

        /// <summary>
        /// 释放Stream
        /// </summary>
        public void Dispose()
        {
            _stream?.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await _stream.DisposeAsync();
        }
    }
}
