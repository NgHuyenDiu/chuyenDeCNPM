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
using System.Windows;

namespace deTai1
{
    public partial class WebForm11 : System.Web.UI.Page
    {    
        public static List<Table> listTableQuery = new List<Table>();    
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
            listTableQuery.Clear();
            foreach (ListItem item in CheckBoxListColumn.Items)
            {
                if (item.Selected)
                {
                    temp.Add(item.Text);
                }
            }

            CheckBoxListColumn.Items.Clear();
            foreach (ListItem item in CheckBoxListTable.Items)
            {
                if (item.Selected)
                {
                    
                    Table tb = new Table();
                    tb.tableName = item.Text;
                    tb.Table_Object_id = item.Value;
                    listTableQuery.Add(tb);
                }
            }
            for (int i = 0; i < listTableQuery.Count; i++)
            {
                GetColumnName(listTableQuery[i].tableName);
            }

            for (int i = 0; i < temp.Count; i++)
            {
                ListItem listItem = this.CheckBoxListColumn.Items.FindByText(temp[i].ToString());
                if (listItem != null) listItem.Selected = true;
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

        protected void ButtonClearColumn_Click(object sender, EventArgs e)
        {
            CheckBoxListColumn.Items.Clear();
            GridView1.Controls.Clear();
            CheckBoxListTable.Items.Clear();
            TextBox1.Text = "";
            this.GetTableName();
        }

        protected void CheckBoxListColumn_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Tên Cột", Type.GetType("System.String"));
            dt.Columns.Add("Tên Bảng", Type.GetType("System.String"));
            foreach (ListItem item in CheckBoxListColumn.Items)
            {
                if (item.Selected)
                {
                    dt.Rows.Add(item.Text, item.Value);
                }
            }
            GridView1.DataSource = dt;
            GridView1.DataBind();

        }
        public Boolean isAggregateFunctions()
        {
            Boolean result = false;
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                DropDownList suDung = (DropDownList)GridView1.Rows[i].Cells[1].FindControl("DropDownList1");
                if (suDung.Text.Equals("SUM") || suDung.Text.Equals("COUNT") || suDung.Text.Equals("MIN") || suDung.Text.Equals("MAX") || suDung.Text.Equals("AVG"))
                {
                    result = true;
                }
            }
            return result;
        }

        // add list  foreign key
        public String getForeignKey(String object_id_a, String object_id_b)
        {
            List<String> values = new List<string>();
            String query= "exec sp_TimKhoaNgoai " + object_id_a + ", " + object_id_b;
            SqlDataReader sdr = ExecSqlDataReader(query);
            while (sdr.Read())
            {
                values.Add(sdr["table_name"].ToString() + "." + sdr["column_name"].ToString());
            }
            return String.Join("=", values);
        }




        protected void ButtonQuery_Click(object sender, EventArgs e)
        {
            List<String> conditions = new List<string>();
            string query = "";
            query = "SELECT ";

            string tableName = "";  
            for(int i=0; i< listTableQuery.Count; i++)
            {
                tableName +=listTableQuery[i].tableName +" ";
            }
            tableName = tableName.Trim().Replace(" ",", ");
            
           

            String columnName = "", condition = "",  sort = "", groupBy = "", havings="", where=""; 
           
            // select all column selected
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {              
                DropDownList sapXep = (DropDownList)GridView1.Rows[i].Cells[0].FindControl("DropDownList2");
                DropDownList suDung = (DropDownList)GridView1.Rows[i].Cells[1].FindControl("DropDownList1");
                String strBang = GridView1.Rows[i].Cells[4].Text.ToString();
                String strCot = GridView1.Rows[i].Cells[3].Text.ToString();
                TextBox dieuKien = (TextBox)GridView1.Rows[i].Cells[2].FindControl("TextBoxDieuKien");

                if (dieuKien.Text.ToString() != "")
                {
                    condition += " AND " + dieuKien.Text.ToString();
                }

                if (!suDung.Text.Equals("SELECT") && !suDung.Text.Equals("GROUP BY"))
                {
                    columnName += suDung.Text + "(" + strCot+ ") ";
                }
                else if (suDung.Text.Equals("GROUP BY"))
                {
                    groupBy += " " + suDung.Text + " " + strBang + "." + strCot;
                    columnName += strBang + "." + strCot;
                }
                else
                {
                    columnName += strBang + "." + strCot;
                }

                if (i < GridView1.Rows.Count - 1)
                {
                    columnName += ",";
                }

                // sap xep
                if (!sapXep.Text.Equals("sort"))
                {
                    sort += " ORDER BY " + strBang + "." + strCot + " " + sapXep.SelectedValue.ToString();
                }
            }
            query += columnName;
            Boolean satisfied = true;

            if (isAggregateFunctions() == true && groupBy == "")
            {
                MessageBox.Show( "Vui lòng chọn group by khi thực hiện các hàm SUM, COUNT, MIN, MAX, AVG");
                satisfied = false;
            }
            // dieu kien where
            // them cac dieu kien khoa ngoại
            if (listTableQuery.Count > 1)
            {
               
                String a_id, b_id, relationship;
                for (int i = 0; i < listTableQuery.Count - 1; i++)
                {
                    for (int j = i + 1; j < listTableQuery.Count; j++)
                    {
                        a_id = listTableQuery[i].Table_Object_id;
                        b_id = listTableQuery[j].Table_Object_id;
                        relationship = getForeignKey(a_id, b_id);
                        if (!relationship.Equals(""))
                            conditions.Add(relationship);
                    }
                }
                    
            }
            if (conditions.Count != 0)
                where += " where " + string.Join(" and ", conditions);

            query += " FROM " + tableName + where + condition + sort + groupBy;

            if (satisfied == true)
            {
                TextBox1.Text = query;

            }

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