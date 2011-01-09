<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="LibrarySeatsManager.HomePage" Theme="MyTheme"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>图书馆座位管理系统</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Label ID="Label3" runat="server" Font-Size="XX-Large" 
    Text="图书馆座位管理系统"></asp:Label>
        <br />
    
        <br />
    
        <asp:Panel 
        ID="Panel1" runat="server">
        <asp:Button ID="Query" runat="server" Text="座位查询" onclick="Query_Click" 
                Font-Size="Large" Height="30px" Width="90px"/>
        <asp:Button ID="Authorize" runat="server" Text="授权新管理员" onclick="Authorize_Click" 
                Font-Size="Large" Height="30px" Width="130px" Visible="False" />
        <asp:Button ID="Clear" runat="server" Text="清除占位" Font-Size="Large" Height="30px" 
                onclick="Clear_Click" Width="90px" Visible="False" />
        <asp:Button ID="Exit" runat="server" Text="退出系统" onclick="Exit_Click" 
                Font-Size="Large" Height="30px" Width="90px" />
            <br />
            <asp:GridView ID="SeatGrid" runat="server" BackColor="White" 
                BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
                Width="100%" AllowPaging="True" AutoGenerateColumns="False">
                <RowStyle ForeColor="#000066" />
                <Columns>
                    <asp:BoundField DataField="Row" HeaderText="座位行" />
                    <asp:BoundField DataField="Col" HeaderText="座位列" />
                    <asp:BoundField DataField="States" HeaderText="座位状态" />
                </Columns>
                <FooterStyle BackColor="White" ForeColor="#000066" />
                <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
            </asp:GridView>
        <br />
    </asp:Panel>
    </div>
    </form>
</body>
</html>
