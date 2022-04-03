using DevExpress.Office.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
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

        protected void CheckBoxListTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            // lay cac column dang duoc check
            List<String> temp = new List<string>();
            listTableQuery.Clear();
            foreach (ListItem item in CheckBoxListColumn.Items)
            {
                if (item.Selected)
                {
                    temp.Add(item.Text);
                }
            }

            // xoa checkboxlist hien tai
            CheckBoxListColumn.Items.Clear();

            // lay danh sach table được checked
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
            // duyet danh sâch table duoc chon, lay danh sach column
            for (int i = 0; i < listTableQuery.Count; i++)
            {
                Getselect(listTableQuery[i].tableName);
            }
            // ckeched lai nhưng item column da duoc chon truoc do
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

        private void Getselect(String tableName)
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
        private static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }

        protected void CheckBoxListColumn_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridView grTemp = new GridView();
            if (GridView1.Rows.Count > 0)
            {
                DataTable temp = new DataTable();
                temp.Columns.Add("Sắp xếp", Type.GetType("System.String"));
                temp.Columns.Add("Sử dụng", Type.GetType("System.String"));
                temp.Columns.Add("Điều kiện", Type.GetType("System.String"));
                temp.Columns.Add("Tên cột", Type.GetType("System.String"));
                temp.Columns.Add("Tên Bảng", Type.GetType("System.String"));

                for (int i = 0; i < GridView1.Rows.Count; i++)
                {
                    DropDownList sapXep = (DropDownList)GridView1.Rows[i].Cells[0].FindControl("DropDownListSort");
                    DropDownList suDung = (DropDownList)GridView1.Rows[i].Cells[1].FindControl("DropDownListUse");
                    TextBox DieuKien = (TextBox)GridView1.Rows[i].Cells[2].FindControl("TextBoxDieuKien");
                    String TenCot = GridView1.Rows[i].Cells[3].Text.ToString();
                    String TenBang = GridView1.Rows[i].Cells[4].Text.ToString();
                    temp.Rows.Add(sapXep.SelectedIndex, suDung.SelectedIndex, DieuKien.Text.ToString(), TenCot, TenBang);
                }

                grTemp.DataSource = temp;
                grTemp.DataBind();
            }

            // them cac column dieu kien vao gridview1
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

            // đổ dữ liệu grTemp vào gridview1
            if (grTemp.Rows.Count > 0)
            {
                for (int i = 0; i < GridView1.Rows.Count; i++)
                {
                    DropDownList sapXep = (DropDownList)GridView1.Rows[i].Cells[0].FindControl("DropDownListSort");
                    DropDownList suDung = (DropDownList)GridView1.Rows[i].Cells[1].FindControl("DropDownListUse");
                    TextBox dieuKien = (TextBox)GridView1.Rows[i].Cells[2].FindControl("TextBoxDieuKien");
                    for (int j = 0; j < grTemp.Rows.Count; j++)
                    {
                        if (GridView1.Rows[i].Cells[3].Text.ToString().Equals(grTemp.Rows[j].Cells[3].Text.ToString()) && GridView1.Rows[i].Cells[4].Text.ToString().Equals(grTemp.Rows[j].Cells[4].Text.ToString()))
                        {
                            sapXep.SelectedIndex = int.Parse(grTemp.Rows[j].Cells[0].Text.ToString());
                            suDung.SelectedIndex = int.Parse(grTemp.Rows[j].Cells[1].Text.ToString());
                            String dk = (String)grTemp.Rows[j].Cells[2].Text.ToString();
                            String dkText = HtmlToPlainText(dk);
                            if (dkText.Trim() != "")
                            {
                                dieuKien.Text = dkText;
                            }
                            else
                            {
                                dieuKien.Text = null;
                            }


                        }
                    }

                }
            }

        }


        public String getForeignKey(String object_id_a, String object_id_b)
        {
            List<String> values = new List<string>();
            String query = "exec sp_TimKhoaNgoai " + object_id_a + ", " + object_id_b;
            SqlDataReader sdr = ExecSqlDataReader(query);
            while (sdr.Read())
            {
                values.Add(sdr["table_name"].ToString() + "." + sdr["column_name"].ToString());
            }
            return String.Join("=", values);
        }

        protected void ButtonQuery_Click(object sender, EventArgs e)
        {

            List<String> select = new List<string>();
            List<String> ListUserConditions = new List<string>();
            List<String> groupBy = new List<String>();
            List<String> sortBy = new List<string>();
            List<String> UserCondition = new List<string>();
            List<String> havings = new List<string>();
            Boolean isAggregateFunction = false;
            DropDownList sapXep, suDung;
            String tenBang, tenCot, where = "", query = "SELECT ";
            TextBox dieuKien;

            // lấy danh sách table 
            string tableName = "";
            for (int i = 0; i < listTableQuery.Count; i++)
            {
                tableName += listTableQuery[i].tableName + " ";
            }
            tableName = tableName.Trim().Replace(" ", ", ");

            // duyệt grid view. tạo query
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                sapXep = (DropDownList)GridView1.Rows[i].Cells[0].FindControl("DropDownListSort");
                suDung = (DropDownList)GridView1.Rows[i].Cells[1].FindControl("DropDownListUse");
                dieuKien = (TextBox)GridView1.Rows[i].Cells[2].FindControl("TextBoxDieuKien");
                tenCot = GridView1.Rows[i].Cells[3].Text.ToString();
                tenBang = GridView1.Rows[i].Cells[4].Text.ToString();

                // sap xep
                if (!sapXep.Text.Equals("sort"))
                {
                    sortBy.Add(tenBang + "." + tenCot + " " + sapXep.SelectedValue.ToString());
                }

                // suDung
                if (!suDung.Text.Equals("SELECT") && !suDung.Text.Equals("GROUP BY"))
                {
                    select.Add(suDung.Text + "(" + tenCot + ") ");
                    isAggregateFunction = true;
                    if (dieuKien.Text.ToString().Trim() != "")
                    {
                        havings.Add(dieuKien.Text.ToString());
                    }


                }
                else if (suDung.Text.Equals("GROUP BY"))
                {
                    groupBy.Add(tenBang + "." + tenCot);
                    select.Add(tenBang + "." + tenCot);
                }
                else
                {
                    select.Add(tenBang + "." + tenCot);
                }

                // điều kiện người dùng
                if (dieuKien.Text.ToString() != "" && suDung.Text.Equals("SELECT"))
                {
                    UserCondition.Add(dieuKien.Text.ToString());
                }
            }

            // điều kiện khoá ngoại
            if (listTableQuery.Count > 1)
            {
                String table_i_id, table_j_id, relationship;
                for (int i = 0; i < listTableQuery.Count - 1; i++)
                {
                    for (int j = i + 1; j < listTableQuery.Count; j++)
                    {
                        table_i_id = listTableQuery[i].Table_Object_id;
                        table_j_id = listTableQuery[j].Table_Object_id;
                        relationship = getForeignKey(table_i_id, table_j_id);
                        if (!relationship.Equals(""))
                        {
                            ListUserConditions.Add(relationship);
                        }

                    }
                }

            }

            // duyet lai grid view neu co các hàm tổng hơp => lấy các column không trong hàm để group
            if (isAggregateFunction == true)
            {
                for (int i = 0; i < GridView1.Rows.Count; i++)
                {
                    suDung = (DropDownList)GridView1.Rows[i].Cells[1].FindControl("DropDownListUse");
                    tenBang = GridView1.Rows[i].Cells[4].Text.ToString();
                    tenCot = GridView1.Rows[i].Cells[3].Text.ToString();
                    if (suDung.Text.Equals("SELECT"))
                    {
                        groupBy.Add(tenBang + "." + tenCot);
                    }

                }
            }

            // gộp tạo query
            if (ListUserConditions.Count != 0)
            {
                where += " WHERE " + string.Join(" AND ", ListUserConditions);
            }

            query += String.Join(", ", select) + " FROM " + String.Join(" AND ", tableName) + where;
            if (UserCondition.Count > 0)
            {
                query += " AND " + String.Join(" AND ", UserCondition);
            }
            if (groupBy.Count > 0)
            {
                query += " GROUP BY " + String.Join(", ", groupBy);
                if (havings.Count > 0)
                {
                    query += " HAVING " + String.Join(" AND ", havings);
                }
            }
            if (sortBy.Count > 0)
            {
                query += " ORDER BY " + String.Join(",", sortBy);
            }
            TextBox1.Text = query;

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