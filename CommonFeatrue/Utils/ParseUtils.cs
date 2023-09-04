namespace CommonFeatrue.Utils
{
    public static class ParseUtils
    {
        /// <summary>
        /// 通用值类型转换
        /// </summary>
        /// <typeparam name="T">转换值类型</typeparam>
        /// <param name="value">被转换的值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>转换后的值或默认值</returns>
        public static T Parse<T>(this object value, T defaultValue = default)
        {
            try
            {
                var type = typeof(T);
                var nullableType = Nullable.GetUnderlyingType(type);
                if (nullableType is not null)
                {
                    return (T)Convert.ChangeType(value, nullableType);
                }
                return (T)Convert.ChangeType(value, type);
            }
            catch
            {
                return default;
            }
        }
    }
}
