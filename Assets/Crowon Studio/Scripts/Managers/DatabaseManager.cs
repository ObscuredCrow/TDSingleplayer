using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    private static DatabaseManager _instance;
    public static DatabaseManager Instance { get { return _instance; } }

    [Header("Config")]
    [SerializeField] private string address = "127.0.0.1";
    [SerializeField] private string user = "projectvoid";
    [SerializeField] private string pass = "10201990";
    [SerializeField] private string schema = "void";

    private MySQL mysql = new MySQL();

    private void Awake() => _instance = _instance ?? this;

    private void Start()
    {
        mysql.Connect(address, user, pass, schema);
        mysql.CreateTable<Logs>();
        mysql.CreateTable<Profanity>();
        mysql.CreateTable<Accounts>();
    }

    private void OnApplicationQuit() => mysql.Close();

    public bool AccountExists(string id) => mysql.Select<Accounts>(new[] { "SteamId" }, false, new[] { id }).Count > 0;

    public bool VerifyAccount(Accounts account)
    {
        bool exists = AccountExists(account.SteamId);
        if (!exists) mysql.Insert(account);
        if (!account.SteamVacBanned)
        {
            if (exists) account = mysql.Select<Accounts>(new[] { "SteamId" }, false, new[] { account.SteamId })[0];
            if (!account.Banned)
            {
                account.Online = true;
                account.LastLogin = System.DateTime.Now;
                mysql.Replace(account);
                GameManager.Instance.account = account;
                return true;
            }
        }
        else mysql.Replace(account);
        return false;
    }

    public void DisconnectAccount(Accounts account)
    {
        account.Online = false;
        mysql.Replace(account);
    }

    public void Log(string message) => mysql.Insert(new Logs { Date = System.DateTime.Now, Message = message });

    public List<Profanity> ProfanityWords() => mysql.Select<Profanity>();
}