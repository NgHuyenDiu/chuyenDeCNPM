using DevExpress.Office.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace deTai1
{
    public partial class WebForm11 : System.Web.UI.Page
    {
        public static List<String> listTableName = new List<string>();
        public static List<String> listColumnName = new List<string>();
        public static List<String> listColumnNameTemp1 = new List<string>();
        public static List<String> listTableNameTemp1 = new List<string>();
        public static List<String> listColumnNameTemp2 = new List<string>();
        public static List<String> listTableNameTemp2 = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.GetTableName();
            }
        }


        protected void CheckBoxListTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<String> temp = new List<string>();

            listTableName.Clear();
            listTableNameTemp2.Clear();

            foreach (ListItem item in CheckBoxListColumn.Items)
            {
                if (item.Selected)
                {
                    temp.Add(item.Text);
                }
            }

            CheckBoxListColumn.Items.Clear();
            listColumnNameTemp2.Clear();

            foreach (ListItem item in CheckBoxListTable.Items)
            {
                if (item.Selected)
                {
                    listTableName.Add(item.Text);
                }
            }

            for (int i = 0; i < listTableName.Count; i++)
            {
                GetColumnName(listTableName[i].ToString());
            }

            for (int i = 0; i < temp.Count; i++)
            {
                ListItem listItem = this.CheckBoxListColumn.Items.FindByText(temp[i].ToString());
                if (listItem != null) listItem.Selected = true;
            }

            foreach (ListItem item in CheckBoxListColumn.Items)
            {
                listColumnNameTemp2.Add(item.Text.ToString());
                listTableNameTemp2.Add(item.Value.ToString());
            }

        }
        private void GetTableName()
        {
            CheckBoxListTable.ClearSelection();
            string lenh = "SELECT name, object_id FROM QLVT_DATHANG.sys.Tables WHERE is_ms_shipped = 0 AND name != 'sysdiagrams'";
            SqlDataReader sdr = ExecSqlDataReader(lenh);

            while (sdr.Read())
            {
                ListItem item = new ListItem();
                item.Text = sdr["name"].ToString();
                item.Value = sdr["object_id"].ToString();
                CheckBoxListTable.Items.Add(item);
                CheckBoxListTable.AutoPostBack = true;
            }
        }

        public static SqlDataReader ExecSqlDataReader(String strLenh)
        {
            String connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlDataReader myReader;
            SqlCommand sqlcmt = new SqlCommand(strLenh, conn);
            sqlcmt.CommandType = System.Data.CommandType.Text;
            sqlcmt.CommandTimeout = 600;
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }
            try
            {
                myReader = sqlcmt.ExecuteReader();
                return myReader;
            }
            catch (SqlException ex)
            {
                conn.Close();
                return null;
            }

        }

        private void GetColumnName(String tableName)
        {
            string query = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tableName + "' AND COLUMN_NAME NOT LIKE 'rowguid%'";
            SqlDataReader sdr = ExecSqlDataReader(query);

            while (sdr.Read())
            {
                ListItem item = new ListItem();

                item.Text = sdr["COLUMN_NAME"].ToString();
                item.Value = tableName.ToString();
                CheckBoxListColumn.Items.Add(item);

            }


        }

        public static DataTable ExecSqlDataTable(String cmd)
        {
            String connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            DataTable dt = new DataTable();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlDataAdapter da = new SqlDataAdapter(cmd, conn);
            da.Fill(dt);
            conn.Close();
            return dt;
        }

        protected void ButtonClearColumn_Click(object sender, EventArgs e)
        {
            CheckBoxListColumn.Items.Clear();
            listColumnName.Clear();
            GridView1.Controls.Clear();
            CheckBoxListTable.Items.Clear();
            this.GetTableName();
        }

        protected void CheckBoxListColumn_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (ListItem item in CheckBoxListColumn.Items)
            {

                if (item.Selected)
                {
                    listColumnNameTemp1.Add(item.Text.ToString());
                    listTableNameTemp1.Add(item.Value.ToString());
                }
            }

            DataTable dt = new DataTable();

            dt.Columns.Add("TenCot", Type.GetType("System.String"));
            dt.Columns.Add("TenBang", Type.GetType("System.String"));

            string[] arrTemp1 = listColumnNameTemp1.ToArray();
            string[] arrTemp2 = listTableNameTemp1.ToArray();

            for (int i = 0; i < arrTemp1.GetLength(0); i++)
            {
                dt.Rows.Add();
                dt.Rows[i]["TenCot"] = arrTemp1[i];
                dt.Rows[i]["TenBang"] = arrTemp2[i];
            }
            listColumnNameTemp1.Clear();
            listTableNameTemp1.Clear();

            GridView1.DataSource = dt;
            GridView1.DataBind();

        }

        protected void ButtonQuery_Click(object sender, EventArgs e)
        {
            string mess = "";

            string tableName = string.Join(", ", listTableName);
            String columnName = "";
            mess = "SELECT ";
            String dk = "";
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                TextBox strBang = new TextBox();
                TextBox strCot = new TextBox();
                TextBox dieuKien = (TextBox)GridView1.Rows[i].Cells[2].FindControl("TextBoxDieuKien");
                if (dieuKien.Text.ToString() != "")
                {
                    strBang.Text = GridView1.Rows[i].Cells[4].Text;
                    strCot.Text = GridView1.Rows[i].Cells[3].Text;
                    dk += " AND " + strBang.Text.ToString() + "." + strCot.Text.ToString() + dieuKien.Text.ToString();
                }

                strBang.Text = GridView1.Rows[i].Cells[4].Text;
                strCot.Text = GridView1.Rows[i].Cells[3].Text;



                columnName = strBang.Text.ToString() + "." + strCot.Text.ToString();

                if (i < GridView1.Rows.Count - 1)
                {
                    columnName += ", ";
                }
                mess += columnName;

            }

            String where = "";
            int w = 0;
            for (int i = 0; i < listColumnNameTemp2.Count - 1; i++)
            {
                for (int j = i + 1; j < listColumnNameTemp2.Count; j++)
                {
                    if (listColumnNameTemp2[j] == listColumnNameTemp2[i])
                    {

                        w++;
                        if (w > 1)
                        {
                            where += " AND " + listTableNameTemp2[i].ToString() + "." + listColumnNameTemp2[i] + " = " + listTableNameTemp2[j].ToString() + "." + listColumnNameTemp2[j];
                        }
                        else
                        {
                            where += listTableNameTemp2[i].ToString() + "." + listColumnNameTemp2[i] + " = " + listTableNameTemp2[j].ToString() + "." + listColumnNameTemp2[j];
                        }
                    }
                }
            }
            if (!where.Equals(""))
            {
                where = " WHERE " + where;
            }

            mess += " FROM " + tableName + where + dk;
            TextBox1.Text = mess;
        }

        protected void btnReport_Click(object sender, EventArgs e)
        {
            String query = TextBox1.Text;
            String title = TextBoxNhapTieuDe.Text;

            Session["query"] = query;
            Session["title"] = title;
            Response.Redirect("WebForm2.aspx");
            Server.Execute("WebForm2.aspx");
        }
    }

}