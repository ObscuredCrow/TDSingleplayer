using System;

public class Accounts
{
    [PrimaryKey, LimitKey(17)] public string SteamId;
    public bool SteamVacBanned;
    public bool Banned;
    public DateTime Created;
    public DateTime LastLogin;
    public bool Online;
}