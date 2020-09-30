using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace com.pareo.maruyamaAya.Code.Utils
{
    class SqlHelper
    {
        public static string GetSqlConnectionString()
        {
            String database_ip = Tools.GetValue("DB_PARA","server");
            String database_name = Tools.GetValue("DB_PARA","name");
            String database_username = Tools.GetValue("DB_PARA","user");
            String database_password = Tools.GetValue("DB_PARA","passwd");
            return "Data Source=" + database_ip + ";Initial Catalog=" + database_name + ";User ID=" + database_username + ";Password=" + Tools.DecodeBase64("utf-8",database_password);
        }
        /// <summary>
        /// 增删改操作，返回影响条数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回影响条数</returns>
        public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(GetSqlConnectionString()))
            {
                using (SqlCommand comm = conn.CreateCommand())
                {
                    conn.Open();
                    comm.CommandText = sql;
                    comm.Parameters.AddRange(parameters);
                    return comm.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// 查询操作，返回查询结果中的第一行第一列的值
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回查询结果中的第一行第一列的值</returns>
        public static object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(GetSqlConnectionString()))
            {
                using (SqlCommand comm = conn.CreateCommand())
                {
                    conn.Open();
                    comm.CommandText = sql;
                    comm.Parameters.AddRange(parameters);
                    return comm.ExecuteScalar();
                }
            }
        }
        /// <summary>
        /// 查询操作，返回DataTable
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回DataTable</returns>
        public static DataTable ExecuteDataTable(string sql, params SqlParameter[] parameters)
        {
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, GetSqlConnectionString()))
            {
                DataTable dt = new DataTable();
                adapter.SelectCommand.Parameters.AddRange(parameters);
                adapter.Fill(dt);
                return dt;
            }
        }
        /// <summary>
        /// 查询操作，返回DataSet
        /// </summary>
        /// <param name="sql">Sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>返回DataSet</returns>
        public static DataSet ExecuteDataSet(string sql,params SqlParameter[] parameters)
        {
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, GetSqlConnectionString()))
            {
                DataSet ds = new DataSet();
                adapter.SelectCommand.Parameters.AddRange(parameters);
                adapter.Fill(ds);
                return ds;
            }
        }
    }
}
