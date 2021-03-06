﻿using Discord;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

public static class SqlService
{
    private static SqlConnection conn = new SqlConnection(
        "server=sql9.freesqldatabase.com,3306;" +
        "database=sql9172400;" +
        "uid=sql9172400;" +
        "pwd=ZxgDeFHSlP;");

    public static string GetProfile(int _id)
    {
        Console.WriteLine("Nomnom is using this.");
        using (conn)
        {
            try
            {
                conn.OpenAsync();
            }
            catch(SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("Opened");
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM players WHERE id = ?id";
                cmd.Parameters.Add("?id", SqlDbType.VarChar).Value = _id;

                SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    reader.Close();
                    conn.Close();
                    return "Wut";
                }
                Console.WriteLine("Reading");
                while (reader.Read())
                {
                    return reader["id"].ToString();
                }
                Console.WriteLine("After Read");
                reader.Close();
                conn.Close();
            }
        }
        return "No User Found";
    }
    public static void UploadUser(User _user)
    {
        Console.WriteLine("Nomnom is using this");
        using (conn)
        {
            Console.WriteLine("Using cmd");
            try
            {
                conn.OpenAsync();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("Using conn");
            using (SqlCommand cmd = conn.CreateCommand())
            {
                Console.WriteLine("Opened cmd");

                cmd.CommandText = @"INSERT INTO players(name, id, xp, level, karma) VALUES(?name, ?id, ?xp, ?level, ?karma)";
                cmd.Parameters.Add("?name", SqlDbType.Text).Value = SnipUserId(_user.Name);
                cmd.Parameters.Add("?id", SqlDbType.Int).Value = _user.Id;
                cmd.Parameters.Add("?xp", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("?level", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("?karma", SqlDbType.Int).Value = 0;

                Console.WriteLine("Before execute");

                cmd.ExecuteNonQueryAsync();

                Console.WriteLine("After execute");

                conn.Close();

                Console.WriteLine("Added the user " + SnipUserId(_user.Name) + " to the db");
            }
        }
    }
    public static string SnipUserId(string _name)
    {
        return _name.Substring(0, _name.Length - 5);
    }
}