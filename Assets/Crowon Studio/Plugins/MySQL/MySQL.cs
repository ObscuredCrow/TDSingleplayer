using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/* Example Table Class
 * public class Accounts
 * {
 *     [PrimaryKey, LimitKey(17)] public string SteamID;
 *     public bool VacBanned;
 * }
 */

public class MySQL
{
    private MySqlConnection connection;

    public void Connect(string address, string username, string password, string database)
    {
        connection = new MySqlConnection(@"server=" + address + ";userid=" + username + ";password=" + password + ";database=" + database);
        connection.Open();
    }

    public void Close() => connection.Close();

    public void CreateTable<T>(bool debug = false)
    {
        string table = typeof(T).Name;
        string fields = string.Empty;
        int count = 0;
        foreach (FieldInfo field in typeof(T).GetFields())
        {
            bool hasPrimary = false;
            bool hasUnique = false;
            bool hasNotNull = false;
            int limitKey = 0;
            object defKey = null;

            System.Attribute[] attrs = System.Attribute.GetCustomAttributes(field);
            foreach (System.Attribute attr in attrs)
            {
                if (attr is PrimaryKey && !hasUnique && field.FieldType.Name != "Boolean") hasPrimary = true;
                if (attr is UniqueKey && !hasPrimary && field.FieldType.Name != "Boolean") hasUnique = true;
                if (attr is NotNullKey) hasNotNull = true;
                if (attr is LimitKey && field.FieldType.Name != "Boolean") limitKey = ((LimitKey)attr).GetValue();
                if (attr is DefaultKey) defKey = ((DefaultKey)attr).GetValue();
            }

            string primary = hasPrimary ? " PRIMARY KEY" : "";
            string unique = hasUnique ? " UNIQUE" : "";
            string notNull = hasNotNull ? " NOT NULL" : "";
            string limit = limitKey != 0 ? $"({limitKey})" : "";
            string def = defKey != null ? $" DEFAULT '{defKey}'" : "";
            string fieldName = GetSqlField(field.FieldType.Name);
            string fieldType = fieldName == "VARCHAR" && limitKey == 0 ? "VARCHAR(255)" :
                fieldName == "TINYINT(1)" && def == "" ? "TINYINT(1) DEFAULT '0'" :
                fieldName;

            if (count == 0) fields += $"{field.Name} {fieldType}{limit}{def}{primary}{unique}{notNull}";
            else fields += $",{field.Name} {fieldType}{limit}{def}{primary}{unique}{notNull}";

            count++;
        }

        if (debug) Debug.Log($"CREATE TABLE IF NOT EXISTS {table}({fields})");
        new MySqlCommand($"CREATE TABLE IF NOT EXISTS {table}({fields})", connection).ExecuteNonQuery();
    }

    public void DropTable<T>(bool debug = false)
    {
        if (debug) Debug.Log($"DROP TABLE IF EXISTS {typeof(T).Name}");
        new MySqlCommand($"DROP TABLE IF EXISTS {typeof(T).Name}", connection).ExecuteNonQuery();
    }

    public void Index<T>(string[] columns, bool debug = false)
    {
        string table = typeof(T).Name;
        string index = table + "_" + string.Join("_", columns);
        string value = string.Join(",", columns);

        bool hasUnique = false;
        MethodBase method = MethodBase.GetCurrentMethod();
        UniqueKey attr = (UniqueKey)method.GetCustomAttribute(typeof(UniqueKey), true);
        if (attr is UniqueKey) hasUnique = true;
        string uniqueText = hasUnique ? "UNIQUE " : "";

        if (debug) Debug.Log($"CREATE {uniqueText}INDEX {index} ON {table} ({value})");
        string returnCheck = new MySqlCommand($"SELECT COUNT(1) IndexIsThere FROM INFORMATION_SCHEMA.STATISTICS WHERE table_schema=DATABASE() AND table_name='{table}' AND index_name='{index}';", connection).ExecuteScalar().ToString();
        if (returnCheck == "0")
            new MySqlCommand($"CREATE {uniqueText}INDEX {index} ON {table} ({value})", connection).ExecuteNonQuery();
    }

    public List<T> Select<T>(bool debug = false) where T : new()
    {
        string table = typeof(T).Name;
        MySqlCommand command = new MySqlCommand($"SELECT * FROM {table}", connection);
        if (debug) Debug.Log(command.CommandText);

        List<T> recieved = new List<T>();
        MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            object values = new T();
            FieldInfo[] fields = typeof(T).GetFields();
            for (int i = 0; i < reader.FieldCount; i++)
                fields[i].SetValue(values, reader.GetValue(i));
            recieved.Add((T)values);
        }

        reader.Close();
        return recieved;
    }

    public List<T> Select<T>(string[] columns, bool debug = false, params object[] args) where T : new()
    {
        string table = typeof(T).Name;
        string where = GetWhere(columns);
        MySqlCommand command = new MySqlCommand($"SELECT * FROM {table} WHERE {where}", connection);

        command = SetParams(command, CreateParams(columns, args));
        if (debug) Debug.Log(command.CommandText);

        List<T> recieved = new List<T>();
        MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            object values = new T();
            FieldInfo[] fields = typeof(T).GetFields();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (fields[i].FieldType == typeof(Vector3)) fields[i].SetValue(values, reader.GetValue(i).ToString().ToVector3());
                else if (fields[i].FieldType == typeof(Quaternion)) fields[i].SetValue(values, reader.GetValue(i).ToString().ToQuaternion());
                else fields[i].SetValue(values, reader.GetValue(i));
            }
            recieved.Add((T)values);
        }

        reader.Close();
        return recieved;
    }

    public void Insert(object obj, bool debug = false) => InsertReplace(obj, false, debug);

    public void Replace(object obj, bool debug = false) => InsertReplace(obj, true, debug);

    private void InsertReplace(object obj, bool replace = false, bool debug = false)
    {
        string table = obj.GetType().Name;
        int count = 0;
        int paramCount = obj.GetType().GetFields().Length;
        string[] columns = new string[paramCount];
        object[] args = new object[paramCount];
        foreach (FieldInfo field in obj.GetType().GetFields())
        {
            string fieldName = GetSqlField(field.FieldType.Name);
            string column = field.Name;
            var variable = field.GetValue(obj);
            columns[count] = column;
            args[count] = variable;

            count++;
        }

        string fields = string.Join(",", columns);
        string values = string.Join(",", columns.Select(e => "@" + e));
        string cmd = replace ? "REPLACE" : "INSERT";
        Query($"{cmd} INTO {table}({fields}) VALUES({values})", columns, debug, args);
    }

    public void Delete<T>(string[] columns, bool debug = false, params object[] args)
    {
        string table = typeof(T).Name;
        string where = GetWhere(columns);
        Query($"DELETE FROM {table} WHERE {where}", columns, debug, args);
    }

    public int Count<T>(bool debug = false)
    {
        string table = typeof(T).Name;
        return int.Parse(Query($"SELECT COUNT(*) FROM {table}", debug, true));
    }

    public int Count<T>(string[] columns, bool debug = false, params object[] args)
    {
        string table = typeof(T).Name;
        string where = GetWhere(columns);
        return int.Parse(Query($"SELECT COUNT(*) FROM {table} WHERE {where}=@{where}", CreateParams(columns, args), debug));
    }

    public void Query(string query, string[] columns, bool debug = false, params object[] args)
    {
        MySqlCommand command = new MySqlCommand(query, connection);
        command = SetParams(command, CreateParams(columns, args));
        if (debug) Debug.Log(command.CommandText);
        command.ExecuteNonQuery();
    }

    public void Query(string query, bool debug = false)
    {
        MySqlCommand command = new MySqlCommand(query, connection);
        if (debug) Debug.Log(command.CommandText);
        command.ExecuteNonQuery();
    }

    public string Query(string query, bool debug = false, bool allowReturn = false)
    {
        MySqlCommand command = new MySqlCommand(query, connection);
        if (debug) Debug.Log(command.CommandText);
        return command.ExecuteScalar().ToString();
    }

    public string Query(string query, List<MySqlParameter> args, bool debug = false)
    {
        MySqlCommand command = new MySqlCommand(query, connection);
        command = SetParams(command, args);
        if (debug) Debug.Log(command.CommandText);
        return command.ExecuteScalar().ToString();
    }

    private string GetWhere(string[] columns)
    {
        string where = string.Empty;
        for (int i = 0; i < columns.Length; i++)
        {
            if (i == 0) where += $"{columns[i]}=@{columns[i]}";
            else if (i == columns.Length - 1) where += $"AND {columns[i]}=@{columns[i]}";
            else where += $"AND {columns[i]}=@{columns[i]}";
        }
        return where;
    }

    private string GetSqlField(string name)
    {
        switch (name)
        {
            case "String": case "Vector3": case "Quaternion": return "VARCHAR";
            case "Int32": return "INT";
            case "Single": return "FLOAT";
            case "Int64": return "BIGINT";
            case "Boolean": return "TINYINT(1)";
            case "Double": return "DOUBLE";
            case "DateTime": return "DATETIME";
        }

        return null;
    }

    private List<MySqlParameter> CreateParams(string[] columns, params object[] args)
    {
        List<MySqlParameter> sqlParams = new List<MySqlParameter>();
        for (int i = 0; i < args.Length; i++)
            sqlParams.Add(new MySqlParameter($"@{columns[i]}", args[i]));

        return sqlParams;
    }

    private MySqlCommand SetParams(MySqlCommand command, List<MySqlParameter> sqlParams)
    {
        foreach (MySqlParameter sqlParam in sqlParams)
            command.Parameters.Add(sqlParam);

        return command;
    }
}

public class PrimaryKey : System.Attribute
{ }

public class UniqueKey : System.Attribute
{ }

public class NotNullKey : System.Attribute
{ }

public class LimitKey : System.Attribute
{
    private int value;

    public LimitKey(int value) => this.value = value;

    public int GetValue() => value;
}

public class DefaultKey : System.Attribute
{
    private object value;

    public DefaultKey(int value) => this.value = value;

    public object GetValue() => value;
}