<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="LibrarySeatsManager.RegisterPage" Theme="MyTheme" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>注册新用户</title>
    <style type="text/css">

        .style1
        {
            width: 863px;
        }
        .style2
        {
            width: 926px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Label ID="Label4" runat="server" Font-Size="XX-Large" Text="图书馆座位管理系统"></asp:Label>
        <br />
        <br />
    
        <asp:Label ID="Label3" runat="server" Font-Size="X-Large" Text="注册新用户"></asp:Label>
        <br />
        <br />
    
        <table style="width:280px" align="center">
            <tr align="center">
                <td class="style1">
                    <asp:Label ID="Label1" runat="server" Text="用户名"></asp:Label>
                    </td>
                <td class="style2">
                    <asp:TextBox ID="UsernameText" runat="server"
                        Width="150px" Height="30px"></asp:TextBox>
                </td>
            </tr>
            <tr align="center">
                <td class="style1">
                    <asp:Label ID="Label2" runat="server" Text="密码"></asp:Label>
                    </td>
                <td class="style2">
                    <asp:TextBox ID="PasswordText" runat="server" Width="150px" Height="30px" 
                        TextMode="Password"></asp:TextBox>
                </td>
            </tr>
            <tr align="center">
                <td class="style1">
                   </td>
                <td class="style2">
                    <asp:Button ID="Register" runat="server" Text="注册" OnClick="Register_Click" 
                        Width="60px" Height="30px" Font-Size="Medium" />
                    <asp:Button ID="Cancel" runat="server" Text="返回" onclick="Cancel_Click" 
                        Height="30px" Width="60px" Font-Size="Medium" />
                </td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
</html>
