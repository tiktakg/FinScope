// CandelsRoot myDeserializedClass = JsonConvert.DeserializeObject<List<CandelsRoot>>(myJsonResponse);
using System.Collections.Generic;

public class Charsetinfo
{
    public string name { get; set; }
}

public class Dataversion
{
    public int data_version { get; set; }
    public long seqnum { get; set; }
    public string trade_date { get; set; }
    public string trade_session_date { get; set; }
}

public class Marketdatum
{
    public string SECID { get; set; }
    public double? LAST { get; set; }
    public double LASTCHANGE { get; set; }
    public double LASTCHANGEPRCNT { get; set; }
    public long VOLTODAY { get; set; }
}

public class Root
{
    public Charsetinfo charsetinfo { get; set; }
    public List<Security> securities { get; set; }
    public List<Marketdatum> marketdata { get; set; }
    public List<Dataversion> dataversion { get; set; }
    public List<object> marketdata_yields { get; set; }
}

public class Security
{
    public string SECID { get; set; }
    public string SECNAME { get; set; }
    public object SECTORID { get; set; }
}

