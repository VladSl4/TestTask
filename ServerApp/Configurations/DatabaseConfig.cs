namespace ServerApp.Configurations;

public class DatabaseConfig
{
    public string InitialCatalog {  get; set; }

    public string DataSource { get; set; }

    public bool IntegratedSecurity { get; set; }
}

