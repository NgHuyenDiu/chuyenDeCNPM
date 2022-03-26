<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="deTai1.WebForm11" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form2" runat="server">
        <div id="header">
            <asp:Label ID="LabelTitle" runat="server" style="color: #161b4e;font-size: 24px;line-height: 40px;" Text="Nhập tiêu đề báo cáo: "></asp:Label>
            <asp:TextBox ID="TextBoxNhapTieuDe" runat="server" Height="24px" Width="833px"></asp:TextBox>
            <asp:Button ID="btnReport" runat="server" Height="40px" OnClick="btnReport_Click" Text="Tạo REPORT" />
        </div>
        <div id="main">
            <div id="table-content" style="width: 1532px; display:flex; justify-content:space-around; height: 319px;">
                <asp:Panel ID="PanelChonBang" runat="server" BackColor="#78cbdb" ForeColor="#0b1215" style="margin-left: 79px" Width="399px">
                    <asp:Label ID="LabelChonBang" runat="server" style="color: #944646;font-size: 20px;" Text="Chọn TABLE cần in báo cáo: "></asp:Label>
                    <br />
                    <asp:CheckBoxList ID="CheckBoxListTable" runat="server" OnSelectedIndexChanged="CheckBoxListTable_SelectedIndexChanged" Width="381px">
                    </asp:CheckBoxList>
                </asp:Panel>
                <asp:Panel ID="PanelChonCot" runat="server" BackColor="#99ccff" ForeColor="Maroon" style=" margin-top: 0px; margin-right: 116px;" Width="913px">
                    <asp:Button ID="ButtonClearColumn" runat="server" Height="38px" OnClick="ButtonClearColumn_Click" style="margin-left: 665px" Text="CLEAR All COLUMN" Width="245px" />
                        <br />
                        <asp:Label ID="LabelChonCot" runat="server" style="color: #944646;font-size: 20px;" Text="Chọn COLUMN cần in báo cáo: "></asp:Label>
                        <br />
                        <br />
                        <asp:CheckBoxList ID="CheckBoxListColumn" runat="server" AutoPostBack="True" OnSelectedIndexChanged="CheckBoxListColumn_SelectedIndexChanged" RepeatColumns="5" RepeatLayout="Table" TextAlign="Right" Width="909px">
                    </asp:CheckBoxList>
                        <br />
                        <br />
                    </asp:Panel>
            </div>
            <asp:Panel ID="Panel1" runat="server" Height="166px" style="margin-left: 79px">
                <asp:Button ID="ButtonQuery" runat="server" Height="37px" OnClick="ButtonQuery_Click" Text="Tạo QUERY" />
                        <br />
                        <asp:TextBox ID="TextBox1" runat="server" Rows="5" TextMode="MultiLine" Width="1111px"></asp:TextBox>
            </asp:Panel>
            <asp:Panel ID="PanelGridViewColumn" runat="server" BackColor="White" ForeColor="white" Height="367px" style=" margin-top: 25px; margin-left: 84px; margin-right:100px">
                            <br />
                            <br />
                            <asp:GridView ID="GridView1" runat="server" BackColor="white" BorderColor="#CCCCCC" BorderWidth="1px" CellPadding="3" Height="16px" style="margin-top: 0px" Width="1105px">
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:Label ID="Label1" runat="server" Text="Sắp xếp"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:DropDownList ID="DropDownList2" runat="server" Width="205px">
                                                <asp:ListItem Value="sort">Chọn</asp:ListItem>
                                                <asp:ListItem Value="ASC">Sắp xếp tăng dần</asp:ListItem>
                                                <asp:ListItem Value="DESC">Sắp xếp giảm dần</asp:ListItem>
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="State">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label2" runat="server" Text="Sử dụng"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:DropDownList ID="DropDownList1" runat="server" Width="213px">
                                                <asp:ListItem Text="SELECT" Value="SELECT"></asp:ListItem>
                                                <asp:ListItem Text="SUM" Value="SUM"></asp:ListItem>
                                                <asp:ListItem Text="COUNT" Value="COUNT"></asp:ListItem>
                                                <asp:ListItem Text="MIN" Value="MIN"></asp:ListItem>
                                                <asp:ListItem Text="MAX" Value="MAX"></asp:ListItem>
                                                <asp:ListItem Text="AVG" Value="AVG"></asp:ListItem>
                                                <asp:ListItem>GROUP BY</asp:ListItem>
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Điều Kiện">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label3" runat="server" Text="Điều kiện"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:TextBox ID="TextBoxDieuKien" runat="server" Width="247px"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <FooterStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                                <HeaderStyle BackColor="#006699" Font-Bold="true" ForeColor="White" />
                                <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                                <RowStyle ForeColor="#000066" />
                                <SelectedRowStyle BackColor="#006699" Font-Bold="true" ForeColor="White" />
                                <SortedAscendingCellStyle BackColor="#000066" />
                                <SortedAscendingHeaderStyle BackColor="#000066" />
                                <SortedAscendingCellStyle BackColor="#007DBB" />
                                <SortedDescendingCellStyle BackColor="#CAC9C9" />
                                <SortedDescendingHeaderStyle BackColor="#00547E" />
                </asp:GridView>
            </asp:Panel>
        </div>
        <div id="query-content">
        </div>
    </form>
   
</body>
</html>
