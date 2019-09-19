using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace Interface
{
    public class Integration
    {
        public void Log(string msg)
        {
            SqlConnection conn = new SqlConnection("Data Source=server-erp2\\tciasms;Initial Catalog=ASMSSAGEINTEGRATION;Integrated Security=True");
            SqlCommand cmd_inv = new SqlCommand();

            cmd_inv.CommandText = "exec sp_Log @msg";
            cmd_inv.Parameters.AddWithValue("@msg", msg);
            cmd_inv.Connection = conn;
            conn.Open();
            cmd_inv.ExecuteNonQuery();
            conn.Close();
        }
    }
}