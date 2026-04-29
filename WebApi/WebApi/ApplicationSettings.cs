namespace WebApi
{
    public class ApplicationSettings
    {
        /// <summary>
        /// Строки подключения к БД
        /// </summary>
        public ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
    }

    public class ConnectionStrings
    {
        /// <summary>
        /// Строка подключения к тестовой БД
        /// </summary>
        public string? TestDBConnections { get; set; }
    }
}