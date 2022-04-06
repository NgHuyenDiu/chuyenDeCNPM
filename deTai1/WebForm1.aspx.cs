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
                this.GetItemCheckboxListTable();
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
        public List<String> getColumnChecked()
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
            return temp;
        }

        public void checkedListColumn(List<String> temp)
        {
            for (int i = 0; i < temp.Count; i++)
            {
                ListItem listItem = this.CheckBoxListColumn.Items.FindByText(temp[i].ToString());
                if (listItem != null) listItem.Selected = true;
            }
        }

        public void GetItemCheckboxListTableChecked()
        {
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
        }

        public void updateConditionTableWhenTableChange()
        {
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                Boolean has = false;
                String TenBang = GridView1.Rows[i].Cells[4].Text.ToString();
                for (int j = 0; j < listTableQuery.Count; j++)
                {
                    if (TenBang.Equals(listTableQuery[j].tableName.ToString()))
                    {
                        has = true;
                        break;
                    }
                }
                if (has == false)
                {

                    DataTable dttemp = getTableConditionToTemp();
                    GridView grTemp = new GridView();
                    grTemp.DataSource = dttemp;
                    grTemp.DataBind();

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Tên Cột", Type.GetType("System.String"));
                    dt.Columns.Add("Tên Bảng", Type.GetType("System.String"));
                    for (int t = 0; t < GridView1.Rows.Count; t++)
                    {
                        String TenCot = GridView1.Rows[t].Cells[3].Text.ToString();
                        String Tb = GridView1.Rows[t].Cells[4].Text.ToString();
                        dt.Rows.Add(TenCot, Tb);
                    }
                    dt.Rows[i].Delete();
                    GridView1.DataSource = dt;
                    GridView1.DataBind();

                    copyConditionTempToConditionNow(grTemp);
                    i = i - 1;
                }
            }
        }

        protected void CheckBoxListTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            // lay cac column dang duoc check
            List<String> temp = getColumnChecked();
            // xoa checkboxlist hien tai
            CheckBoxListColumn.Items.Clear();
            // lay danh sach table được checked
            GetItemCheckboxListTableChecked();
            // duyet danh sâch table duoc chon, lay danh sach column
            for (int i = 0; i < listTableQuery.Count; i++)
            {
                GetItemCheckboxListColumn(listTableQuery[i].tableName);
            }
            // ckeched lai nhưng item column da duoc chon truoc do
            checkedListColumn(temp);
            // update table condition
            updateConditionTableWhenTableChange();
        }

        private void GetItemCheckboxListTable()
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

        private void GetItemCheckboxListColumn(String tableName)
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
            txtQuery.Text = "";
            GridView1.Controls.Clear();
            this.GetItemCheckboxListTable();
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

        public DataTable getTableConditionToTemp()
        {
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
                    DropDownList sort = (DropDownList)GridView1.Rows[i].Cells[0].FindControl("DropDownListSort");
                    DropDownList use = (DropDownList)GridView1.Rows[i].Cells[1].FindControl("DropDownListUse");
                    TextBox DieuKien = (TextBox)GridView1.Rows[i].Cells[2].FindControl("TextBoxDieuKien");
                    String TenCot = GridView1.Rows[i].Cells[3].Text.ToString();
                    String TenBang = GridView1.Rows[i].Cells[4].Text.ToString();
                    temp.Rows.Add(sort.SelectedIndex, use.SelectedIndex, DieuKien.Text.ToString(), TenCot, TenBang);
                }
                return temp;
            }
            return null;
        }

        public void copyConditionTempToConditionNow(GridView grTemp)
        {
            if (grTemp.Rows.Count > 0)
            {
                for (int i = 0; i < GridView1.Rows.Count; i++)
                {
                    DropDownList sort = (DropDownList)GridView1.Rows[i].Cells[0].FindControl("DropDownListSort");
                    DropDownList use = (DropDownList)GridView1.Rows[i].Cells[1].FindControl("DropDownListUse");
                    TextBox dieuKien = (TextBox)GridView1.Rows[i].Cells[2].FindControl("TextBoxDieuKien");
                    for (int j = 0; j < grTemp.Rows.Count; j++)
                    {
                        if (GridView1.Rows[i].Cells[3].Text.ToString().Equals(grTemp.Rows[j].Cells[3].Text.ToString()) && GridView1.Rows[i].Cells[4].Text.ToString().Equals(grTemp.Rows[j].Cells[4].Text.ToString()))
                        {
                            sort.SelectedIndex = int.Parse(grTemp.Rows[j].Cells[0].Text.ToString());
                            use.SelectedIndex = int.Parse(grTemp.Rows[j].Cells[1].Text.ToString());
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

        public void updateColumnConditionWhenColumnChange()
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

        protected void CheckBoxListColumn_SelectedIndexChanged(object sender, EventArgs e)
        {
            // chuyen datatable to temp de luu thong tin dieu kien
            GridView grTemp = new GridView();
            grTemp.DataSource = getTableConditionToTemp();
            grTemp.DataBind();
            // them cac column dieu kien vao gridview1
            updateColumnConditionWhenColumnChange();
            // đổ dữ liệu grTemp vào gridview1
            copyConditionTempToConditionNow(grTemp);
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

        public String getTableNameInQuery()
        {
            string tableName = "";
            for (int i = 0; i < listTableQuery.Count; i++)
            {
                tableName += listTableQuery[i].tableName + " ";
            }
            tableName = tableName.Trim().Replace(" ", ", ");
            return tableName;
        }

        public List<String> getListConditionFoignKey()
        {
            List<String> ListConditions = new List<string>();
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
                            ListConditions.Add(relationship);
                        }

                    }
                }

            }
            return ListConditions;
        }

        protected void ButtonQuery_Click(object sender, EventArgs e)
        {

            List<String> ListColumnInQuery = new List<string>();
            List<String> ListConditions = new List<string>();
            List<String> groupBy = new List<String>();
            List<String> sortBy = new List<string>();
            List<String> UserCondition = new List<string>();
            List<String> havings = new List<string>();
            Boolean isAggregateFunction = false;
            DropDownList sort, use;
            String tenBang, tenCot, where = "", query = "SELECT ";
            TextBox dieuKien;

            // lấy danh sách table 
            string tableName =getTableNameInQuery();
          

            // duyệt grid view. tạo query
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                sort = (DropDownList)GridView1.Rows[i].Cells[0].FindControl("DropDownListSort");
                use = (DropDownList)GridView1.Rows[i].Cells[1].FindControl("DropDownListUse");
                dieuKien = (TextBox)GridView1.Rows[i].Cells[2].FindControl("TextBoxDieuKien");
                tenCot = GridView1.Rows[i].Cells[3].Text.ToString();
                tenBang = GridView1.Rows[i].Cells[4].Text.ToString();

                // sap xep
                if (!sort.Text.Equals("sort"))
                {
                    sortBy.Add(tenBang + "." + tenCot + " " + sort.SelectedValue.ToString());
                }

                // use
                if (!use.Text.Equals("SELECT") && !use.Text.Equals("GROUP BY"))
                {
                    ListColumnInQuery.Add(use.Text + "(" + tenCot + ") ");
                    isAggregateFunction = true;
                    if (dieuKien.Text.ToString().Trim() != "")
                    {
                        havings.Add(dieuKien.Text.ToString());
                    }

                }
                else if (use.Text.Equals("GROUP BY"))
                {
                    groupBy.Add(tenBang + "." + tenCot);
                    ListColumnInQuery.Add(tenBang + "." + tenCot);
                }
                else
                {
                    ListColumnInQuery.Add(tenBang + "." + tenCot);
                }

                // điều kiện người dùng
                if (dieuKien.Text.ToString() != "" && use.Text.Equals("SELECT"))
                {
                    UserCondition.Add(dieuKien.Text.ToString());
                }
            }

            //lay danh sach lien ket khoa ngoai
            ListConditions = getListConditionFoignKey();

            // duyet lai grid view neu co các hàm tổng hơp => lấy các column không trong hàm để group
            if (isAggregateFunction == true)
            {
                for (int i = 0; i < GridView1.Rows.Count; i++)
                {
                    use = (DropDownList)GridView1.Rows[i].Cells[1].FindControl("DropDownListUse");
                    tenBang = GridView1.Rows[i].Cells[4].Text.ToString();
                    tenCot = GridView1.Rows[i].Cells[3].Text.ToString();
                    if (use.Text.Equals("SELECT"))
                    {
                        groupBy.Add(tenBang + "." + tenCot);
                    }

                }
            }

            // gộp tạo query
            if (ListConditions.Count != 0)
            {
                where += " WHERE " + string.Join(" AND ", ListConditions);
            }

            query += String.Join(", ", ListColumnInQuery) + " FROM " + String.Join(" AND ", tableName) + where;
            if (UserCondition.Count > 0 && ListConditions.Count != 0)
            {
                query += " AND " + String.Join(" AND ", UserCondition);
            }
            else if (UserCondition.Count > 0 && ListConditions.Count == 0)
            {
                query += " WHERE " + String.Join(" AND ", UserCondition);
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
            txtQuery.Text = query;

        }

        protected void btnReport_Click(object sender, EventArgs e)
        {
            String query = txtQuery.Text;
            String title = TextBoxNhapTieuDe.Text;

            Session["query"] = query;
            Session["title"] = title;
            Response.Redirect("WebForm2.aspx");
            Server.Execute("WebForm2.aspx");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            txtQuery.Text = "";
        }
    }

}