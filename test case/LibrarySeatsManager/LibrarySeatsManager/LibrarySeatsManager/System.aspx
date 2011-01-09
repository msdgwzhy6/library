<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="System.aspx.cs" Inherits="LibrarySeatsManager.WebForm1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>无标题页</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Panel ID="Panel4" runat="server">
        <asp:Label ID="Label3" runat="server" Font-Size="XX-Large" 
    Text="图书馆座位管理系统"></asp:Label>
    </asp:Panel>
</body>
</html>
    <asp:Panel ID="Panel2" runat="server">
        <html xmlns="http://www.w3.org/1999/xhtml" >
        <body>
            <br />
            <asp:Label ID="Label1" runat="server" Text="用户名"></asp:Label>
            &nbsp;&nbsp;
            <asp:TextBox ID="TextBox1" runat="server" ></asp:TextBox>
        </body>
        </html>
        <br />
        <br />
        &nbsp;
        <asp:Label ID="Label2" runat="server" Text="密码"></asp:Label>
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="TextBox2" runat="server" ></asp:TextBox>
        <br />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <br />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button2" runat="server" Text="登录" 
    onclick="Button2_Click" />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button1" runat="server" Text="注册" 
    onclick="Button1_Click" />
    </asp:Panel>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Panel 
        ID="Panel1" runat="server" Visible="False">
        <asp:Button ID="Button9" runat="server" Text="座位查询" onclick="Button9_Click" />
        <asp:Button ID="Button10" runat="server" Text="信息刷新" onclick="Button10_Click" />
        <asp:Button ID="Button11" runat="server" onclick="Button11_Click" Text="授权新管理员" 
            Visible="False" />
        <asp:Button ID="Button12" runat="server" Text="清除占位" Visible="False" 
            onclick="Button12_Click" />
        <asp:Button ID="Button13" runat="server" onclick="Button13_Click" Text="退出系统" />
        <br />
        <asp:GridView ID="GridView1" runat="server" DataSourceID="ObjectDataSource1" 
            onselectedindexchanged="GridView1_SelectedIndexChanged">
        </asp:GridView>
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" 
            SelectMethod="QuerySeatsStates" TypeName="LibrarySeatsManager.SeatStateManager">
        </asp:ObjectDataSource>
    </asp:Panel>
    <asp:Panel ID="Panel3" runat="server" Visible="False">
        <br />
        <asp:Label ID="Label5" runat="server" Text="用户名"></asp:Label>
        &nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="TextBox4" runat="server"></asp:TextBox>
        <br />
        <br />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button15" runat="server" onclick="Button15_Click" Text="确认" />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button16" runat="server" onclick="Button16_Click" Text="取消" />
    </asp:Panel>
</form>

